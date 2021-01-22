using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;
using System.IO;

namespace PriceCalculator.Areas.Model.Pages
{
    public class FileModel : PageModel
    {
        private readonly SiteContext _context;
        private readonly UserManager<SiteUser> _userManager;
        private readonly IHostingEnvironment _env;
        public PrintModel PrintModel { get; set; }

        public FileModel(SiteContext context, UserManager<SiteUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // filename isn't used. It's only there because the STL viewer checks the file extension of the URL referenced filename.
        public async Task<ActionResult> OnGetAsync(int id, string filename)
        {
            PrintModel model = await _context.PrintModels.FirstOrDefaultAsync(pm => pm.ID == id);
            var user = await _userManager.GetUserAsync(User);

            if (user == null || model == null)
            {
                return NotFound();
            }

            // _env.ContentRootPath + "\\Upload\\" + model.Filename
            return PhysicalFile(_env.ContentRootPath + "\\Upload\\" + model.Filename, "text/stl", model.Filename);
        }
    }
}