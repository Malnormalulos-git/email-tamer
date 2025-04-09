using EmailTamer.Core.Models;

namespace EmailTamer.Parts.Sync.Models;

public class MessagesThreadShortDto : IOutbound
{
    public string ThreadId { get; set; }
    public MessageDto LastMessage { get; set; } 
    public string? Subject { get; set; } 
    public DateTime StartDate { get; set; } 
    public DateTime EndDate { get; set; } 
    public int Length { get; set; } 
}