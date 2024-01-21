namespace AuthCenter.Models.Dtos.Authentization
{
    public class AuthResponse
    {
        public string TokenId { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public string Token { get; set; }

        public string Secret { get; set; }

        public DateTime? Expiration { get; set; }
    }
}
