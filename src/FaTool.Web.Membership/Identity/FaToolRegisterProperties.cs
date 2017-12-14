using System.ComponentModel.DataAnnotations;

namespace FaTool.Web.Membership.Identity
{

    public sealed class FaToolRegisterProperties : FaToolPersonalProperties
    {

        public FaToolRegisterProperties() { }

        public FaToolRegisterProperties(IFaToolPersonalProperties properties) 
            : base(properties)  {  }

        [Display(Name = "User Name")]
        [CustomValidationAttribute(typeof(FaToolUserValidator), "ValidateUserName")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [CustomValidationAttribute(typeof(FaToolPasswordValidator), "ValidatePassword")]
        public string Password { get; set; }

        [CustomValidationAttribute(typeof(FaToolPasswordValidator), "ValidatePassword")]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}