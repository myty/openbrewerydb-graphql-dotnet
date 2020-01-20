using AutoMapper;
using DTO = OpenBreweryDB.Core.Models;
using Microsoft.AspNetCore.Mvc;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace OpenBreweryDB.API.Controllers
{
    [Route("breweries/autocomplete")]
    public class AutocompleteController : Controller
    {
        private readonly BreweryDbContext _context;
        private readonly IMapper _mapper;
        readonly IBreweryFilterConductor _filterConductor;

        public AutocompleteController(BreweryDbContext context, IMapper mapper, IBreweryFilterConductor filterConductor)
        {
            _context = context;
            _mapper = mapper;
            _filterConductor = filterConductor;
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
            var filter = _filterConductor.BuildAutocompleteQueryFilter(query);

            // Return Results
            var dataResults = _context.Breweries
                .Where(filter)
                .OrderBy(b => b.Name)
                .Skip((page - 1) * per_page)
                .Take(per_page);

            return _mapper.Map<IEnumerable<Brewery>, List<DTO.AutocompleteBrewery>>(dataResults);
        }
    }
}