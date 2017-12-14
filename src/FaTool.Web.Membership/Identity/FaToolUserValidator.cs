#region license
// The MIT License (MIT)

// FaToolUserValidator.cs

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

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace FaTool.Web.Membership.Identity
{
    public sealed class FaToolUserValidator : UserValidator<FaToolUser>
    {

        public const string PATTERN = @"^[A-Za-z0-9]{5,16}";
        public const string ERROR_MSG = "User name must contain: minimum 5, maximum 16 alphanumeric characters.";

        public FaToolUserValidator(FaToolUserManager faToolUserManager)
            : base(faToolUserManager) 
        {
            AllowOnlyAlphanumericUserNames = true;
            RequireUniqueEmail = true;
        }

        #region IIdentityValidator<FaToolUser> Members

        public override async Task<IdentityResult> ValidateAsync(FaToolUser item)
        {
            var result = ValidateUserName(item.UserName);

            if (result == ValidationResult.Success)
                return await base.ValidateAsync(item);
            else
                return new IdentityResult(result.ErrorMessage);
        }

        #endregion

        public static ValidationResult ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return new ValidationResult(ERROR_MSG);

            if (Regex.IsMatch(userName, PATTERN))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ERROR_MSG);
            }
        }
    }
}
