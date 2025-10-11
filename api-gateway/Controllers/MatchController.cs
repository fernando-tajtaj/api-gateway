using api_gateway.Models.DTO.Match;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace api_gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MatchController> _logger;

        public MatchController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<MatchController> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<MatchReadDto>>> GetMatches()
        {
            try
            {
                var baseUrl = this._configuration["Services:MatchService"] ?? "http://match-service:8080";

                var client = this._httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync("/api/match");

                if (!response.IsSuccessStatusCode)
                {
                    this._logger.LogError("Error al obtener juegos: {StatusCode}", response.StatusCode);
                    return StatusCode(500, "Error al obtener juegos");
                }

                var content = await response.Content.ReadAsStringAsync();

                var matches = JsonSerializer.Deserialize<List<MatchReadDto>>(content) ?? new List<MatchReadDto>();

                return Ok(matches);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al obtener juegos");
                return StatusCode(500, "Error interno al obtener juegos");
            }
        }

        [HttpGet("{uuid}")]
        public async Task<IActionResult> GetMatchByUuid(string uuid)
        {
            var baseUrl = _configuration["Services:MatchService"] ?? "http://match-service:8080";

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            var response = await client.GetAsync($"api/match/{uuid}");

            if (!response.IsSuccessStatusCode)
            {
                this._logger.LogError("Error al obtener juego: {StatusCode}", response.StatusCode);
                return StatusCode(500, "Error al obtener juego");
            }

            var content = await response.Content.ReadAsStringAsync();
            var match = JsonSerializer.Deserialize<MatchReadDto>(content);

            if (match is null)
                return NotFound();

            return Ok(match);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMatch([FromBody] MatchCreateDto macthDto)
        {
            if (macthDto == null)
                return BadRequest(new { message = "Datos inválidos" });

            try
            {
                var baseUrl = this._configuration["Services:MatchService"] ?? "http://match-service:8080";
                var client = this._httpClientFactory.CreateClient();

                var content = new StringContent(
                    JsonSerializer.Serialize(macthDto),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync($"{baseUrl}/api/match", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    this._logger.LogError($"Error al crear juego: {response.StatusCode} - {errorText}");
                    return StatusCode((int)response.StatusCode, new { message = errorText });
                }

                var result = await response.Content.ReadAsStringAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al crear juego");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPut("{uuid}")]
        public async Task<IActionResult> UpdateMatch(string uuid, [FromBody] MatchUpdateDto matchDto)
        {
            if (matchDto == null)
                return BadRequest(new { message = "Datos inválidos" });

            try
            {
                var client = this._httpClientFactory.CreateClient();
                var baseUrl = this._configuration["Services:MatchService"] ?? "http://match-service:8080";

                var content = new StringContent(
                    JsonSerializer.Serialize(matchDto),
                    Encoding.UTF8,
                    "application/json"
                );

                var url = $"{baseUrl}/api/match/{uuid}";
                var response = await client.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    this._logger.LogError($"Error al actualizar juego: {error}");
                    return StatusCode((int)response.StatusCode, new { message = error });
                }

                var result = await response.Content.ReadAsStringAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al actualizar juego");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpDelete("{uuid}")]
        public async Task<IActionResult> DeleteMatch(string uuid)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var baseUrl = _configuration["Services:MatchService"] ?? "http://match-service:8080";

                var url = $"{baseUrl}/api/match/{uuid}";
                this._logger.LogInformation($"Eliminando juego en: {url}");

                var response = await client.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    this._logger.LogError($"Error al eliminar juego: {error}");
                    return StatusCode((int)response.StatusCode, new { message = error });
                }

                return Ok(new { message = "Juego eliminado correctamente" });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al eliminar juego");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}
