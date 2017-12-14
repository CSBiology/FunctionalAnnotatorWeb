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
