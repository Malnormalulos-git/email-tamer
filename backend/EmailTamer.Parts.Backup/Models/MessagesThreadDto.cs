using EmailTamer.Core.Models;

namespace EmailTamer.Parts.Sync.Models;

public class MessagesThreadDto : IOutbound
{
    public string ThreadId { get; set; }

    public List<MessageDto> Messages { get; set; } = [];

    public string? Subject { get; set; }

    public MessageDetailsDto LastMessage { get; set; }
}