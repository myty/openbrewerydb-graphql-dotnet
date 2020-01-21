using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTO = OpenBreweryDB.Core.Models;
using OpenBreweryDB.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenBreweryDB.Data;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;

namespace OpenBreweryDB.API.Controllers
{
    [Route("breweries")]
    public class BreweriesController : Controller
    {
        private readonly BreweryDbContext _context;
        private readonly IMapper _mapper;
        readonly IBreweryFilterConductor _filterConductor;

        public BreweriesController(BreweryDbContext context, IMapper mapper, IBreweryFilterConductor filterConductor)
        {
            _context = context;
            _mapper = mapper;
            _filterConductor = filterConductor;
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
            if (HasValidationErrors(by_state, by_type, out var errors))
            {
                return this.BadRequest();
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
                tags.AddRange(by_tags.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                tags = tags.Distinct().ToList();
            }

            // Filtering
            var filter = _filterConductor.BuildFilter(by_name, by_state, by_city, by_type, tags);

            // Sorting
            if (!String.IsNullOrEmpty(sort?.Trim()))
            {
                var sortingList = sort.Split(',', StringSplitOptions.RemoveEmptyEntries);

                // TODO: Add sorting solution
            }

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

        [HttpGet("{id}")]
        public ActionResult<DTO.Brewery> Get(long id)
        {
            var result = _context.Breweries
                .Include(b => b.BreweryTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(b => b.BreweryId == id)
                .FirstOrDefault();

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
            if (HasValidationErrors(dto, out var errors))
            {
                return this.BadRequest();
            }

            var brewery = _mapper.Map<Brewery>(dto);

            var now = DateTime.Now;
            brewery.CreatedAt = now;
            brewery.UpdatedAt = now;

            var existingTags = _context.Tags
                .Where(t => dto.Tags.Contains(t.Name))
                .ToList();

            var existingTagNames = existingTags
                .Select(t => t.Name)
                .ToList();

            var tagsToCreate = dto.Tags
                .Where(t => !existingTagNames.Contains(t))
                .Select(t => new Tag { Name = t })
                .ToList();

            var breweryTags = tagsToCreate
                .Concat(existingTags)
                .Select(t => new BreweryTag
                {
                    Brewery = brewery,
                    Tag = t
                })
                .ToList();

            _context.BreweryTags.AddRange(breweryTags);
            _context.Tags.AddRange(tagsToCreate);
            _context.Breweries.Add(brewery);

            _context.SaveChanges();

            dto = _mapper.Map<DTO.Brewery>(brewery);

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public ActionResult Update(
            [FromRoute] long id,
            [FromBody] DTO.Brewery dto)
        {
            // Validation
            if (HasPutValidationErrors(id, dto, out var errors))
            {
                return this.BadRequest();
            }

            var brewery = _mapper.Map<Brewery>(dto);

            var now = DateTime.Now;
            brewery.UpdatedAt = now;

            _context.Breweries.Update(brewery);
            _context.SaveChanges();

            dto = _mapper.Map<DTO.Brewery>(brewery);

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] long id)
        {
            var foundBrewery = _context.Breweries.FirstOrDefault(b => b.BreweryId == id);

            if (!(foundBrewery is null))
            {
                _context.Breweries.Remove(foundBrewery);
                _context.SaveChanges();

                return Ok();
            }

            return NotFound();
        }

        bool HasValidationErrors(
            string by_state,
            string by_type,
            out string[] errors)
        {
            var errorList = new List<string>();

            if (!String.IsNullOrEmpty(by_state?.Trim()) && by_state.Contains(" "))
            {
                errorList.Add("by_state must contain the full state name in snake_case, no abbreviation (ex. \"new_york\")");
            }

            var allowedTypes = new string[] { "micro", "regional", "brewpub", "large", "planning", "bar", "contract", "proprietor" };
            if (!String.IsNullOrEmpty(by_type?.Trim()) && !allowedTypes.Contains(by_type))
            {
                errorList.Add("by_type must be one of the following: micro, regional, brewpub, large, planning, bar, contract, proprietor.");
            }

            errors = errorList.ToArray();

            return errorList.Any();
        }

        bool HasValidationErrors(DTO.Brewery dto, out string[] errors)
        {
            var errorList = new List<string>();

            var allowedTypes = new string[] { "micro", "regional", "brewpub", "large", "planning", "bar", "contract", "proprietor" };
            if (String.IsNullOrEmpty(dto.BreweryType?.Trim()) || !allowedTypes.Contains(dto.BreweryType))
            {
                errorList.Add("BreweryType must be one of the following: micro, regional, brewpub, large, planning, bar, contract, proprietor.");
            }

            errors = errorList.ToArray();

            return errorList.Any();
        }

        bool HasPutValidationErrors(long id, DTO.Brewery dto, out string[] errors)
        {
            var errorList = new List<string>();

            if (id != dto.Id)
            {
                errorList.Add("Ids for the object and route must match");
            }

            var allowedTypes = new string[] { "micro", "regional", "brewpub", "large", "planning", "bar", "contract", "proprietor" };
            if (String.IsNullOrEmpty(dto.BreweryType?.Trim()) || !allowedTypes.Contains(dto.BreweryType))
            {
                errorList.Add("BreweryType must be one of the following: micro, regional, brewpub, large, planning, bar, contract, proprietor.");
            }

            errors = errorList.ToArray();

            return errorList.Any();
        }
    }
}