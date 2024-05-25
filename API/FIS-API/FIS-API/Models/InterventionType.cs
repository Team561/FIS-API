using System;
using System.Collections.Generic;

namespace FIS_API.Models;

public partial class InterventionType
{
    public int IdType { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
}
