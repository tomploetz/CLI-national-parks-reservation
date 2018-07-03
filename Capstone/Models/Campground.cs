using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
	public class Campground
	{
		private Dictionary<int, string> _months = new Dictionary<int, string>
		{
			{1, "January"},
			{2, "February" },
			{3, "March" },
			{4, "April" },
			{5, "May" },
			{6, "June" },
			{7, "July" },
			{8, "August" },
			{9, "September" },
			{10, "October" },
			{11, "November" },
			{12, "December" }
		};

        #region Properties
        public int CampgroundId { get; set; }
        public int ParkId { get; set; }
        public string Name { get; set; }
        public int OpenFromMM { get; set; }
        public int OpenToMM { get; set; }
        public decimal DailyFee { get; set; }
		public string OpenFromMMStr
		{
			get
			{
				return _months[OpenFromMM];
			}
		}
		public string OpenToMMStr
		{
			get
			{
				return _months[OpenToMM];
			}
		}
        #endregion

        #region Methods
        //Method to calculate cost of a stay based on daily rate x price:
        public decimal CalculateCost(Campground campground, int numDays)
        {
            decimal cost = campground.DailyFee * numDays;
            return cost;
        }
        #endregion
    }
}
