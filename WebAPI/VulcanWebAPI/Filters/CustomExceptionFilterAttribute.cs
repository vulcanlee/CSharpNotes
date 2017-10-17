using DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace VulcanWebAPI.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            APIResult foo = new APIResult();
            // Unhandled errors
            var msg = context.Exception.GetBaseException().Message;
            string stack = context.Exception.StackTrace;

            foo.Success = false;
            foo.Message = msg;
            foo.Payload = stack;

            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(foo);

            base.OnException(context);
        }

    }
}
