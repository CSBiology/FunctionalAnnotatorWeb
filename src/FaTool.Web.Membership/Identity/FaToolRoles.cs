using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace FaTool.Web.Membership.Identity
{
    public static class FaToolRoles
    {
        public const string USER = "user";
        public const string ANNOTATOR = "annotator";
        public const string ADMIN = "administrator";

        public static IEnumerable<string> Values
        {
            get
            {
                return typeof(FaToolRoles)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                    .Select(x => x.GetRawConstantValue())
                    .OfType<string>()
                    .ToArray();
            }
        }
    }
}