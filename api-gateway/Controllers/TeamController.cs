namespace api_gateway.Controllers
{
    using api_gateway.Models.DTO.Team;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Text;
    using System.Text.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TeamController> _logger;

        public TeamController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<TeamController> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            this._logger = logger;
        }

        [Authorize]
        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Archivo no recibido" });

                // Validaciones
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new { message = "Formato no permitido" });
                }

                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Archivo muy grande. Máximo 5MB" });
                }

                // Ruta de logos
                var logosPath = Environment.GetEnvironmentVariable("LOGOS_PATH")
                    ?? _configuration["PathLogo"]
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "logos");

                if (!Directory.Exists(logosPath))
                {
                    Directory.CreateDirectory(logosPath);
                }

                // Limpiar nombre de archivo (eliminar caracteres peligrosos)
                var safeFileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(logosPath, safeFileName);

                // Si el archivo ya existe, agregar timestamp
                if (System.IO.File.Exists(filePath))
                {
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(safeFileName);
                    safeFileName = $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    filePath = Path.Combine(logosPath, safeFileName);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new
                {
                    success = true,
                    path = $"/logos/{safeFileName}"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = "Sin permisos", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al subir archivo", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<TeamReadDto>>> GetTeams()
        {
            try
            {
                var baseUrl = this._configuration["Services:TeamsService"];

                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("Service endpoint not configured: Services:TeamsService");
                }

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

        [Authorize]
        [HttpGet("{uuid}")]
        public async Task<IActionResult> GetTeamByUuid(string uuid)
        {
            var baseUrl = this._configuration["Services:TeamsService"];
            
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("Service endpoint not configured: Services:TeamsService");
            }

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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDto teamDto)
        {
            if (teamDto == null)
                return BadRequest(new { message = "Datos inválidos" });

            try
            {
                var baseUrl = this._configuration["Services:TeamsService"];

                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("Service endpoint not configured: Services:TeamsService");
                }

                var client = _httpClientFactory.CreateClient();

                var content = new StringContent(
                    JsonSerializer.Serialize(teamDto),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync($"{baseUrl}/api/teams", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error al crear equipo: {response.StatusCode} - {errorText}");
                    return StatusCode((int)response.StatusCode, new { message = errorText });
                }

                var result = await response.Content.ReadAsStringAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al crear equipo");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{uuid}")]
        public async Task<IActionResult> UpdateTeam(string uuid, [FromBody] TeamUpdateDto teamDto)
        {
            if (teamDto == null)
                return BadRequest(new { message = "Datos inválidos" });

            try
            {
                var client = _httpClientFactory.CreateClient();
                
                var baseUrl = this._configuration["Services:TeamsService"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("Service endpoint not configured: Services:TeamsService");
                }

                var content = new StringContent(
                    JsonSerializer.Serialize(teamDto),
                    Encoding.UTF8,
                    "application/json"
                );

                // IMPORTANTE: Agregar el uuid a la URL
                var url = $"{baseUrl}/api/teams/{uuid}";

                var response = await client.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error al actualizar equipo: {error}");
                    return StatusCode((int)response.StatusCode, new { message = error });
                }

                var result = await response.Content.ReadAsStringAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al actualizar equipo");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{uuid}")]
        public async Task<IActionResult> DeleteTeam(string uuid)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var baseUrl = this._configuration["Services:TeamsService"];
                
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("Service endpoint not configured: Services:TeamsService");
                }

                // IMPORTANTE: Agregar el uuid a la URL
                var url = $"{baseUrl}/api/teams/{uuid}";
                _logger.LogInformation($"Eliminando equipo en: {url}");

                var response = await client.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error al eliminar equipo: {error}");
                    return StatusCode((int)response.StatusCode, new { message = error });
                }

                return Ok(new { message = "Equipo eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al eliminar equipo");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}
