namespace PiControlPanel.Api.GraphQL
{
    using System;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Boxed.AspNetCore;
    using global::GraphQL;
    using global::GraphQL.Server;
    using global::GraphQL.Server.Ui.Playground;
    using LightInject;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using NLog;
    using NLog.Web;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Api.GraphQL.Schemas;
    using PiControlPanel.Application.BackgroundServices;
    using PiControlPanel.Application.BackgroundServices.Cpu;
    using PiControlPanel.Application.Services;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// Application startup class.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="webHostEnvironment">IHostingEnvironment.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = NLogBuilder.ConfigureNLog("Configuration/nlog.config").GetCurrentClassLogger();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(); // enables Access-Control-Allow-Origin (angular calling graphql methods)

            services.AddGraphQL(options => { options.ExposeExceptions = true; })
                .AddUserContextBuilder(context => new GraphQLUserContext { User = context.User });

            services.AddCustomGraphQL(this.webHostEnvironment);

            services.AddRequiredServices(this.configuration, this.logger);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = this.configuration["Jwt:Issuer"],
                        ValidAudience = this.configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Jwt:Key"]))
                    };

                    if (this.webHostEnvironment.IsDevelopment())
                    {
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                this.logger.Warn("OnAuthenticationFailed: " + context.Exception.Message);
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                this.logger.Trace("OnTokenValidated: " + context.SecurityToken);
                                return Task.CompletedTask;
                            }
                        };
                    }
                });

            services.AddGraphQLAuth(settings =>
            {
                settings.AddPolicy(
                    AuthorizationPolicyName.AuthenticatedPolicy,
                    p => p
                        .RequireClaim(CustomClaimTypes.IsAnonymous, new string[] { bool.FalseString })
                        .RequireClaim(CustomClaimTypes.IsAuthenticated, new string[] { bool.TrueString })
                        .RequireClaim(ClaimTypes.Role, new string[] { Roles.User }));
                settings.AddPolicy(
                    AuthorizationPolicyName.SuperUserPolicy,
                    p => p
                        .RequireClaim(CustomClaimTypes.IsAnonymous, new string[] { bool.FalseString })
                        .RequireClaim(CustomClaimTypes.IsAuthenticated, new string[] { bool.TrueString })
                        .RequireClaim(ClaimTypes.Role, new string[] { Roles.SuperUser }));
            });

            services.AddHealthChecks();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // Allowing Synchronous calls to be made in the pipeline
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddHostedService<NetworkWorker>();
            services.AddHostedService<DiskWorker>();
            services.AddHostedService<MemoryWorker<RandomAccessMemory, RandomAccessMemoryStatus>>();
            services.AddHostedService<MemoryWorker<SwapMemory, SwapMemoryStatus>>();
            services.AddHostedService<NetworkInterfaceStatusWorker>();

            if (!this.IsRunningInContainer())
            {
                services.AddHostedService<CpuWorker>();
                services.AddHostedService<ChipsetWorker>();
                services.AddHostedService<GpuWorker>();
                services.AddHostedService<OsWorker>();
                services.AddHostedService<CpuFrequencyWorker>();
            }
            else
            {
                this.logger.Warn("Running on Docker, not creating incompatible background services.");
            }

            // Configuring SPA Path
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "PiControlPanel.Ui.Angular/dist";
            });
        }

        /// <summary>
        ///     This method is used to add services directly to LightInject.
        /// </summary>
        /// <param name="container">LightInject service container.</param>
        public void ConfigureContainer(IServiceContainer container)
        {
            // Sets LightInject as GraphQL's dependency resolver
            container.RegisterSingleton<IDependencyResolver>(s => new FuncDependencyResolver(container.GetInstance));

            // Registers all services required for the Application layer
            container.RegisterFrom<ApplicationCompositionRoot>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder.</param>
        /// <param name="webHostEnvironment">IWebHostEnvironment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment webHostEnvironment)
        {
            app.UseIf(
                    !webHostEnvironment.IsProduction(),
                    a => a
                        .UseDeveloperExceptionPage()
                        .UseGraphQLPlayground(new GraphQLPlaygroundOptions() { Path = "/playground" }));

            // Enables Access-Control-Allow-Origin (angular calling webapi methods)
            app.UseCors(builder => builder
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            // Add GraphQL web sockets middleware to the request pipeline (support for subscriptions)
            app.UseWebSockets();
            app.UseGraphQLWebSockets<ControlPanelSchema>();

            app.UseForwardedHeaders()
                .UseAuthentication()
                .UseHealthChecks("/healthcheck")
                .UseGraphQL<ControlPanelSchema>();

            // Add web sockets middleware to the request pipeline (support for shell commands through SSH)
            app.UseMiddleware<WebSocketToSshMiddleware>();

            // Enables static files to serve Ui pages
            app.UseStaticFiles();
            if (!webHostEnvironment.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "PiControlPanel.Ui.Angular";
            });
        }

        /// <summary>
        /// Returns true if running the application inside a Docker container.
        /// </summary>
        private bool IsRunningInContainer()
        {
            return true.ToString().Equals(
                Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
