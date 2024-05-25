using System;
using System.Collections.Generic;

namespace FIS_API.Models;

public partial class FirefighterIntervention
{
    public int Id { get; set; }

    public int IntId { get; set; }

    public Guid FfId { get; set; }

    public virtual Firefighter Ff { get; set; } = null!;

    public virtual Intervention Int { get; set; } = null!;
}
