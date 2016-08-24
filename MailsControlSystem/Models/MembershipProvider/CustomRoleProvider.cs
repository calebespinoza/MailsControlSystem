using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MailsControlSystem.DAL;

namespace MailsControlSystem.Models.MembershipProvider
{
    public class CustomRoleProvider : System.Web.Security.RoleProvider
     {
        private MailDBContext db = new MailDBContext();

        #region UNIMPLEMENTED

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            List<string> roles = new List<string>();
            var role = from r in db.ListRoles select r;
            foreach (var query in role)
            {
                roles.Add(query.RoleName);
            }
            return roles.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();
            var role = from lr in db.ListRoles join ua in db.UsersAccounts 
                       on lr.ListRoleID equals ua.Role where ua.Account == username select lr;
            
            foreach (var query in role)
            {
                roles.Add(query.RoleName);
            }
            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override bool IsUserInRole(string username, string roleName)
        {
            int id = 0;
            var currentUserRole = from u in db.UsersAccounts
                                  select u;

            foreach (var query in currentUserRole)
            {
                id = query.UserAccountID;
            }

            UserAccount user = db.UsersAccounts.Find(id);
            if (user != null)
            {
                if (user.ListRole.RoleName.ToLower() == roleName.ToLower())
                    return true;
                else
                    return false;
            }
            return false;
        }

    }
}