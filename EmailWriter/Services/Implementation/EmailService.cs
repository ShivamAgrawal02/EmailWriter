using EmailWriter.Models.Request;
using EmailWriter.Models.Response;
using EmailWriter.Services.Interface;
using Microsoft.Extensions.AI;
using OpenAI;
using System.Net;

namespace EmailWriter.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IChatClient _chatClient;
        private readonly string ApiKey;
        private readonly string Endpoint;
        private readonly string ModelId;
        private readonly IConfiguration _configuration;


        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            ApiKey = _configuration["Gemini:ApiKey"];
            Endpoint = _configuration["Gemini:Endpoint"];
            ModelId = _configuration["Gemini:ModelId"];
            _chatClient = new OpenAIClient(
                           new System.ClientModel.ApiKeyCredential(ApiKey),//wrapping key - so that sdk can use it 
                           new OpenAIClientOptions
                           {
                               Endpoint = new Uri(Endpoint)
                           })// providing endpoint - in our case its gemini,it could be open router or some else provider
                       .GetChatClient(ModelId)    // defines which model to use for chat 
                       .AsIChatClient();
        }
        public async Task<EmailResponse> GenerateEmailAsync(EmailRequest request)
        {
            var systemPrompt = $"""
                You are a professional email writer.
                Write a complete email in a {request.Tone} tone.
                Include a proper subject line prefixed with "Subject:" on the first line,
                followed by a blank line and then the email body.
                Do not include any explanation or commentary — only the email.
                """;
            var chatMessages = new List<ChatMessage>
            {
                new(ChatRole.System, systemPrompt),
                new(ChatRole.User, request.Context)
            };
            var response = await _chatClient.GetResponseAsync(chatMessages);
            return new EmailResponse(response.Text);
        }
    }
}

