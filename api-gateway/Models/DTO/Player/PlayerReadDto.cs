using api_gateway.Models.DTO.Nationality;
using api_gateway.Models.DTO.Position;
using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Player
{
    public class PlayerReadDto
    {
        [JsonPropertyName("uuid")]
        public required string Uuid { get; set; }
        [JsonPropertyName("uuidTeam")]
        public string UuidTeam { get; set; } = string.Empty;
        [JsonPropertyName("team")]
        public string? TeamName { get; set; }
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;
        [JsonPropertyName("number")]
        public int? Number { get; set; }
        [JsonPropertyName("position")]
        public required PositionReadDto Position { get; set; }
        [JsonPropertyName("height")]
        public decimal? Height { get; set; }
        [JsonPropertyName("age")]
        public int? Age { get; set; }
        [JsonPropertyName("nationality")]
        public required NationalityReadDto Nationality { get; set; }
    }
}
