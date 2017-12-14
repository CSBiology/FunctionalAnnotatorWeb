using System.ComponentModel.DataAnnotations;

namespace FaTool.Web.Membership.Identity
{
    public sealed class FaToolChangePasswordProperties
    {
                
        [Display(Name = "Current Password")]
        [CustomValidationAttribute(typeof(FaToolPasswordValidator), "ValidatePassword")]
        public string CurrentPassword { get; set; }
        
        [Display(Name = "New Password")]
        [CustomValidationAttribute(typeof(FaToolPasswordValidator), "ValidatePassword")]
        public string NewPassword { get; set; }
        
        [Display(Name = "Confirm Password")]
        [CustomValidationAttribute(typeof(FaToolPasswordValidator), "ValidatePassword")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }  
    }
}