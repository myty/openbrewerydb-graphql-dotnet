using GraphQL.AspNetCore.DependencyInjection;
using GraphQL.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenBreweryDB.API.Extensions;
using OpenBreweryDB.Data;
using OpenBreweryDB.Schema;

namespace OpenBreweryDB.API
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOpenBreweryServices()
                .AddLogging(builder => builder.AddConsole())
                .AddHttpContextAccessor()
                .AddAutoMapper(typeof(BreweryProfile))
                .AddDbContextPool<BreweryDbContext>(options =>
                {
                    var connectionString = Configuration["Database:ConnectionString"];
                    options.UseSqlServer(connectionString);
                })
                .AddControllers();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddGraphQLSchemaConfiguration<OpenBrewerySchemaConfiguration>(
                includeInstrumentation: Environment.IsDevelopment()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.ApplicationServices
                    .SeedDatabaseAsync("https://github.com/openbrewerydb/openbrewerydb/raw/master/breweries.csv")
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseRouting();

            app.UseAuthentication();

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseGraphQL();

            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
