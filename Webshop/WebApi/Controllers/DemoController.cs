using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Client;

namespace WebApi.Controllers
{
    public class Body
    {
        public string Message { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private const string StoreName = "statestore";
        private const string StoreKey = "statestore";

        private readonly ILogger<DemoController> _logger;

        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IList<Tuple<string, DateTime>>> GetMessage([FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<IList<Tuple<string, DateTime>>>(StoreName, StoreKey);
            return state.Value;
        }

        [HttpPost]
        public async Task SetMessage([FromBody] Body message, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<IList<Tuple<string, DateTime>>>(StoreName, StoreKey);
            if (state.Value == null)
            {
                state.Value = new List<Tuple<string, DateTime>>{ Tuple.Create(message.Message, DateTime.UtcNow) };
            }
            else
            {
                state.Value.Add(Tuple.Create(message.Message, DateTime.UtcNow));
            }

            await state.SaveAsync();
        }
    }
}
