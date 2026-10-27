using EmailWriter.Models.Request;
using EmailWriter.Models.Response;

namespace EmailWriter.Services.Interface
{
    public interface IEmailService
    {
        Task<EmailResponse> GenerateEmailAsync(EmailRequest request);
    }
}
