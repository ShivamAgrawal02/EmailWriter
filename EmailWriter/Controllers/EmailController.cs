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
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpPost("CreateEmail")]
        public async Task<IActionResult> WriteEmail([FromBody] EmailRequest request)
        {
            if(request == null || string.IsNullOrWhiteSpace(request.Context))
            {
                return BadRequest();
            }
            var emailResponse = await _emailService.GenerateEmailAsync(request);
            return Ok(emailResponse);
        }

    }
}
