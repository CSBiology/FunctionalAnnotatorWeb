using System.ComponentModel.DataAnnotations;

namespace FaTool.Web.Membership.Identity
{
    public sealed class FaToolLoginProperties
    {

        [Display(Name = "User Name")]
        [CustomValidationAttribute(typeof(FaToolUserValidator), "ValidateUserName")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [CustomValidationAttribute(typeof(FaToolPasswordValidator), "ValidatePassword")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}