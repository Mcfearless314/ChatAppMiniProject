namespace SecureChatApp.Entities;

public class SessionData
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
    public SessionData(string userName, string email, string userId)
    {
        UserName = userName;
        Email = email;
        UserId = userId;
    }
}