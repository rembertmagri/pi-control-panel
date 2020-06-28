namespace PiControlPanel.Application.SecureShell
{
    using System;
    using System.IO;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
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

            this.logger.Info("SSH connection open");
            var webSocketData = await this.ReceiveWebSocketData(webSocket);

            while (webSocketData != null)
            {
                await this.ExecuteCommandAsync(webSocket, webSocketData);
                webSocketData = await this.ReceiveWebSocketData(webSocket);
            }
        }

        private async Task<bool> HandleAuthenticationHandshake(WebSocket webSocket)
        {
            var webSocketData = await this.ReceiveWebSocketData(webSocket);

            var authenticated = await this.AuthenticateAsync(webSocketData);
            if (!authenticated)
            {
                this.logger.Warn($"Closing socket, user could not be authenticated");
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Could not authenticate", CancellationToken.None);
            }

            return authenticated;
        }

        private async Task<WebSocketData> ReceiveWebSocketData(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;
            var resultString = string.Empty;

            using (var stream = new MemoryStream())
            {
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.CloseStatus.HasValue)
                    {
                        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                        this.logger.Info($"SSH connection closed with status {result.CloseStatus.Value}");
                        return null;
                    }

                    if (result.Count == 0)
                    {
                        this.logger.Trace($"Socket received zero bytes with state {webSocket.State}");
                        break;
                    }

                    stream.Write(buffer, 0, result.Count);
                }
                while (!result.EndOfMessage); // check end of message mark

                resultString = Encoding.UTF8.GetString(stream.ToArray());
            }

            this.logger.Trace($"Received message: {resultString}");
            return JsonConvert.DeserializeObject<WebSocketData>(resultString);
        }

        private async Task<bool> AuthenticateAsync(WebSocketData webSocketData)
        {
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

        private async Task ExecuteCommandAsync(WebSocket webSocket, WebSocketData webSocketData)
        {
            if (!WebSocketDataType.StandardInput.Equals(webSocketData?.Type))
            {
                this.logger.Warn($"Invalid data type {webSocketData?.Type}");
                return;
            }

            var command = webSocketData?.Payload;
            if (string.IsNullOrEmpty(command))
            {
                this.logger.Warn($"Empty data payload, the command is required");
                return;
            }

            var standardOutput = $"TODO: run command {command}";
            var result = new WebSocketData()
            {
                Type = WebSocketDataType.StandardOutput,
                Payload = standardOutput
            };

            var resultString = JsonConvert.SerializeObject(
                result,
                Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            var byteArray = Encoding.UTF8.GetBytes(resultString);
            await webSocket.SendAsync(new ArraySegment<byte>(byteArray, 0, byteArray.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
