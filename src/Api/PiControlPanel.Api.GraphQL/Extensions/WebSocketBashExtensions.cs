namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System;
    using System.Net;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Contains extension methods for Web Socket connection handling.
    /// </summary>
    public static class WebSocketBashExtensions
    {
        /// <summary>
        /// Adds custom Web Socket middleware to the application that forwards
        /// commands to the shell through SSH.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public static void UseWebSocketsToSsh(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if ("/shell".Equals(context.Request.Path))
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    await next();
                }
            });
        }

        private static async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
