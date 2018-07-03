using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;
using Capstone.DAL;


namespace Capstone
{
    public class NationalParkCLI
    {
		#region Display & String Variables for CLI & DB Connection
		const string DatabaseConnection = @"Data Source=.\SQLEXPRESS;Initial Catalog=CampgroundReservations;Integrated Security = True";
        const string Command_Quit = "q";
        const string siteNo = "Site No.";
        const string maxOcc = "Max Occup.";
        const string access = "Accessible?";
        const string rvLngth = "Max RV Length";
        const string utility = "Utility";
        const string cost = "Cost";
        const string campId = "CampID";
        const string name = "Campground Name";
        const string opnMnth = "Open Month";
        const string closeMnth = "Close Month";
        const string dailyFee = "Daily Fee";
        #endregion

        #region Dictionary variables for Park & Campground
        Dictionary<int, Park> _parks = new Dictionary<int, Park>();
        Dictionary<int, Campground> _campgrounds = new Dictionary<int, Campground>();
        #endregion
        
        #region Display & Setup Methods
        //Initial Run CLI method called at start of program
        public void RunCLI()
        {
            PopulateParksDictionary();
            PopulateCampgroundDictionary();
            Header();
            MainMenu();
        }

        //ASCII header:
        public void Header()
        {
            Console.WriteLine(@"    _   __      __  _                   __   ____             __           ____                                  __  _           ");
            Console.WriteLine(@"   / | / /___ _/ /_(_)___  ____  ____ _/ /  / __ \____ ______/ /_______   / __ \___  ________  ______   ______ _/ /_(_)___  ____ ");
            Console.WriteLine(@"  /  |/ / __ `/ __/ / __ \/ __ \/ __ `/ /  / /_/ / __ `/ ___/ //_/ ___/  / /_/ / _ \/ ___/ _ \/ ___/ | / / __ `/ __/ / __ \/ __ \");
            Console.WriteLine(@" / /|  / /_/ / /_/ / /_/ / / / / /_/ / /  / ____/ /_/ / /  / ,< (__  )  / _, _/  __(__  )  __/ /   | |/ / /_/ / /_/ / /_/ / / / /");
            Console.WriteLine(@"/_/ |_/\__,_/\__/_/\____/_/ /_/\__,_/_/  /_/    \__,_/_/  /_/|_/____/  /_/ |_|\___/____/\___/_/    |___/\__,_/\__/_/\____/_/ /_/ ");
            Console.WriteLine();
        }

        //Main Great Parks Display screen, scales to any number of Parks from DB
        // Displays the Main park names to begin reservation search
        private void PrintMainMenu()
        {
            Header();
            Console.WriteLine();
            Console.WriteLine("* * * Select a Park for Further Details * * *");
            Console.WriteLine();
            for (int i = 0; i < _parks.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {_parks[i].Name}");
            }
			Console.WriteLine("Q) to Quit");
			Console.WriteLine();            
        }

		//Method to collect user input after the Main Menu displays
        public void MainMenu()
        { 
            bool timeToExit = false;
			//we realize this is messy, will come back to try and clean up
			while (!timeToExit)
            {
				Console.Clear();
				PrintMainMenu();

				string command = CLIHelper.GetString("Please select a Park for more info.");
				int commandInt;

				if (int.TryParse(command, out commandInt))
				{
					if (commandInt < 1 || commandInt > _parks.Count)
					{
						Console.WriteLine();
						Console.WriteLine("Invalid entry, please try again.");
					}
					else
					{
						Console.Clear();
						Header();
						ParkInformationMenu(_parks[commandInt - 1]);
						
					}
				}
				else if(command.ToLower() == "q")
				{
					timeToExit = true;
				}
				else
				{
					Console.WriteLine("Invalid entry, please try again.");
				}
			}
		}

		//this method loads the dictionary for Park details in CLI
		public void PopulateParksDictionary()
        {
            ParkSqlDAL parkDal = new ParkSqlDAL(DatabaseConnection);
            List<Park> parkList = parkDal.GetAllParks();

            for (int i = 0; i < parkList.Count; i++)
            {
                _parks.Add(i, parkList[i]);
            }
        }

        //this method loads the Campground Dictionary for use in CLI
        public void PopulateCampgroundDictionary()
        {
            CampgroundSqlDAL campDal = new CampgroundSqlDAL(DatabaseConnection);
            List<Campground> campList = campDal.GetAllCampgrounds();

            for (int i = 0; i < campList.Count; i++)
            {
                _campgrounds.Add(i, campList[i]);
            }
        }
		#endregion

		#region Methods for Park Menu & Selections
		/// <summary>
		/// This Method Displays the General Park Description and Info
		/// Then calls park Selection Method to get user input
		/// </summary>
		/// <param name="park"></param>
		public void PrintParkInformation(Park park)
		{
			Console.Clear();
			Header();
			Console.WriteLine("* * * Parks Information Screen * * * ");
			Console.WriteLine();
			Console.WriteLine($"{park.Name} National Park");
			Console.WriteLine($"Location: \t \t {park.Location}");
			Console.WriteLine($"Established: \t \t {park.EstablishDate.ToShortDateString()}");
			Console.WriteLine($"Area: \t \t \t {park.Area.ToString("N0")} sq km");
			Console.WriteLine($"Annual Visitors: \t {park.Visitors.ToString("N0")}");
			Console.WriteLine();
			Console.WriteLine($"{park.Description}");
			Console.WriteLine();
			Console.WriteLine("Select a Command");
			Console.WriteLine();
			Console.WriteLine("1) View Campgrounds");
			Console.WriteLine("2) Return to Previous Screen");
			Console.WriteLine();
		}

		public void ParkInformationMenu(Park park)
		{ 
			bool timeToExit = false;
			while(!timeToExit)
			{
				PrintParkInformation(park);
				int option = CLIHelper.GetInteger("Make your Selection from the above choice:");

				if (option < 1 || option > 2)
				{
					Console.WriteLine("Please enter a valid option.");
				}
				else if (option == 1)
				{
					CampgroundInformation(park.ParkId);
				}
				else if (option == 2)
				{
					timeToExit = true;
				}
				else
				{
					Console.WriteLine("Please enter a valid option.");
				}
			}
        }

        #endregion

        #region Methods for Campground Menu and Selections
        /// <summary>
        /// This displays the Campground options for a given Park, selected by user
        /// </summary>
        /// <param name="parkId"></param>
        public List<int> ParkCampgrounds(int parkId)
		{
			Console.Clear();
			Header();
			CampgroundSqlDAL campgroundDal = new CampgroundSqlDAL(DatabaseConnection);
			List<Campground> cmpgrds = campgroundDal.GetParkCampgrounds(parkId);
			List<int> campgroundIds = new List<int>();

			Console.WriteLine("Park Campgrounds");
			Console.WriteLine();
			Console.WriteLine($"{campId,7} | {name,19} | {opnMnth,10} | {closeMnth,10} | {dailyFee,9}");
			for(int i = 0; i < cmpgrds.Count; i++)
			{
				campgroundIds.Add(cmpgrds[i].CampgroundId);
				Console.WriteLine($"#{cmpgrds[i].CampgroundId,6} | {cmpgrds[i].Name,-19} | {cmpgrds[i].OpenFromMMStr,10} | {cmpgrds[i].OpenToMMStr,11} | {cmpgrds[i].DailyFee.ToString("c"),9}");
			} // Iterates through the List of Campgrounds to display each to user
            Console.WriteLine();

			return campgroundIds;
        }
        
        /// <summary>
        /// Method to give user the option to see available sites at their selected Campground
        /// </summary>
        /// <param name="parkId"></param>
        public void CampgroundInformation(int parkId)
		{
			ParkCampgrounds(parkId);

            bool timeToExit = false;
            while (!timeToExit)
            {
                Console.WriteLine();
                Console.WriteLine("1) Search for availability & book your reservation");
                Console.WriteLine("2) Return to Previous Screen");
                Console.WriteLine();
                int command = CLIHelper.GetInteger("Press (1) to Reserve, or (2) to go back:");

                if (command == 1)
                {
					timeToExit = true;
                    CampgroundReservation(parkId);
                }
                else if(command == 2)
                {
					timeToExit = true;
                }
				else
				{
					Console.WriteLine("Please enter a valid selection.");
				}
            }
		}

		/// <summary>
		/// Method gives user the options to Reserve a campsite and enter dates
		/// It will return an error if they try to book a site that already has an existing booking
		/// of any duration of overlapping days
		/// </summary>
		/// <param name="parkId"></param>
		public void CampgroundReservation(int parkId)
		{
			Console.Clear();
			Header();
			Console.WriteLine("Search for Campground Reservation");

			List<int> campgroundIds = ParkCampgrounds(parkId);
			
			bool timeToExit = false;
			while (!timeToExit)
			{
				int campId = CLIHelper.GetInteger("Which campground (enter 0 to cancel)?");

				if (campId == 0)
				{
					timeToExit = true;
					return;
				}
				if (campgroundIds.Contains(campId))
				{
					DateTime arriveDate = CLIHelper.GetDate("What is the arrival date (YYYY/MM/DD)? ");
					DateTime departDate = CLIHelper.GetDate("What is the departure date (YYYY/MM/DD)? ");
					timeToExit = true;
					DisplayAvailableSites(campId, arriveDate, departDate);
				}
				else
				{
					Console.WriteLine("Please enter a valid option.");
				}
			}
		}
        #endregion

        #region Method to Display Campsites
        /// <summary>
        /// Displays available campsites within a campground based on the user's selection and options
        /// </summary>
        /// <param name="campId"></param>
        /// <param name="arriveDate"></param>
        /// <param name="departDate"></param>
        public void DisplayAvailableSites(int campId, DateTime arriveDate, DateTime departDate)
		{
			SiteSqlDAL siteDal = new SiteSqlDAL(DatabaseConnection);
			List<Site> sites = siteDal.GetCampgroundSites(campId, arriveDate, departDate);
            bool timeToExit = false;
            while (!timeToExit)
            {
                if (sites.Count == 0)
                {
                    Console.WriteLine("There are no available sites on that date, try again.");
                    Console.ReadKey();
                    Console.Clear();
                    Header();
                    timeToExit = true;
                    //CampgroundReservation(_parks[campId]);
                }
                else
                {
                    int numDays = (int)(departDate - arriveDate).TotalDays + 1;

                    decimal totalCost = _campgrounds[campId - 1].CalculateCost(_campgrounds[campId - 1], numDays);

                    Console.Clear();
                    Header();
                    Console.WriteLine("Results Matching Your Search Criteria");
                    Console.WriteLine();
                    Console.WriteLine($"{siteNo,6} | {maxOcc,9} | {access,9} | {rvLngth,9} | {utility,9} | {cost,9}");
                    for (int i = 0; i < sites.Count; i++)
                    {
                        Console.WriteLine($"{sites[i].SiteNumber,8} | {sites[i].MaxOccupancy,10} | {sites[i].Accessible,11} | {sites[i].MaxRvLength,13} | {sites[i].Utilities,9} | {totalCost.ToString("c"),9}");
                    }  //iterates through the Sites List to display to user

                    Console.WriteLine();
                    int siteId = CLIHelper.GetInteger("Which site would you like to reserve ? (enter 0 to cancel)");
                    if (siteId == 0)
                    {
                        Console.Clear();
                        Header();
                        timeToExit = true;
                    }

                    Console.WriteLine();
                    string resName = CLIHelper.GetString("What name should your reservation be called?");

                    ReservationSqlDAL sqlDal = new ReservationSqlDAL(DatabaseConnection);

                    int resConfirmationId = sqlDal.BookReservation(siteId, arriveDate, departDate, resName);

                    Console.WriteLine();
                    Console.WriteLine($"The reservation has been made and your confirmation number is {resConfirmationId}");
					Console.WriteLine("Click any key to exit the program.");

					timeToExit = true;

					Console.ReadKey();
                }
            }
		}
        #endregion

        //not being used yet... future functionality planned
        public void ParkWideReservation()
		{
			Console.Write("What is the arrival date (xxxx/xx/xx)? ");
			Console.Write("What is the departure date (xxxx/xx/xx)? ");

			Console.Write("Which site should be reserved? ");
			Console.Write("What name should the reservation be made under? ");
		}
	}
}
