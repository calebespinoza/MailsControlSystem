using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MailsControlSystem.Models
{
    public class Mail
    {
        [Key]
        public int MailID { set; get; }

        [Display(Name = "Número de Guía")]
        [Required(ErrorMessage = "Se requiere el número de guía.")]
        public long GuideNumber { set; get; }
        
        public int MailStatus { set; get; }
        
        [ForeignKey("MailStatus")]
        public virtual ListMailStatus ListMailStatus { set; get; }
    }
}