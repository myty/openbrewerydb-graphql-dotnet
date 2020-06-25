using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OpenBreweryDB.Core.Conductors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

using DTO = OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.API.Controllers
{
    [Route("api/breweries")]
    public class BreweriesController : Controller
    {
        readonly IBreweryConductor _breweryConductor;
        readonly IBreweryFilterConductor _filterConductor;
        readonly IMapper _mapper;
        readonly IBreweryOrderConductor _orderConductor;
        readonly IBreweryValidationConductor _validationConductor;

        public BreweriesController(
            IBreweryConductor breweryConductor,
            IBreweryFilterConductor filterConductor,
            IBreweryOrderConductor orderConductor,
            IBreweryValidationConductor validationConductor,
            IMapper mapper)
        {
            _breweryConductor = breweryConductor;
            _filterConductor = filterConductor;
            _mapper = mapper;
            _orderConductor = orderConductor;
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
                return BadRequest();
            }

            // Filtering
            var filter = _filterConductor.BuildFilter(by_name, by_state, by_city, by_type, BuildTags(by_tag, by_tags));

            // Sorting
            var orderBy = _orderConductor.OrderByFields(
                sort?
                    .Trim()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s =>
                    {
                        if (s.FirstOrDefault() == '-')
                        {
                            return new KeyValuePair<string, SortDirection>(s.Substring(1), SortDirection.DESC);
                        }

                        return new KeyValuePair<string, SortDirection>(s, SortDirection.ASC);
                    })
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            );

            // Return Results
            var dataResults = _breweryConductor.FindAll(
                filter: filter,
                orderBy: orderBy,
                skip: (page - 1) * per_page,
                take: per_page
            );

            if (dataResults.HasErrors || dataResults.ResultObject is null)
            {
                return BadRequest();
            }

            return _mapper.Map<IEnumerable<Brewery>, List<DTO.Brewery>>(dataResults.ResultObject);
        }

        [HttpGet("{id}")]
        public ActionResult<DTO.Brewery> Get(long id)
        {
            var result = _breweryConductor.Find(id);

            if (result.HasErrors || result.ResultObject is null)
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
                return BadRequest();
            }

            var brewery = _breweryConductor.Create(_mapper.Map<Brewery>(dto));

            if (brewery.HasErrors || brewery.ResultObject is null)
            {
                return BadRequest(brewery.Errors);
            }

            dto = _mapper.Map<DTO.Brewery>(brewery.ResultObject);

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(
            [FromRoute] long id,
            [FromBody] DTO.Brewery dto)
        {
            // Validation
            if (!_validationConductor.CanUpdate(id, dto, out _))
            {
                return BadRequest();
            }

            var brewery = _breweryConductor.Update(_mapper.Map<Brewery>(dto));

            if (brewery.HasErrors || brewery.ResultObject is null)
            {
                return BadRequest(brewery.Errors);
            }

            _ = _mapper.Map<DTO.Brewery>(brewery.ResultObject);

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] long id)
        {
            var result = _breweryConductor.Delete(id);

            if (result.HasErrors || !result.ResultObject)
            {
                return NotFound();
            }

            return Ok();
        }

        IEnumerable<string> BuildTags(string by_tag, string by_tags)
        {
            // by_tag
            var tags = new List<string>();

            if (!string.IsNullOrEmpty(by_tag?.Trim()))
            {
                tags.Add(by_tag.Trim());
            }

            // by_tags
            if (!string.IsNullOrEmpty(by_tags?.Trim()))
            {
                tags.AddRange(by_tags.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                tags = tags.Distinct().ToList();
            }

            return tags;
        }
    }
}
