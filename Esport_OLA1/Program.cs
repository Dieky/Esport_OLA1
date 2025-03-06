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
            // Microsoft SQL
            // DBActions dBActions = new DBActions();
            // dBActions.GetPlayerUsername(3);



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
