using System;
using System.Collections.Generic;

namespace _VEHRSv1.Models;

public partial class Pemeheader
{
    public int Pemeid { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string PositionRef { get; set; } = null!;

    public int StatusId { get; set; }

    public string? Idnumber { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public virtual ICollection<Pemedetail> Pemedetails { get; set; } = new List<Pemedetail>();

    public virtual Pemestatus Status { get; set; } = null!;
}
