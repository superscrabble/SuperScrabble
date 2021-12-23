using AngleSharp;
using WordScrapers.Slovored;

//var mainWordForms = await slovoredScraper.ScrapeMainWordFormsAsync();

/*var mainWordForms = await File.ReadAllLinesAsync("./Resources/remaining-main-word-forms.txt");
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

var mainWordForms = (await File.ReadAllLinesAsync("../../../../../../resources/new-words/main-word-forms.txt")).ToHashSet();

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

var first = await GetAllUniqueWordsAsync("../../../../../../resources/final-list/all");

//var second = await GetAllUniqueWordsAsync("../../../../../../resources/words/");

Console.WriteLine("All first: " + first.Distinct().Count());
/*Console.WriteLine("All second: " + second.Count());

var uniqueFirst = first.Distinct().ToHashSet();
var uniqueSecond = second.Distinct().ToHashSet();

Console.WriteLine("Unique first: " + uniqueFirst.Count());
Console.WriteLine("Unique second: " + uniqueSecond.Count());

uniqueFirst.UnionWith(uniqueSecond);

Console.WriteLine("All unique words: " + uniqueFirst.Count);

var groups = uniqueFirst.GroupBy(x => x.First()).ToDictionary(x => x.Key, x => x.ToList());
Console.WriteLine(groups.Count);

int sum = 0;

foreach (var group in groups)
{
    Console.WriteLine(group.Key + " -> " + group.Value.Count);

    await File.WriteAllLinesAsync($"./res/total-of-{group.Value.Count}-words-with-{group.Key}.txt", group.Value);

    sum += group.Value.Count;
}

Console.WriteLine("Sum " + sum);

/*var finalList = await GetAllUniqueWordsAsync("../../../../../../resources/final-list", "main-word-forms.txt", "remaining-main-word-forms.txt");

newWords.UnionWith(finalList);

var uniqueWords = newWords.GroupBy(x => x.First()).ToDictionary(x => x.Key, x => x.ToList());
int total = 0;

foreach (var item in uniqueWords)
{
    await File.WriteAllLinesAsync($"./res/words-with-{item.Key}.txt", item.Value);
    total += item.Value.Count;
}

Console.WriteLine("total: " + total);*/

static async Task<IEnumerable<string>> GetAllUniqueWordsAsync(string directory)
{
    var uniqueWords = new List<string>();
    string[] files = Directory.GetFiles(directory);

    foreach (string file in files)
    {
        string[] words = await File.ReadAllLinesAsync(file);
        Console.WriteLine(words.Length);
        foreach (string word in words)
        {
            uniqueWords.Add(word);
        }
    }

    return uniqueWords;
}