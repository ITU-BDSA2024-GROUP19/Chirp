using System.Globalization;
using Chirp.Core;

namespace Chirp.Infrastructure;

public record CheepViewModel(string Author, string Message, string TimeStamp, bool IsFollowed);

public interface ICheepService
{
    List<CheepViewModel> GetCheeps(int page, string userName);
    List<CheepViewModel> GetCheepsFromAuthor(int page, string author, string userName);
    List<CheepViewModel> GetCheepsFromMe(int page, string userName);
    List<CheepViewModel> GetAllCheepsFromAuthor(string author, string userName);
    Author GetAuthorByName(string name);
    Author GetAuthorByEmail(string email);
    void AddCheep(Author author, string message);
    void FollowAuthor(string followerName, string authorName);
    void UnfollowAuthor(string followerName, string authorName);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;
    
    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public List<CheepViewModel> GetCheeps(int page, string userName)
    {
        List<CheepDTO> cheepDTOs = _repository.GetCheepDTO(page, userName).Result;
        List<CheepViewModel> result = cheepDTOs.ConvertAll(cheep => new CheepViewModel(cheep.Author, cheep.Message, TimestampToCEST(cheep.Timestamp),cheep.IsFollowed));
        return result;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(int page, string author, string userName)
    {
        List<CheepDTO> cheepDTOs = _repository.GetCheepDTOFromAuthor(page, author, userName).Result;
        List<CheepViewModel> result = cheepDTOs.ConvertAll(cheep => new CheepViewModel(cheep.Author, cheep.Message, TimestampToCEST(cheep.Timestamp),cheep.IsFollowed));
        return result;
    }
    
    public List<CheepViewModel> GetCheepsFromMe(int page, string userName)
    {
        List<CheepDTO> cheepDTOs = _repository.GetCheepDTOFromMe(page, userName).Result;
        List<CheepViewModel> result = cheepDTOs.ConvertAll(cheep => new CheepViewModel(cheep.Author, cheep.Message, TimestampToCEST(cheep.Timestamp),cheep.IsFollowed));
        return result;
    }
    
    public List<CheepViewModel> GetAllCheepsFromAuthor(string author, string userName)
    {
        List<CheepDTO> cheepDTOs = _repository.GetAllCheepDTOFromAuthor(author, userName).Result;
        List<CheepViewModel> result = cheepDTOs.ConvertAll(cheep => new CheepViewModel(cheep.Author, cheep.Message, TimestampToCEST(cheep.Timestamp), cheep.IsFollowed));
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
    
    public void FollowAuthor(string followerName, string authorName)
    {
        _repository.FollowAuthor(followerName, authorName);
    }
    public void UnfollowAuthor(string followerName, string authorName)
    {
        _repository.UnfollowAuthor(followerName, authorName);
    }
}