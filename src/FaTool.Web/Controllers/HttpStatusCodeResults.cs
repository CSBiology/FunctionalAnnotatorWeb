using System;
using System.Net;
using System.Web.Mvc;
using System.Linq;

namespace FaTool.Web.Controllers
{
    public static class HttpStatusCodeResults
    {

        public static ActionResult HttpBadRequest(string statusDescription = "")
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, statusDescription);
        }

        public static ActionResult HttpBadRequest(ModelStateDictionary ModelState)
        {
            if (ModelState.Any() && ModelState.First().Value.Errors.Any())
                return HttpBadRequest(
                    string.Format("Error(s) in request model: {0}.", ModelState.First().Value.Errors.First().ErrorMessage));
            else
                return HttpBadRequest("Error in request model.");
        }
        
        public static ActionResult HttpConflict(string message = "")
        {
            return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
        }

        public static ActionResult HttpInternalServerError(Exception ex)
        {
            return HttpInternalServerError(ex.Message);
        }

        public static ActionResult HttpInternalServerError(string message = "")
        {
            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, message);
        }

        public static ActionResult HttpUnauthorized(string message = "")
        {
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, message);
        }
    }
}