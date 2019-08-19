using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SnuggleDestructionBangazonWorkforce.Models.ViewModels
{
    public class ComputerCreateViewModel
    {
        public List<SelectListItem> Employees { get; set; }
        public Computer Computer { get; set; }

        private readonly string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public ComputerCreateViewModel() { }

        public ComputerCreateViewModel(string connectionString)
        {
            _connectionString = connectionString;

            Employees = GetAllEmployees()
                .Select(employee => new SelectListItem
                {
                    Text = employee.FullName,
                    Value = employee.Id.ToString()
                })
                .ToList();

            Employees.Insert(0, new SelectListItem
            {
                Text = "Assign employee...",
                Value = "0"
            });
        }

        private List<Employee> GetAllEmployees()
        {
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

                    List<Employee> employees = new List<Employee>();
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

                        employees.Add(employee);
                    }

                    reader.Close();

                    return employees;
                }
            }
        }
    }
}
