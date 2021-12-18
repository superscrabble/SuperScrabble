namespace SuperScrabble.LanguageResources
{
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
            get => resourceCulture;
            set => resourceCulture = value;
        }

        public static string EmailAddressDisplayName
            => ResourceManager.GetString(nameof(EmailAddressDisplayName), resourceCulture);

        public static string EmailAddressIsInvalid
            => ResourceManager.GetString(nameof(EmailAddressIsInvalid), resourceCulture);

        public static string EmailAddressIsRequired
            => ResourceManager.GetString(nameof(EmailAddressIsRequired), resourceCulture);

        public static string PasswordDisplayName
            => ResourceManager.GetString(nameof(PasswordDisplayName), resourceCulture);

        public static string PasswordIsRequired
            => ResourceManager.GetString(nameof(PasswordIsRequired), resourceCulture);

        public static string RepeatedPasswordDisplayName
            => ResourceManager.GetString(nameof(RepeatedPasswordDisplayName), resourceCulture);

        public static string RepeatedPasswordIsRequired
            => ResourceManager.GetString(nameof(RepeatedPasswordIsRequired), resourceCulture);

        public static string UserNameDisplayName
            => ResourceManager.GetString(nameof(UserNameDisplayName), resourceCulture);

        public static string UserNameIsRequired
            => ResourceManager.GetString(nameof(UserNameIsRequired), resourceCulture);

        public static string PasswordRequiresDigit
            => ResourceManager.GetString(nameof(PasswordRequiresDigit), resourceCulture);

        public static string PasswordRequiresUppercase
            => ResourceManager.GetString(nameof(PasswordRequiresUppercase), resourceCulture);

        public static string PasswordRequiresLowercase
            => ResourceManager.GetString(nameof(PasswordRequiresLowercase), resourceCulture);

        public static string PasswordRequiresNonAlphanumeric
            => ResourceManager.GetString(nameof(PasswordRequiresNonAlphanumeric), resourceCulture);

        public static string PasswordIsTooShort
            => ResourceManager.GetString(nameof(PasswordIsTooShort), resourceCulture);

        public static string UserNameDoesNotExist
            => ResourceManager.GetString(nameof(UserNameDoesNotExist), resourceCulture);

        public static string PasswordIsInvalid
            => ResourceManager.GetString(nameof(PasswordIsInvalid), resourceCulture);

        public static string EmailAddressAlreadyExists
            => ResourceManager.GetString(nameof(EmailAddressAlreadyExists), resourceCulture);

        public static string UserNameAlreadyExists
            => ResourceManager.GetString(nameof(UserNameAlreadyExists), resourceCulture);

        public static string OldUserNameIsRequired
            => ResourceManager.GetString(nameof(OldUserNameIsRequired), resourceCulture);

        public static string NewUserNameIsRequired
            => ResourceManager.GetString(nameof(NewUserNameIsRequired), resourceCulture);

        public static string NewUserNameDisplayName
            => ResourceManager.GetString(nameof(NewUserNameDisplayName), resourceCulture);

        public static string OldUserNameDisplayName
            => ResourceManager.GetString(nameof(OldUserNameDisplayName), resourceCulture);

        public static string OldPasswordIsRequired
            => ResourceManager.GetString(nameof(OldPasswordIsRequired), resourceCulture);

        public static string NewPasswordIsRequired
            => ResourceManager.GetString(nameof(NewPasswordIsRequired), resourceCulture);

        public static string OldPasswordDisplayName
            => ResourceManager.GetString(nameof(OldPasswordDisplayName), resourceCulture);

        public static string NewPasswordDisplayName
            => ResourceManager.GetString(nameof(NewPasswordDisplayName), resourceCulture);

        public static string OldEmailAddressDisplayName
            => ResourceManager.GetString(nameof(OldEmailAddressDisplayName), resourceCulture);

        public static string NewEmailAddressDisplayName
            => ResourceManager.GetString(nameof(NewEmailAddressDisplayName), resourceCulture);

        public static string PasswordsMismatch
            => ResourceManager.GetString(nameof(PasswordsMismatch), resourceCulture);

        public static string InvalidInputTilesCount
            => ResourceManager.GetString(nameof(InvalidInputTilesCount), resourceCulture);

        public static string UnexistingPlayerTile
            => ResourceManager.GetString(nameof(UnexistingPlayerTile), resourceCulture);

        public static string TilePositionOutsideBoardRange
            => ResourceManager.GetString(nameof(TilePositionOutsideBoardRange), resourceCulture);

        public static string TilePositionAlreadyTaken
            => ResourceManager.GetString(nameof(TilePositionAlreadyTaken), resourceCulture);

        public static string TilesNotOnTheSameLine
            => ResourceManager.GetString(nameof(TilesNotOnTheSameLine), resourceCulture);

        public static string InputTilesPositionsCollide
            => ResourceManager.GetString(nameof(InputTilesPositionsCollide), resourceCulture);

        public static string FirstWordMustBeOnTheBoardCenter
            => ResourceManager.GetString(nameof(FirstWordMustBeOnTheBoardCenter), resourceCulture);

        public static string WordDoesNotExist
            => ResourceManager.GetString(nameof(WordDoesNotExist), resourceCulture);

        public static string GapsBetweenInputTilesNotAllowed
            => ResourceManager.GetString(nameof(GapsBetweenInputTilesNotAllowed), resourceCulture);

        public static string NewTilesNotConnectedToTheOldOnes
            => ResourceManager.GetString(nameof(NewTilesNotConnectedToTheOldOnes), resourceCulture);

        public static string TheGivenPlayerIsNotOnTurn
            => ResourceManager.GetString(nameof(TheGivenPlayerIsNotOnTurn), resourceCulture);

        public static string ImpossibleTileExchange
            => ResourceManager.GetString(nameof(ImpossibleTileExchange), resourceCulture);
    }
}
