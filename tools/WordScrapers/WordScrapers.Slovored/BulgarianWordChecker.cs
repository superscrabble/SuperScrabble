namespace WordScrapers.Slovored
{
    using System.Text.RegularExpressions;

    public class BulgarianWordChecker : IWordChecker
    {
        private const string UpperYery = "042B";
        private const string LowerYery = "044B";
        private const string UpperBackwardsE = "042D";
        private const string LowerBackwardsE = "044D";
        private const string RangeStart = "0410";
        private const string RangeEnd = "044F";

        private const string ExcludePathFormat = @"\u{0}";
        private const string PathFormat = @"^[\u{0}-\u{1}][^{2}]+$";

        private static readonly string[] CharactersToExclude = new[]
        {
            UpperYery, LowerYery, UpperBackwardsE, LowerBackwardsE
        };

        private readonly Regex validWordRegex;

        public BulgarianWordChecker()
        {
            var formattedCharacters = CharactersToExclude.Select(character => string.Format(ExcludePathFormat, character));

            string excludePath = string.Join(string.Empty, formattedCharacters);
            string regexPath = string.Format(PathFormat, RangeStart, RangeEnd, excludePath);

            this.validWordRegex = new Regex(regexPath);
        }

        public bool IsWordValid(string word) => this.validWordRegex.IsMatch(word);
    }
}
