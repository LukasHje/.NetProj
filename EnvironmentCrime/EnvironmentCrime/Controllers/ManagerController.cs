using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnvironmentCrime.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private IECrimeRepository repository;
        public ManagerController(IECrimeRepository repo)
        {
            repository = repo;
        }

        //this method includes the dynamic and uniqe table-content for an manager
        public IActionResult CrimeManager(int id)
        {
            ViewBag.ID = id;
            ViewBag.ListOfEmployees = repository.DepartmentEmployees();

            TempData["ID"] = id;
            return View();
        }

        public IActionResult StartManager()
        {
            return View(repository);
        }

        /*
        * Method used to assign and save a new employee (investigator) too an errand.  
        */
        public IActionResult UpdateEmployee(bool noAction, string EmployeeId, string reason)
        {
            int someID = int.Parse(TempData["ID"].ToString());

            if (noAction == true)
            {
                repository.ManagerNoAction(someID, reason);

                return RedirectToAction("CrimeManager", new { id = someID });
            }
            else
            {
                repository.ManagerUpdate(someID, EmployeeId);

                return RedirectToAction("CrimeManager", new { id = someID });
            }
        }

    }
}
