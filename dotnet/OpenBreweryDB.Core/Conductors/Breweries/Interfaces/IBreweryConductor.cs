using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AndcultureCode.CSharp.Core.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries.Interfaces
{
    public interface IBreweryConductor
    {
        IResult<Brewery> Find(long id);
        IResult<IEnumerable<Brewery>> FindAll(
            Expression<Func<Brewery, bool>> filter = null,
            Func<IQueryable<Brewery>, IQueryable<Brewery>> orderBy = null,
            int skip = default,
            int take = 100);
        IResult<Brewery> Create(Brewery brewery);
        IResult<Brewery> Update(Brewery brewery);
        IResult<bool> Delete(long id);
    }
}
