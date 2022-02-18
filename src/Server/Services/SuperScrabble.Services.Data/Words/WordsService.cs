namespace SuperScrabble.Services.Data.Words;

public class WordsService : IWordsService
{
    private readonly IRepository<Word> wordsRepository;

    public WordsService(IRepository<Word> wordsRepository)
    {
        this.wordsRepository = wordsRepository;
    }

    public bool IsWordValid(string? word)
    {
        return this.wordsRepository.All().FirstOrDefault(w => w.Value == word) != null;
    }
}
