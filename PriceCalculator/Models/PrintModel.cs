using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace PriceCalculator.Models
{
    public class PrintModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Filename { get; set; }
        public string Snapshot { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Volume { get; set; }
        public string PrintDirection { get; set; }  // Euler angles in the string format <x,y,z>.
        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; }
        public Material Material { get; set; }
        public LayerThickness LayerThickness { get; set; }
        public Printer Printer { get; set; }
        public SiteUser User { get; set; }
        public Quotation Quotation { get; set; }
        public Industry Industry { get; set; }
        public int Amount { get; set; }
        [Display(Name = "Price Per Part")]
        public double PricePerPart { get; set; }
        [Display(Name = "Price Total")]
        public double PriceTotal { get; set; }
        public ICollection<PrintModelPostProcess> PrintModelPostProcesses { get; set; }

        public bool IsValid()
        {
            // Check dimensions
            if (X <= 0 || Y <= 0 || Z <= 0 || Volume <= 0)
                return false;
            // Material and layer thickness choice
            if (Material == null || LayerThickness == null)
                return false;

            return true;
        }

        public bool DeleteFile(IHostingEnvironment env)
        {
            FileInfo file = new FileInfo(env.ContentRootPath + "/Upload/" + Filename);

            if (file.Exists)
                try
                {
                    file.Delete();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

            return false;
        }
    }
}
