using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortenerAPI.Models;

namespace UrlShortener.Server.Controllers
{
    [ApiController] 
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly UrlshortenerContext dbContext;

        public UrlController (UrlshortenerContext _dbContext)
        {
            dbContext = _dbContext;
        }


        [HttpGet(Name = "GetURL")]
        [Route("{UrlCode}")]
        public ObjectResult Get(string UrlCode)
        {
            Url? url = dbContext.Urls.Where(u => u.UrlCode == UrlCode).FirstOrDefault();
            if (url != null)
            {
                if (url.IsActive)
                {
                    // Update the ClickCount
                    url.ClickCount += 1;
                    dbContext.Urls.Update(url);
                    dbContext.Clicks.AddAsync(new Click
                    {
                        UrlId = url.Id,
                        ClickedAt = DateTime.UtcNow,
                        UserAgent = Request.Headers["User-Agent"].ToString(),
                        Referer = Request.Headers["Referer"].ToString(),
                        IpAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                    });

                    dbContext.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status200OK, url.OriginalUrl);
                }
                else
                {
                    return StatusCode(StatusCodes.Status410Gone, "This URL has expired or is inactive.");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, "URL not found.");
            }
        }

        [HttpPost(Name = "ShortenURL")]
        public ObjectResult Post(string LongUrl)
        {
            string UrlShortCode = "";

            if (dbContext.Urls.Where(u => u.OriginalUrl == LongUrl).FirstOrDefault() == null)
            {
                Url newUrl = new()
                {
                    OriginalUrl = LongUrl,
                    UrlCode = CreateUrlCode(),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(365),
                    IsActive = true,
                    ClickCount = 0
                };

                UrlShortCode = newUrl.UrlCode;
                dbContext.Urls.AddAsync(newUrl);
                dbContext.SaveChangesAsync();

            }
           
            else
            {
                Url existingUrl = dbContext.Urls.Where(u => u.OriginalUrl == LongUrl).First();
                UrlShortCode = existingUrl.UrlCode;
            }

           
            return StatusCode(StatusCodes.Status200OK, UrlShortCode);




        }

        private string CreateUrlCode()
        {   
            bool unique= false;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string urlCode = String.Empty;
            while (!unique)
            {
                urlCode = new string(Enumerable.Range(0, 6)
                    .Select(_ => chars[new Random().Next(chars.Length)])
                    .ToArray());

                if (dbContext.Urls.Where(u => u.UrlCode == urlCode).FirstOrDefault() == null)
                {
                    unique = true;    
                }
            }
            return urlCode;

        }
    }
}
