using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace EnvironmentCrime.Controllers
{
    [Authorize(Roles = "Investigator")]
    public class InvestigatorController : Controller
    {
        private IECrimeRepository repository;
        private IWebHostEnvironment environment;
        public InvestigatorController(IECrimeRepository repo, IWebHostEnvironment env)
        {
            repository = repo;
            environment = env;
        }

        //this method includes the dynamic and uniqe table-content for an investigator
        public IActionResult CrimeInvestigator(int id)
        {
            ViewBag.ID = id;
            ViewBag.ListOfStatuses = repository.ErrandStatuses;

            TempData["ID"] = id;
            return View();
        }

        public IActionResult StartInvestigator()
        {
            return View(repository);
        }

        /*
        * Method used to assign and save a; status, investigator-event & info aswell as sample- and image-files too an errand.  
        */
        public async Task<IActionResult> UpdateStatus(string StatusId, string events, string information, IFormFile loadSample, IFormFile loadImage)
        {
            int someID = int.Parse(TempData["ID"].ToString());

            String picName = "";
            String sampleName = "";

            /*
             * save sample
             */
            if (loadSample != null)
            {
                if (loadSample.Length > 0)
                {
                    //temporary path for file
                    var tempPathSample = Path.GetTempFileName();
                    using (var stream = new FileStream(tempPathSample, FileMode.Create))
                    {
                        await loadSample.CopyToAsync(stream);
                    }

                    //creats a new path
                    var path = Path.Combine(environment.WebRootPath, "uploadedSamples", loadSample.FileName);

                    //moves the temporary file to the correct map
                    System.IO.File.Move(tempPathSample, path);

                    sampleName = loadSample.FileName;
                    ViewBag.Path = path;
                }
            }

            /*
             * save image
             */
            if (loadImage != null)
            {
                if (loadImage.Length > 0)
                {
                    //temporary path for file
                    var tempPathImage = Path.GetTempFileName();
                    using (var stream = new FileStream(tempPathImage, FileMode.Create))
                    {
                        await loadImage.CopyToAsync(stream);
                    }

                    //creats a new path
                    var path = Path.Combine(environment.WebRootPath, "uploadedImages", loadImage.FileName);

                    //moves the temporary file to the correct map
                    System.IO.File.Move(tempPathImage, path);

                    picName = loadImage.FileName;
                    ViewBag.Path = path;
                }
            }
            
            repository.InvestigatorUpdate(someID, StatusId, events, information, sampleName, picName);
            
            return RedirectToAction("CrimeInvestigator", new { id = someID });
        }
    }
}
