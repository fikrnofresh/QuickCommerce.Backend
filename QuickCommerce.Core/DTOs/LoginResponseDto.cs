namespace QuickCommerce.Core.DTOs
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }
}
