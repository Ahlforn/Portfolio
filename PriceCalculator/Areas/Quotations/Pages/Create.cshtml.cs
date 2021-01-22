using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Quotations.Pages
{
    public class CreateModel : PageModel
    {
        private readonly SiteContext _context;
        private readonly UserManager<SiteUser> _userManager;
        private readonly IConfiguration _configuration;

        public CreateModel(SiteContext context, UserManager<SiteUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public void OnGet()
        {
            
        }

        [BindProperty]
        public Quotation Quotation { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            Quotation.User = await _userManager.GetUserAsync(User);
            Quotation.Created = DateTime.Now;
            Quotation.EngineFile = _configuration["CalculatorSettings:ExcelFile"];

            _context.Quotations.Add(Quotation);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { Area = "Quotations" });
        }
    }
}