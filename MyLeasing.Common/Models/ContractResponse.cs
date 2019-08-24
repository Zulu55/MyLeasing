using System;

namespace MyLeasing.Common.Models
{
    public class ContractResponse
    {
        public int Id { get; set; }

        public string Remarks { get; set; }

        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public LesseeResponse Lessee { get; set; }

        public DateTime StartDateLocal => StartDate.ToLocalTime();

        public DateTime EndDateLocal => EndDate.ToLocalTime();
    }
}
