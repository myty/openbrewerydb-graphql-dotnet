using AndcultureCode.CSharp.Core;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using AutoMapper;
using OpenBreweryDB.Core.Conductors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL
{
    public class Query
    {
        readonly IBreweryConductor _breweryConductor;
        readonly IBreweryFilterConductor _filterConductor;
        readonly IMapper _mapper;
        readonly IBreweryOrderConductor _orderConductor;
        readonly IBreweryValidationConductor _validationConductor;

        public Query(IBreweryConductor breweryConductor, IBreweryFilterConductor filterConductor, IBreweryOrderConductor orderConductor, IMapper mapper, IBreweryValidationConductor validationConductor)
        {
            _breweryConductor = breweryConductor;
            _filterConductor = filterConductor;
            _mapper = mapper;
            _orderConductor = orderConductor;
            _validationConductor = validationConductor;
        }

        public IResult<IEnumerable<DTO.Brewery>> GetBreweries(
            string by_state = null,
            string by_type = null,
            string city = null,
            string name = null,
            string search = null,
            string[] sort = null,
            string[] tags = null,
            int skip = 0,
            int limit = 10) => Do<IEnumerable<DTO.Brewery>>.Try((r) =>
        {
            if (!_validationConductor.CanSearch(by_state, by_type, out var errors))
            {
                foreach (var (key, message) in errors)
                {
                    r.AddValidationError(key, message);
                }

                return null;
            }

            var filter = _filterConductor.BuildFilter(
                by_name: name,
                by_state: by_state,
                by_type: by_type,
                by_city: city,
                by_tags: tags);

            if (!string.IsNullOrEmpty(search))
            {
                filter = filter.AndAlso(_filterConductor
                    .BuildFilter(by_name: search));
            }

            // Sorting
            Func<IQueryable<Entity.Brewery>, IQueryable<Entity.Brewery>> orderBy = null;
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

            var result = _breweryConductor.FindAll(filter: filter, orderBy: orderBy, skip: skip, take: limit);

            if (result.HasErrorsOrResultIsNull())
            {
                r.AddErrors(result.Errors);
                return null;
            }

            return _mapper.Map<IEnumerable<DTO.Brewery>>(result.ResultObject);
        }).Result;

        public IResult<DTO.Brewery> GetBrewery(long id) => Do<DTO.Brewery>.Try((r) =>
        {
            var breweryResult = _breweryConductor.Find(id);

            if (!breweryResult.HasErrorsOrResultIsNull())
            {
                return _mapper.Map<DTO.Brewery>(breweryResult.ResultObject);
            }

            r.AddErrors(breweryResult.Errors);
            return null;

        }).Result;
    }
}
