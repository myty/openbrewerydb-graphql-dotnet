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
using OpenBreweryDB.API.GraphQL.Queries;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors.Breweries;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data;
using System.Threading.Tasks;

namespace OpenBreweryDB.API
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBreweryConductor, BreweryConductor>();
            services.AddScoped<IBreweryFilterConductor, BreweryFilterConductor>();
            services.AddScoped<IBreweryOrderConductor, BreweryOrderConductor>();
            services.AddScoped<IBreweryValidationConductor, BreweryValidationConductor>();

            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpContextAccessor();

            services.AddAutoMapper(typeof(BreweryProfile), typeof(BreweryMappingProfile));
            services.AddDbContext<BreweryDbContext>(options => options.UseSqlite("Data Source=openbrewery.db"));
            services.AddControllers();

            services.AddGraphQL(sp => SchemaBuilder.New()
                .EnableRelaySupport()
                .AddServices(sp)

                // Adds the authorize directive and
                // enable the authorization middleware.
                .AddAuthorizeDirectiveType()

                .AddQueryType<BreweriesQuery>()
                .AddType<BreweryType>()
                .AddMutationType(d => d.Name("Mutation"))
                .AddType<BreweryMutations>()
                .Create(),
                new QueryExecutionOptions
                {
                    IncludeExceptionDetails = true,
                    TracingPreference = TracingPreference.Always
                });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    // TODO: Build based upon the Tye params
                    builder.WithOrigins("http://localhost:3000")
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

            app.UseWebSockets();
            app.UseRouting();

            app.UseGraphQL("/graphql");
            app.UsePlayground("/graphql");
            app.UseVoyager("/graphql");

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

            app.UseCors();

            // Parse and seed the db
            app.SeedDatabase("https://github.com/openbrewerydb/openbrewerydb/raw/master/breweries.csv");
        }
    }
}
