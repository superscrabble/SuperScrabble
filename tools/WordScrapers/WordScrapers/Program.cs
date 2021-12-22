using AngleSharp;

/*const string domain = "slovored.com";
const string path = "sitemap/pravopisen-rechnik/letter";

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
        string url = $"https://{domain}/{path}/{first}/{first}{second}";
        var document = await context.OpenAsync(url);

        var words = document.QuerySelectorAll("div.words > a")
            .Select(a => a?.TextContent.Trim()).Where(w => w != null);

        mainWordForms.AddRange(words);
    }

    await File.AppendAllLinesAsync("./Resources/main-word-forms.txt", mainWordForms);
    mainWordForms.Clear();
    Console.WriteLine(first);
}
*/
var lines = await File.ReadAllLinesAsync("./Resources/main-word-forms.txt");
Console.WriteLine(lines.Length);
