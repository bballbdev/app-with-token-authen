using AppWithTokenAuthen.Constants;
using AppWithTokenAuthen.Database;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace AppWithTokenAuthen.Providers
{
    public class AuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly MyAppEntities _entities;

        public AuthProvider()
        {
            if (_entities == null)
                _entities = new MyAppEntities();
        }



        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (!context.TryGetBasicCredentials(out var clientId, out var clientSecret))
                context.TryGetFormCredentials(out clientId, out clientSecret);

            if (string.IsNullOrWhiteSpace(clientId))
            {
                context.SetError("invalid_client", "Client Id should be sent.");
                return;
            }

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                context.SetError("invalid_client", "Client Secret should be sent.");
                return;
            }

            if (!_entities.Token_Audience.Any(w => w.Client_Id == clientId && w.Client_Secret == clientSecret))
            {
                context.SetError("invalid_client", "Client Id with Client Secret is not match.");
                return;
            }

            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            if (!_entities.User.Any(a => a.Username == context.UserName && a.Password == context.Password))
            {
                context.SetError("invalid_grant", "The username or password is incorrect.");
                return;
            }


            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            string guid = Guid.NewGuid().ToString("n");

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType, TokenClaim.userName, TokenClaim.role);
            claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, guid));
            claimsIdentity.AddClaim(new Claim(TokenClaim.userName, context.UserName));
            claimsIdentity.AddClaim(new Claim(TokenClaim.role, "user"));


            var dictionaryItem = new Dictionary<string, string>();
            dictionaryItem.Add("as:userName", context.UserName);
            dictionaryItem.Add("as:guid", guid);
            dictionaryItem.Add("as:audience", context.ClientId);

            var props = new AuthenticationProperties(dictionaryItem);
            var ticket = new AuthenticationTicket(claimsIdentity, props);

            context.Validated(ticket);
        }
    }



    public class CustomAccessTokenFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private const string AudiencePropertyKey = "as:audience";

        private readonly string _issuer = string.Empty;

        public CustomAccessTokenFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            // can config secretString in database and find secretString by use "AudiencePropertyKey" as client_id

            string secretString = WebConfigurationManager.AppSettings["JWT_Secret_Key"];
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretString));
            var signingKey = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityToken(_issuer, null, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}