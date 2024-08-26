using AnimeSite.Application.Services;
using AnimeSite.Core.Interfaces;
using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
using AnimeSite.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace anime_site
{
    public static class ApiExtensions
    {
        public static void AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();


            services.AddMailKit(optionBuilder =>
            {
                optionBuilder.UseMailKit(new MailKitOptions()
                {
                    //get options from sercets.json
                    Server = "smtp.gmail.com",
                    Port = 587,
                    SenderName = "animeq",
                    SenderEmail = "shok3904@gmail.com",

                    // can be optional with no authentication 
                    Account = "shok3904@gmail.com",
                    Password = "oixnkofbasypzyji",
                    // enable ssl or tls
                    Security = true
                });
            });
        }
    }
}
