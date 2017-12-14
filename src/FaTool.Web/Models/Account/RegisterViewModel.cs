using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FaTool.Web.Membership.Identity;
using System.Linq;

namespace FaTool.Web.Models.Account
{
    public sealed class RegisterViewModel : UserPropertyViewModelBase<FaToolRegisterProperties>
    {

        public RegisterViewModel(
            FaToolRegisterProperties properties,
            IEnumerable<SelectListItem> countryOptions)
            : base(properties)
        {
            if (countryOptions == null)
                throw new ArgumentNullException("countryOptions");

            CountryOptions = countryOptions.ToArray();
        }

        public RegisterViewModel(IEnumerable<SelectListItem> countryOptions)
            : this(new FaToolRegisterProperties(), countryOptions) { }

        public override string Caption
        {
            get { return "Sign Up to Functional Annotator"; }
        }

        public IEnumerable<SelectListItem> CountryOptions { get; private set; }
    }
}