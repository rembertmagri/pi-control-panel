namespace PiControlPanel.Application.SecureShell
{
    using System;
    using System.IO;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Authentication;
    using Renci.SshNet;

    /// <inheritdoc/>
    public class SshService : ISshService
    {
        private static readonly int BUFFERSIZE = 8 * 1024;
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
        public async Task BindAsync(WebSocket webSocket, int sshPort)
        {
            this.logger.Debug($"Application layer -> SshService -> BindAsync");

            var authenticatedAccount = await this.HandleAuthenticationHandshake(webSocket);
            if (authenticatedAccount == null)
            {
                this.logger.Error($"Authentication handshake failed, closing socket");
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Could not authenticate", CancellationToken.None);
                return;
            }

            var dimensions = await this.HandleTerminalDimensionsHandshake(webSocket);
            if (dimensions == null)
            {
                this.logger.Error($"Terminal dimensions handshake failed, closing socket");
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Could not get terminal dimensions", CancellationToken.None);
                return;
            }

            using var client = new SshClient("localhost", sshPort, authenticatedAccount.Username, authenticatedAccount.Password);
            client.Connect();
            this.logger.Info($"SSH connection open, creating terminal with {dimensions.Rows} rows and {dimensions.Columns} columns");

            using var shellStream = client.CreateShellStream("xterm", dimensions.Rows, dimensions.Columns, 0, 0, BUFFERSIZE);
            this.KeepSendingWebSocketData(webSocket, shellStream);
            string webSocketRawData;
            do
            {
                webSocketRawData = await this.ReceiveWebSocketRawData(webSocket);
                shellStream.Write(webSocketRawData);
            }
            while (webSocketRawData != null);

            client.Disconnect();
        }

        private async Task<UserAccount> HandleAuthenticationHandshake(WebSocket webSocket)
        {
            var webSocketData = await this.ReceiveWebSocketData(webSocket);

            if (!WebSocketDataType.Token.Equals(webSocketData?.Type))
            {
                this.logger.Error($"Invalid data type {webSocketData?.Type}, expected {WebSocketDataType.Token}");
                return null;
            }

            var token = webSocketData?.Payload;
            if (string.IsNullOrEmpty(token))
            {
                this.logger.Error($"Empty data payload, the JWT is required");
                return null;
            }

            return await this.securityService.GetUserAccountAsync(token);
        }

        private async Task<TerminalDimensions> HandleTerminalDimensionsHandshake(WebSocket webSocket)
        {
            var webSocketData = await this.ReceiveWebSocketData(webSocket);

            if (!WebSocketDataType.Dimensions.Equals(webSocketData?.Type))
            {
                this.logger.Error($"Invalid data type {webSocketData?.Type}, expected {WebSocketDataType.Dimensions}");
                return null;
            }

            var payload = webSocketData?.Payload;
            if (string.IsNullOrEmpty(payload))
            {
                this.logger.Error($"Empty data payload, the dimensions are required");
                return null;
            }

            var dimensions = payload.Split('|');
            if (dimensions.Length != 2 || !uint.TryParse(dimensions[0], out var rows) || !uint.TryParse(dimensions[1], out var columns))
            {
                this.logger.Error($"Could not parse terminal dimensions: {payload}");
                return null;
            }

            return new TerminalDimensions()
            {
                Rows = rows,
                Columns = columns
            };
        }

        private async Task<WebSocketData> ReceiveWebSocketData(WebSocket webSocket)
        {
            var webSocketRawData = await this.ReceiveWebSocketRawData(webSocket);
            this.logger.Trace($"Received message: {webSocketRawData}");
            return JsonConvert.DeserializeObject<WebSocketData>(webSocketRawData);
        }

        private async Task<string> ReceiveWebSocketRawData(WebSocket webSocket)
        {
            var buffer = new byte[BUFFERSIZE];
            WebSocketReceiveResult result;

            using var stream = new MemoryStream();
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.CloseStatus.HasValue)
                {
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    this.logger.Warn($"SSH connection closed with status {result.CloseStatus.Value}");
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

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private void KeepSendingWebSocketData(WebSocket webSocket, ShellStream shellStream)
        {
            new Thread(async () =>
            {
                while (!webSocket.CloseStatus.HasValue && shellStream.CanRead)
                {
                    var buffer = new byte[BUFFERSIZE];
                    using var stream = new MemoryStream();
                    var bytesRead = 0;
                    var i = 0;
                    do
                    {
                        bytesRead = await shellStream.ReadAsync(buffer);
                        stream.Write(buffer, i, bytesRead);
                        i += bytesRead;
                    }
                    while (bytesRead > 0); // end of message not reached

                    var webSocketRawData = stream.ToArray();
                    if (webSocketRawData.Length > 0)
                    {
                        await webSocket.SendAsync(new ArraySegment<byte>(webSocketRawData, 0, webSocketRawData.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        Thread.Sleep(300); // if nothing to send, sleep for some time before checking again
                    }
                }

                if (webSocket.CloseStatus.HasValue)
                {
                    this.logger.Trace($"Socket closed, stop getting shell response");
                }

                if (shellStream.CanRead)
                {
                    this.logger.Trace($"Can't read from shell, stop getting shell response");
                }
            }).Start();
        }
    }
}
