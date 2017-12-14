#region license
// The MIT License (MIT)

// FaToolIdentityDbInitializer.cs

// Copyright (c) 2016 Alexander Lüdemann
// alexander.luedemann@outlook.com
// luedeman@rhrk.uni-kl.de

// Computational Systems Biology, Technical University of Kaiserslautern, Germany
 

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

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