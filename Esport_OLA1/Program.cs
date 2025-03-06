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
            // dBActionsMySQL.GetPlayerUsername(3);
            //dBActionsMySQL.JoinTournament(11, 1);
            dBActionsMySQL.SubmitMatchResult(8, 1);


        }
    }
}
