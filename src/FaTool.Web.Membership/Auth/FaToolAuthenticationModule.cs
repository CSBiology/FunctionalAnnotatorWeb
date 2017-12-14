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
