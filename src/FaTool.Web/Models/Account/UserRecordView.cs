using FaTool.Web.Membership.Identity;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Account
{
    public sealed class UserRecordView : ObjectView<FaToolUser>
    {
        public UserRecordView(FaToolUser user)
            : base(user)
        {
            Caption = string.Format("FaTool Membership User '{0}'", user.UserName);
            AddField("User Id", x => x.Id);
            AddField("User Name", x => x.UserName);
            AddField("Access Failed Count", x => x.AccessFailedCount);
            AddField("Lockout Enabled", x => x.LockoutEnabled);
            AddField("Lockout End Date", x => x.LockoutEndDateUtc);
            AddField("Email", x => x.Email);
            AddField("Email Confirmed", x => x.EmailConfirmed);
            AddField("Personal Name", x => string.Format("{0} {1}", x.FirstName, x.LastName));
            AddField("Company/Adress", x => x.Company, x => FormatAdress(x));
        }

        public static string FormatAdress(FaToolUser user)
        {
            var country = Countries.CanResolve(user.Country)
                ? Countries.ResolveName(user.Country)
                : string.Empty;

            return string.Format("{0}; {1} {2}; {3}; {4}",
                user.Address,
                user.ZipCode,
                user.City,
                user.State,
                country);
        }
    }
}