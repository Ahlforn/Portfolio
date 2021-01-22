using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceCalculator.Models
{
    // Add profile data for application users by adding properties to the SiteUser class
    public class SiteUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public string Title { get; set; }
        [PersonalData]
        public string Department { get; set; }
        [PersonalData]
        public int? PhoneCountryCode { get; set; }
    }
}
    