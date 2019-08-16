using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models.ViewModels
{
    public class DepartmentDisplayViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Budget { get; set; }

        public int DepartmentSize { get; set; }
    }
}
