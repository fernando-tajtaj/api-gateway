using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Report
{
    public class ReportBaseDto
    {
        [JsonPropertyName("uuid")]
        public required string Uuid { get; set; }

        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("description")]
        public required string Description { get; set; }

        [JsonPropertyName("tableName")]
        public required string TableName { get; set; }
    }
}
