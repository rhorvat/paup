using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Ime")]
        [Required(ErrorMessage = "Ime je obavezno")]
        public string Name { get; set; }

        [Display(Name = "Prezime")]
        [Required(ErrorMessage = "Prezime je obavezno")]
        public string Surname { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Zaporka je obavezna")]
        [MinLength(6, ErrorMessage = "Adresa mora sadržavati minimalno 6 znakova.")]
        public string Adress { get; set; }

        [Display(Name = "E-mail")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "E-mail je obavezan")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public int AccountType { get; set; }
        public string Password { get; set; }
    }
}