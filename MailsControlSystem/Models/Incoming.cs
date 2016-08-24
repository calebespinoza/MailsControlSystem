using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MailsControlSystem.Models
{
    public class Incoming
    {
        [Key]
        public int IncomingID { set; get; }       

        [Display(Name = "De:")]
        public int From { set; get; }

        public string IncomingDate { set; get; }
        public string IncomingTime { set; get; }

        [Display(Name = "Cantidad recibida:")]
        [Required(ErrorMessage = "Se requiere la cantidad recibida.")]
        public string Quanty { set; get; }

        [Display(Name = "Descripción de la correspondencia:")]
        public string Description { set; get; }       
        
        public int IncomingMail { set; get; }

        [Display(Name = "Observaciones en la Recepción")]
        public string Observations { set; get; }
        
        [ForeignKey("From")]
        public virtual UserAccount UserAccount { set; get; }
        
        public virtual Mail Mail { set; get; }
    }
}