using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.MoyeBuyComSite.Filters;

namespace MoyeBuy.Com.MoyeBuyComSite.Controllers
{
    public class AccountController : Controller
    {
        private string strValidaCode = "";
        ValidateCode vCode = new ValidateCode();
        public AccountController()
        {
            strValidaCode = vCode.CreateValidateCode(4);
        }

        [CompressFilter]
        [UserAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [CompressFilter]
        public ActionResult ValidateCode()
        {
            this.Session.Add("VALCODE", strValidaCode);
            byte[] bytGraphic = vCode.CreateValidateGraphic(strValidaCode);
            return File(bytGraphic, "image/jpeg");
        }

        [HttpPost]
        public ActionResult LogOnProcess(string username, string pwd, string authcode, string chkRememberMe)
        {
            BLL.Account bll = new BLL.Account();
            if (string.IsNullOrEmpty(authcode))
                return Redirect("/account/logon?msg=3");
            if (authcode != this.Session["VALCODE"].ToString())
                return Redirect("/account/logon?msg=4");

            string strResult = "";
            strResult = bll.LogOn(username, pwd);
            if (strResult == MoyeBuyUtility.WebConstant.LoginSuccess)
            {
                Model.User user = bll.GetUserByEmail(username)[0];
                user.MoyeBuyComPwdHash = "";
                user.MoyeBuyComPwdSalt = "";
                user.RoleDesc = "";
                string strSerializUser = MoyeBuyUtility.Gadget.Serialize(user).OuterXml;
                //strSerializUser = MoyeBuyUtility.Encryption.SalRijndaelEncrypt(strSerializUser);
                
                DateTime expiration = DateTime.Now;
                if (!string.IsNullOrEmpty(chkRememberMe))
                    expiration = expiration.AddDays(7);
                else
                    expiration = expiration.AddMinutes(60);

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user.MoyeBuyComUserName, DateTime.Now, expiration, true, strSerializUser);
                string strEntrcyedTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie authorCookie = new HttpCookie(FormsAuthentication.FormsCookieName, strEntrcyedTicket);
                authorCookie.Expires = expiration;

                //this.Session.Add("USER", strSerializUser);
                this.Response.Cookies.Add(authorCookie);
                if (user.RoleName.ToUpper() == "ADMIN")
                    return Redirect("/Admin/home/index");
                else
                    return Redirect("/home/index");
            }
            else if (strResult ==MoyeBuyUtility.WebConstant.PwdIncorrect)
                return Redirect("/account/logon?msg=1");
            else
                return Redirect("/account/logon?msg=2");
        }

        [CompressFilter]
        public ActionResult LogOn(string msg)
        {
            string strErrorMsg = "";
            if (msg == "1")
                strErrorMsg = "账号/密码错误";
            else if (msg == "2")
                strErrorMsg = "账号不存在";
            else if (msg == "3")
                strErrorMsg = "验证码不能为空";
            else if (msg == "4")
                strErrorMsg = "验证码错误";
            else
                strErrorMsg = "";
            ViewBag.Error = strErrorMsg;
            return View();
        }


        public ActionResult LogOut()
        {
            this.Response.Cookies.Clear();
            this.Session.Clear();
            FormsAuthentication.SignOut();
            this.Response.Redirect("/home/index");
            return View();
        }

        [CompressFilter]
        public ActionResult Register(string msg)
        {
            string strErrorMsg = "";
            if (msg == "1")
                strErrorMsg = "用户名不能为空";
            else if (msg == "2")
                strErrorMsg = "密码不能为空";
            else if (msg == "3")
                strErrorMsg = "验证码不能为空";
            else if (msg == "4")
                strErrorMsg = "验证码错误";
            else if (msg == "5")
                strErrorMsg = "系统出错";
            else
                strErrorMsg = "";
            ViewBag.Error = strErrorMsg;
            return View();
        }

        [HttpPost]
        public ActionResult RegisterProcess(string username, string pwd, string authcode)
        {
            if(string.IsNullOrEmpty(username))
                return Redirect("/account/register?msg=1");
            if (string.IsNullOrEmpty(pwd))
                return Redirect("/account/register?msg=2");
            if (string.IsNullOrEmpty(authcode))
                return Redirect("/account/register?msg=3");
            if (authcode != this.Session["VALCODE"].ToString())
                return Redirect("/account/register?msg=4");

            Model.User user = new Model.User();
            user.MoyeBuyComEmail = username;
            user.MoyeBuyComUserName = username;
            user.MoyeBuyComPwdHash = pwd;

            BLL.Account bll = new BLL.Account();
            if (!String.IsNullOrEmpty(bll.Register(user)))
            {
                string strSerializUser = MoyeBuyUtility.Gadget.Serialize(user).OuterXml;
                //strSerializUser = MoyeBuyUtility.Encryption.SalRijndaelEncrypt(strSerializUser);

                DateTime expiration = DateTime.Now;
                expiration = expiration.AddMinutes(30);

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user.MoyeBuyComUserName, DateTime.Now, expiration, true, strSerializUser);
                string strEntrcyedTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie authorCookie = new HttpCookie(FormsAuthentication.FormsCookieName, strEntrcyedTicket);
                authorCookie.Expires = expiration;

                //this.Session.Add("USER", strSerializUser);
                this.Response.Cookies.Add(authorCookie);
                return Redirect("/home/index");
            }
            else
                return Redirect("/account/register?msg=5");
        }

        [HttpPost]
        public JsonResult ChangePassword(string strUID, string strOldPwd, string strNewPwd)
        {
            if (String.IsNullOrEmpty(strUID)||String.IsNullOrEmpty(strOldPwd)||String.IsNullOrEmpty(strNewPwd))
                return Json("FAIL");
            if (strOldPwd != strNewPwd)
                return Json("FAIL");
            BLL.Account bll = new BLL.Account();
            if(bll.ChangePassword(strUID,strNewPwd))
                return Json("SUCCESS");
            else
                return Json("FAIL");
        }

        public ActionResult UpdateUser(Model.User user)
        {
            BLL.Account bll = new BLL.Account();
            if (!String.IsNullOrEmpty(bll.Register(user)))
            {
                return Redirect("/account/index");
            }
            else
                return Redirect("/Error");
            return View();
        }
    }
}
