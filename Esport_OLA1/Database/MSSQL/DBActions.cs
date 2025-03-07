using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Text.RegularExpressions;

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

        public void JoinTournament(int playerId, int tournamentId)
        {
            try
            {
                var connection = DBConnection.Instance().Connection;

                using (var command = new SqlCommand("joinTournament", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@playerId", playerId);
                    command.Parameters.AddWithValue("@tournamentId", tournamentId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine(reader["message"].ToString()); // Read success message
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

        }

        public void JoinTournamentPreparedStatements(int playerId, int tournamentId)
        {
            string query = @"
            SET NOCOUNT ON;

            -- Check if the player exists
            IF NOT EXISTS (SELECT 1 FROM Players WHERE player_id = @playerId)
            BEGIN
                THROW 50000, 'Player not found!', 1;
                RETURN;
            END

            -- Check if the tournament exists
            IF NOT EXISTS (SELECT 1 FROM Tournaments WHERE tournament_id = @tournamentId)
            BEGIN
                THROW 50001, 'Tournament not found!', 1;
                RETURN;
            END

            -- Check if the player is already registered in the tournament
            IF EXISTS (
                SELECT 1 FROM Tournament_Registrations 
                WHERE FK_tournament_registrations_player_ID = @playerId 
                    AND FK_tournament_registrations_tournament_ID = @tournamentId
            )
            BEGIN
                THROW 50002, 'Player is already registered in this tournament!', 1;
                RETURN;
            END

            -- Insert player into the tournament
            INSERT INTO Tournament_Registrations (
                FK_tournament_registrations_player_ID, 
                FK_tournament_registrations_tournament_ID, 
                registered_at
            ) 
            VALUES (@playerId, @tournamentId, GETDATE());

            -- Return success message
            SELECT 'Player successfully registered for the tournament' AS message;
            ";
            try
            {
                var connection = DBConnection.Instance().Connection;

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@playerId", playerId);
                    command.Parameters.AddWithValue("@tournamentId", tournamentId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine(reader["message"].ToString()); // Read success message
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

        }

        public void SubmitMatchResult(int matchId, int winnerId)
        {
            try
            {
                var connection = DBConnection.Instance().Connection;

                using (var command = new SqlCommand("submitMatchResult", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@matchId", matchId);
                    command.Parameters.AddWithValue("@winnerId", winnerId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine(reader["message"].ToString()); // Read success message
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
        }

        public void SubmitMatchResultPreparedStatements(int matchId, int winnerId)
        {
            string query = @"
                SET NOCOUNT ON;

                -- Check if the match exists
                IF NOT EXISTS (SELECT 1 FROM Matches WHERE match_id = @matchId)
                BEGIN
                    THROW 50000, 'Match not found!', 1;
                    RETURN;
                END

                -- Check if the winner is one of the match participants
                IF NOT EXISTS (
                    SELECT 1 FROM Matches 
                    WHERE match_id = @matchId 
                    AND (FK_matches_player1_id = @winnerId OR FK_matches_player2_id = @winnerId)
                )
                BEGIN
                    THROW 50001, 'Winner must be one of the match participants!', 1;
                    RETURN;
                END

                -- Update the match result with the winner
                UPDATE Matches 
                SET FK_matches_winner_id = @winnerId
                WHERE match_id = @matchId;

                -- Return success message
                SELECT 'Match result updated successfully' AS message;
            ";
            try
            {
                var connection = DBConnection.Instance().Connection;

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@matchId", matchId);
                    command.Parameters.AddWithValue("@winnerId", winnerId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine(reader["message"].ToString()); // Read success message
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
        }

        public void RecreateDatabase()
        {
            string SQLFilePath = "C:\\Users\\dieky\\source\\repos\\Esport_OLA1\\Esport_OLA1\\Database\\MSSQL\\ESPORT OLA1.sql";
            string ConnectionString = "data source=Diekmann-Laptop;trusted_connection=true;TrustServerCertificate=True;";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'ESPORT_OLA1') " +
                                      "BEGIN DROP DATABASE ESPORT_OLA1 END";
                command.ExecuteNonQuery();
            }

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "CREATE DATABASE ESPORT_OLA1";
                command.ExecuteNonQuery();
            }

            string script = File.ReadAllText(SQLFilePath);
            using (var connection = new SqlConnection(ConnectionString + "Initial Catalog=ESPORT_OLA1;"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = script;
                command.ExecuteNonQuery();
            }

            Console.WriteLine("Succesfully restored database");

            ApplyProceduresTriggersFunctions();
        }

        public void ApplyProceduresTriggersFunctions()
        {
            string SQLFilePath = "C:\\Users\\dieky\\source\\repos\\Esport_OLA1\\Esport_OLA1\\Database\\MSSQL\\esport_ola1 stored procedures functions triggers.sql";
            string ConnectionString = "data source=Diekmann-Laptop;trusted_connection=true;TrustServerCertificate=True;";
            string script = File.ReadAllText(SQLFilePath);

            // Splitting the creation of each procedure/function/trigger on "GO" this is the keyword used in SQL to send a batch of code
            // Which is needed between creating any of the above. Running the script inside MSSQL, possibly workbench too?, works but
            // due to ExecuteNonQuery() in C# it is not respecting the GO in between causing issues.
            // Thx chatgpt 
            string[] commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            try
            {
                using (var connection = new SqlConnection(ConnectionString + "Initial Catalog=ESPORT_OLA1;"))
                {
                    connection.Open();

                    foreach (var commandText in commands)
                    {
                        if (!string.IsNullOrWhiteSpace(commandText))
                        {
                            using (var command = new SqlCommand(commandText, connection))
                            {
                                command.CommandType = System.Data.CommandType.Text;
                                try
                                {
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error executing command: {ex.Message}\nCommand:\n{commandText}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database script execution failed: " + ex.Message);
            }

            Console.WriteLine("Succesfully added procedures, funtioncs and triggers");
        }

    }
}
