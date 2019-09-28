using System.ComponentModel.DataAnnotations;

namespace MyLeasing.Common.Models
{
    public class PropertyRequest
    {
        public int Id { get; set; }

        [Required]
        public string Neighborhood { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int SquareMeters { get; set; }

        [Required]
        public int Rooms { get; set; }

        [Required]
        public int Stratum { get; set; }

        public bool HasParkingLot { get; set; }

        public bool IsAvailable { get; set; }

        public string Remarks { get; set; }

        [Required]
        public int PropertyTypeId { get; set; }

        [Required]
        public int OwnerId { get; set; }
    }
}
