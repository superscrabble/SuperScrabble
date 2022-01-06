﻿namespace SuperScrabble.Common
{
    using Microsoft.AspNetCore.Identity;

    using static SuperScrabble.WebApi.Resources.User.ErrorCodes;
    using static SuperScrabble.WebApi.Resources.User.PropertyNames;

    public static class IdentityResultExtensions
    {
        private static readonly Dictionary<string, string> propertyNamesByErrorCodes = new()
        {
            [PasswordMismatch] = Password,
            [InvalidUserName] = UserName,
            [InvalidEmail] = Email,
            [DuplicateUserName] = UserName,
            [DuplicateEmail] = Email,
            [PasswordTooShort] = Password,
            [PasswordRequiresNonAlphanumeric] = Password,
            [PasswordRequiresDigit] = Password,
            [PasswordRequiresLower] = Password,
            [PasswordRequiresUpper] = Password,
        };

        public static Dictionary<string, List<string>> GetErrors(this IdentityResult result)
        {
            return result.Errors.GroupBy(err =>
                propertyNamesByErrorCodes[err.Code], err => err.Code)
                .ToDictionary(errors => errors.Key, errors => errors.ToList());
        }
    }
}
