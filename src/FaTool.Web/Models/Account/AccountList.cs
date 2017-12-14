using System.Collections.Generic;
using FaTool.Web.Membership.Identity;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Account
{
    public sealed class AccountList : TableView<FaToolUser>
    {

        public AccountList(IEnumerable<FaToolUser> rows)
            : base(rows)
        {
            Caption = "FaTool Membership Accounts";
            AddField("Account", x => x.UserName, x => x.Email);
            AddField("Personal Name", x => string.Format("{0} {1}", x.FirstName, x.LastName));
            AddField("Company/Adress", x => x.Company, x => UserRecordView.FormatAdress(x));
        }        
    }
}