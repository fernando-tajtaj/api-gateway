using api_gateway.Models.DTO.Report;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace api_gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReportController> _logger;
        private readonly string _reportServiceBaseUrl;

        public ReportController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ReportController> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            this._logger = logger;

            this._reportServiceBaseUrl = this._configuration["Services:ReportService"] ?? "http://report-service:8000";
        }

        // ✅ GET: api/report
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            try
            {
                var client = this._httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(this._reportServiceBaseUrl);

                var reports = await client.GetFromJsonAsync<List<ReportBaseDto>>("reports/");
                return Ok(reports);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error al obtener los reportes");
                return StatusCode(500, "Error interno al obtener los reportes");
            }
        }

        // ✅ GET: api/report/{uuid}
        [HttpGet("{uuid}")]
        public async Task<IActionResult> GetReportById(string uuid)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_reportServiceBaseUrl);

                var response = await client.GetAsync($"reports/{uuid}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("No se encontró el reporte con UUID {Uuid}", uuid);
                    return Ok(new
                    {
                        hasData = false,
                        message = "El reporte no existe.",
                        report = (object?)null,
                        data = new List<object>()
                    });
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Fallo al obtener el reporte: {StatusCode}", response.StatusCode);
                    return Ok(new
                    {
                        hasData = false,
                        message = $"Error al obtener el reporte (HTTP {(int)response.StatusCode})",
                        report = (object?)null,
                        data = new List<object>()
                    });
                }

                var rawJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(rawJson);
                var root = doc.RootElement;

                ReportBaseDto? report = null;
                if (root.TryGetProperty("report", out var reportElement))
                {
                    report = reportElement.Deserialize<ReportBaseDto>(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                var data = new List<Dictionary<string, object>>();
                if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in dataElement.EnumerateArray())
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in element.EnumerateObject())
                        {
                            dict[prop.Name] = prop.Value.ValueKind switch
                            {
                                JsonValueKind.String => prop.Value.GetString() ?? string.Empty,
                                JsonValueKind.Number => prop.Value.TryGetInt32(out int i)
                                    ? i
                                    : (prop.Value.TryGetDouble(out double d) ? d : 0),
                                JsonValueKind.True => true,
                                JsonValueKind.False => false,
                                JsonValueKind.Null => string.Empty,
                                _ => prop.Value.ToString() ?? string.Empty
                            };
                        }
                        data.Add(dict);
                    }
                }

                return Ok(new
                {
                    hasData = data.Count > 0,
                    message = data.Count > 0
                        ? "Reporte cargado correctamente."
                        : "El reporte no contiene datos.",
                    report,
                    data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener el reporte con UUID {Uuid}", uuid);
                return Ok(new
                {
                    hasData = false,
                    message = "Ocurrió un error interno al procesar el reporte.",
                    report = (object?)null,
                    data = new List<object>()
                });
            }
        }
    }
}
