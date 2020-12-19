
using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBreweryDB.Schema
{
    public class OpenBrewerySchema : GraphQL.Types.Schema
    {
        public OpenBrewerySchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<OpenBreweryQuery>();
        }
    }
}
