using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sympli.Models;
using Sympli.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sympli.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISearchResultsService _searchResultsService;

        public HomeController(ILogger<HomeController> logger, ISearchResultsService searchResultsService)
        {
            _logger = logger;
            _searchResultsService = searchResultsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ResponseCache(Duration =3600)]
        public async Task<IActionResult> Search(InputRequestModel request)
        {
            if (ModelState.IsValid)
            {
                var result = await _searchResultsService.GetScrapeResultsAsync(request);
                ViewBag.SearchResults = result;

                return View();
            }

            return RedirectToAction("Error");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
