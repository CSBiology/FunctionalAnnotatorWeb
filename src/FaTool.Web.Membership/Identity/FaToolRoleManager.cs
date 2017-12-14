using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FaTool.Web.Membership.Identity
{
    public class FaToolRoleManager : RoleManager<IdentityRole>
    {

        public FaToolRoleManager(FaToolRoleStore store)
            : base(store) { }

    }
}