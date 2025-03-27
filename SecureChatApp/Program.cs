using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureChatApp;
using SecureChatApp.Controllers;
using SecureChatApp.Entities;
using SecureChatApp.Logging;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSignalR(options =>
{
    options.AddFilter<SignalRLoggingFilter>();  // Add filter here
});
builder.Services.AddSingleton<SignalRLoggingFilter>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AppDb");
    options.UseSqlite(connectionString);
});
builder
    .Services.AddIdentityApiEndpoints<IdentityUser>(options => { options.SignIn.RequireConfirmedAccount = false; })
    .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


// Configure JWT Authentication
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<JwtTokenService>(sp =>
{
    var jwtSettings = sp.GetRequiredService<IOptions<JwtSettings>>().Value;
    return new JwtTokenService(jwtSettings.SecretKey, jwtSettings.Issuer, jwtSettings.Audience);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

var app = builder.Build();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.UseStaticFiles();
// Seed
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DbSeeder>().SeedAsync().Wait();
}

app.UseWebSockets();

app.UseHttpsRedirection();
app.UseRouting();
app.Run();