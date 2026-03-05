using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models;

public partial class Mwrdate
{

    public int MwrdateId { get; set; }
    
    public int MwrlistId { get; set; }

    public DateTime MwractDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }
}
