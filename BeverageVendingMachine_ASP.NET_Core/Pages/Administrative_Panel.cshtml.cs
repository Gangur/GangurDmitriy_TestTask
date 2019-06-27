using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeverageVendingMachine_ASP.NET_Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BeverageVendingMachine_ASP.NET_Core.Pages
{
    public class AdministrativePanel : PageModel
    {
        public ApplicationContext db;
        IHostingEnvironment _appEnvironment;

        public AdministrativePanel(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public async Task<IActionResult> OnPostAddExemplar(Beverage beverage, IFormFile Photo)
        {
            beverage.Created = DateTime.Now;
            if (Photo != null)
            {
                // путь к папке Files
                string path = "/image/" + Photo.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
                beverage.PathToImage = path;
            }

            using (db = new ApplicationContext())
            {
                db.Beverage.Add(beverage);

                await db.SaveChangesAsync();
                return Redirect("Administrative_Panel");
            }
        }

        public async Task<IActionResult> OnPostEditExemplar(Beverage beverage, IFormFile Photo)
        {
            if (Photo != null)
            {
                FileInfo fileInf = new FileInfo(_appEnvironment.WebRootPath + beverage.PathToImage);
                if (fileInf.Exists)
                    fileInf.Delete();

                string path = "/image/" + Photo.FileName;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
                beverage.PathToImage = path;
            }

            using (db = new ApplicationContext())
            {
                Beverage original_beverage = db.Beverage.Find(beverage.Id);

                original_beverage.Name = beverage.Name;
                original_beverage.Price = beverage.Price;
                original_beverage.Calories = beverage.Calories;
                original_beverage.Amount = beverage.Amount;
                original_beverage.Description = beverage.Description;
                if(Photo != null)
                    original_beverage.PathToImage = beverage.PathToImage;

                await db.SaveChangesAsync();

                return Redirect("Administrative_Panel");
            }
        }

        public async Task<IActionResult> OnPostRemoveExemplar(int Id)
        {
            using (db = new ApplicationContext())
            {
                Beverage beverage = db.Beverage.Find(Id);

                FileInfo fileInf = new FileInfo(_appEnvironment.WebRootPath + beverage.PathToImage);
                if (fileInf.Exists)
                    fileInf.Delete();

                Beverage original_beverage = db.Beverage.Find(beverage.Id);
                original_beverage.Removed = DateTime.Now;

                await db.SaveChangesAsync();
                return Redirect("Administrative_Panel");
            }
        }

        private string Login = "89041486500";
        private string Password = "Gangur";

        public IActionResult OnPostAuthentication(string login, string password)
        {
            if(Login == login && Password == password && Startup.Attempts > 0)
            {
                Access.ChangeAccess();

                Thread TimerThread = new Thread(new ThreadStart(Access.TimerAccess));
                TimerThread.Start();

                Access.RecoverAttempts();
            }
            else
            {
                Startup.Attempts--;
            }

            if(Startup.Attempts <= 0)
            {
                Thread TimerThread = new Thread(new ThreadStart(Access.TimerAttempts));
                TimerThread.Start();
            }

           return Redirect("Administrative_Panel");
        }

        public void OnGet()
        {
            db = new ApplicationContext();
        }
    }
}