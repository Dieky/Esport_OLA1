using System;
using MySql.Data.MySqlClient;

namespace Esport_OLA1.Database.MySQL
{
    public class DBActionsMySQL
    {
        public void GetPlayerUsername(int id)
        {
            string query = "SELECT username FROM Players WHERE player_id = @player_id;";
            string username = "";

            try
            {
                // Get the MySQL connection instance
                using (var connection = DBConnectionMySQL.Instance().Connection)
                {
                    using (var command = new MySqlCommand(query, connection)) // ✅ Using MySqlCommand instead of SqlCommand
                    {
                        command.Parameters.AddWithValue("@player_id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                username = reader.GetString(0);
                            }
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
                DBConnectionMySQL.Instance().CloseConnection();
            }

            Console.WriteLine($"Username: {username}");
        }
    }
}
