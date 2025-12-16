using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortenerAPI.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;

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
        public RedirectResult Get(string UrlCode)
        {
            Url? url=dbContext.Urls.Where(u => u.UrlCode == UrlCode).FirstOrDefault();
            if (url != null && url.IsActive)
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

                return Redirect(url.OriginalUrl);
            }
            else
                return Redirect("http://www.google.com"); // OJO Modify to redirect to a custom 404 page
        }

        [HttpPost(Name = "ShortenURL")]
        public ObjectResult Post(string LongUrl)
        {
            string shortenedUrl = "Mydomain/";

            if (dbContext.Urls.Where(u => u.OriginalUrl == LongUrl).FirstOrDefault() == null)
            {
                Url newUrl = new Url
                {
                    OriginalUrl = LongUrl,
                    UrlCode = CreateUrlCode(),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(365),
                    IsActive = true,
                    ClickCount = 0
                };

                shortenedUrl += newUrl.UrlCode;
                dbContext.Urls.AddAsync(newUrl);
                dbContext.SaveChangesAsync();

            }
           
            else
            {
                Url existingUrl = dbContext.Urls.Where(u => u.OriginalUrl == LongUrl).First();   
                shortenedUrl += existingUrl.UrlCode;
            }

           
            return StatusCode(StatusCodes.Status200OK, shortenedUrl);




        }

        public string CreateUrlCode()
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
