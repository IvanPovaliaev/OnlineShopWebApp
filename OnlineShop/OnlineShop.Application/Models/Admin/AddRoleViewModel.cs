using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models.Admin
{
    public class AddRoleViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^[a-zA-Z0-9_ -]+$", ErrorMessage = "Наименование роли может содержать только латинские буквы, цифры, пробелы, знаки \"_\" и \"-\'")]
        public string Name { get; set; }
    }
}
