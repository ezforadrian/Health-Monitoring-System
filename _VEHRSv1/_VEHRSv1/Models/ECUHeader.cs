using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class ECUHeader
{
    public int EcuheaderId { get; set; }

    public DateOnly Ecudate { get; set; }

    public DateOnly? RunDate { get; set; }

    public string Idnumber { get; set; } = null!;

    public string Branch { get; set; } = null!;
    public string? Remarks { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public virtual ICollection<ECUDetail> Ecudetails { get; set; } = new List<ECUDetail>();
}
