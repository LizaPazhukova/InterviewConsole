using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using EmployeeService.Models;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IEmployeeService
    {
        const string connectionString = "Data Source=(local);Initial Catalog=Test;User ID=sa;Password=pass@word1;";
        public Employee GetEmployeeById(int id)
        {
            Employee employee = null;
            string query = "SELECT * FROM Employee WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = reader["Name"].ToString(),
                            Enable = Convert.ToBoolean(reader["Enable"]),
                            ManagerID = reader["ManagerID"] != DBNull.Value ? Convert.ToInt32(reader["ManagerID"]) : (int?)null,
                        };
                    }

                    reader.Close();
                }
            }

            if (employee != null)
            {
                employee.Employees = GetEmployees(employee.ID);
            }

            return employee;
        }

        public void EnableEmployee(int id, int enable)
        {
            string query = "UPDATE Employee SET Enable = @EnableStatus WHERE ID = @EmployeeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EnableStatus", enable);
                    command.Parameters.AddWithValue("@EmployeeID", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private List<Employee> GetEmployees(int managerID)
        {
            List<Employee> employees = new List<Employee>();
            string query = "SELECT * FROM Employee WHERE ManagerID = @ManagerID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ManagerID", managerID);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = reader["Name"].ToString(),
                            Enable = Convert.ToBoolean(reader["Enable"]),
                            ManagerID = Convert.ToInt32(reader["ManagerID"])
                        };

                        employee.Employees = GetEmployees(employee.ID);

                        employees.Add(employee);
                    }

                    reader.Close();
                }
            }

            return employees;
        }
    }
}
