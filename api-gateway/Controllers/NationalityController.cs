using api_gateway.Models.DTO.Nationality;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace api_gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalityController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public NationalityController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthController> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            this._logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<NationalityReadDto>>> GetNationalities()
        {
            try
            {
                var baseUrl = this._configuration["Services:PlayersService"] ?? "http://players-service:8080";

                var client = this._httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync("/api/nationality");

                if (!response.IsSuccessStatusCode)
                {
                    this._logger.LogError("Error al obtener nacionalidades: {StatusCode}", response.StatusCode);
                    return StatusCode(500, "Error al obtener nacionalidades");
                }

                var content = await response.Content.ReadAsStringAsync();

                var nationalities = JsonSerializer.Deserialize<List<NationalityReadDto>>(content) ?? new();

                return Ok(nationalities);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al obtener nacionalidades");
                return StatusCode(500, "Error interno al obtener nacionalidades");
            }
        }
    }
}
