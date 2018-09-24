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
                list.Add($"{path}/{item.Name}");
                getAllNames(list, client, item.PathDisplay);
            }

            foreach (var item in names.Entries.Where(i => i.IsFile))
            {
                list.Add($"{path}/{item.Name}");
            }
        }
        public void Delete([FromBody]List<string> paths)
        {
            using (var dbx = new DropboxClient(token))
            {
                foreach (string s in paths)
                {
                    var result = dbx.Files.DeleteV2Async(s).Result;
                }
            }
        }

        public FileResult Download([FromBody]List<string> paths)
        {
            byte[] fileBytes = null;

            using (var dbx = new DropboxClient(token))
            {
                using (var response = dbx.Files.DownloadAsync(paths.FirstOrDefault()).Result)
                {
                    fileBytes = response.GetContentAsByteArrayAsync().Result;
                }
            }

            if (fileBytes == null)
            {
                return null;
            }

            var contentDispositionHeader = new System.Net.Mime.ContentDisposition
            {
                Inline = false,
                FileName = "someFilename.txt"
            };

            Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet);
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
