using EmailWriter.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmailWriter.Models.Request
{
    public sealed record EmailRequest
    {
        [Required, StringLength(2000, MinimumLength = 10)]
        public required string Context { get; init; }

        [EnumDataType(typeof(Tone))]
        public Tone Tone { get; init; } = Tone.Respectful;
    }
}
