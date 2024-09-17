namespace Chirp.SimpleDB;

public interface IDatabaseRepository<T>
{
    /// <summary>
    /// Returns an enumerator over records on the database. 
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    public IEnumerable<T> Read(int? limit = null);

    /// <summary>
    /// Stores a record on the database. 
    /// </summary>
    /// <param name="record"></param>
    public void Store(T record);
}
