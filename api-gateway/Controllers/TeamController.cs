namespace api_gateway.Controllers
{
    using api_gateway.Models.DTO.Team;
    using Microsoft.AspNetCore.Mvc;
    using System.Text;
    using System.Text.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public TeamController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthController> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            this._logger = logger;
        }

        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Archivo no recibido");

                var filePath = Path.Combine(this._configuration["PathLogo"].ToString(), file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { path = $"/resources/logos/{file.FileName}" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<TeamReadDto>>> GetTeams()
        {
            try
            {
                var baseUrl = this._configuration["Services:TeamsService"] ?? "http://team-service:8081";

                var client = this._httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync("/api/teams");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al obtener equipos: {StatusCode}", response.StatusCode);
                    return StatusCode(500, "Error al obtener equipos");
                }

                var content = await response.Content.ReadAsStringAsync();

                var teams = JsonSerializer.Deserialize<List<TeamReadDto>>(content) ?? new List<TeamReadDto>();

                return Ok(teams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al obtener equipos");
                return StatusCode(500, "Error interno al obtener equipos");
            }
        }

        [HttpGet("{uuid}")]
        public async Task<IActionResult> GetTeamByUuid(string uuid)
        {
            var baseUrl = this._configuration["Services:TeamsService"] ?? "http://team-service:8081";

            var client = this._httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            var response = await client.GetAsync($"api/teams/{uuid}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al obtener equipo: {StatusCode}", response.StatusCode);
                return StatusCode(500, "Error al obtener equipo");
            }

            var content = await response.Content.ReadAsStringAsync();

            var team = JsonSerializer.Deserialize<TeamReadDto>(content);

            if (team is null)
            {
                return NotFound();
            }

            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDto teamDto)
        {
            if (teamDto == null)
                return BadRequest("Datos inválidos");

            var baseUrl = this._configuration["Services:TeamsService"] ?? "http://team-service:8080";
            var client = this._httpClientFactory.CreateClient();

            var content = new StringContent(
                JsonSerializer.Serialize(teamDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync($"{baseUrl}/api/team", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al crear el equipo: {StatusCode}", response.StatusCode);
                var errorText = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorText);
            }

            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        [HttpPut("{uuid}")]
        public async Task<IActionResult> UpdateTeam(string uuid, [FromBody] TeamUpdateDto teamDto)
        {
            if (teamDto == null)
                return BadRequest("Datos inválidos");

            var client = this._httpClientFactory.CreateClient();
            var baseUrl = this._configuration["Services:TeamsService"] ?? "http://team-service:8080";

            var content = new StringContent(JsonSerializer.Serialize(teamDto), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(baseUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error al actualizar equipo: {Error}", error);
                return StatusCode((int)response.StatusCode, error);
            }

            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        [HttpDelete("{uuid}")]
        public async Task<IActionResult> DeleteTeam(string uuid)
        {
            var client = this._httpClientFactory.CreateClient();
            var baseUrl = this._configuration["Services:TeamsService"] ?? "http://team-service:8080";

            var response = await client.DeleteAsync(baseUrl);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error al eliminar equipo: {Error}", error);
                return StatusCode((int)response.StatusCode, error);
            }

            return Ok(new { message = "Equipo eliminado correctamente" });
        }
    }
}
