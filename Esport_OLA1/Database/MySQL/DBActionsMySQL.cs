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

        public void SubmitMatchResult(int matchId, int winnerId)
        {
            try
            {
                // Get the MySQL connection instance
                using (var connection = DBConnectionMySQL.Instance().Connection)
                {
                    // Open the connection if it's not already open
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    // Call the stored procedure
                    using (var command = new MySqlCommand("submitMatchResult", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@matchId", matchId);
                        command.Parameters.AddWithValue("@winnerId", winnerId);

                        // Execute the stored procedure
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine(reader["message"].ToString()); // Read success message
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("MySQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                DBConnectionMySQL.Instance().CloseConnection();
            }
        }
        public void JoinTournament(int playerId, int tournamentId)
        {
            try
            {
                // Get the MySQL connection instance
                using (var connection = DBConnectionMySQL.Instance().Connection)
                {
                    // Open the connection if it's not already open
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    // Call the stored procedure
                    using (var command = new MySqlCommand("joinTournament", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@playerId", playerId);
                        command.Parameters.AddWithValue("@tournamentId", tournamentId);

                        // Execute the stored procedure
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine(reader["message"].ToString()); // Read success message
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("MySQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                DBConnectionMySQL.Instance().CloseConnection();
            }
        }


    }
}
