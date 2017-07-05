using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AuthDemo.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User user, string password)
        {
            var db = new UserAuthDb(Properties.Settings.Default.ConStr);
            db.AddUser(user, password);
            return Redirect("/");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var db = new UserAuthDb(Properties.Settings.Default.ConStr);
            var user = db.Login(email, password);
            if (user == null)
            {
                return Redirect("/home/login");
            }
            FormsAuthentication.SetAuthCookie(email, true);
            return Redirect("/home/secret");
        }

        [Authorize]
        public ActionResult Secret()
        {
            bool isLoggedIn = User.Identity.IsAuthenticated;// true/false if user is logged in
            string email = User.Identity.Name; //will always match the first argument in SetAuthCookie
            var db = new UserAuthDb(Properties.Settings.Default.ConStr);
            User user = db.GetByEmail(email);
            return View(new SecretPageViewModel {User = user});
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    }
}