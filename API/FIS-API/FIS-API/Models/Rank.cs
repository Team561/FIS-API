using System;
using System.Collections.Generic;

namespace FIS_API.Models;

public partial class Rank
{
    public int IdRank { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Firefighter> Firefighters { get; set; } = new List<Firefighter>();
}
