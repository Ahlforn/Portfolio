using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PriceCalculator.Models
{
    public class Quotation
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Quotation Number")]
        public string Title { get; set; }
        public SiteUser User { get; set; }
        public ICollection<PrintModel> PrintModels { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
        public string Customer { get; set; }
        [Display(Name = "Contact Person")]
        public string DeliveryAddress { get; set; }
        public string EngineFile { get; set; }
    }
}
