using System.Collections.Generic;
using OpenBreweryDB.API.Controllers.Dto;
using OpenBreweryDB.API.Core.Dto;

namespace OpenBreweryDB.API.Data.Models
{
    public class TagDto : BaseDto
    {
        public string Name { get; set; }

        public List<BreweryDto> Breweries { get; set; }
    }
}