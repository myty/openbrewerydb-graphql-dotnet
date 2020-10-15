using System;
using System.Threading.Tasks;
using AutoMapper;
using HotChocolate;
using HotChocolate.Execution;
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
using Shouldly;
using Snapshooter.Xunit;
using Xunit;

namespace OpenBreweryDB.Tests.Integration
{
    public class BreweryTests
    {
        readonly ISchema _schema;
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

            _serviceProvider = services.BuildServiceProvider();

            _schema = SchemaBuilder.New()
                .EnableRelaySupport()
                .AddServices(_serviceProvider)

                // Adds the authorize directive and
                // enable the authorization middleware.
                .AddAuthorizeDirectiveType()

                .AddQueryType(d => d.Name("Query"))
                .AddType<BreweryQueries>()
                .AddType<BreweryType>()
                .AddMutationType(d => d.Name("Mutation"))
                .AddType<UserMutations>()
                .AddType<BreweryMutations>()
                .AddType<ReviewMutations>()
                .AddSubscriptionType(d => d.Name("Subscription"))
                .AddType<ReviewSubscriptions>()
                .Create();
        }

        [Fact]
        public void SchemaReturnsCorrectStructure()
        {
            _schema
                .ToString()
                .MatchSnapshot();
        }

        [Fact]
        public async Task ReturnResults()
        {
            // Arrange
            var executor = _schema.MakeExecutable();
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
            result.Errors.ShouldBeEmpty();
            result.MatchSnapshot();
        }
    }
}
