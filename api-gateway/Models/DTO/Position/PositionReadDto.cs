using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Position
{
    public class PositionReadDto
    {
        [JsonPropertyName("uuid")]
        public required string Uuid { get; set; }
        [JsonPropertyName("description")]
        public required string Description { get; set; }
    }
}
