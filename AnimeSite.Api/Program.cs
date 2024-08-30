using anime_site;
using anime_site.Endpoints;
using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});
builder.Services.AddAuthorization();

builder.Services.AddCors();
builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.SignIn.RequireConfirmedEmail = false;
    opt.SignIn.RequireConfirmedPhoneNumber = false;
    opt.SignIn.RequireConfirmedAccount = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 1;
})
                    .AddEntityFrameworkStores<UserDbContext>()
                    .AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
   opt.TokenLifespan = TimeSpan.FromMinutes(15));

builder.Services.AddCors();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AnimeSiteDbContext")));
builder.Services.AddDbContext<AnimeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AnimeDb")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(builder => builder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
app.LoginEndpoints();
app.RegisterUserEndpoints();
app.RolesEndpoints();
app.UseHttpsRedirection();

app.Run();