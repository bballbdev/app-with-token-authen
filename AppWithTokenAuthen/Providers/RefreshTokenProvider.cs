using AppWithTokenAuthen.Database;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace AppWithTokenAuthen.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly MyAppEntities _entities;

        public RefreshTokenProvider()
        {
            if (_entities == null)
                _entities = new MyAppEntities();
        }



        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var userName = context.Ticket.Properties.Dictionary["as:userName"];
            if (string.IsNullOrEmpty(userName))
                return Task.FromResult<object>(null);

            var access_token_guid = context.Ticket.Properties.Dictionary["as:access_token_guid"];
            if (string.IsNullOrEmpty(access_token_guid))
                return Task.FromResult<object>(null);



            var refreshTokenId = Guid.NewGuid().ToString("n");
            var refreshToken = context.SerializeTicket();

            var refreshTokenMinute = WebConfigurationManager.AppSettings["Refresh_Token_Minute"];

            var IssuedUtc = DateTime.UtcNow;
            var ExpiredUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenMinute));

            // create refresh token into db
            _entities.Refresh_Token.Add(new Refresh_Token()
            {
                Refresh_Token_Id = refreshTokenId,
                Refresh_Token1 = refreshToken,
                Username = userName,
                Issued_At_Utc = IssuedUtc,
                Expired_At_Utc = ExpiredUtc
            });
            _entities.SaveChanges();


            context.Ticket.Properties.IssuedUtc = IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = ExpiredUtc;

            context.SetToken(refreshTokenId);
            return Task.FromResult<object>(null);
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            string refreshTokenId = context.Token;
            string refreshToken = _entities.Refresh_Token
                                            .Where(w => w.Refresh_Token_Id == refreshTokenId && w.Expired_At_Utc > DateTime.UtcNow)
                                            .Select(s => s.Refresh_Token1)
                                            .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                context.DeserializeTicket(refreshToken);
                // remove used refresh token
                _entities.Refresh_Token.Remove(_entities.Refresh_Token.Where(w => w.Refresh_Token1 == refreshToken).FirstOrDefault());
                _entities.SaveChanges();
            }


            return Task.FromResult<object>(null);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}