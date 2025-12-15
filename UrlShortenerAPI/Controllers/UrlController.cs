using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {


        [HttpGet(Name = "GetURL")]
        public RedirectResult Get()
        {
            return Redirect("http://www.google.com");
        }



        [HttpPost(Name = "ShortenURL")]
        public string Post(string LongUrl)
        {
            string shortenedUrl = "Mydomain/" + LongUrl.GetHashCode();
            return shortenedUrl;
        }
    }
}
