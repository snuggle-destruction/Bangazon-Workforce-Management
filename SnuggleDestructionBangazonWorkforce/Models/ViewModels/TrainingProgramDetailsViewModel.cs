using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models.ViewModels
{
    public class TrainingProgramDetailsViewModel
    {
        public TrainingProgram TrainingProgram { get; set; }
        public Employee Employee { get; set; }
        public List<Employee> AttendingEmployees { get; set; }

        public TrainingProgramDetailsViewModel() { }

        public TrainingProgramDetailsViewModel(TrainingProgram trainingProgram, List<Employee> employees)
        {
            TrainingProgram = trainingProgram;
            AttendingEmployees = employees;
        }
    }
}