#region license
// The MIT License (MIT)

// IdentityExtensions.cs

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

using System.Security.Principal;

namespace FaTool.Web.Membership.Identity
{
    public static class IdentityExtensions
    {

        public static bool IsAuthenticated(this IPrincipal principal)
        {
            if (principal == null)
                return false;
            else
                return principal.Identity.IsAuthenticated;
        }

        public static bool IsUser(this IPrincipal principal, string userName)
        {
            if (principal.IsAuthenticated() == false)
                return false;
            else
                return principal.Identity.Name == userName;
        }

        public static bool IsAdminUser(this IPrincipal principal)
        {
            return principal.IsUser(FaToolUsers.ADMIN);
        }

        public static bool IsRole(this IPrincipal principal, string role)
        {
            if (principal.IsAuthenticated() == false)
                return false;
            else
                return principal.IsInRole(role);
        }

        public static bool IsRoles(this IPrincipal principal, params string[] roles)
        {
            foreach (var role in roles)
            {
                if (principal.IsRole(role))
                    return true;
            }

            return false;
        }

        public static bool IsAdminRole(this IPrincipal principal)
        {
            return principal.IsRole(FaToolRoles.ADMIN);
        }

        public static bool IsAdminUserOrAdminRole(this IPrincipal principal)
        {
            return principal.IsUserOrInRole(FaToolUsers.ADMIN, FaToolRoles.ADMIN);
        }

        public static bool IsAnnotatorRole(this IPrincipal principal)
        {
            return principal.IsRole(FaToolRoles.ANNOTATOR);
        }

        public static bool IsUserOrInRole(this IPrincipal principal, string userName, string role)
        {
            return principal.IsUser(userName) || principal.IsRole(role);
        }
    }
}
