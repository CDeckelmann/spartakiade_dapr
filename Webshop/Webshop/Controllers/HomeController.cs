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

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _daprHttpPortEnv;
        private readonly string _stateStoreUri;
        private readonly string _key = "wert";

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            this._httpClient = httpClient;
            _daprHttpPortEnv = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
            _stateStoreUri = $"http://localhost:{_daprHttpPortEnv}/v1.0/state/statestore";
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_stateStoreUri}/{_key}");
            var wert = await response.Content.ReadAsStringAsync();

            return View(new StateViewModel { Wert = wert });
        }

        [HttpPost]
        public async Task<IActionResult> Save(StateViewModel model)
        {

            var states = new List<KeyValuePair<string, object>>
            {
              new KeyValuePair<string, object>(_key, model.Wert)
            };

            var response = await _httpClient.PostAsync(
              _stateStoreUri,
              JsonContent.Create(states)
            );

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
