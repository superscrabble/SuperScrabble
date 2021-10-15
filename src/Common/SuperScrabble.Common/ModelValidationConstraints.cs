namespace SuperScrabble.Common
{
    public static class ModelValidationConstraints
    {
        public static class Password
        {
            public const int MinLength = 8;
            public const bool RequireLowercase = true;
            public const bool RequireUppercase = true;
            public const bool RequireDigit = true;
            public const bool RequireNonAlphanumeric = true;
            public const int RequiredUniqueChars = 1;
        }
    }
}
