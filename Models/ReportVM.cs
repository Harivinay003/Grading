using Grading.Models;

public class ReportVM
{
    public ScreeningBatches Batch { get; set; }
    public List<Screening> Screenings { get; set; } = new();
    public List<GradingSummary> Summaries { get; set; } = new();
    public decimal TotalKgs { get; set; }
}
