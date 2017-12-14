using System.Collections.Generic;
using FaTool.Web.Membership.Identity;

namespace FaTool.Web.Models.Account
{
    public sealed class AccountListViewModel
    {

        public AccountListViewModel(IEnumerable<FaToolUser> users)
        {
            Accounts = new AccountList(users);
        }

        public AccountList Accounts { get; private set; }
    }
}