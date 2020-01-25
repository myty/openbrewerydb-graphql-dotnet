using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenBreweryDB.Data;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;

using DTO = OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.API.Controllers
{
    [Route("breweries")]
    public class BreweriesController : Controller
    {
        readonly IBreweryConductor _breweryConductor;
        readonly IBreweryFilterConductor _filterConductor;
        readonly IMapper _mapper;
        readonly IBreweryValidationConductor _validationConductor;

        public BreweriesController(
            IBreweryConductor breweryConductor,
            IBreweryFilterConductor filterConductor,
            IBreweryValidationConductor validationConductor,
            IMapper mapper)
        {
            _breweryConductor = breweryConductor;
            _filterConductor = filterConductor;
            _mapper = mapper;
            _validationConductor = validationConductor;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DTO.Brewery>> Index(
            [FromQuery] string by_city = null,
            [FromQuery] string by_name = null,
            [FromQuery] string by_state = null,
            [FromQuery] string by_tag = null,
            [FromQuery] string by_tags = null,
            [FromQuery] string by_type = null,
            [FromQuery] string sort = null,
            [FromQuery] int page = 1,
            [FromQuery] int per_page = 20
        )
        {
            // Validation
            if (!_validationConductor.CanSearch(by_state, by_type, out var errors))
            {
                return this.BadRequest();
            }

            // Filtering
            var filter = _filterConductor.BuildFilter(by_name, by_state, by_city, by_type, BuildTags(by_tag, by_tags));

            // Sorting
            if (!String.IsNullOrEmpty(sort?.Trim()))
            {
                var sortingList = sort.Split(',', StringSplitOptions.RemoveEmptyEntries);

                // TODO: Add sorting solution
            }

            // Return Results
            var dataResults = _breweryConductor.FindAll(
                filter: filter,
                skip: (page - 1) * per_page,
                take: per_page
            );

            return _mapper.Map<IEnumerable<Brewery>, List<DTO.Brewery>>(dataResults);
        }

        [HttpGet("{id}")]
        public ActionResult<DTO.Brewery> Get(long id)
        {
            var result = _breweryConductor.Find(id);

            if (result is null)
            {
                return NotFound();
            }

            return _mapper.Map<DTO.Brewery>(result);
        }

        [HttpPost]
        public ActionResult Create([FromBody] DTO.Brewery dto)
        {
            // Validation
            if (!_validationConductor.CanCreate(dto, out var errors))
            {
                return this.BadRequest();
            }

            var brewery = _breweryConductor.Create(_mapper.Map<Brewery>(dto));

            dto = _mapper.Map<DTO.Brewery>(brewery);

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public ActionResult Update(
            [FromRoute] long id,
            [FromBody] DTO.Brewery dto)
        {
            // Validation
            if (!_validationConductor.CanUpdate(id, dto, out var errors))
            {
                return this.BadRequest();
            }

            var brewery = _breweryConductor.Update(_mapper.Map<Brewery>(dto));

            dto = _mapper.Map<DTO.Brewery>(brewery);

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] long id)
        {
            if (!_breweryConductor.Delete(id))
            {
                return NotFound();
            }

            return Ok();
        }

        IEnumerable<string> BuildTags(string by_tag, string by_tags)
        {
            // by_tag
            var tags = new List<string>();

            if (!String.IsNullOrEmpty(by_tag?.Trim()))
            {
                tags.Add(by_tag.Trim());
            }

            // by_tags
            if (!String.IsNullOrEmpty(by_tags?.Trim()))
            {
                tags.AddRange(by_tags.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                tags = tags.Distinct().ToList();
            }

            return tags;
        }
    }
}
