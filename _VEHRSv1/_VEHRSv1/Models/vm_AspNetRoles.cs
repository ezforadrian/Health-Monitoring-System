using Microsoft.AspNetCore.Identity;

namespace _VEHRSv1.Models
{
    public partial class vm_AspNetRoles
    {
        public string? Id { get; set; }  = Guid.NewGuid().ToString();
        public string? Name { get; set; } = string.Empty;
        
    }
}
