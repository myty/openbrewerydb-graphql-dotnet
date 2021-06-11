using System;
using System.Linq;
using System.Linq.Expressions;
using AndcultureCode.CSharp.Core.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.Tags.Interfaces
{
    public interface ITagConductor
    {
        IResult<Tag> Find(long id);

        IResult<IQueryable<Tag>> FindAll(
            Expression<Func<Tag, bool>> filter = null,
            Func<IQueryable<Tag>, IQueryable<Tag>> orderBy = null,
            int skip = default,
            int take = 100);
    }
}
