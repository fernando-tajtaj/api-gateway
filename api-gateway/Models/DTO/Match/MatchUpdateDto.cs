using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Match
{
    public class MatchUpdateDto
    {
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

        [JsonPropertyName("uuidStatus")]
        public string UuidStatus { get; set; } = string.Empty;

        [JsonPropertyName("updatedBy")]
        public string? UpdatedBy { get; set; }
    }
}
