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
    public class Material
    {
        [DataMember]
        [Key]
        public int ID { get; set; }

        [DataMember]
        [StringLength(100)]
        [Display(Name = "Material Name")]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Material Description")]
        public string Description { get; set; }

        [Display(Name = "Layer Thicknesses")]
        [DataMember]
        public ICollection<LayerThickness> LayerThicknesses { get; set; }
        [DataMember]
        public ICollection<MaterialPostProcess> MaterialPostProcesses { get; set; }
    }
}
