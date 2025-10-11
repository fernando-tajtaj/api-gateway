using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Player
{
    public class PlayerUpdateDto
    {
        [JsonPropertyName("uuidTeam")]
        public string UuidTeam { get; set; } = string.Empty;
        [JsonPropertyName("team")]
        public string Team { get; set; } = string.Empty;
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;
        [JsonPropertyName("number")]
        public int? Number { get; set; }
        [JsonPropertyName("uuidPosition")]
        public string? UuidPosition { get; set; }
        [JsonPropertyName("height")]
        public decimal? Height { get; set; }
        [JsonPropertyName("age")]
        public int? Age { get; set; }
        [JsonPropertyName("uuidNationality")]
        public string? UuidNationality { get; set; }
        [JsonPropertyName("updateby")]
        public string? UpdateBy { get; set; }
    }
}
