using System;
using System.Collections.Generic;

namespace UrlShortenerAPI.Models;

public partial class User
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int UserTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Url> Urls { get; set; } = new List<Url>();

    public virtual UserType UserType { get; set; } = null!;
}
