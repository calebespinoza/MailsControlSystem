using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailsControlSystem.Models;
using MailsControlSystem.DAL;
using System.Net.Mail;
using MailsControlSystem.Models.MembershipProvider;

namespace MailsControlSystem.Controllers
{
    public class IncomingController : Controller
    {
        private MailDBContext db = new MailDBContext();
        private CustomRoleProvider provider = new CustomRoleProvider();
        //
        // GET: /Incoming/
         [Authorize(Roles = "Administrador, Recepción, Enlace")]
        public ViewResult Index(string currentFilter, string searchString)
        {
            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var outcoming = from o in db.Outcomings
                            select o;
            var incoming = from i in db.Incomings
                           select i;
            var mail = from m in db.Mails
                       select m;
            List<Outcoming> listOutcoming = new List<Outcoming>();
            foreach (var item in outcoming)
            {
                listOutcoming.Add(item);
            }
            List<Incoming> listIncoming = new List<Incoming>();
            foreach (var item in incoming)
            {
                listIncoming.Add(item);
            }
            List<Mail> listMail = new List<Mail>();
            foreach (var item in mail)
            {
                listMail.Add(item);
            }

            List<Outcoming> listOut = outcoming.ToList();
            List<Incoming> listIn = incoming.ToList();

            List<IOMailWrapperModel> list = new List<IOMailWrapperModel>();
            //Geting the current Login Account
            String currentUser = User.Identity.Name;
            for (int i = 0; i < listOutcoming.Count; i++)
            {
                IOMailWrapperModel model = new IOMailWrapperModel();

                model.outcoming = listOutcoming.ElementAt(i);
                model.incoming = listIncoming.ElementAt(i);
                model.mail = listMail.ElementAt(i);

                if (!provider.GetRoleByUser(currentUser).Equals("Administrador"))
                {
                    //adding only the same region that the login user
                    if (provider.GetRegionByUser(currentUser) == model.outcoming.City)
                    {
                        list.Add(model);
                    }
                }
                else
                {
                    list.Add(model);
                }
            }

            IEnumerable<IOMailWrapperModel> enumModel = list;

            if (!String.IsNullOrEmpty(searchString))
            {
                int value = int.Parse(searchString);
                enumModel = list.Where(u => u.mail.GuideNumber.Equals(value));
                //incoming = incoming.Where(i => i.Mail.GuideNumber.Equals(value));
            }
            return View(enumModel); //concat two list
        }

        //
        // GET: /Incoming/Details/5

        public ActionResult Details(int id)
        {
            Incoming incoming = db.Incomings.Find(id);
            Outcoming outcoming = db.Outcomings.Find(id);
            Mail mail = db.Mails.Find(id);
            IOMailWrapperModel model = new IOMailWrapperModel();
            model.outcoming = outcoming;
            model.incoming = incoming;
            model.mail = mail;
            return View(model);
        }

        //
        // GET: /Incoming/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Incoming/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Incoming/Edit/5

        public ActionResult Edit(int id)
        {
            Outcoming outcoming = db.Outcomings.Find(id);
            Incoming incoming = db.Incomings.Find(id);
            Mail mail = db.Mails.Find(id);

            ViewBag.To = new SelectList(db.People, "PersonID", "FullName", outcoming.To);
            ViewBag.From = new SelectList(db.People, "PersonID", "FullName", incoming.From);
            ViewBag.MailStatus = new SelectList(db.LMStatus, "MailStatusID", "MailStatusDescription", mail.MailStatus);
            ViewBag.IncomingID = new SelectList(db.Mails, "MailID", "MailID", incoming.IncomingID);

            IOMailWrapperModel model = new IOMailWrapperModel();
            model.outcoming = outcoming;
            model.incoming = incoming;
            model.mail = mail;
            return View(model);
        }

        //
        // POST: /Incoming/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, IOMailWrapperModel model)
        {
            // TODO: Add update logic here         
            model.incoming.IncomingDate = DateTime.Now.ToShortDateString();
            model.incoming.IncomingTime = DateTime.Now.ToShortTimeString();
            try
            {
                if (ModelState.IsValid)
                {
                    Mail currentMail = new Mail()
                    {
                        MailID = model.outcoming.OutcomingID,
                        GuideNumber = model.mail.GuideNumber,
                        MailStatus = 2
                    };

                    model.outcoming.OutcomingMail = currentMail.MailID;
                    model.outcoming.Mail = currentMail;

                    model.incoming.IncomingMail = currentMail.MailID;
                    model.incoming.Mail = currentMail;
                    model.incoming.Description = model.outcoming.Description;
                    //model.outcoming.Observations = model.incoming.Observations;

                    model.mail = currentMail;

                    db.Entry(model.outcoming).State = EntityState.Modified;
                    db.Entry(model.incoming).State = EntityState.Modified;
                    db.Entry(model.mail).State = EntityState.Modified;
                    db.SaveChanges();

                    IOMailWrapperModel SendMail = new IOMailWrapperModel();
                    SendMail = model;

                    int from = model.incoming.From;
                    int to = model.outcoming.To;


                    //Sending the Email Notification
                    SendEmailNotification(from, to, SendMail);

                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "No se guardaron los cambios, Vuelva a intentarlo, si el problema persiste notifique al administrador del sistema.");
            }
            ViewBag.MailStatus = new SelectList(db.LMStatus, "MailStatusID", "MailStatusDescription", model.mail.MailStatus);
            return View();
        }

        private void SendEmailNotification(int from, int to, IOMailWrapperModel mailModel)
        {
            Person incoming = db.People.Find(from);
            Person outcoming = db.People.Find(to);

            //Componiendo el email en HTML
            string notificationFrom = "<p>Le notificamos que se registro la llegada y recepción de la siguiente correspondencia:</p>" +
                    "<table style='width:100%;' align='center' title='Detalle' border='1'>" +
                        "<tr>" +
                            "<td align='center'><strong>Nro de Guía</Strong></td>" +
                            "<td align='center'><strong>Remitente</Strong></td>" +
                            "<td align='center'><strong>Destinatario</Strong></td>" +
                            "<td align='center'><strong>Hora Recepción</Strong></td>" +
                            "<td align='center'><strong>Fecha Recepción</Strong></td>" +
                            "<td align='center'><strong>Descripción</Strong></td>" +
                            "<td align='center'><strong>Cant. Recibida</Strong></td>" +
                            "<td align='center'><strong>Destino</Strong></td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td align='center'>" + mailModel.mail.GuideNumber + "</td>" +
                            "<td align='center'>" + incoming.FullName + "</td>" +
                            "<td align='center'>" + outcoming.FullName + "</td>" +
                            "<td align='center'>" + mailModel.incoming.IncomingTime + "</td>" +
                            "<td align='center'>" + mailModel.incoming.IncomingDate + "</td>" +
                            "<td align='center'>" + mailModel.outcoming.Description + "</td>" +
                            "<td align='center'>" + mailModel.incoming.Quanty + "</td>" +
                            "<td align='center'>" + mailModel.outcoming.City + "</td>" +
                        "</tr>" +
                    "</table>" +
                    "<p>NOTA: Correo electrónico de prueba</p>" +
                    "<p>Este correo electrónico fue generado por el Sistema de Control de Correspondencia y los datos indicados no son reales.</p>" +
                    "<p>Si usted duda de la procedencia de éste correo puede contactarse con Janeth Mendoza (Recepción) o Caleb Espinoza (Asistente IT)</p>" +
                    "<a href='www.compassionbo.com'>Sistema de Control de Correspondencia</a>" +
                    "<hr />";

            //Correo para el Remitente
            MailMessage emailFrom = new MailMessage(incoming.Email.Trim(), incoming.Email.Trim());
            emailFrom.Subject = "Notificación de Envío de Correspondencia - Nro. Guía: " + mailModel.mail.GuideNumber;
            emailFrom.Body = "<p>Saludos " + incoming.FullName + ": </p>" + notificationFrom;
            emailFrom.IsBodyHtml = true;

            //Correo para el Destinatario
            MailMessage emailTo = new MailMessage(outcoming.Email.Trim(), outcoming.Email.Trim());
            emailTo.Subject = "Notificación de Envío de Correspondencia - Nro. Guía: " + mailModel.mail.GuideNumber;
            emailTo.Body = "<p>Saludos " + outcoming.FullName + ": </p>" + notificationFrom;
            emailTo.IsBodyHtml = true;

            SmtpClient notification = new SmtpClient("smtp.myopera.com", 587);
            notification.Credentials = new System.Net.NetworkCredential("compassionbo@myopera.com", "compassionbo");
            notification.EnableSsl = true;

            try
            {
                notification.Send(emailFrom);
                notification.Send(emailTo);
            }
            catch (Exception)
            {

            }
        } 

        //
        // GET: /Incoming/Delete/5

        public ActionResult Delete(int id)
        {
            Outcoming outcoming = db.Outcomings.Find(id);
            Incoming incoming = db.Incomings.Find(id);
            return View();
        }

        //
        // POST: /Incoming/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
