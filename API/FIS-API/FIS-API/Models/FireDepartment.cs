using System;
using System.Collections.Generic;

namespace FIS_API.Models;

public partial class FireDepartment
{
    public Guid IdFd { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public Guid? CmdrId { get; set; }

    public bool Active { get; set; }

    public virtual Firefighter? Cmdr { get; set; }

    public virtual ICollection<Firefighter> Firefighters { get; set; } = new List<Firefighter>();
}
