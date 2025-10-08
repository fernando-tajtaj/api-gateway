using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.User
{
    public class UserResponseDto
    {
        [JsonPropertyName("uuid")]
        public required string Uuid { get; set; }
        [JsonPropertyName("username")]
        public required string Username { get; set; }
        [JsonPropertyName("firstname")]
        public required string Firstname { get; set; }
        [JsonPropertyName("lastname")]
        public required string Lastname { get; set; }
        [JsonPropertyName("role")]
        public required string Role { get; set; }
    }
}
