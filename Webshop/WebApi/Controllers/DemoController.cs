using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Client;

namespace WebApi.Controllers
{
    public class DemoDTO
    {
        public string Message { get; set; }
    }

    public class DemoMessage
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
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
        public async Task<IList<DemoMessage>> GetMessage([FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<IList<DemoMessage>>(StoreName, StoreKey);
            return state.Value;
        }

        [HttpPost]
        public async Task<ActionResult<DemoMessage>> SetMessage([FromBody] DemoDTO message, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<IList<DemoMessage>>(StoreName, StoreKey);
            var value = new DemoMessage { Message = message.Message, Timestamp = DateTime.UtcNow };
            if (state.Value == null)
            {
                state.Value = new List<DemoMessage> { value };
            }
            else
            {
                state.Value.Add(value);
            }

            await state.SaveAsync();
            return value;
        }
    }
}
