using AppWithTokenAuthen.App_Start;
using AppWithTokenAuthen.Constants;
using AppWithTokenAuthen.Providers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

[assembly: OwinStartup(typeof(AppWithTokenAuthen.Startup))]

namespace AppWithTokenAuthen
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            RegisterTokenIssuer(app);
            RegisterTokenAudience(app);

            var config = CreateHttpConfiguration();
            AppSeed.RegisterStaticFile(app);
        }

        private static HttpConfiguration CreateHttpConfiguration()
        {
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);

            return httpConfiguration;
        }

        public static void RegisterTokenIssuer(IAppBuilder app)
        {
            // setup for generating token (jwt format) to use for access content

            string accessTokenValidMinute = WebConfigurationManager.AppSettings["Access_Token_Minute"] ?? "0";
            string tokenIssuer = WebConfigurationManager.AppSettings["Access_Token_Minute"];

            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = false,
                TokenEndpointPath = new PathString("/auth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(accessTokenValidMinute)),
                Provider = new AuthProvider(),
                AccessTokenFormat = new CustomAccessTokenFormat(tokenIssuer),
                RefreshTokenProvider = new RefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(options);
        }

        public static void RegisterTokenAudience(IAppBuilder app)
        {
            // setup for receiving and validating token (jwt format)

            string tokenIssuer = WebConfigurationManager.AppSettings["Token_Issuer"];
            string secretString = WebConfigurationManager.AppSettings["JWT_Secret_Key"];

            JwtBearerAuthenticationOptions jwtOption = new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = !string.IsNullOrWhiteSpace(tokenIssuer),
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenIssuer,
                    //ValidAudience = "http://mysite.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretString)),
                }
            };

            app.UseJwtBearerAuthentication(jwtOption);
        }
    }
}
