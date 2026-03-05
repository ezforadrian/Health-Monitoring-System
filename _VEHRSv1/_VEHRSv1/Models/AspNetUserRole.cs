using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models;

public partial class AspNetUserRole
{
    [Key]
    public string? UserId { get; set; } = string.Empty;
    public string? RoleId { get; set; } = string.Empty;
}
