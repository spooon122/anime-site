using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

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
