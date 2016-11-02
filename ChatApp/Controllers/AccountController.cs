using ChatApp.Common;
using ChatApp.Domain.Abstract;
using ChatApp.Models;
using System;
using System.Web.Mvc;

namespace ChatApp.Controllers
{
    public class AccountController : Controller
    {
        private IUser _UserRepo;
        public AccountController(IUser UserRepo)
        {
            this._UserRepo = UserRepo;
        }
        [HttpGet]
        public ActionResult Login()
        {
            if (MySession.Current.UserID > 0)
            {
                return RedirectToAction("Chat", "User");
            }
            UserModel objmodel = new UserModel();
            return View(objmodel);
        }
        [HttpPost]
        public ActionResult Login(UserModel objmodel)
        {
            if (objmodel.FormType == "Login")
            {
                return CreateLogin(objmodel);
            }
            else
            {
                ChatApp.Domain.Entity.User objentity = new Domain.Entity.User();
                objentity.CreatedOn = System.DateTime.Now;
                objentity.IsActive = true;
                objentity.Name = objmodel.Name;
                objentity.Password = objmodel.Password1;
                objentity.UpdatedOn = System.DateTime.Now;
                objentity.UserName = objmodel.UserName1;
                objentity.DOB = Convert.ToDateTime(objmodel.DOB);
                objentity.Gender = objmodel.Gender;
                var result = _UserRepo.SaveUser(objentity);
                if (!string.IsNullOrEmpty(result.Item2))
                {
                    objmodel.Error = result.Item2;
                    TempData["ReturnFrom"] = "SignUp";
                    return View("Login", objmodel);
                }
                objmodel.UserName = objmodel.UserName1;
                objmodel.Password = objmodel.Password1;
                return CreateLogin(objmodel);
            }
        }
        public ActionResult CreateLogin(UserModel objmodel)
        {
            var result = _UserRepo.CheckLogin(objmodel.UserName, objmodel.Password);
            if (result != null)
            {
                MySession.Current.UserID = result.UserID;
                MySession.Current.Name = result.Name;
                MySession.Current.ProfilePicture = CommonFunctions.GetProfilePicture(result.ProfilePicture, result.Gender); ;
                return RedirectToAction("Chat", "User");
            }
            else
            {
                TempData["ReturnFrom"] = "Login";
                objmodel.LoginError = "Login credentials is not valid. Please try again.";
                return View("Login", objmodel);
            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
