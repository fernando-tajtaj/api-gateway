using api_gateway.Models.DTO.City;
using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Team
{
    public class TeamReadDto
    {
        [JsonPropertyName("uuid")]
        public required string Uuid { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("city")]
        public required CityReadDto City { get; set; }
        [JsonPropertyName("logo")]
        public string Logo { get; set; } = string.Empty;
    }
}
