namespace Trachytalk.Models;

public class Word
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Text { get; set; }

    public bool IsCurrentWord { get; set; } = false;

    public Word(string word, bool currentWord = false)
    {
        this.Text = word;
        this.IsCurrentWord = currentWord;
    }

    public Word()
    {
        
    }
}