using Esport_OLA1.Database.MSSQL;
using Esport_OLA1.Database.MySQL;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Esport_OLA1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string databaseType = "MSSQL";

            if(databaseType == "MSSQL")
            {
                DBActions dbactions = new DBActions();

                //dbactions.JoinTournament(3, 3);
                //dbactions.JoinTournamentPreparedStatements(7, 4);
                //dbactions.SubmitMatchResult(1, 1);
                //dbactions.SubmitMatchResultPreparedStatements(1, 2);
                //dbactions.RecreateDatabase();
            }


            if(databaseType == "MYSQL")
            {

                // MySQL 
                DBActionsMySQL dBActionsMySQL = new DBActionsMySQL();
            
                try
                {
                    Console.WriteLine("Resetting database to original state...");
                    dBActionsMySQL.ResetDatabase();
                
                    Console.WriteLine("\nTesting tournament registration...");
                    dBActionsMySQL.JoinTournamentPrepared(5, 3);

                    // Add a small delay to see the output clearly
                    System.Threading.Thread.Sleep(1000);
                }
                finally
                {
                    // Close the connection after all operations are complete
                    DBConnectionMySQL.Instance().CloseConnection();
                }
            }
        }
    }
}
