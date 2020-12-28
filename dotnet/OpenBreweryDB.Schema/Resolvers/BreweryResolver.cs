using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AndcultureCode.CSharp.Core.Enumerations;
using AndcultureCode.CSharp.Core.Interfaces;
using AndcultureCode.CSharp.Core.Models.Errors;
using AndcultureCode.CSharp.Extensions;
using GraphQL;
using GraphQL.Builders;
using OpenBreweryDB.Core.Conductors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Schema.Resolvers
{
    public class BreweryResolver
    {
        private readonly IBreweryConductor _breweryConductor;
        private readonly IBreweryFilterConductor _filterConductor;
        private readonly IBreweryOrderConductor _orderConductor;
        private readonly IBreweryValidationConductor _validationConductor;

        public BreweryResolver(
            IBreweryConductor breweryConductor,
            IBreweryFilterConductor filterConductor,
            IBreweryOrderConductor orderConductor,
            IBreweryValidationConductor validationConductor)
        {
            _breweryConductor = breweryConductor;
            _filterConductor = filterConductor;
            _orderConductor = orderConductor;
            _validationConductor = validationConductor;
        }

        public IResult<IQueryable<Brewery>> ResolveBreweries(IResolveFieldContext context)
        {
            if (ValidationFails(context, out var errorResult))
            {
                return errorResult;
            }

            return _breweryConductor
                .FindAllQueryable(
                    filter: BuildFilter(context),
                    orderBy: BuildOrderBy(context))
                .Include(
                    nameof(Brewery.BreweryTags),
                    context.ContainsField("tag_list"));
        }

        public IResult<IQueryable<Brewery>> ResolveNearbyBreweries(IResolveConnectionContext context)
        {
            var latitude = GetLatitude(context);
            var longitude = GetLongitude(context);

            if (latitude.HasValue && longitude.HasValue)
            {
                var within = context.GetArgument("within", 25);

                return _breweryConductor
                    .FindAllByLocation(
                        latitude.Value,
                        longitude.Value,
                        within)
                    .Include(
                        nameof(Brewery.BreweryTags),
                        context.ContainsField("tag_list"));
            }

            return new Result<IQueryable<Brewery>>(Enumerable.Empty<Brewery>().AsQueryable());
        }

        private decimal? GetLatitude(IResolveFieldContext context)
        {
            if (context is IResolveConnectionContext<Brewery> breweryContext)
            {
                return breweryContext.Source.Latitude;
            }

            return context.GetArgument<decimal?>("latitude");
        }

        private decimal? GetLongitude(IResolveFieldContext context)
        {
            if (context is IResolveConnectionContext<Brewery> breweryContext)
            {
                return breweryContext.Source.Longitude;
            }

            return context.GetArgument<decimal?>("longitude");
        }

        private Expression<Func<Brewery, bool>> BuildFilter(IResolveFieldContext context)
        {
            var search = context.GetArgument<string>("search");
            var filter = BuildMainFilter(context);

            if (!string.IsNullOrEmpty(search))
            {
                filter = filter.AndAlso(_filterConductor
                    .BuildFilter(by_name: search));
            }

            return filter;
        }

        private Expression<Func<Brewery, bool>> BuildMainFilter(IResolveFieldContext context)
        {
            var breweryId = context.GetArgument<string>("brewery_id");
            var city = context.GetArgument<string>("city");
            var name = context.GetArgument<string>("name");
            var state = context.GetArgument<string>("state");
            var tags = context.GetArgument<List<string>>("tags");
            var type = context.GetArgument<string>("type");

            if (!string.IsNullOrEmpty(breweryId?.Trim()))
            {
                return (b) => b.BreweryId == breweryId;
            }

            return _filterConductor.BuildFilter(
                by_name: name,
                by_state: state,
                by_type: type,
                by_city: city,
                by_tags: tags);
        }

        private Func<IQueryable<Brewery>, IQueryable<Brewery>> BuildOrderBy(IResolveFieldContext context)
        {
            var sort = context.GetArgument<List<string>>("sort");

            Func<IQueryable<Brewery>, IQueryable<Brewery>> orderBy = null;
            if (sort != null)
            {
                orderBy = _orderConductor.OrderByFields(
                    sort?
                        .Select(s => s.FirstOrDefault() == '-'
                            ? new KeyValuePair<string, SortDirection>(s.Substring(1), SortDirection.DESC)
                            : new KeyValuePair<string, SortDirection>(s, SortDirection.ASC))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                );
            }

            return orderBy;
        }

        private bool ValidationFails(IResolveFieldContext context, out IResult<IQueryable<Brewery>> errorResult)
        {
            var state = context.GetArgument<string>("state");
            var type = context.GetArgument<string>("type");

            if (!_validationConductor.CanSearch(state, type, out var errors))
            {
                errorResult = new Result<IQueryable<Brewery>>
                {
                    Errors = errors
                        .Select(err => new Error
                        {
                            ErrorType = ErrorType.ValidationError,
                            Key = err.key,
                            Message = err.message
                        })
                        .ToList<IError>()
                };

                return true;
            }

            errorResult = null;

            return false;
        }
    }
}
