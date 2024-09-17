using System.Globalization;
using CsvHelper;

namespace Chirp.SimpleDB;

/// <summary>
/// CSV file implementation of <c>IDatabaseRepository</c> interface.<br/>
/// Utilizes the CsvHelper library by Josh Close. Web: https://joshclose.github.io/CsvHelper/
/// </summary>
/// <typeparam name="T">the type of record in this database.</typeparam>
/// Singleton pattern is based on C# in depth. Web: https://csharpindepth.com/Articles/Singleton

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private static readonly Lazy<CSVDatabase<T>> Lazy;
    private static string _path = null!;
    
    static CSVDatabase()
    {
        Lazy = new Lazy<CSVDatabase<T>>(() => new CSVDatabase<T>(_path));
    }

    public static CSVDatabase<T> Instance(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
        return Lazy.Value;
    }

    private CSVDatabase(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        using StreamReader sr = new StreamReader(_path);
        using CsvReader csv = new CsvReader(sr, CultureInfo.InvariantCulture);
        IEnumerable<T> records = csv.GetRecords<T>();
        if (limit != 0)
        {
            List<T> results = new List<T>();
            IEnumerator<T> iterator = records.GetEnumerator();
            for (int i = 0; i < limit && iterator.MoveNext(); i++)
            {
                results.Add(iterator.Current);
            }
            return results;
        }
        else
        {
            return new List<T>(records);
        }
    }

    public void Store(T record)
    {
        using StreamWriter sw = File.AppendText(_path);
        using CsvWriter csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        sw.WriteLine();
        csv.WriteRecord(record);
    }
}
