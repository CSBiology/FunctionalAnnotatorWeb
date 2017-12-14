using FaTool.Web.Membership.Identity;

namespace FaTool.Web.Models.Account
{
    public sealed class ChangePasswordViewModel : UserViewModelBase<FaToolChangePasswordProperties>
    {

        public ChangePasswordViewModel(FaToolUser user, FaToolChangePasswordProperties properties)
            : base(user, properties) { }

        public ChangePasswordViewModel(FaToolUser user)
            : this(user, new FaToolChangePasswordProperties()) { }

        public override string Caption
        {
            get
            {
                return string.Format("Change Password for User '{0}'", User.UserName);
            }
        }
    }
}