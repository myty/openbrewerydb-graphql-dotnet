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
using Snapshooter.Xunit;
using Xunit;

namespace OpenBreweryDB.Tests.Integration
{
    public class BreweryTests
    {
        readonly IServiceCollection _serviceCollection;
        readonly IServiceProvider _serviceProvider;

        public BreweryTests()
        {
            var services = new ServiceCollection();
            services.AddScoped<IBreweryConductor, BreweryConductor>();
            services.AddScoped<IUserConductor, UserConductor>();
            services.AddScoped<IBreweryFilterConductor, BreweryFilterConductor>();
            services.AddScoped<IBreweryOrderConductor, BreweryOrderConductor>();
            services.AddScoped<IBreweryValidationConductor, BreweryValidationConductor>();

            services.AddAutoMapper(typeof(BreweryProfile), typeof(BreweryMappingProfile));
            services.AddDbContext<BreweryDbContext>(options => options.UseInMemoryDatabase("Data Source=openbrewery.db"));

            _serviceCollection = services;
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        private IRequestExecutorBuilder GetRequestExecutorBuilder()
        {
            return _serviceCollection
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

        [Fact]
        public async Task SchemaReturnsCorrectStructure()
        {
            var schema = await GetRequestExecutorBuilder()
                .BuildSchemaAsync();

            schema
                .ToString()
                .MatchSnapshot();
        }

        [Fact]
        public async Task ReturnResults()
        {
            // Arrange
            var executor = await GetRequestExecutorBuilder()
                .BuildRequestExecutorAsync();
            var dataContext = _serviceProvider.GetService<BreweryDbContext>();
            _ = dataContext.Breweries.Add(new Data.Models.Brewery
            {
                Name = "Test",
                BreweryId = "test-id",
                Street = "123 Any St.",
                City = "My Town",
                State = "PA",
                BreweryType = "micro"
            });
            _ = dataContext.Breweries.Add(new Data.Models.Brewery
            {
                Name = "Test",
                BreweryId = "test-id",
                Street = "123 Any St.",
                City = "My Town",
                State = "PA",
                BreweryType = "micro"
            });

            await dataContext.SaveChangesAsync();

            // Act
            var result = await executor.ExecuteAsync(@"
                query Breweries {
                    breweries(first: 1) {
                        edges {
                            cursor
                            node {
                                name
                                brewery_id
                                id
                                street
                                city
                                state
                                brewery_type
                            }
                        }
                    }
                }
            ");

            // Assert
            result.MatchSnapshot();
        }
    }
}
