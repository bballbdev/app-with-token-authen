using AppWithTokenAuthen.Attributes;
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

        [TokenAuthorize(Roles = "admin,user,president"), HttpPost]
        public IHttpActionResult UserInfo()
        {
            var identity = (ClaimsIdentity)User.Identity;

            var claims = identity.Claims.ToList();
            
            
            return Ok("Hello Secret Content");
        }
    }
}
