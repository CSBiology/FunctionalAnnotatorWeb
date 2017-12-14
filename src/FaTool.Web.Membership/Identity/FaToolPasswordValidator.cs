#region license
// The MIT License (MIT)

// FaToolPasswordValidator.cs

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

    public sealed class FaToolPasswordValidator : IIdentityValidator<string>
    {

        public const string PATTERN = @"(?=^.{8,16}$)((?=.*\d)|(?=.*\W+))(?!.*\s)(?=.*[A-Z])(?=.*[a-z]).*$";
        public const string ERROR_MSG = "Password must contain: Minimum 8, Maximum 16 characters, at least 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Number and 1 Special Character.";

        public FaToolPasswordValidator() { }

        #region IIdentityValidator<string> Members

        public Task<IdentityResult> ValidateAsync(string item)
        {
            if (string.IsNullOrWhiteSpace(item))
                return Task.FromResult(new IdentityResult(ERROR_MSG));
            if (Regex.IsMatch(item, PATTERN))
                return Task.FromResult(IdentityResult.Success);
            else
                return Task.FromResult(new IdentityResult(ERROR_MSG));
        }

        #endregion

        public static ValidationResult ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return new ValidationResult(ERROR_MSG);

            if (Regex.IsMatch(password, PATTERN))
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
