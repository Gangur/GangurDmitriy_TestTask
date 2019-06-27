using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeverageVendingMachine_ASP.NET_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeverageVendingMachine_ASP.NET_Core.Pages
{
    public class IndexModel : PageModel
    {
        public ApplicationContext db;
        public void OnGet()
        {
            db = new ApplicationContext();
        }

        protected internal string Hello()
        {
            return "Hello ASP.NET";
        }
    }
}
