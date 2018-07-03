using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
	public class SiteSqlDAL
	{
        #region Variables
        private string _connectionString;
        #endregion

        #region Constructor
        // Single Parameter Constructor
        public SiteSqlDAL(string dbConnectionString)
        {
            _connectionString = dbConnectionString;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method to get all campsites from the DB and output a List
        /// </summary>
        /// <returns></returns>
        public List<Site> GetAllSites()
		{
			List<Site> output = new List<Site>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				const string sqlGetSites = "Select site_id, campground_id, site_number, max_occupancy, accessible, max_rv_length, utilities From site;";

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sqlGetSites;
				cmd.Connection = connection;

				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Site site = PopulateSitesFromReader(reader);
					output.Add(site);
				}
			}
			return output;
		}

        /// <summary>
        /// Method to get a List of Sites from a specific campground that falls within 
        /// a proper date range if that range is not already booked at a specific site.
        /// </summary>
        /// <param name="campgroundId"></param>
        /// <param name="arriveDate"></param>
        /// <param name="departDate"></param>
        /// <returns></returns>
		public List<Site> GetCampgroundSites(int campgroundId, DateTime arriveDate, DateTime departDate)
		{
			List<Site> output = new List<Site>();
			int arriveMonth = arriveDate.Month;
			int departMonth = departDate.Month;
			
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				//add joins for checking available dates
				const string sqlGetCampgroundSites = "SELECT TOP 5 * FROM site " +
													 "JOIN campground ON site.campground_id = campground.campground_id " +
													 "WHERE campground.campground_id = @campground_id AND site_id NOT IN " +
													 "(SELECT site_id FROM reservation " +
													 "WHERE from_date BETWEEN @from_date AND @to_date " +
													 "OR to_date BETWEEN @from_date AND @to_date AND " +
													 "(from_date BETWEEN @from_date AND @to_date " +
													 "AND to_date BETWEEN @from_date AND @to_date) " +
													 "OR from_date < @from_date AND to_date > @to_date);";
					
				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sqlGetCampgroundSites;
				cmd.Connection = connection;
				cmd.Parameters.AddWithValue("@campground_id", campgroundId);
				cmd.Parameters.AddWithValue("@from_date", arriveDate);
				cmd.Parameters.AddWithValue("@to_date", departDate);

				SqlDataReader reader = cmd.ExecuteReader();

				while(reader.Read())
				{
					Site site = PopulateSitesFromReader(reader);
					output.Add(site);
				}
			}
			return output;
		}
		
        /// <summary>
        /// Method to populate the Site object properties
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
		private Site PopulateSitesFromReader(SqlDataReader reader)
		{
			Site item = new Site
			{
				CampgroundId = Convert.ToInt32(reader["campground_id"]),
				SiteId = Convert.ToInt32(reader["site_id"]),
				SiteNumber = Convert.ToInt32(reader["site_number"]),
				MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]),
				Accessible = Convert.ToBoolean(reader["accessible"]),
				Utilities = Convert.ToBoolean(reader["utilities"]),
				MaxRvLength = Convert.ToInt32(reader["max_rv_length"])
			};
			return item;
		}
        #endregion
    }
}
