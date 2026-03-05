using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models;

public partial class AspNetRole
{
    [Key]
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; } = string.Empty;
    public string? NormalizedName { get; set; } = string.Empty;

   
}
