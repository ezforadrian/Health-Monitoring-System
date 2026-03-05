using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class TestDetail
{
    public int TestId { get; set; }

    public string TestName { get; set; } = null!;

    public string TestCategory { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public virtual ICollection<Amedetail> Amedetails { get; set; } = new List<Amedetail>();

    public virtual ICollection<ECUDetail> Ecudetails { get; set; } = new List<ECUDetail>();
}
