using System;
using System.Linq;
using System.Linq.Expressions;
using AndcultureCode.CSharp.Core;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using OpenBreweryDB.Core.Conductors.BreweryTags.Interfaces;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.BreweryTags
{
    public class BreweryTagConductor : IBreweryTagConductor
    {
        readonly BreweryDbContext _data;

        public BreweryTagConductor(BreweryDbContext data)
        {
            _data = data;
        }

        public IResult<BreweryTag> Find(long id) => Do<BreweryTag>.Try((r) =>
        {
            var tagResult = FindAll(b => b.Id == id, take: 1);

            if (tagResult.HasErrors || tagResult.ResultObject is null)
            {
                r.AddError("Not Found", $"The brewery (id: {id}) was not found.");
            }

            return tagResult.ResultObject.SingleOrDefault();
        }).Result;

        public IResult<IQueryable<BreweryTag>> FindAll(
            Expression<Func<BreweryTag, bool>> filter = null,
            Func<IQueryable<BreweryTag>, IQueryable<BreweryTag>> orderBy = null,
            int skip = 0,
            int take = 100) => Do<IQueryable<BreweryTag>>.Try((r) =>
        {
            var query = FindAllQueryable(filter, orderBy);

            if (query.HasErrorsOrResultIsNull())
            {
                r.AddError("Error", $"Results for the brewery query could not be retrieved.");
            }

            return query.ResultObject
                .Skip(skip)
                .Take(take);
        }).Result;

        public IResult<IQueryable<BreweryTag>> FindAllQueryable(
            Expression<Func<BreweryTag, bool>> filter = null,
            Func<IQueryable<BreweryTag>, IQueryable<BreweryTag>> orderBy = null
        ) => Do<IQueryable<BreweryTag>>.Try((r) =>
        {
            if (filter == null)
            {
                filter = b => true;
            }

            var query = _data.BreweryTags.Where(filter);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query;
        }).Result;
    }
}
