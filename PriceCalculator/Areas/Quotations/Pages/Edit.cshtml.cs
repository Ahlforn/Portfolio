using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace PriceCalculator.Areas.Quotations.Pages
{
    public class EditModel : PageModel
    {
        private readonly SiteContext _context;
        private readonly UserManager<SiteUser> _userManager;
        private readonly IHostingEnvironment _env;

        [BindProperty]
        public Quotation Quotation { get; set; }

        public EditModel(SiteContext context, UserManager<SiteUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Quotation = await _context.Quotations
                .Include(q => q.PrintModels)
                    .ThenInclude(pm => pm.Material)
                .Include(q => q.PrintModels)
                    .ThenInclude(pm => pm.LayerThickness)
                .FirstOrDefaultAsync(q => q.ID == id);

            if(Quotation == null)
            {
                return NotFound();
            }

            //foreach(var printModel in Quotation.PrintModels)
            //{
            //    printModel.Material = await _context.PrintModels.FirstOrDefaultAsync(m => m.ID == printModel.)
            //}

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Quotation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            if (id == null)
                return RedirectToPage("./Index", new { Area = "Quotations" });
            else
                return RedirectToPage("./Edit", new { Area = "Quotations", id = id });
        }

        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files, int? id)
        {
            if (files != null && files.Count > 0)
            {
                string path = _env.ContentRootPath + "/Upload/";

                foreach (IFormFile file in files)
                {
                    if (file.Length > 0)
                    {
                        FileInfo fileInfo = new FileInfo(file.FileName);
                        string filename = Guid.NewGuid().ToString() + fileInfo.Extension;
                        string filePath = path + filename;

                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        SiteUser user = await _userManager.GetUserAsync(User);

                        try
                        {
                            PrintModelUpload printModelUpload = new PrintModelUpload(file.FileName, file);
                            await printModelUpload.AddToDb(filePath, _context, user, id);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest();
                        }
                    }
                }
            }

            return RedirectToPage("/Index", new { area = "Quotations" });
        }
    }
}