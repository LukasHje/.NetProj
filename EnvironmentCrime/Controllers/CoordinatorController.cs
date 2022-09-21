using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;

namespace EnvironmentCrime.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : Controller
    {
        private IECrimeRepository repository;
        public CoordinatorController(IECrimeRepository repo)
        {
            repository = repo;
        }
        public IActionResult CrimeCoordinator(int id)
        {
            ViewBag.ID = id;
            ViewBag.ListOfDepartments = repository.Departments;

            TempData["ID"] = id;
            return View();
        }

        public IActionResult StartCoordinator()
        {
            return View(repository);
        }

        /*
         * When the coordinator goes too the view where you can report a crime this method checks 
         * whether there is already an errand getting reported or not.
         * If: No report has been started, returns only the view 
         * else: a report has already started to be filled in, then that session is returned 
         */
        public IActionResult ReportCrime()
        {
            var myErrand = HttpContext.Session.GetJson<Errand>("NewCoordinatorErrand");
            if (myErrand == null)
            {
                return View();
            }
            else
            {
                return View(myErrand);
            }
        }

        /*
         * The thanks-action gets the saved session and stores it in a variabel, 
         * the methode the use the variabel to save the errand too the repository
         * and then returns the errand so the user can see what errandID their reported crime got.
         */
        public IActionResult Thanks()
        {
            var myErrand = HttpContext.Session.GetJson<Errand>("NewCoordinatorErrand");
            repository.SaveErrand(myErrand);
            HttpContext.Session.Remove("NewCoordinatorErrand");
            return View(myErrand);
        }

        /*
        * Creates a session which holds the errand-info a user types into the form.
        * The session, with errand-info from the form, is saved until the user press the thanks-action
        */
        public IActionResult Validate(Errand er)
        {
            HttpContext.Session.SetJson("NewCoordinatorErrand", er);
            return View(er);
        }

        /*
        * Method used to assign and save a new department too an errand.  
        */
        public IActionResult UpdateDepartment(string DepartmentId)
        {
            int someID = int.Parse(TempData["ID"].ToString());

            repository.CoordinatorUpdate(someID, DepartmentId);

            return RedirectToAction("CrimeCoordinator", new { id = someID });
        }

    }
}
