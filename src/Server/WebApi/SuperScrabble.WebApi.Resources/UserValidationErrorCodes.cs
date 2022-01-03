namespace SuperScrabble.WebApi.Resources
{
    public static class UserValidationErrorCodes
    {
        public const string EmailRequired = "EmailRequired";
        public const string InvalidEmail = "InvalidEmail";
        public const string UserNameRequired = "UserNameRequired";
        public const string PasswordRequired = "PasswordRequired";
        public const string RepeatedPasswordRequired = "RepeatedPasswordRequired";
        public const string PasswordsDoNotMatch = "PasswordsDoNotMatch";
        public const string UserWithSuchNameNotFound = "UserWithSuchNameNotFound";
    }
}
