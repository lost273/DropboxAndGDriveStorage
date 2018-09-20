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
        static private string token;

        [HttpGet]
        public IActionResult Index(List<string> credentials)
        {
            return View(credentials);
        }
        [HttpPost]
        public IActionResult Login(string dropboxToken)
        {
            token = dropboxToken;
            List<string> loginCredentials = new List<string>();

            using (var dbx = new DropboxClient(token))
            {
                var full = dbx.Users.GetCurrentAccountAsync().Result;
                //loginCredentials.Add(full.Name.DisplayName);
                loginCredentials.Add(full.Email);
            }
            return RedirectToAction("Index", "Home", new { credentials = loginCredentials });
        }

        public IActionResult Content()
        {
            List<string> content = new List<string>();
            using (var dbx = new DropboxClient(token))
            {
                getAllNames(content, dbx, string.Empty);
            }
            return View(content);
        }
        private void getAllNames(List<string> list, DropboxClient client, string path)
        {
            var names = client.Files.ListFolderAsync(path).Result;

            foreach (var item in names.Entries.Where(i => i.IsFolder))
            {
                list.Add($"[ {path}/{item.Name} ]");
                getAllNames(list, client, item.PathDisplay);
            }

            foreach (var item in names.Entries.Where(i => i.IsFile))
            {
                list.Add($"{path}/{item.Name}");
            }
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
