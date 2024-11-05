using System.Globalization;
using Chirp.Core;

namespace Chirp.Infrastructure;

public record CheepViewModel(string Author, string Message, string TimeStamp);

public interface ICheepService
{
    List<CheepViewModel> GetCheeps(int page);
    List<CheepViewModel> GetCheepsFromAuthor(int page, string author);
    Author GetAuthorByName(string name);
    Author GetAuthorByEmail(string email);
    void AddCheep(Author author, string message);
    void AddAuthor(Author author);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;
    
    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public List<CheepViewModel> GetCheeps(int page)
    {
        List<CheepDTO> cheepDTOs = _repository.GetCheepDTO(page).Result;
        List<CheepViewModel> result = cheepDTOs.ConvertAll(cheep => new CheepViewModel(cheep.Author, cheep.Message, TimestampToCEST(cheep.Timestamp)));
        return result;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(int page, string author)
    {
        List<CheepDTO> cheepDTOs = _repository.GetCheepDTOFromAuthor(page, author).Result;
        List<CheepViewModel> result = cheepDTOs.ConvertAll(cheep => new CheepViewModel(cheep.Author, cheep.Message, TimestampToCEST(cheep.Timestamp)));
        return result;
    }

    private static string TimestampToCEST(long timestamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture); // ensures the right format that is required
    }

    public Author GetAuthorByName(string name)
    {
        Author author = _repository.GetAuthorByName(name).Result;
        return author;
    }
    
    public Author GetAuthorByEmail(string email)
    {
        Author author = _repository.GetAuthorByName(email).Result;
        return author;
    }

    public void AddCheep(Author author, string message)
    {
        Cheep cheep = new()
        {
            Author = author,
            Text = message,
            TimeStamp = DateTime.Now
        };
        _repository.AddCheep(cheep);
    }

    public void AddAuthor(Author author)
    {
        _repository.AddAuthor(author);
    }
}