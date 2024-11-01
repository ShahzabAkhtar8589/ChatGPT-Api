using ChatGPT_Consumer_Api.Services.Consumer;
using Microsoft.AspNetCore.Mvc;

namespace ChatGPT_Consumer_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumerController : ControllerBase
    {
        private readonly IConsumer _consumerService;
        public ConsumerController(IConsumer consumer)
        {
            _consumerService = consumer;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var result = _consumerService.RunExectible();
            return Ok();
        }
    }
}
