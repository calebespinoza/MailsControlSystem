using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailsControlSystem.Models;
using MailsControlSystem.DAL;
using System.Linq.Expressions;

namespace MailsControlSystem.Controllers
{
    public class UserController : Controller
    {
        private MailDBContext db = new MailDBContext();

        //
        // GET: /User/
         [Authorize(Roles = "Administrador")]
        public ViewResult Index(string currentFilter, string searchString)
        {

            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var person = from u in db.People
                         select u;   
            

            if (!String.IsNullOrEmpty(searchString))
            {
                person = person.Where(u => u.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            return View(person.ToList());
        }

        //
        // GET: /User/Details/5

        public ViewResult Details(int id)
        {
            Person person = db.People.Find(id);
            return View(person);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            ViewBag.PersonID = new SelectList(db.UsersAccounts, "UserAccountID", "Account");
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(Person person, UserAccount useraccount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.People.Add(person);
                    UserAccount account = new UserAccount()
                    {
                        UserAccountID = person.PersonID,
                        //PersonID = person.PersonID,
                        Account = person.Name.ElementAt(0) + person.FirstName,
                        Password = person.IdentityNumber.ToString(),
                        Positions = useraccount.Positions,
                        AccountStatus = 1,
                        Role = 4
                    };
                    person.UserAccount = account;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "No se guardaron los cambios, Vuelva a intentarlo, si el problema persiste notifique al administrador del sistema.");
            }

            //ViewBag.PersonID = new SelectList(db.UsersAccounts, "UserAccountID", "Account", person.PersonID);
            return View(person);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            Person person = db.People.Find(id);
            UserAccount useraccount = db.UsersAccounts.Find(id);
            ViewBag.PersonID = new SelectList(db.UsersAccounts, "UserAccountID", "Account", person.PersonID);
            return View(person);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(Person person, UserAccount useraccount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserAccount account = new UserAccount()
                        {
                            UserAccountID = person.PersonID,
                            Account = person.Name.ElementAt(0) + person.FirstName,
                            Password = person.UserAccount.Password,
                            Positions = useraccount.Positions,
                            AccountStatus = int.Parse(useraccount.AccountStatus.ToString()),
                            Role = int.Parse(useraccount.Role.ToString())
                        };
                    useraccount = account;
                    person.UserAccount = account;
                    db.Entry(useraccount).State = EntityState.Modified;
                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "No se guardaron los cambios, Vuelva a intentarlo, si el problema persiste notifique al administrador del sistema.");
            }
            //ViewBag.PersonID = new SelectList(db.UsersAccounts, "UserAccountID", "Account", person.PersonID);
            return View(person);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id)
        {
            Person person = db.People.Find(id);
            return View(person);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Person person = db.People.Find(id);
            UserAccount useraccount = db.UsersAccounts.Find(id);
            db.People.Remove(person);
            db.UsersAccounts.Remove(useraccount);
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