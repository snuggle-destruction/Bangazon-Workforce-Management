using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models
{
    public class Computer
    {
        [Required]
        [Display(Name = "Computer Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Date Purchased")]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Date Decommissioned")]
        public DateTime DecomissionDate { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Manufacturer { get; set; }
    }
}
