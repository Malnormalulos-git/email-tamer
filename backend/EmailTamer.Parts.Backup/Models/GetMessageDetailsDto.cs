using EmailTamer.Core.Models;
using FluentValidation;

namespace EmailTamer.Parts.Sync.Models;

public class GetMessageDetailsDto : IInbound
{
    public string MessageId { get; set; } = null!;
    
    public class EmailBoxDtoValidator : AbstractValidator<GetMessageDetailsDto>
    {
        public EmailBoxDtoValidator()
        {
            RuleFor(x => x.MessageId)
                .NotNull()
                .NotEmpty();
        }
    }
}