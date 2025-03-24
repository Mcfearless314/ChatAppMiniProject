using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureChatApp.Entities;

namespace SecureChatApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtTokenService _tokenService;
    
    public LoginController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, JwtTokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestData loginRequest)
    {
        // Find the user by email
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null) return Unauthorized("Invalid email or password");

        // Validate password
        var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, false);
        if (!result.Succeeded) return Unauthorized("Invalid email or password");

         // Generate token
         var sessionData = new SessionData(user.UserName, user.Email, user.Id);
         var token = _tokenService.GenerateJwtToken(sessionData);

        return Ok(token);
    }

    public class LoginRequestData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
} 
