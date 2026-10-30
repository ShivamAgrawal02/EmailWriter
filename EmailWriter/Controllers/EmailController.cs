using EmailWriter.Models.Request;
using EmailWriter.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailWriter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;  
        public EmailController(IEmailService emailService,ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }
        [HttpPost("CreateEmail")]
        public async Task<IActionResult> WriteEmail([FromBody] EmailRequest request)
        {
            _logger.LogInformation($"Email Generation Request Received. Tone {request.Tone} with Context: {request.Context}");
            if (request == null || string.IsNullOrWhiteSpace(request.Context))
            {
                _logger.LogWarning("Invalid email request received.");
                return BadRequest();
            }
            var emailResponse = await _emailService.GenerateEmailAsync(request);
            _logger.LogInformation($"Email Generation Completed.Generated Email: { emailResponse.Email}");
            return Ok(emailResponse);
        }

    }
}
