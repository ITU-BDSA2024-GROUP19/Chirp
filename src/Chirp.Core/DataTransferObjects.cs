namespace Chirp.Core;

public record CheepDTO(string Author, string Message, long Timestamp, bool is_followed);