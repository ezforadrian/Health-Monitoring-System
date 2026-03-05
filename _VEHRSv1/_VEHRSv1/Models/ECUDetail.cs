using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class ECUDetail
{
    public int EcudetailId { get; set; }

    public int EcuheaderId { get; set; }

    public int TestId { get; set; }

    public bool Result { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public virtual ECUHeader Ecuheader { get; set; } = null!;

    public virtual TestDetail Test { get; set; } = null!;
}
