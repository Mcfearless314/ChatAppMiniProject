using Microsoft.AspNetCore.Identity;

namespace SecureChatApp.Entities;

public class User : IdentityUser
{
    public string PublicKey { get; set; } = null!;
}