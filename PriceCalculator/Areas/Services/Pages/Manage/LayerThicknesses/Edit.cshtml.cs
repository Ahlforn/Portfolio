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
    public class EditModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public CultureInfo culture;

        public EditModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
            culture = Thread.CurrentThread.CurrentCulture;
        }

        [BindProperty]
        public LayerThickness LayerThickness { get; set; }
        [BindProperty]
        public List<Printer> Printers { get; set; }
        [BindProperty]
        public List<int> SelectedPrinters { get; set; }
        [BindProperty]
        public List<string> DefinedNames { get; set; }
        [BindProperty]
        public List<PrinterLayerThickness> PrinterLayerThicknesses { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LayerThickness = await _context.LayerThicknesses
                .Include(l => l.Material).FirstOrDefaultAsync(m => m.ID == id);

            if (LayerThickness == null)
            {
                return NotFound();
            }

            ViewData["MaterialID"] = new SelectList(_context.Materials, "ID", "Name");

            Printers = await _context.Printers.ToListAsync();

            SelectedPrinters = new List<int>();
            PrinterLayerThicknesses = await _context.PrinterLayerThicknesses.Where(p => p.LayerThicknessID == id).ToListAsync();
            foreach(PrinterLayerThickness p in PrinterLayerThicknesses)
            {
                SelectedPrinters.Add(p.PrinterID);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            _context.Attach(LayerThickness).State = EntityState.Modified;

            List<PrinterLayerThickness> oldSelection = await _context.PrinterLayerThicknesses.Where(pl => pl.LayerThicknessID == LayerThickness.ID).ToListAsync();
            _context.PrinterLayerThicknesses.RemoveRange(oldSelection);

            //Printers = await _context.Printers.ToListAsync();
            for(int i = 0; i < SelectedPrinters.Count; i++)
            {
                PrinterLayerThickness pl = new PrinterLayerThickness
                {
                    PrinterID = SelectedPrinters[i],
                    LayerThicknessID = LayerThickness.ID,
                    ExcelDefinedName = DefinedNames[i]
                };
                _context.PrinterLayerThicknesses.Add(pl);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LayerThicknessExists(LayerThickness.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LayerThicknessExists(int id)
        {
            return _context.LayerThicknesses.Any(e => e.ID == id);
        }
    }
}
