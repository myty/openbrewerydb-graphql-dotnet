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
            string includeProperties = null,
            Func<IQueryable<Brewery>, IQueryable<Brewery>> orderBy = null,
            int skip = default,
            int take = 100);

        IResult<IQueryable<Brewery>> FindAllByLocation(
            decimal latitude,
            decimal longitude,
            int? mileRadius = null);

        IResult<IQueryable<Brewery>> FindAllQueryable(
            Expression<Func<Brewery, bool>> filter = null,
            string includeProperties = null,
            Func<IQueryable<Brewery>, IQueryable<Brewery>> orderBy = null);

        IResult<Brewery> Create(Brewery brewery);
        IResult<IEnumerable<Brewery>> BulkCreate(IEnumerable<Brewery> breweries);
        IResult<Brewery> Update(Brewery brewery);
        IResult<bool> Delete(long id);
    }
}
