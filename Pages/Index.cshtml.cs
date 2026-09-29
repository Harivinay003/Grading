
using Grading.Data;
using Grading.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }
    [BindProperty]
    public ScreeningBatches Batch { get; set; }
    [BindProperty]
    public Screening Screening { get; set; }
    public List<Screening> ScreeningList { get; set; } = new();
    [BindProperty]
    public GradingSummary Summary { get; set; }
    [BindProperty]
    public int EditScreeningId { get; set; }
    public List<Grade> Grades { get; set; } = new();
    public List<ProductVarieties> ProductVarieties { get; set; } = new();
    public List<SummaryRowVM> SummaryRows { get; set; } = new();
    public bool IsSummarySaved { get; set; }
    public decimal TotalKgs { get; set; }
    public bool IsBatchSaved { get; set; }
    public bool IsGradingSaved { get; set; }
    [BindProperty]
    public List<Screening> Screenings { get; set; } = new();
    public int SheetNo { get; set; }
    public bool IsEditMode { get; set; }
    public bool IsSummaryEditable { get; set; }


    public void OnGet(int? batchId, bool edit = false)
    {
        IsEditMode = edit;

        Grades = _context.Grades.OrderBy(g => g.Name).ToList();
        ProductVarieties = _context.ProductVarieties.OrderBy(p => p.Name).ToList();

        SummaryRows = new();
        TotalKgs = 0;

        if (batchId.HasValue)
        {
            Batch = _context.ScreeningBatches
                            .FirstOrDefault(b => b.BatchId == batchId.Value);
        }
        else
        {
            Batch = _context.ScreeningBatches
                            .Where(b => b.EndTime == null)
                            .OrderByDescending(b => b.BatchId)
                            .FirstOrDefault();
        }

        if (Batch == null)
        {
            Batch = new ScreeningBatches
            {
                BatchDate = DateTime.Today,
                StartTime = DateTime.Now,
                EndTime = null
            };

            SheetNo = GetNextSheetNoForDate(Batch.BatchDate);

            Screenings = Enumerable.Range(1, 8)
                .Select(b => new Screening { BIN = b })
                .ToList();

            IsBatchSaved = false;
            IsGradingSaved = false;
            IsSummarySaved = false;
            IsSummaryEditable = true;
            return;
        }

        SheetNo = Batch.SheetNo;
        IsBatchSaved = true;

        int id = Batch.BatchId;

        var savedScreenings = _context.Screenings
                                      .Where(s => s.BatchId == id)
                                      .ToList();

        IsGradingSaved = savedScreenings.Any();

        Screenings = Enumerable.Range(1, 8)
            .Select(bin =>
                savedScreenings.FirstOrDefault(s => s.BIN == bin)
                ?? new Screening { BIN = bin, BatchId = id }
            )
            .ToList();

        SummaryRows = savedScreenings
            .Where(s => s.Manual > 0)
            .GroupBy(s => s.Manual)
            .Select(g => new SummaryRowVM
            {
                Manual = g.Key,
                TotalBS = g.Max(x => x.BS)
            })
            .OrderBy(x => x.Manual)
            .ToList();

        var summaries = _context.GradingSummary
                                .Where(g => g.BatchId == id)
                                .ToList();

        IsSummarySaved = summaries.Any();

        if (IsSummarySaved)
        {
            foreach (var row in SummaryRows)
            {
                var s = summaries.FirstOrDefault(x => x.Count == row.Manual);
                if (s != null)
                {
                    row.GradeId = s.GradeId;
                    row.ProductVarietyId = s.ProductVarietyId;
                    row.Kgs = s.Kgs;
                    row.Percentage = s.Percentage;
                    row.PONumber = s.PONumber;
                }
            }

            TotalKgs = summaries.Sum(x => x.Kgs);
        }

        IsSummaryEditable = Batch.EndTime == null || IsEditMode;
    }

    public JsonResult OnGetSheetNoForDate(DateTime date)
    {
        int sheetNo = GetNextSheetNoForDate(date);
        return new JsonResult(sheetNo);
    }

    private int GetNextSheetNoForDate(DateTime date)
    {
        if (date < new DateTime(1753, 1, 1))
            date = DateTime.Today;

        date = date.Date;

        int lastSheetNo = _context.ScreeningBatches
            .Where(b => b.BatchDate.Date == date)
            .Select(b => (int?)b.SheetNo)
            .Max() ?? 0;

        return lastSheetNo + 1;
    }

    public IActionResult OnPostSaveBatch()
    {
        if (Batch.BatchId > 0)
        {
            var existingBatch = _context.ScreeningBatches
                                    .FirstOrDefault(b => b.BatchId == Batch.BatchId);

            if (existingBatch == null)
                return NotFound();

            if (Batch.BatchDate < new DateTime(1753, 1, 1))
                Batch.BatchDate = DateTime.Today;

            if (Batch.StartTime < new DateTime(1753, 1, 1))
                Batch.StartTime = DateTime.Now;

            existingBatch.BatchDate = Batch.BatchDate;
            //existingBatch.StartTime = Batch.StartTime;
            //existingBatch.EndTime = Batch.EndTime;
            existingBatch.VehicleNo = Batch.VehicleNo;
            existingBatch.Center = Batch.Center;
            existingBatch.From = Batch.From;
            existingBatch.HLHot = Batch.HLHot?.ToLower();
            existingBatch.HLCount = Batch.HLCount?.ToLower();
            existingBatch.SamCount = Batch.SamCount;
            existingBatch.AvgCount = Batch.AvgCount;
            existingBatch.B2 = Batch.B2;
            existingBatch.B4 = Batch.B4;
            existingBatch.G2 = Batch.G2?.ToLower();
            existingBatch.ShrimpPerSec = Batch.ShrimpPerSec?.ToLower();
            existingBatch.Remarks = Batch.Remarks;

            _context.SaveChanges();

            TempData["Message"] = "Batch details updated successfully.";
            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Edited Batch Saved";

            return RedirectToPage(new { batchId = existingBatch.BatchId });
        }

        if (Batch.BatchDate < new DateTime(1753, 1, 1))
            Batch.BatchDate = DateTime.Today;

        Batch.BatchDate = Batch.BatchDate.Date;
        Batch.StartTime = DateTime.Now;
        Batch.EndTime = null;

        Batch.SheetNo = GetNextSheetNoForDate(Batch.BatchDate);
        Batch.HLHot = Batch.HLHot?.ToLower();
        Batch.HLCount = Batch.HLCount?.ToLower();
        Batch.G2 = Batch.G2?.ToLower();
        Batch.ShrimpPerSec = Batch.ShrimpPerSec?.ToLower();
        Batch.Remarks = Batch.Remarks?.ToLower();
        _context.ScreeningBatches.Add(Batch);
        _context.SaveChanges();

        TempData["Message"] = "Batch saved successfully.";
        TempData["MessageType"] = "success";
        TempData["MessageTitle"] = "Batch Saved";

        return RedirectToPage(new { batchId = Batch.BatchId });
    }

    public IActionResult OnPostSaveAllGrading()
    {
        bool isEditGrading = false;
        if (Batch.BatchId <= 0)
            return RedirectToPage();

        foreach (var row in Screenings)
        {
            if ((row.From == null || row.From == 0) &&
                  (row.To == null || row.To == 0))
            {
                continue;
            }

            var existing = _context.Screenings
                .FirstOrDefault(s =>
                    s.BatchId == Batch.BatchId &&
                    s.BIN == row.BIN);

            if (existing == null)
            {
                row.BatchId = Batch.BatchId;
                row.SavedTime = DateTime.Now;
                _context.Screenings.Add(row);
            }
            else
            {
                existing.From = row.From;
                existing.To = row.To;
                existing.Manual = row.Manual;
                existing.Screen = row.Screen;
                existing.BS = row.BS;
                //existing.SavedTime = DateTime.Now;
                isEditGrading = true;
            }
        }

        _context.SaveChanges();
        if (isEditGrading)
        {

            TempData["Message"] = "Grading details Updated successfully.";
            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Grading Updated";

        }
        else
        {
            TempData["Message"] = "Grading details saved successfully.";
            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Grading Saved";
        }

        return Redirect($"/Index?batchId={Batch.BatchId}#gradingSection");
    }

    public IActionResult OnPostSaveSummary(
    Dictionary<decimal, decimal> Kgs,
    Dictionary<decimal, decimal> Percentage,
    Dictionary<decimal, int?> GradeId,
    Dictionary<decimal, int?> ProductVarietyId,
    Dictionary<decimal, string> PONumber)
    {
        bool isEditSummary = false;
        if (Batch.BatchId <= 0)
            return RedirectToPage();

        decimal totalKgs = Kgs?.Values.Sum() ?? 0;

        foreach (var manual in Kgs.Keys)
        {
            var kgs = Kgs[manual];

            var existing = _context.GradingSummary
                .FirstOrDefault(s =>
                    s.BatchId == Batch.BatchId &&
                    s.Count == manual);

            if (existing == null)
            {
                existing = new GradingSummary
                {
                    BatchId = Batch.BatchId,
                    Count = manual
                };
                _context.GradingSummary.Add(existing);
            }

            existing.BS = _context.Screenings
                .Where(s => s.BatchId == Batch.BatchId &&
                            s.Manual == manual)
                .Max(s => s.BS);

            existing.Kgs = kgs;
            existing.Percentage = totalKgs > 0
                ? Math.Round((kgs / totalKgs) * 100, 2)
                : 0;

            existing.GradeId = GradeId.ContainsKey(manual)
                ? GradeId[manual]
                : null;

            existing.ProductVarietyId = ProductVarietyId.ContainsKey(manual)
                ? ProductVarietyId[manual]
                : null;

            existing.PONumber = PONumber.ContainsKey(manual)
                ? PONumber[manual]
                : null;

            existing.SavedTime = DateTime.Now;
            isEditSummary = true;
        }

        _context.SaveChanges();
        if (isEditSummary)
        {
            TempData["Message"] = "Summary updated successfully.";
            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Info";
        }
        else
        {
            TempData["Message"] = "Grading summary saved successfully.";
            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Summary Saved";
        }

        return Redirect($"/Index?batchId={Batch.BatchId}#summarySection");
    }
    public IActionResult OnPostEndBatch()
    {
        if (Batch.BatchId <= 0)
        {
            TempData["Message"] = "No active batch to end.";
            TempData["MessageType"] = "warning";
            TempData["MessageTitle"] = "Info";
            return RedirectToPage();
        }

        var batch = _context.ScreeningBatches
                            .FirstOrDefault(b => b.BatchId == Batch.BatchId);

        if (batch == null)
        {
            TempData["Message"] = "Batch not found.";
            TempData["MessageType"] = "danger";
            TempData["MessageTitle"] = "Info";
            return RedirectToPage();
        }

        //batch.EndTime = DateTime.Now;
        if (batch.EndTime == null)   
        {
            batch.EndTime = DateTime.Now;
            _context.SaveChanges();
        }

        _context.SaveChanges();
        TempData["Message"] = "Batch ended successfully.";
        TempData["MessageType"] = "success";
        TempData["MessageTitle"] = "Info";

        return RedirectToPage();
    }
}
