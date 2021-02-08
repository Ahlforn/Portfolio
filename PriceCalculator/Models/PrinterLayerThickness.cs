using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceCalculator.Models
{
    public class PrinterLayerThickness
    {
        public int PrinterID { get; set; }
        public Printer Printer { get; set; }
        public int LayerThicknessID { get; set; }
        public LayerThickness LayerThickness { get; set; }
        public string ExcelDefinedName { get; set; }
    }
}
