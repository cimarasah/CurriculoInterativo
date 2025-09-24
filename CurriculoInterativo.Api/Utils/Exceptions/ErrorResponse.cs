namespace CurriculoInterativo.Api.Utils.Exceptions
{
    public class ErrorResponse
    {
        public required string Message { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
