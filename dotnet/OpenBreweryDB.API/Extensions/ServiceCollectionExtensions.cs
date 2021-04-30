using HotChocolate;
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

namespace OpenBreweryDB.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenBreweryServices(this IServiceCollection services) => services
            .AddScoped<IBreweryConductor, BreweryConductor>()
            .AddScoped<IUserConductor, UserConductor>()
            .AddScoped<IBreweryFilterConductor, BreweryFilterConductor>()
            .AddScoped<IBreweryOrderConductor, BreweryOrderConductor>()
            .AddScoped<IBreweryValidationConductor, BreweryValidationConductor>();

        public static IRequestExecutorBuilder AddOpenBreweryGraphQLServer(this IServiceCollection services, bool seedData = false)
        {
            var requestExecutorBuilder = services
                .AddGraphQLServer()
                .AddQueryType(d => d.Name("Query"))
                    .AddTypeExtension<BreweryQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                    .AddTypeExtension<UserMutations>()
                    .AddTypeExtension<BreweryMutations>()
                    .AddTypeExtension<ReviewMutations>()
                .AddSubscriptionType(d => d.Name("Subscription"))
                    .AddTypeExtension<ReviewSubscriptions>()
                .AddType<BreweryType>()
                .AddFiltering()
                .AddSorting()
                .EnableRelaySupport()
                .AddInMemorySubscriptions()
                .AddAuthorization()
                .AddApolloTracing();
            if (seedData)
            {
                requestExecutorBuilder.ConfigureSchemaAsync(async (services, builder, ct) =>
                {
                    await services.SeedDatabaseAsync("https://github.com/openbrewerydb/openbrewerydb/raw/master/breweries.csv");
                });
            }

            return requestExecutorBuilder;
        }
    }
}
