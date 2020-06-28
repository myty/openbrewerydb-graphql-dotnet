using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlatFiles.TypeMapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data;

using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.Extensions
{
    public static class DbContextExtension
    {
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public static void EnsureSeeded(this BreweryDbContext context, Action generator)
        {
            if (context.Breweries.Any() == false)
            {
                generator();
            }
        }

        public static void SeedDatabase(this IApplicationBuilder app, string csvUrl)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetService<ILogger<Startup>>();
                var dbContext = serviceScope.ServiceProvider.GetService<BreweryDbContext>();
                var breweryConductor = serviceScope.ServiceProvider.GetService<IBreweryConductor>();

                if (!dbContext.AllMigrationsApplied())
                {
                    logger.LogInformation($"Initializing Database");

                    dbContext.Database.Migrate();
                }

                dbContext.EnsureSeeded(() =>
                {
                    logger.LogInformation($"Downloading: {csvUrl}");

                    using (var file = new FileDownload(csvUrl))
                    {
                        var streamReader = file.StreamReader().ConfigureAwait(false).GetAwaiter().GetResult();

                        var breweryImportEntities = GetBreweries(streamReader)
                            .Select(b =>
                            {
                                b.Id = default;
                                b.BreweryTags = new List<Entity.BreweryTag>();

                                return b;
                            })
                            .ToList();

                        logger.LogInformation($"Importing {breweryImportEntities.Count} breweries");

                        var results = breweryConductor.BulkCreate(breweryImportEntities);

                        static IEnumerable<Entity.Brewery> GetBreweries(StreamReader breweriesStreamReader)
                        {
                            var mapper = SeparatedValueTypeMapper.Define<Entity.Brewery>();
                            mapper.Property(c => c.BreweryId).ColumnName("id");
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
                    }
                });
            }
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
