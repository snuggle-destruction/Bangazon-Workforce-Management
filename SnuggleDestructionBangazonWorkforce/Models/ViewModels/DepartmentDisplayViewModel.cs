using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models.ViewModels
{
    public class DepartmentDisplayViewModel
    {
        [Required]
        [Display(Name = "Department Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Budget")]
        public int Budget { get; set; }

        [Required]
        [Display(Name = "Number of Employees")]
        public int DepartmentSize { get; set; }
    }
}
