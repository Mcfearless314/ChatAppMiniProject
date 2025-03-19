using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SecureChatApp.Entities;

public class ChatRoom
{
    [Key] public int Id { get; set; }
    
    public string Name { get; set; }
    public string Password { get; set; }
    public List<IdentityUser> Users { get; set; } = new List<IdentityUser>();
    
}
