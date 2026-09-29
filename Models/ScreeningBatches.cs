using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Grading.Models
{
    public class ScreeningBatches
    {
        public int BatchId { get; set; }
        public DateTime BatchDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string? From { get; set; }
        public string? VehicleNo { get; set; }
        public string? Center { get; set; }
        public string? HLHot { get; set; }

        public string? HLCount { get; set; }
        public decimal? SamCount { get; set; }
        public decimal? AvgCount { get; set; }

        public decimal? B2 { get; set; }
        public decimal? B4 { get; set; }
         
        public string? ShrimpPerSec { get; set; }
        public string? G2 { get; set; }

        public string? Remarks { get; set; }
        public int SheetNo { get; set; }

        // NAVIGATION
        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}
