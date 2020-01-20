using OpenBreweryDB.Core.Models;
using System.Text.Json.Serialization;

namespace OpenBreweryDB.Core.Models
{
    public class AutocompleteBrewery : BaseDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}