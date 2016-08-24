using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MailsControlSystem.Models
{
    public class ListMailStatus
    {
        [Key]
        public int MailStatusID { set; get; }
        public string MailStatusDescription { set; get; }

        public ICollection<Mail> Mail { set; get; }
    }
}