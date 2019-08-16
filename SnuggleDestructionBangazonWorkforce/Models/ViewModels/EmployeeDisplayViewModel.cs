using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models.ViewModels
{
    public class EmployeeDisplayViewModel
    {
        public Employee Employee { get; set; }

        public Computer Computer { get; set; }

        public List<TrainingProgram> TrainingPrograms { get; set; }

        public TrainingProgram TrainingProgram { get; set; }

        public Department Department { get; set; }
    }
}
