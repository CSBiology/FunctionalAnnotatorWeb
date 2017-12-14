using System.ComponentModel.DataAnnotations;

namespace FaTool.Web.Membership.Identity
{
    public sealed class FaToolLockoutProperties
    {

        public FaToolLockoutProperties()
        {
            this.IsLockedOut = false;            
        }

        [Display(Name = "Is Locked Out")]
        public bool IsLockedOut { get; set; }
       
    }
}
