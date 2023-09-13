using SQLite;

namespace Trachytalk.Data;

public class TextEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Text { get; set; }

    public bool IsPhrase { get; set; }

    public int Count { get; set; }
}
