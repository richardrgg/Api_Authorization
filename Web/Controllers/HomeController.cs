using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public IActionResult Index()
        {
            var model = new LoginViewModel { Username = "richard123", Password = "123456" };
            var tokenResponse = new TokenResponse();
;           Task.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    var responseHttp = await client.PostAsJsonAsync("http://localhost:3524/api/account/login", model);
                    tokenResponse = await responseHttp.Content.ReadAsAsync<TokenResponse>();
                }

            }).GetAwaiter().GetResult();

            Set("token", tokenResponse.Token, 30);
            Set("token_refresh", tokenResponse.RefreshToken, 30);

            return View();
        }

        public void Set(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);
            Response.Cookies.Append(key, value, option);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            string token_refrehs = _httpContextAccessor.HttpContext.Request.Cookies["token_refresh"];

            string token1 = Request.Cookies["token"];
            string token_refrehs1 = Request.Cookies["token_refresh"];

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
