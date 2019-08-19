using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models
{
    public class Employee
    {
        [Required]
        [Display(Name = "Employee Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Range(1,100, ErrorMessage = "Please select a department")]
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Is Supervisor")]
        public bool IsSupervisor { get; set; }

        public Computer Computer { get; set; }

        public List<TrainingProgram> TrainingPrograms { get; set; }

        public TrainingProgram TrainingProgram { get; set; }

        public Department Department { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
