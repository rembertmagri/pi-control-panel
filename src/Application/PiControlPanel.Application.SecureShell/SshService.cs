namespace PiControlPanel.Application.SecureShell
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;

    /// <inheritdoc/>
    public class SshService : ISshService
    {
        private readonly ISecurityService securityService;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SshService"/> class.
        /// </summary>
        /// <param name="securityService">The application layer SecurityService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public SshService(
            ISecurityService securityService,
            ILogger logger)
        {
            this.securityService = securityService;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task BindAsync(WebSocket webSocket)
        {
            this.logger.Debug($"Application layer -> SshService -> BindAsync");

            var authenticated = await this.HandleAuthenticationHandshake(webSocket);
            if (!authenticated)
            {
                this.logger.Warn($"Authentication handshake failed");
                return;
            }

            var buffer = new byte[1024 * 4];
            this.logger.Info("SSH connection open");
            var webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!webSocketReceiveResult.CloseStatus.HasValue)
            {
                this.logger.Trace($"Received {webSocketReceiveResult.Count} bytes");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, webSocketReceiveResult.Count), webSocketReceiveResult.MessageType, webSocketReceiveResult.EndOfMessage, CancellationToken.None);
                webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(webSocketReceiveResult.CloseStatus.Value, webSocketReceiveResult.CloseStatusDescription, CancellationToken.None);
            this.logger.Info($"SSH connection closed with status {webSocketReceiveResult.CloseStatus.Value}");
        }

        private async Task<bool> HandleAuthenticationHandshake(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (webSocketReceiveResult.CloseStatus.HasValue)
            {
                this.logger.Warn($"SSH connection closed before authentication with status {webSocketReceiveResult.CloseStatus.Value}");
                return false;
            }

            var jsonResult = Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count);
            var authenticated = await this.AuthenticateAsync(jsonResult);

            if (!authenticated)
            {
                this.logger.Warn($"Closing socket, user could not be authenticated");
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Could not authenticate", CancellationToken.None);
            }

            return authenticated;
        }

        private async Task<bool> AuthenticateAsync(string result)
        {
            this.logger.Trace($"Received string {result}");
            var webSocketData = JsonConvert.DeserializeObject<WebSocketData>(result);
            if (!WebSocketDataType.Token.Equals(webSocketData?.Type))
            {
                this.logger.Warn($"Invalid data type {webSocketData?.Type}");
                return false;
            }

            var token = webSocketData?.Payload;
            if (string.IsNullOrEmpty(token))
            {
                this.logger.Warn($"Empty data payload, the JWT is required");
                return false;
            }

            var userAccount = await this.securityService.GetUserAccountAsync(token);
            return userAccount?.Username != null && userAccount?.Password != null;
        }
    }
}
