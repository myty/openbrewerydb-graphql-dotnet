using AutoMapper;
using HotChocolate;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenBreweryDB.API.Extensions;
using OpenBreweryDB.API.GraphQL.Breweries;
using OpenBreweryDB.API.GraphQL.Reviews;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.API.GraphQL.Users;
using OpenBreweryDB.Core.Conductors.Breweries;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Conductors.Users.Interfaces;
using OpenBreweryDB.Data;

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
                .AddGraphQLServer()
                .AddInMemorySubscriptions()
                .AddQueryType(d => d.Name("Query"))
                    .AddTypeExtension<BreweryQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                    .AddTypeExtension<UserMutations>()
                    .AddTypeExtension<BreweryMutations>()
                    .AddTypeExtension<ReviewMutations>()
                .AddSubscriptionType(d => d.Name("Subscription"))
                    .AddTypeExtension<ReviewSubscriptions>()
                .AddType<BreweryType>()

                .EnableRelaySupport()
                .SetPagingOptions(new PagingOptions
                {
                    IncludeTotalCount = true
                })
                .AddAuthorization()
                .AddApolloTracing();

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
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Parse and seed the db
            app.SeedDatabase("https://github.com/openbrewerydb/openbrewerydb/raw/master/breweries.csv");
        }
    }
}
