using Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    public List<CheepViewModel> GetCheeps(int page)
    {
        return DBFacade.Read(page);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return DBFacade.UserRead(author);
    }
}
