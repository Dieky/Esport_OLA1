-- Stored Procedures
-- 1. Register en ny spiller
DELIMITER //
CREATE PROCEDURE registerPlayer(
    IN playerUsername VARCHAR(25), 
    IN playerEmail VARCHAR(50)
)
BEGIN
    -- Check if the username or email already exists
    IF EXISTS (SELECT 1 FROM Players WHERE username = playerUsername OR email = playerEmail) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Username or email already exists!';
    ELSE
        INSERT INTO Players (username, email, ranking, createdAt)
        VALUES (playerUsername, playerEmail, 1000, CURRENT_TIMESTAMP);
        
        -- Return the inserted playerId
        SELECT LAST_INSERT_ID() AS playerId;
    END IF;
END //
DELIMITER ;

CALL registerPlayer('JohnDoe', 'john@example.com');

-- 2. En spiller tilmelder sig en turnering.
DELIMITER //
CREATE PROCEDURE joinTournament(
    IN playerId INT, 
    IN tournamentId INT
)
BEGIN
    -- Check if the player exists
    IF NOT EXISTS (SELECT 1 FROM Players WHERE player_id = playerId) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Player not found!';
    
    -- Check if the tournament exists
    ELSEIF NOT EXISTS (SELECT 1 FROM Tournaments WHERE tournament_id = tournamentId) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Tournament not found!';
    
    -- Check if the player is already registered in the tournament
    ELSEIF EXISTS (
        SELECT 1 FROM Tournament_Registrations 
        WHERE FK_tournament_registrations_player_ID = playerId 
          AND FK_tournament_registrations_tournament_ID = tournamentId
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Player is already registered in this tournament!';
    
    ELSE
        -- Insert player into the tournament
        INSERT INTO Tournament_Registrations (
            FK_tournament_registrations_player_ID, 
            FK_tournament_registrations_tournament_ID, 
            registered_at
        ) 
        VALUES (playerId, tournamentId, CURRENT_TIMESTAMP);

        SELECT 'Player successfully registered for the tournament' AS message;
    END IF;
END //
DELIMITER ;

CALL joinTournament(1, 2);

-- 3. Registrer en kamps resultat
DELIMITER //
CREATE PROCEDURE submitMatchResult(
    IN matchId INT, 
    IN winnerId INT
)
BEGIN
    -- Check if the match exists
    IF NOT EXISTS (SELECT 1 FROM Matches WHERE match_id = matchId) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Match not found!';
    
    -- Check if the winner is one of the match participants
    ELSEIF NOT EXISTS (
        SELECT 1 FROM Matches 
        WHERE match_id = matchId 
        AND (FK_matches_player1_id = winnerId OR FK_matches_player2_id = winnerId)
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Winner must be one of the match participants!';
    
    ELSE
        -- Update the match result with the winner
        UPDATE Matches 
        SET FK_matches_winner_id = winnerId
        WHERE match_id = matchId;

        SELECT 'Match result updated successfully' AS message;
    END IF;
END //
DELIMITER ;

CaLL submitMatchResult(1,2);

-- Functions:
-- Function: getTotalWins
-- Returns the number of wins for a given player
DELIMITER //
CREATE FUNCTION getTotalWins(player_id INT) 
RETURNS INT DETERMINISTIC
BEGIN
    DECLARE totalWins INT;
    
    -- Count the number of matches where the player is the winner
    SELECT COUNT(*) INTO totalWins
    FROM Matches
    WHERE FK_matches_winner_id = player_id;
    
    RETURN totalWins;
END //
DELIMITER ;

-- 2. Retunerer turneringens status (upcoming, ongoing, completed)
DELIMITER //
CREATE FUNCTION getTournamentStatus(tournamentId INT) 
RETURNS VARCHAR(20) DETERMINISTIC
BEGIN
    DECLARE tournamentStartDate DATETIME;
    DECLARE totalMatches INT;
    DECLARE completedMatches INT;
    DECLARE tournamentStatus VARCHAR(20);

    -- Get the tournament start date
    SELECT start_date INTO tournamentStartDate 
    FROM Tournaments 
    WHERE tournament_id = tournamentId;

    -- Check if the tournament exists
    IF tournamentStartDate IS NULL THEN
        RETURN 'Tournament not found';
    END IF;

    -- If start date is in the future, return 'upcoming'
    IF tournamentStartDate > NOW() THEN
        RETURN 'upcoming';
    END IF;

    -- Count total matches in the tournament
    SELECT COUNT(*) INTO totalMatches 
    FROM Matches 
    WHERE FK_matches_tournament_id = tournamentId;

    -- Count matches that have a winner
    SELECT COUNT(*) INTO completedMatches 
    FROM Matches 
    WHERE FK_matches_tournament_id = tournamentId 
    AND FK_matches_winner_id IS NOT NULL;

    -- Determine tournament status
    IF totalMatches = 0 THEN
        RETURN 'ongoing'; -- Tournament started but no matches yet
    ELSEIF completedMatches = totalMatches THEN
        RETURN 'completed'; -- All matches have a winner
    ELSE
        RETURN 'ongoing'; -- Some matches are still ongoing
    END IF;
END //
DELIMITER ;

SELECT getTournamentStatus(1);

-- Triggers
-- Ensures a tournament does not exceed the maximum number of players
DELIMITER //
CREATE TRIGGER beforeInsertRegistration
BEFORE INSERT ON Tournament_Registrations
FOR EACH ROW
BEGIN
    DECLARE currentPlayers INT;
    DECLARE maxPlayers INT;
    
    -- Get current number of players registered for the tournament
    SELECT COUNT(*) INTO currentPlayers 
    FROM Tournament_Registrations
    WHERE FK_tournament_registrations_tournament_ID = NEW.FK_tournament_registrations_tournament_ID;
    
    -- Get the tournament's max player limit
    SELECT max_players INTO maxPlayers 
    FROM Tournaments
    WHERE tournament_id = NEW.FK_tournament_registrations_tournament_ID;
    
    -- Check if the limit is reached
    IF currentPlayers >= maxPlayers THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Tournament player limit reached!';
    END IF;
END //
DELIMITER ;

-- 2. Opdaterer rank for spillere efter en kamp.
DELIMITER //
CREATE TRIGGER afterInsertMatch
AFTER INSERT ON Matches
FOR EACH ROW
BEGIN
    DECLARE winnerId INT;
    DECLARE loserId INT;

    -- Check if a winner is recorded
    IF NEW.FK_matches_winner_id IS NOT NULL THEN
        SET winnerId = NEW.FK_matches_winner_id;

        -- Determine the loser
        IF winnerId = NEW.FK_matches_player1_id THEN
            SET loserId = NEW.FK_matches_player2_id;
        ELSE
            SET loserId = NEW.FK_matches_player1_id;
        END IF;

        -- Update the winner's ranking (+10 points)
        UPDATE Players 
        SET ranking = ranking + 10
        WHERE player_id = winnerId;

        -- Update the loser's ranking (-5 points, but not below 0)
        UPDATE Players 
        SET ranking = GREATEST(ranking - 5, 0)
        WHERE player_id = loserId;
    END IF;
END //
DELIMITER ;