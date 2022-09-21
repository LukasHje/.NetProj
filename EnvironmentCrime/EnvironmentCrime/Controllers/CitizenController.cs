using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;

namespace EnvironmentCrime.Controllers
{
    public class CitizenController : Controller
    {
        private IECrimeRepository repository;
        public CitizenController(IECrimeRepository repo)
        {
            repository = repo;
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }


        /*
         * Creates a session which holds the errand-info a user types into the form.
         * The session, with errand-info from the form, is saved until the user press the thanks-action
         */
        [HttpPost]
        public IActionResult Validate(Errand er)
        {
            HttpContext.Session.SetJson("NewErrand", er);
            return View(er);
        }

        /*
         * The thanks-action gets the saved session and stores it in a variabel, 
         * the methode the use the variabel to save the errand too the repository
         * and then returns the errand so the user can see what errandID their reported crime got.
         */
        public IActionResult Thanks()
        {
            var myErrand = HttpContext.Session.GetJson<Errand>("NewErrand");
            repository.SaveErrand(myErrand);
            HttpContext.Session.Remove("NewErrand");
            return View(myErrand);
        }

        

    }
}
