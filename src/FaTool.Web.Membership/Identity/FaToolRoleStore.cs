using Microsoft.AspNet.Identity.EntityFramework;

namespace FaTool.Web.Membership.Identity
{
    public class FaToolRoleStore : RoleStore<IdentityRole>
    {

        public FaToolRoleStore(FaToolIdentityDbContext ctx)
            : base(ctx)
        {
            DisposeContext = false;
        }

        public FaToolRoleStore()
            : base(new FaToolIdentityDbContext())
        {
            DisposeContext = true;
        }

    }
}