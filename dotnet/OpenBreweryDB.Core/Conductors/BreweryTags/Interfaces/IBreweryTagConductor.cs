using System;
using System.Linq;
using System.Linq.Expressions;
using AndcultureCode.CSharp.Core.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.BreweryTags.Interfaces
{
    public interface IBreweryTagConductor
    {
        IResult<BreweryTag> Find(long id);

        IResult<IQueryable<BreweryTag>> FindAll(
            Expression<Func<BreweryTag, bool>> filter = null,
            Func<IQueryable<BreweryTag>, IQueryable<BreweryTag>> orderBy = null,
            int skip = default,
            int take = 100);
    }
}
