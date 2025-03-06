-- Create Players table
CREATE TABLE Players (
    player_id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(25) NOT NULL UNIQUE,
    email VARCHAR(50) NOT NULL UNIQUE,
    ranking INT DEFAULT 1000,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Create Tournaments table
CREATE TABLE Tournaments (
    tournament_id INT PRIMARY KEY AUTO_INCREMENT,
    tournament_name VARCHAR(25) NOT NULL,
    game VARCHAR(25) NOT NULL,
    max_players INT NOT NULL,
    start_date DATETIME NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Create Tournament_Registrations table
CREATE TABLE Tournament_Registrations (
    registration_id INT PRIMARY KEY AUTO_INCREMENT,
    FK_tournament_registrations_player_ID INT NOT NULL,
    FK_tournament_registrations_tournament_ID INT NOT NULL,
    registered_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_tournament_registrations_player FOREIGN KEY (FK_tournament_registrations_player_ID) REFERENCES Players(player_id) ON DELETE CASCADE,
    CONSTRAINT FK_tournament_registrations_tournament FOREIGN KEY (FK_tournament_registrations_tournament_ID) REFERENCES Tournaments(tournament_id) ON DELETE CASCADE
);

-- Create Matches table
CREATE TABLE Matches (
    match_id INT PRIMARY KEY AUTO_INCREMENT,
    match_date DATETIME,
    FK_matches_tournament_id INT NOT NULL,
    FK_matches_player1_id INT NOT NULL,
    FK_matches_player2_id INT NOT NULL,
    FK_matches_winner_id INT NULL,
    CONSTRAINT FK_matches_tournament FOREIGN KEY (FK_matches_tournament_id) REFERENCES Tournaments(tournament_id) ON DELETE CASCADE,
    CONSTRAINT FK_matches_player1 FOREIGN KEY (FK_matches_player1_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    CONSTRAINT FK_matches_player2 FOREIGN KEY (FK_matches_player2_id) REFERENCES Players(player_id) ON DELETE CASCADE,
    CONSTRAINT FK_matches_winner FOREIGN KEY (FK_matches_winner_id) REFERENCES Players(player_id) ON DELETE SET NULL
);

-- Insert dummy players
INSERT INTO Players (username, email) 
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

-- Insert dummy tournaments
INSERT INTO Tournaments (tournament_name, game, max_players, start_date, created_at) 
VALUES 
('DreamHack Summer 2025', 'CS:GO', 32, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('ESL One 2025', 'Dota 2', 32, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('Katowice 2025', 'Starcraft 2',  16, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('IEM Cologne 2025', 'Fifa', 16, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Register players for tournaments
INSERT INTO Tournament_Registrations (FK_tournament_registrations_player_ID, FK_tournament_registrations_tournament_ID) 
VALUES 
(1,1), (2,1), (3,1), (4,1),
(1,2), (2,2), (3,2), (4,2),
(5,1), (6,1), (7,1), (8,1), 
(5,2), (6,2), (7,2), (8,2), 
(9,3), (10,3), (1,3), (2,3), 
(9,4), (10,4), (3,4), (4,4); 

-- Insert matches with formatted dates
INSERT INTO Matches (match_date, FK_matches_tournament_id, FK_matches_player1_id, FK_matches_player2_id) 
VALUES 
(STR_TO_DATE('25-03-2025', '%d-%m-%Y'), 1, 1, 2),
(STR_TO_DATE('25-03-2025', '%d-%m-%Y'), 1, 3, 4),
(STR_TO_DATE('26-03-2025', '%d-%m-%Y'), 1, 5, 6),
(STR_TO_DATE('26-03-2025', '%d-%m-%Y'), 1, 7, 8),
(STR_TO_DATE('27-03-2025', '%d-%m-%Y'), 2, 1, 3),
(STR_TO_DATE('27-03-2025', '%d-%m-%Y'), 2, 2, 4);
