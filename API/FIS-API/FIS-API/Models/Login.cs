using System;
using System.Collections.Generic;

namespace FIS_API.Models;

public partial class Login
{
    public string Username { get; set; } = null!;

    public byte[]? Password { get; set; }

    public Guid? UserGuid { get; set; }

    public virtual Firefighter? User { get; set; }
}
