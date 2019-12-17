using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL;
using GraphQL.Http;
using Microsoft.Extensions.Logging;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Voyager;
using OpenBreweryDB.API.Data.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace OpenBreweryDB.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(Program));
            services.AddDbContext<BreweryDbContext>();
            services.AddControllers();

            // services.AddGraphQL(_ =>
            // {
            //     _.EnableMetrics = true;
            //     _.ExposeExceptions = true;
            // })
            // .AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User });
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
            // app.UseGraphQLWebSockets<OrdersSchema>("/graphql");
            // app.UseGraphQL<OrdersSchema>("/graphql");
            // app.UseGraphQLPlayground(new GraphQLPlaygroundOptions()
            // {
            //     Path = "/ui/playground"
            // });
            // app.UseGraphiQLServer(new GraphiQLOptions
            // {
            //     GraphiQLPath = "/ui/graphiql",
            //     GraphQLEndPoint = "/graphql"
            // });
            // app.UseGraphQLVoyager(new GraphQLVoyagerOptions()
            // {
            //     GraphQLEndPoint = "/graphql",
            //     Path = "/ui/voyager"
            // });
        }
    }
}
