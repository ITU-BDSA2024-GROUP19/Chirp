using System.Globalization;
using CsvHelper;

namespace Chirp.SimpleDB;

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
        
    }
}