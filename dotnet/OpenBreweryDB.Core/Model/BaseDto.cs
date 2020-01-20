using System.Text.Json.Serialization;

namespace OpenBreweryDB.Core.Model
{
    public class BaseDto
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }
    }
}