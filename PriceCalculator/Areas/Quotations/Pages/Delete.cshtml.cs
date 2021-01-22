using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PriceCalculator.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace PriceCalculator.Areas.Quotations.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly SiteContext _context;
        private readonly IHostingEnvironment _env;

        [BindProperty]
        public Quotation Quotation { get; set; }

        public DeleteModel(SiteContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            Quotation = await _context.Quotations.FirstOrDefaultAsync(q => q.ID == id);

            if(Quotation == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            Quotation = await _context.Quotations.Include(q => q.PrintModels).FirstOrDefaultAsync(q => q.ID == id);

            if(Quotation != null)
            {
                foreach(PrintModel pm in Quotation.PrintModels)
                {
                    _context.PrintModels.Remove(pm);
                    pm.DeleteFile(_env);
                }

                _context.Quotations.Remove(Quotation);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}