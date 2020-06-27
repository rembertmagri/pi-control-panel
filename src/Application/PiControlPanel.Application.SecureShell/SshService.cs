namespace PiControlPanel.Application.SecureShell
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;

    /// <inheritdoc/>
    public class SshService : ISshService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SshService"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger instance.</param>
        public SshService(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the NLog logger instance.
        /// </summary>
        private ILogger Logger { get; }

        /// <inheritdoc/>
        public async Task BindAsync(WebSocket webSocket)
        {
            this.Logger.Debug($"Application layer -> SshService -> BindAsync");
            this.Logger.Info("SSH connection opened");
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                this.Logger.Trace($"Received {result.Count} bytes");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            this.Logger.Info("SSH connection closed");
        }
    }
}
