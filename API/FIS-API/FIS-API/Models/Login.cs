using System;
using System.Collections.Generic;

namespace FIS_API.Models;

public partial class Login
{
    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public Guid UserGuid { get; set; }

    public virtual Firefighter User { get; set; } = null!;
}
