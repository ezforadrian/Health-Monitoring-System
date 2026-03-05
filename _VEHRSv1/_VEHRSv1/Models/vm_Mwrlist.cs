using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class vm_Mwrlist
{
    public int MwrlistId { get; set; }

    public string ActivityName { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public int ReferenceCode { get; set; }
    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }
    public string ActivityType { get; set; } = null!;
}
