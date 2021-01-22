using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;
using System.IO;
using Microsoft.Extensions.Logging;

namespace PriceCalculator.Areas.Model.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly SiteContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(SiteContext context, IHostingEnvironment env, ILogger<DeleteModel> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            //PrintModel model = await _context.PrintModels.Include(q => q.ID).FirstOrDefaultAsync(pm => pm.ID == id);
            //int quotationID = model.Quotation.ID;

            //if(model != null)
            //{
            //    _context.PrintModels.Remove(model);
            //    await _context.SaveChangesAsync();
            //    model.DeleteFile(_env);

            //    RedirectToPage("Edit", new { area = "Quotations", id = quotationID });
            //}

            //return BadRequest();

            PrintModel model = await _context.PrintModels.Include(pm => pm.Quotation).FirstOrDefaultAsync(pm => pm.ID == id);

            if(model != null)
            {
                Quotation quotation = model.Quotation;
                List<PrintModelPostProcess> ppRels = await _context.PrintModelPostProcesses.Where(pmpp => pmpp.PrintModel == model).ToListAsync();
                FileInfo file = new FileInfo(_env.ContentRootPath + "/Upload/" + model.Filename);

                try
                {
                    if (file.Exists)
                        file.Delete();

                    _context.PrintModelPostProcesses.RemoveRange(ppRels);
                    _context.PrintModels.Remove(model);

                    await _context.SaveChangesAsync();

                    return RedirectToPage("Edit", new { area = "Quotations", id = quotation.ID });
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to delete print model: {ex}", ex);

                    return BadRequest();
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> OnGetPurgeAsync()
        {
            List<PrintModel> printModels = await _context.PrintModels.ToListAsync();
            DirectoryInfo dirInfo = new DirectoryInfo(_env.ContentRootPath + "/Upload");

            foreach(FileInfo file in dirInfo.GetFiles())
            {
                if(!printModels.Exists(pm => pm.Filename == file.Name))
                {
                    file.Delete();
                }
            }

            return Page();
        }
    }
}
