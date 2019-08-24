using System.ComponentModel.DataAnnotations;

namespace MyLeasing.Common.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
