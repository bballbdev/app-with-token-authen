using AppWithTokenAuthen.Attributes;
using AppWithTokenAuthen.Constants;
using AppWithTokenAuthen.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace AppWithTokenAuthen.Controllers
{
    public class UserController : ApiController
    {
        private MyAppEntities _entities { get; set; }

        public UserController()
        {
            _entities = new MyAppEntities();
        }



        [AllowAnonymous, HttpGet]
        public IHttpActionResult Ping()
        {
            return Ok(new
            {
                Pong = "Server is running.",
                DateTime = DateTime.Now
            });
        }

        [AllowAnonymous, HttpPost]
        public IHttpActionResult Register()
        {
            return Ok();
        }

        [TokenAuthorize(), HttpPost]
        public IHttpActionResult UserInfo()
        {
            var identity = (ClaimsIdentity)User.Identity;
            
            return Ok($"Hello {identity.Name}");
        }

        [TokenAuthorize(Roles = Roles.admin), HttpPost]
        public IHttpActionResult AdminInfo()
        {
            var identity = (ClaimsIdentity)User.Identity;

            return Ok($"Hello Admin {identity.Name}");
        }
    }
}
