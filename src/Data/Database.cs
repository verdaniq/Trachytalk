#nullable enable
using LiteDB;
using Trachytalk.Services;

namespace Trachytalk.Data;

public class Database(ILoggingService loggingService)
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
        try
        {
            if (string.IsNullOrEmpty(text))
            {
                return new List<TextEntry>();
            }

            using var db = new LiteDatabase(DatabasePath);

            var collection = db.GetCollection<TextEntry>("TextEntries");

            var results = collection.Query()
                .Where(e => e.Text.StartsWith(text.ToLower()) && !e.IsPhrase)
                .OrderByDescending(e => e.Count)
                .ToList();

            return results;
        }
        catch (Exception e)
        {
            loggingService.LogError(e);
            return new List<TextEntry>();
        }
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

    public TextEntry? GetTopPhrase(string text)
    {
        try
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            using var db = new LiteDatabase(DatabasePath);

            var collection = db.GetCollection<TextEntry>("TextEntries");

            if (collection is null)
            {
                return null;
            }

            var result = collection.Query()
                .Where(e => e.Text.StartsWith(text.ToLower()) && e.IsPhrase)
                .OrderByDescending(e => e.Count)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(result.Text))
            {
                return null;
            }
            
            return result;
        }
        catch (Exception e)
        {
            loggingService.LogMessage("[Database] Error getting top phrase.");
            loggingService.LogError(e);
            return new TextEntry();
        }
    }

    public void AddOrUpdateEntry(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        
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
    