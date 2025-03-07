CREATE TABLE Players (
    player_id INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(25) NOT NULL UNIQUE,
    email NVARCHAR(50) NOT NULL UNIQUE,
    ranking INT default 1000,
	created_at DATETIME default GETDATE(),
);

CREATE TABLE Tournaments (
	tournament_id INT PRIMARY KEY IDENTITY(1,1),
	tournament_name NVARCHAR(25) NOT NULL,
	game NVARCHAR(25) NOT NULL,
	max_players INT NOT NULL,
	start_date DATETIME NOT NULL,
	created_at DATETIME default GETDATE()
);

CREATE TABLE Tournament_Registrations (
	registration_id INT PRIMARY KEY IDENTITY(1,1),
	registered_at DATETIME default GETDATE()
);

CREATE TABLE Matches (
	match_id INT PRIMARY KEY IDENTITY(1,1),
	match_date DATETIME
);

ALTER TABLE Tournament_Registrations
ADD FK_tournament_registrations_player_ID INT NOT NULL, FK_tournament_registrations_tournament_ID INT NOT NULL,
CONSTRAINT FK_tournament_registrations_player_ID FOREIGN KEY (FK_tournament_registrations_player_ID) REFERENCES Players(player_id),
CONSTRAINT FK_tournament_registrations_tournament_ID FOREIGN KEY (FK_tournament_registrations_tournament_ID) REFERENCES Tournaments(tournament_id);
	-- FKs til Tournament registration
	-- player_id
	-- tournament_id NVARCHAR(25)

ALTER TABLE Matches
ADD FK_matches_tournament_id INT NOT NULL, FK_matches_player1_id INT NOT NULL, FK_matches_player2_id INT NOT NULL, FK_matches_winner_id INT default null,
CONSTRAINT FK_matches_tournament_id FOREIGN KEY (FK_matches_tournament_id) REFERENCES Tournaments(tournament_id),
CONSTRAINT FK_matches_player1_id FOREIGN KEY (FK_matches_player1_id) REFERENCES Players(player_id),
CONSTRAINT FK_matches_player2_id FOREIGN KEY (FK_matches_player2_id) REFERENCES Players(player_id),
CONSTRAINT FK_matches_winner_id FOREIGN KEY (FK_matches_winner_id) REFERENCES Players(player_id);
	-- FKs til Matches
	-- tournament_id
	-- player1_id
	-- player2_id
	-- winner_id

-- Dummy data

INSERT INTO PLAYERS (username, email) 
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
('Olivia', 'oliviaMaster@gmail.com');

-- Insert more tournaments
INSERT INTO Tournaments (tournament_name, game, max_players, start_date, created_at) 
VALUES 
('DreamHack Summer 2025', 'CS:GO', 32, GETDATE(), GETDATE()),
('ESL One 2025', 'Dota 2', 32, GETDATE(), GETDATE()),
('Katowice 2025', 'Starcraft 2',  16, GETDATE(), GETDATE()),
('IEM Cologne 2025', 'Fifa', 16, GETDATE(), GETDATE());

-- Register players for tournaments
INSERT INTO Tournament_Registrations (FK_tournament_registrations_player_ID, FK_tournament_registrations_tournament_ID) 
VALUES 
(1,1), (2,1), (3,1), (4,1),
(1,2), (2,2), (3,2), (4,2),
(5,1), (6,1), (7,1), (8,1), 
(5,2), (6,2), (7,2), (8,2), 
(9,3), (10,3), (1,3), (2,3), 
(9,4), (10,4), (3,4), (4,4); 

-- Add more matches with winners
INSERT INTO Matches (match_date, FK_matches_tournament_id, FK_matches_player1_id, FK_matches_player2_id, FK_matches_winner_id) 
VALUES 
(CONVERT(DATE, '25-03-2025', 105), 1, 1, 2, 1),
(CONVERT(DATE, '25-03-2025', 105), 1, 3, 4, 3),
(CONVERT(DATE, '26-03-2025', 105), 1, 5, 6, 5),
(CONVERT(DATE, '26-03-2025', 105), 1, 7, 8, 7),
(CONVERT(DATE, '27-03-2025', 105), 2, 1, 3, 3),
(CONVERT(DATE, '27-03-2025', 105), 2, 2, 4, 4);