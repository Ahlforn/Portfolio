using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.PriceCalcEngines
{
    public class DeleteModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public DeleteModel(PriceCalculator.Models.SiteContext context, IConfiguration configuration, IHostingEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        [BindProperty]
        public string Filename { get; set; }

        public IActionResult OnGet(string file)
        {
            if (file == null)
            {
                return NotFound();
            }

            FileInfo engine = new FileInfo(_env.ContentRootPath + "/" + _configuration["CalculatorSettings:ExcelFilePath"] + file);

            if (engine == null || !engine.Exists)
            {
                return NotFound();
            }

            Filename = engine.Name;

            return Page();
        }

        public IActionResult OnPostAsync(string file)
        {
            if (file == null)
            {
                return NotFound();
            }

            FileInfo engine = new FileInfo(_env.ContentRootPath + "/" + _configuration["CalculatorSettings:ExcelFilePath"] + file);

            if (engine != null && engine.Exists)
            {
                engine.Delete();
            }

            return RedirectToPage("./Index");
        }
    }
}
