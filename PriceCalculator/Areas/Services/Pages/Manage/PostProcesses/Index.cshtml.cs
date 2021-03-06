﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.PostProcesses
{
    public class IndexModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public IndexModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        public IList<PostProcess> PostProcesses { get;set; }

        public async Task OnGetAsync()
        {
            PostProcesses = await _context.PostProcesses.ToListAsync();
        }
    }
}
