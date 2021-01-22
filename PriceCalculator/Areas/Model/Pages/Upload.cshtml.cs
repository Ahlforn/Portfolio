using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Model.Pages
{
    public class UploadModel : PageModel
    {
        private readonly IHostingEnvironment _env;
        private readonly UserManager<SiteUser> _userManager;
        private SiteContext _context;

        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public int? id { get; set; }

        public UploadModel(IHostingEnvironment env, UserManager<SiteUser> userManager, SiteContext context)
        {
            _env = env;
            _userManager = userManager;
            _context = context;
        }

        public void OnGet(int? orderID)
        {
            this.id = orderID;
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile> files, int? orderID)
        {
            if (files != null && files.Count > 0)
            {
                string path = _env.ContentRootPath + "/Upload/";

                foreach(IFormFile file in files)
                {
                    if(file.Length > 0)
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
                            await printModelUpload.AddToDb(filePath, _context, user, orderID);
                        }
                        catch (Exception ex)
                        {
                            StatusMessage = "Error: STL file could not be processed.";
                            return BadRequest();
                        }
                    }
                }
            }

            return RedirectToPage("/Index");
        }
    }
}