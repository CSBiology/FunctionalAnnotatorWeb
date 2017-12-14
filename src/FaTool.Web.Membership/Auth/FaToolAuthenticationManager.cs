#region license
// The MIT License (MIT)

// FaToolAuthenticationManager.cs

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
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using FaTool.Web.Membership.ConfigFile;
using FaTool.Web.Membership.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FaTool.Web.Membership.Auth
{
    public sealed class FaToolAuthenticationManager
    {
        
        public static string LoginPath
        {
            get
            {
                var config = WebConfigurationManager.GetSection("faToolMembership") 
                    as FaToolMembershipSection;

                if (config != null)
                    return config.LoginPath;
                else
                    return null;
            }
        }

        public async Task<AuthenticationResult> SignInAsync(
            HttpContext context,
            FaToolUserManager userManager,
            string userName,
            string password,
            bool isPersistent)
        {

            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return AuthenticationResult.UNKNOWN_USER;
            }

            if (await userManager.IsLockedOutAsync(user.Id))
            {
                return AuthenticationResult.USER_LOCKED_OUT;
            }

            if (await userManager.CheckPasswordAsync(user, password))
            {
                await SignInAsync(context, userManager, user, isPersistent);
                return AuthenticationResult.SUCCESS;
            }
            else
            {
                return AuthenticationResult.INVALID_PASSWORD;
            }

        }
        
        public Task SignOutAsync()
        {
            FormsAuthentication.SignOut();
            return Task.FromResult(true);
        }

        public async Task AuthenticateRequestAsync(
            HttpContext context,
            FaToolUserManager userManager)
        {
            var authCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                
                if (authTicket != null && !authTicket.Expired)
                {                    
                    var user = await userManager.FindByNameAsync(authTicket.Name);

                    if (user != null)
                    {
                        var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
                else if (authTicket.Expired)
                {
                    throw new HttpException((int)HttpStatusCode.Unauthorized, "Login has been expired.");
                }
            }
        }

        private async Task SignInAsync(
            HttpContext context,
            FaToolUserManager userManager,
            FaToolUser user,
            bool isPersistent)
        {
            var roles = await userManager.GetRolesAsync(user.Id);
            var userData = string.Join(";", roles);

            var ticket = new FormsAuthenticationTicket(
                1,
                user.UserName,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                isPersistent,
                userData,
                FormsAuthentication.FormsCookiePath);

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            context.Response.Cookies.Add(cookie);

        }

        private async Task<string> GenerateJwtTokenAsync(
            FaToolUserManager userManager, 
            FaToolUser user)
        {
            var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            var claims = identity.Claims;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SOME_RANDOM_KEY_DO_NOT_SHARE"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(30);
            var issuer = "http://fatool.com";
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return tokenHandler.WriteToken(token);
        }

    }

    public enum AuthenticationResult
    {
        UNKNOWN_USER, INVALID_PASSWORD, USER_LOCKED_OUT, SUCCESS
    }
}