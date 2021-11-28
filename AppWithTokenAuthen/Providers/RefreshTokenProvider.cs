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
        //4.login/5.RefreshToken After Login Or Refresh Token Generate Next Token and Store
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            //refresh token generation logic inside method “CreateAsync”,
            var userName = context.Ticket.Properties.Dictionary["as:userName"];
            if (string.IsNullOrEmpty(userName))
                return;

            var guid = context.Ticket.Properties.Dictionary["as:guid"];
            if (string.IsNullOrEmpty(guid))
                return;

            //We are generating a unique identifier for the refresh token
            var refreshTokenId = Guid.NewGuid().ToString("n");


            //Then we are reading the refresh token life time value from the Owin context where we set this value once we validate the client,
            //this value will be used to determine how long the refresh token will be valid for,
            //this should be in minutes.
            var refreshTokenMinute = WebConfigurationManager.AppSettings["Refresh_Token_Minute"];

            var IssuedUtc = DateTime.UtcNow;
            var ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenMinute));

            context.Ticket.Properties.IssuedUtc = IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = ExpiresUtc;

            //serialize the ticket content and we’ll be able to store this magical serialized string on the database
            var NewToken = context.SerializeTicket();
            //ID => Subject (User) and the Client unique key
            //if it not unique I’ll delete the existing one and store new refresh token.
            //It is better to hash the refresh token identifier before storing it,
            //so if anyone has access to the database he’ll not see the real refresh tokens
            //var autofac = context.OwinContext.GetAutofacLifetimeScope();
            //var refreshTokenRepo = autofac.Resolve<IRefreshTokenRepo>();
            //var result = refreshTokenRepo.Create(token);
            //if (result != null)
            //{
            //    //Lastly we will send back the refresh token id (without hashing it) in the response body.
            //    context.SetToken(refreshTokenId);
            //}
            //await Task.FromResult(true);

            context.SetToken(NewToken);
        }

        //2.RefreshToken  Active Refresh Token in Token Store
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);



            //string hashedTokenId = GetHash(context.Token);

            //var autofac = context.OwinContext.GetAutofacLifetimeScope();
            //var refreshTokenRepo = autofac.Resolve<IRefreshTokenRepo>();
            //var refreshToken = refreshTokenRepo.Filter(x => x.NewTokenKey == hashedTokenId).FirstOrDefault();

            //if (refreshToken != null && refreshToken.ExpiresUtc > DateTime.UtcNow)
            //{
            //    //Get protectedTicket from refreshToken class
            //    context.DeserializeTicket(refreshToken.NewToken);
            //    var result = refreshTokenRepo.Delete(x => x.NewTokenKey == hashedTokenId);
            //    return;
            //}
            //await Task.FromResult(true);
        }


        public static string GetHash(string input)
        {

            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
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