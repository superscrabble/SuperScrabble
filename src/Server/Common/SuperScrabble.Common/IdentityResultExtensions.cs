namespace SuperScrabble.Common
{
    using Microsoft.AspNetCore.Identity;

    public static class IdentityResultExtensions
    {
        private static readonly Dictionary<string, string> propertyNamesByErrorCodes = new()
        {
            ["PasswordMismatch"] = "Password",
            ["InvalidUserName"] = "UserName",
            ["InvalidEmail"] = "Email",
            ["DuplicateUserName"] = "UserName",
            ["DuplicateEmail"] = "Email",
            ["PasswordTooShort"] = "Password",
            ["PasswordRequiresNonAlphanumeric"] = "Password",
            ["PasswordRequiresDigit"] = "Password",
            ["PasswordRequiresLower"] = "Password",
            ["PasswordRequiresUpper"] = "Password",
        };

        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetErrors(this IdentityResult result)
        {
            return result.Errors.GroupBy(err =>
                propertyNamesByErrorCodes[err.Code], err => err.Code)
                .ToDictionary(x => x.Key, x => x.AsEnumerable());
        }
    }
}
