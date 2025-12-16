using System;
using System.Collections.Generic;

namespace UrlShortenerAPI.Models;

public partial class Click
{
    public long Id { get; set; }

    public long UrlId { get; set; }

    public DateTime ClickedAt { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Referer { get; set; }

    public virtual Url Url { get; set; } = null!;
}
