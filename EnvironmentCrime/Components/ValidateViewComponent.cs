using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Component
{
    public class ValidateViewComponent : ViewComponent
    {
        private IECrimeRepository repository;

        public ValidateViewComponent(IECrimeRepository repo)
        {
            repository = repo;
        }

        //fungerar inte att hämta session till validate sidorna..........
        public async Task<IViewComponentResult> InvokeAsync(Errand er)
        {
            return View(er);
        }
    }
}
