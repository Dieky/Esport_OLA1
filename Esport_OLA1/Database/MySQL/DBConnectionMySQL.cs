using MySql.Data.MySqlClient;
using System;

namespace Esport_OLA1.Database.MySQL
{
    public class DBConnectionMySQL
    {
        private static readonly Lazy<DBConnectionMySQL> _instance =
            new Lazy<DBConnectionMySQL>(() => new DBConnectionMySQL());

        private MySqlConnection _connection;
        private readonly string _connectionString;
        private bool _isConnectionOpen = false;

        // Private constructor to prevent instantiation from outside
        private DBConnectionMySQL()
        {
            _connectionString = "server=your_server;database=your_database;user=your_username;password=your_password;";
            _connection = new MySqlConnection(_connectionString);
        }

        // Singleton instance accessor
        public static DBConnectionMySQL Instance() => _instance.Value;

        // Property to get the connection
        public MySqlConnection Connection
        {
            get
            {
                if (_connection.State != System.Data.ConnectionState.Open && !_isConnectionOpen)
                {
                    try
                    {
                        _connection.Open();
                        _isConnectionOpen = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error opening MySQL connection: " + ex.Message);
                    }
                }
                return _connection;
            }
        }

        // Close connection method
        public void CloseConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
                _isConnectionOpen = false;
            }
        }

        // Reset connection (optional)
        public void ResetConnection()
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
            {
                _connection.Close();
            }
            _isConnectionOpen = false;
        }
    }
}
