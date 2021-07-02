using GraphQL;
using GraphQL.AspNetCore.DependencyInjection;
using GraphQL.DataLoader;
using GraphQL.Instrumentation;
using GraphQL.MicrosoftDI;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using OpenBreweryDB.Schema.Dataloaders;
using OpenBreweryDB.Schema.Resolvers;

namespace OpenBreweryDB.Schema
{
    public class OpenBrewerySchemaConfiguration : IGraphQLSchemaConfiguration
    {
        public IServiceCollection Configure(IServiceCollection services, bool includeInstrumentation = false)
        {
            var baseServices = services
                .AddSingleton<ISchema, OpenBrewerySchema>(services =>
                    new OpenBrewerySchema(new SelfActivatingServiceProvider(services)))
                .AddSingleton<IDocumentExecuter, OpenBreweryDocumentExecuter>()
                .AddSingleton<IDocumentWriter, DocumentWriter>()
                .AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>()
                .AddSingleton<DataLoaderDocumentListener>()
                .AddScoped<TagResolver>()
                .AddScoped<BreweryResolver>()
                .AddScoped<BreweryDataloader>()
                .AddScoped<TagDataloader>();

            if (includeInstrumentation)
            {
                baseServices = baseServices.AddSingleton<IFieldMiddleware, InstrumentFieldsMiddleware>();
            }

            return baseServices;

        }
    }
}
