using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [PasswordValidation()]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        public string? Name { get; set; }

        [RegularExpression(@"^\+7\s?\(?\d{3}\)?\s?\d{3}(-|\s)?\d{2}(-|\s)?\d{2}$",
            ErrorMessage = "Номер должен соответствовать формату: +7 (XXX) XXX-XX-XX. Допускается отсутствие кода номера и" +
            " разделителей \"-\", наличие одного пробела между значащими цифрами. Например: +7 XXX XXX XX XX")]
        public string? Phone { get; set; }
    }
}
