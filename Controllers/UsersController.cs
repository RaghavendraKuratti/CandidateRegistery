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
    public class UsersController : Controller
    {
        private UserRegisteryEntities db = new UserRegisteryEntities();

        // GET: Users
        public ActionResult Index(string sorting = "asc", string sortingparam = "Name", string searchParam = "")
        {
            ViewBag.sorting = sorting;
            ViewBag.sortingparam = sortingparam;
            ViewBag.searchParam = searchParam;
            List<User> users = (from us in db.Users
                                join rol in db.UserRoles on us.Id equals rol.UserId
                                where rol.RoleId == 1
                                select us).ToList();
            if (searchParam != "")
                users = users.Where(x => x.FirstName.ToLower().Contains(searchParam.ToLower())).ToList();
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
