namespace SecureChatApp.Entities;

public class SessionData
{
    public string Email { get; set; }
    public string UserId { get; set; }
    public SessionData(string email, string userId)
    {
        Email = email;
        UserId = userId;
    }
}