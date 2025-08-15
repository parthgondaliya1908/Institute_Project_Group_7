namespace Api.Common.Options;

public class OAuthOptions
{
    public const string SectionName = "OAuth";
    
    public string GoogleClientId { get; set; } = null!;
}