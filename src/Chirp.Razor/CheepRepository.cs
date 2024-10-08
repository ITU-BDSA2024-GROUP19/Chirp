namespace Chirp.Razor;

public interface ICheepRepository
{
    Task<List<Cheep>> GetCheeps(int page);
    Task<List<Cheep>> GetCheepsFromAuthor(string authorName, int page);
    Task AddCheep(Cheep cheep);
    Task AddAuthor(Author author);
}

public class CheepRepository : ICheepRepository
{
    public Task<List<Cheep>> GetCheeps(int page)
    {
        throw new NotImplementedException();
    }

    public Task<List<Cheep>> GetCheepsFromAuthor(string authorName, int page)
    {
        throw new NotImplementedException();
    }

    public Task AddCheep(Cheep cheep)
    {
        throw new NotImplementedException();
    }

    public Task AddAuthor(Author author)
    {
        throw new NotImplementedException();
    }
}