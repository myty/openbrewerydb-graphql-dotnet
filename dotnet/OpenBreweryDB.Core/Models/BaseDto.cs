using System.Text.Json.Serialization;

namespace OpenBreweryDB.Core.Models
{
    public class BaseDto
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }
    }
}
