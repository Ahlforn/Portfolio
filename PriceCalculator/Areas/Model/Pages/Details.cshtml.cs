using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PriceCalculator.Models;
using Newtonsoft.Json;
using System.Threading;
using System.Globalization;
using SpreadsheetGear;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;

namespace PriceCalculator.Areas.Model.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly SiteContext _context;
        private readonly UserManager<SiteUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly Calculator _calculator;
        private readonly ILogger<DetailsModel> _logger;
        public PrintModel PrintModel { get; set; }
        public List<Material> Materials { get; set; }

        [BindProperty]
        public string printDirection { get; set; }
        [BindProperty]
        public string aabb { get; set; }
        [BindProperty]
        public int materialOption { get; set; }
        [BindProperty]
        public int layerThicknessOption { get; set; }
        [BindProperty]
        public int printerOption { get; set; }
        [BindProperty]
        public int industryOption { get; set; }
        [BindProperty]
        public int[] postProcess { get; set; }
        [BindProperty]
        public int printAmount { get; set; }
        [BindProperty]
        public string pricePerPart { get; set; }
        [BindProperty]
        public string priceTotal { get; set; }
        [BindProperty]
        public string snapshot { get; set; }

        public DetailsModel(SiteContext context, UserManager<SiteUser> userManager, IConfiguration configuration, ILogger<DetailsModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Calculator.CalculatorSettings settings = new Calculator.CalculatorSettings();
            _configuration.GetSection("CalculatorSettings").Bind(settings);

            string file = configuration.GetValue<string>("ExcelPriceEngine");
            Dictionary<string, string> definedNames = configuration.GetValue<Dictionary<string, string>>("ExcelDefinedNames");

            _calculator = new Calculator(settings, _context);
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            PrintModel = await _context.PrintModels.Include(pm => pm.Material).Include(pm => pm.LayerThickness).Include(pm => pm.Printer).Include(pm => pm.PrintModelPostProcesses).Include(pm => pm.Industry).FirstOrDefaultAsync(pm => pm.ID == id);
            Materials = await _context.Materials.Include(m => m.LayerThicknesses).ToListAsync();
            printAmount = PrintModel.Amount == 0 ? 1 : PrintModel.Amount;
            printDirection = PrintModel.PrintDirection;
            SiteUser user = await _userManager.GetUserAsync(User);
            ViewData["name"] = PrintModel.Name;

            if(PrintModel == null || user == null)
            {
                NotFound();
            }

            List<PrintModelPostProcess> ppRel = await _context.PrintModelPostProcesses.Where(pmpp => pmpp.PrintModelID == id).ToListAsync();
            postProcess = new int[ppRel.Count];

            foreach(PrintModelPostProcess rel in ppRel)
            {
                postProcess.Append(rel.PostProcessID);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            PrintModel model = await buildModel(id);

            if (model != null)
            {
                if (!model.IsValid())
                    return BadRequest();

                model.Amount = printAmount;
                model.PricePerPart = ParsePrice(pricePerPart);
                model.PriceTotal = ParsePrice(priceTotal);
                model.Snapshot = snapshot;
                _context.Attach(model).State = EntityState.Modified;

                List<PrintModelPostProcess> ppRels = await _context.PrintModelPostProcesses.Where(pmpp => pmpp.PrintModelID == id).ToListAsync();
                _context.PrintModelPostProcesses.RemoveRange(ppRels);

                foreach (int ppid in postProcess)
                {
                    PostProcess p = await _context.PostProcesses.FirstOrDefaultAsync(pp => pp.ID == ppid);

                    if (p != null)
                    {
                        PrintModelPostProcess ppRel = new PrintModelPostProcess();
                        ppRel.PostProcess = p;
                        ppRel.PrintModel = model;
                        _context.PrintModelPostProcesses.Add(ppRel);
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("Edit", new { area = "Quotations", id = model.Quotation.ID });
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> OnPostPriceAsync(int id)
        {
            PrintModel model = await buildModel(id);
            PrinterLayerThickness plt = null;
            List<PostProcess> postProcesses = await ParsePostProcesses(postProcess);

            if (model != null && model.Printer != null && model.LayerThickness != null)
            {
                plt = await _context.PrinterLayerThicknesses.FirstOrDefaultAsync(p => p.PrinterID == model.Printer.ID && p.LayerThicknessID == model.LayerThickness.ID);
            }

            if(model != null && plt != null)
            {
                if (!model.IsValid())
                    return BadRequest();

                if(aabb.Length > 0) {
                    Dimensions dim = ParseAABB(aabb);
                    model.X = dim.X;
                    model.Y = dim.Z; // xeogl and system represent 3D space with Y as height axis, while system is required to represent height axis as Z.
                    model.Z = dim.Y;
                }

                Dictionary<int, double> postProcessPrices;
                _calculator.SetExcelFile(model.Quotation.EngineFile);
                double price = _calculator.GetPrice(model, plt, postProcesses, printAmount, out postProcessPrices);

                return new JsonResult(new { Price = price, PostProcesses = postProcessPrices });
            }
            
            return BadRequest();
        }

        public async Task<IActionResult> OnPostExcel(int id)
        {
            PrintModel model = await buildModel(id);
            PrinterLayerThickness plt = null;
            List<PostProcess> postProcesses = await ParsePostProcesses(postProcess);

            if (model != null && model.Printer != null && model.LayerThickness != null)
            {
                plt = await _context.PrinterLayerThicknesses.FirstOrDefaultAsync(p => p.PrinterID == model.Printer.ID && p.LayerThicknessID == model.LayerThickness.ID);
            }

            if (model != null && plt != null)
            {
                if (!model.IsValid())
                    return BadRequest();

                if (aabb.Length > 0)
                {
                    Dimensions dim = ParseAABB(aabb);
                    model.X = dim.X;
                    model.Y = dim.Z; // xeogl and system represent 3D space with Y as height axis, while system is required to represent height axis as Z.
                    model.Z = dim.Y;
                }

                _calculator.SetExcelFile(model.Quotation.EngineFile);
                IWorkbook workbook = _calculator.GenerateWorkbook(model, plt, postProcesses, printAmount);
                Stream stream = new MemoryStream();
                workbook.SaveToStream(stream, FileFormat.OpenXMLWorkbook);
                workbook.SaveAs("debug.xlsx", FileFormat.OpenXMLWorkbook);

                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", model.Name);
            }

            return BadRequest();
        }

        public async Task<IActionResult> OnGetMaterials()
        {
            Materials = await _context.Materials.Include(m => m.LayerThicknesses).ToListAsync();

            List<PrinterLayerThickness> printerLayerThicknesses = await _context.PrinterLayerThicknesses.Include(pl => pl.Printer).ToListAsync();
            List<PrinterJson> printerJsons = new List<PrinterJson>();

            foreach(PrinterLayerThickness pl in printerLayerThicknesses)
            {
                PrinterJson json = new PrinterJson();
                json.Printer = pl.Printer;
                json.LayerThicknessID = pl.LayerThicknessID;
                printerJsons.Add(json);
            }

            List<MaterialPostProcess> materialPostProcesses = await _context.MaterialPostProcesses.Include(mpp => mpp.PostProcess).ToListAsync();
            List<PostProcessJson> postProcessJsons = new List<PostProcessJson>();

            foreach(MaterialPostProcess pmpp in materialPostProcesses)
            {
                PostProcessJson json = new PostProcessJson();
                json.PostProcess = pmpp.PostProcess;
                json.MaterialID = pmpp.MaterialID;
                postProcessJsons.Add(json);
            }

            List<Industry> industries = await _context.Industries.ToListAsync();

            return new JsonResult(new { Materials, Printers = printerJsons, PostProcesses = postProcessJsons, Industries = industries });
        }

        private bool IsValid()
        {
            if (
                //(height > 0) &&
                materialOption > 0 &&   // IDs in the DB starts at 1, so if it's 0 it's not set.
                layerThicknessOption > 0 && // Same as above.
                printerOption > 0 // Same as above.
            )
                return true;
            return false;
        }

        private async Task<PrintModel> buildModel(int id)
        {
            if (!IsValid())
                return null;

            PrintModel model = await _context.PrintModels.Include(pm => pm.Quotation).Include(pm => pm.User).FirstOrDefaultAsync(pm => pm.ID == id);

            if (model != null)
            {
                model.PrintDirection = printDirection;

                if(aabb.Length > 0)
                {
                    Dimensions dim = ParseAABB(aabb);
                    model.X = dim.X;
                    model.Y = dim.Z; // xeogl and system represent 3D space with Y as height axis, while system is required to represent height axis as Z.
                    model.Z = dim.Y;
                }
                else
                {
                    _logger.LogError("AABB string empty.");
                }
                
                
                model.Material = await _context.Materials.FirstOrDefaultAsync(m => m.ID == materialOption);
                model.LayerThickness = await _context.LayerThicknesses.FirstOrDefaultAsync(l => l.ID == layerThicknessOption);
                model.Printer = await _context.Printers.FirstOrDefaultAsync(p => p.ID == printerOption);
                model.Industry = await _context.Industries.FirstOrDefaultAsync(i => i.Id == industryOption);

                return model;
            }

            return null;
        }

        private double ParsePrice(string price)
        {
            string separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            price = price.Replace(".", separator);

            System.Diagnostics.Debug.WriteLine(price);
            return Double.Parse(price);
        }

        public Dimensions ParseAABB(string str)
        {
            if (str.Length == 0) throw new Exception("AABB string empty.");

            Dimensions dim = new Dimensions();

            try
            {
                CultureInfo culture = new CultureInfo("en-US", false);
                string[] values = str.Split(",");
                double[] numbers = new double[6];

                for (int i = 0; i < values.Length; i++)
                {
                    numbers[i] = double.Parse(values[i], culture);
                }

                dim.X = numbers[3] - numbers[0];
                dim.Y = numbers[4] - numbers[1];
                dim.Z = numbers[5] - numbers[2];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return dim;
        }

        private async Task<List<PostProcess>> ParsePostProcesses(int[] ids)
        {
            List<PostProcess> result = new List<PostProcess>();
            if (ids == null) return result;

            foreach (int ppid in ids)
            {
                PostProcess p = await _context.PostProcesses.FirstOrDefaultAsync(pp => pp.ID == ppid);

                if (p != null)
                    result.Add(p);
            }

            return result;
        }

        public struct Dimensions
        {
            public double X;
            public double Y;
            public double Z;
        }

        public struct PrinterJson
        {
            public Printer Printer;
            public int LayerThicknessID;
        }

        public struct PostProcessJson
        {
            public PostProcess PostProcess;
            public int MaterialID;
        }
    }
}