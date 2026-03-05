using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class Pemestatus
{
    public int StatusId { get; set; }

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public virtual ICollection<Pemeheader> Pemeheaders { get; set; } = new List<Pemeheader>();
}
