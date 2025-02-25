﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using PearAdminMvcOA.Models;
using PearAdminMvcOA.Tools;
using System.Web.Security;
using System.Net.Http.Headers;
using System.Web.WebPages;
using System.Net;

namespace PearAdminMvcOA.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        //加载页面
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// 重载一个登录方法
        /// </summary>
        /// <param name="name">用户输入信息</param>
        /// <returns>验证状态</returns>
        [HttpPost]
        public ActionResult Login(string WorkId, string pwd)
        {
            if (!string.IsNullOrEmpty(WorkId) && !string.IsNullOrEmpty(pwd))
            {
                pwd = CommonBLL.Md5Encoding(pwd);
                using (OAEntities db = new OAEntities())
                {
                    UserInfo us = db.UserInfo.FirstOrDefault(n => n.WordId == WorkId && n.PassWord == pwd);
                    if (us != null)
                    {
                        FormsAuthentication.SetAuthCookie(us.UserId.ToString(), false);
                        HttpCookie cookie = new HttpCookie("UserId");
                        Session["User"] = us;
                        LoginLog lg = new LoginLog()
                        {
                            UserId = us.UserId,
                            LoginTime = DateTime.Now,
                            IfSuccess = 1,
                            LoginUserIp = Request.UserHostAddress,
                            LoginDesc = "post",
                            Browser = Request.Browser.Browser
                        };
                        db.LoginLog.Add(lg);
                        db.SaveChanges();
                        cookie.Value = us.UserId.ToString();
                        cookie.Expires = DateTime.Now.AddDays(2);
                        Response.Cookies.Add(cookie);
                        return Json(new { code = 200  });
                    }
                    return Json(new { code = 500 });
                }
            }
            else return Json(new { code = 500 });
        }
        //用户退出
        public ActionResult UserExit()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return View("~/Views/Account/Login.cshtml");
        }


    }
}