using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MailsControlSystem.Models
{
    public class UserAccount
    {
        [Key]
        public int UserAccountID { set; get; }

        [MaxLength(20)]
        public string Account { set; get; }
        
        [MaxLength(30)]
        public string Password { set; get; }

        [Display(Name = "Cargo en oficina:")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Se requiere el cargo en oficina")]
        public string Positions { set; get; }

        public int AccountStatus { set; get; }

        public int Role { set; get; }

        public virtual Person Person { set; get; }

        [ForeignKey("AccountStatus")]
        [Display(Name = "Estado de la cuenta:")]
        public virtual ListAccountStatus ListAccountStatus { set; get; }

        [ForeignKey("Role")]
        [Display(Name = "Nivel de privilegio:")]
        public virtual ListRole ListRole { set; get; }

        public virtual ICollection<Outcoming> Outcoming { set; get; }
        public virtual ICollection<Incoming> Incoming { set; get; }
    }
}