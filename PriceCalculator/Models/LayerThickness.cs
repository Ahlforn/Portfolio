using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace PriceCalculator.Models
{
    [DataContract]
    public class LayerThickness
    {
        [DataMember]
        [Key]
        public int ID { get; set; }

        [DataMember]
        public double Thickness { get; set; }

        [DataMember]
        public string Unit { get; set; }

        [Display(Name = "Excel Defined Name")]
        public string ExcelDefinedName { get; set; }

        public int MaterialID { get; set; }

        public Material Material { get; set; }

        public ICollection<PrinterLayerThickness> PrinterLayerThicknesses { get; set; }
    }
}
