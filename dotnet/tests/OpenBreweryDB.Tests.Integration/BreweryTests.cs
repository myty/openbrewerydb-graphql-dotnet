using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenBreweryDB.Data;
using Snapshooter.Xunit;
using Xunit;

namespace OpenBreweryDB.Tests.Integration
{
    public class BreweryTests : IClassFixture<IntegrationFixture>
    {
        private readonly IntegrationFixture _fixture;

        public BreweryTests(IntegrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task When_Query_Schema_It_ReturnResults()
        {
            var schema = await _fixture.GetSchemaAsync();

            schema
                .ToString()
                .MatchSnapshot();
        }

        [Fact]
        public async Task When_Query_Breweries_It_ReturnResults()
        {
            // Arrange
            var executor = await _fixture.GetRequestExecutorAsync();

            using var scope = _fixture.ServiceProvider.CreateScope();

            var dataContext = scope.ServiceProvider.GetService<BreweryDbContext>();
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

        [Fact]
        public async Task When_Query_Node_It_ReturnResults()
        {
            // Arrange
            var executor = await _fixture.GetRequestExecutorAsync();

            using var scope = _fixture.ServiceProvider.CreateScope();

            var dataContext = scope.ServiceProvider.GetService<BreweryDbContext>();
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
                query Brewery($id: ID!) {
                    brewery: node(id: $id) {
                        id
                        ...BreweryBaseFields
                    }
                }

                fragment BreweryBaseFields on Brewery {
                    name
                    brewery_id
                    street
                    city
                    state
                    country
                    website_url
                    brewery_type
                }
            ", new Dictionary<string, object> { { "id", "QnJld2VyeQpsMQ==" } });

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task When_Query_BreweryById_It_ReturnResults()
        {
            // Arrange
            var executor = await _fixture.GetRequestExecutorAsync();

            using var scope = _fixture.ServiceProvider.CreateScope();

            var dataContext = scope.ServiceProvider.GetService<BreweryDbContext>();
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
                query BreweryById($brewery_id: String!) {
                    brewery: breweryById(brewery_id: $brewery_id) {
                        id
                        ...BreweryBaseFields
                    }
                }

                fragment BreweryBaseFields on Brewery {
                    name
                    brewery_id
                    street
                    city
                    state
                    country
                    website_url
                    brewery_type
                }
            ", new Dictionary<string, object> { { "brewery_id", "test-id" } });

            // Assert
            result.MatchSnapshot();
        }
    }
}
