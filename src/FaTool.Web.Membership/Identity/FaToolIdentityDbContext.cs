using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FaTool.Web.Membership.Identity
{
    public class FaToolIdentityDbContext : IdentityDbContext<FaToolUser>
    {

        static FaToolIdentityDbContext()
        {
            Database.SetInitializer(new FaToolIdentityDbInitializer());
        }

        public FaToolIdentityDbContext()
            : base("FaToolIdentityDbConnectionString", throwIfV1Schema: false)
        {
            RequireUniqueEmail = true;            
        }
    }
}