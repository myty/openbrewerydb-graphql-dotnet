using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate.Execution;
using HotChocolate.Types.Relay;
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
        private readonly IIdSerializer _idSerializer;

        public BreweryTests(IntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.ResetDatabase();

            _idSerializer = new IdSerializer();
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
                BreweryId = "test-id-2",
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
            var entry = dataContext.Breweries.Add(new Data.Models.Brewery
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
            var queryParams = new Dictionary<string, object> {
                { "id", SerializedId(entry.Entity, (e) => e.Id) },
            };

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
            ", queryParams);

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task When_Query_Node_Twice_It_Returns_Two_Results()
        {
            // Arrange
            var executor = await _fixture.GetRequestExecutorAsync();

            using var scope = _fixture.ServiceProvider.CreateScope();

            var dataContext = scope.ServiceProvider.GetService<BreweryDbContext>();
            var entity1 = dataContext.Breweries.Add(new Data.Models.Brewery
            {
                Name = "Test",
                BreweryId = "test-id",
                Street = "123 Any St.",
                City = "My Town",
                State = "PA",
                BreweryType = "micro"
            });
            var entity2 = dataContext.Breweries.Add(new Data.Models.Brewery
            {
                Name = "Test",
                BreweryId = "test-id-2",
                Street = "123 Any St.",
                City = "My Town",
                State = "PA",
                BreweryType = "micro"
            });

            await dataContext.SaveChangesAsync();

            // Act
            var queryParams = new Dictionary<string, object> {
                { "id", SerializedId(entity1.Entity, (e) => e.Id) },
                { "id2", SerializedId(entity2.Entity, (e) => e.Id) }
            };

            var result = await executor.ExecuteAsync(@"
                query Brewery($id: ID!, $id2: ID!) {
                    brewery1: node(id: $id) {
                        id
                        ...BreweryBaseFields
                    }
                    brewery2: node(id: $id2) {
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
            ", queryParams);

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

        private string SerializedId<T, V>(string typeName, T entity, Func<T, V> idValueFunc) =>
            _idSerializer.Serialize(null, typeName, idValueFunc(entity));

        private string SerializedId<T, V>(T entity, Func<T, V> idValueFunc) =>
            SerializedId(typeof(T).Name, entity, idValueFunc);
    }
}
