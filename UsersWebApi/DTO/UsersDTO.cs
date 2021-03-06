using System;
using System.ComponentModel.DataAnnotations;

namespace UsersWebApi.DTO
{
    public class UsersDTO
    {
        [Required(ErrorMessage = "Введите имя")]
        [RegularExpression("^[A-Za-zА-Яа-я]+$", ErrorMessage = ("Запрещены все символы кроме латинских и русских букв"))]
        public string Name { get; set; }

        [Range(0, 2, ErrorMessage = "Пол может быть следующим: 0 - муж, 1 - жен, 2 - неизвестно")]
        [Required(ErrorMessage = "Укажите пол")]
        public int Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        public DateTime? RevokedOn { get; set; }
    }
}
