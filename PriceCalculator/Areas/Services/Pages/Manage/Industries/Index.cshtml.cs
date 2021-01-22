using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.Industries
{
    public class IndexModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public IndexModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        public IList<Industry> Industries { get;set; }

        public async Task OnGetAsync()
        {
            Industries = await _context.Industries.ToListAsync();
        }
    }
}
