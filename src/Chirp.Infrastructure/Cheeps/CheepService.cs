using System.Globalization;

using Chirp.Core;
using Chirp.Infrastructure.Authors;


namespace Chirp.Infrastructure.Cheeps;

/// <summary>
/// Defines services related to the Cheep entity on <i>Chirp!</i>
/// </summary>
public interface ICheepService
{
    List<CheepDto> GetCheeps(int page, string userName);
    List<CheepDto> GetCheepsFromAuthor(int page, string author, string userName);
    List<CheepDto> GetCheepsFromAuthorLikes(int page, string userName);
    List<CheepDto> GetCheepsFromMe(int page, string userName);
    List<CheepDto> GetAllCheepsFromAuthor(string author, string userName);
    Task LikeCheep(int cheepId, string authorName);
    Task RemoveLikeCheep(int cheepId, string authorName);
    void AddCheep(Author author, string message);
}

/// <summary>
/// Implements services related to the Cheep entity on <i>Chirp!</i>
/// </summary>
public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public CheepService(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public List<CheepDto> GetCheeps(int page, string userName)
    {
        return _cheepRepository.GetCheepDTO(page, userName).Result;
    }

    public List<CheepDto> GetCheepsFromAuthor(int page, string author, string userName)
    {
        return _cheepRepository.GetCheepDTOFromAuthor(page, author, userName).Result;
    }

    public List<CheepDto> GetCheepsFromAuthorLikes(int page, string userName)
    {
        return _cheepRepository.GetCheepDTOFromAuthorLikes(page, userName).Result;
    }
    
    public List<CheepDto> GetCheepsFromMe(int page, string userName)
    {
        return _cheepRepository.GetCheepDTOFromMe(page, userName).Result;
    }

    public List<CheepDto> GetAllCheepsFromAuthor(string author, string userName)
    {
        return _cheepRepository.GetAllCheepDTOFromAuthor(author, userName).Result;
    }

    public async Task LikeCheep(int cheepId, string authorName)
    {
        var cheep = await _cheepRepository.GetCheepWithLikes(cheepId);
        var author = await _authorRepository.GetAuthorByUsernameAsync(authorName);
        if (cheep == null || author == null)
        {
            throw new ArgumentException("Cheep or author does not exist.");
        }
        cheep.Likes.Add(author);
        await _cheepRepository.UpdateCheep(cheep);
    }

    public async Task RemoveLikeCheep(int cheepId, string authorName)
    {
        var cheep = await _cheepRepository.GetCheepWithLikes(cheepId);
        var author = await _authorRepository.GetAuthorByUsernameAsync(authorName);
        if (cheep == null || author == null)
        {
            throw new ArgumentException("Cheep or author does not exist.");
        }
        cheep.Likes.Remove(author);
        await _cheepRepository.UpdateCheep(cheep);
    }

    private static string TimestampToCEST(long timestamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture); // ensures the right format that is required
    }

    public void AddCheep(Author author, string message)
    {
        Cheep cheep = new()
        {
            Author = author,
            Text = message,
            TimeStamp = DateTime.Now,
            Likes = []
        };
        _cheepRepository.AddCheep(cheep);
    }
}