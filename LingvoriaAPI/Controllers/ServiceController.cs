using Microsoft.AspNetCore.Mvc;

namespace LingvoriaAPI.Controllers;

[ApiController]
public class ServiceController : ControllerBase
{
    [HttpGet]
    [Route("/status")]
    public IActionResult GetServiceStatus()
    {
        return Ok();
    }
}