using Microsoft.AspNet.Identity.EntityFramework;

namespace FaTool.Web.Membership.Identity
{

    public sealed class FaToolUserStore : UserStore<FaToolUser>
    {

        public FaToolUserStore(FaToolIdentityDbContext ctx)
            : base(ctx)
        {
            DisposeContext = false;
        }

        public FaToolUserStore()
            : base(new FaToolIdentityDbContext())
        {
            DisposeContext = true;
        }

        
    }

}