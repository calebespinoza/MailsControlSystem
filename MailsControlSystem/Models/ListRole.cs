using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MailsControlSystem.Models
{
    public class ListRole
    {
        [Key]
        public int ListRoleID { set; get; }

        [MaxLength(50)]
        public string RoleName { set; get; }

        [MaxLength(70)]
        public string RoleDescription { set; get; }

        public virtual ICollection<UserAccount> UserAccount { set; get; }

        //public virtual ICollection<UserRole> UserRole { set; get; }
    }
}