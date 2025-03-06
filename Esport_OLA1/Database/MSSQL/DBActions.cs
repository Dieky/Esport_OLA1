using Microsoft.Data.SqlClient;
using System;

namespace Esport_OLA1.Database.MSSQL
{
    public class DBActions
    {
        public void GetPlayerUsername(int id)
        {
            string query = "SELECT * FROM Players WHERE player_id = @player_id;";
            string username = "";
            string email = "";

            try
            {
                var connection = DBConnection.Instance().Connection;

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@player_id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            username = reader.GetString(1);
                            email = reader.GetString(2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
            }
            finally
            {
                DBConnection.Instance().CloseConnection();
            }

            Console.WriteLine($"Username: {username} || Email: {email}");
        }
    }
}
