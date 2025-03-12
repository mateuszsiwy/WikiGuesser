using Microsoft.AspNetCore.Mvc;

namespace WikiGuesser.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticsController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "API is responding", timestamp = DateTime.UtcNow });
    }
    
    [HttpGet("error")]
    public IActionResult Error()
    {
        return BadRequest(new { error = "Test error response", timestamp = DateTime.UtcNow });
    }
}