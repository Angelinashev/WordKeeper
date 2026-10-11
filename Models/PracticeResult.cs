namespace WordKeeper.Models;

public class PracticeResult
{
    public int WordId { get; set; }
    public string English { get; set; } = "";
    public string Mode { get; set; } = "";
    public string SelectedAnswer { get; set; } = "";
    public string CorrectAnswer { get; set; } = "";
    public bool IsCorrect { get; set; }
    public DateTime AnsweredAt { get; set; } =  DateTime.Now;
}