using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.PostProcesses
{
    public class CreateModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public CreateModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PostProcess PostProcess { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.PostProcesses.Add(PostProcess);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}