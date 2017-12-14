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
