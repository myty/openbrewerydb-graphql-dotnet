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
            string by_tag = null,
            string by_city = null,
            string by_tags = null,
            string by_type = null)
        {

            Expression<Func<Brewery, bool>> filter = b => true;

            // by_city
            if (!String.IsNullOrEmpty(by_city?.Trim()))
            {
                filter = filter.AndAlso(b => b.City.ToLower().Contains(by_city.Trim()));
            }

            // by_name
            if (!String.IsNullOrEmpty(by_name?.Trim()))
            {
                filter = filter.AndAlso(b => b.Name.ToLower().Contains(by_name.Trim()));
            }

            // by_state
            if (!String.IsNullOrEmpty(by_state?.Trim()))
            {
                filter = filter.AndAlso(b => b.State.ToLower().Replace(" ", "_") == by_state.Trim());
            }

            // by_tag
            var tags = new List<string>();

            if (!String.IsNullOrEmpty(by_tag?.Trim()))
            {
                tags.Add(by_tag.Trim());
            }

            // by_tags
            if (!String.IsNullOrEmpty(by_tags?.Trim()))
            {
                tags.AddRange(by_tags.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                tags = tags.Distinct().ToList();
            }

            if (tags.Any())
            {
                filter = filter.AndAlso(b => b.BreweryTags.Select(bt => bt.Tag.Name).Any(t => tags.Contains(t)));
            }

            // by_type
            if (!String.IsNullOrEmpty(by_type?.Trim()))
            {
                filter = filter.AndAlso(b => b.BreweryType.ToLower() == by_type.Trim());
            }

            return filter;
        }

        public Expression<Func<Brewery, bool>> BuildSearchQueryFilter(string query = null)
        {
            Expression<Func<Brewery, bool>> filter = b => true;
            var formattedQuery = query?.Trim();

            if (String.IsNullOrEmpty(formattedQuery))
            {
                return filter;
            }

            // by_name
            return filter.OrElse(b => b.Name.ToLower().Contains(formattedQuery));
        }
    }
}