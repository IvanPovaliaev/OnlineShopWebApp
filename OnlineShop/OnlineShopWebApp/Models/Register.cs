using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        [NotCompare("Password", ErrorMessage = "Email и пароль не должны совпадать")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [PasswordValidation(ErrorMessage = $"Пароль должен:\n" +
            $"Cодержать как минимум одну заглавную букву;\n" +
            $"Cодержать как минимум одну строчную букву;\n" +
            $"Cодержать как минимум одну цифру;\n" +
            $"Cодержать как минимум один из специальных символов \"#?!@$%^&*-\";\n" +
            $"Быть длиной от 8 до 20 символов.")]
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
