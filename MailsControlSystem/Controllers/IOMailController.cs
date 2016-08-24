using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailsControlSystem.Models;
using MailsControlSystem.Models.MembershipProvider;
using MailsControlSystem.DAL;
using System.Web.Helpers;
using System.Net.Mail;
using System.Web.Security;

namespace MailsControlSystem.Controllers
{
    public class IOMailController : Controller
    {
        private MailDBContext db = new MailDBContext();
        private CustomRoleProvider provider = new CustomRoleProvider();
        //
        // GET: /IOMail/

        //public ActionResult Index()
        //{    
        //    return View();
        //}
        [Authorize(Roles = "Administrador,Recepción, Enlace")]
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
            for (int i  = 0; i  < listOutcoming.Count; i ++)
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
                        //Cheking if the Outcoming have been created by the current User
                        if(model.outcoming.Sender != null && model.outcoming.Sender.Equals(currentUser))
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
            }
            return View(enumModel); 
        }

        //
        // GET: /IOMail/Details/5

        public ActionResult Details(int id)
        {
            Outcoming outcoming = db.Outcomings.Find(id);
            Incoming incoming = db.Incomings.Find(id);
            IOMailWrapperModel model = new IOMailWrapperModel();
            model.outcoming = outcoming;
            model.incoming = incoming;
            return View(model);
        }
        //
        // GET: /IOMail/Create

        public ActionResult Create()
        {      
            ViewBag.To = new SelectList(db.People, "PersonID", "FullName");
            ViewBag.From = new SelectList(db.People, "PersonID", "FullName");
            return View();
        }

        //
        // POST: /IOMail/Create

        [HttpPost,AcceptVerbs(HttpVerbs.Post) ] 
        public ActionResult Create(IOMailWrapperModel mailModel)
        {
            // Sorry Caleb,
            // by some reason I cannot get the value directly  from the DropDownList, in that case I'd to populate from the request
            int from = int.Parse(Request.Form["From"]); 
            int to = int.Parse(Request.Form["To"]);

            //Adding the Accunt that was created the Outcoming
            string sender = User.Identity.Name;

            mailModel.outcoming.To = to;
            mailModel.incoming.From = from;

            if (ModelState.IsValid)
            {
                Mail currentMail = new Mail()
                {
                    MailID = mailModel.outcoming.OutcomingID,
                    GuideNumber = mailModel.mail.GuideNumber,
                    MailStatus = 1
                };

                mailModel.outcoming.OutcomingMail = currentMail.MailID;
                mailModel.outcoming.Mail = currentMail;
                mailModel.outcoming.Sender = sender;

                mailModel.incoming.IncomingMail = currentMail.MailID;
                mailModel.incoming.Mail = currentMail;
                mailModel.incoming.Description = mailModel.outcoming.Description;
                mailModel.incoming.Quanty = "0";
              
                db.Outcomings.Add(mailModel.outcoming);
                db.Incomings.Add(mailModel.incoming);                
                db.SaveChanges();

                IOMailWrapperModel SendMail = new IOMailWrapperModel();
                SendMail = mailModel;

                //Sending the Email Notification
                SendEmailNotification(from, to, SendMail);
                return RedirectToAction("Index");
            }
            return View(mailModel);
        }

        /// <summary>
        /// Send a Email Notification to Senter and Receiver
        /// </summary>
        /// <param name="from">Id for Senter</param>
        /// <param name="to">Id for Reciever</param>
        private void SendEmailNotification(int from, int to, IOMailWrapperModel mailModel)
        {
            Person incoming = db.People.Find(from);
            Person outcoming = db.People.Find(to);

            //Componiendo el email en HTML
            string notificationFrom = "<p>Le notificamos que se registro el envío de correspondencia con el siguiente detalle:</p>" +
                    "<table style='width:100%;' align='center' title='Detalle' border='1'>" +
                        "<tr>" +
                            "<td align='center'><strong>Nro de Guía</Strong></td>" +
                            "<td align='center'><strong>Remitente</Strong></td>" +
                            "<td align='center'><strong>Destinatario</Strong></td>" +
                            "<td align='center'><strong>Hora de Envío</Strong></td>" +
                            "<td align='center'><strong>Fecha de Envío</Strong></td>" +
                            "<td align='center'><strong>Descripción</Strong></td>" +
                            "<td align='center'><strong>Cantidad</Strong></td>" +
                            "<td align='center'><strong>Destino</Strong></td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td align='center'>" + mailModel.mail.GuideNumber + "</td>" +
                            "<td align='center'>" + incoming.FullName + "</td>" +
                            "<td align='center'>" + outcoming.FullName + "</td>" +
                            "<td align='center'>" + mailModel.outcoming.SendTime + "</td>" +
                            "<td align='center'>" + mailModel.outcoming.SendDate + "</td>" +
                            "<td align='center'>" + mailModel.outcoming.Description + "</td>" +
                            "<td align='center'>" + mailModel.outcoming.Quanty + "</td>" +
                            "<td align='center'>" + mailModel.outcoming.City + "</td>" +
                        "</tr>" +
                    "</table>" +
                    "<p>NOTA: Correo electrónico de prueba</p>" +
                    "<p>Este correo electrónico fue generado por el Sistema de Control de Correspondencia y los datos indicados no son reales.</p>" +
                    "<p>Si usted duda de la procedencia de éste correo puede contactarse con Janeth Mendoza (Recepción) o Caleb Espinoza (Asistente IT)</p>"+
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
        // GET: /IOMail/Edit/5

        public ActionResult Edit(int id)
        {
            Outcoming outcoming = db.Outcomings.Find(id);
            Incoming incoming = db.Incomings.Find(id);
            Mail mail = db.Mails.Find(id);

            ViewBag.To = new SelectList(db.People, "PersonID", "FullName", outcoming.To);
            ViewBag.From = new SelectList(db.People,"PersonID", "FullName", incoming.From);
            ViewBag.OutcomingID = new SelectList(db.Mails, "MailID", "MailID", outcoming.OutcomingID);         
            IOMailWrapperModel model = new IOMailWrapperModel();
            model.outcoming = outcoming;
            model.incoming = incoming;
            model.mail = mail;
            return View(model);
        }

        //
        // POST: /IOMail/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, IOMailWrapperModel model)
        {
            // TODO: Add update logic here
            int from = int.Parse(Request.Form["From"]);
            int to = int.Parse(Request.Form["To"]);
            string cheked = Request.Form["DateTime"];
      
            model.outcoming.To = to;
            model.incoming.From = from;
            try 
            {
                if (ModelState.IsValid)
                {
                    Mail currentMail = new Mail()
                    {
                        MailID = model.outcoming.OutcomingID,
                        GuideNumber = model.mail.GuideNumber,
                        MailStatus = 1
                    };

                    model.outcoming.OutcomingMail = currentMail.MailID;
                    model.outcoming.Mail = currentMail;

                    model.incoming.IncomingMail = currentMail.MailID;
                    model.incoming.Mail = currentMail;
                    model.incoming.Description = model.outcoming.Description;
                    model.incoming.Quanty = "0";
                    //model.incoming.Observations = model.outcoming.Observations;

                    model.mail = currentMail;

                    db.Entry(model.outcoming).State = EntityState.Modified;
                    db.Entry(model.incoming).State = EntityState.Modified;
                    db.Entry(model.mail).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "No se guardaron los cambios, Vuelva a intentarlo, si el problema persiste notifique al administrador del sistema.");
            }
            ViewBag.To = new SelectList(db.People, "PersonID", "FullName", model.outcoming.To);
            ViewBag.From = new SelectList(db.People, "PersonID", "FullName", model.incoming.From);
            return View();
        }

        //
        // GET: /IOMail/Delete/5

        public ActionResult Delete(int id)
        {
            Outcoming outcoming = db.Outcomings.Find(id);
            Incoming incoming = db.Incomings.Find(id);
            IOMailWrapperModel model = new IOMailWrapperModel();
            model.outcoming = outcoming;
            model.incoming = incoming;
            return View(model);
        }

        //
        // POST: /IOMail/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            
            try
            {
                // TODO: Add delete logic here
                Outcoming outcoming = db.Outcomings.Find(id);
                Incoming incoming = db.Incomings.Find(id);
                Mail mail = db.Mails.Find(id);

                Mail m = new Mail();
                UserAccount to = new UserAccount();
                UserAccount from = new UserAccount();
                Outcoming o = new Outcoming();

                m = mail;
                to = outcoming.UserAccount;
                from = incoming.UserAccount;
                o = outcoming;

                string emailTo = outcoming.UserAccount.Person.Email;
                string emailFrom = incoming.UserAccount.Person.Email;

                db.Outcomings.Remove(outcoming);
                db.Incomings.Remove(incoming);
                db.Mails.Remove(mail);
                SendDeleteEmailNotification(m, to, from, o);
                db.SaveChanges();
                //SendDeleteEmailNotification(incoming.UserAccount.Person.Email,outcoming.UserAccount.Person.Email);
                
                return RedirectToAction("Index","IOMail");
            }
            catch
            {
                return View();
            }
        }

        private void SendDeleteEmailNotification(Mail mail, UserAccount to, UserAccount from, Outcoming outcoming)//string from, string to)
        {
            string notification = "<p>Le notificamos que el envío de correspondencia registrado con los siguientes datos: </p>" +
                "<p><strong>Nro de Guía: </strong> " + mail.GuideNumber + " </p>" +
                "<p><strong>Remitente: </strong> " + from.Person.FullName + "</p>"+
                "<p><strong>Enviado para: </strong> " + to.Person.FullName+ " </p>" +
                "<p><strong>Descripción: </strong> " + outcoming.Description + " </p>" +
                "<p><strong>Cantidad: </strong> " + outcoming.Quanty + " </p>" +
                "<p><strong>Destino: </strong> " + outcoming.City + " </p>" +
                "<p><strong>HA SIDO CANCELADO</strong></p>" +
                "<p>NOTA: Correo electrónico de prueba</p>" +
                "<p>Este correo electrónico fue generado por el Sistema de Control de Correspondencia y los datos indicados no son reales.</p>" +
                "<p>Si usted duda de la procedencia de éste correo puede contactarse con Janeth Mendoza (Recepción) o Caleb Espinoza (Asistente IT)</p>";

            //Correo para el Remitente
            MailMessage emailFrom = new MailMessage(from.Person.Email.Trim(), from.Person.Email.Trim());
            emailFrom.Subject = "Notificación Correspondencia Cancelada - Nro. Guía: " + mail.GuideNumber;
            emailFrom.Body = "<p>Saludos " + from.Person.FullName + ": </p>" + notification;
            emailFrom.IsBodyHtml = true;

            //Correo para el Destinatario
            MailMessage emailTo = new MailMessage(to.Person.Email.Trim(), to.Person.Email.Trim());
            emailTo.Subject = "Notificación Correspondencia Cancelada - Nro. Guía: " + mail.GuideNumber;
            emailTo.Body = "<p>Saludos " + to.Person.FullName + ": </p>" + notification;
            emailTo.IsBodyHtml = true;

            SmtpClient notificationSend = new SmtpClient("smtp.myopera.com", 587);
            notificationSend.Credentials = new System.Net.NetworkCredential("compassionbo@myopera.com", "compassionbo");
            notificationSend.EnableSsl = true;
            try
            {
                notificationSend.Send(emailFrom);
                notificationSend.Send(emailTo);
            }
            catch (Exception)
            {

            }
        }
    }
}
