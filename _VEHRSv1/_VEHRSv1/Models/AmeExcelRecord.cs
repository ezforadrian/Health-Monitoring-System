using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class AmeExcelRecord
{
    public int Id { get; set; }

    public string EmployeeId { get; set; } = null!;

    public string? Branch { get; set; }

    public string? PatientInfo { get; set; }

    public string? BirthMonth { get; set; }

    public string AmeDate { get; set; }

    public string AmeMonth { get; set; } = null!;

    public string AmeQuarter { get; set; } = null!;

    public string? Request { get; set; }

    public string? Soa { get; set; }

    public string? Name { get; set; }

    public string? Position { get; set; }

    public string? Age { get; set; }

    public string? AgeGroup { get; set; }

    public string? Sex { get; set; }

    public bool? Hpn { get; set; }

    public bool? DmIi { get; set; }

    public bool? Dyslipidemia { get; set; }

    public bool? Hyperuricemia { get; set; }

    public bool? Obese { get; set; }

    public bool? NormalFinding { get; set; }

    public byte? TotalFinding { get; set; }
}
