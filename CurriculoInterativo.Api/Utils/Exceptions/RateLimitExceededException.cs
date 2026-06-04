namespace CurriculoInterativo.Api.Utils.Exceptions
{
    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException(string message) : base(message)
        {
        }
    }
}
