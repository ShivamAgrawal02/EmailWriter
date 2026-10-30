using EmailWriter.Options;
using EmailWriter.Services.Implementation;
using EmailWriter.Services.Interface;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(allowIntegerValues: false)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEmailService,EmailService>();

builder.Services.Configure<GeminiOptions>(
    builder.Configuration.GetSection("Gemini"));
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<GeminiOptions>>().Value;
    return new OpenAIClient(
           new ApiKeyCredential(settings.ApiKey),
           new OpenAIClientOptions { Endpoint = new Uri(settings.Endpoint) })
       .GetChatClient(settings.ModelId)
       .AsIChatClient();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
