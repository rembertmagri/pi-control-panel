namespace PiControlPanel.Application.SecureShell
{
    /// <summary>
    /// The web socket data.
    /// </summary>
    public class WebSocketData
    {
        /// <summary>
        /// Gets or sets the CPU model.
        /// </summary>
        public WebSocketDataType Type { get; set; }

        /// <summary>
        /// Gets or sets the CPU model.
        /// </summary>
        public string Payload { get; set; }
    }
}
