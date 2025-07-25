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
using AuthECAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5084");
// Add services to the container.
builder.Services.AddControllers(); ;
builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppConfig(builder.Configuration)   
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);


var app = builder.Build();

app.ConfigureSwaggerExplorer().
    ConfigureCORS(builder.Configuration)
    .AddIdentityAuthMiddlewares();

app.MapControllers(); ;
app.MapGroup("/api")
    .MapIdentityApi<AppUser>();
app.MapGroup("/api")
    .MapIdentitiyUserEndpoints();

app.Run();

