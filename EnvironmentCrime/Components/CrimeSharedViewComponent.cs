using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Component
{
    public class CrimeSharedViewComponent:ViewComponent
    {
        private IECrimeRepository repository;

        public CrimeSharedViewComponent(IECrimeRepository repo)
        {
            repository = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var errandDetail = await repository.GetErrandDetail(id);
            return View(errandDetail);
        }
    }
}
