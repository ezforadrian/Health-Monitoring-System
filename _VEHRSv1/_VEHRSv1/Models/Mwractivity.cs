using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class Mwractivity
{
    public int MwractId { get; set; }

    public int MwrlistId { get; set; }

    public DateTime ActDate { get; set; }

    public string Idnumber { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public virtual Mwrlist Mwrlist { get; set; } = null!;
}
