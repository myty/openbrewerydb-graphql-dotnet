using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;
using DTO = OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.API.Controllers
{
    [Route("api/breweries/search")]
    public class SearchController : Controller
    {
        readonly IBreweryConductor _breweryConductor;
        readonly IBreweryFilterConductor _filterConductor;
        private readonly IMapper _mapper;

        public SearchController(IBreweryConductor breweryConductor, IBreweryFilterConductor filterConductor, IMapper mapper)
        {
            _breweryConductor = breweryConductor;
            _filterConductor = filterConductor;
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
            var filter = _filterConductor.BuildSearchQueryFilter(query);

            // Return Results
            var dataResults = _breweryConductor.FindAll(
                filter: filter,
                skip: (page - 1) * per_page,
                take: per_page
            );

            if (dataResults.HasErrors || dataResults.ResultObject is null)
            {
                return BadRequest();
            }

            return _mapper.Map<IEnumerable<Brewery>, List<DTO.Brewery>>(dataResults.ResultObject);
        }
    }
}
