using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTO = OpenBreweryDB.Core.Model;
using OpenBreweryDB.Data.Models;
using OpenBreweryDB.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenBreweryDB.Data;

namespace OpenBreweryDB.API.Controllers
{
    [Route("breweries/search")]
    public class SearchController : Controller
    {
        private readonly BreweryDbContext _context;
        private readonly IMapper _mapper;

        public SearchController(BreweryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DTO.Brewery>> Index(
            [FromQuery] string query = null,
            [FromQuery] string sort = null,
            [FromQuery] int page = 1,
            [FromQuery] int per_page = 20
        )
        {
            // Filtering
            var filter = BuildFilter(query);

            // Return Results
            var dataResults = _context.Breweries
                .Include(b => b.BreweryTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(filter)
                .OrderBy(b => b.Name)
                .Skip((page - 1) * per_page)
                .Take(per_page);

            return _mapper.Map<IEnumerable<Brewery>, List<DTO.Brewery>>(dataResults);
        }

        Expression<Func<Brewery, bool>> BuildFilter(string query = null)
        {
            Expression<Func<Brewery, bool>> filter = b => true;
            var formattedQuery = query?.Trim();

            if (String.IsNullOrEmpty(formattedQuery))
            {
                return filter;
            }

            return filter
                // by_city
                .OrElse(b => b.City.ToLower().Contains(formattedQuery))

                // by_name
                .OrElse(b => b.Name.ToLower().Contains(formattedQuery))

                // by_tag
                .OrElse(b => b.BreweryTags.Select(bt => bt.Tag.Name).Any(t => t.Contains(formattedQuery)))

                // by_type
                .OrElse(b => b.BreweryType.ToLower().Contains(formattedQuery));
        }
    }
}