using AutoMapper;
using GraphQL.Server;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenBreweryDB.Core;
using OpenBreweryDB.Core.Conductors.Breweries;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.API.GraphQL;
using OpenBreweryDB.API.GraphQL.Queries;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Data;

namespace OpenBreweryDB.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // GraphQL
            services.AddScoped<BreweriesQuery>();
            services.AddScoped<BreweryType>();
            services.AddScoped<ISchema, BreweriesSchema>();

            services.AddScoped<IBreweryFilterConductor, BreweryFilterConductor>();

            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpContextAccessor();

            services.AddAutoMapper(typeof(BreweryProfile));
            services.AddDbContext<BreweryDbContext>();
            services.AddControllers();

            services.AddGraphQL(graphqlConfig =>
            {
                graphqlConfig.EnableMetrics = true;
                graphqlConfig.ExposeExceptions = true;
            })
            .AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User });

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
            app.UseWebSockets();

            // add http for Schema at default url http://localhost:5000/graphql
            app.UseGraphQL<ISchema>();

            // use graphql-playground at default url http://localhost:5000/ui/playground
            app.UseGraphQLPlayground();
        }
    }
}
