using Microsoft.AspNetCore.Mvc;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;
using System.Security.Cryptography;
using System.Text;

namespace AnastasiiaPortfolio.Controllers;

[ApiController]
[Route("api/valentine")]
public class ValentineController : ControllerBase
{
    private readonly MongoDBService _mongoDBService;
    private readonly ILogger<ValentineController> _logger;

    public ValentineController(MongoDBService mongoDBService, ILogger<ValentineController> logger)
    {
        _mongoDBService = mongoDBService;
        _logger = logger;
    }

    [HttpPost("yes")]
    public async Task<IActionResult> LogYes()
    {
        try
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var ipHash = string.IsNullOrEmpty(ip) ? null : Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(ip)))[..16];

            var response = new ValentineResponse
            {
                UserAgent = userAgent.Length > 200 ? userAgent[..200] : userAgent,
                IpHash = ipHash
            };

            await _mongoDBService.CreateValentineResponseAsync(response);
            _logger.LogInformation("Valentine Yes logged at {Time}", DateTime.UtcNow);

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log Valentine response");
            return Ok(new { success = false }); // Still return OK so frontend doesn't show error
        }
    }
}
