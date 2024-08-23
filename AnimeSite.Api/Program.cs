using anime_site.Endpoints;
using AnimeSite.Application.Services;
using AnimeSite.Application.Validations;
using AnimeSite.Core.Interfaces;
using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
using AnimeSite.Infrastructure.Authentication;
using MailKit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using System.Text;
using static System.Net.WebRequestMethods;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false,
        ValidIssuer = "",
        ValidAudience = "",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt"]!))
    };
}).AddCookie();

builder.Services.AddAuthorization();
builder.Services.AddCors();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.SignIn.RequireConfirmedEmail = false;
    opt.SignIn.RequireConfirmedPhoneNumber = false;
    opt.SignIn.RequireConfirmedAccount = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 1;
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddMailKit(optionBuilder =>
{
    optionBuilder.UseMailKit(new MailKitOptions()
    {
        //get options from sercets.json
        Server = "smtp.gmail.com",
        Port = 587,
        SenderName = "animeq",
        SenderEmail = "",

        // can be optional with no authentication 
        Account = "",
        Password = "",
        // enable ssl or tls
        Security = true
    });
});
builder.Services.AddCors();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AnimeSiteDbContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
app.LoginEndpoints();
app.RegisterUserEndpoints();
app.UseHttpsRedirection();


app.Run();