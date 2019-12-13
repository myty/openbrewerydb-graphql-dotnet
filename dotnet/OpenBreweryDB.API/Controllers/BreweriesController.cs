using System.Net.Http.Headers;
using System.Linq.Expressions;
using System;

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OpenBreweryDB.API.Data.Core;
using OpenBreweryDB.API.Data.Models;
using OpenBreweryDB.API.Extensions;
using OpenBreweryDB.API.Controllers.Dto;
using AutoMapper;

namespace OpenBreweryDB.API.Controllers
{
    [Route("[controller]")]
    public class BreweriesController : Controller
    {
        private readonly BreweryDbContext _context;
        private readonly IMapper _mapper;

        public BreweriesController(BreweryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: breweries
        [HttpGet]
        public ActionResult<IEnumerable<BreweryDto>> Index(
            [FromRoute] string by_city = null,
            [FromRoute] string by_name = null,
            [FromRoute] string by_state = null,
            [FromRoute] string by_tag = null,
            [FromRoute] string by_tags = null,
            [FromRoute] string by_type = null,
            [FromRoute] string sort = null,
            [FromRoute] int page = 1,
            [FromRoute] int per_page = 20
        )
        {
            // Validation
            if (ValidationFailure(by_city, by_name, by_state, by_tag, by_tags, by_type, out var errors))
            {
                return this.BadRequest();
            }

            // Filtering
            var filter = BuildFilter(by_city, by_name, by_state, by_tag, by_tags, by_type);

            // Sorting
            if (!String.IsNullOrEmpty(sort?.Trim()))
            {
                var sortingList = sort.Split(',', StringSplitOptions.RemoveEmptyEntries);

                // TODO: Add sorting solution
            }

            // Return Results
            var dataResults = _context.Breweries
                .Where(filter)
                .OrderBy(b => b.Name)
                .Skip((page - 1) * per_page)
                .Take(per_page)
                .ToList();

            return _mapper.Map<List<BreweryDto>>(dataResults);
        }

        bool ValidationFailure(
            string by_name,
            string by_state,
            string by_tag,
            string by_city,
            string by_tags,
            string by_type,
            out string[] errors)
        {
            errors = Array.Empty<string>();

            return true;
        }

        Expression<Func<Brewery, bool>> BuildFilter(
            string by_name = null,
            string by_state = null,
            string by_tag = null,
            string by_city = null,
            string by_tags = null,
            string by_type = null)
        {

            Expression<Func<Brewery, bool>> filter = b => true;

            // by_city
            if (!String.IsNullOrEmpty(by_city?.Trim()))
            {
                filter = filter.AndAlso(b => b.City.ToLowerInvariant().Contains(by_city.Trim()));
            }

            // by_name
            if (!String.IsNullOrEmpty(by_name?.Trim()))
            {
                filter = filter.AndAlso(b => b.Name.ToLowerInvariant().Contains(by_name.Trim()));
            }

            // by_state
            if (!String.IsNullOrEmpty(by_state?.Trim()))
            {
                filter = filter.AndAlso(b => b.State.ToLowerInvariant().Replace(" ", "_") == by_state.Trim());
            }

            // by_tag
            var tags = new List<string>();

            if (!String.IsNullOrEmpty(by_tag?.Trim()))
            {
                tags.Add(by_tag.Trim());
            }

            // by_tags
            if (!String.IsNullOrEmpty(by_tags?.Trim()))
            {
                tags.AddRange(by_tags.Split(',', StringSplitOptions.RemoveEmptyEntries));
                tags = tags.Distinct().ToList();
            }

            if (tags.Any())
            {
                filter = filter.AndAlso(b => b.BreweryTags.Select(bt => bt.Tag.Name).Union(tags).Any());
            }

            // by_type
            if (!String.IsNullOrEmpty(by_type?.Trim()))
            {
                filter = filter.AndAlso(b => b.BreweryType.ToLowerInvariant() == by_type.Trim());
            }

            return filter;
        }
    }
}