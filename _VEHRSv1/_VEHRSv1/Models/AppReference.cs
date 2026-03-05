using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class AppReference
{
    public int ReferenceId { get; set; }

    public int ReferenceGroup { get; set; }

    public int ReferenceCode { get; set; }

    public int Sort { get; set; }

    public string ReferenceName { get; set; } = null!;

    public string? ReferenceDescription { get; set; }

    public DateOnly? EffectivityDate { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateOnly CreatedDateTime { get; set; }

    public string? ModifiedBy { get; set; }

    public DateOnly? ModifiedDateTime { get; set; }
}
