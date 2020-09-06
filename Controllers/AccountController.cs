using EmployeeRegistery.Data;
using EmployeeRegistery.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using System.Data.Entity;
using System.Configuration;

namespace EmployeeRegistery.Controllers
{
    public class AccountController : Controller
    {
        private UserRegisteryEntities db = new UserRegisteryEntities();
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModal modal)
        {
            User user = (from u in db.Users
                         join ur in db.UserRoles on u.Id equals ur.UserId
                         where u.Email == modal.Email && u.Password == modal.Password && ur.RoleId == 1
                         select u).FirstOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(modal.Email, false);
                return RedirectToAction("Index", "Candidates");
            }
            else
            {
                ViewBag.Error = "Invalid Credentials";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(User user, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath("~/Profiles"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    user.ProfilePic = file.FileName;
                }
                else
                {
                    user.ProfilePic = string.Empty;
                }
                db.Users.Add(user);
                user.UserRoles.Add(new UserRole
                {
                    RoleId = db.Roles.Where(x => x.Name == "User").FirstOrDefault().Id,
                    UserId = user.Id
                });
                db.SaveChanges();
                try
                {
                    SendMail(user.Email, user.Id);
                    ViewBag.Message = "Please check Your Gmail for Furthure Process";
                }
                catch(Exception ex)
                {

                    ViewBag.Message = "Somthing Went Wrong. Please enable Less security to GMAIL https://myaccount.google.com/u/0/lesssecureapps?pli=1";
                }
            }
            return View();
        }

        public void SendMail(string ToMail,int userid)
        {
            Gmail mail = new Gmail
            {
                To = ToMail,
                From = ConfigurationManager.AppSettings["SMTPFromMail"].ToString(),
                Subject = "Password Setting",
                Body = ConfigurationManager.AppSettings["PasswordUpdateUrl"].ToString() + userid
            };
            MailMessage mm = new MailMessage(mail.From, mail.To);
            mm.Body = mail.Body;
            mm.Subject = mail.Subject;
            mm.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential(mail.From, ConfigurationManager.AppSettings["SMTPFromMailPWD"].ToString());
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);
        }

        public ActionResult PasswordUpdate(int id)
        {
            PasswordModal modal = new PasswordModal
            {
                UserId = id
            };
            return View(modal);
        }
        [HttpPost]
        public ActionResult PasswordUpdate(PasswordModal modal)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.Find(modal.UserId);
                user.Password = modal.Password;
                db.SaveChanges();
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpGet]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}