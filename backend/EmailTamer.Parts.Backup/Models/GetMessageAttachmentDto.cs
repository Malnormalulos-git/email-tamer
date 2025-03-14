using EmailTamer.Core.Models;
using FluentValidation;

namespace EmailTamer.Parts.Sync.Models;

public class GetMessageAttachmentDto : IInbound
{
    public string MessageId { get; set; } = null!;
    
    public string FileName { get; set; } = null!;
    
    public class EmailBoxDtoValidator : AbstractValidator<GetMessageAttachmentDto>
    {
        public EmailBoxDtoValidator()
        {
            RuleFor(x => x.MessageId)
                .NotNull()
                .NotEmpty();
            
            RuleFor(x => x.FileName)
                .NotNull()
                .NotEmpty();
        }
    }
}