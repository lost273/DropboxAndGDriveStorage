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
        private string token;

        [HttpGet]
        public IActionResult Index(string loginName)
        {
            return View(loginName);
        }
        [HttpPost]
        public IActionResult Login(string dropboxToken)
        {
            this.token = dropboxToken;
            string name = "";
            using (var dbx = new DropboxClient(this.token))
            {
                var full = dbx.Users.GetCurrentAccountAsync().Result;
                name = name + $"Name: {full.Name.DisplayName}, email: {full.Email}";
            }
            return RedirectToAction("Index", "Home", new { loginName = name });
        }

        public List<string> Content()
        {
            List<string> content = new List<string>();
            using (var dbx = new DropboxClient(this.token))
            {
                var list = dbx.Files.ListFolderAsync(string.Empty).Result;

                foreach (var item in list.Entries.Where(i => i.IsFolder))
                {
                    content.Add(item.Name);
                }

                foreach (var item in list.Entries.Where(i => i.IsFile))
                {
                    content.Add(item.AsFile.Size + " " + item.Name);
                }
            }
            return content;
        }

        public IActionResult Download()
        {
            ViewData["Message"] = "Your download page.";

            return View();
        }

        public IActionResult Upload()
        {
            ViewData["Message"] = "Your upload page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
