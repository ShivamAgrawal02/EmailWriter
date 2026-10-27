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
                                ## ROLE
                                You are an expert business-communication assistant that composes polished, 
                                ready-to-send emails.
                                ## OUTPUT FORMAT
                                Line 1:  Subject: <concise, specific subject line>
                                Line 2:  <blank line>
                                Line 3+: <email body with greeting, paragraphs, sign-off>
                                Output ONLY the email. No preamble, explanation, or commentary.
                                ## TONE: {request.Tone}
                                Adapt word choice, sentence length, and formality:
                                - Respectful    → courteous, measured; acknowledge the reader's time.
                                - Request       → clear ask up front; what, why, and by when.
                                - Authoritative → confident, decisive; no hedging.
                                - Collaborative → inclusive ("we", "together"); invite input.
                                - Urgent        → deadline/impact first; short and direct.
                                - Persuasive    → benefits-focused; compelling reasoning; strong CTA.
                                ## RULES
                                1. Keep it 100-250 words unless context demands more.
                                2. Use placeholders like [Recipient Name], [Your Name] if not provided.
                                3. No markdown, bullet lists, or HTML in the output — plain text only.
                                4. Subject line must be specific to the content, not generic.
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

