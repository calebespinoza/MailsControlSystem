using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MailsControlSystem.Models
{
    public class ListAccountStatus
    {
        [Key]
        [Range(0, 1)]
        public int UserStatustID { set; get; }

        [MaxLength(15)]
        public string UserStatusDescription { set; get; }

        public virtual ICollection<UserAccount> UserAccount { set; get; }
    }
}