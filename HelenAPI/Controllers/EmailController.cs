using Helen.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Helen.Domain.Invites.Response;
using Helen.Domain.GenericResponse;
using System.Threading.Tasks;
using Helen.Domain.Google.Response; // Ensure this is the correct namespace
using static EmailService;

[ApiController]
[Route("[controller]")]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> _logger;
    private readonly IInviteService _inviteService;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public EmailController(
        ILogger<EmailController> logger,
        IInviteService inviteService,
        IMemoryCache cache,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _logger = logger;
        _inviteService = inviteService;
        _cache = cache;
        _configuration = configuration;
        _emailService = emailService;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
    {
        if (emailRequest == null)
        {
            return BadRequest("Invalid email request.");
        }

        var response = await _emailService.SendEmailAsync(emailRequest);

        return Ok(response);
    }
}
