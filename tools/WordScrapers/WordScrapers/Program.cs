using AngleSharp;
using WordScrapers.Slovored;

//var mainWordForms = await slovoredScraper.ScrapeMainWordFormsAsync();

var mainWordForms = await File.ReadAllLinesAsync("./Resources/remaining-main-word-forms.txt");
var wordsByStartingLetters = mainWordForms.GroupBy(x => x.First()).ToDictionary(x => x.Key, x => x.ToList());

var tasks = new List<Task>();

int counter = 0;

foreach (var wordsByStartingLetter in wordsByStartingLetters)
{
    var task = new Task(async () =>
    {
        var config = Configuration.Default.WithDefaultLoader();
        var browsingContext = BrowsingContext.New(config);

        var wordChecker = new BulgarianWordChecker();
        var slovoredScraper = new SlovoredScraper(browsingContext, wordChecker);

        var list = new List<string>();

        foreach (string word in wordsByStartingLetter.Value)
        {
            var subforms = await slovoredScraper.ScrapeWordSubformsAsync(word);
            list.AddRange(subforms);

            if (list.Count >= 100)
            {
                counter += list.Count;
                global::System.Console.WriteLine(counter);
                await File.AppendAllLinesAsync($"./Resources/words-with-{wordsByStartingLetter.Key}.txt", list);
                list.Clear();
            }
        }
    });

    tasks.Add(task);
    task.Start();
}

while (tasks.Any())
{
    var completedTask = await Task.WhenAny(tasks);
    tasks.Remove(completedTask);
}

Console.ReadLine();

/*var mainWordForms = (await File.ReadAllLinesAsync("../../../../../../resources/new-words/main-word-forms.txt")).ToHashSet();

var files = Directory.GetFiles("../../../../../../resources/new-words");

Console.WriteLine(mainWordForms.Count);

var allUniqueWords = new HashSet<string>();

foreach (string file in files)
{
    if (file.EndsWith("main-word-forms.txt"))
    {
        continue;
    }

    Console.WriteLine(file);
    Console.WriteLine(mainWordForms.Count);
    var fileLines = (await File.ReadAllLinesAsync(file)).ToHashSet();

    foreach (var line in fileLines)
    {
        allUniqueWords.Add(line);
        mainWordForms.Remove(line);
    }
}
Console.WriteLine(allUniqueWords.Count);
Console.WriteLine(mainWordForms.Count);
await File.WriteAllLinesAsync("./Resources/Output/remaining-main-word-forms.txt", mainWordForms);*/