﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using FlatFiles;
using FlatFiles.TypeMapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenBreweryDB.Core.Conductors.Breweries;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data;

using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IBreweryConductor, BreweryConductor>()
                .AddScoped<IBreweryFilterConductor, BreweryFilterConductor>()
                .AddScoped<IBreweryOrderConductor, BreweryOrderConductor>()
                .AddScoped<IBreweryValidationConductor, BreweryValidationConductor>()
                .AddAutoMapper(typeof(BreweryProfile))
                .AddDbContext<BreweryDbContext>(options => options.UseSqlite("Data Source=../OpenBreweryDB.API/openbrewery.db"))
                .BuildServiceProvider();

            var breweryConductor = serviceProvider.GetService<IBreweryConductor>();

            var breweriesResult = breweryConductor.FindAll(take: 1000);
            if (breweriesResult.ErrorCount > 0)
            {
                throw new Exception(breweriesResult.Errors.FirstOrDefault()?.Message);
            }

            System.Console.WriteLine($"About to delete {breweriesResult.ResultObject.Count()} records.");
            foreach (var brewery in breweriesResult.ResultObject)
            {
                breweryConductor.Delete(brewery.Id);
            }

            var url = "https://raw.githubusercontent.com/chrisjm/openbrewerydb-rails-api/master/db/breweries.csv";

            using (var file = new FileDownload(url))
            {
                var streamReader = await file.StreamReader();

                var breweryImportResults = GetBreweries(streamReader)
                    .Select(b => {
                        b.Id = default;
                        b.BreweryTags = new List<Entity.BreweryTag>();

                        System.Console.WriteLine($"Importing {b.Name}");

                        return breweryConductor.Create(b);
                    })
                    .ToList();

                var successfulImports = breweryImportResults.Where(r => r.ErrorCount < 1).ToList();
                var erroredImports = breweryImportResults.Where(r => r.ErrorCount > 0).ToList();

                System.Console.WriteLine($"Successful Imports: {successfulImports.Count}");
                System.Console.WriteLine($"Errored Imports: {erroredImports.Count}");
            }
        }

        public static IEnumerable<Entity.Brewery> GetBreweries(StreamReader breweriesStreamReader)
        {
            var mapper = SeparatedValueTypeMapper.Define<Entity.Brewery>();
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

            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };

            return mapper.Read(breweriesStreamReader, options);
        }
    }

    public class FileDownload : IDisposable
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
