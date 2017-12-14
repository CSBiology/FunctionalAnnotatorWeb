using FaTool.Web.Membership.Identity;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Account
{
    public sealed class UserViewModel : UserViewModelBase<IObjectView<FaToolUser>>
    {

        public UserViewModel(FaToolUser user) 
            : base(user, new UserRecordView(user))
        {            
        }


        public override string Caption
        {
            get { return Properties.Caption; }
        }
    }
}