#region license
// The MIT License (MIT)

// FaToolAuthenticationModule.cs

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
using System.Threading.Tasks;
using System.Web;
using FaTool.Web.Membership.Identity;

namespace FaTool.Web.Membership.Auth
{
    public sealed class FaToolAuthenticationModule : IHttpModule
    {

        private readonly FaToolAuthenticationManager authenticationManager;
        private readonly FaToolUserStore userStore;
        private readonly FaToolUserManager userManager;

        public FaToolAuthenticationModule()
        {
            authenticationManager = new FaToolAuthenticationManager();
            userStore = new FaToolUserStore();
            userManager = new FaToolUserManager(userStore);
        }

        #region IHttpModule Members

        public void Dispose()
        {
            if (userManager != null)
                userManager.Dispose();
            if (userStore != null)
                userStore.Dispose();
        }

        public void Init(HttpApplication app)
        {
            var auth = new EventHandlerTaskAsyncHelper(OnAuthenticateRequestAsync);
            app.AddOnPostAuthenticateRequestAsync(auth.BeginEventHandler, auth.EndEventHandler);
            //app.EndRequest += OnEndRequest;
        }

        #endregion

        private async Task OnAuthenticateRequestAsync(Object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            await authenticationManager.AuthenticateRequestAsync(app.Context, userManager);
        }

        //private void OnEndRequest(object sender, EventArgs e)
        //{
        //    var app = (HttpApplication)sender;
        //    var response = app.Context.Response;
        //    var request = app.Context.Request;

        //    if (response.StatusCode == 401 && request.HttpMethod == "GET")
        //    {
        //        var loginPath = FaToolAuthenticationManager.LoginPath;

        //        if (!string.IsNullOrWhiteSpace(FaToolAuthenticationManager.LoginPath))
        //            RedirectToLogin(response, loginPath, request);
        //    }
        //}

        //private void RedirectToLogin(HttpResponse response, string loginPath, HttpRequest request)
        //{
        //    if (response.IsClientConnected)
        //    {
        //        var redirectQuery = string.Format("?returnUrl={0}", request.Url.PathAndQuery);
        //        var ub = new UriBuilder(request.Url.Scheme, request.Url.Host, request.Url.Port, loginPath, redirectQuery);                
        //        response.Redirect(ub.ToString(), false);
        //    }
        //    else
        //    {
        //        response.End();
        //    }
        //}                
    }
}
