using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SnuggleDestructionBangazonWorkforce.Models;
using SnuggleDestructionBangazonWorkforce.Models.ViewModels;

namespace SnuggleDestructionBangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Employees
        public ActionResult Index()
        {


            List<Employee> employees = new List<Employee>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor, d.Name, d.Id As DeptId, d.Budget
                        FROM Employee e
                        LEFT JOIN Department d ON d.Id = e.DepartmentId
                        ";

                    SqlDataReader reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        var employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                        };
                        var department = new Department()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("DeptId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                        };
                        employee.Department = department;
                        employees.Add(employee);
                    }



                    reader.Close();
                }
            }

            return View(employees);
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {


            var employee = GetOneEmplyee(id);
            var computer = GetComputer(id);
            var trainingPrograms = GetTrainingPrograms(id);
            var department = GetDepartment(id);
            employee.Computer = computer;
            employee.TrainingPrograms = trainingPrograms;
            employee.Department = department;

            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var viewModel = new EmployeeCreateViewModel();
            var departments = GetDepartments();
            var selectItems = departments
                .Select(department => new SelectListItem
                {
                    Text = department.Name,
                    Value = department.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose cohort...",
                Value = "0"
            });
            viewModel.Departments = selectItems;

            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                   INSERT INTO Employee (
                                   FirstName,
                                   LastName,
                                   IsSupervisor,
                                   DepartmentId
                                  )  Values (
                                    @firstName,
                                    @lastName,
                                    @isSupervisor,
                                    @departmentId
                                   )
                                ";

                        cmd.Parameters.AddWithValue("@firstName", employee.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", employee.LastName);
                        cmd.Parameters.AddWithValue("@isSupervisor", employee.IsSupervisor);
                        cmd.Parameters.AddWithValue("@departmentId", employee.DepartmentId);

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            var viewModel = new EmployeeEditViewModel();
            var employee = GetOneEmplyee(id);
            var departments = GetDepartments();
            var eComputer = GetComputer(id);
            var deptSelectItems = departments
                .Select(department => new SelectListItem
                {
                    Text = department.Name,
                    Value = department.Id.ToString()
                })
                .ToList();

            deptSelectItems.Insert(0, new SelectListItem
            {
                Text = "Choose department...",
                Value = "0"
            });
            var computers = GetComputers();
            var compSelectItems = computers
                .Select(computer => new SelectListItem
                {
                    Text = computer.Make,
                    Value = computer.Id.ToString()
                })
                .ToList();

            compSelectItems.Insert(0, new SelectListItem
            {
                Text = "Choose computer...",
                Value = "0"
            });

            viewModel.Computer = eComputer;
            viewModel.Employee = employee;
            viewModel.Departments = deptSelectItems;
            viewModel.Computers = compSelectItems;

            return View(viewModel);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                   UPDATE Employee 
                                    SET LastName = @lastName,
                                        DepartmentId = @departmentId
                                    WHERE Id = @id;

                                    UPDATE ComputerEmployee
                                    Set EmployeeId = @id,
                                        ComputerId = @computerId,
                                        AssignDate = GETDATE(),
                                        UnassignDate = null
                                    WHERE EmployeeId = @id
                                    ";



                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@lastName", model.Employee.LastName);
                        cmd.Parameters.AddWithValue("@departmentId", model.Employee.DepartmentId);
                        cmd.Parameters.AddWithValue("@computerId", model.Computer.Id);

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // Get Training Programs
        public ActionResult TrainingProgramsForm(int id)
        {
            var viewModel = new EmployeeTrainingProgramViewModel();
            var employee = GetOneEmplyee(id);
            var department = GetDepartment(id);
            viewModel.Employee = employee;
            viewModel.Department = department;
            viewModel.TrainingProgramList = trainingList(id);
            return View(viewModel);
        }

        // Assign Training Program to Employee
        // move to training program controller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTrainingProgram(Employee employee, TrainingProgram trainingProgram)
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"IF @trainingProgramId > 0
                                        BEGIN

                                        INSERT INTO EmployeeTraining (TrainingProgramId, EmployeeId)
                                        Values(@trainingProgramId, @employeeId)

                                        END";

                    //shelley work here
                    cmd.Parameters.Add(new SqlParameter("@trainingProgramId", trainingProgram.Id));
                    cmd.Parameters.Add(new SqlParameter("@employeeId", employee.Id));

                    cmd.ExecuteNonQuery();

                }
            }

            return RedirectToAction("details", new { id = employee.Id });
        }

        private Employee GetOneEmplyee(int id)
        {
            Employee employee = null;

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, DepartmentId, IsSupervisor
                        FROM Employee
                        WHERE Id = @id
                        ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();



                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                        };
                    }

                    reader.Close();
                }
            }

            return (employee);
        }

        private Computer GetComputer(int id)
        {
            Computer computer = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id, c.DecomissionDate, c.Make, c.Manufacturer, c.PurchaseDate
                        FROM ComputerEmployee ce
                        LEFT JOIN Computer c ON c.Id = ce.ComputerId 
                        WHERE ce.EmployeeId = @id
                    ";
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }
                    }
                    reader.Close();
                }
            }
            return computer;
        }

        private List<TrainingProgram> GetTrainingPrograms(int id)
        {
            List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT tp.Id, tp.EndDate, tp.MaxAttendees, tp.Name, tp.StartDate 
                        FROM EmployeeTraining et
                        LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                        WHERE et.EmployeeId = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        trainingPrograms.Add(new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        });
                    }

                    reader.Close();
                }
            }

            return (trainingPrograms);
        }

        private Department GetDepartment(int id)
        {
            Department department = null;

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT d.Id, d.Name, d.Budget
                        FROM Department d
                        LEFT JOIN Employee e ON e.DepartmentId = d.Id
                        WHERE e.id = @id
                        ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();



                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                        };
                    }

                    reader.Close();
                }
            }

            return (department);
        }

        private List<Department> GetDepartments()
        {
            List<Department> departments = new List<Department>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Budget
                        FROM Department
                        ";

                    SqlDataReader reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        departments.Add(new Department()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        });
                    }

                    reader.Close();
                }
            }

            return (departments);
        }

        private List<Computer> GetComputers()
        {
            List<Computer> computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer 
                            FROM Computer
                            WHERE Id NOT IN (
                                SELECT ComputerId
                                FROM ComputerEmployee
                            )
                    ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }

                        computers.Add(computer);

                    }
                    reader.Close();
                }
            }
            return (computers);
        }

        private List<TrainingProgram> GetAllTrainingPrograms(int employeeId = 0)
        {
            List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, tp.EndDate, tp.MaxAttendees, tp.Name, tp.StartDate 
                                        FROM TrainingProgram tp
                                        WHERE (
                                        SELECT COUNT(1) 
                                        FROM EmployeeTraining et
                                        WHERE et.TrainingProgramId = tp.Id) < tp.MaxAttendees
                                        AND tp.StartDate > GetDate()";
                    if (employeeId > 0)
                    {
                        cmd.CommandText += @" AND tp.Id NOT IN (
                                           SELECT TrainingProgramId
                                           FROM EmployeeTraining
                                           WHERE EmployeeId = @employeeId)";
                        cmd.Parameters.AddWithValue("@employeeid", employeeId);
                    }

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        trainingPrograms.Add(new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        });
                    }

                    reader.Close();
                }
            }

            return (trainingPrograms);
        }


        private List<SelectListItem> trainingList(int employeeId = 0)
        {
            var trainingProgram = GetAllTrainingPrograms(employeeId);
            var selectItems = trainingProgram
                .Select(program => new SelectListItem
                {
                    Text = program.Name,
                    Value = program.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose training program...",
                Value = "0"
            });
            return selectItems;
        }
    }
}