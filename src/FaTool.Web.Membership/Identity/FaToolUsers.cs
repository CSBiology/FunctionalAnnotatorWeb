
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace FaTool.Web.Membership.Identity
{
    public static class FaToolUsers
    {
        public const string ADMIN = "admin";

        public static IEnumerable<string> Values
        {
            get
            {
                return typeof(FaToolUsers)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                    .Select(x => x.GetRawConstantValue())
                    .OfType<string>()
                    .ToArray();
            }
        }
    }
}