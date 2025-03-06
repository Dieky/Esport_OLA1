-- 1. Hent Alle Tournetinger, der starter inden for de næste 30 dage
SELECT * 
FROM tournaments t 
WHERE t.start_date BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 30 DAY);

-- 2. Find det antal turneringer, en spiller har deltaget i
SELECT p.player_id, p.username, COUNT(tr.registration_id) AS tournaments_participated
FROM Players p
JOIN Tournament_Registrations tr ON p.player_id = tr.FK_tournament_registrations_player_ID
GROUP BY p.player_id, p.username;

-- 3. Vis en liste over spillere registreret i en bestemt turnering.
SELECT p.player_id, p.username, p.email, p.ranking FROM players p join tournament_registrations tr
on p.player_id = tr.FK_tournament_registrations_player_ID
where tr.FK_tournament_registrations_tournament_ID = 1;

-- 4. Find spillere med flest sejre i en bestemt turnering
SELECT p.player_id, p.username, COUNT(m.FK_matches_winner_id) AS wins
FROM Players p
JOIN Matches m ON p.player_id = m.FK_matches_winner_id
WHERE m.FK_matches_tournament_id = 1 -- Change this to filter for a specific tournament
GROUP BY p.player_id, p.username
ORDER BY wins DESC
LIMIT 5;

-- 5. Hent alle kampe, hvor en bestemt spiller har deltaget.
SELECT * FROM Matches
WHERE FK_matches_player1_id = 1 OR FK_matches_player2_id = 1;

-- 6. Hent en spillers tilmeldte turneringer.
SELECT t.tournament_id, t.tournament_name, t.game, t.start_date
FROM Tournaments t
JOIN Tournament_Registrations tr ON t.tournament_id = tr.FK_tournament_registrations_tournament_ID
WHERE tr.FK_tournament_registrations_player_ID = 1; -- Change this to filter for a specific player

-- 7. Find de 5 bedst rangerede spillere.
SELECT username, ranking
FROM Players
ORDER BY ranking DESC
LIMIT 5;

-- 8. Beregn gennemsnitlig ranking for alle spillere.
SELECT AVG(ranking) AS average_ranking FROM Players;

-- 9. Vis turneringer med mindst 5 deltagere.
SELECT t.tournament_name, t.game, t.start_date, t.max_players, COUNT(tr.registration_id) AS participant_count
FROM Tournaments t
JOIN Tournament_Registrations tr ON t.tournament_id = tr.FK_tournament_registrations_tournament_ID
GROUP BY t.tournament_id, t.tournament_name, t.game, t.start_date, t.max_players
HAVING COUNT(tr.FK_tournament_registrations_player_ID) >= 5;

-- 10. Find det samlede antal spillere i systemet.
SELECT COUNT(*) AS total_players FROM Players;

-- 11. Find alle kampe, der mangler en vinder.
SELECT *
FROM Matches
WHERE FK_matches_winner_id IS NULL;

-- 12. Vis de mest populære spil baseret på turneringsantal.
SELECT game, COUNT(tournament_id) AS tournament_count
FROM Tournaments
GROUP BY game
ORDER BY tournament_count DESC;

-- 13. Find de 5 nyeste oprettede turneringer.
SELECT t.tournament_name, t.created_at
FROM Tournaments t
ORDER BY t.created_at DESC
LIMIT 5;

-- 14. Find spillere, der har registreret sig i flere end 3 turneringer.
SELECT p.player_id, p.username, COUNT(tr.registration_id) AS tournaments_registered
FROM Players p
JOIN Tournament_Registrations tr ON p.player_id = tr.FK_tournament_registrations_player_ID
GROUP BY p.player_id, p.username
HAVING COUNT(tr.registration_id) > 3;

-- 15. Hent alle kampe i en turnering sorteret efter dato.
SELECT * 
FROM Matches m
WHERE m.FK_matches_tournament_id = 1
ORDER BY m.match_date ASC;
