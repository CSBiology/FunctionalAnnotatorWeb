using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace FaTool.Web.Membership.Identity
{
    public static class UserManagerExtensions
    {

        public static async Task<IdentityResult> CreateAccountAsync(this FaToolUserManager um, FaToolUser user, string password, params string[] roles)
        {
            var result = await um.CreateAsync(user, password);

            if (result.Succeeded && roles != null && roles.Length > 0)
            {
                result = await um.AddToRolesAsync(user.Id, roles);
            }

            return result;
        }

        public static async Task<IdentityResult> DeleteAccountAsync(this FaToolUserManager um, FaToolUser user)
        {
            if (user.IsAdminUser())
            {
                return IdentityResult.Failed("Can't delete admin user account.");
            }

            var currentRoles = await um.GetRolesAsync(user.Id);
            var result = await um.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());

            if (result.Succeeded)
            {
                result = await um.DeleteAsync(user);
            }

            return result;
        }

        public static async Task<IdentityResult> UpdateRolesAsync(this FaToolUserManager um, FaToolUser user, params string[] roles)
        {

            if (roles == null)
                roles = new string[] { };

            var currentRoles = await um.GetRolesAsync(user.Id);
            var result = await um.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());

            if (result.Succeeded)
            {
                result = await um.AddToRolesAsync(user.Id, roles);
            }

            return result;

        }

        public static async Task<IdentityResult> LockAccountAsync(this FaToolUserManager um, FaToolUser user, int? forMinutes)
        {

            if (user.IsAdminUser())
            {                
                return IdentityResult.Failed("Can't lock admin user account.");
            }

            var result = await um.SetLockoutEnabledAsync(user.Id, true);

            if (result.Succeeded)
            {
                if (forMinutes.HasValue)
                {
                    result = await um.SetLockoutEndDateAsync(user.Id, DateTimeOffset.UtcNow.AddMinutes(forMinutes.Value));
                }
                else
                {
                    result = await um.SetLockoutEndDateAsync(user.Id, DateTimeOffset.MaxValue);
                }
            }

            return result;
        }

        public static async Task<IdentityResult> UnlockAccountAsync(this FaToolUserManager um, FaToolUser user)
        {
            
            if (await um.IsLockedOutAsync(user.Id) == false)
                return IdentityResult.Success;

            var result = await um.SetLockoutEndDateAsync(user.Id, DateTimeOffset.UtcNow);

            if (!result.Succeeded)
                return result;

            result = await um.SetLockoutEnabledAsync(user.Id, false);

            if (!result.Succeeded)
                return result;

            result = await um.ResetAccessFailedCountAsync(user.Id);

            if (!result.Succeeded)
                return result;

            return result;
        }

    }
}
