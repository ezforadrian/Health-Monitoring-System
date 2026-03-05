using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class Pemedetail
{
    public int DetailId { get; set; }

    public int Pemeid { get; set; }

    public DateTime ExamDate { get; set; }

    public int? MedicalEvaluatorId { get; set; }

    public string? Remarks { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public virtual Pemeheader Peme { get; set; } = null!;
}
