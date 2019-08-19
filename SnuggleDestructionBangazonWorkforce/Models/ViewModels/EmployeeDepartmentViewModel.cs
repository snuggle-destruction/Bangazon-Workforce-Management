using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models.ViewModels
{
    public class EmployeeDepartmentViewModel
    {
        public List<Employee> Employees { get; set; }
        public List<Department> Departments { get; set; }
    }
}
