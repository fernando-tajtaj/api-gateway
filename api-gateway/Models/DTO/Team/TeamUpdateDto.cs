using api_gateway.Models.DTO.City;
using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Team
{
    public class TeamUpdateDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("uuidCity")]
        public required string UuidCity { get; set; }
        [JsonPropertyName("logo")]
        public string Logo { get; set; } = string.Empty;
        [JsonPropertyName("updatedby")]
        public string? UpdatedBy { get; set; }
    }
}
