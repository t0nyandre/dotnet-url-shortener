namespace urlshortener.Models;

public class Urls
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string ShortUrl { get; set; } = "";
}