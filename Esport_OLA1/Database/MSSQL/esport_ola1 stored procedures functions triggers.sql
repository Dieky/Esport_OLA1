-- Procedure 1
-- Registers a player
CREATE PROCEDURE registerPlayer  
    @playerUsername NVARCHAR(25),  
    @playerEmail NVARCHAR(50)
AS  
BEGIN  
    SET NOCOUNT ON;  -- Prevents extra result sets from affecting performance

    -- Check if username or email already exists
    IF EXISTS (SELECT 1 FROM Players WHERE username = @playerUsername OR email = @playerEmail)  
    BEGIN  
        THROW 50000, 'Username or email already exists!', 1;  
        RETURN;
    END  

    -- Insert new player
    INSERT INTO Players (username, email, ranking, created_at)  
    VALUES (@playerUsername, @playerEmail, 1000, GETDATE());  

    -- Return the inserted playerId
    SELECT SCOPE_IDENTITY() AS playerId;
END;
GO
-- EXEC registerPlayer @playerUsername = 'JohnDoe', @playerEmail = 'john@example.com';

-- Procedure 2
-- Assigns a player to a tournament
CREATE PROCEDURE joinTournament
    @playerId INT, 
    @tournamentId INT
AS
BEGIN
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
END;
GO
-- EXEC joinTournament @playerId = 1, @tournamentId = 7;

-- Procedure 3
-- Submits a match result
CREATE PROCEDURE submitMatchResult
    @matchId INT, 
    @winnerId INT
AS
BEGIN
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
END;
GO
-- EXEC submitMatchResult @matchId = 1, @winnerId = 2;

-- Function
-- Counts the total amount of wins from a player across all matches in all tournaments
CREATE FUNCTION getTotalWins(@player_id INT)  
RETURNS INT  
AS
BEGIN

	DECLARE @winCount INT;
	SELECT @winCount = COUNT(*)
	FROM Matches
	WHERE FK_matches_winner_id = @player_id;  

    -- If no matches are found, return 0
    RETURN ISNULL(@winCount, 0);  

END;
GO
-- SELECT dbo.getTotalWins(1) AS TotalWins;

-- Trigger
-- Ensures the max player count is not exceeded
CREATE TRIGGER beforeInsertRegistration  
ON Tournament_Registrations  
AFTER INSERT  
AS  
BEGIN  
    -- If the tournament already has 16 players, rollback the insert
    IF EXISTS (
        SELECT 1  
        FROM Tournaments t  
        JOIN (
            SELECT FK_tournament_registrations_tournament_ID, COUNT(*) AS registeredPlayers  
            FROM Tournament_Registrations  
            GROUP BY FK_tournament_registrations_tournament_ID  
        ) r ON t.tournament_id = r.FK_tournament_registrations_tournament_ID  
        JOIN inserted i ON r.FK_tournament_registrations_tournament_ID = i.FK_tournament_registrations_tournament_ID  
        WHERE r.registeredPlayers > t.max_players  
    )  
    BEGIN  
        -- Prevent the insertion
        RAISERROR ('Tournament is full. Registration denied.', 16, 1);  
        ROLLBACK TRANSACTION;  
    END  
END;
GO

