using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AndcultureCode.CSharp.Core;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using GeoCoordinatePortable;
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

        public IResult<IEnumerable<Brewery>> BulkCreate(IEnumerable<Brewery> breweries) => Do<IEnumerable<Brewery>>.Try((r) =>
        {
            breweries = breweries.Select(b => ProcessBrewery(b));

            _data.Breweries.AddRange(breweries);
            _data.SaveChanges();

            return breweries;
        }).Result;

        public IResult<Brewery> Create(Brewery brewery) => Do<Brewery>.Try((r) =>
        {
            brewery = ProcessBrewery(brewery);

            _ = _data.Breweries.Add(brewery);
            _ = _data.SaveChanges();

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
            string includeProperties = null,
            Func<IQueryable<Brewery>, IQueryable<Brewery>> orderBy = null,
            int skip = 0,
            int take = 100) => Do<IEnumerable<Brewery>>.Try((r) =>
        {
            var query = FindAllQueryable(filter, includeProperties, orderBy);

            if (query.HasErrorsOrResultIsNull())
            {
                r.AddError("Error", $"Results for the brewery query could not be retrieved.");
            }

            return query.ResultObject
                .Skip(skip)
                .Take(take);
        }).Result;

        public IResult<IQueryable<Brewery>> FindAllByLocation(
            decimal latitude,
            decimal longitude,
            int? mileRadius = null) => Do<IQueryable<Brewery>>.Try((r) =>
        {
            var breweryResult = FindAllQueryable(b => b.Latitude.HasValue && b.Longitude.HasValue);
            var currentCoordinates = new GeoCoordinate(Convert.ToDouble(latitude), Convert.ToDouble(longitude));

            if (breweryResult.HasErrorsOrResultIsNull())
            {
                return r.AddErrorsAndReturnDefault(breweryResult);
            }

            return breweryResult.ResultObject
                .AsEnumerable()
                .Select(b => new
                {
                    Brewery = b,
                    DistanceFrom = Math.Abs(new GeoCoordinate(Convert.ToDouble(b.Latitude), Convert.ToDouble(b.Longitude)).GetDistanceTo(currentCoordinates)),
                })
                .OrderBy(b => b.DistanceFrom)
                .Where(b => !mileRadius.HasValue || (0.00062137 * b.DistanceFrom) <= mileRadius)
                .Select(b => b.Brewery)
                .AsQueryable();
        }).Result;


        public IResult<IQueryable<Brewery>> FindAllQueryable(
            Expression<Func<Brewery, bool>> filter = null,
            string includeProperties = null,
            Func<IQueryable<Brewery>, IQueryable<Brewery>> orderBy = null
        ) => Do<IQueryable<Brewery>>.Try((r) =>
        {
            if (filter == null)
            {
                filter = b => true;
            }

            var query = _data.Breweries.Include($"{nameof(Brewery.BreweryTags)}").Where(filter);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (!string.IsNullOrEmpty(includeProperties?.Trim()))
            {
                query = query.Include(includeProperties);
            }

            return query;
        }).Result;

        public IResult<Brewery> Update(Brewery brewery) => Do<Brewery>.Try((r) =>
        {
            var now = DateTime.Now;
            brewery.UpdatedAt = now;

            _data.Breweries.Update(brewery);
            _data.SaveChanges();

            return brewery;
        }).Result;

        private Brewery ProcessBrewery(Brewery brewery)
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

            return brewery;
        }
    }
}
