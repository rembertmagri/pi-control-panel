namespace PiControlPanel.Application.SecureShell
{
    /// <summary>
    /// Contains the web socket data types.
    /// </summary>
    public enum WebSocketDataType
    {
        /// <summary>
        /// Token type.
        /// </summary>
        Token,

        /// <summary>
        /// Dimensions type.
        /// </summary>
        Dimensions,

        /// <summary>
        /// Command standard input type.
        /// </summary>
        StandardInput,

        /// <summary>
        /// Command standard output type.
        /// </summary>
        StandardOutput,

        /// <summary>
        /// Command standard error type.
        /// </summary>
        StandardError
    }
}
