using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
	public class CampgroundSqlDAL
	{
        #region Variables
        private string _connectionString;
        #endregion

        #region Constructor
        // Single Parameter Constructor
        public CampgroundSqlDAL(string dbConnectionString)
        {
            _connectionString = dbConnectionString;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method to read in all Campgrounds from the SQL DB
        /// and load into a List
        /// </summary>
        /// <returns> </returns>
        public List<Campground> GetAllCampgrounds()
		{
			List<Campground> output = new List<Campground>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				const string sqlGetCampgrounds = "Select name, campground_id, open_from_mm, open_to_mm, daily_fee, park_id From campground;";

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sqlGetCampgrounds;
				cmd.Connection = connection;

				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Campground campground = PopulateCampgroundsFromReader(reader);
					output.Add(campground);
				}
			}
			return output;
		}

        /// <summary>
        /// Method to get a specific LIST of Campgrounds from the SQL DB
        /// which loads campgrounds based on a certain parkID input parameter
        /// </summary>
        /// <param name="parkId"></param>
        /// <returns></returns>
		public List<Campground> GetParkCampgrounds(int parkId)
		{
			List<Campground> result = new List<Campground>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				const string sqlGetParkCampgrounds = "SELECT name, open_from_mm, open_to_mm, daily_fee, park_id, campground_id FROM campground WHERE park_id = @park_id;";

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sqlGetParkCampgrounds;
				cmd.Connection = connection;
				cmd.Parameters.AddWithValue("@park_id", parkId);

				SqlDataReader reader = cmd.ExecuteReader();

				while(reader.Read())
				{
					Campground campground = PopulateCampgroundsFromReader(reader);
					result.Add(campground);
				}
			}
			return result;
		}

        /// <summary>
        /// Method called in our above methods to load our Campground object properties
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
		private Campground PopulateCampgroundsFromReader(SqlDataReader reader)
		{
			Campground item = new Campground
			{
				CampgroundId = Convert.ToInt32(reader["campground_id"]),
				ParkId = Convert.ToInt32(reader["park_id"]),
				OpenFromMM = Convert.ToInt32(reader["open_from_mm"]),
				OpenToMM = Convert.ToInt32(reader["open_to_mm"]),
				Name = Convert.ToString(reader["name"]),
				DailyFee = Convert.ToDecimal(reader["daily_fee"])
			};
			return item;
		}
        #endregion
    }
}
