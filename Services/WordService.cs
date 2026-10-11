using WordKeeper.Models;
using System.Text.RegularExpressions;
namespace WordKeeper.Services;

public class WordService
{
    private const string StorageKey = "wordkeeper_words";

    private readonly LocalStorageService _localStorageService;

    private List<WordItem> _words = new();

    public WordService(LocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public List<WordItem> GetWords()
    {
        return _words;
    }

    public async Task LoadWordsAsync()
    {
        List<WordItem>? savedWords =
            await _localStorageService.GetItemAsync<List<WordItem>>(StorageKey);

        if (savedWords is not null)
        {
            _words = savedWords;
        }
    }

    public async Task AddWordAsync(
        string english,
        string ukrainian,
        string example)
    {
        var exampleData = BuildExampleData(example, english);

        var word = new WordItem
        {
            Id = _words.Any() ? _words.Max(w => w.Id) + 1 : 1,
            English = english.Trim(),
            Ukrainian = ukrainian.Trim(),
            Example = exampleData.CleanExample,
            GapSentence = exampleData.GapSentence,
            DateCreated = DateTime.Now
        };

        _words.Add(word);

        await SaveWordsAsync();
    }

    public async Task MarkAsKnownAsync(int id)
    {
        WordItem? word = FindWordById(id);

        if (word is null)
        {
            return;
        }

        word.CorrectAnswers++;

        if (word.CorrectAnswers >= 3)
        {
            word.IsLearned = true;
        }

        await SaveWordsAsync();
    }

    public async Task MarkAsUnknownAsync(int id)
    {
        WordItem? word = FindWordById(id);

        if (word is null)
        {
            return;
        }

        word.WrongAnswers++;
        word.IsLearned = false;

        await SaveWordsAsync();
    }

    public async Task DeleteWordAsync(int id)
    {
        WordItem? word = FindWordById(id);

        if (word is null)
        {
            return;
        }

        _words.Remove(word);

        await SaveWordsAsync();
    }

    private WordItem? FindWordById(int id)
    {
        foreach (WordItem currentWord in _words)
        {
            if (currentWord.Id == id)
            {
                return currentWord;
            }
        }

        return null;
    }

    private async Task SaveWordsAsync()
    {
        await _localStorageService.SetItemAsync(StorageKey, _words);
    }
    private static (string CleanExample, string GapSentence) BuildExampleData(
        string example,
        string english)
    {
        if (string.IsNullOrWhiteSpace(example))
        {
            return ("", "");
        }

        example = example.Trim();

        var markedPart = Regex.Match(example, @"\[\[(.+?)\]\]");

        if (markedPart.Success)
        {
            string cleanExample = Regex.Replace(example, @"\[\[(.+?)\]\]", "$1");
            string gapSentence = Regex.Replace(example, @"\[\[(.+?)\]\]", "___");

            return (cleanExample.Trim(), gapSentence.Trim());
        }

        if (!string.IsNullOrWhiteSpace(english))
        {
            string gapSentence = Regex.Replace(
                example,
                Regex.Escape(english.Trim()),
                "___",
                RegexOptions.IgnoreCase
            );

            if (gapSentence != example)
            {
                return (example, gapSentence.Trim());
            }
        }

        return (example, "");
    }
}