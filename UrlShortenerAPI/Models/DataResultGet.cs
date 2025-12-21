using System.ComponentModel.DataAnnotations;

namespace UrlShortenerAPI.Models
{
    public class DataResultGet
    {
        public string? LongUrl { get; set; }
        public string? Message { get; set; }
    }
}
