using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.Materials
{
    public class EditModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public EditModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Material Material { get; set; }
        public IList<PostProcess> PostProcesses { get; set; }
        [BindProperty]
        public List<int> PostProcessChecked { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Material = await _context.Materials
                .Include(m => m.MaterialPostProcesses)
                .FirstOrDefaultAsync(m => m.ID == id);
            PostProcesses = await _context.PostProcesses.ToArrayAsync();

            if (Material == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Material).State = EntityState.Modified;

            var postProcessLinks = new List<MaterialPostProcess>();
            // Previously attached post-processes
            List<MaterialPostProcess> selectedPostprocesses = await _context.MaterialPostProcesses.Where(mp => mp.MaterialID == Material.ID).ToListAsync();
            foreach (int postProcessID in PostProcessChecked)
            {
                MaterialPostProcess mp = await _context.MaterialPostProcesses.Where(m => m.MaterialID == Material.ID && m.PostProcessID == postProcessID).FirstOrDefaultAsync();

                if (mp == null)
                {
                    mp = new MaterialPostProcess
                    {
                        MaterialID = Material.ID,
                        PostProcessID = postProcessID
                    };
                    postProcessLinks.Add(mp);
                }
                selectedPostprocesses.Remove(mp);

            }
            _context.MaterialPostProcesses.AddRange(postProcessLinks);
            _context.MaterialPostProcesses.RemoveRange(selectedPostprocesses);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(Material.ID))
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

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.ID == id);
        }
    }
}
