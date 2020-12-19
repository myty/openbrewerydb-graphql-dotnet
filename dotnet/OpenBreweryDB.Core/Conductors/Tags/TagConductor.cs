
using System;
using System.Linq;
using System.Linq.Expressions;
using AndcultureCode.CSharp.Core;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using OpenBreweryDB.Core.Conductors.Tags.Interfaces;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.Tags
{
    public class TagConductor : ITagConductor
    {
        readonly BreweryDbContext _data;

        public TagConductor(BreweryDbContext data)
        {
            _data = data;
        }

        public IResult<Tag> Find(long id) => Do<Tag>.Try((r) =>
        {
            var tagResult = FindAll(b => b.Id == id, take: 1);

            if (tagResult.HasErrors || tagResult.ResultObject is null)
            {
                r.AddError("Not Found", $"The brewery (id: {id}) was not found.");
            }

            return tagResult.ResultObject.SingleOrDefault();
        }).Result;

        public IResult<IQueryable<Tag>> FindAll(
            Expression<Func<Tag, bool>> filter = null,
            Func<IQueryable<Tag>, IQueryable<Tag>> orderBy = null,
            int skip = 0,
            int take = 100) => Do<IQueryable<Tag>>.Try((r) =>
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

        public IResult<IQueryable<Tag>> FindAllQueryable(
            Expression<Func<Tag, bool>> filter = null,
            Func<IQueryable<Tag>, IQueryable<Tag>> orderBy = null
        ) => Do<IQueryable<Tag>>.Try((r) =>
        {
            if (filter == null)
            {
                filter = b => true;
            }

            var query = _data.Tags.Where(filter);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query;
        }).Result;
    }
}
