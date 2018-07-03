using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
	public class ReservationSqlDAL
	{
        #region Variables
        private string _connectionString;
        #endregion

        #region Constructor
        // Single Parameter Constructor
        public ReservationSqlDAL(string dbConnectionString)
        {
            _connectionString = dbConnectionString;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get all Reservations from SQL DB
        /// </summary>
        /// <returns></returns>
        public List<Reservation> GetAllReservations()
		{
			List<Reservation> output = new List<Reservation>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				const string sqlGetReservations = "Select name, from_date, to_date, create_date, reservation_id, site_id From reservation;";

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sqlGetReservations;
				cmd.Connection = connection;

				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Reservation reservation = PopulateReservationsFromReader(reader);
					output.Add(reservation);
				}
			}
			return output;
		}

        /// <summary>
        /// Method to book a reservation and INSERT the values into the SQL DB, and return 
        /// the Reservation ID - ** later to add ability to return all details of the Reservation
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="arriveDate"></param>
        /// <param name="departDate"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public int BookReservation(int siteId, DateTime arriveDate, DateTime departDate, string resName) 
		{
			//Reservation newRes = new...
			DateTime timeStamp = DateTime.Now;

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				const string sqlMakeReservation = "INSERT INTO reservation (site_id, name, from_date, to_date, create_date)" +
												  "VALUES (@site_id, @name, @from_date, @to_date, @create_date);" +
												  "SELECT CAST(scope_identity() AS int);";

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sqlMakeReservation;
				cmd.Connection = connection;
				cmd.Parameters.AddWithValue("@site_id", siteId);
				cmd.Parameters.AddWithValue("@name", resName);
				cmd.Parameters.AddWithValue("@from_date", arriveDate);
				cmd.Parameters.AddWithValue("@to_date", departDate);
				cmd.Parameters.AddWithValue("@create_date", timeStamp);

				//int rowsAffected = cmd.ExecuteNonQuery();
				int newResId = (int)cmd.ExecuteScalar();

				//create a reservation object, populate, and return
				return newResId;
			}
		}

        /// <summary>
        /// Populates the property values of the Reservation object
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
		private Reservation PopulateReservationsFromReader(SqlDataReader reader)
		{
			Reservation item = new Reservation
			{
				ReservationId = Convert.ToInt32(reader["reservation_id"]),
				SiteId = Convert.ToInt32(reader["site_id"]),
				Name = Convert.ToString(reader["name"]),
				FromDate = Convert.ToDateTime(reader["from_date"]),
				ToDate = Convert.ToDateTime(reader["to_date"])
            };
            if (!reader.IsDBNull(reader.GetOrdinal("create_date")))
            {
                item.CreateDate = Convert.ToDateTime(reader["create_date"]);
            }
			return item;
		}
        #endregion
    }
}
