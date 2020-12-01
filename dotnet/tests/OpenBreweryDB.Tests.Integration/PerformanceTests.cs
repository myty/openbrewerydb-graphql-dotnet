using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenBreweryDB.Data;
using Xunit;
using Xunit.Abstractions;

namespace OpenBreweryDB.Tests.Integration
{
    public class PerformanceTests : IClassFixture<IntegrationFixture>
    {
        private readonly ITestOutputHelper _output;
        private readonly IntegrationFixture _fixture;

        public PerformanceTests(IntegrationFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _fixture = fixture;
            _fixture.Database.RestoreSnapshot();
        }

        [Fact]
        public async Task When_Query_Schema_It_Is_Performant()
        {
            // Arrange
            var iterations = 10;
            var timer = new Stopwatch();

            // Act
            timer.Start();
            for (var i = 0; i < iterations; i++)
            {
                _ = await _fixture.GetSchemaAsync();
            }
            timer.Stop();

            // Assert
            var avgElapsedTime = timer.ElapsedMilliseconds / iterations;

            _output.WriteLine($"Avg. Elapsed Time: {avgElapsedTime}ms.");
            Assert.True(avgElapsedTime < 100);
        }

        [Fact]
        public async Task When_Query_Breweries_It_Is_Performant()
        {
            // Arrange
            var timer = new Stopwatch();
            timer.Start();

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

            _ = await dataContext.SaveChangesAsync();

            // Act
            var executor = await _fixture.GetRequestExecutorAsync();
            _ = await executor.ExecuteAsync(@"
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

            timer.Stop();

            // Assert
            _output.WriteLine($"Elapsed Time: {timer.ElapsedMilliseconds}ms.");
            Assert.True(timer.ElapsedMilliseconds < 700);
        }

        [Fact]
        public async Task When_Query_Node_It_Is_Performant()
        {
            // Arrange
            var timer = new Stopwatch();
            timer.Start();

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
            var executor = await _fixture.GetRequestExecutorAsync();
            _ = await executor.ExecuteAsync(@"
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

            timer.Stop();

            // Assert
            _output.WriteLine($"Elapsed Time: {timer.ElapsedMilliseconds}ms.");
            Assert.True(timer.ElapsedMilliseconds < 100);
        }

        [Fact]
        public async Task When_Query_BreweryById_It_Is_Performant()
        {
            // Arrange
            var timer = new Stopwatch();
            timer.Start();

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
            var executor = await _fixture.GetRequestExecutorAsync();
            _ = await executor.ExecuteAsync(@"
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

            timer.Stop();

            // Assert
            _output.WriteLine($"Elapsed Time: {timer.ElapsedMilliseconds}ms.");
            Assert.True(timer.ElapsedMilliseconds < 700);
        }
    }
}
