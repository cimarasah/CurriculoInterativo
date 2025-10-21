namespace CurriculoInterativo.Api.DTOs
{
    public class ContactRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string GitHub { get; set; } = string.Empty;
    }
}

