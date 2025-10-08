using api_gateway.Models.DTO.City;

namespace api_gateway.Models.DTO.Team
{
    public class TeamUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public required string UuidCity { get; set; }
        public string Logo { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }
}
