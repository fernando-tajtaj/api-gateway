using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Nationality
{
    public class NationalityReadDto
    {
        [JsonPropertyName("uuid")]
        public required string Uuid { get; set; }
        [JsonPropertyName("description")]
        public required string Description { get; set; }
    }
}
