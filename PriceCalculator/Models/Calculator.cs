using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SpreadsheetGear;

namespace PriceCalculator.Models
{
    /// <summary>
    /// Class <c>calculator</c> is an API for communication with the Excel price calculator engine.
    /// </summary>
    public class Calculator
    {
        private readonly SiteContext _context;
        private CalculatorSettings settings;
        private bool CommaDecimalSeparator { get; }


        public Calculator(CalculatorSettings settings, SiteContext context)
        {
            this.settings = settings;
            _context = context;
            CommaDecimalSeparator = settings.CommaDecimalSeparator;
        }

        public IWorkbook GenerateWorkbook(PrintModel model, PrinterLayerThickness plt, List<PostProcess> pps, int count)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            IWorkbook workbook = Factory.GetWorkbook(settings.ExcelFilePath + settings.ExcelFile, culture);
            Dictionary<string, string> DefinedNames = this.settings.DefinedNames;

            if (DefinedNames == null || DefinedNames.Count == 0)
            {
                throw new Exception("No defined names defined.");
            }
            if(workbook == null)
            {
                throw new Exception("No excel workbook available");
            }

            IRange rCustomer = workbook.Names[DefinedNames["Customer"]].RefersToRange;
            IRange rAddress = workbook.Names[DefinedNames["DeliveryAddresss"]].RefersToRange;
            IRange rFilename = workbook.Names[DefinedNames["Filename"]].RefersToRange;
            IRange rDate = workbook.Names[DefinedNames["Date"]].RefersToRange;
            IRange rInitials = workbook.Names[DefinedNames["Initials"]].RefersToRange;
            IRange rX = workbook.Names[DefinedNames["X"]].RefersToRange;
            IRange rY = workbook.Names[DefinedNames["Y"]].RefersToRange;
            IRange rZ = workbook.Names[DefinedNames["Z"]].RefersToRange;
            IRange rVolume = workbook.Names[DefinedNames["Volume"]].RefersToRange;
            IRange rCount = workbook.Names[DefinedNames["Count"]].RefersToRange;
            IRange rLayerThickness = workbook.Names[plt.ExcelDefinedName].RefersToRange;
            IRange rPrice = workbook.Names[DefinedNames["Price"]].RefersToRange;            

            rCustomer.Formula = model.Quotation.Customer == null ? "" : model.Quotation.Customer;
            rAddress.Formula = model.Quotation.DeliveryAddress == null ? "" : model.Quotation.DeliveryAddress;
            rFilename.Formula = model.Name == null ? "" : model.Name;
            rDate.Formula = model.UploadDate.ToString();
            rInitials.Formula = model.User.Email;
            rX.Formula = Math.Round(model.X, 2).ToString();
            rY.Formula = Math.Round(model.Y, 2).ToString();
            rZ.Formula = Math.Round(model.Z, 2).ToString();
            rVolume.Formula = Math.Round(model.Volume, 2).ToString();
            rCount.Formula = count.ToString();
            rLayerThickness.Formula = "1";

            if (workbook.Names[DefinedNames["QuotationNumber"]] != null)
            {
                IRange rQuotationNumber = workbook.Names[DefinedNames["QuotationNumber"]].RefersToRange;
                rQuotationNumber.Formula = model.Quotation.Title;
            }

                if (workbook.Names[DefinedNames["BusinessArea"]] != null && model.Industry != null)
            {
                IRange rBusinessArea = workbook.Names[DefinedNames["BusinessArea"]].RefersToRange;
                rBusinessArea.Formula = model.Industry.Name;
            }

            foreach (PostProcess pp in pps)
            {
                IRange r = workbook.Names[pp.ExcelDefinedName].RefersToRange;
                r.Formula = "1";
            }

            rPrice.NumberFormat = "[=0]0;###.###,00";
            double price;

            try
            {
                price = Double.Parse(workbook.Names[DefinedNames["Price"]].RefersToRange.Text);
            }
            catch
            {
                price = default;
            }

            //_workbook.SaveAs("debug.xlsx", FileFormat.OpenXMLWorkbook);
            //System.Diagnostics.Debug.WriteLine(price);
            return workbook;
        }

        public double GetPrice(PrintModel model, PrinterLayerThickness plt, List<PostProcess> pps, int count, out Dictionary<int, double> processes)
        {
            IWorkbook workbook = GenerateWorkbook(model, plt, pps, count);

            double price;

            try
            {
                price = Double.Parse(workbook.Names[settings.DefinedNames["Price"]].RefersToRange.Text);

                List<PostProcess> postProcesses = _context.PostProcesses.ToList();
                processes = new Dictionary<int, double>();
                foreach(PostProcess pp in postProcesses)
                {
                    IRange r = workbook.Names[pp.ExcelDefinedName].RefersToRange;
                    IRange rP = workbook.Names[pp.ExcelDefinedPriceName].RefersToRange;
                    string value = r.Text;

                    r.Formula = "1";
                    processes.Add(pp.ID, Double.Parse(rP.Text));
                    r.Formula = value;
                }
            }
            catch
            {
                price = default;
                processes = null;
            }

            return price;
        }

        public void SetExcelFile(string filename)
        {
            settings.ExcelFile = filename;
        }

        public class CalculatorSettings
        {
            public string ExcelFilePath { get; set; }
            public string ExcelFile { get; set; }
            public bool CommaDecimalSeparator { get; set; }
            public Dictionary<string, string> DefinedNames { get; set; }
        }
    }
}
