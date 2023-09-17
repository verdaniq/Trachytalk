using LiteDB;

namespace Trachytalk.Data;

public class Database
{
    private const string DatabaseFilename = "Trachytalk.db";
    
    private static string DatabasePath =>
    Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    
    public IEnumerable<TextEntry> GetPhrases()
    {
        using var db = new LiteDatabase(DatabasePath);

        var collection = db.GetCollection<TextEntry>("TextEntries");

        return collection.FindAll();
    }

    public IEnumerable<TextEntry> GetMatchingWords(string text)
    {
        using var db = new LiteDatabase(DatabasePath);

        var collection = db.GetCollection<TextEntry>("TextEntries");

        var results = collection.Query()
            .Where(e => e.Text.StartsWith(text.ToLower()) && !e.IsPhrase)
            .OrderByDescending(e => e.Count)
            .ToList();

        return results;
    }

    public IEnumerable<TextEntry> GetMatchingPhrases(string text)
    {
        using var db = new LiteDatabase(DatabasePath);

        var collection = db.GetCollection<TextEntry>("TextEntries");

        var results = collection.Query()
            .Where(e => e.Text.StartsWith(text.ToLower()) && e.IsPhrase)
            .OrderByDescending(e => e.Count)
            .ToList();

        return results;
    }

    public void AddOrUpdateEntry(string text)
    {
        var isPhrase = text.Contains(" ");

        using var db = new LiteDatabase(DatabasePath);
        
        var collection = db.GetCollection<TextEntry>("TextEntries");
        
        var entry = collection.Query()
            .Where(e => e.Text == text.ToLower())
            .FirstOrDefault();

        if (entry is null)
        {
            entry = new TextEntry
            {
                Text = text.ToLower(),
                IsPhrase = isPhrase,
                Count = 1
            };
        }
        else
        {
            entry.Count++;
        }

        collection.Upsert(entry);
    }
}
    