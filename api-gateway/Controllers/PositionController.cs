using api_gateway.Models.DTO.Position;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace api_gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PositionController> _logger;

        public PositionController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PositionController> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<PositionReadDto>>> GetPositions()
        {
            try
            {
                var baseUrl = this._configuration["Services:PlayersService"] ?? "http://players-service:8080";

                var client = this._httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync("/api/position");

                if (!response.IsSuccessStatusCode)
                {
                    this._logger.LogError("Error al obtener posiciones: {StatusCode}", response.StatusCode);
                    return StatusCode(500, "Error al obtener posiciones");
                }

                var content = await response.Content.ReadAsStringAsync();

                var nationalities = JsonSerializer.Deserialize<List<PositionReadDto>>(content) ?? new();

                return Ok(nationalities);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al obtener posiciones");
                return StatusCode(500, "Error interno al obtener posiciones");
            }
        }
    }
}
