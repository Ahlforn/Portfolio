using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PriceCalculator.Models
{
    [DataContract]
    public class Printer
    {
        [DataMember]
        [Key]
        public int ID { get; set; }

        [DataMember]
        [Display(Name = "Printer Name")]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Width")]
        public double W { get; set; }

        [DataMember]
        [Display(Name = "Height")]
        public double H { get; set; }

        [DataMember]
        [Display(Name = "Depth")]
        public double D { get; set; }

        public ICollection<PrinterLayerThickness> PrinterLayerThicknesses { get; set; }
    }
}
