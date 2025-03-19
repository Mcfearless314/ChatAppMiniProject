using System.ComponentModel.DataAnnotations;

namespace SecureChatApp.Entities;

public class Message
{
    [Key] public int Id { get; set; }
    
    public string Content { get; set; }
    public string SenderId { get; set; } = null!;
    public string RoomId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}