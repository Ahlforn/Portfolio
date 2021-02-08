using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PriceCalculator.Models
{
    [DataContract]
    public class Industry
    {
        [DataMember]
        [Key]
        public int Id { get; set; }
        [DataMember]
        [Display(Name = "Industry Name")]
        public string Name { get; set; }
    }
}
