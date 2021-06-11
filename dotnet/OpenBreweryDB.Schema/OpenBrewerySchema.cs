
using System;
using System.Collections.Generic;
using GraphQL.Instrumentation;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBreweryDB.Schema
{
    public class OpenBrewerySchema : GraphQL.Types.Schema
    {
        public OpenBrewerySchema(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

            Query = serviceProvider.GetRequiredService<OpenBreweryQuery>();

            var middlewares = serviceProvider
                .GetRequiredService<IEnumerable<IFieldMiddleware>>();

            foreach (var middleware in middlewares)
            {
                _ = FieldMiddleware.Use(middleware);
            }
        }
    }
}
