using Microsoft.AspNet.Identity.EntityFramework;

namespace FaTool.Web.Membership.Identity
{
    public sealed class FaToolUser : IdentityUser, IFaToolPersonalProperties
    {

        public FaToolUser() : base() { }

        public FaToolUser(string userName)
            : base(userName) {  }

        public FaToolUser(FaToolRegisterProperties properties)
            : this(properties.UserName) 
        {
            this.SetProperties(properties);
        }

        #region IFaToolUserProperties Members

        public string FirstName { get; set; }        
        public string LastName { get; set; }        
        public string Company { get; set; }        
        public string Address { get; set; }        
        public string City { get; set; }        
        public string ZipCode { get; set; }        
        public string State { get; set; }        
        public string Country { get; set; }

        #endregion        
    }
}