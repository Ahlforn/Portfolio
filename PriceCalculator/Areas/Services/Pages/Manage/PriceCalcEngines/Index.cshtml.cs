using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PriceCalculator.Areas.Services.Pages.Manage.PriceCalcEngines
{
    public class IndexModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        [BindProperty]
        public IFormFile Upload { get; set; }

        public IndexModel(PriceCalculator.Models.SiteContext context, IConfiguration configuration, IHostingEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _env = env;

            string path = _env.ContentRootPath + "/" + _configuration["CalculatorSettings:ExcelFilePath"];
            PriceCalcEngines = Directory.GetFiles(path).ToList();
            for (int i = 0; i < PriceCalcEngines.Count; i++)
            {
                FileInfo file = new FileInfo(PriceCalcEngines[i]);
                PriceCalcEngines[i] = file.Name;
            }
        }

        public IList<string> PriceCalcEngines { get;set; }
        public string CurrentEngine { get; set; }

        public void OnGet()
        {
            CurrentEngine = _configuration["CalculatorSettings:ExcelFile"];
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Upload == null) return NotFound();

            var file = Path.Combine(_env.ContentRootPath, _configuration["CalculatorSettings:ExcelFilePath"], Upload.FileName);
            using(var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetActivate(int? id)
        {
            if (id == null) RedirectToPage("./Index");
            _configuration["CalculatorSettings:ExcelFile"] = PriceCalcEngines[(int) id];

            string path = Path.Combine(_env.ContentRootPath, "appsettings.json");

            if(System.IO.File.Exists(path))
            {
                JObject jsonContent = JObject.Parse(System.IO.File.ReadAllText(path));
                JObject calcSettings = (JObject)jsonContent["CalculatorSettings"];
                calcSettings.Property("ExcelFile").Value = PriceCalcEngines[(int)id];
                System.IO.File.WriteAllText(path, jsonContent.ToString());
            }
            else
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}
