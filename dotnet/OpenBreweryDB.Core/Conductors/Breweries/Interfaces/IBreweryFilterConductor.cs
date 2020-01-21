using System;
using System.Linq.Expressions;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries.Interfaces
{
    public interface IBreweryFilterConductor
    {
        Expression<Func<Brewery, bool>> BuildFilter(
            string by_name = null,
            string by_state = null,
            string by_tag = null,
            string by_city = null,
            string by_tags = null,
            string by_type = null);

        Expression<Func<Brewery, bool>> BuildSearchQueryFilter(string query = null);
    }
}