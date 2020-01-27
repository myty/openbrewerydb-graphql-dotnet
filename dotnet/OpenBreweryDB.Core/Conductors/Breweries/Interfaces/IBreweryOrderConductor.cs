using System;
using System.Collections.Generic;
using System.Linq;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries.Interfaces
{
    public interface IBreweryOrderConductor
    {
        Func<IQueryable<Brewery>, IQueryable<Brewery>> OrderByFields(IDictionary<string, SortDirection> fields);
    }
}
