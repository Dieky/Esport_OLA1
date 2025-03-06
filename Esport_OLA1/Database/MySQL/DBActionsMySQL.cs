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
        /*
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
        */
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

                    // Step 1: Check if the match exists and get the player IDs
                    string checkMatchQuery = "SELECT FK_matches_player1_id, FK_matches_player2_id FROM Matches WHERE match_id = @matchId;";
                    int player1Id = 0, player2Id = 0;

                    using (var command = new MySqlCommand(checkMatchQuery, connection))
                    {
                        command.Parameters.AddWithValue("@matchId", matchId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                Console.WriteLine("Match not found!");
                                return;
                            }
                            player1Id = reader.GetInt32(0);
                            player2Id = reader.GetInt32(1);
                        }
                    }

                    // Step 2: Validate that the winner is one of the participants
                    if (winnerId != player1Id && winnerId != player2Id)
                    {
                        Console.WriteLine("Winner must be one of the match participants!");
                        return;
                    }

                    // Step 3: Update the match result with the winner ID
                    string updateQuery = "UPDATE Matches SET FK_matches_winner_id = @winnerId WHERE match_id = @matchId;";
                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@winnerId", winnerId);
                        command.Parameters.AddWithValue("@matchId", matchId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Match result updated successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Failed to update match result.");
                        }
                    }

                    // Step 4: Update player rankings
                    UpdatePlayerRankings(connection, winnerId, (winnerId == player1Id ? player2Id : player1Id));
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
        private void UpdatePlayerRankings(MySqlConnection connection, int winnerId, int loserId)
        {
            try
            {
                // Increase winner's ranking by 10
                string updateWinner = "UPDATE Players SET ranking = ranking + 10 WHERE player_id = @winnerId;";
                using (var command = new MySqlCommand(updateWinner, connection))
                {
                    command.Parameters.AddWithValue("@winnerId", winnerId);
                    command.ExecuteNonQuery();
                }

                // Decrease loser's ranking by 5, ensuring it doesn't go below 0
                string updateLoser = "UPDATE Players SET ranking = GREATEST(ranking - 5, 0) WHERE player_id = @loserId;";
                using (var command = new MySqlCommand(updateLoser, connection))
                {
                    command.Parameters.AddWithValue("@loserId", loserId);
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Player rankings updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating rankings: " + ex.Message);
            }
        }



    }
}
