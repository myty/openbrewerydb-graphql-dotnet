using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries
{
    public class BreweryConductor : IBreweryConductor
    {
        readonly BreweryDbContext _data;

        public BreweryConductor(BreweryDbContext data)
        {
            _data = data;
        }

        public Brewery Create(Brewery brewery)
        {
            var now = DateTime.Now;
            brewery.CreatedAt = now;
            brewery.UpdatedAt = now;

            var tags = brewery.BreweryTags.Select(bt => bt.Tag.Name);

            var existingTags = _data.Tags
                .Where(t => tags.Contains(t.Name))
                .ToList();

            var existingTagNames = existingTags
                .Select(t => t.Name)
                .ToList();

            var tagsToCreate = tags
                .Where(t => !existingTagNames.Contains(t))
                .Select(t => new Tag { Name = t })
                .ToList();

            var breweryTags = tagsToCreate
                .Concat(existingTags)
                .Select(t => new BreweryTag
                {
                    Brewery = brewery,
                    Tag = t
                })
                .ToList();

            _data.BreweryTags.AddRange(breweryTags);
            _data.Tags.AddRange(tagsToCreate);
            _data.Breweries.Add(brewery);

            _data.SaveChanges();

            return brewery;
        }

        public bool Delete(long id)
        {
            var brewery = _data.Breweries.Find(id);

            if (brewery == null)
            {
                return false;
            }

            _data.Remove<Brewery>(brewery);
            _data.SaveChanges();

            return true;
        }

        public Brewery Find(long id)
        {
            return FindAll(b => b.Id == id).SingleOrDefault();
        }

        public IEnumerable<Brewery> FindAll(
            Expression<Func<Brewery, bool>> filter = null,
            Expression<Func<Brewery, object>> orderBy = null,
            int skip = 0,
            int take = 100)
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
        }

        public Brewery Update(Brewery brewery)
        {
            var now = DateTime.Now;
            brewery.UpdatedAt = now;

            _data.Breweries.Update(brewery);
            _data.SaveChanges();

            return brewery;
        }
    }
}
