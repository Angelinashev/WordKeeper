using WordKeeper.Models;

namespace WordKeeper.Services;

public class PracticeResultService
{
    private const string StorageKey = "practice_results";

    private readonly LocalStorageService _localStorageService;

    private List<PracticeResult> _results = new();

    private bool _isLoaded;

    public PracticeResultService(LocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task LoadResultsAsync()
    {
        List<PracticeResult>? savedResults =
            await _localStorageService.GetItemAsync<List<PracticeResult>>(StorageKey);

        _results = savedResults ?? new List<PracticeResult>();

        _isLoaded = true;
    }

    public List<PracticeResult> GetResults()
    {
        return _results
            .OrderByDescending(result => result.AnsweredAt)
            .ToList();
    }

    public List<PracticeResult> GetLastResults(int count)
    {
        return _results
            .OrderByDescending(result => result.AnsweredAt)
            .Take(count)
            .ToList();
    }

    public async Task AddResultAsync(PracticeResult result)
    {
        if (!_isLoaded)
        {
            await LoadResultsAsync();
        }

        if (result.AnsweredAt == default)
        {
            result.AnsweredAt = DateTime.Now;
        }

        _results.Add(result);

        await SaveResultsAsync();
    }

    public int GetTotalAnswersCount()
    {
        return _results.Count;
    }

    public int GetCorrectAnswersCount()
    {
        return _results.Count(result => result.IsCorrect);
    }

    public int GetWrongAnswersCount()
    {
        return _results.Count(result => !result.IsCorrect);
    }

    public int GetAccuracyPercent()
    {
        if (_results.Count == 0)
        {
            return 0;
        }

        int correctAnswers = GetCorrectAnswersCount();

        return (int)Math.Round((double)correctAnswers / _results.Count * 100);
    }

    public async Task ClearResultsAsync()
    {
        if (!_isLoaded)
        {
            await LoadResultsAsync();
        }

        _results.Clear();

        await SaveResultsAsync();
    }

    private async Task SaveResultsAsync()
    {
        await _localStorageService.SetItemAsync(StorageKey, _results);
    }
}