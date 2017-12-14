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
