using OnlineShopWebApp.Helpers;
using System;

namespace OnlineShopWebApp.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        [ExcludeFromExcelExport]
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }

        [ExcelExportGetSubfield("Name")]
        public Role Role { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
