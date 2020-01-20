using OpenBreweryDB.Core.Model;
using System.Text.Json.Serialization;

namespace OpenBreweryDB.Core.Model
{
    public class AutocompleteBrewery : BaseDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}