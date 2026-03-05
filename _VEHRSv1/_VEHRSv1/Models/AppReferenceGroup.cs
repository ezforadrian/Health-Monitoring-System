using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class AppReferenceGroup
{
    public int ReferenceGroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public string GroupDescription { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }
}
