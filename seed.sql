SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE `Albums`;
TRUNCATE TABLE `Artists`;
TRUNCATE TABLE `Genres`;
SET FOREIGN_KEY_CHECKS = 1;

INSERT INTO `Genres` (`Id`, `Name`) VALUES
(1, 'Rock'),
(2, 'Jazz'),
(3, 'Hip-Hop'),
(4, 'Indie Rock'),
(5, 'Alternative Rock');

INSERT INTO `Artists` (`Id`, `Name`, `ImagePath`, `Biography`) VALUES
(1, 'Waits, Tom', '/img/artists/tom_waits.jpg', 'Американський музикант, автор пісень і актор, відомий хрипким голосом і експериментальним поєднанням блюзу, джазу та авангарду.'),
(2, 'Cullum, Jamie', '/img/artists/jamie_cullum.jpg', 'Британський джазовий піаніст і вокаліст, що поєднує джаз із поп- та рок-впливами.'),
(3, 'KRS One', '/img/artists/krs_one.jpg', 'Один із піонерів хіп-хопу, репер і продюсер, знаний соціально загостреними текстами.'),
(4, 'Franz Ferdinand', '/img/artists/franz_ferdinand.jpg', 'Шотландський інді-рок гурт, що прославився танцювальними гітарними риффами на початку 2000-х.'),
(5, 'Arcade Fire', '/img/artists/arcade_fire.jpg', 'Канадський інді/альтернативний рок-гурт, відомий масштабними оркестровими аранжуваннями.'),
(6, 'Pretty Reckless', '/img/artists/the_pretty_reckless.jpg', 'Американський рок-гурт під проводом Тейлор Момсен, що поєднує гранж і хард-рок.'),
(7, 'White Stripes', '/img/artists/the_white_stripes.jpg', 'Американський рок-дует із Детройта, відомий мінімалістичним звучанням і гаражним роком.'),
(8, 'Van Halen', '/img/artists/van_halen.jpg', 'Легендарний американський хард-рок гурт, один із засновників гітарного вірусотапінгу.');

INSERT INTO `Albums` (`Id`, `Title`, `ArtistId`, `GenreId`, `Rating`, `ReviewCount`, `Price`, `Stock`, `ImagePath`) VALUES
(1, 'Mule Variations', 1, 1, 4.6, 12, 1050.00, 6, '/img/1.jpg'),
(2, 'Momentum', 2, 2, 4.2, 8, 1090.00, 3, '/img/2.jpg'),
(3, 'Return of the Boom Bap', 3, 3, 4.5, 15, 990.00, 7, '/img/3.jpg'),
(4, 'Tonight: Franz Ferdinand', 4, 4, 4.0, 6, 990.00, 2, '/img/4.jpg'),
(5, 'Everything Now (Night Version)', 5, 5, 4.1, 9, 990.00, 4, '/img/5.jpg'),
(6, 'Going To Hell (Coloured)', 6, 1, 4.3, 5, 890.00, 6, '/img/6.jpg'),
(7, 'Get Behind Me Satan', 7, 1, 4.7, 20, 1790.00, 3, '/img/7.jpg'),
(8, 'Fair Warning', 8, 1, 4.8, 18, 1190.00, 8, '/img/8.jpg');

SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE `Songs`;
SET FOREIGN_KEY_CHECKS = 1;

-- Album 1: Tom Waits - Mule Variations (16 tracks)
INSERT INTO `Songs` (`Title`, `AlbumId`, `Duration`) VALUES
('Big in Japan', 1, '00:04:05'),
('Lowside of the Road', 1, '00:02:59'),
('Hold On', 1, '00:05:33'),
('Get Behind the Mule', 1, '00:06:52'),
('House Where Nobody Lives', 1, '00:04:14'),
('Cold Water', 1, '00:05:23'),
('Pony', 1, '00:04:32'),
('What''s He Building?', 1, '00:03:20'),
('Black Market Baby', 1, '00:05:02'),
('Eyeball Kid', 1, '00:04:26'),
('Picture in a Frame', 1, '00:03:40'),
('Chocolate Jesus', 1, '00:03:55'),
('Georgia Lee', 1, '00:04:24'),
('Filipino Box Spring Hog', 1, '00:03:09'),
('Take It With Me', 1, '00:04:25'),
('Come on Up to the House', 1, '00:04:36'),

-- Album 2: Jamie Cullum - Momentum (12 tracks)
('The Same Things', 2, '00:03:46'),
('Edge of Something', 2, '00:04:40'),
('Everything You Didn''t Do', 2, '00:03:48'),
('When I Get Famous', 2, '00:04:34'),
('Love for $ale', 2, '00:05:20'),
('Pure Imagination', 2, '00:04:10'),
('Anyway', 2, '00:04:00'),
('Sad Sad World', 2, '00:04:20'),
('Take Me Out (Of Myself)', 2, '00:04:05'),
('Save Your Soul', 2, '00:04:15'),
('Get a Hold of Yourself', 2, '00:04:30'),
('You''re Not the Only One', 2, '00:04:29'),

-- Album 3: KRS-One - Return of the Boom Bap (14 tracks)
('KRS-One Attacks', 3, '00:02:50'),
('Outta Here', 3, '00:04:28'),
('Black Cop', 3, '00:02:59'),
('Mortal Thought', 3, '00:03:19'),
('I Can''t Wake Up', 3, '00:03:34'),
('Slap Them Up', 3, '00:03:58'),
('Sound of Da Police', 3, '00:04:18'),
('Mad Crew', 3, '00:04:24'),
('Uh Oh', 3, '00:04:05'),
('Brown Skin Woman', 3, '00:04:38'),
('Return of the Boom Bap', 3, '00:03:46'),
('"P" Is Still Free', 3, '00:04:56'),
('Stop Frontin''', 3, '00:03:19'),
('Higher Level', 3, '00:04:00'),

-- Album 4: Franz Ferdinand - Tonight: Franz Ferdinand (12 tracks)
('Ulysses', 4, '00:03:11'),
('Turn It On', 4, '00:02:20'),
('No You Girls', 4, '00:03:41'),
('Send Him Away', 4, '00:02:59'),
('Twilight Omens', 4, '00:02:29'),
('Bite Hard', 4, '00:03:26'),
('What She Came For', 4, '00:03:33'),
('Live Alone', 4, '00:03:29'),
('Can''t Stop Feeling', 4, '00:03:02'),
('Lucid Dreams', 4, '00:07:56'),
('Dream Again', 4, '00:03:18'),
('Katherine Kiss Me', 4, '00:02:55'),

-- Album 5: Arcade Fire - Everything Now (Night Version) (13 tracks)
('Everything_Now (Continued)', 5, '00:00:46'),
('Everything Now', 5, '00:05:03'),
('Signs of Life', 5, '00:04:37'),
('Creature Comfort', 5, '00:04:44'),
('Peter Pan', 5, '00:02:49'),
('Chemistry', 5, '00:03:38'),
('Infinite Content', 5, '00:01:42'),
('Infinite_Content', 5, '00:01:37'),
('Electric Blue', 5, '00:04:02'),
('Good God Damn', 5, '00:03:34'),
('Put Your Money on Me', 5, '00:05:53'),
('We Don''t Deserve Love', 5, '00:06:29'),
('Everything Now (Continued)', 5, '00:02:22'),

-- Album 6: The Pretty Reckless - Going to Hell (12 tracks)
('Follow Me Down', 6, '00:04:40'),
('Going to Hell', 6, '00:03:38'),
('Heaven Knows', 6, '00:03:52'),
('House on a Hill', 6, '00:03:41'),
('Sweet Things', 6, '00:03:50'),
('Dear Sister', 6, '00:01:50'),
('Absolution', 6, '00:04:05'),
('Blame Me', 6, '00:03:35'),
('Burn', 6, '00:03:48'),
('Why''d You Bring a Shotgun to the Party', 6, '00:03:20'),
('Fucked Up World', 6, '00:04:16'),
('Waiting for a Friend', 6, '00:03:12'),

-- Album 7: The White Stripes - Get Behind Me Satan (13 tracks)
('Blue Orchid', 7, '00:02:40'),
('The Nurse', 7, '00:03:54'),
('My Doorbell', 7, '00:04:05'),
('Forever for Her (Is Over for Me)', 7, '00:03:20'),
('As Ugly As I Seem', 7, '00:04:13'),
('The Denial Twist', 7, '00:02:37'),
('White Moon', 7, '00:04:02'),
('Instinct Blues', 7, '00:04:25'),
('Passive Manipulation', 7, '00:00:39'),
('Take, Take, Take', 7, '00:04:24'),
('Little Ghost', 7, '00:02:23'),
('Red Rain', 7, '00:03:53'),
('I''m Lonely (But I Ain''t That Lonely Yet)', 7, '00:04:22'),

-- Album 8: Van Halen - Fair Warning (9 tracks)
('Mean Street', 8, '00:05:01'),
('Dirty Movies', 8, '00:04:07'),
('Sinner''s Swing!', 8, '00:03:09'),
('Hear About It Later', 8, '00:04:34'),
('Unchained', 8, '00:03:29'),
('Push Comes to Shove', 8, '00:03:48'),
('So This Is Love?', 8, '00:03:06'),
('Sunday Afternoon in the Park', 8, '00:01:58'),
('One Foot Out the Door', 8, '00:01:57');

-- Admin account: email admin@vinylshop.local / password Admin123!
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE `UserRole`;
TRUNCATE TABLE `UserAdmin`;
SET FOREIGN_KEY_CHECKS = 1;

INSERT INTO `UserAdmin` (`Id`, `Email`, `PasswordHash`) VALUES
(1, 'admin@vinylshop.local', '$2b$11$w4dCqlIV22PdDczvgmjuLuKCNcDnokwsGfwEz6HQchc/ft.B1Hx56');

INSERT INTO `UserRole` (`UserAdminId`, `RoleName`) VALUES
(1, 0);
