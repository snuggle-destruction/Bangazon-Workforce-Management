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
    public class ComputersController : Controller
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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

        // GET: Computers
        public ActionResult Index()
        {
            var computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id AS ComputerId, c.PurchaseDate, c.DecomissionDate, c.Make, c.Manufacturer, 
                            e.Id AS EmployeeId, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor
                        FROM Computer c
                        LEFT JOIN ComputerEmployee ce ON c.Id = ce.ComputerId
                        LEFT JOIN Employee e ON e.Id = ce.EmployeeId
                    ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            computer.EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));

                            var employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                            };

                            computer.Employee = employee;
                        }

                        computers.Add(computer);
                    }
                    reader.Close();
                }
            }
            return View(computers);
        }

        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {
            var computer = GetComputerById(id);
            return View(computer);
        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            var viewModel = new ComputerCreateViewModel(_config.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
        {
            string commandText;
            // if an employee has been assigned to computer, this query will add an instance to the ComputerEmployeeTable
            if (computer.EmployeeId != 0)
            {
                commandText = @"
                            DECLARE @OutputTbl TABLE (ComputerId INT)
                            DECLARE @ComputerId int;
                            
                            INSERT INTO Computer (
                                PurchaseDate,
                                Make,
                                Manufacturer
                            ) OUTPUT Inserted.Id INTO @OutputTbl(ComputerId)
                            VALUES
                            (
                                @PurchaseDate,
                                @Make,
                                @Manufacturer
                            );

                            SET @ComputerId = (SELECT ComputerId FROM @OutputTbl);

                            INSERT INTO ComputerEmployee (
                                ComputerId,
                                EmployeeId,
                                AssignDate
                            ) VALUES (
                                @ComputerId,
                                @EmployeeId,
                                @AssignDate
                            );
                        ";
            } else
            {
                commandText = @"
                                INSERT INTO Computer (
                                    PurchaseDate,
                                    Make,
                                    Manufacturer
                                )
                                VALUES
                                (
                                    @PurchaseDate,
                                    @Make,
                                    @Manufacturer
                                );
                            ";
            }

            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {

                        cmd.CommandText = commandText;

                        cmd.Parameters.AddWithValue("@PurchaseDate", computer.PurchaseDate);
                        cmd.Parameters.AddWithValue("@Make", computer.Make);
                        cmd.Parameters.AddWithValue("@Manufacturer", computer.Manufacturer);
                        cmd.Parameters.AddWithValue("@EmployeeId", computer.EmployeeId);
                        cmd.Parameters.AddWithValue("@AssignDate", DateTime.Today);

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


        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            var computer = GetComputerById(id);

            if (computer.Employee == null)
            {
                return View(computer);
            }

            return View(nameof(Index));
        }

        // POST: Computers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Computer computer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        DELETE FROM Computer
                        WHERE Id = @id
                    ";

                        cmd.Parameters.AddWithValue("@id", computer.Id);
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

        // GET: Computers By Search
        public ActionResult SearchComputers(IFormCollection search)
        {
            if(string.IsNullOrEmpty(search["SearchString"][0]))
            {
                return RedirectToAction(nameof(Index));
            }

            List<Computer> computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id AS ComputerId, c.PurchaseDate, c.DecomissionDate, c.Make, c.Manufacturer, 
                            e.Id AS EmployeeId, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor
                        FROM Computer c
                        LEFT JOIN ComputerEmployee ce ON c.Id = ce.ComputerId
                        LEFT JOIN Employee e ON e.Id = ce.EmployeeId
                        WHERE c.Make LIKE '%' + @search + '%'
                        OR c.Manufacturer LIKE '%' + @search + '%'";

                    cmd.Parameters.AddWithValue("@search", search["SearchString"][0]);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            computer.EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));

                            var employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                            };

                            computer.Employee = employee;
                        }

                        computers.Add(computer);
                    }
                    reader.Close();
                }
            }
            return View(computers);
        }

        public Computer GetComputerById(int id)
        {
            Computer computer = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer 
                        FROM Computer
                        WHERE Id = @id
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

    }
}