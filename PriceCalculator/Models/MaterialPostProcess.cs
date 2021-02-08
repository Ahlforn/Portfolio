using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceCalculator.Models
{
    public class MaterialPostProcess
    {
        public int MaterialID { get; set; }
        [JsonIgnore]
        public Material Material { get; set; }
        public int PostProcessID { get; set; }
        public PostProcess PostProcess { get; set; }
    }
}
