namespace api_gateway.Controllers
{
    using api_gateway.Models.DTO.Login;
    using api_gateway.Models.DTO.User;
    using Microsoft.AspNetCore.Mvc;
    using System.Text;
    using System.Text.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthController> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            this._logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var client = this._httpClientFactory.CreateClient();
                var authServiceUrl = this._configuration["Services:AuthService"] ?? "http://auth-service:4000";

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{authServiceUrl}/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return Ok(JsonSerializer.Deserialize<LoginResponseDto>(responseData));
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { message = errorContent });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error al intentar hacer login");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto request)
        {
            try
            {
                var client = this._httpClientFactory.CreateClient();
                var authServiceUrl = this._configuration["Services:AuthService"] ?? "http://auth-service:4000";

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{authServiceUrl}/api/auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return Ok(JsonSerializer.Deserialize<object>(responseData));
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { message = errorContent });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error al intentar registrar usuario");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidateToken([FromHeader(Name = "Authorization")] string authorization)
        {
            try
            {
                if (string.IsNullOrEmpty(authorization))
                {
                    return Unauthorized(new { message = "Token no proporcionado" });
                }

                var client = this._httpClientFactory.CreateClient();
                var authServiceUrl = this._configuration["Services:AuthService"] ?? "http://auth-service:4000";

                client.DefaultRequestHeaders.Add("Authorization", authorization);

                var response = await client.GetAsync($"{authServiceUrl}/api/auth/validate");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return Ok(JsonSerializer.Deserialize<object>(responseData));
                }

                return Unauthorized(new { message = "Token inválido" });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error al validar token");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}
