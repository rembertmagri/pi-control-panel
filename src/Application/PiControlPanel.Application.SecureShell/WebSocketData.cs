namespace PiControlPanel.Application.SecureShell
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The web socket data.
    /// </summary>
    public class WebSocketData
    {
        /// <summary>
        /// Gets or sets the web socket data type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public WebSocketDataType Type { get; set; }

        /// <summary>
        /// Gets or sets the web socket data payload.
        /// </summary>
        public string Payload { get; set; }
    }
}
