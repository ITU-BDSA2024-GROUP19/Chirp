using System.Globalization;
using CsvHelper;

namespace Chirp.SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    const string path = "chirp_cli_db.csv"; 
    
    public IEnumerable<T> Read(int? limit = null)
    {
        using StreamReader sr = new (path);
        using CsvReader csv = new (sr, CultureInfo.InvariantCulture);
        IEnumerable<T> records = csv.GetRecords<T>();
        return records;
    }

    public void Store(T record)
    {
        
    }
}