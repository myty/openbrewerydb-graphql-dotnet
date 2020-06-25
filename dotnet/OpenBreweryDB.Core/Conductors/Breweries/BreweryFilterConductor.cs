using System;
using System.Linq.Expressions;
using OpenBreweryDB.Core.Extensions;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace OpenBreweryDB.Core.Conductors.Breweries
{
    public class BreweryFilterConductor : IBreweryFilterConductor
    {
        public Expression<Func<Brewery, bool>> BuildFilter(
            string by_name = null,
            string by_state = null,
            string by_city = null,
            string by_type = null,
            IEnumerable<string> by_tags = null)
        {

            Expression<Func<Brewery, bool>> filter = b => true;

            // by_city
            if (!string.IsNullOrEmpty(by_city?.Trim()))
            {
                by_city = by_city.ToLower().Trim();

                filter = filter.AndAlso(b => b.City.ToLower().Contains(by_city));
            }

            // by_name
            if (!string.IsNullOrEmpty(by_name?.Trim()))
            {
                filter = filter.AndAlso(b => b.Name.ToLower().Contains(by_name.ToLower().Trim()));
            }

            // by_state
            if (!string.IsNullOrEmpty(by_state?.Trim()))
            {
                filter = filter.AndAlso(b => b.State.ToLower().Replace(" ", "_") == by_state.ToLower().Trim().Replace(" ", "_"));
            }

            //  by_tags
            if (by_tags?.Any() == true)
            {
                filter = filter.AndAlso(b => b.BreweryTags.Select(bt => bt.Tag.Name).Any(t => by_tags.Contains(t)));
            }

            // by_type
            if (!string.IsNullOrEmpty(by_type?.Trim()))
            {
                filter = filter.AndAlso(b => b.BreweryType.ToLower() == by_type.ToLower().Trim());
            }

            return filter;
        }

        public Expression<Func<Brewery, bool>> BuildSearchQueryFilter(string query = null)
        {
            Expression<Func<Brewery, bool>> filter = b => true;
            var formattedQuery = query?.Trim();

            if (string.IsNullOrEmpty(formattedQuery))
            {
                return filter;
            }

            // by_name
            return filter.OrElse(b => b.Name.ToLower().Contains(formattedQuery));
        }
    }
}
