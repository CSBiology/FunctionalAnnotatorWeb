using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using FaTool.Web.Membership.Auth;
using FaTool.Web.Membership.Identity;
using FaTool.Web.Models.Account;
using FaTool.Web.Models.UserInterface;
using Microsoft.AspNet.Identity;
using System.Linq;

namespace FaTool.Web.Controllers
{

    public sealed class AccountController : AccountControllerBase
    {

        #region Actions

        [HttpGet]
        [Authorize(Roles = FaToolRoles.ADMIN, Users = FaToolUsers.ADMIN)]
        public async Task<ActionResult> Index()
        {
            var users = await UserManager.Users.OrderBy(x => x.UserName).ToArrayAsync();
            var model = new AccountListViewModel(users);
            model.Accounts.AddRowAction(ShowUserAction);
            model.Accounts.AddRowAction(EditUserAction);
            model.Accounts.AddRowAction(ChangePasswordAction);
            model.Accounts.AddRowAction(GrantRolesAction);
            model.Accounts.AddRowAction(LockUserAction);
            model.Accounts.AddRowAction(DeleteUserAction);
            model.Accounts.Actions.Add(AddUserAction());
            return View(model);
        }

        [HttpGet]
        [ActionName("User")]
        [Authorize]
        public async Task<ActionResult> Get(string id)
        {

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            if (User.IsUserOrInRole(user.UserName, FaToolRoles.ADMIN) == false)
                return HttpStatusCodeResults.HttpUnauthorized();

            var model = new UserViewModel(user);

            model.Properties.Actions.Add(EditUserAction(user));
            model.Properties.Actions.Add(ChangePasswordAction(user));
            model.Properties.Actions.Add(GrantRolesAction(user));
            model.Properties.Actions.Add(LockUserAction(user));
            model.Properties.Actions.Add(ShowUserListAction());

            return View("User", model);

        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            FaToolLoginProperties login = new FaToolLoginProperties();
            login.RememberMe = false;
            login.UserName = string.Empty;
            login.Password = string.Empty;
            login.ReturnUrl = returnUrl;

            return View("Login", login);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(FaToolLoginProperties login)
        {
            if (ModelState.IsValid)
            {
                if (User.IsAuthenticated())
                    await AuthenticationManager.SignOutAsync();

                var result = await AuthenticationManager.SignInAsync(
                        HttpContext.ApplicationInstance.Context,
                        UserManager,
                        login.UserName,
                        login.Password,
                        login.RememberMe);

                switch (result)
                {
                    case AuthenticationResult.SUCCESS:
                        return RedirectToLocal(login.ReturnUrl);
                    case AuthenticationResult.INVALID_PASSWORD:
                        ModelState.AddModelError("Password", "Invalid password.");
                        return View("Login", login);
                    case AuthenticationResult.UNKNOWN_USER:
                        ModelState.AddModelError("UserName", "Unknown user name.");
                        return View("Login", login);
                    case AuthenticationResult.USER_LOCKED_OUT:
                        ModelState.AddModelError("UserName", "User account is locked out.");
                        return View("Login", login);
                    default:
                        throw new InvalidOperationException("Unhandled result.");
                }
            }
            else
            {
                return View("Login", login);
            }
        }

        [HttpGet]
        [Authorize]
        [ActionName("Logout")]
        public ActionResult Logout()
        {
            return View("Logout");
        }

        [HttpPost]
        [ActionName("Logout")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogoutConfirmed()
        {
            await AuthenticationManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            var model = new RegisterViewModel(GetCountryOptions(Countries.Default.Code));
            return View("Register", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(
            [Bind(Prefix = "Properties")] FaToolRegisterProperties properties)
        {

            if (ModelState.IsValid)
            {
                FaToolUser user = new FaToolUser(properties);
                IdentityResult result = await UserManager.CreateAccountAsync(
                    user,
                    properties.Password,
                    FaToolRoles.USER);

                if (result.Succeeded)
                {
                    if (User.IsAdminUserOrAdminRole())
                    {
                        return this.Redirect(ShowUserAction(user).Url);
                    }
                    else
                    {
                        if (User.IsAuthenticated())
                            await AuthenticationManager.SignOutAsync();
                        // TODO implement email approve
                        //return View("RegisterWelcome");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    AddErrors(result);

                    var model = new RegisterViewModel(
                        properties,
                        GetCountryOptions(properties.Country));

                    return View("Register", model);
                }
            }
            else
            {
                var model = new RegisterViewModel(
                        properties,
                        GetCountryOptions(properties.Country));

                return View("Register", model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Current()
        {

            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return HttpNotFound("User not found.");

            return Redirect(ShowUserAction(user).Url);

        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            if (User.IsUserOrInRole(user.UserName, FaToolRoles.ADMIN) == false)
                return HttpStatusCodeResults.HttpUnauthorized();

            var model = new EditUserViewModel(
                user,
                GetCountryOptions(user.Country));

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit(
            string id,
            [Bind(Prefix = "Properties")] FaToolPersonalProperties properties)
        {

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            if (User.IsUserOrInRole(user.UserName, FaToolRoles.ADMIN) == false)
                return HttpStatusCodeResults.HttpUnauthorized();

            if (ModelState.IsValid)
            {
                user.SetProperties(properties);

                IdentityResult result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Redirect(ShowUserAction(user).Url);
                }
                else
                {
                    AddErrors(result);

                    var model = new EditUserViewModel(
                    user,
                    properties,
                    GetCountryOptions(properties.Country));

                    return View(model);
                }
            }
            else
            {
                var model = new EditUserViewModel(
                        user,
                        properties,
                        GetCountryOptions(properties.Country));

                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> ChangePassword(string id)
        {

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            var model = new ChangePasswordViewModel(user);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> ChangePassword(
            string id,
            [Bind(Prefix = "Properties")] FaToolChangePasswordProperties properties)
        {

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            if (ModelState.IsValid)
            {
                if (await UserManager.CheckPasswordAsync(user, properties.CurrentPassword))
                {
                    var result = await UserManager.ChangePasswordAsync(
                        user.Id,
                        properties.CurrentPassword,
                        properties.NewPassword);

                    if (result.Succeeded)
                    {
                        return Redirect(ShowUserAction(user).Url);
                    }
                    else
                    {
                        AddErrors(result);
                        var model = new ChangePasswordViewModel(user, properties);
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("Properties.CurrentPassword", "The password is invalid.");
                    var model = new ChangePasswordViewModel(user, properties);
                    return View(model);
                }
            }
            else
            {
                var model = new ChangePasswordViewModel(user, properties);
                return View(model);
            }

        }

        [HttpGet]
        [Authorize(Roles = FaToolRoles.ADMIN, Users = FaToolUsers.ADMIN)]
        public async Task<ActionResult> Roles(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            var roleOptions = await GetRoleOptions(user);

            var model = new EditUserRolesViewModel(user, roleOptions);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = FaToolRoles.ADMIN, Users = FaToolUsers.ADMIN)]
        public async Task<ActionResult> Roles(
            string id,
            params string[] selectedRoles)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            var result = await UserManager.UpdateRolesAsync(user, selectedRoles);

            if (result.Succeeded == false)
            {
                AddErrors(result);
            }

            return Redirect(ShowUserAction(user).Url);
        }

        [HttpGet]
        [Authorize(Users = FaToolUsers.ADMIN)]
        public async Task<ActionResult> Lockout(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            var model = new LockoutUserViewModel(user);

            model.Properties.IsLockedOut = await UserManager.IsLockedOutAsync(user.Id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Users = FaToolUsers.ADMIN)]
        public async Task<ActionResult> Lockout(
            string id,
            [Bind(Prefix = "Properties")] FaToolLockoutProperties properties)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            if (ModelState.IsValid)
            {

                IdentityResult result;

                if (properties.IsLockedOut)
                {
                    result = await UserManager.LockAccountAsync(user, null);
                }
                else
                {
                    result = await UserManager.UnlockAccountAsync(user);
                }

                if (result.Succeeded)
                {
                    return Redirect(ShowUserAction(user).Url);
                }
                else
                {
                    AddErrors(result);
                    var model = new LockoutUserViewModel(user, properties);
                    return View(model);
                }
            }
            else
            {
                var model = new LockoutUserViewModel(user, properties);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Users = FaToolUsers.ADMIN)]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            return View(user);            
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Users = FaToolUsers.ADMIN)]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound("User not found.");

            var result = await UserManager.DeleteAccountAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                AddErrors(result);
                return View(user);
            }
        }

        #endregion

        #region helper

        private ActionResult RedirectToLocal(string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl) == false
                && Url.IsLocalUrl(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            else
            {
                return Redirect(Url.Action("Index", "Home"));
            }
        }

        #endregion

        #region actions

        private ActionLink AddUserAction()
        {
            bool disabled = User.IsAdminUserOrAdminRole() == false;

            return new ActionLink("Add User", Url.Action("Register"), disabled);
        }

        private ActionLink ShowUserListAction()
        {
            bool disabled = User.IsAdminUserOrAdminRole() == false;

            return new ActionLink(
                "User List",
                Url.Action("Index"), disabled);
        }

        private ActionLink ChangePasswordAction(FaToolUser user)
        {
            return new ActionLink(
                "Change Password",
                Url.Action("ChangePassword", new { id = user.Id }));
        }

        private ActionLink EditUserAction(FaToolUser user)
        {
            return new ActionLink(
                "Edit User",
                Url.Action("Edit", new { id = user.Id }));
        }

        private ActionLink GrantRolesAction(FaToolUser user)
        {
            bool disabled = User.IsAdminUserOrAdminRole() == false;

            return new ActionLink(
                "Grant/Revoke Roles",
                Url.Action("Roles", new { id = user.Id }), disabled);
        }

        private ActionLink LockUserAction(FaToolUser user)
        {
            bool disabled = User.IsAdminUser() == false || user.IsAdminUser();

            return new ActionLink(
                "Lock/Unlock User",
                Url.Action("Lockout", new { id = user.Id }), disabled);
        }

        private ActionLink DeleteUserAction(FaToolUser user)
        {
            bool disabled = User.IsAdminUser() == false || user.IsAdminUser();

            return new ActionLink(
                "Delete User",
                Url.Action("Delete", new { id = user.Id }), disabled);
        }

        private ActionLink ShowUserAction(FaToolUser user)
        {
            return new ActionLink(
                "Show",
                Url.Action("User", new { id = user.Id }));
        }

        #endregion
    }
}