using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailsControlSystem.Models;
using MailsControlSystem.DAL;

namespace MailsControlSystem.Controllers
{
    public class OutcomingController : Controller
    {
        private MailDBContext db = new MailDBContext();

        //
        // GET: /Outcoming/

        public ViewResult Index(string currentFilter, string searchString)
        {          
            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var outcoming = from o in db.Outcomings
                            select o;

            if (!String.IsNullOrEmpty(searchString))
            {
                int value = int.Parse(searchString);
                outcoming = outcoming.Where(u => u.Mail.GuideNumber.Equals(value));//.Contains(searchString.ToUpper()));
            }
            //var outcomings = db.Outcomings.Include(o => o.UserAccount).Include(o => o.Mail);
            return View(outcoming.ToList());
        }

        //
        // GET: /Outcoming/Details/5

        public ViewResult Details(int id)
        {
            Outcoming outcoming = db.Outcomings.Find(id);
            return View(outcoming);
        }

        //
        // GET: /Outcoming/Create

        public ActionResult Create()
        {
            ViewBag.OutcomingMail = new SelectList(db.Mails, "MailID", "Mail");
            ViewBag.To = new SelectList(db.UsersAccounts, "UserAccountID", "Account");
            ViewBag.From = new SelectList(db.UsersAccounts, "UserAccountID", "Account");
            return View();
        }

        //
        // POST: /Outcoming/Create

        [HttpPost]
        public ActionResult Create(Outcoming outcoming, Mail mail)
        {
            if (ModelState.IsValid)
            {
                Mail currentMail = new Mail()
                {
                    MailID = outcoming.OutcomingID,
                    GuideNumber = mail.GuideNumber,
                    MailStatus = 1
                };
                outcoming.OutcomingMail = currentMail.MailID;
                outcoming.Mail = currentMail;
                db.Outcomings.Add(outcoming);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.To = new SelectList(db.UsersAccounts, "UserAccountID", "Account", outcoming.To);
            ViewBag.OutcomingID = new SelectList(db.Mails, "MailID", "MailID", outcoming.OutcomingID);
            return View(outcoming);
        }

        //
        // GET: /Outcoming/Edit/5

        public ActionResult Edit(int id)
        {
            Outcoming outcoming = db.Outcomings.Find(id);
            ViewBag.To = new SelectList(db.UsersAccounts, "UserAccountID", "Account", outcoming.To);
            ViewBag.OutcomingID = new SelectList(db.Mails, "MailID", "MailID", outcoming.OutcomingID);
            return View(outcoming);
        }

        //
        // POST: /Outcoming/Edit/5

        [HttpPost]
        public ActionResult Edit(Outcoming outcoming)
        {
            if (ModelState.IsValid)
            {
                db.Entry(outcoming).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.To = new SelectList(db.UsersAccounts, "UserAccountID", "Account", outcoming.To);
            ViewBag.OutcomingID = new SelectList(db.Mails, "MailID", "MailID", outcoming.OutcomingID);
            return View(outcoming);
        }

        //
        // GET: /Outcoming/Delete/5

        public ActionResult Delete(int id)
        {
            Outcoming outcoming = db.Outcomings.Find(id);
            return View(outcoming);
        }

        //
        // POST: /Outcoming/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Outcoming outcoming = db.Outcomings.Find(id);
            db.Outcomings.Remove(outcoming);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}