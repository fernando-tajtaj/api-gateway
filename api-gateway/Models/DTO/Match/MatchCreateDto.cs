using api_gateway.Models.DTO.MatchPlayer;
using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Match
{
    public class MatchCreateDto
    {
        [JsonPropertyName("uuidTeamHome")]
        public string UuidTeamHome { get; set; } = string.Empty;

        [JsonPropertyName("teamHome")]
        public string TeamHome { get; set; } = string.Empty;

        [JsonPropertyName("uuidTeamAway")]
        public string UuidTeamAway { get; set; } = string.Empty;

        [JsonPropertyName("teamAway")]
        public string TeamAway { get; set; } = string.Empty;

        [JsonPropertyName("matchDateTime")]
        public DateTime MatchDateTime { get; set; }

        [JsonPropertyName("uuidStatus")]
        public required string UuidStatus { get; set; }

        [JsonPropertyName("players")]
        public List<MatchPlayerCreateDto>? MatchPlayerCreateDto { get; set; } = new();

        [JsonPropertyName("createdBy")]
        public required string CreatedBy { get; set; }
    }
}
