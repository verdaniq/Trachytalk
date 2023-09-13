namespace Trachytalk.Models;

public class Word
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Text { get; set; }

    public Word(string word)
    {
        this.Text = word;
    }

    public Word()
    {
        
    }
}