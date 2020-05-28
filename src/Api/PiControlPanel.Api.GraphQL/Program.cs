namespace PiControlPanel.Api.GraphQL
{
    using System;
    using System.IO;
    using System.Reflection;
    using Boxed.AspNetCore;
    using LightInject.Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using NLog;
    using NLog.Web;
    using PiControlPanel.Api.GraphQL.Options;

    /// <summary>
    /// Application entrypoint class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entrypoint method.
        /// </summary>
        /// <param name="args">The arguments used when running the application.</param>
        /// <returns>The execution result code.</returns>
        public static int Main(string[] args)
        {
            var logger = BuildLogger();
            try
            {
                logger.Info("Starting application PiControlPanel");
                CreateWebHostBuilder(args).Build().Run();
                logger.Info("Stopped application PiControlPanel");
                return 0;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Application terminated unexpectedly");
                return 1;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        /// <summary>
        /// Creates a .Net Core web host builder.
        /// </summary>
        /// <param name="args">The arguments used when running the application.</param>
        /// <returns>The web host builder.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseLightInject()
                .UseIf(
                    x => string.IsNullOrEmpty(x.GetSetting(WebHostDefaults.ContentRootKey)),
                    x => x.UseContentRoot(Directory.GetCurrentDirectory()))
                .UseIf(
                    args != null,
                    x => x.UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build()))

                .ConfigureAppConfiguration((hostingContext, config) => AddConfiguration(config, hostingContext.HostingEnvironment, args))

                .UseDefaultServiceProvider((context, options) => options.ValidateScopes = context.HostingEnvironment.IsDevelopment())

                .UseKestrel(
                    (builderContext, options) =>
                    {
                        // Do not add the Server HTTP header.
                        options.AddServerHeader = false;

                        // Configure Kestrel from appsettings.json.
                        options.Configure(builderContext.Configuration.GetSection(nameof(ApplicationOptions.Kestrel)));
                        ConfigureKestrelServerLimits(builderContext, options);
                    })
                .UseNLog()
                .UseIIS()
                .UseStartup<Startup>();

        /// <summary>
        /// Gets the logger to be used in the application.
        /// </summary>
        /// <returns>An NLog Logger instance.</returns>
        private static Logger BuildLogger() => NLogBuilder.ConfigureNLog("Configuration/nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// Loads the configuration information for the app.settings file and it's corresponding environemnt settings file.
        /// </summary>
        /// <param name="configurationBuilder">IConfigurationBuilder where the application configuration is loadded into.</param>
        /// <param name="hostingEnvironment">IHostingEnvironment contains current hosting envitonment information.</param>
        /// <param name="args">parameters passed to the aplication at start.</param>
        /// <returns>The configuration builder.</returns>
        private static IConfigurationBuilder AddConfiguration(
            IConfigurationBuilder configurationBuilder,
            IWebHostEnvironment hostingEnvironment,
            string[] args) =>
            configurationBuilder

                // Add configuration from the appsettings.json file.
                .AddJsonFile("Configuration/appsettings.json", optional: true, reloadOnChange: true)

                // Add configuration from an optional appsettings.development.json, appsettings.staging.json or
                // appsettings.production.json file, depending on the environment. These settings override the ones in
                // the appsettings.json file.
                .AddJsonFile($"Configuration/appsettings.{hostingEnvironment.EnvironmentName.ToLower()}.json", optional: true, reloadOnChange: true)

                // This reads the configuration keys from the secret store. This allows you to store connection strings
                // and other sensitive settings, so you don't have to check them into your source control provider.
                // Only use this in Development, it is not intended for Production use. See
                // http://docs.asp.net/en/latest/security/app-secrets.html
                .AddIf(
                    hostingEnvironment.IsDevelopment(),
                    x => x.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true))

                // Add configuration specific to the Development, Staging or Production environments. This config can
                // be stored on the machine being deployed to or if you are using Azure, in the cloud. These settings
                // override the ones in all of the above config files. See
                // http://docs.asp.net/en/latest/security/app-secrets.html
                .AddEnvironmentVariables()

                // Add command line options. These take the highest priority.
                .AddIf(
                    args != null,
                    x => x.AddCommandLine(args));

        /// <summary>
        /// Configure Kestrel server limits from appsettings.json is not supported, so we manually copy them from config.
        /// See https://github.com/aspnet/KestrelHttpServer/issues/2216 .
        /// </summary>
        private static void ConfigureKestrelServerLimits(
            WebHostBuilderContext builderContext,
            KestrelServerOptions options)
        {
            // Allowing Synchronous calls to be made in the pipeline
            options.AllowSynchronousIO = true;

            var source = new KestrelServerOptions();
            builderContext.Configuration.GetSection(nameof(ApplicationOptions.Kestrel)).Bind(source);

            var limits = options.Limits;
            var sourceLimits = source.Limits;

            var http2 = limits.Http2;
            var sourceHttp2 = sourceLimits.Http2;

            http2.HeaderTableSize = sourceHttp2.HeaderTableSize;
            http2.InitialConnectionWindowSize = sourceHttp2.InitialConnectionWindowSize;
            http2.InitialStreamWindowSize = sourceHttp2.InitialStreamWindowSize;
            http2.MaxFrameSize = sourceHttp2.MaxFrameSize;
            http2.MaxRequestHeaderFieldSize = sourceHttp2.MaxRequestHeaderFieldSize;
            http2.MaxStreamsPerConnection = sourceHttp2.MaxStreamsPerConnection;

            limits.KeepAliveTimeout = sourceLimits.KeepAliveTimeout;
            limits.MaxConcurrentConnections = sourceLimits.MaxConcurrentConnections;
            limits.MaxConcurrentUpgradedConnections = sourceLimits.MaxConcurrentUpgradedConnections;
            limits.MaxRequestBodySize = sourceLimits.MaxRequestBodySize;
            limits.MaxRequestBufferSize = sourceLimits.MaxRequestBufferSize;
            limits.MaxRequestHeaderCount = sourceLimits.MaxRequestHeaderCount;
            limits.MaxRequestHeadersTotalSize = sourceLimits.MaxRequestHeadersTotalSize;
            limits.MaxRequestLineSize = sourceLimits.MaxRequestLineSize;
            limits.MaxResponseBufferSize = sourceLimits.MaxResponseBufferSize;
            limits.MinRequestBodyDataRate = sourceLimits.MinRequestBodyDataRate;
            limits.MinResponseDataRate = sourceLimits.MinResponseDataRate;
            limits.RequestHeadersTimeout = sourceLimits.RequestHeadersTimeout;
        }
    }
}
