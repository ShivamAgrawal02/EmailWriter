using System.ComponentModel.DataAnnotations;

namespace EmailWriter.Models.Request
{
    public sealed record EmailRequest
    {
        [Required, StringLength(2000, MinimumLength = 10)]
        public required string Context { get; init; }

        
        public string Tone { get; init; } = string.Empty;
    }
}
