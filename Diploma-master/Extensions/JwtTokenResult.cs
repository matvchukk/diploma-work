namespace Diploma.Extensions;

public class JwtTokenResult
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
