namespace OnlineShopWebApp.Models
{
    public class Register
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
    }
}
