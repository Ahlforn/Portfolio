using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.Printers
{
    public class CreateModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        [BindProperty]
        public Printer Printer { get; set; }
        [BindProperty]
        public IList<Material> Materials { get; set; }
        [BindProperty]
        public List<int> Selected { get; set; }

        public CreateModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Materials = await _context.Materials.Include(m => m.LayerThicknesses).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Printers.Add(Printer);

            List<PrinterLayerThickness> oldSelection = await _context.PrinterLayerThicknesses.Where(pl => pl.PrinterID == Printer.ID).ToListAsync();
            _context.PrinterLayerThicknesses.RemoveRange(oldSelection);

            foreach (int layerThicknessID in Selected)
            {
                PrinterLayerThickness pl = new PrinterLayerThickness
                {
                    PrinterID = Printer.ID,
                    LayerThicknessID = layerThicknessID
                };
                _context.PrinterLayerThicknesses.Add(pl);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }

            return RedirectToPage("./Index");
        }
    }
}