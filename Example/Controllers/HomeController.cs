using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Example.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Login(String ticket)
        {
            ViewData["username"] = null;
            HttpContext.Request.Cookies.TryGetValue("Login", out String loginUser);

            if (loginUser != null)
            {
                ViewData["username"] = loginUser;
                return View();
            }
            else if (!String.IsNullOrEmpty(ticket))
            {
                HttpClient httpClient = new HttpClient();
                String url =
                    "http://localhost:5000/Api/LoginVerification?ticket=" + ticket + "&appkey=abcd";
                var response = httpClient.GetAsync(url).Result;
                var resStr = await response.Content.ReadAsStringAsync();

                LoginReturn ret = JsonConvert.DeserializeObject<LoginReturn>(resStr);
                
                HttpContext.Response.Cookies.Append("Login", ret.Username, new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30)
                });

                ViewData["username"] = ret.Username;
            }
            else
            {
                return Redirect("http://localhost:5000/Account?appkey=abcd");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}