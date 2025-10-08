using api_gateway.Models.DTO.User;
using System.Text.Json.Serialization;

namespace api_gateway.Models.DTO.Login
{
    public class LoginResponseDto
    {
        [JsonPropertyName("result")]
        public bool Result { get; set; }
        [JsonPropertyName("message")]
        public required string Message { get; set; }
        [JsonPropertyName("token")]
        public required string Token { get; set; }
        [JsonPropertyName("user")]
        public required UserResponseDto User { get; set; }
    }
}
