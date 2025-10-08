using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Login
{
    public class LoginRequestDto
    {
        [JsonPropertyName("username")]
        public required string Username { get; set; }
        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
