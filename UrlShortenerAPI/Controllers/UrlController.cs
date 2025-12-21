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

        public UrlController(UrlshortenerContext _dbContext)
        {
            dbContext = _dbContext;
        }


        [HttpGet(Name = "GetURL")]
        [Route("{UrlCode}")]
        public ObjectResult Get(DataRequestGet request)
        {
            DataResultGet result = new();

            Url? url = dbContext.Urls.Where(u => u.UrlCode == request.Url).FirstOrDefault();
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
                    result.LongUrl = url.OriginalUrl;
                    return StatusCode(StatusCodes.Status200OK, result);
                }
                else
                {
                    result.Message = "This URL has expired or is inactive.";
                    return StatusCode(StatusCodes.Status410Gone,result);
                }
            }
            else
            {
                result.Message = "URL not found.";
                return StatusCode(StatusCodes.Status404NotFound, result);
            }
        }

        [HttpPost(Name = "ShortenURL")]
        public ObjectResult Post([FromBody] DataRequestPost request)
        {
            DataResultPost result = new();


            try
            {
                if (dbContext.Urls.Where(u => u.OriginalUrl == request.Url).FirstOrDefault() == null)
                {
                    Url newUrl = new()
                    {
                        OriginalUrl = request.Url,
                        UrlCode = CreateUrlCode(),
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(365),
                        IsActive = true,
                        ClickCount = 0
                    };

                    result.ShortenedUrl = newUrl.UrlCode;
                    dbContext.Urls.AddAsync(newUrl);
                    dbContext.SaveChangesAsync();

                }

                else
                {
                    Url existingUrl = dbContext.Urls.Where(u => u.OriginalUrl == request.Url).First();
                    result.ShortenedUrl = existingUrl.UrlCode;
                    result.Message = "URL already exists.";
                }


                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                result.Message = "An error occurred server side, please contact admin";
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
           




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
