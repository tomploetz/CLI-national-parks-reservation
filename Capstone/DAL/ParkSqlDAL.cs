using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
	public class ParkSqlDAL
	{
        #region Variables
        private string _connectionString;
        #endregion

        #region Constructor
        // Single Parameter Constructor
        public ParkSqlDAL(string dbConnectionString)
        {
            _connectionString = dbConnectionString;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method to get all parks from the SQL DB
        /// </summary>
        /// <returns>a list of Park objects</returns>
        public List<Park> GetAllParks()
		{
			List<Park> output = new List<Park>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				const string sqlGetParks = "Select name, location, establish_date, area, visitors, description, park_id From park ORDER BY name ASC;";

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sqlGetParks;
				cmd.Connection = connection;

				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Park park = PopulateParksFromReader(reader);
					output.Add(park);
				}
			}
				return output;
		}

        /// <summary>
        /// Method to get a specific park from SQL DB based on parkID
        /// </summary>
        /// <param name="parkId"></param>
        /// <returns>Park object</returns>
		public Park GetPark(int parkId)
		{
			Park result = new Park();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				const string sqlGetPark = "SELECT name, location, establish_date, area, visitors, description, park_id FROM park WHERE park_id = @park_id;";

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sqlGetPark;
				cmd.Connection = connection;
				cmd.Parameters.AddWithValue("@park_id", parkId);

				SqlDataReader reader = cmd.ExecuteReader();

				while(reader.Read())
				{
					Park park = PopulateParksFromReader(reader);
					result = park;
				}
			}
			return result;
		}

        /// <summary>
        /// Method which loads the Park Object properties, used in the GetPark and GetAllParks methods above
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
		private Park PopulateParksFromReader(SqlDataReader reader)
		{
			Park item = new Park
			{
				ParkId = Convert.ToInt32(reader["park_id"]),
				Name = Convert.ToString(reader["name"]),
				Location = Convert.ToString(reader["location"]),
				EstablishDate = Convert.ToDateTime(reader["establish_date"]),
				Area = Convert.ToInt32(reader["area"]),
				Visitors = Convert.ToInt32(reader["visitors"]),
				Description = Convert.ToString(reader["description"])
			};
			return item;
		}
        #endregion
    }
}
