using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Team
{
    public class TeamCreateDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("uuidCity")]
        public string? UuidCity { get; set; }
        [JsonPropertyName("logo")]
        public string Logo { get; set; } = string.Empty;
        [JsonPropertyName("createdby")]
        public string? CreatedBy { get; set; }
    }
}
