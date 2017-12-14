using System.Collections.Generic;
using System.Web.Mvc;
using System;
using FaTool.Web.Membership.Identity;
using System.Linq;

namespace FaTool.Web.Models.Account
{
    public sealed class EditUserViewModel : UserViewModelBase<FaToolPersonalProperties>
    {

        public EditUserViewModel(
            FaToolUser user,
            FaToolPersonalProperties properties,
            IEnumerable<SelectListItem> countryOptions)
            : base(user, properties)
        {
            if (countryOptions == null)
                throw new ArgumentNullException("countryOptions");

            CountryOptions = countryOptions.ToArray();
        }

        public EditUserViewModel(
            FaToolUser user,
            IEnumerable<SelectListItem> countryOptions)
            : this(user, new FaToolPersonalProperties(user), countryOptions) { }

        public IEnumerable<SelectListItem> CountryOptions { get; private set; }

        public override string Caption
        {
            get
            {
                return string.Format("Edit User '{0}'", User.UserName);
            }
        }
    }
}