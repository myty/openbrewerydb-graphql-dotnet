using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DTO = OpenBreweryDB.Core.Model;
using OpenBreweryDB.API.Extensions;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenBreweryDB.API.Controllers
{
    [Route("breweries/autocomplete")]
    public class AutocompleteController : Controller
    {
        private readonly BreweryDbContext _context;
        private readonly IMapper _mapper;

        public AutocompleteController(BreweryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DTO.AutocompleteBrewery>> Index(
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
                .Where(filter)
                .OrderBy(b => b.Name)
                .Skip((page - 1) * per_page)
                .Take(per_page);

            return _mapper.Map<IEnumerable<Brewery>, List<DTO.AutocompleteBrewery>>(dataResults);
        }

        Expression<Func<Brewery, bool>> BuildFilter(string query = null)
        {
            Expression<Func<Brewery, bool>> filter = b => true;
            var formattedQuery = query?.Trim();

            if (String.IsNullOrEmpty(formattedQuery))
            {
                return filter;
            }

            // by_name
            return filter.OrElse(b => b.Name.ToLower().Contains(formattedQuery));
        }
    }
}