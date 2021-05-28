using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Webshop.Models;
using System.Net.Http.Json;
using Dapr.Client;

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _key = "wert";
        private readonly string _storeName = "statestore";


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index([FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<string>(_storeName, _key);

            return View(new StateViewModel { Wert = state.Value });
        }

        [HttpPost]
        public async Task<IActionResult> Save(StateViewModel model, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<string>(_storeName, _key);

            state.Value = model.Wert;
            await state.SaveAsync();

            return RedirectToAction("Index", new { Wert = model.Wert});
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
