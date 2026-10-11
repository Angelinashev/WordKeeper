namespace WordKeeper.Models;

public class WordItem
{
    public int Id { get; set; }
    public string English { get; set; } = "";
    public string Ukrainian { get; set; } = "";
    public string Example { get; set; } = "";
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public bool IsLearned { get; set; }
    public DateTime DateCreated { get; set; } =  DateTime.Now;
    public string GapSentence { get; set; } = "";
}