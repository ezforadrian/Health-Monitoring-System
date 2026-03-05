using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models
{
    public class vm_LoginView
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
