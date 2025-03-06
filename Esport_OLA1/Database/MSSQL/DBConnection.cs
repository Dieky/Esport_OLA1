using System;
using Microsoft.Data.SqlClient;

namespace Esport_OLA1.Database.MSSQL
{
    public class DBConnection
    {
        private static readonly Lazy<DBConnection> _instance = new Lazy<DBConnection>(() => new DBConnection());
        private readonly SqlConnection _connection;
        private bool _isConnectionOpen = false;
        private readonly string _connectionString;

        // Private constructor
        private DBConnection()
        {
            _connectionString = "data source=Diekmann-Laptop;initial catalog=ESPORT_OLA1;trusted_connection=true;TrustServerCertificate=True;";
            _connection = new SqlConnection(_connectionString);
        }

        public static DBConnection Instance() => _instance.Value;

        public SqlConnection Connection
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
                        Console.WriteLine("Error opening connection: " + ex.Message);
                    }
                }
                return _connection;
            }
        }

        public void CloseConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
                _isConnectionOpen = false;
            }
        }

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
