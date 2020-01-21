using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries.Interfaces
{
    public interface IBreweryFilterConductor
    {
        Expression<Func<Brewery, bool>> BuildFilter(
            string by_name = null,
            string by_state = null,
            string by_city = null,
            string by_type = null,
            IEnumerable<string> by_tags = null);

        Expression<Func<Brewery, bool>> BuildSearchQueryFilter(string query = null);
    }
}