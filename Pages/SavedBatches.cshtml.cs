
using Grading.Data;
using Grading.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Grading.Pages
{
    public class SavedBatchesModel : PageModel
    {
        private readonly AppDbContext _context;

        public SavedBatchesModel(AppDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public DateTime Date { get; set; } = DateTime.Today;

        public List<ScreeningBatches> Batches { get; set; } = new();

        public void OnGet()
        {
            Date = DateTime.Today;

            Batches = _context.ScreeningBatches
                              .Where(b => b.BatchDate.Date == Date)
                              .OrderBy(b => b.BatchId)
                              .AsNoTracking()
                              .ToList();
        }

        public void OnPostSearchByDate()
        {
            Batches = _context.ScreeningBatches
                              .Where(b => b.BatchDate.Date == Date.Date)
                              .OrderBy(b => b.BatchId)
                              .AsNoTracking()
                              .ToList();
        }

        public PartialViewResult OnGetLoadReport(int batchId)
        {
            var batch = _context.ScreeningBatches
                                .AsNoTracking()
                                .FirstOrDefault(b => b.BatchId == batchId);

            if (batch == null)
                return Partial("_BatchReportPartial", null);

            var screenings = _context.Screenings
                                     .Where(s => s.BatchId == batchId)
                                     .OrderBy(s => s.BIN)
                                     .AsNoTracking()
                                     .ToList();

            var summaries = _context.GradingSummary
                                    .Include(g => g.Grade)
                                    .Include(g => g.ProductVariety)
                                    .Where(g => g.BatchId == batchId)
                                    .AsNoTracking()
                                    .ToList();

            var vm = new ReportVM
            {
                Batch = batch,
                Screenings = screenings,
                Summaries = summaries,
                TotalKgs = summaries.Sum(x => x.Kgs)
            };

            return Partial("_BatchReportPartial", vm);
        }
    }
}
