﻿using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using FaTool.Web.Membership.Auth;
using FaTool.Web.Membership.Identity;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FaTool.Web.Controllers
{

    [Authorize]
    public abstract class AccountControllerBase : Controller
    {

        private FaToolAuthenticationManager authenticationManager;
        private FaToolUserStore userStore;
        private FaToolUserManager userManager;
        private FaToolRoleStore roleStore;
        private FaToolRoleManager roleManager;

        protected FaToolAuthenticationManager AuthenticationManager
        {
            get
            {
                if (authenticationManager == null)
                    authenticationManager = new FaToolAuthenticationManager();
                return authenticationManager;
            }
        }

        protected FaToolUserStore UserStore
        {
            get
            {
                if (userStore == null)
                    userStore = new FaToolUserStore();
                return userStore;
            }
        }

        protected FaToolUserManager UserManager
        {
            get
            {
                if (userManager == null)
                    userManager = new FaToolUserManager(UserStore);
                return userManager;
            }
        }

        protected FaToolRoleStore RoleStore
        {
            get
            {
                if (roleStore == null)
                    roleStore = new FaToolRoleStore();
                return roleStore;
            }
        }

        protected FaToolRoleManager RoleManager
        {
            get
            {
                if (roleManager == null)
                    roleManager = new FaToolRoleManager(RoleStore);
                return roleManager;
            }
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected IEnumerable<SelectListItem> GetCountryOptions(string selected = null)
        {
            return Countries.Values.Select(x => new SelectListItem()
            {
                Selected = x.Code == selected,
                Value = x.Code,
                Text = x.Name
            });
        }

        protected async Task<IEnumerable<SelectListItem>> GetRoleOptions(FaToolUser user)
        {
            var selectedRoles = await UserManager.GetRolesAsync(user.Id);

            var allRoles = await RoleManager
                .Roles
                .OrderBy(x => x.Name)
                .ToArrayAsync();

            return allRoles.Select(x => new SelectListItem()
                {
                    Value = x.Name,
                    Text = x.Name,
                    Selected = selectedRoles.Contains(x.Name)
                });
        }

        #region controller override

        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {
                if (userManager != null)
                    userManager.Dispose();
                if (userStore != null)
                    userStore.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}