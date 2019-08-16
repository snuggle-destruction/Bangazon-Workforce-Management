using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models.ViewModels
{
    public class EmployeeTrainingProgramViewModel
    {
        public List<SelectListItem> TrainingProgramList { get; set; }

        public TrainingProgram TrainingProgram { get; set; }
        public Employee Employee { get; set; }

    }
}
