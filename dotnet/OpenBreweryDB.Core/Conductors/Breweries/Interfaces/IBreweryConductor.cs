using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries.Interfaces
{
    public interface IBreweryConductor
    {
        Brewery Find(long id);
        IEnumerable<Brewery> FindAll(
            Expression<Func<Brewery, bool>> filter = null,
            Expression<Func<Brewery, object>> orderBy = null,
            int skip = default,
            int take = 100);
        Brewery Create(Brewery brewery);
        Brewery Update(Brewery brewery);
        bool Delete(long id);
    }
}
