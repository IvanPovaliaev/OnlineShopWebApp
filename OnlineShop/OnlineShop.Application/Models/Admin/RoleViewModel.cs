using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Application.Models.Admin
{
    public class RoleViewModel
    {
        public string Id { get; init; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^[a-zA-Z0-9_ -]+$", ErrorMessage = "Наименование роли может содержать только латинские буквы, цифры, пробелы, знаки \"_\" и \"-\'")]
        public string Name { get; set; }
        public bool CanBeDeleted { get; init; } = true;
    }
}
