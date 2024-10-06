using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,20}$",
            ErrorMessage = $"Пароль должен:\n" +
            $"содержать как минимум одну заглавную букву;\n" +
            $"содержать как минимум одну строчную букву;\n" +
            $"содержать как минимум одну цифру;\n" +
            $"содержать как минимум один из специальных символов \"#?!@$%^&*-\";\n" +
            $"быть длиной от 8 до 20 символов.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        public string? Name { get; set; }

        [RegularExpression(@"^\+7\s{0,1}\(\d{3}\)\s{0,1}\d{3}-\d{2}-\d{2}$",
            ErrorMessage = "Введите номер в формате +7(XXX)XXX-XX-XX")]
        public string? Phone { get; set; }
    }
}
