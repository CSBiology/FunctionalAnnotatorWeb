using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Linq;

namespace FaTool.Web.Membership.Identity
{
    public sealed class FaToolIdentityDbInitializer : CreateDatabaseIfNotExists<FaToolIdentityDbContext>
    {

        protected override void Seed(FaToolIdentityDbContext context)
        {
            CreateRoles(context);
            CreateAdminUser(context);

            base.Seed(context);
        }

        private static void CreateRoles(FaToolIdentityDbContext context)
        {
            using (var store = new FaToolRoleStore(context))
            using (var manager = new FaToolRoleManager(store))
            {
                foreach (var name in FaToolRoles.Values)
                {
                    CreateRole(manager, name);
                }
            }
        }

        private static void CreateRole(FaToolRoleManager roleManager, string roleName)
        {
            bool roleExists = roleManager.RoleExists(roleName);

            if (roleExists == false)
            {
                var role = new IdentityRole(roleName);
                var result = roleManager.Create(role);

                if (result.Succeeded == false)
                    throw new ApplicationException(
                        string.Format("Error creating role '{0}', reason: {1}.", roleName, result.Errors.FirstOrDefault()));
            }

        }

        private static void CreateAdminUser(FaToolIdentityDbContext context)
        {
            using (var store = new FaToolUserStore(context))
            using (var manager = new FaToolUserManager(store))
            {
                var user = manager.FindByName(FaToolUsers.ADMIN);

                if (user == null)
                {
                    user = new FaToolUser(FaToolUsers.ADMIN);

                    user.Email = "admin@fatool.com";
                    user.FirstName = "FirstName";
                    user.LastName = "LastName";
                    user.Company = "Company";
                    user.Address = "Address";
                    user.City = "City";
                    user.ZipCode = "ZipCode";
                    user.State = "State";
                    user.Country = Countries.Default.Code;                    

                    var result = manager.Create(user, "Admin123#");

                    if (result.Succeeded == false)
                        throw new ApplicationException(
                            string.Format("Error creating admin user, reason: {0}.", result.Errors.FirstOrDefault()));

                    result = manager.AddToRoles(user.Id, FaToolRoles.Values.ToArray());

                    if (result.Succeeded == false)
                        throw new ApplicationException(
                            string.Format("Error grant admin user to roles, reason: {0}.", result.Errors.FirstOrDefault()));
                }
            }
        }
    }
}