using System.Globalization;
using CsvHelper;

namespace Chirp.SimpleDB;

/// <summary>
/// CSV file implementation of <c>IDatabaseRepository</c> interface.<br/>
/// Utilizes the CsvHelper library by Josh Close. Web: https://joshclose.github.io/CsvHelper/
/// </summary>
/// <typeparam name="T">the type of record in this database.</typeparam>
public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    readonly string _path;

    public CSVDatabase(string path) {
        _path = path;
    }
    
    public IEnumerable<T> Read(int? limit = null)
    {
        using StreamReader sr = new (_path);
        using CsvReader csv = new (sr, CultureInfo.InvariantCulture);
        IEnumerable<T> records = csv.GetRecords<T>();
        List<T> results = new(records);
        return results;
    }

    public void Store(T record)
    {
        using StreamWriter sw = File.AppendText(_path);
        using CsvWriter csv = new (sw, CultureInfo.InvariantCulture);
        sw.WriteLine();
        csv.WriteRecord(record);
    }
}