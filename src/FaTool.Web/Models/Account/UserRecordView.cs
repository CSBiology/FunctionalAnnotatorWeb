#region license
// The MIT License (MIT)

// UserRecordView.cs

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