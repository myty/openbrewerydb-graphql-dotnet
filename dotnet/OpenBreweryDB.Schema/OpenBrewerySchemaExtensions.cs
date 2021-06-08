using GraphQL.Instrumentation;
using GraphQL.MicrosoftDI;
using GraphQL.Types;
using OpenBreweryDB.Schema;
using OpenBreweryDB.Schema.Dataloaders;
using OpenBreweryDB.Schema.Resolvers;
using OpenBreweryDB.Schema.Types;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpenBrewerySchemaExtensions
    {
        public static IServiceCollection AddOpenBrewerySchema(
            this IServiceCollection services,
            bool isDevelopment = true)
        {
            var baseServices = services
                .AddSingleton<ISchema, OpenBrewerySchema>(services =>
                    new OpenBrewerySchema(new SelfActivatingServiceProvider(services)))
                .AddScoped<TagResolver>()
                .AddScoped<BreweryResolver>()
                .AddScoped<BreweryDataloader>()
                .AddScoped<TagDataloader>();

            if (isDevelopment)
            {
                baseServices = baseServices.AddSingleton<IFieldMiddleware, InstrumentFieldsMiddleware>();
            }

            return baseServices;
        }
    }
}
