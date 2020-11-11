using System;
using System.Threading.Tasks;
using AutoMapper;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenBreweryDB.API.GraphQL.Breweries;
using OpenBreweryDB.API.GraphQL.Reviews;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.API.GraphQL.Users;
using OpenBreweryDB.Core.Conductors.Breweries;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Conductors.Users.Interfaces;
using OpenBreweryDB.Data;
using ThrowawayDb;

namespace OpenBreweryDB.Tests.Integration
{
    public class IntegrationFixture : IDisposable
    {
        readonly ThrowawayDatabase _throwawayDatabase;
        readonly IServiceCollection _serviceCollection;
        private ISchema _graphQLSchema = null;
        private IRequestExecutor _graphQLRequestExecutor = null;

        private static class DbSettings
        {
            public const string Username = "sa";
            public const string Password = "Pass123!";
            public const string Host = "localhost";
        }

        public IntegrationFixture()
        {
            _throwawayDatabase = ThrowawayDatabase.FromLocalInstance("localhost\\SQLEXPRESS", "graphqltests_");

            var services = new ServiceCollection();
            services.AddScoped<IBreweryConductor, BreweryConductor>();
            services.AddScoped<IUserConductor, UserConductor>();
            services.AddScoped<IBreweryFilterConductor, BreweryFilterConductor>();
            services.AddScoped<IBreweryOrderConductor, BreweryOrderConductor>();
            services.AddScoped<IBreweryValidationConductor, BreweryValidationConductor>();

            services.AddAutoMapper(typeof(BreweryProfile), typeof(BreweryMappingProfile));
            services.AddDbContext<BreweryDbContext>(options => options.UseSqlServer(_throwawayDatabase.ConnectionString));

            _serviceCollection = services;
            ServiceProvider = _serviceCollection.BuildServiceProvider();

            ServiceProvider.GetService<BreweryDbContext>().Database.EnsureCreated();
        }

        public void Dispose()
        {
            _throwawayDatabase.Dispose();
        }

        public IServiceProvider ServiceProvider { get; }

        public async Task<ISchema> GetSchemaAsync()
        {
            if (_graphQLSchema == null)
            {
                _graphQLSchema = await GetRequestExecutorBuilder().BuildSchemaAsync();
            }

            return _graphQLSchema;
        }

        public async Task<IRequestExecutor> GetRequestExecutorAsync()
        {
            if (_graphQLSchema == null)
            {
                _graphQLRequestExecutor = await GetRequestExecutorBuilder().BuildRequestExecutorAsync();
            }

            return _graphQLRequestExecutor;
        }
        private IRequestExecutorBuilder GetRequestExecutorBuilder() => _serviceCollection
            .AddGraphQL()
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
            });
    }
}
