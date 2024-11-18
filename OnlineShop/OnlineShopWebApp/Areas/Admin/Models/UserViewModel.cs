namespace OnlineShopWebApp.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string Id { get; init; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string RoleName { get; set; }
    }
}
