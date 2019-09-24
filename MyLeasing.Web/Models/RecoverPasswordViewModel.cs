using System.ComponentModel.DataAnnotations;

namespace MyLeasing.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
