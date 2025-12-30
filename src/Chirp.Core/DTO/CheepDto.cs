namespace Chirp.Core.DTO;

public record CheepDto(int CheepId, string Author, string Message, string Timestamp, int LikeCount, bool HasLiked);
