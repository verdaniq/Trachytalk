using SQLite;

namespace Trachytalk.Data;

public class Database
{
    private const string DatabaseFilename = "Trachytalk.db3";

    private const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    private static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    private SQLiteAsyncConnection _database;

    private bool IsInitialized { get; set; }

    public async Task Initialize()
    {
        if (IsInitialized || _database is not null)
        {
            return;
        }

        SQLitePCL.Batteries_V2.Init();

        _database = new SQLiteAsyncConnection(DatabasePath, Flags);
        
        var result = await _database.CreateTableAsync<TextEntry>();
    }

    public async Task<IEnumerable<TextEntry>> GetPhrases()
    {
        await Initialize();

        return await _database.Table<TextEntry>().ToListAsync();
    }

    public async Task<IEnumerable<TextEntry>> GetMatchingWords(string text)
    {
        await Initialize();

        return await _database.Table<TextEntry>()
            .Where(p => p.Text.StartsWith(text) && !p.IsPhrase)
            .OrderByDescending(p => p.Count)
            .ToListAsync();
    }

    public async Task<IEnumerable<TextEntry>> GetMatchingPhrases(string text)
    {
        await Initialize();

        return await _database.Table<TextEntry>()
            .Where(p => p.Text.StartsWith(text) && p.IsPhrase == true)
            .OrderByDescending(p => p.Count)
            .ToListAsync();
    }

    public async Task AddOrUpdateEntry(string text)
    {
        await Initialize();
        
        var isPhrase = text.Contains(" ");

        var entry = await _database.Table<TextEntry>()
            .FirstOrDefaultAsync(p => p.Text == text);

        if (entry is null)
        {
            entry = new TextEntry
            {
                Text = text,
                IsPhrase = isPhrase,
                Count = 1
            };

            await _database.InsertAsync(entry);
        }
        else
        {
            entry.Count++;
            await _database.UpdateAsync(entry);
        }
    }
}
    