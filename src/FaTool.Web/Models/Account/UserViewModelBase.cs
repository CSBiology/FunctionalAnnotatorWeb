using System;
using FaTool.Web.Membership.Identity;

namespace FaTool.Web.Models.Account
{
    public abstract class UserPropertyViewModelBase<TProperties>
    {

        public UserPropertyViewModelBase(TProperties properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            Properties = properties;
        }

        public abstract string Caption { get; }

        public TProperties Properties { get; private set; }

    }    

    public abstract class UserViewModelBase<TProperties>
        : UserPropertyViewModelBase<TProperties>
    {

        public UserViewModelBase(FaToolUser user, TProperties properties)
            : base(properties)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            User = user;
        }

        public FaToolUser User { get; private set; }

        public string Id { get { return User.Id; } }
    }
}