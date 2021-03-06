using System;
using System.ComponentModel.DataAnnotations;

namespace UsersWebApi.DTO
{
    public class UsersCreateDTO
    {
        [Required(ErrorMessage = "Введите логин")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = ("Запрещены все символы кроме латинских букв и цифр"))]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = ("Запрещены все символы кроме латинских букв и цифр"))]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [RegularExpression("^[A-Za-zА-Яа-я]+$", ErrorMessage = ("Запрещены все символы кроме латинских и русских букв"))]
        public string Name { get; set; }

        [Range(0, 2, ErrorMessage = "Пол может быть следующим: 0 - муж, 1 - жен, 2 - неизвестно")]
        [Required(ErrorMessage = "Укажите пол")]
        public int Gender { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Укажите будет ли пользователь администратором")]
        public bool Admin { get; set; }
    }
}
