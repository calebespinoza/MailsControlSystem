using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using MailsControlSystem.Models;

namespace MailsControlSystem.DAL
{
    public class MailDBContext : DbContext
    {
        public DbSet<Person> People { set; get; }
        public DbSet<UserAccount> UsersAccounts { set; get; }
        public DbSet<ListRole> ListRoles { set; get; }
        public DbSet<ListAccountStatus> LAStatus { set; get; }
        public DbSet<ListMailStatus> LMStatus { set; get; }
        public DbSet<Incoming> Incomings { set; get; }
        public DbSet<Outcoming> Outcomings { set; get; }
        public DbSet<Mail> Mails { set; get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Person>().HasOptional(p => p.UserAccount).WithRequired(p => p.Person);
            //modelBuilder.Entity<Mail>().HasOptional(o => o.Outcoming).WithMany().HasForeignKey(o => o.MailID);
            //modelBuilder.Entity<Mail>().HasOptional(i => i.Incoming).WithMany().HasForeignKey(i => i.MailID);
            //modelBuilder.Entity<Outcoming>().HasOptional(o => o.Mail).WithRequired();//.Map(o => o.ToTable("Outcoming"));//.WillCascadeOnDelete(false);
            
            modelBuilder.Entity<Outcoming>().HasRequired(o => o.Mail).
                WithMany().HasForeignKey(u => u.OutcomingMail).WillCascadeOnDelete(false);

            modelBuilder.Entity<Incoming>().HasRequired(i => i.Mail).
                WithMany().HasForeignKey(j => j.IncomingMail).WillCascadeOnDelete(false);

            //modelBuilder.Entity<Mail>().Map(d => d.ToTable("Outcoming"));
            //modelBuilder.Entity<Incoming>().HasOptional(i => i.Mail).WithRequired();
            //modelBuilder.Entity<Mail>().HasOptional(o => o.Outcoming).WithRequired();
            //base.OnModelCreating(modelBuilder);
        }
    }
}