using api_gateway.Models.DTO.MatchPlayer;
using api_gateway.Models.DTO.Status;
using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Match
{
    public class MatchReadDto
    {
        [JsonPropertyName("uuid")]
        public required string Uuid { get; set; }

        [JsonPropertyName("uuidTeamHome")]
        public required string UuidTeamHome { get; set; } = string.Empty;

        [JsonPropertyName("teamHome")]
        public required string TeamHomeName { get; set; } = string.Empty;

        [JsonPropertyName("uuidTeamAway")]
        public required string UuidTeamAway { get; set; } = string.Empty;

        [JsonPropertyName("teamAway")]
        public required string TeamAwayName { get; set; } = string.Empty;

        [JsonPropertyName("scoreHome")]
        public short ScoreHome { get; set; }

        [JsonPropertyName("scoreAway")]
        public short ScoreAway { get; set; }

        [JsonPropertyName("foulHome")]
        public short FoulHome { get; set; }

        [JsonPropertyName("foulAway")]
        public short FoulAway { get; set; }

        [JsonPropertyName("quarter")]
        public short Quarter { get; set; }

        [JsonPropertyName("matchDateTime")]
        public required DateTime matchDateTime { get; set; }

        [JsonPropertyName("status")]
        public required StatusReadDto Status { get; set; }

        [JsonPropertyName("players")]
        public List<MatchPlayerReadDto>? MatchPlayerReadDto { get; set; } = new();
    }
}
