-- 1. Hent alle turneringer, der starter inden for de næste 30 dage.
-- Microsoft SQL:
	SELECT *
	FROM Tournaments
	WHERE start_date BETWEEN GETDATE() AND DATEADD(DAY, 30, GETDATE());
	
-- MySQL:
	SELECT * 
	FROM tournaments t 
	WHERE t.start_date BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 30 DAY);

-- 2. Find det antal turneringer, en spiller har deltaget i. 
-- Microsoft SQL
	SELECT p.player_id, p.username, COUNT(tr.registration_id) AS tournaments_participated
	FROM Players p
	INNER JOIN Tournament_Registrations tr 
		ON p.player_id = tr.FK_tournament_registrations_player_ID
	GROUP BY p.player_id, p.username;

-- MySQL
	SELECT p.player_id, p.username, COUNT(tr.registration_id) AS tournaments_participated
	FROM Players p
	JOIN Tournament_Registrations tr ON p.player_id = tr.FK_tournament_registrations_player_ID
	GROUP BY p.player_id, p.username;

-- 3. Vis en liste over spillere registreret i en bestemt turnering.
-- Microsoft SQL:
	SELECT player_id, username, email FROM Players 
	JOIN Tournament_Registrations tr ON player_id = FK_tournament_registrations_player_ID
	WHERE tr.FK_tournament_registrations_tournament_ID = 1;
	
-- MySQL:
	SELECT p.player_id, p.username, p.email, p.ranking FROM players p join tournament_registrations tr
	on p.player_id = tr.FK_tournament_registrations_player_ID
	where tr.FK_tournament_registrations_tournament_ID = 1;

-- 4. Find spillere med flest sejre i en bestemt turnering.
-- Microsoft SQL
	SELECT TOP 5 p.player_id, p.username, COUNT(m.FK_matches_winner_id) AS wins
	FROM Players p
	INNER JOIN Matches m 
		ON p.player_id = m.FK_matches_winner_id
	WHERE m.FK_matches_tournament_id = 1  -- Change this to filter for a specific tournament
	GROUP BY p.player_id, p.username
	ORDER BY wins DESC;

-- MySQL
	SELECT p.player_id, p.username, COUNT(m.FK_matches_winner_id) AS wins
	FROM Players p
	JOIN Matches m ON p.player_id = m.FK_matches_winner_id
	WHERE m.FK_matches_tournament_id = 1 -- Change this to filter for a specific tournament
	GROUP BY p.player_id, p.username
	ORDER BY wins DESC
	LIMIT 5;
	
-- 5. Hent alle kampe, hvor en bestemt spiller har deltaget.
-- Microsoft SQL:
	SELECT * FROM Matches
	WHERE FK_matches_player1_id = 1 OR FK_matches_player2_id = 1;
	
-- MySQL:
	SELECT * FROM Matches
	WHERE FK_matches_player1_id = 1 OR FK_matches_player2_id = 1;
	
-- 6. Hent en spillers tilmeldte turneringer.
-- Microsoft SQL
	SELECT t.tournament_id, t.tournament_name, t.game, t.start_date
	FROM Tournaments t
	INNER JOIN Tournament_Registrations tr 
		ON t.tournament_id = tr.FK_tournament_registrations_tournament_ID
	WHERE tr.FK_tournament_registrations_player_ID = 1;  -- Change this to filter for a specific player

-- MySQL
	SELECT t.tournament_id, t.tournament_name, t.game, t.start_date
	FROM Tournaments t
	JOIN Tournament_Registrations tr ON t.tournament_id = tr.FK_tournament_registrations_tournament_ID
	WHERE tr.FK_tournament_registrations_player_ID = 1; -- Change this to filter for a specific player

-- 7. Find de 5 bedst rangerede spillere.
-- Microsoft SQL:
	SELECT TOP 5 username, ranking
	FROM Players
	ORDER BY ranking DESC;
	
-- MySQL:
	SELECT username, ranking
	FROM Players
	ORDER BY ranking DESC
	LIMIT 5;

-- 8. Beregn gennemsnitlig ranking for alle spillere.
-- Microsoft SQL
	SELECT AVG(CAST(ranking AS FLOAT)) AS average_ranking 
	FROM Players;

-- MySQL
	SELECT AVG(ranking) AS average_ranking FROM Players;
	
-- 9. Vis turneringer med mindst 5 deltagere.
-- Microsoft SQL:
	SELECT t.tournament_name, t.game, t.start_date, t.max_players, COUNT(tr.registration_id) AS participant_count
	FROM Tournaments t
	JOIN Tournament_Registrations tr ON t.tournament_id = tr.FK_tournament_registrations_tournament_ID
	GROUP BY t.tournament_id, t.tournament_name, t.game, t.start_date, t.max_players
	HAVING COUNT(tr.FK_tournament_registrations_player_ID) >= 5;
	
-- MySQL:
	SELECT t.tournament_name, t.game, t.start_date, t.max_players, COUNT(tr.registration_id) AS participant_count
	FROM Tournaments t
	JOIN Tournament_Registrations tr ON t.tournament_id = tr.FK_tournament_registrations_tournament_ID
	GROUP BY t.tournament_id, t.tournament_name, t.game, t.start_date, t.max_players
	HAVING COUNT(tr.FK_tournament_registrations_player_ID) >= 5;

-- 10. Find det samlede antal spillere i systemet.
-- Microsoft SQL
	SELECT COUNT(*) AS total_players FROM Players;

-- MySQL
	SELECT COUNT(*) AS total_players FROM Players;
	
-- 11. Find alle kampe, der mangler en vinder.	
-- Microsoft SQL:
	SELECT *
	FROM Matches
	WHERE FK_matches_winner_id IS NULL;
	
-- MySQL:
	SELECT *
	FROM Matches
	WHERE FK_matches_winner_id IS NULL;

-- 12. Vis de mest populære spil baseret på turneringsantal.
-- Microsoft SQL
	SELECT game, COUNT(tournament_id) AS tournament_count
	FROM Tournaments
	GROUP BY game
	ORDER BY tournament_count DESC;

-- MySQL
	SELECT game, COUNT(tournament_id) AS tournament_count
	FROM Tournaments
	GROUP BY game
	ORDER BY tournament_count DESC;
	
-- 13. Find de 5 nyeste oprettede turneringer.
-- Microsoft SQL:
	SELECT TOP 5 t.tournament_name, t.created_at
	FROM Tournaments t
	ORDER BY t.created_at DESC;
	
-- MySQL:
	SELECT t.tournament_name, t.created_at
	FROM Tournaments t
	ORDER BY t.created_at DESC
	LIMIT 5;

-- 14. Find spillere, der har registreret sig i flere end 3 turneringer.
-- Microsoft SQL
	SELECT p.player_id, p.username, COUNT(tr.registration_id) AS tournaments_registered
	FROM Players p
	INNER JOIN Tournament_Registrations tr 
		ON p.player_id = tr.FK_tournament_registrations_player_ID
	GROUP BY p.player_id, p.username
	HAVING COUNT(tr.registration_id) > 3;

-- MySQL
	SELECT p.player_id, p.username, COUNT(tr.registration_id) AS tournaments_registered
	FROM Players p
	JOIN Tournament_Registrations tr ON p.player_id = tr.FK_tournament_registrations_player_ID
	GROUP BY p.player_id, p.username
	HAVING COUNT(tr.registration_id) > 3;
	
-- 15. Hent alle kampe i en turnering sorteret efter dato.
-- Microsoft SQL:
	SELECT * 
	FROM Matches m
	WHERE m.FK_matches_tournament_id = 1
	ORDER BY m.match_date ASC;
	
-- MySQL:
	SELECT * 
	FROM Matches m
	WHERE m.FK_matches_tournament_id = 1
	ORDER BY m.match_date ASC;