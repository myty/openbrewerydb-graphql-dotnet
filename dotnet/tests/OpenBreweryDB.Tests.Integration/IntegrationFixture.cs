using System;
using System.Threading.Tasks;
using AutoMapper;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenBreweryDB.API.Extensions;
using OpenBreweryDB.API.GraphQL.Breweries;
using OpenBreweryDB.Data;
using ThrowawayDb;

namespace OpenBreweryDB.Tests.Integration
{
    public class IntegrationFixture : IDisposable
    {
        private readonly IServiceCollection _serviceCollection;
        private ISchema _graphQLSchema = null;
        private readonly IRequestExecutorBuilder _requestExecutorBuilder;
        private IRequestExecutor _graphQLRequestExecutor = null;

        public IntegrationFixture()
        {
            Database = ThrowawayDatabase.FromLocalInstance(".");

            _serviceCollection = new ServiceCollection()
                .AddOpenBreweryServices()
                .AddAutoMapper(typeof(BreweryProfile), typeof(BreweryMappingProfile))
                .AddDbContext<BreweryDbContext>(options => options.UseSqlServer(Database.ConnectionString));

            _requestExecutorBuilder = _serviceCollection
                .AddOpenBreweryGraphQLServer();

            ServiceProvider = _serviceCollection.BuildServiceProvider();

            _ = ServiceProvider.GetService<BreweryDbContext>().Database.EnsureCreated();

            Database.CreateSnapshot();
        }

        public void Dispose() => Database.Dispose();

        public IServiceProvider ServiceProvider { get; }

        public ThrowawayDatabase Database { get; }

        public async Task<ISchema> GetSchemaAsync()
        {
            if (_graphQLSchema == null)
            {
                _graphQLSchema = await _requestExecutorBuilder.BuildSchemaAsync();
            }

            return _graphQLSchema;
        }

        public async Task<IRequestExecutor> GetRequestExecutorAsync()
        {
            if (_graphQLRequestExecutor == null)
            {
                _graphQLRequestExecutor = await _requestExecutorBuilder.BuildRequestExecutorAsync();
            }

            return _graphQLRequestExecutor;
        }
    }
}
