using AutoMapper;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenBreweryDB.API.Extensions;
using OpenBreweryDB.API.GraphQL.Breweries;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors.Breweries;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using OpenBreweryDB.Data.Models.Users;
using OpenBreweryDB.Core.Conductors.Users.Interfaces;
using OpenBreweryDB.API.GraphQL.Users;
using OpenBreweryDB.API.GraphQL.Reviews;

namespace OpenBreweryDB.API
{
    public partial class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBreweryConductor, BreweryConductor>();
            services.AddScoped<IUserConductor, UserConductor>();
            services.AddScoped<IBreweryFilterConductor, BreweryFilterConductor>();
            services.AddScoped<IBreweryOrderConductor, BreweryOrderConductor>();
            services.AddScoped<IBreweryValidationConductor, BreweryValidationConductor>();

            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpContextAccessor();

            services.AddAutoMapper(typeof(BreweryProfile), typeof(BreweryMappingProfile));
            services.AddDbContext<BreweryDbContext>(options => options.UseSqlite("Data Source=openbrewery.db"));
            services.AddControllers();

            services
                .AddInMemorySubscriptions()
                .AddGraphQL(sp => SchemaBuilder.New()
                .EnableRelaySupport()
                .AddServices(sp)

                // Adds the authorize directive and
                // enable the authorization middleware.
                .AddAuthorizeDirectiveType()

                .AddQueryType(d => d.Name("Query"))
                .AddType<BreweryQueries>()
                .AddType<BreweryType>()
                .AddMutationType(d => d.Name("Mutation"))
                .AddType<UserMutations>()
                .AddType<BreweryMutations>()
                .AddType<ReviewMutations>()
                .AddSubscriptionType(d => d.Name("Subscription"))
                .AddType<ReviewSubscriptions>()
                .Create(),
                new QueryExecutionOptions
                {
                    ForceSerialExecution = true,
                    IncludeExceptionDetails = true,
                    TracingPreference = TracingPreference.Always
                });

            services.AddQueryRequestInterceptor(async (context, builder, ct) =>
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    var userId =
                        Guid.Parse(context.User.FindFirst(WellKnownClaimTypes.UserId).Value);

                    builder.AddProperty(
                        "currentUserId",
                        userId);
                    builder.AddProperty(
                        "currentUserEmail",
                        context.User.FindFirst(ClaimTypes.Email).Value);

                    await Task.Yield();
                }
            });

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

            app.UseWebSockets()
                .UseGraphQL("/graphql")
                .UsePlayground("/graphql")
                .UseVoyager("/graphql");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/graphql/playground");
                    return Task.CompletedTask;
                });
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Parse and seed the db
            app.SeedDatabase("https://github.com/openbrewerydb/openbrewerydb/raw/master/breweries.csv");
        }
    }
}
