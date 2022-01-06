namespace SuperScrabble.WebApi.Resources
{
    public static class User
    {
        public static class ErrorCodes
        {
            public const string EmailRequired = "EmailRequired";
            public const string InvalidEmail = "InvalidEmail";
            public const string UserNameRequired = "UserNameRequired";
            public const string PasswordRequired = "PasswordRequired";
            public const string RepeatedPasswordRequired = "RepeatedPasswordRequired";
            public const string PasswordsDoNotMatch = "PasswordsDoNotMatch";
            public const string UserWithSuchNameNotFound = "UserWithSuchNameNotFound";
            public const string UserWithSuchEmailNotFound = "UserWithSuchEmailNotFound";
            public const string PasswordTooShort = "PasswordTooShort";

            public const string PasswordMismatch = "PasswordMismatch";
            public const string InvalidUserName = "InvalidUserName";
            public const string DuplicateUserName = "DuplicateUserName";
            public const string DuplicateEmail = "DuplicateEmail";
            public const string PasswordRequiresNonAlphanumeric = "PasswordRequiresNonAlphanumeric";
            public const string PasswordRequiresDigit = "PasswordRequiresDigit";
            public const string PasswordRequiresLower = "PasswordRequiresLower";
            public const string PasswordRequiresUpper = "PasswordRequiresUpper";
        }

        public static class PropertyNames
        {
            public const string Email = "Email";
            public const string UserName = "UserName";
            public const string Password = "Password";
            public const string RepeatedPassword = "RepeatedPassword";
        }
    }
}
