
using System;
using System.Collections.Generic;
using GraphQL.Instrumentation;

namespace OpenBreweryDB.Schema
{
    public class OpenBrewerySchema : GraphQL.Types.Schema
    {
        public OpenBrewerySchema(
            IServiceProvider serviceProvider,
            OpenBreweryQuery query,
            IEnumerable<IFieldMiddleware> middlewares)
            : base(serviceProvider)
        {
            Query = query;

            foreach (var middleware in middlewares)
            {
                _ = FieldMiddleware.Use(middleware);
            }
        }
    }
}
