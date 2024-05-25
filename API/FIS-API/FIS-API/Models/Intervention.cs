using System;
using System.Collections.Generic;

namespace FIS_API.Models;

public partial class Intervention
{
    public int IdInt { get; set; }

    public string Location { get; set; } = null!;

    public Guid CmdrId { get; set; }

    public int TypeId { get; set; }

    public bool Active { get; set; }

    public virtual Firefighter Cmdr { get; set; } = null!;

    public virtual ICollection<FirefighterIntervention> FirefighterInterventions { get; set; } = new List<FirefighterIntervention>();

    public virtual InterventionType Type { get; set; } = null!;
}
