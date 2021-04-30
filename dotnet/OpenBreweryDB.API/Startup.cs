using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenBreweryDB.API.Extensions;
using OpenBreweryDB.API.GraphQL.Breweries;
using OpenBreweryDB.Data;

namespace OpenBreweryDB.API
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenBreweryServices()
                .AddLogging(builder => builder.AddConsole())
                .AddHttpContextAccessor()
                .AddAutoMapper(typeof(BreweryProfile), typeof(BreweryMappingProfile))
                .AddDbContext<BreweryDbContext>(options =>
                {
                    var connectionString = Configuration["Database:ConnectionString"];
                    options.UseSqlServer(connectionString);
                })
                .AddControllers();

            services
                .AddOpenBreweryGraphQLServer(true);

            // TODO: Add JWT user authentication and authorization
            // services.AddQueryRequestInterceptor(async (context, builder, ct) =>
            // {
            //     if (context.User.Identity.IsAuthenticated)
            //     {
            //         var userId =
            //             Guid.Parse(context.User.FindFirst(WellKnownClaimTypes.UserId).Value);

            //         builder.AddProperty(
            //             "currentUserId",
            //             userId);
            //         builder.AddProperty(
            //             "currentUserEmail",
            //             context.User.FindFirst(ClaimTypes.Email).Value);

            //         await Task.Yield();
            //     }
            // });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            ConfigureAuthenticationServices(services);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseRouting();

            app.UseAuthentication();

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
