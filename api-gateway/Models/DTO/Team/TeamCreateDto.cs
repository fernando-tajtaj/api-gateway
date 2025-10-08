namespace api_gateway.Models.DTO.Team
{
    public class TeamCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? UuidCity { get; set; }
        public string Logo { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
    }
}
