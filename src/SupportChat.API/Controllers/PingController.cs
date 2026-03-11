using Microsoft.AspNetCore.Mvc;

namespace SupportChat.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {

        [HttpGet(Name = "Ping")]
        public string Get()
        {
            return "Pong";
        }
    }
}
