using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SnuggleDestructionBangazonWorkforce.Models;
using SnuggleDestructionBangazonWorkforce.Models.ViewModels;

namespace SnuggleDestructionBangazonWorkforce.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
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
        // GET: Departments
        public ActionResult Index()
        {
            List<Department> departments = new List<Department>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT d.Id, d.Name, d.Budget, COUNT(e.DepartmentId) AS DepartmentSize
                                          FROM Department AS d
                                          LEFT JOIN Employee AS e ON d.Id = e.DepartmentId
                                          GROUP BY d.Id, d.Name, d.Budget ";

                    SqlDataReader reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        departments.Add(new Department()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            DepartmentSize = reader.GetInt32(reader.GetOrdinal("DepartmentSize"))
                        });
                    }

                    reader.Close();
                }
            }

            return View(departments);
        }

        // GET: Departments/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, d.[Name], d.[Budget], e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor
                                        FROM Department AS d
                                        LEFT JOIN Employee AS e ON e.DepartmentId = d.Id
                                        Where d.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();
                    Department department = null;
                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                        {
                            employees.Add(new Employee()
                            {
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                            });
                        }
                    }
                    department.Employees = employees;
                    return View(department);
                }
            }
        }


        public ActionResult Create()
        {
            return View();
        }
        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO Department ([Name], Budget)
                            VALUES (@name, @budget);
                        ";

                        cmd.Parameters.AddWithValue("@name", department.Name);
                        cmd.Parameters.AddWithValue("@budget", department.Budget);
                        

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

        private Department GetOneDepartment(int id)
        {
            using (SqlConnection conn = Connection)
            {
                Department department = null;
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, Budget
                        FROM Department
                        WHERE Id = @id
                    ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        department = new Department()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                           
                        };
                    }
                }
                return department;
            }
        }
    }
}