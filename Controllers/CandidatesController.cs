using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeRegistery.Data;

namespace EmployeeRegistery.Controllers
{
    [Authorize]
    public class CandidatesController : Controller
    {
        private UserRegisteryEntities db = new UserRegisteryEntities();

        // GET: Users
        public ActionResult Index(string sorting = "asc", string sortingparam = "Name", string searchParam = "", double exp = 0)
        {
            ViewBag.sorting = sorting;
            ViewBag.sortingparam = sortingparam;
            ViewBag.searchParam = searchParam;
            ViewBag.exp = exp;
            ViewBag.exerienceList = (from us in db.Users
                                     select us.Experience).Where(x=> x != null).Distinct().ToList();
            List<User> users = (from us in db.Users
                                join rol in db.UserRoles on us.Id equals rol.UserId
                                where rol.RoleId == 2
                                select us).ToList();
            if (searchParam != "")
                users = users.Where(x => x.FirstName.ToLower().Contains(searchParam.ToLower())).ToList();
            if (exp != 0)
                users = users.Where(x => x.Experience >= exp).ToList();
            switch (sortingparam)
            {
                case "Name":
                    if (sorting == "desc")
                    {
                        ViewBag.sorting = "asc";
                        users = users.OrderByDescending(x => x.FirstName).ToList();
                    }
                    else
                    {
                        ViewBag.sorting = "desc";
                        users = users.OrderBy(x => x.FirstName).ToList();
                    }
                    break;
                case "Email":
                    if (sorting == "desc")
                    {
                        ViewBag.sorting = "asc";
                        users = users.OrderByDescending(x => x.Email).ToList();
                    }
                    else
                    {
                        ViewBag.sorting = "desc";
                        users = users.OrderBy(x => x.Email).ToList();
                    }
                    break;
                default:
                    break;
            }
            return View(users);
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user, HttpPostedFileBase file)
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
                if (user.ToDate != null && user.FromDate != null)
                {
                    TimeSpan diff = (TimeSpan)(user.ToDate - user.FromDate);
                    int totalmonth = diff.Days / 30;
                    int year = totalmonth / 12;
                    int month = totalmonth - (year * 12);
                    user.Employer = user.Employer;
                    user.ToDate = user.ToDate;
                    user.FromDate = user.FromDate;
                    user.Experience = Double.Parse(string.Concat(year, '.', month));
                }
                user.UserRoles.Add(new UserRole
                {
                    RoleId = db.Roles.Where(x => x.Name == "Employe").FirstOrDefault().Id,
                    UserId = user.Id
                });
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            foreach (var role in db.UserRoles)
            {
                if (role.UserId == id)
                    db.UserRoles.Remove(role);
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
