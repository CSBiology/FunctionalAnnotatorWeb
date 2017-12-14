using Microsoft.AspNet.Identity;

namespace FaTool.Web.Membership.Identity
{
    public static class UserExtensions
    {

        public static bool IsUser(this IUser user, string userName)
        {
            return user.UserName == userName;
        }

        public static bool IsAdminUser(this IUser user)
        {
            return user.IsUser(FaToolUsers.ADMIN);
        }

    }
}
