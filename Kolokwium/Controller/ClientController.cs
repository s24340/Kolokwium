using Microsoft.AspNetCore.Mvc;
using Kolokwium.Service;

namespace Kolokwium.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientController(IClientService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientWithSubscriptions(int id)
        {
            var client = await _service.GetClientWithSubscriptionsAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }
    }
}