using api_gateway.Models.DTO.City;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api_gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public CityController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthController> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            this._logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<CityReadDto>>> GetCities()
        {
            try
            {
                var baseUrl = _configuration["Services:TeamsService"] ?? "http://team-service:8081";

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync("/api/city");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al obtener ciudades: {StatusCode}", response.StatusCode);
                    return StatusCode(500, "Error al obtener ciudades");
                }

                var content = await response.Content.ReadAsStringAsync();

                var cities = JsonConvert.DeserializeObject<List<CityReadDto>>(content) ?? new List<CityReadDto>();

                return Ok(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al obtener ciudades");
                return StatusCode(500, "Error interno al obtener ciudades");
            }
        }
    }
}
