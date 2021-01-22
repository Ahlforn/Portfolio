using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PriceCalculator.Models
{
    public class PrintModelUpload
    {
        [Required]
        [Display(Name = "Model Name")]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Model Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Model File")]
        public IFormFile ModelFile { get; set; }

        public PrintModelUpload(string name, IFormFile modelFile)
        {
            Name = name;
            ModelFile = modelFile;
        }

        public async Task AddToDb(string filepath, SiteContext context, SiteUser user, int? orderID)
        {
            FileInfo file = new FileInfo(filepath);
            PrintModel model = new PrintModel();
            model.Name = this.Name;
            model.Description = this.Description;
            model.Filename = file.Name;
            model.UploadDate = DateTime.Now;

            try
            {
                PrintModelFile printModelFile = new PrintModelFile(filepath, PrintModelFile.FileType.STL);

                model.X = printModelFile.X;
                model.Y = printModelFile.Y;
                model.Z = printModelFile.Z;
                model.Volume = printModelFile.Volume;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            if (orderID == null)
                throw new Exception("No quotation ID provided");

            Quotation quotation = await context.Quotations.Include(q => q.PrintModels).FirstOrDefaultAsync(q => q.ID == orderID);
            quotation.PrintModels.Add(model);
            context.Quotations.Update(quotation);

            model.User = user;
            model.Quotation = quotation;
            
            context.PrintModels.Add(model);
            await context.SaveChangesAsync();
        }
    }
}
