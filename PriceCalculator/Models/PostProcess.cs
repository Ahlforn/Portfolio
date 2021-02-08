using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PriceCalculator.Models
{
    [DataContract]
    public class PostProcess
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        public string ExcelDefinedName { get; set; }
        public string ExcelDefinedPriceName { get; set; }
        public ICollection<MaterialPostProcess> MaterialPostProcesses { get; set; }
        public ICollection<PrintModelPostProcess> PrintModelPostProcesses { get; set; }
    }
}
