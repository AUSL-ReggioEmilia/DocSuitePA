using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace VecompSoftware.DocSuite.SPID.AuthEngine.Helpers
{
    public static class CookiesExtension
    {
        public static string GetCookie(this Controller controller, string key)
        {
            return controller.Request.Cookies[key];
        }

        public static void SetCookie(this Controller controller, string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions() { SameSite = SameSiteMode.None };
            if (expireTime.HasValue)
            {
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            }
            else
            {
                option.Expires = DateTime.Now.AddMilliseconds(10);
            }
            controller.Response.Cookies.Append(key, value, option);
        }

        public static void RemoveCookie(this Controller controller, string key)
        {
            controller.Response.Cookies.Delete(key);
        }
    }
}
