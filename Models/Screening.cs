using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Grading.Models
{
    public class Screening
    {
        [Key]
        public int ScreeningId { get; set; }
        public int BatchId { get; set; }
        public int BIN { get; set; }
        public decimal From { get; set; }
        public decimal To { get; set; }
        public decimal Manual { get; set; }
        public decimal Screen { get; set; }
        public decimal BS { get; set; }
        public DateTime SavedTime { get; set; }
        //public ScreeningBatches ScreeningBatches { get; set; }
        public ScreeningBatches? ScreeningBatches { get; set; }


    }

}
