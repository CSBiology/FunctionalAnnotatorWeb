using FaTool.Web.Membership.Identity;

namespace FaTool.Web.Models.Account
{
    public sealed class LockoutUserViewModel : UserViewModelBase<FaToolLockoutProperties>
    {

        public LockoutUserViewModel(FaToolUser user, FaToolLockoutProperties properties)
            : base(user, properties) { }

        public LockoutUserViewModel(FaToolUser user)
            : this(user, new FaToolLockoutProperties()) { }

        public override string Caption
        {
            get
            {
                return string.Format("Lock/Unlock Account for User '{0}'", User.UserName);
            }
        }

    }
}