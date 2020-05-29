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
using OpenBreweryDB.API.GraphQL.Mutations;
using OpenBreweryDB.API.GraphQL.InputTypes;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.API.Extensions;
using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Entity = OpenBreweryDB.Data.Models;
using System.Linq;

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
            services.AddScoped<BreweriesMutation>();
            services.AddScoped<BreweryType>();
            services.AddScoped<BreweryInputType>();
            services.AddScoped<ISchema, BreweriesSchema>();

            services.AddScoped<IBreweryConductor, BreweryConductor>();
            services.AddScoped<IBreweryFilterConductor, BreweryFilterConductor>();
            services.AddScoped<IBreweryOrderConductor, BreweryOrderConductor>();
            services.AddScoped<IBreweryValidationConductor, BreweryValidationConductor>();

            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpContextAccessor();

            services.AddAutoMapper(typeof(BreweryProfile));
            services.AddDbContext<BreweryDbContext>(options => options.UseSqlite("Data Source=openbrewery.db"));
            services.AddControllers();

            services.AddGraphQL(graphqlConfig =>
            {
                graphqlConfig.EnableMetrics = false;
                graphqlConfig.ExposeExceptions = false;
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

            // add http for Schema at default url http://localhost:5001/graphql
            app.UseGraphQL<ISchema>();

            // use graphql-playground at default url http://localhost:5001/ui/playground
            app.UseGraphQLPlayground();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<BreweryDbContext>();
                var breweryConductor = serviceScope.ServiceProvider.GetService<IBreweryConductor>();

                if (!dbContext.AllMigrationsApplied())
                {
                    dbContext.Database.Migrate();
                    dbContext.EnsureSeeded(async () =>
                    {
                        var url = "https://raw.githubusercontent.com/chrisjm/openbrewerydb-rails-api/master/db/breweries.csv";

                        using (var file = new FileDownload(url))
                        {
                            var streamReader = await file.StreamReader();

                            var breweryImportResults = GetBreweries(streamReader)
                                .Select(b =>
                                {
                                    b.Id = default;
                                    b.BreweryTags = new List<Entity.BreweryTag>();

                                    Console.WriteLine($"Importing {b.Name}");

                                    return breweryConductor.Create(b);
                                })
                                .ToList();

                            var successfulImports = breweryImportResults.Where(r => r.ErrorCount < 1).ToList();
                            var erroredImports = breweryImportResults.Where(r => r.ErrorCount > 0).ToList();

                            Console.WriteLine($"Successful Imports: {successfulImports.Count}");
                            Console.WriteLine($"Failed Imports: {erroredImports.Count}");
                        }
                    });
                }
            }
        }

        public static IEnumerable<Entity.Brewery> GetBreweries(StreamReader breweriesStreamReader)
        {
            var mapper = FlatFiles.TypeMapping.SeparatedValueTypeMapper.Define<Entity.Brewery>();
            mapper.Property(c => c.Id).ColumnName("id");
            mapper.Property(c => c.Name).ColumnName("name");
            mapper.Property(c => c.BreweryType).ColumnName("brewery_type");
            mapper.Property(c => c.Street).ColumnName("street");
            mapper.Property(c => c.City).ColumnName("city");
            mapper.Property(c => c.State).ColumnName("state");
            mapper.Property(c => c.PostalCode).ColumnName("postal_code");
            mapper.Property(c => c.WebsiteURL).ColumnName("website_url");
            mapper.Property(c => c.Phone).ColumnName("phone");
            mapper.Property(c => c.CreatedAt).ColumnName("created_at");
            mapper.Property(c => c.UpdatedAt).ColumnName("updated_at");
            mapper.Property(c => c.Country).ColumnName("country");
            mapper.Property(c => c.Longitude).ColumnName("longitude");
            mapper.Property(c => c.Latitude).ColumnName("latitude");

            var options = new FlatFiles.SeparatedValueOptions() { IsFirstRecordSchema = true };

            return mapper.Read(breweriesStreamReader, options);
        }

        class FileDownload : IDisposable
        {
            readonly HttpClient _client;
            readonly string _url;
            HttpResponseMessage _responseMessage;
            StreamReader _streamReader;

            public FileDownload(string url)
            {
                _url = url;
                _client = new HttpClient();
            }

            public async Task<StreamReader> StreamReader()
            {
                _responseMessage = await _client.GetAsync(_url);

                if (_responseMessage.IsSuccessStatusCode)
                {
                    var stream = await _responseMessage.Content.ReadAsStreamAsync();
                    _streamReader = new StreamReader(stream);

                    return _streamReader;
                }

                return null;
            }

            public void Dispose()
            {
                _streamReader?.Dispose();
                _responseMessage?.Dispose();
                _client?.Dispose();
            }
        }
    }
}
