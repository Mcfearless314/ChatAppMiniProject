using Microsoft.AspNetCore.Identity;

namespace SecureChatApp;

public class DbSeeder
{
    private readonly AppDbContext _ctx;
    private readonly ILogger<DbSeeder> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public DbSeeder(ILogger<DbSeeder> logger, AppDbContext ctx, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _ctx = ctx;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        await _ctx.Database.EnsureDeletedAsync();
        await _ctx.Database.EnsureCreatedAsync();

        await CreateUser("Bob","Bob@bob.com", "S3cret1!");
        await CreateUser("Alice","A@alice.com", "S3cret2!");
        await _ctx.SaveChangesAsync();
    }

    private async Task CreateUser(string username, string email,  string password)
    {
        var user = new IdentityUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            foreach (var error in result.Errors)
                _logger.LogWarning("{Code}: {Description}", error.Code, error.Description);
    }
}