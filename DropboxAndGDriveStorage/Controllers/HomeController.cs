using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DropboxAndGDriveStorage.Models;
using Dropbox.Api;

namespace DropboxAndGDriveStorage.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public string Access(string dropboxToken)
        {
            string name = "";
            using (var dbx = new DropboxClient(dropboxToken))
            {
                var full = dbx.Users.GetCurrentAccountAsync().Result;
                //Console.WriteLine("{0} - {1}", full.Name.DisplayName, full.Email);
                name = name + $"Name: {full.Name.DisplayName}, email: {full.Email}";
            }

            return name;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
