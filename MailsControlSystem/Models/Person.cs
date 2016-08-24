using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MailsControlSystem.Models
{
    public class Person
    {
        [Key]
        public int PersonID { set; get; }
        
        [Display(Name = "Nombre:")]
        [MaxLength(100)]
        [Required(ErrorMessage = "Se requiere el nombre de la persona")]
        [StringLength(100, ErrorMessage = "Solo puede escribir hasta 100 caracteres")]
        public string Name { set; get; }

        [Display(Name = "Apellido Paterno:")]
        [MaxLength(100)]
        [Required(ErrorMessage = "Se requiere el apellido paterno")]
        [StringLength(100, ErrorMessage = "Solo puede escribir hasta 100 caracteres")]
        public string FirstName { set; get; }

        [Display(Name = "Apellido Materno:")]
        [MaxLength(100)]
        [StringLength(100, ErrorMessage = "Solo puede escribir hasta 100 caracteres")]
        public string LastName { set; get; }

        [Display(Name = "Carnet de Identidad:")]
        [Required(ErrorMessage = "Se requiere el carnet de identidad")]
        public long IdentityNumber { set; get; }

        [Display(Name = "Correo Electrónico:")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Se requiere el correo electrónico")]
        [StringLength(50, ErrorMessage = "Solo puede escribir hasta 50 caracteres")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Por favor, ingrese un correo electrónico válido")]
        public string Email { set; get; }

        [Display(Name = "Celular:")]
        [Required(ErrorMessage = "Se requiere el número de celular")]
        public long MobilePhone { set; get; }
        
        [Display(Name = "Región:")]
        [MaxLength(11)]
        [Required(ErrorMessage = "Se requiere la región")]
        public string Region { set; get; }

        public string FullName
        {
            get
            {
                return Name + " " + FirstName;
            }
        }

        public virtual UserAccount UserAccount { set; get; }
    }
}