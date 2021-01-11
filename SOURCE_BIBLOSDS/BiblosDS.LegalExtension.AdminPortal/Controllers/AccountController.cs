using System;
using System.Web.Mvc;
using System.Web.Security;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.LegalExtension.AdminPortal.Infrastructure.Services.Common;
using BiblosDS.LegalExtension.AdminPortal.Models;
using BiblosDS.Library.Common.Services;
using log4net;

namespace BiblosDS.LegalExtension.AdminPortal.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        #region [ Fields ]
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountController));
        private readonly ILogger _loggerService;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public AccountController()
        {
            _loggerService = new LoggerService(_logger);
        }
        #endregion

        #region [ Methods ]        
        [NoCache]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }, _loggerService);
        }

        [NoCache]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (CustomerService.CustomerLoginExists(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Lo username o la password forniti non sono corretti.");
                }
                return View(model);
            }, _loggerService);
        }

        [NoCache]
        public ActionResult LogOff()
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                FormsAuthentication.SignOut();
                Session["idCompany"] = null;
                return RedirectToAction("Index", "Home");
            }, _loggerService);
        }

        [NoCache]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [NoCache]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                bool changePasswordSucceeded;
                try
                {
                    CustomerService.ChangeCustomerPassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    changePasswordSucceeded = true;
                }
                catch (Exception ex)
                {
                    _loggerService.Error(ex.Message, ex);
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Errore nella procedura di cambio password.");
                }
                return View(model);
            }, _loggerService);
        }

        [NoCache]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        #endregion
    }
}
