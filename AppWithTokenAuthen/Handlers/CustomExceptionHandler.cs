using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace AppWithTokenAuthen.Handlers
{
    public class CustomExceptionHandler : ExceptionHandler
    {
        private const string DefaultExceptionMessage = "An unhandled error occured.";

        public override void Handle(ExceptionHandlerContext context)
        {
            if ((context.Exception as UnauthorizedAccessException) != null)
            {
                context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.Unauthorized, context.Exception.Message));
            }
            else
            {
                context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.InternalServerError, DefaultExceptionMessage));
            }
        }
    }
}