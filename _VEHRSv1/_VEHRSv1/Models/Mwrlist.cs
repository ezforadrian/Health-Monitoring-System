using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class Mwrlist
{
    public int MwrlistId { get; set; }

    public string ActivityName { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public string? ActivityType { get; set; }

    public virtual ICollection<Mwractivity> Mwractivities { get; set; } = new List<Mwractivity>();
}
