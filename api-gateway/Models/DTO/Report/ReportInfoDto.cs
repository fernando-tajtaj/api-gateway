using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Report
{
    public class ReportInfoDto
    {
        [JsonPropertyName("report")]
        public  required ReportBaseDto ReportBaseDto { get; set; }

        [JsonPropertyName("data")]
        public List<Dictionary<string, object>>? Data { get; set; }
    }
}
