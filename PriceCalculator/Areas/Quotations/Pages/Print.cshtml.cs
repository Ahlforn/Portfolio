using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Quotations.Pages
{
    public class PrintQuotationModel : PageModel
    {
        private readonly SiteContext _context;

        public Quotation quotation { get; set; }
        public List<Models.PrintModel> models { get; set; }
        public Dictionary<int, List<PrintModelPostProcess>> postProcesses { get; set; }

        public PrintQuotationModel(SiteContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            quotation = await _context.Quotations.Include(p => p.User).FirstOrDefaultAsync(q => q.ID == id);
            models = await _context.PrintModels.Include(pm => pm.Material).Include(pm => pm.LayerThickness).Include(pm => pm.Printer).Include(pm => pm.Industry).Where(pm => pm.Quotation == quotation).ToListAsync();

            postProcesses = new Dictionary<int, List<PrintModelPostProcess>>();
            foreach (Models.PrintModel model in models)
            {
                List<PrintModelPostProcess> processes = await _context.PrintModelPostProcesses.Include(pmpp => pmpp.PostProcess).Where(pmpp => pmpp.PrintModel == model).ToListAsync();
                postProcesses.Add(model.ID, processes);
            }

            return Page();
        }
    }
}
