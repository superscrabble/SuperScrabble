namespace SuperScrabble.WebApi.Resources
{
    public static class UserValidationErrorCodes
    {
        public const string EmailRequired = nameof(EmailRequired);
        public const string InvalidEmail = nameof(InvalidEmail);
        public const string UserNameRequired = nameof(UserNameRequired);
        public const string PasswordRequired = nameof(PasswordRequired);
        public const string RepeatedPasswordRequired = nameof(RepeatedPasswordRequired);
        public const string PasswordsDoNotMatch = nameof(PasswordsDoNotMatch);
        public const string UserWithSuchNameNotFound = nameof(UserWithSuchNameNotFound);
        public const string UserWithSuchEmailNotFound = nameof(UserWithSuchEmailNotFound);
        public const string PasswordTooShort = nameof(PasswordTooShort);
    }
}
