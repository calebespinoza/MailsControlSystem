using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MailsControlSystem.Models;

namespace MailsControlSystem.DAL
{
    public class MailsInitializer : DropCreateDatabaseIfModelChanges<MailDBContext>  //DropCreateDatabaseAlways<MailDBContext>
    {
        protected override void Seed(MailDBContext context)
        {
            context.Database.ExecuteSqlCommand("ALTER TABLE Outcoming ADD CONSTRAINT uc_OutcomingMail UNIQUE(OutcomingMail)");
            context.Database.ExecuteSqlCommand("ALTER TABLE Incoming ADD CONSTRAINT uc_IncomingMail UNIQUE(IncomingMail)");
            
            var account_status = new List<ListAccountStatus>
            {
                new ListAccountStatus { UserStatusDescription = "Habilitado" },
                new ListAccountStatus { UserStatusDescription = "Deshabilitado" },
            };
            account_status.ForEach(s => context.LAStatus.Add(s));
            context.SaveChanges();

            var list_role = new List<ListRole>
            {
                new ListRole { RoleName = "Administrador", RoleDescription = "Primer nivel de acceso." },
                new ListRole { RoleName = "Recepción", RoleDescription = "Segundo nivel de acceso." },
                new ListRole { RoleName = "Enlace", RoleDescription = "Tercer nivel de acceso." },
                new ListRole { RoleName = "Personal CIB", RoleDescription = "Cuarto nivel de acceso." },
            };
            list_role.ForEach(s => context.ListRoles.Add(s));
            context.SaveChanges();

            var person = new List<Person>
            {
                new Person { Name = "Martha", FirstName = "Ucharico", LastName = "Asquicho", 
                    Email="mucharico@bo.ci.org", IdentityNumber=1234567, MobilePhone=79978872, Region="Cochabamba"},
                new Person { Name = "Caleb", FirstName = "Espinoza", LastName = "Gutiérrez", 
                    Email="cespinoza@bo.ci.org", IdentityNumber=6559305, MobilePhone=70777184, Region="Cochabamba"},
            };
            person.ForEach(s => context.People.Add(s));
            context.SaveChanges();

            var user_account = new List<UserAccount>
            {
                new UserAccount { UserAccountID=1, Account="MUcharico", Password="compassion123!@#", 
                    Positions="IT Lead", AccountStatus=1, Role=1 },
                new UserAccount { UserAccountID=2, Account="CEspinoza", Password="compassion123!@#", 
                    Positions="IT Assistant", AccountStatus=1, Role=1 },
            };
            user_account.ForEach(s => context.UsersAccounts.Add(s));
            context.SaveChanges();

            var mail_status = new List<ListMailStatus>
            {
                new ListMailStatus {MailStatusDescription="Enviado"},
                new ListMailStatus {MailStatusDescription="Recibido"},
            };
            mail_status.ForEach(s => context.LMStatus.Add(s));
            context.SaveChanges();
            //base.Seed(context);
        }
    }
}