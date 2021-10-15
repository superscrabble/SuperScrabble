namespace SuperScrabble.LanguageResources
{
    using System;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource()
        {
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SuperScrabble.LanguageResources.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        public static string EmailAddressDisplayName
            => ResourceManager.GetString("EmailAddressDisplayName", resourceCulture);

        public static string EmailAddressIsInvalid
            => ResourceManager.GetString("EmailAddressIsInvalid", resourceCulture);

        public static string EmailAddressIsRequired
            => ResourceManager.GetString("EmailAddressIsRequired", resourceCulture);

        public static string PasswordDisplayName
            => ResourceManager.GetString("PasswordDisplayName", resourceCulture);

        public static string PasswordIsRequired
            => ResourceManager.GetString("PasswordIsRequired", resourceCulture);

        public static string RepeatedPasswordDisplayName
            => ResourceManager.GetString("RepeatedPasswordDisplayName", resourceCulture);

        public static string RepeatedPasswordIsRequired
            => ResourceManager.GetString("RepeatedPasswordIsRequired", resourceCulture);

        public static string UserNameDisplayName
            => ResourceManager.GetString("UserNameDisplayName", resourceCulture);

        public static string UserNameIsRequired
            => ResourceManager.GetString("UserNameIsRequired", resourceCulture);

        public static string PasswordRequiresDigit
            => ResourceManager.GetString("PasswordRequiresDigit", resourceCulture);

        public static string PasswordRequiresUppercase
            => ResourceManager.GetString("PasswordRequiresUppercase", resourceCulture);

        public static string PasswordRequiresLowercase
            => ResourceManager.GetString("PasswordRequiresLowercase", resourceCulture);

        public static string PasswordRequiresNonAlphanumeric
            => ResourceManager.GetString("PasswordRequiresNonAlphanumeric", resourceCulture);

        public static string PasswordIsTooShort
            => ResourceManager.GetString("PasswordIsTooShort", resourceCulture);
    }
}
