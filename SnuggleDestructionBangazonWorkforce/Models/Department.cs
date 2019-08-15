using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public int AssignedEmployees { get; set; }
    }
}
