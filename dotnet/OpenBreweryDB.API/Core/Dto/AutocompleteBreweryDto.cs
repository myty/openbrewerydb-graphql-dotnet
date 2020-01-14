using OpenBreweryDB.API.Core.Dto;
using System.Text.Json.Serialization;

namespace OpenBreweryDB.API.Controllers.Dto
{
    public class AutocompleteBreweryDto : BaseDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}