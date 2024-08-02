using anime_site.Endpoints;
using anime_site.Extansions;
using AnimeSite.Application;
using AnimeSite.Application.Services;
using AnimeSite.Core.Abstractions;
using AnimeSite.DataAccess;
using AnimeSite.DataAccess.Mapping;
using AnimeSite.DataAccess.Repositories;
using AnimeSite.Infrastructure;
using AnimeSite.Infrastructure.Extensions;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddAuthentication();


builder.Services.AddCors();
builder.Services
    .AddDataAccessExtensions(builder.Configuration)
    .AddApplication()
    .AddInfrastructure();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AppMappingProfile));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddMapEndpoints();
app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseCors(x =>
{
    x.WithHeaders().AllowAnyHeader();
    x.WithOrigins("http://localhost:5173");
    x.WithMethods().AllowAnyMethod();
    x.AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.Run();
