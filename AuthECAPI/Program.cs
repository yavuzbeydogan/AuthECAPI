using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthECAPI.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5084");
// Add services to the container.
builder.Services.AddControllers(); ;

builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

builder.Services.AddAuthentication(x=>
{
    x.DefaultAuthenticateScheme=
    x.DefaultChallengeScheme =
    x.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(y =>
{
    y.SaveToken=false;
    y.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey= new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JWTSecret"]))
    };
});

var app = builder.Build();

app.ConfigureSwaggerExplorer().
    ConfigureCORS(builder.Configuration)
    .AddIdentityAuthMiddlewares();

app.MapControllers(); ;

app
    .MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapPost("/api/signup", async (
    UserManager<AppUser> userManager,
    [FromBody] UserRegistrationModel userRegistrationModel
    ) =>
{
AppUser user = new AppUser()
{
UserName = userRegistrationModel.Email,
Email = userRegistrationModel.Email,
FullName = userRegistrationModel.FullName,
};
var result = await userManager.CreateAsync(
    user,
    userRegistrationModel.Password);

if (result.Succeeded)
return Results.Ok(result);
else
return Results.BadRequest(result);
});

app.MapPost("/api/signin", async (
    UserManager < AppUser > userManager,
    [FromBody] LoginModel loginModel) => 
{ 
    var user = await userManager.FindByEmailAsync(loginModel.Email);
    if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
    {
        var signInKey= new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JWTSecret"]));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("UserID",user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(10),
            SigningCredentials = new SigningCredentials(
                signInKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);
        return Results.Ok(new { token});
    }
    else 
        return Results.BadRequest(new { message = "Username or password is incorrect." });
});

app.Run();

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}