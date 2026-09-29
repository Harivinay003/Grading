namespace Grading.Models
{
 
        public class SummaryRowVM
        {
        public decimal Manual { get; set; }
        public decimal TotalBS { get; set; }

        public int? GradeId { get; set; }
        public int? ProductVarietyId { get; set; }
        public decimal? Kgs { get; set; }
        public decimal? Percentage { get; set; }
        public string? PONumber { get; set; }
    }

    
}
