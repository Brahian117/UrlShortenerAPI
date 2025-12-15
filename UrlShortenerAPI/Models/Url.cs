using System;
using System.Collections.Generic;

namespace UrlShortenerAPI.Models;

public partial class Url
{
    public long Id { get; set; }

    public string OriginalUrl { get; set; } = null!;

    public string UrlCode { get; set; } = null!;

    public long? UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool IsActive { get; set; }

    public long ClickCount { get; set; }

    public virtual ICollection<Click> Clicks { get; set; } = new List<Click>();

    public virtual User? User { get; set; }
}
