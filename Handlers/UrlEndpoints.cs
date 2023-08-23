using Microsoft.EntityFrameworkCore;
using shortid;
using shortid.Configuration;
using urlshortener.Dto;
using urlshortener.Models;

namespace urlshortener.Handlers;

public static class UrlEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/urls", async (UrlDto url, ApiDbContext db, HttpContext ctx) => {
            if (!Uri.TryCreate(url.Url, UriKind.Absolute, out var inputUrl))
                return Results.BadRequest($"Invalid URL: {url.Url}");

            var options = new GenerationOptions(useNumbers: true, useSpecialCharacters: false, length: 9);
            var newUrls = new Urls()
            {
                Url = url.Url,
                ShortUrl = ShortId.Generate(options)
            };

            db.Urls.Add(newUrls);
            await db.SaveChangesAsync();

            var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/r/{newUrls.ShortUrl}";
            return Results.Ok(new UrlResponseDto(){
                Url = result,
            });
        });

        // Redirect url when you have r=true and default is to return UrlResponseDto
        app.MapGet("/urls/{shortUrl}", async (ApiDbContext db, HttpContext ctx, string shortUrl, string? r) => {
            var urlMatch = await db.Urls.FirstOrDefaultAsync(x => x.ShortUrl.Trim() == shortUrl.Trim());
            if (urlMatch == null)
                return Results.BadRequest($"Invalid url: {shortUrl}");

            var urls = new UrlResponseDto() {
                Url = urlMatch.Url,
            };

            return (r is string outputString && outputString.Contains("true")) ? 
                Results.Redirect(urls.Url) : Results.Ok(urls);
        });
    }
}
