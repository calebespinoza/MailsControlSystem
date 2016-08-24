using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MailsControlSystem.Models
{
    public class Outcoming
    {
        [Key]
        public int OutcomingID { set; get; }

        [Display(Name = "Para:")]
        public int To { set; get; }

        public string Sender { get; set; }
        public string SendDate { set; get; }
        public string SendTime { set; get; }

        [Display(Name = "Cantidad enviada:")]
        [Required(ErrorMessage = "Se requiere la cantidad envidada.")]
        public string Quanty { set; get; }

        [Display(Name = "Descripción de la correspondencia:")]
        [Required(ErrorMessage = "Se requiere la descripción de la correspondencia.")]
        public string Description { set; get; }

        public int OutcomingMail { set; get; }

        [Display(Name = "Destino:")]
        [Required(ErrorMessage = "Se requiere el destino.")]
        public string City { set; get; }

        [Display(Name = "Observaciones del Envío")]
        public string Observations { set; get; }

        [ForeignKey("To")]
        public virtual UserAccount UserAccount { set; get; }
        public virtual Mail Mail { set; get; }
    }
}