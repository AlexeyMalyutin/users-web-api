using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        //[DisplayFormat(DataFormatString = "{dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Birthday { get; set; }

        public DateTime? RevokedOn { get; set; }

    }
}
