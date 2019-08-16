using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SnuggleDestructionBangazonWorkforce.Models;

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
            List<Computer> computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer 
                        FROM Computer
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
            return View();
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO Computer (
                                PurchaseDate,
                                Make,
                                Manufacturer
                            ) VALUES (
                                @PurchaseDate,
                                @Make,
                                @Manufacturer
                            )
                        ";

                        cmd.Parameters.AddWithValue("@PurchaseDate", computer.PurchaseDate);
                        cmd.Parameters.AddWithValue("@Make", computer.Make);
                        cmd.Parameters.AddWithValue("@Manufacturer", computer.Manufacturer);

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

        // GET: Computers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

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
            return View();
        }

        // POST: Computers/Delete/5
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

        // GET: Computers By Search
        public ActionResult SearchComputers(IFormCollection search)
        {
            List<Computer> computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer 
                                        FROM Computer
                                        WHERE Make LIKE '%' + @search + '%'
                                        OR Manufacturer LIKE '%' + @search + '%'";

                    cmd.Parameters.AddWithValue("@search", search["SearchString"][0]);
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