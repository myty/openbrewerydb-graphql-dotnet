using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.API.Controllers.Dto;
using OpenBreweryDB.API.Data.Core;
using OpenBreweryDB.API.Data.Models;
using OpenBreweryDB.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenBreweryDB.API.Controllers
{
    [Route("breweries")]
    public class BreweriesController : Controller
    {
        private readonly BreweryDbContext _context;
        private readonly IMapper _mapper;

        public BreweriesController(BreweryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BreweryDto>> Index(
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

            // Filtering
            var filter = BuildFilter(by_name, by_state, by_tag, by_city, by_tags, by_type);

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

            return _mapper.Map<IEnumerable<Brewery>, List<BreweryDto>>(dataResults);
        }

        [HttpGet("{id}")]
        public ActionResult<BreweryDto> Get(long id)
        {
            var result = _context.Breweries
                .Where(b => b.BreweryId == id)
                .FirstOrDefault();

            if (result is null)
            {
                return NotFound();
            }

            return _mapper.Map<BreweryDto>(result);
        }

        [HttpPost]
        public ActionResult Create([FromBody] BreweryDto dto)
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

            dto = _mapper.Map<BreweryDto>(brewery);

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public ActionResult Update(
            [FromRoute] long id,
            [FromBody] BreweryDto dto)
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

            dto = _mapper.Map<BreweryDto>(brewery);

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

        bool HasValidationErrors(BreweryDto dto, out string[] errors)
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

        bool HasPutValidationErrors(long id, BreweryDto dto, out string[] errors)
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
                filter = filter.AndAlso(b => b.City.ToLower().Contains(by_city.Trim()));
            }

            // by_name
            if (!String.IsNullOrEmpty(by_name?.Trim()))
            {
                filter = filter.AndAlso(b => b.Name.ToLower().Contains(by_name.Trim()));
            }

            // by_state
            if (!String.IsNullOrEmpty(by_state?.Trim()))
            {
                filter = filter.AndAlso(b => b.State.ToLower().Replace(" ", "_") == by_state.Trim());
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
                filter = filter.AndAlso(b => b.BreweryTags.Select(bt => bt.Tag.Name).Any(t => tags.Contains(t)));
            }

            // by_type
            if (!String.IsNullOrEmpty(by_type?.Trim()))
            {
                filter = filter.AndAlso(b => b.BreweryType.ToLower() == by_type.Trim());
            }

            return filter;
        }
    }
}