namespace PiControlPanel.Api.GraphQL.Options
{
    using System.ComponentModel.DataAnnotations;
    using global::GraphQL.Server;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Server.Kestrel.Core;

    /// <summary>
    /// All options for the application.
    /// </summary>
    public class ApplicationOptions
    {
        /// <summary>
        /// Gets or sets the possible CachingProfile for the application.
        /// </summary>
        [Required]
        public CacheProfileOptions CacheProfiles { get; set; }

        /// <summary>
        /// Gets or sets the possible CompressionOptions for the application.
        /// </summary>
        [Required]
        public CompressionOptions Compression { get; set; }

        /// <summary>
        /// Gets or sets the possible ForwardedHeadersOptions for the application.
        /// </summary>
        [Required]
        public ForwardedHeadersOptions ForwardedHeaders { get; set; }

        /// <summary>
        /// Gets or sets the possible GraphQLOptions for the application.
        /// </summary>
        [Required]
        public GraphQLOptions GraphQL { get; set; }

        /// <summary>
        /// Gets or sets the possible KestrelServerOptions (Web Server) for the application.
        /// </summary>
        [Required]
        public KestrelServerOptions Kestrel { get; set; }
    }
}
