using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Status
{
    public class StatusReadDto
    {
        [JsonPropertyName("uuid")]
        public required string Uuid { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}
