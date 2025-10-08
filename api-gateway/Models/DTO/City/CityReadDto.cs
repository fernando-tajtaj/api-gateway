using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.City
{
    public class CityReadDto
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
