using System.Globalization;
using Chirp.Core;

namespace Chirp.Infrastructure;

public record CheepViewModel(string Author, string Message, string TimeStamp);

public interface ICheepService
{
    List<CheepViewModel> GetCheeps(int page);
    List<CheepViewModel> GetCheepsFromAuthor(int page, string author);
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
}