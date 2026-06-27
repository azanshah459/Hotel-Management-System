namespace HotelManagement.Models
{
    public class AuthUser
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthSettings
    {
        public AuthUser Admin { get; set; } = new();
        public AuthUser User { get; set; } = new();
    }
}
