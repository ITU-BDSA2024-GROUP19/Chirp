using System.Globalization;
using Chirp.Core;

namespace Chirp.Infrastructure.Cheeps;

public interface ICheepService
{
    List<CheepDto> GetCheeps(int page, string userName);
    List<CheepDto> GetCheepsFromAuthor(int page, string author, string userName);
    List<CheepDto> GetCheepsFromMe(int page, string userName);
    List<CheepDto> GetAllCheepsFromAuthor(string author, string userName);
    Author GetAuthorByName(string name);
    Author GetAuthorByEmail(string email);
    void AddCheep(Author author, string message);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;
    
    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public List<CheepDto> GetCheeps(int page, string userName)
    {
        return _repository.GetCheepDTO(page, userName).Result;
    }

    public List<CheepDto> GetCheepsFromAuthor(int page, string author, string userName)
    {
        return _repository.GetCheepDTOFromAuthor(page, author, userName).Result;
    }
    
    public List<CheepDto> GetCheepsFromMe(int page, string userName)
    {
        return _repository.GetCheepDTOFromMe(page, userName).Result;
    }
    
    public List<CheepDto> GetAllCheepsFromAuthor(string author, string userName)
    {
        return _repository.GetAllCheepDTOFromAuthor(author, userName).Result;
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
}