using System.Text.Json.Serialization;
using OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.Core.Models
{
    public class AutocompleteBrewery : BaseDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
