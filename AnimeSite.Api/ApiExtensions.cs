using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
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


            services.AddMailKit(optionBuilder =>
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
        }
    }
}
