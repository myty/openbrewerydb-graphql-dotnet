using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries
{
    public class BreweryOrderConductor : IBreweryOrderConductor
    {
        public Func<IQueryable<Brewery>, IQueryable<Brewery>> OrderByFields(IDictionary<string, SortDirection> fields)
        {
            return q => {
                IOrderedQueryable<Brewery> orderBy = null;

                if (fields?.Any() == true)
                {
                    foreach (var kvp in fields)
                    {
                        if (kvp.Key == "name")
                        {
                            orderBy = (orderBy is null)
                                ? OrderBy(q, (b) => b.Name, kvp.Value)
                                : ThenBy(orderBy, (b) => b.Name, kvp.Value);
                        }
                    }
                }

                return orderBy ?? q;
            };
        }

        private IOrderedQueryable<Brewery> OrderBy<T>(
            IQueryable<Brewery> brewery,
            Expression<Func<Brewery, T>> sortOptionExpression,
            SortDirection direction = SortDirection.ASC)
        {
            if (direction == SortDirection.ASC)
            {
                return brewery.OrderBy(sortOptionExpression);
            }

            return brewery.OrderByDescending(sortOptionExpression);
        }

        private IOrderedQueryable<Brewery> ThenBy<T>(
            IOrderedQueryable<Brewery> brewery,
            Expression<Func<Brewery, T>> sortOptionExpression,
            SortDirection direction = SortDirection.ASC)
        {
            if (direction == SortDirection.ASC)
            {
                return brewery.ThenBy(sortOptionExpression);
            }

            return brewery.ThenByDescending(sortOptionExpression);
        }
    }
}
