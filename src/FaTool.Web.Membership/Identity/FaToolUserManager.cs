using Microsoft.AspNet.Identity;

namespace FaTool.Web.Membership.Identity
{
    public sealed class FaToolUserManager : UserManager<FaToolUser>
    {

        public FaToolUserManager(FaToolUserStore store)
            : base(store)
        {
            UserValidator = new FaToolUserValidator(this);
            PasswordValidator = new FaToolPasswordValidator();
        }        
    }
}