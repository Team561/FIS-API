using System;
using System.Collections.Generic;

namespace FIS_API.Models;

public partial class Firefighter
{
    public Guid IdFf { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime? ActiveDate { get; set; }

    public int RankId { get; set; }

    public Guid FdId { get; set; }

    public bool Active { get; set; }

    public virtual FireDepartment Fd { get; set; } = null!;

    public virtual ICollection<FireDepartment> FireDepartments { get; set; } = new List<FireDepartment>();

    public virtual ICollection<FirefighterIntervention> FirefighterInterventions { get; set; } = new List<FirefighterIntervention>();

    public virtual ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();

    public virtual Rank Rank { get; set; } = null!;
}
