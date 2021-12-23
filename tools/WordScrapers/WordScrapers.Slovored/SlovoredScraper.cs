namespace WordScrapers.Slovored
{
    using AngleSharp;

    public class SlovoredScraper
    {
        private readonly string domain = "slovored.com";
        private readonly string path = "sitemap/pravopisen-rechnik/letter";

        private readonly IBrowsingContext browsingContext;
        private readonly IWordChecker wordChecker;

        public SlovoredScraper(IBrowsingContext browsingContext, IWordChecker wordChecker)
        {
            this.browsingContext = browsingContext;
            this.wordChecker = wordChecker;
        }

        public async Task<IEnumerable<string>> ScrapeMainWordFormsAsync()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            const char start = 'а', end = 'я';

            var mainWordForms = new List<string>();

            for (char first = start; first <= end; first++)
            {
                if (first == 'ь' || first == 'ы' || first == 'э')
                {
                    continue;
                }

                for (char second = start; second <= end; second++)
                {
                    string url = $"https://{this.domain}/{this.path}/{first}/{first}{second}";
                    var document = await context.OpenAsync(url);

                    var words = document.QuerySelectorAll("div.words > a")
                        .Select(a => a?.TextContent.Trim()).Where(w => w != null);

                    mainWordForms.AddRange(words);
                }
            }

            return mainWordForms;
        }

        public async Task<IEnumerable<string>> ScrapeWordSubformsAsync(string mainWordForm)
        {
            if (string.IsNullOrEmpty(mainWordForm) || string.IsNullOrWhiteSpace(mainWordForm))
            {
                throw new ArgumentException($"{nameof(mainWordForm)} cannot be null, empty or white space.");
            }

            string url = $"https://{this.domain}/search/pravopisen-rechnik/{mainWordForm}";

            var document = await this.browsingContext.OpenAsync(url);

            var domElement = document.QuerySelector("div.result > table > tbody > tr > td.translation > pre");

            var words = domElement?.TextContent
                                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                                .Skip(1)
                                .Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)?[0] ?? string.Empty)
                                .Where(x => this.wordChecker.IsWordValid(x) && mainWordForm != x)
                                .ToHashSet();

            return words?.ToList() ?? new List<string>();
        }
    }
}
