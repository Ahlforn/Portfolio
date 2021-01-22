using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceCalculator.Models
{
    public class PrintModelPostProcess
    {
        public int PrintModelID { get; set; }
        public PrintModel PrintModel { get; set; }
        public int PostProcessID { get; set; }
        public PostProcess PostProcess { get; set; }
    }
}
