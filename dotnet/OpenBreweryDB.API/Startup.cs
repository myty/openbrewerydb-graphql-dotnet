using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenBreweryDB.Core.Conductors.Breweries;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.API.GraphQL;
using OpenBreweryDB.API.GraphQL.Queries;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Data;
using OpenBreweryDB.API.GraphQL.Mutations;
using OpenBreweryDB.API.GraphQL.InputTypes;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.API.Extensions;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Execution.Configuration;
using HotChocolate.Subscriptions;

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
            services.AddScoped<Query>();

            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpContextAccessor();

            services.AddAutoMapper(typeof(BreweryProfile));
            services.AddDbContext<BreweryDbContext>(options => options.UseSqlite("Data Source=openbrewery.db"));
            services.AddControllers();

            services.AddGraphQL(sp => SchemaBuilder.New()
                .AddServices(sp)

                // Adds the authorize directive and
                // enable the authorization middleware.
                .AddAuthorizeDirectiveType()

                .AddQueryType<BreweriesQuery>()
                .AddType<BreweryType>()
                // .AddMutationType<BreweriesMutation>()
                // .AddType<BreweryInputType>()
                .Create());

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseWebSockets()
                .UseGraphQL("/graphql")
                .UsePlayground("/graphql")
                .UseVoyager("/graphql");

            // Parse and seed the db
            app.SeedDatabase("https://raw.githubusercontent.com/chrisjm/openbrewerydb-rails-api/master/db/breweries.csv");
        }
    }
}
