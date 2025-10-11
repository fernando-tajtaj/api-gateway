using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.MatchPlayer
{
    public class MatchPlayerCreateDto
    {
        [JsonPropertyName("uuidPlayer")]
        public required string UuidPlayer { get; set; }

        [JsonPropertyName("playerName")]
        public required string PlayerName { get; set; }

        [JsonPropertyName("isStarter")]
        public required bool IsStarter { get; set; }
    }
}
