namespace PiControlPanel.Domain.Contracts.Application
{
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    /// <summary>
    /// Application layer service that contains logic to connect to terminal via SSH.
    /// </summary>
    public interface ISshService
    {
        /// <summary>
        /// Binds the web scoket to the shell via SSH to run commands.
        /// </summary>
        /// <param name="webSocket">The reference to the web socket from the application middleware.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RunSsh(WebSocket webSocket);
    }
}
