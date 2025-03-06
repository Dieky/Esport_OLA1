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

        // Method using prepared statements for joining tournament
        public void JoinTournamentPrepared(int playerId, int tournamentId)
        {
            try
            {
                var connection = DBConnectionMySQL.Instance().Connection;
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                // Check if tournament exists and has space
                string checkQuery = @"SELECT COUNT(*) as count, t.max_players 
                                    FROM Tournament_Registrations tr 
                                    RIGHT JOIN Tournaments t ON t.tournament_id = @tournamentId 
                                    WHERE tr.FK_tournament_registrations_tournament_ID = @tournamentId";

                using (var checkCmd = new MySqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@tournamentId", tournamentId);
                    using (var reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int currentPlayers = Convert.ToInt32(reader["count"]);
                            int maxPlayers = Convert.ToInt32(reader["max_players"]);
                            if (currentPlayers >= maxPlayers)
                            {
                                Console.WriteLine("Tournament is full!");
                                return;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Tournament not found!");
                            return;
                        }
                    }
                }

                // Check if player is already registered
                string checkPlayerQuery = @"SELECT COUNT(*) 
                                          FROM Tournament_Registrations 
                                          WHERE FK_tournament_registrations_player_ID = @playerId 
                                          AND FK_tournament_registrations_tournament_ID = @tournamentId";

                using (var checkPlayerCmd = new MySqlCommand(checkPlayerQuery, connection))
                {
                    checkPlayerCmd.Parameters.AddWithValue("@playerId", playerId);
                    checkPlayerCmd.Parameters.AddWithValue("@tournamentId", tournamentId);
                    int existingRegistrations = Convert.ToInt32(checkPlayerCmd.ExecuteScalar());
                    
                    if (existingRegistrations > 0)
                    {
                        Console.WriteLine("Player already registered for this tournament!");
                        return;
                    }
                }

                // Register player for tournament
                string insertQuery = @"INSERT INTO Tournament_Registrations 
                                     (FK_tournament_registrations_player_ID, FK_tournament_registrations_tournament_ID) 
                                     VALUES (@playerId, @tournamentId)";

                using (var cmd = new MySqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    cmd.Parameters.AddWithValue("@tournamentId", tournamentId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Successfully joined tournament!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to join tournament.");
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
        }

        // Method to reset database to original state
        public void ResetDatabase()
        {
            try
            {
                var connection = DBConnectionMySQL.Instance().Connection;
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                // Clear existing data
                string[] clearQueries = {
                    "DELETE FROM Tournament_Registrations;",
                    "DELETE FROM Matches;",
                    "DELETE FROM Players;",
                    "DELETE FROM Tournaments;",
                    "ALTER TABLE Players AUTO_INCREMENT = 1;",
                    "ALTER TABLE Tournaments AUTO_INCREMENT = 1;",
                    "ALTER TABLE Tournament_Registrations AUTO_INCREMENT = 1;",
                    "ALTER TABLE Matches AUTO_INCREMENT = 1;"
                };

                foreach (string query in clearQueries)
                {
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // Reinsert original data
                string[] insertQueries = {
                    @"INSERT INTO Players (username, email) 
                    VALUES 
                    ('Patrick', 'dieky91@gmail.com'),
                    ('Frederik', 'frederikTheWinner@gmail.com'),
                    ('William', 'WilliamTheKing@gmail.com'),
                    ('Emilio', 'emilioNotMarriedYet@gmail.com'),
                    ('Larsen', 'larsenPro@gmail.com'),
                    ('Sophia', 'sophiaGamer@gmail.com'),
                    ('Benjamin', 'benjiTheChamp@gmail.com'),
                    ('Isabella', 'isabellaQueen@gmail.com'),
                    ('Noah', 'noahWinner@gmail.com'),
                    ('Olivia', 'oliviaMaster@gmail.com');",

                    @"INSERT INTO Tournaments (tournament_name, game, max_players, start_date, created_at) 
                    VALUES 
                    ('DreamHack Summer 2025', 'CS:GO', 32, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                    ('ESL One 2025', 'Dota 2', 32, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                    ('Katowice 2025', 'Starcraft 2', 16, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                    ('IEM Cologne 2025', 'Fifa', 16, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);",

                    @"INSERT INTO Tournament_Registrations (FK_tournament_registrations_player_ID, FK_tournament_registrations_tournament_ID) 
                    VALUES 
                    (1,1), (2,1), (3,1), (4,1),
                    (1,2), (2,2), (3,2), (4,2),
                    (5,1), (6,1), (7,1), (8,1), 
                    (5,2), (6,2), (7,2), (8,2), 
                    (9,3), (10,3), (1,3), (2,3), 
                    (9,4), (10,4), (3,4), (4,4);",

                    @"INSERT INTO Matches (match_date, FK_matches_tournament_id, FK_matches_player1_id, FK_matches_player2_id) 
                    VALUES 
                    (STR_TO_DATE('25-03-2025', '%d-%m-%Y'), 1, 1, 2),
                    (STR_TO_DATE('25-03-2025', '%d-%m-%Y'), 1, 3, 4),
                    (STR_TO_DATE('26-03-2025', '%d-%m-%Y'), 1, 5, 6),
                    (STR_TO_DATE('26-03-2025', '%d-%m-%Y'), 1, 7, 8),
                    (STR_TO_DATE('27-03-2025', '%d-%m-%Y'), 2, 1, 3),
                    (STR_TO_DATE('27-03-2025', '%d-%m-%Y'), 2, 2, 4);"
                };

                foreach (string query in insertQueries)
                {
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Database reset to original state successfully!");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("MySQL error during reset: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during reset: " + ex.Message);
            }
        }
    }
}
