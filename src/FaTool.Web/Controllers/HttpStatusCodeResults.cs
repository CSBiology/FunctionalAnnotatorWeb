#region license
// The MIT License (MIT)

// HttpStatusCodeResults.cs

// Copyright (c) 2016 Alexander Lüdemann
// alexander.luedemann@outlook.com
// luedeman@rhrk.uni-kl.de

// Computational Systems Biology, Technical University of Kaiserslautern, Germany
 

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

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