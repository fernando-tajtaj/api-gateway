using api_gateway.Models.DTO.Player;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace api_gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PlayerController> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            this._logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<PlayerReadDto>>> GetPlayers()
        {
            try
            {
                var baseUrl = this._configuration["Services:PlayersService"] ?? "http://players-service:8080";

                var client = this._httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync("/api/player");

                if (!response.IsSuccessStatusCode)
                {
                    this._logger.LogError("Error al obtener jugadores: {StatusCode}", response.StatusCode);
                    return StatusCode(500, "Error al obtener jugadores");
                }

                var content = await response.Content.ReadAsStringAsync();

                var players = JsonSerializer.Deserialize<List<PlayerReadDto>>(content) ?? new List<PlayerReadDto>();

                return Ok(players);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al obtener jugadores");
                return StatusCode(500, "Error interno al obtener jugadores");
            }
        }

        [Authorize]
        [HttpGet("by-team/{uuidTeam}")]
        public async Task<ActionResult<List<PlayerReadDto>>> GetPlayersByTeam(string uuidTeam)
        {
            try
            {
                var baseUrl = this._configuration["Services:PlayersService"] ?? "http://players-service:8080";

                var client = this._httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync($"/api/player/by-team/{uuidTeam}");

                if (!response.IsSuccessStatusCode)
                {
                    this._logger.LogError("Error al obtener jugadores por equipo: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, "Error al obtener jugadores");
                }

                var content = await response.Content.ReadAsStringAsync();

                var players = JsonSerializer.Deserialize<List<PlayerReadDto>>(content) ?? new List<PlayerReadDto>();

                return Ok(players);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al obtener jugadores por equipo");
                return StatusCode(500, "Error interno al obtener jugadores");
            }
        }

        [Authorize]
        [HttpGet("{uuid}")]
        public async Task<IActionResult> GetPlayerByUuid(string uuid)
        {
            var baseUrl = _configuration["Services:PlayersService"] ?? "http://players-service:8080";

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            var response = await client.GetAsync($"api/player/{uuid}");

            if (!response.IsSuccessStatusCode)
            {
                this._logger.LogError("Error al obtener jugador: {StatusCode}", response.StatusCode);
                return StatusCode(500, "Error al obtener jugador");
            }

            var content = await response.Content.ReadAsStringAsync();
            var player = JsonSerializer.Deserialize<PlayerReadDto>(content);

            if (player is null)
                return NotFound();

            return Ok(player);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] PlayerCreateDto playerDto)
        {
            if (playerDto == null)
                return BadRequest(new { message = "Datos inválidos" });

            try
            {
                var baseUrl = _configuration["Services:PlayersService"] ?? "http://players-service:8080";
                var client = _httpClientFactory.CreateClient();

                var content = new StringContent(
                    JsonSerializer.Serialize(playerDto),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync($"{baseUrl}/api/player", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    this._logger.LogError($"Error al crear jugador: {response.StatusCode} - {errorText}");
                    return StatusCode((int)response.StatusCode, new { message = errorText });
                }

                var result = await response.Content.ReadAsStringAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al crear jugador");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{uuid}")]
        public async Task<IActionResult> UpdatePlayer(string uuid, [FromBody] PlayerUpdateDto playerDto)
        {
            if (playerDto == null)
                return BadRequest(new { message = "Datos inválidos" });

            try
            {
                var client = _httpClientFactory.CreateClient();
                var baseUrl = _configuration["Services:PlayersService"] ?? "http://players-service:8080";

                var content = new StringContent(
                    JsonSerializer.Serialize(playerDto),
                    Encoding.UTF8,
                    "application/json"
                );

                var url = $"{baseUrl}/api/player/{uuid}";
                var response = await client.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    this._logger.LogError($"Error al actualizar jugador: {error}");
                    return StatusCode((int)response.StatusCode, new { message = error });
                }

                var result = await response.Content.ReadAsStringAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al actualizar jugador");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{uuid}")]
        public async Task<IActionResult> DeletePlayer(string uuid)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var baseUrl = _configuration["Services:PlayersService"] ?? "http://players-service:8080";

                var url = $"{baseUrl}/api/player/{uuid}";
                this._logger.LogInformation($"Eliminando jugador en: {url}");

                var response = await client.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    this._logger.LogError($"Error al eliminar jugador: {error}");
                    return StatusCode((int)response.StatusCode, new { message = error });
                }

                return Ok(new { message = "Jugador eliminado correctamente" });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Excepción al eliminar jugador");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}
