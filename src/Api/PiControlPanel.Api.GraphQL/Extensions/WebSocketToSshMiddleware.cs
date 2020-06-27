namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Net;
    using System.Net.WebSockets;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using PiControlPanel.Domain.Contracts.Application;

    /// <summary>
    /// Implements web socket to SSH middleware.
    /// </summary>
    public class WebSocketToSshMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketToSshMiddleware"/> class.
        /// </summary>
        /// <param name="next">A function that can process an HTTP request.</param>
        public WebSocketToSshMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Contains the middleware logic that handles HTTP requests.
        /// </summary>
        /// <param name="context">Encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <param name="sshService">The injected instance of SshService.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, ISshService sshService)
        {
            if ("/shell".Equals(context.Request.Path))
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await sshService.BindAsync(webSocket);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await this.next(context);
            }
        }
    }
}
