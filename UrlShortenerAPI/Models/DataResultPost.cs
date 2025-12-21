using System.ComponentModel.DataAnnotations;

namespace UrlShortenerAPI.Models
{
    public class DataResultPost
    {
        public string? ShortenedUrl { get; set; }
        public string? Message { get; set; }
    }
}
