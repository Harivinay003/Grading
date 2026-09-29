using System.ComponentModel.DataAnnotations.Schema;

namespace Grading.Models
{
    [Table("GradingSummaries")]
    public class GradingSummary
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public decimal Count { get; set; }        // Manual
        public decimal BS { get; set; }
        public decimal Kgs { get; set; }
        public decimal Percentage { get; set; }
        public string? PONumber { get; set; }
        public int? GradeId { get; set; }
        public int? ProductVarietyId { get; set; }
        public DateTime SavedTime { get; set; }

        public Grade Grade { get; set; }
        public ProductVarieties ProductVariety { get; set; }
    }
}
