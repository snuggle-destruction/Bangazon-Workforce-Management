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
    public class TrainingProgramsController : Controller
    {
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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
        // GET: TrainingPrograms
        public ActionResult Index()
        {
            List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], StartDate, EndDate, MaxAttendees
                        FROM TrainingProgram
                        WHERE StartDate > CURRENT_TIMESTAMP
                        ";

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

            return View(trainingPrograms);
        }

        // GET: TrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            var trainingProgram = GetSingleTrainingProgram(id);
            var employees = GetAllEmployeesInProgram(id);
            var viewModel = new TrainingProgramDetailsViewModel(trainingProgram, employees);

            return View(viewModel);
        }

        // GET: TrainingPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees)
                            VALUES (@name, @startDate, @endDate, @maxAttendees);
                        ";

                        cmd.Parameters.AddWithValue("@name", trainingProgram.Name);
                        cmd.Parameters.AddWithValue("@startDate", trainingProgram.StartDate);
                        cmd.Parameters.AddWithValue("@endDate", trainingProgram.EndDate);
                        cmd.Parameters.AddWithValue("@maxAttendees", trainingProgram.MaxAttendees);
                        
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

        // GET: TrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {
            var trainingProgram = GetSingleTrainingProgram(id);
            DateTime currentDate = DateTime.Now;

            if (trainingProgram.StartDate > currentDate)
            {
                return View(trainingProgram);
            }
            else
            {
                throw new Exception("NOPE!");
            }

            
        }

        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                            UPDATE TrainingProgram
                                            SET 
                                            [Name] = @name,
                                            StartDate = @startDate,
                                            EndDate = @endDate,
                                            MaxAttendees = @maxAttendees
                                            WHERE Id = @id
                                            ";

                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@name", trainingProgram.Name);
                        cmd.Parameters.AddWithValue("@startDate", trainingProgram.StartDate);
                        cmd.Parameters.AddWithValue("@endDate", trainingProgram.EndDate);
                        cmd.Parameters.AddWithValue("@maxAttendees", trainingProgram.MaxAttendees);

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingPrograms/Delete/5
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

        public TrainingProgram GetSingleTrainingProgram(int id)
        {
            TrainingProgram trainingProgram = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], StartDate, EndDate, MaxAttendees
                        FROM TrainingProgram
                        WHERE Id = @id
                    ";
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }
                    reader.Close();
                }
            }
            return trainingProgram;
        }

        private List<Employee> GetAllEmployeesInProgram(int id)
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT e.Id, e.FirstName, e.LastName FROM Employee e
                                        JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                                        JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                        WHERE tp.Id = 5
                                        ORDER BY e.Id ASC;
                                        ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employees.Add(new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        });
                    }

                    reader.Close();
                }
            }
            return employees;
        }
    }
}