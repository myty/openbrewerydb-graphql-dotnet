using System.Text.Json.Serialization;

namespace OpenBreweryDB.API.Core.Dto
{
    public class BaseDto
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }
    }
}