using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.SetWindowSize(150, 40);
            // Sample Code to get a connection string from the
            // App.Config file
            // Use this so that you don't need to copy your connection string all over your code!
            string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
			NationalParkCLI cli = new NationalParkCLI();
			cli.RunCLI();
        }
    }
}
