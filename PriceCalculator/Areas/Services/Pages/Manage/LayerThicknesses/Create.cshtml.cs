using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.LayerThicknesses
{
    public class CreateModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        [BindProperty]
        public LayerThickness LayerThickness { get; set; }
        [BindProperty]
        public List<Printer> Printers { get; set; }
        [BindProperty]
        public List<int> SelectedPrinters { get; set; }
        [BindProperty]
        public List<string> DefinedNames { get; set; }

        public CultureInfo culture;

        public CreateModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
            culture = Thread.CurrentThread.CurrentCulture;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["MaterialID"] = new SelectList(_context.Materials, "ID", "Name");

            Printers = await _context.Printers.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            _context.LayerThicknesses.Add(LayerThickness);

            for (int i = 0; i < SelectedPrinters.Count; i++)
            {
                PrinterLayerThickness pl = new PrinterLayerThickness
                {
                    PrinterID = SelectedPrinters[i],
                    LayerThicknessID = LayerThickness.ID,
                    ExcelDefinedName = DefinedNames[i]
                };
                _context.PrinterLayerThicknesses.Add(pl);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}