using AndcultureCode.CSharp.Core;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenBreweryDB.Core.Conductors.Breweries
{
    public class BreweryConductor : IBreweryConductor
    {
        readonly BreweryDbContext _data;

        public BreweryConductor(BreweryDbContext data)
        {
            _data = data;
        }

        public IResult<Brewery> Create(Brewery brewery) => Do<Brewery>.Try((r) =>
        {
            var now = DateTime.Now;
            brewery.CreatedAt = now;
            brewery.UpdatedAt = now;

            var tags = brewery.BreweryTags.Select(bt => bt.Tag.Name).ToList();

            var existingTags = _data.Tags
                .Where(t => tags.Contains(t.Name))
                .ToList();

            var existingTagNames = existingTags
                .Select(t => t.Name)
                .Distinct()
                .ToList();

            var tagsToCreate = tags
                .Where(t => !existingTagNames.Contains(t))
                .Select(t => new Tag { Name = t })
                .ToList();

            brewery.BreweryTags = tagsToCreate
                .Concat(existingTags)
                .Select(t => new BreweryTag
                {
                    Brewery = brewery,
                    Tag = t
                })
                .ToList();

            _data.Breweries.Add(brewery);

            _data.SaveChanges();

            return brewery;
        }).Result;

        public IResult<bool> Delete(long id) => Do<bool>.Try((r) =>
        {
            var brewery = _data.Breweries.Find(id);

            if (brewery == null)
            {
                r.AddError("Not Found", $"The brewery (id: {id}) was not found.");

                return false;
            }

            _data.Remove<Brewery>(brewery);
            _data.SaveChanges();

            return true;
        }).Result;

        public IResult<Brewery> Find(long id) => Do<Brewery>.Try((r) =>
        {
            var breweryResult = FindAll(b => b.Id == id, take: 1);

            if (breweryResult.HasErrors || breweryResult.ResultObject is null)
            {
                r.AddError("Not Found", $"The brewery (id: {id}) was not found.");
            }

            return breweryResult.ResultObject.SingleOrDefault();
        }).Result;

        public IResult<IEnumerable<Brewery>> FindAll(
            Expression<Func<Brewery, bool>> filter = null,
            Expression<Func<Brewery, object>> orderBy = null,
            int skip = 0,
            int take = 100) => Do<IEnumerable<Brewery>>.Try((r) =>
        {
            if (filter == null)
            {
                filter = b => true;
            }

            if (orderBy == null)
            {
                orderBy = b => b.Name;
            }

            return _data.Breweries
                .Include(b => b.BreweryTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(filter)
                .OrderBy(orderBy)
                .Skip(skip)
                .Take(take);
        }).Result;

        public IResult<Brewery> Update(Brewery brewery) => Do<Brewery>.Try((r) =>
        {
            var now = DateTime.Now;
            brewery.UpdatedAt = now;

            _data.Breweries.Update(brewery);
            _data.SaveChanges();

            return brewery;
        }).Result;
    }
}
