using System.Collections.Generic;
using System.Web.Mvc;
using FaTool.Web.Membership.Identity;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Account
{
    public sealed class EditUserRolesViewModel : UserViewModelBase<CheckList>
    {

        public EditUserRolesViewModel(FaToolUser user, IEnumerable<SelectListItem> roles)
            : base(user, new CheckList("SelectedRoles", roles))
        { }

        public override string Caption
        {
            get
            {
                return string.Format("Grant/Revoke Role Privileges for User '{0}'", User.UserName);
            }
        }

    }
}