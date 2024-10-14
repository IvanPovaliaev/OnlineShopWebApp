﻿using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopWebApp.Models
{
    public class Role
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^[a-zA-Z0-9_ -]+$", ErrorMessage = "Наименование роли может содержать только латинские буквы, цифры, пробелы, знаки \"_\" и \"-\'")]
        public string Name { get; set; }

        public Role()
        {
            Id = Guid.NewGuid();
        }

        public Role(string name) : this()
        {
            Name = name;
        }
    }
}