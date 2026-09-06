-- MovieVerse demo seed data
-- Poster/Profile/Image URL values are intentionally left NULL.
-- Identity users are NOT created here. Register users through the app so ASP.NET Identity creates valid password hashes/roles.
-- Reviews/watchlist/history are added only when existing AspNetUsers are found.

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    ---------------------------------------------------------------------------
    -- 1) GENRES
    ---------------------------------------------------------------------------
    DECLARE @GenreSeed TABLE ([Name] nvarchar(100) NOT NULL PRIMARY KEY);
    INSERT INTO @GenreSeed ([Name]) VALUES
        (N'Action'),
        (N'Adventure'),
        (N'Animation'),
        (N'Comedy'),
        (N'Crime'),
        (N'Drama'),
        (N'Family'),
        (N'Fantasy'),
        (N'Mystery'),
        (N'Romance'),
        (N'Sci-Fi'),
        (N'Thriller'),
        (N'Supernatural'),
        (N'Psychological');

    INSERT INTO dbo.Genres ([Name])
    SELECT s.[Name]
    FROM @GenreSeed s
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Genres g WHERE g.[Name] = s.[Name]);

    ---------------------------------------------------------------------------
    -- 2) ACTORS + ACTOR DETAILS
    ---------------------------------------------------------------------------
    DECLARE @ActorSeed TABLE ([FullName] nvarchar(200) NOT NULL PRIMARY KEY);
    INSERT INTO @ActorSeed ([FullName]) VALUES
        (N'Robert Downey Jr.'),
        (N'Gwyneth Paltrow'),
        (N'Terrence Howard'),
        (N'Jeff Bridges'),
        (N'Don Cheadle'),
        (N'Scarlett Johansson'),
        (N'Mickey Rourke'),
        (N'Sam Rockwell'),
        (N'Christian Bale'),
        (N'Hugh Jackman'),
        (N'Michael Caine'),
        (N'Rebecca Hall'),
        (N'David Bowie'),
        (N'Liam Neeson'),
        (N'Katie Holmes'),
        (N'Gary Oldman'),
        (N'Cillian Murphy'),
        (N'Morgan Freeman'),
        (N'Heath Ledger'),
        (N'Aaron Eckhart'),
        (N'Maggie Gyllenhaal'),
        (N'Tom Hardy'),
        (N'Anne Hathaway'),
        (N'Joseph Gordon-Levitt'),
        (N'Marion Cotillard'),
        (N'Matthew McConaughey'),
        (N'Jessica Chastain'),
        (N'Mackenzie Foy'),
        (N'Matt Damon'),
        (N'Emilia Clarke'),
        (N'Kit Harington'),
        (N'Peter Dinklage'),
        (N'Lena Headey'),
        (N'Nikolaj Coster-Waldau'),
        (N'Sophie Turner'),
        (N'Maisie Williams'),
        (N'Sean Bean'),
        (N'Michelle Fairley'),
        (N'Mark Addy'),
        (N'Megumi Ogata'),
        (N'Akari Kito'),
        (N'Shoya Chiba'),
        (N'Kurumi Mamiya'),
        (N'Reina Ueda'),
        (N'Konomi Kohara'),
        (N'Anna Nagase'),
        (N'Jason Ritter'),
        (N'Kristen Schaal'),
        (N'Alex Hirsch'),
        (N'Linda Cardellini'),
        (N'J.K. Simmons'),
        (N'Eden Sher'),
        (N'Adam McArthur'),
        (N'Jenny Slate'),
        (N'Alan Tudyk'),
        (N'Nia Vardalos'),
        (N'Woo Do-hwan'),
        (N'Lee Sang-yi'),
        (N'Huh Joon-ho'),
        (N'Park Sung-woong'),
        (N'Kim Sae-ron');

    INSERT INTO dbo.Actors ([FullName], [ProfileImageUrl])
    SELECT s.[FullName], NULL
    FROM @ActorSeed s
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Actors a WHERE a.[FullName] = s.[FullName]);

    DECLARE @ActorDetailSeed TABLE (
        [FullName] nvarchar(200) NOT NULL PRIMARY KEY,
        [Biography] nvarchar(max) NULL,
        [BirthDate] date NULL,
        [BirthPlace] nvarchar(200) NULL
    );
    INSERT INTO @ActorDetailSeed ([FullName],[Biography],[BirthDate],[BirthPlace]) VALUES
        (N'Robert Downey Jr.', N'American actor represented in this catalog by the Iron Man films.', CAST(N'1965-04-04' AS date), N'New York City, New York, USA'),
        (N'Christian Bale', N'Actor represented in this catalog by The Prestige and Christopher Nolan''s Batman trilogy.', CAST(N'1974-01-30' AS date), N'Haverfordwest, Wales, UK'),
        (N'Hugh Jackman', N'Australian actor represented in this catalog by The Prestige.', CAST(N'1968-10-12' AS date), N'Sydney, New South Wales, Australia'),
        (N'Scarlett Johansson', N'Actor represented in this catalog by Iron Man 2 and The Prestige.', CAST(N'1984-11-22' AS date), N'New York City, New York, USA'),
        (N'Matthew McConaughey', N'American actor represented in this catalog by Interstellar.', CAST(N'1969-11-04' AS date), N'Uvalde, Texas, USA'),
        (N'Anne Hathaway', N'Actor represented in this catalog by The Dark Knight Rises and Interstellar.', CAST(N'1982-11-12' AS date), N'Brooklyn, New York City, New York, USA'),
        (N'Emilia Clarke', N'Actor represented in this catalog by Game of Thrones.', CAST(N'1986-10-23' AS date), N'London, England, UK'),
        (N'Kit Harington', N'Actor represented in this catalog by Game of Thrones.', CAST(N'1986-12-26' AS date), N'London, England, UK'),
        (N'Peter Dinklage', N'Actor represented in this catalog by Game of Thrones.', CAST(N'1969-06-11' AS date), N'Morristown, New Jersey, USA'),
        (N'Megumi Ogata', N'Japanese voice actor represented in this catalog by Toilet-Bound Hanako-kun.', NULL, N'Tokyo, Japan'),
        (N'Kurumi Mamiya', N'Japanese voice actor represented in this catalog by Takopi''s Original Sin.', NULL, N'Japan'),
        (N'Jason Ritter', N'American actor and voice actor represented in this catalog by Gravity Falls.', CAST(N'1980-02-17' AS date), N'Los Angeles, California, USA'),
        (N'Eden Sher', N'American actor and voice actor represented in this catalog by Star vs. the Forces of Evil.', CAST(N'1991-12-26' AS date), N'Los Angeles, California, USA'),
        (N'Woo Do-hwan', N'South Korean actor represented in this catalog by Bloodhounds.', CAST(N'1992-07-12' AS date), N'Anyang, Gyeonggi Province, South Korea');

    INSERT INTO dbo.ActorDetails ([ActorId],[Biography],[BirthDate],[BirthPlace])
    SELECT a.[Id], s.[Biography], s.[BirthDate], s.[BirthPlace]
    FROM @ActorDetailSeed s
    JOIN dbo.Actors a ON a.[FullName] = s.[FullName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.ActorDetails d WHERE d.[ActorId] = a.[Id]);

    ---------------------------------------------------------------------------
    -- 3) DIRECTORS + DIRECTOR DETAILS
    ---------------------------------------------------------------------------
    DECLARE @DirectorSeed TABLE ([FullName] nvarchar(200) NOT NULL PRIMARY KEY);
    INSERT INTO @DirectorSeed ([FullName]) VALUES
        (N'Jon Favreau'),
        (N'Christopher Nolan'),
        (N'Timothy Van Patten'),
        (N'Miguel Sapochnik'),
        (N'Shin''ya Iino'),
        (N'Michael Mullen'),
        (N'Kim Joo-hwan');

    INSERT INTO dbo.Directors ([FullName],[ProfileImageUrl])
    SELECT s.[FullName], NULL FROM @DirectorSeed s
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Directors d WHERE d.[FullName] = s.[FullName]);

    DECLARE @DirectorDetailSeed TABLE (
        [FullName] nvarchar(200) NOT NULL PRIMARY KEY,
        [Biography] nvarchar(max) NULL,
        [BirthDate] date NULL,
        [BirthPlace] nvarchar(200) NULL
    );
    INSERT INTO @DirectorDetailSeed ([FullName],[Biography],[BirthDate],[BirthPlace]) VALUES
        (N'Jon Favreau', N'American filmmaker represented in this catalog as director of Iron Man and Iron Man 2.', CAST(N'1966-10-19' AS date), N'Queens, New York City, New York, USA'),
        (N'Christopher Nolan', N'Filmmaker represented in this catalog by The Prestige, the Dark Knight trilogy, and Interstellar.', CAST(N'1970-07-30' AS date), N'London, England, UK'),
        (N'Timothy Van Patten', N'Television director represented in this catalog by Game of Thrones.', NULL, N'USA'),
        (N'Miguel Sapochnik', N'Film and television director represented in this catalog by Game of Thrones.', NULL, N'UK'),
        (N'Shin''ya Iino', N'Anime director represented in this catalog by Takopi''s Original Sin.', NULL, N'Japan'),
        (N'Michael Mullen', N'Animation director represented in this catalog by Star vs. the Forces of Evil.', NULL, N'USA'),
        (N'Kim Joo-hwan', N'South Korean filmmaker represented in this catalog by Bloodhounds.', NULL, N'South Korea');

    INSERT INTO dbo.DirectorDetails ([DirectorId],[Biography],[BirthDate],[BirthPlace])
    SELECT d.[Id], s.[Biography], s.[BirthDate], s.[BirthPlace]
    FROM @DirectorDetailSeed s
    JOIN dbo.Directors d ON d.[FullName] = s.[FullName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.DirectorDetails x WHERE x.[DirectorId] = d.[Id]);

    ---------------------------------------------------------------------------
    -- 4) WRITERS + WRITER DETAILS
    ---------------------------------------------------------------------------
    DECLARE @WriterSeed TABLE ([FullName] nvarchar(200) NOT NULL PRIMARY KEY);
    INSERT INTO @WriterSeed ([FullName]) VALUES
        (N'Mark Fergus'),
        (N'Hawk Ostby'),
        (N'Art Marcum'),
        (N'Matt Holloway'),
        (N'Justin Theroux'),
        (N'Christopher Nolan'),
        (N'Jonathan Nolan'),
        (N'David S. Goyer'),
        (N'David Benioff'),
        (N'D.B. Weiss'),
        (N'George R.R. Martin'),
        (N'Shin''ya Iino'),
        (N'Ko Nekota'),
        (N'Taizan 5'),
        (N'Daron Nefcy'),
        (N'Dave Wasson'),
        (N'Jordana Arkin'),
        (N'Michael Mullen'),
        (N'Alex Hirsch'),
        (N'Kim Joo-hwan');

    INSERT INTO dbo.Writers ([FullName],[ProfileImageUrl])
    SELECT s.[FullName], NULL FROM @WriterSeed s
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Writers w WHERE w.[FullName] = s.[FullName]);

    DECLARE @WriterDetailSeed TABLE (
        [FullName] nvarchar(200) NOT NULL PRIMARY KEY,
        [Biography] nvarchar(max) NULL,
        [BirthDate] date NULL,
        [BirthPlace] nvarchar(200) NULL
    );
    INSERT INTO @WriterDetailSeed ([FullName],[Biography],[BirthDate],[BirthPlace]) VALUES
        (N'Christopher Nolan', N'Writer represented in this catalog by The Prestige, the Dark Knight trilogy, and Interstellar.', CAST(N'1970-07-30' AS date), N'London, England, UK'),
        (N'Jonathan Nolan', N'Writer represented in this catalog by The Prestige, The Dark Knight, The Dark Knight Rises, and Interstellar.', NULL, N'London, England, UK'),
        (N'David S. Goyer', N'Screenwriter represented in this catalog by Christopher Nolan''s Batman films.', NULL, N'USA'),
        (N'David Benioff', N'Writer and producer represented in this catalog by Game of Thrones.', NULL, N'USA'),
        (N'D.B. Weiss', N'Writer and producer represented in this catalog by Game of Thrones.', NULL, N'USA'),
        (N'George R.R. Martin', N'Author represented in this catalog through Game of Thrones source material.', CAST(N'1948-09-20' AS date), N'Bayonne, New Jersey, USA'),
        (N'Alex Hirsch', N'Writer and creator represented in this catalog by Gravity Falls.', CAST(N'1985-06-18' AS date), N'Piedmont, California, USA'),
        (N'Daron Nefcy', N'Writer and creator represented in this catalog by Star vs. the Forces of Evil.', NULL, N'USA'),
        (N'Taizan 5', N'Manga creator represented in this catalog by Takopi''s Original Sin.', NULL, N'Japan'),
        (N'Kim Joo-hwan', N'Writer-director represented in this catalog by Bloodhounds.', NULL, N'South Korea');

    INSERT INTO dbo.WriterDetails ([WriterId],[Biography],[BirthDate],[BirthPlace])
    SELECT w.[Id], s.[Biography], s.[BirthDate], s.[BirthPlace]
    FROM @WriterDetailSeed s
    JOIN dbo.Writers w ON w.[FullName] = s.[FullName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.WriterDetails x WHERE x.[WriterId] = w.[Id]);

    ---------------------------------------------------------------------------
    -- 5) MOVIES + MOVIE DETAILS
    ---------------------------------------------------------------------------
    DECLARE @MovieSeed TABLE (
        [Title] nvarchar(300) NOT NULL, [ReleaseDate] date NOT NULL,
        [RuntimeMinutes] int NOT NULL, [ContentRating] nvarchar(50) NULL,
        [Synopsis] nvarchar(max) NOT NULL
    );
    INSERT INTO @MovieSeed ([Title],[ReleaseDate],[RuntimeMinutes],[ContentRating],[Synopsis]) VALUES
        (N'Iron Man', CAST(N'2008-05-02' AS date), 126, N'PG-13', N'After being captured, industrialist Tony Stark builds an armored suit and rethinks how his technology should be used.'),
        (N'Iron Man 2', CAST(N'2010-05-07' AS date), 124, N'PG-13', N'Tony Stark faces political pressure, new rivals, and the consequences of the technology behind the Iron Man armor.'),
        (N'The Prestige', CAST(N'2006-10-20' AS date), 130, N'PG-13', N'Two rival stage magicians turn professional competition into a dangerous obsession with secrets, sacrifice, and illusion.'),
        (N'Batman Begins', CAST(N'2005-06-15' AS date), 140, N'PG-13', N'Bruce Wayne returns to Gotham and becomes Batman while confronting corruption and a threat aimed at the entire city.'),
        (N'The Dark Knight', CAST(N'2008-07-18' AS date), 152, N'PG-13', N'Batman, Gordon, and Harvey Dent face a criminal mastermind whose campaign of chaos pushes Gotham to its limits.'),
        (N'The Dark Knight Rises', CAST(N'2012-07-20' AS date), 164, N'PG-13', N'Years after Batman disappears, a new enemy forces Bruce Wayne to return while Gotham faces collapse.'),
        (N'Interstellar', CAST(N'2014-11-07' AS date), 169, N'PG-13', N'A former pilot joins a mission through a wormhole in search of a future home for humanity.');

    INSERT INTO dbo.Movies ([Title],[PosterUrl],[TrailerUrl],[ReleaseDate],[ContentRating],[RuntimeMinutes],[Synopsis])
    SELECT s.[Title], NULL, NULL, s.[ReleaseDate], s.[ContentRating], s.[RuntimeMinutes], s.[Synopsis]
    FROM @MovieSeed s
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Movies m
        WHERE m.[Title] = s.[Title] AND CAST(m.[ReleaseDate] AS date) = s.[ReleaseDate]
    );

    DECLARE @MovieDetailSeed TABLE (
        [Title] nvarchar(300) NOT NULL PRIMARY KEY,
        [Storyline] nvarchar(max) NULL, [Tagline] nvarchar(max) NULL,
        [OriginalLanguage] nvarchar(max) NULL, [CountryOfOrigin] nvarchar(max) NULL,
        [ProductionCompany] nvarchar(max) NULL, [Color] nvarchar(max) NULL
    );
    INSERT INTO @MovieDetailSeed ([Title],[Storyline],[Tagline],[OriginalLanguage],[CountryOfOrigin],[ProductionCompany],[Color]) VALUES
        (N'Iron Man', N'Tony Stark survives captivity by creating a powered suit, then returns home determined to change the direction of his company.', N'Heroes aren''t born. They''re built.', N'English', N'USA', N'Marvel Studios', N'Color'),
        (N'Iron Man 2', N'With the world aware that Tony Stark is Iron Man, he faces pressure to surrender his technology while new enemies emerge.', NULL, N'English', N'USA', N'Marvel Studios', N'Color'),
        (N'The Prestige', N'A feud between two magicians escalates as each tries to discover and destroy the other''s greatest illusion.', N'Are you watching closely?', N'English', N'USA / UK', N'Touchstone Pictures / Warner Bros. Pictures / Syncopy', N'Color'),
        (N'Batman Begins', N'Bruce Wayne trains abroad, returns to Gotham, and creates a symbol intended to fight fear with fear.', N'Evil fears the knight.', N'English', N'USA / UK', N'Warner Bros. Pictures / Syncopy', N'Color'),
        (N'The Dark Knight', N'Batman confronts the Joker while Gotham''s hopes become tied to district attorney Harvey Dent.', N'Why so serious?', N'English', N'USA / UK', N'Warner Bros. Pictures / Syncopy', N'Color'),
        (N'The Dark Knight Rises', N'Bruce Wayne is drawn back into action when Bane seizes control of Gotham and challenges both Batman and the city.', N'The legend ends.', N'English', N'USA / UK', N'Warner Bros. Pictures / Syncopy', N'Color'),
        (N'Interstellar', N'As Earth becomes less hospitable, a group of explorers crosses a wormhole to search for another place humanity can survive.', N'Mankind was born on Earth. It was never meant to die here.', N'English', N'USA / UK', N'Paramount Pictures / Warner Bros. Pictures / Syncopy', N'Color');

    INSERT INTO dbo.MovieDetails ([MovieId],[Storyline],[Tagline],[OriginalLanguage],[CountryOfOrigin],[ProductionCompany],[Color])
    SELECT m.[Id], s.[Storyline], s.[Tagline], s.[OriginalLanguage], s.[CountryOfOrigin], s.[ProductionCompany], s.[Color]
    FROM @MovieDetailSeed s
    JOIN dbo.Movies m ON m.[Title] = s.[Title]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.MovieDetails d WHERE d.[MovieId] = m.[Id]);

    ---------------------------------------------------------------------------
    -- 6) MOVIE <-> GENRE
    ---------------------------------------------------------------------------
    DECLARE @MovieGenreSeed TABLE ([MovieTitle] nvarchar(300), [GenreName] nvarchar(100));
    INSERT INTO @MovieGenreSeed ([MovieTitle],[GenreName]) VALUES
        (N'Iron Man', N'Action'),
        (N'Iron Man', N'Adventure'),
        (N'Iron Man', N'Sci-Fi'),
        (N'Iron Man 2', N'Action'),
        (N'Iron Man 2', N'Adventure'),
        (N'Iron Man 2', N'Sci-Fi'),
        (N'The Prestige', N'Drama'),
        (N'The Prestige', N'Mystery'),
        (N'The Prestige', N'Sci-Fi'),
        (N'The Prestige', N'Thriller'),
        (N'Batman Begins', N'Action'),
        (N'Batman Begins', N'Crime'),
        (N'Batman Begins', N'Drama'),
        (N'Batman Begins', N'Thriller'),
        (N'The Dark Knight', N'Action'),
        (N'The Dark Knight', N'Crime'),
        (N'The Dark Knight', N'Drama'),
        (N'The Dark Knight', N'Thriller'),
        (N'The Dark Knight Rises', N'Action'),
        (N'The Dark Knight Rises', N'Drama'),
        (N'The Dark Knight Rises', N'Thriller'),
        (N'Interstellar', N'Adventure'),
        (N'Interstellar', N'Drama'),
        (N'Interstellar', N'Sci-Fi');

    INSERT INTO dbo.MovieGenres ([MovieId],[GenreId])
    SELECT m.[Id], g.[Id]
    FROM @MovieGenreSeed s
    JOIN dbo.Movies m ON m.[Title] = s.[MovieTitle]
    JOIN dbo.Genres g ON g.[Name] = s.[GenreName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.MovieGenres x WHERE x.[MovieId]=m.[Id] AND x.[GenreId]=g.[Id]);

    ---------------------------------------------------------------------------
    -- 7) MOVIE <-> DIRECTOR
    ---------------------------------------------------------------------------
    DECLARE @MovieDirectorSeed TABLE ([MovieTitle] nvarchar(300), [DirectorName] nvarchar(200));
    INSERT INTO @MovieDirectorSeed ([MovieTitle],[DirectorName]) VALUES
        (N'Iron Man', N'Jon Favreau'),
        (N'Iron Man 2', N'Jon Favreau'),
        (N'The Prestige', N'Christopher Nolan'),
        (N'Batman Begins', N'Christopher Nolan'),
        (N'The Dark Knight', N'Christopher Nolan'),
        (N'The Dark Knight Rises', N'Christopher Nolan'),
        (N'Interstellar', N'Christopher Nolan');

    INSERT INTO dbo.MovieDirectors ([MovieId],[DirectorId])
    SELECT m.[Id], d.[Id]
    FROM @MovieDirectorSeed s
    JOIN dbo.Movies m ON m.[Title] = s.[MovieTitle]
    JOIN dbo.Directors d ON d.[FullName] = s.[DirectorName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.MovieDirectors x WHERE x.[MovieId]=m.[Id] AND x.[DirectorId]=d.[Id]);

    ---------------------------------------------------------------------------
    -- 8) MOVIE <-> WRITER
    ---------------------------------------------------------------------------
    DECLARE @MovieWriterSeed TABLE ([MovieTitle] nvarchar(300), [WriterName] nvarchar(200));
    INSERT INTO @MovieWriterSeed ([MovieTitle],[WriterName]) VALUES
        (N'Iron Man', N'Mark Fergus'),
        (N'Iron Man', N'Hawk Ostby'),
        (N'Iron Man', N'Art Marcum'),
        (N'Iron Man', N'Matt Holloway'),
        (N'Iron Man 2', N'Justin Theroux'),
        (N'The Prestige', N'Jonathan Nolan'),
        (N'The Prestige', N'Christopher Nolan'),
        (N'Batman Begins', N'Christopher Nolan'),
        (N'Batman Begins', N'David S. Goyer'),
        (N'The Dark Knight', N'Jonathan Nolan'),
        (N'The Dark Knight', N'Christopher Nolan'),
        (N'The Dark Knight', N'David S. Goyer'),
        (N'The Dark Knight Rises', N'Jonathan Nolan'),
        (N'The Dark Knight Rises', N'Christopher Nolan'),
        (N'The Dark Knight Rises', N'David S. Goyer'),
        (N'Interstellar', N'Jonathan Nolan'),
        (N'Interstellar', N'Christopher Nolan');

    INSERT INTO dbo.MovieWriters ([MovieId],[WriterId])
    SELECT m.[Id], w.[Id]
    FROM @MovieWriterSeed s
    JOIN dbo.Movies m ON m.[Title] = s.[MovieTitle]
    JOIN dbo.Writers w ON w.[FullName] = s.[WriterName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.MovieWriters x WHERE x.[MovieId]=m.[Id] AND x.[WriterId]=w.[Id]);

    ---------------------------------------------------------------------------
    -- 9) MOVIE <-> ACTOR / CAST
    ---------------------------------------------------------------------------
    DECLARE @MovieActorSeed TABLE ([MovieTitle] nvarchar(300), [ActorName] nvarchar(200), [CharacterName] nvarchar(200), [CastOrder] int);
    INSERT INTO @MovieActorSeed ([MovieTitle],[ActorName],[CharacterName],[CastOrder]) VALUES
        (N'Iron Man', N'Robert Downey Jr.', N'Tony Stark / Iron Man', 1),
        (N'Iron Man', N'Gwyneth Paltrow', N'Pepper Potts', 2),
        (N'Iron Man', N'Terrence Howard', N'James Rhodes', 3),
        (N'Iron Man', N'Jeff Bridges', N'Obadiah Stane', 4),
        (N'Iron Man 2', N'Robert Downey Jr.', N'Tony Stark / Iron Man', 1),
        (N'Iron Man 2', N'Gwyneth Paltrow', N'Pepper Potts', 2),
        (N'Iron Man 2', N'Don Cheadle', N'James Rhodes / War Machine', 3),
        (N'Iron Man 2', N'Scarlett Johansson', N'Natasha Romanoff / Black Widow', 4),
        (N'Iron Man 2', N'Mickey Rourke', N'Ivan Vanko', 5),
        (N'Iron Man 2', N'Sam Rockwell', N'Justin Hammer', 6),
        (N'The Prestige', N'Christian Bale', N'Alfred Borden', 1),
        (N'The Prestige', N'Hugh Jackman', N'Robert Angier', 2),
        (N'The Prestige', N'Michael Caine', N'Cutter', 3),
        (N'The Prestige', N'Scarlett Johansson', N'Olivia Wenscombe', 4),
        (N'The Prestige', N'Rebecca Hall', N'Sarah Borden', 5),
        (N'The Prestige', N'David Bowie', N'Nikola Tesla', 6),
        (N'Batman Begins', N'Christian Bale', N'Bruce Wayne / Batman', 1),
        (N'Batman Begins', N'Michael Caine', N'Alfred Pennyworth', 2),
        (N'Batman Begins', N'Liam Neeson', N'Henri Ducard / Ra''s al Ghul', 3),
        (N'Batman Begins', N'Katie Holmes', N'Rachel Dawes', 4),
        (N'Batman Begins', N'Gary Oldman', N'James Gordon', 5),
        (N'Batman Begins', N'Cillian Murphy', N'Jonathan Crane / Scarecrow', 6),
        (N'Batman Begins', N'Morgan Freeman', N'Lucius Fox', 7),
        (N'The Dark Knight', N'Christian Bale', N'Bruce Wayne / Batman', 1),
        (N'The Dark Knight', N'Heath Ledger', N'Joker', 2),
        (N'The Dark Knight', N'Aaron Eckhart', N'Harvey Dent / Two-Face', 3),
        (N'The Dark Knight', N'Michael Caine', N'Alfred Pennyworth', 4),
        (N'The Dark Knight', N'Maggie Gyllenhaal', N'Rachel Dawes', 5),
        (N'The Dark Knight', N'Gary Oldman', N'James Gordon', 6),
        (N'The Dark Knight', N'Morgan Freeman', N'Lucius Fox', 7),
        (N'The Dark Knight Rises', N'Christian Bale', N'Bruce Wayne / Batman', 1),
        (N'The Dark Knight Rises', N'Tom Hardy', N'Bane', 2),
        (N'The Dark Knight Rises', N'Anne Hathaway', N'Selina Kyle', 3),
        (N'The Dark Knight Rises', N'Michael Caine', N'Alfred Pennyworth', 4),
        (N'The Dark Knight Rises', N'Gary Oldman', N'James Gordon', 5),
        (N'The Dark Knight Rises', N'Joseph Gordon-Levitt', N'John Blake', 6),
        (N'The Dark Knight Rises', N'Marion Cotillard', N'Miranda Tate', 7),
        (N'The Dark Knight Rises', N'Morgan Freeman', N'Lucius Fox', 8),
        (N'Interstellar', N'Matthew McConaughey', N'Cooper', 1),
        (N'Interstellar', N'Anne Hathaway', N'Amelia Brand', 2),
        (N'Interstellar', N'Jessica Chastain', N'Murph', 3),
        (N'Interstellar', N'Mackenzie Foy', N'Young Murph', 4),
        (N'Interstellar', N'Michael Caine', N'Professor Brand', 5),
        (N'Interstellar', N'Matt Damon', N'Dr. Mann', 6);

    INSERT INTO dbo.MovieActors ([MovieId],[ActorId],[CharacterName],[CastOrder])
    SELECT m.[Id], ac.[Id], s.[CharacterName], s.[CastOrder]
    FROM @MovieActorSeed s
    JOIN dbo.Movies m ON m.[Title] = s.[MovieTitle]
    JOIN dbo.Actors ac ON ac.[FullName] = s.[ActorName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.MovieActors x WHERE x.[MovieId]=m.[Id] AND x.[ActorId]=ac.[Id]);

    ---------------------------------------------------------------------------
    -- 10) TV SHOWS + TV SHOW DETAILS
    ---------------------------------------------------------------------------
    DECLARE @TVSeed TABLE (
        [Title] nvarchar(300) NOT NULL, [OriginalTitle] nvarchar(300) NULL,
        [ReleaseDate] date NOT NULL, [EndDate] date NULL, [ContentRating] nvarchar(50) NULL,
        [RuntimeMinutes] int NULL, [Synopsis] nvarchar(max) NOT NULL
    );
    INSERT INTO @TVSeed ([Title],[OriginalTitle],[ReleaseDate],[EndDate],[ContentRating],[RuntimeMinutes],[Synopsis]) VALUES
        (N'Game of Thrones', N'Game of Thrones', CAST(N'2011-04-17' AS date), CAST(N'2019-05-19' AS date), N'TV-MA', 57, N'Noble families struggle for power while an ancient threat grows beyond the political conflicts of Westeros.'),
        (N'Toilet-Bound Hanako-kun', N'Jibaku Shōnen Hanako-kun', CAST(N'2020-01-09' AS date), NULL, N'TV-14', 24, N'A student fascinated by school mysteries meets Hanako, a supernatural figure connected to the academy''s Seven Wonders.'),
        (N'Takopi''s Original Sin', N'Takopī no Genzai', CAST(N'2025-06-28' AS date), CAST(N'2025-08-02' AS date), N'TV-14', 24, N'A cheerful alien tries to help a deeply unhappy child, but innocent attempts to fix human problems lead to painful consequences.'),
        (N'Gravity Falls', N'Gravity Falls', CAST(N'2012-06-15' AS date), CAST(N'2016-02-15' AS date), N'TV-Y7', 23, N'Twins Dipper and Mabel spend the summer in a strange Oregon town filled with supernatural mysteries.'),
        (N'Star vs. the Forces of Evil', N'Star vs. the Forces of Evil', CAST(N'2015-01-18' AS date), CAST(N'2019-05-19' AS date), N'TV-Y7', 22, N'A magical princess from another dimension moves to Earth and has chaotic adventures with her friend Marco.'),
        (N'Bloodhounds', N'사냥개들', CAST(N'2023-06-09' AS date), NULL, N'TV-MA', 60, N'Two young boxers take on a ruthless money-lending organization while trying to protect people caught in its debt traps.');

    INSERT INTO dbo.TVShows ([Title],[OriginalTitle],[PosterUrl],[TrailerUrl],[Synopsis],[ReleaseDate],[EndDate],[ContentRating],[RuntimeMinutes])
    SELECT s.[Title], s.[OriginalTitle], NULL, NULL, s.[Synopsis], s.[ReleaseDate], s.[EndDate], s.[ContentRating], s.[RuntimeMinutes]
    FROM @TVSeed s
    WHERE NOT EXISTS (SELECT 1 FROM dbo.TVShows t WHERE t.[Title]=s.[Title] AND CAST(t.[ReleaseDate] AS date)=s.[ReleaseDate]);

    DECLARE @TVDetailSeed TABLE (
        [Title] nvarchar(300) NOT NULL PRIMARY KEY, [Storyline] nvarchar(max) NULL,
        [OriginalLanguage] nvarchar(max) NULL, [CountryOfOrigin] nvarchar(max) NULL,
        [ProductionCompany] nvarchar(max) NULL, [Color] nvarchar(max) NULL
    );
    INSERT INTO @TVDetailSeed ([Title],[Storyline],[OriginalLanguage],[CountryOfOrigin],[ProductionCompany],[Color]) VALUES
        (N'Game of Thrones', N'Rival houses compete for the Iron Throne while conflicts across Westeros gradually converge with a supernatural danger.', N'English', N'USA / UK', N'HBO', N'Color'),
        (N'Toilet-Bound Hanako-kun', N'Nene Yashiro becomes involved with Hanako and the supernatural mysteries surrounding Kamome Academy.', N'Japanese', N'Japan', N'Lerche', N'Color'),
        (N'Takopi''s Original Sin', N'Takopi arrives on Earth hoping to spread happiness and becomes determined to help Shizuka, even though he does not understand the full weight of her situation.', N'Japanese', N'Japan', N'Enishiya', N'Color'),
        (N'Gravity Falls', N'Dipper discovers a mysterious journal and investigates the increasingly strange events surrounding Gravity Falls with Mabel.', N'English', N'USA', N'Disney Television Animation', N'Color'),
        (N'Star vs. the Forces of Evil', N'Star Butterfly brings powerful magic to Earth, where friendship and interdimensional conflicts reshape her understanding of her kingdom.', N'English', N'USA', N'Disney Television Animation', N'Color'),
        (N'Bloodhounds', N'Boxers Gun-woo and Woo-jin become involved in a fight against a predatory loan-shark network.', N'Korean', N'South Korea', N'Studio N', N'Color');

    INSERT INTO dbo.TVShowDetails ([TVShowId],[Storyline],[OriginalLanguage],[CountryOfOrigin],[ProductionCompany],[Color])
    SELECT t.[Id], s.[Storyline], s.[OriginalLanguage], s.[CountryOfOrigin], s.[ProductionCompany], s.[Color]
    FROM @TVDetailSeed s
    JOIN dbo.TVShows t ON t.[Title]=s.[Title]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.TVShowDetails d WHERE d.[TVShowId]=t.[Id]);

    ---------------------------------------------------------------------------
    -- 11) TV SHOW <-> GENRE
    ---------------------------------------------------------------------------
    DECLARE @TVGenreSeed TABLE ([TVTitle] nvarchar(300), [GenreName] nvarchar(100));
    INSERT INTO @TVGenreSeed ([TVTitle],[GenreName]) VALUES
        (N'Game of Thrones', N'Action'),
        (N'Game of Thrones', N'Adventure'),
        (N'Game of Thrones', N'Drama'),
        (N'Game of Thrones', N'Fantasy'),
        (N'Toilet-Bound Hanako-kun', N'Animation'),
        (N'Toilet-Bound Hanako-kun', N'Comedy'),
        (N'Toilet-Bound Hanako-kun', N'Fantasy'),
        (N'Toilet-Bound Hanako-kun', N'Mystery'),
        (N'Toilet-Bound Hanako-kun', N'Romance'),
        (N'Toilet-Bound Hanako-kun', N'Supernatural'),
        (N'Takopi''s Original Sin', N'Animation'),
        (N'Takopi''s Original Sin', N'Drama'),
        (N'Takopi''s Original Sin', N'Sci-Fi'),
        (N'Takopi''s Original Sin', N'Psychological'),
        (N'Gravity Falls', N'Animation'),
        (N'Gravity Falls', N'Adventure'),
        (N'Gravity Falls', N'Comedy'),
        (N'Gravity Falls', N'Fantasy'),
        (N'Gravity Falls', N'Mystery'),
        (N'Gravity Falls', N'Family'),
        (N'Star vs. the Forces of Evil', N'Animation'),
        (N'Star vs. the Forces of Evil', N'Adventure'),
        (N'Star vs. the Forces of Evil', N'Comedy'),
        (N'Star vs. the Forces of Evil', N'Fantasy'),
        (N'Star vs. the Forces of Evil', N'Family'),
        (N'Bloodhounds', N'Action'),
        (N'Bloodhounds', N'Crime'),
        (N'Bloodhounds', N'Drama'),
        (N'Bloodhounds', N'Thriller');

    INSERT INTO dbo.TVShowGenres ([TVShowId],[GenreId])
    SELECT t.[Id], g.[Id]
    FROM @TVGenreSeed s
    JOIN dbo.TVShows t ON t.[Title]=s.[TVTitle]
    JOIN dbo.Genres g ON g.[Name]=s.[GenreName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.TVShowGenres x WHERE x.[TVShowId]=t.[Id] AND x.[GenreId]=g.[Id]);

    ---------------------------------------------------------------------------
    -- 12) TV SHOW <-> ACTOR / CAST
    ---------------------------------------------------------------------------
    DECLARE @TVActorSeed TABLE ([TVTitle] nvarchar(300), [ActorName] nvarchar(200), [CharacterName] nvarchar(200), [CastOrder] int);
    INSERT INTO @TVActorSeed ([TVTitle],[ActorName],[CharacterName],[CastOrder]) VALUES
        (N'Game of Thrones', N'Emilia Clarke', N'Daenerys Targaryen', 1),
        (N'Game of Thrones', N'Kit Harington', N'Jon Snow', 2),
        (N'Game of Thrones', N'Peter Dinklage', N'Tyrion Lannister', 3),
        (N'Game of Thrones', N'Lena Headey', N'Cersei Lannister', 4),
        (N'Game of Thrones', N'Nikolaj Coster-Waldau', N'Jaime Lannister', 5),
        (N'Game of Thrones', N'Sophie Turner', N'Sansa Stark', 6),
        (N'Game of Thrones', N'Maisie Williams', N'Arya Stark', 7),
        (N'Game of Thrones', N'Sean Bean', N'Eddard Stark', 8),
        (N'Game of Thrones', N'Michelle Fairley', N'Catelyn Stark', 9),
        (N'Game of Thrones', N'Mark Addy', N'Robert Baratheon', 10),
        (N'Toilet-Bound Hanako-kun', N'Megumi Ogata', N'Hanako', 1),
        (N'Toilet-Bound Hanako-kun', N'Akari Kito', N'Nene Yashiro', 2),
        (N'Toilet-Bound Hanako-kun', N'Shoya Chiba', N'Kou Minamoto', 3),
        (N'Takopi''s Original Sin', N'Kurumi Mamiya', N'Takopi', 1),
        (N'Takopi''s Original Sin', N'Reina Ueda', N'Shizuka Kuze', 2),
        (N'Takopi''s Original Sin', N'Konomi Kohara', N'Marina Kirarazaka', 3),
        (N'Takopi''s Original Sin', N'Anna Nagase', N'Naoki Azuma', 4),
        (N'Gravity Falls', N'Jason Ritter', N'Dipper Pines', 1),
        (N'Gravity Falls', N'Kristen Schaal', N'Mabel Pines', 2),
        (N'Gravity Falls', N'Alex Hirsch', N'Grunkle Stan / Soos / Bill Cipher', 3),
        (N'Gravity Falls', N'Linda Cardellini', N'Wendy Corduroy', 4),
        (N'Gravity Falls', N'J.K. Simmons', N'Ford Pines', 5),
        (N'Star vs. the Forces of Evil', N'Eden Sher', N'Star Butterfly', 1),
        (N'Star vs. the Forces of Evil', N'Adam McArthur', N'Marco Diaz', 2),
        (N'Star vs. the Forces of Evil', N'Jenny Slate', N'Pony Head', 3),
        (N'Star vs. the Forces of Evil', N'Alan Tudyk', N'Ludo / King River Butterfly', 4),
        (N'Star vs. the Forces of Evil', N'Nia Vardalos', N'Angie Diaz', 5),
        (N'Bloodhounds', N'Woo Do-hwan', N'Kim Gun-woo', 1),
        (N'Bloodhounds', N'Lee Sang-yi', N'Hong Woo-jin', 2),
        (N'Bloodhounds', N'Huh Joon-ho', N'Choi Tae-ho', 3),
        (N'Bloodhounds', N'Park Sung-woong', N'Kim Myeong-gil', 4),
        (N'Bloodhounds', N'Kim Sae-ron', N'Cha Hyeon-ju', 5);

    INSERT INTO dbo.TVShowActors ([TVShowId],[ActorId],[CharacterName],[CastOrder])
    SELECT t.[Id], ac.[Id], s.[CharacterName], s.[CastOrder]
    FROM @TVActorSeed s
    JOIN dbo.TVShows t ON t.[Title]=s.[TVTitle]
    JOIN dbo.Actors ac ON ac.[FullName]=s.[ActorName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.TVShowActors x WHERE x.[TVShowId]=t.[Id] AND x.[ActorId]=ac.[Id]);

    ---------------------------------------------------------------------------
    -- 13) SEASONS
    ---------------------------------------------------------------------------
    DECLARE @SeasonSeed TABLE ([TVTitle] nvarchar(300), [SeasonNumber] int);
    INSERT INTO @SeasonSeed ([TVTitle],[SeasonNumber]) VALUES
        (N'Game of Thrones', 1),
        (N'Game of Thrones', 3),
        (N'Game of Thrones', 6),
        (N'Toilet-Bound Hanako-kun', 1),
        (N'Takopi''s Original Sin', 1),
        (N'Gravity Falls', 1),
        (N'Gravity Falls', 2),
        (N'Star vs. the Forces of Evil', 1),
        (N'Bloodhounds', 1);

    INSERT INTO dbo.Seasons ([SeasonNumber],[TVShowId])
    SELECT s.[SeasonNumber], t.[Id]
    FROM @SeasonSeed s
    JOIN dbo.TVShows t ON t.[Title]=s.[TVTitle]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Seasons x WHERE x.[TVShowId]=t.[Id] AND x.[SeasonNumber]=s.[SeasonNumber]);

    ---------------------------------------------------------------------------
    -- 14) EPISODES
    ---------------------------------------------------------------------------
    DECLARE @EpisodeSeed TABLE (
        [TVTitle] nvarchar(300), [SeasonNumber] int, [EpisodeNumber] int,
        [Title] nvarchar(300), [ReleaseDate] date NULL, [RuntimeMinutes] int NULL,
        [Description] nvarchar(max) NULL
    );
    INSERT INTO @EpisodeSeed ([TVTitle],[SeasonNumber],[EpisodeNumber],[Title],[ReleaseDate],[RuntimeMinutes],[Description]) VALUES
        (N'Game of Thrones', 1, 1, N'Winter Is Coming', CAST(N'2011-04-17' AS date), 62, N'The Stark family becomes entangled with royal politics while events beyond the Wall hint at an older danger.'),
        (N'Game of Thrones', 1, 2, N'The Kingsroad', CAST(N'2011-04-24' AS date), 56, N'The royal party travels south while Jon Snow heads north and tensions rise around the Stark family.'),
        (N'Game of Thrones', 1, 9, N'Baelor', CAST(N'2011-06-12' AS date), 57, N'Political choices in King''s Landing reach a devastating turning point.'),
        (N'Game of Thrones', 1, 10, N'Fire and Blood', CAST(N'2011-06-19' AS date), 53, N'The consequences of recent events reshape alliances across Westeros and beyond the Narrow Sea.'),
        (N'Game of Thrones', 3, 9, N'The Rains of Castamere', CAST(N'2013-06-02' AS date), 51, N'Several storylines collide during a wedding that changes the balance of the war.'),
        (N'Game of Thrones', 6, 9, N'Battle of the Bastards', CAST(N'2016-06-19' AS date), 60, N'Jon Snow and his allies fight for control of Winterfell while Daenerys faces an attack in Meereen.'),
        (N'Game of Thrones', 6, 10, N'The Winds of Winter', CAST(N'2016-06-26' AS date), 68, N'Major political and personal turning points reshape the struggle for power.'),
        (N'Toilet-Bound Hanako-kun', 1, 1, N'Hanako-san of the Bathroom', CAST(N'2020-01-09' AS date), 24, N'Nene Yashiro summons the famous school mystery Hanako-san and discovers the rumor is not quite what she expected.'),
        (N'Toilet-Bound Hanako-kun', 1, 2, N'Episode 2', CAST(N'2020-01-16' AS date), 24, N'Nene becomes more deeply involved in the supernatural incidents surrounding the school.'),
        (N'Toilet-Bound Hanako-kun', 1, 3, N'Episode 3', CAST(N'2020-01-23' AS date), 24, N'Hanako and Nene confront another mystery connected to Kamome Academy.'),
        (N'Takopi''s Original Sin', 1, 1, N'Episode 1', CAST(N'2025-06-28' AS date), 24, N'Takopi meets Shizuka and decides to use Happy Planet gadgets to make her smile.'),
        (N'Takopi''s Original Sin', 1, 2, N'Episode 2', CAST(N'2025-07-05' AS date), 24, N'Takopi''s attempts to solve Shizuka''s problems reveal how little he understands human pain.'),
        (N'Takopi''s Original Sin', 1, 3, N'Episode 3', CAST(N'2025-07-12' AS date), 24, N'The consequences of earlier choices become increasingly difficult to undo.'),
        (N'Takopi''s Original Sin', 1, 4, N'Episode 4', CAST(N'2025-07-19' AS date), 24, N'Relationships between the children are reexamined as Takopi continues searching for a solution.'),
        (N'Takopi''s Original Sin', 1, 5, N'Episode 5', CAST(N'2025-07-26' AS date), 24, N'Past actions and hidden motivations push the story toward its conclusion.'),
        (N'Takopi''s Original Sin', 1, 6, N'Episode 6', CAST(N'2025-08-02' AS date), 24, N'Takopi faces the final consequences of his attempt to bring happiness to the people he met.'),
        (N'Gravity Falls', 1, 1, N'Tourist Trapped', CAST(N'2012-06-15' AS date), 23, N'Dipper suspects that Mabel''s new boyfriend may not be what he seems.'),
        (N'Gravity Falls', 1, 2, N'The Legend of the Gobblewonker', CAST(N'2012-06-29' AS date), 23, N'Dipper and Mabel search the lake for a legendary creature.'),
        (N'Gravity Falls', 2, 18, N'Weirdmageddon Part 1', CAST(N'2015-10-26' AS date), 23, N'Gravity Falls falls into supernatural chaos after Bill Cipher gains new power.'),
        (N'Gravity Falls', 2, 19, N'Weirdmageddon 2: Escape from Reality', CAST(N'2015-11-23' AS date), 23, N'Dipper searches for Mabel while the town remains trapped in Weirdmageddon.'),
        (N'Gravity Falls', 2, 20, N'Weirdmageddon 3: Take Back The Falls', CAST(N'2016-02-15' AS date), 44, N'The residents of Gravity Falls unite for a final attempt to stop Bill Cipher.'),
        (N'Star vs. the Forces of Evil', 1, 1, N'Star Comes to Earth', CAST(N'2015-01-18' AS date), 11, N'Star Butterfly is sent to Earth and meets Marco Diaz after receiving a powerful magic wand.'),
        (N'Star vs. the Forces of Evil', 1, 2, N'Party with a Pony', CAST(N'2015-01-18' AS date), 11, N'Star tries to balance her old friendships with her new life on Earth.'),
        (N'Bloodhounds', 1, 1, N'Episode 1', CAST(N'2023-06-09' AS date), 60, N'Boxer Gun-woo crosses paths with Woo-jin as financial pressure and predatory lending close in.'),
        (N'Bloodhounds', 1, 2, N'Episode 2', CAST(N'2023-06-09' AS date), 60, N'Gun-woo and Woo-jin become more involved in the conflict surrounding a ruthless loan shark.'),
        (N'Bloodhounds', 1, 3, N'Episode 3', CAST(N'2023-06-09' AS date), 60, N'The two boxers commit themselves to protecting people targeted by the criminal lending operation.'),
        (N'Bloodhounds', 1, 8, N'Episode 8', CAST(N'2023-06-09' AS date), 60, N'The conflict reaches its final confrontation as Gun-woo and Woo-jin pursue justice.');

    INSERT INTO dbo.Episodes ([Title],[EpisodeNumber],[Description],[ReleaseDate],[RuntimeMinutes],[ImageUrl],[SeasonId])
    SELECT e.[Title], e.[EpisodeNumber], e.[Description], e.[ReleaseDate], e.[RuntimeMinutes], NULL, s.[Id]
    FROM @EpisodeSeed e
    JOIN dbo.TVShows t ON t.[Title]=e.[TVTitle]
    JOIN dbo.Seasons s ON s.[TVShowId]=t.[Id] AND s.[SeasonNumber]=e.[SeasonNumber]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Episodes x WHERE x.[SeasonId]=s.[Id] AND x.[EpisodeNumber]=e.[EpisodeNumber]);

    ---------------------------------------------------------------------------
    -- 15) EPISODE <-> ACTOR / CAST
    ---------------------------------------------------------------------------
    DECLARE @EpisodeShowCastSeed TABLE ([TVTitle] nvarchar(300), [ActorName] nvarchar(200), [CharacterName] nvarchar(200), [CastOrder] int);
    INSERT INTO @EpisodeShowCastSeed ([TVTitle],[ActorName],[CharacterName],[CastOrder]) VALUES
        (N'Game of Thrones', N'Kit Harington', N'Jon Snow', 1),
        (N'Game of Thrones', N'Peter Dinklage', N'Tyrion Lannister', 2),
        (N'Game of Thrones', N'Emilia Clarke', N'Daenerys Targaryen', 3),
        (N'Toilet-Bound Hanako-kun', N'Megumi Ogata', N'Hanako', 1),
        (N'Toilet-Bound Hanako-kun', N'Akari Kito', N'Nene Yashiro', 2),
        (N'Toilet-Bound Hanako-kun', N'Shoya Chiba', N'Kou Minamoto', 3),
        (N'Takopi''s Original Sin', N'Kurumi Mamiya', N'Takopi', 1),
        (N'Takopi''s Original Sin', N'Reina Ueda', N'Shizuka Kuze', 2),
        (N'Takopi''s Original Sin', N'Konomi Kohara', N'Marina Kirarazaka', 3),
        (N'Takopi''s Original Sin', N'Anna Nagase', N'Naoki Azuma', 4),
        (N'Gravity Falls', N'Jason Ritter', N'Dipper Pines', 1),
        (N'Gravity Falls', N'Kristen Schaal', N'Mabel Pines', 2),
        (N'Gravity Falls', N'Alex Hirsch', N'Grunkle Stan / Soos / Bill Cipher', 3),
        (N'Star vs. the Forces of Evil', N'Eden Sher', N'Star Butterfly', 1),
        (N'Star vs. the Forces of Evil', N'Adam McArthur', N'Marco Diaz', 2),
        (N'Bloodhounds', N'Woo Do-hwan', N'Kim Gun-woo', 1),
        (N'Bloodhounds', N'Lee Sang-yi', N'Hong Woo-jin', 2);

    INSERT INTO dbo.EpisodeActors ([EpisodeId],[ActorId],[CharacterName],[CastOrder])
    SELECT ep.[Id], ac.[Id], c.[CharacterName], c.[CastOrder]
    FROM @EpisodeShowCastSeed c
    JOIN dbo.TVShows t ON t.[Title]=c.[TVTitle]
    JOIN dbo.Seasons s ON s.[TVShowId]=t.[Id]
    JOIN dbo.Episodes ep ON ep.[SeasonId]=s.[Id]
    JOIN @EpisodeSeed seedEp ON seedEp.[TVTitle]=t.[Title] AND seedEp.[SeasonNumber]=s.[SeasonNumber] AND seedEp.[EpisodeNumber]=ep.[EpisodeNumber]
    JOIN dbo.Actors ac ON ac.[FullName]=c.[ActorName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.EpisodeActors x WHERE x.[EpisodeId]=ep.[Id] AND x.[ActorId]=ac.[Id]);

    ---------------------------------------------------------------------------
    -- 16) EPISODE <-> DIRECTOR
    ---------------------------------------------------------------------------
    DECLARE @EpisodeDirectorSeed TABLE ([TVTitle] nvarchar(300), [SeasonNumber] int, [EpisodeNumber] int, [DirectorName] nvarchar(200));
    INSERT INTO @EpisodeDirectorSeed ([TVTitle],[SeasonNumber],[EpisodeNumber],[DirectorName]) VALUES
        (N'Game of Thrones', 1, 1, N'Timothy Van Patten'),
        (N'Game of Thrones', 6, 9, N'Miguel Sapochnik'),
        (N'Takopi''s Original Sin', 1, 1, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 2, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 3, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 4, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 5, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 6, N'Shin''ya Iino'),
        (N'Star vs. the Forces of Evil', 1, 1, N'Michael Mullen'),
        (N'Star vs. the Forces of Evil', 1, 2, N'Michael Mullen'),
        (N'Bloodhounds', 1, 1, N'Kim Joo-hwan'),
        (N'Bloodhounds', 1, 2, N'Kim Joo-hwan'),
        (N'Bloodhounds', 1, 3, N'Kim Joo-hwan'),
        (N'Bloodhounds', 1, 8, N'Kim Joo-hwan');

    INSERT INTO dbo.EpisodeDirectors ([EpisodeId],[DirectorId])
    SELECT ep.[Id], d.[Id]
    FROM @EpisodeDirectorSeed x
    JOIN dbo.TVShows t ON t.[Title]=x.[TVTitle]
    JOIN dbo.Seasons s ON s.[TVShowId]=t.[Id] AND s.[SeasonNumber]=x.[SeasonNumber]
    JOIN dbo.Episodes ep ON ep.[SeasonId]=s.[Id] AND ep.[EpisodeNumber]=x.[EpisodeNumber]
    JOIN dbo.Directors d ON d.[FullName]=x.[DirectorName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.EpisodeDirectors z WHERE z.[EpisodeId]=ep.[Id] AND z.[DirectorId]=d.[Id]);

    ---------------------------------------------------------------------------
    -- 17) EPISODE <-> WRITER
    ---------------------------------------------------------------------------
    DECLARE @EpisodeWriterSeed TABLE ([TVTitle] nvarchar(300), [SeasonNumber] int, [EpisodeNumber] int, [WriterName] nvarchar(200));
    INSERT INTO @EpisodeWriterSeed ([TVTitle],[SeasonNumber],[EpisodeNumber],[WriterName]) VALUES
        (N'Game of Thrones', 1, 1, N'David Benioff'),
        (N'Game of Thrones', 1, 1, N'D.B. Weiss'),
        (N'Game of Thrones', 6, 9, N'David Benioff'),
        (N'Game of Thrones', 6, 9, N'D.B. Weiss'),
        (N'Takopi''s Original Sin', 1, 1, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 1, N'Taizan 5'),
        (N'Takopi''s Original Sin', 1, 2, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 3, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 4, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 5, N'Shin''ya Iino'),
        (N'Takopi''s Original Sin', 1, 6, N'Shin''ya Iino'),
        (N'Gravity Falls', 1, 1, N'Alex Hirsch'),
        (N'Star vs. the Forces of Evil', 1, 1, N'Daron Nefcy'),
        (N'Star vs. the Forces of Evil', 1, 1, N'Dave Wasson'),
        (N'Star vs. the Forces of Evil', 1, 1, N'Jordana Arkin'),
        (N'Star vs. the Forces of Evil', 1, 1, N'Michael Mullen'),
        (N'Bloodhounds', 1, 1, N'Kim Joo-hwan'),
        (N'Bloodhounds', 1, 2, N'Kim Joo-hwan'),
        (N'Bloodhounds', 1, 3, N'Kim Joo-hwan'),
        (N'Bloodhounds', 1, 8, N'Kim Joo-hwan');

    INSERT INTO dbo.EpisodeWriters ([EpisodeId],[WriterId])
    SELECT ep.[Id], w.[Id]
    FROM @EpisodeWriterSeed x
    JOIN dbo.TVShows t ON t.[Title]=x.[TVTitle]
    JOIN dbo.Seasons s ON s.[TVShowId]=t.[Id] AND s.[SeasonNumber]=x.[SeasonNumber]
    JOIN dbo.Episodes ep ON ep.[SeasonId]=s.[Id] AND ep.[EpisodeNumber]=x.[EpisodeNumber]
    JOIN dbo.Writers w ON w.[FullName]=x.[WriterName]
    WHERE NOT EXISTS (SELECT 1 FROM dbo.EpisodeWriters z WHERE z.[EpisodeId]=ep.[Id] AND z.[WriterId]=w.[Id]);

    ---------------------------------------------------------------------------
    -- 18) EXISTING USERS -> PROFILE / REVIEWS / WATCHLIST / WATCH HISTORY
    --     No raw AspNetUsers inserts: create users with your Register endpoint.
    ---------------------------------------------------------------------------
    DECLARE @User1 uniqueidentifier = (SELECT TOP (1) [Id] FROM dbo.AspNetUsers ORDER BY [UserName], [Id]);
    DECLARE @User2 uniqueidentifier = (SELECT TOP (1) [Id] FROM dbo.AspNetUsers WHERE [Id] <> @User1 ORDER BY [UserName], [Id]);

    IF @User1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.UserProfiles WHERE [AppUserId]=@User1)
    BEGIN
        INSERT INTO dbo.UserProfiles ([AppUserId],[DisplayName],[Bio],[ProfileImageUrl])
        SELECT @User1, LEFT(COALESCE(NULLIF([UserName],N''),N'MovieVerse User'),50), N'Demo profile created by MovieVerse_DemoSeed.sql.', NULL
        FROM dbo.AspNetUsers WHERE [Id]=@User1;
    END;

    IF @User2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.UserProfiles WHERE [AppUserId]=@User2)
    BEGIN
        INSERT INTO dbo.UserProfiles ([AppUserId],[DisplayName],[Bio],[ProfileImageUrl])
        SELECT @User2, LEFT(COALESCE(NULLIF([UserName],N''),N'MovieVerse User'),50), N'Demo profile created by MovieVerse_DemoSeed.sql.', NULL
        FROM dbo.AspNetUsers WHERE [Id]=@User2;
    END;

    DECLARE @MovieReview1 TABLE ([Title] nvarchar(300), [Rating] decimal(3,1), [Content] nvarchar(4000));
    INSERT INTO @MovieReview1 ([Title],[Rating],[Content]) VALUES
        (N'Iron Man', 8.8, N'A strong superhero origin story with a great lead performance.'),
        (N'Iron Man 2', 7.8, N'Messier than the first film, but still very entertaining.'),
        (N'The Prestige', 9.4, N'A tense rivalry story that rewards paying attention.'),
        (N'Batman Begins', 9.0, N'A grounded and atmospheric Batman origin.'),
        (N'The Dark Knight', 9.7, N'A gripping crime drama with unforgettable conflict.'),
        (N'The Dark Knight Rises', 8.8, N'A huge and emotional conclusion to the trilogy.'),
        (N'Interstellar', 9.6, N'Ambitious science fiction with strong emotional stakes.');

    DECLARE @MovieReview2 TABLE ([Title] nvarchar(300), [Rating] decimal(3,1), [Content] nvarchar(4000));
    INSERT INTO @MovieReview2 ([Title],[Rating],[Content]) VALUES
        (N'Iron Man', 8.4, NULL),
        (N'Iron Man 2', 7.4, NULL),
        (N'The Prestige', 9.1, NULL),
        (N'Batman Begins', 8.7, NULL),
        (N'The Dark Knight', 9.5, NULL),
        (N'The Dark Knight Rises', 8.5, NULL),
        (N'Interstellar', 9.3, NULL);

    IF @User1 IS NOT NULL
    BEGIN
        INSERT INTO dbo.MovieReviews ([Rating],[Content],[MovieId],[UserId])
        SELECT r.[Rating], r.[Content], m.[Id], @User1
        FROM @MovieReview1 r JOIN dbo.Movies m ON m.[Title]=r.[Title]
        WHERE NOT EXISTS (SELECT 1 FROM dbo.MovieReviews x WHERE x.[MovieId]=m.[Id] AND x.[UserId]=@User1);
    END;

    IF @User2 IS NOT NULL
    BEGIN
        INSERT INTO dbo.MovieReviews ([Rating],[Content],[MovieId],[UserId])
        SELECT r.[Rating], r.[Content], m.[Id], @User2
        FROM @MovieReview2 r JOIN dbo.Movies m ON m.[Title]=r.[Title]
        WHERE NOT EXISTS (SELECT 1 FROM dbo.MovieReviews x WHERE x.[MovieId]=m.[Id] AND x.[UserId]=@User2);
    END;

    DECLARE @TVReview1 TABLE ([Title] nvarchar(300), [Rating] decimal(3,1), [Content] nvarchar(4000));
    INSERT INTO @TVReview1 ([Title],[Rating],[Content]) VALUES
        (N'Game of Thrones', 9.3, N'Huge-scale fantasy television with memorable characters and political conflict.'),
        (N'Toilet-Bound Hanako-kun', 8.6, N'Stylish supernatural comedy with a strong central trio.'),
        (N'Takopi''s Original Sin', 9.2, N'Short, painful, and emotionally intense.'),
        (N'Gravity Falls', 9.7, N'Funny, clever, and packed with mysteries that actually pay off.'),
        (N'Star vs. the Forces of Evil', 8.3, N'Chaotic magical fun with a lot of personality.'),
        (N'Bloodhounds', 9.0, N'Fast action and a friendship that carries the series.');

    DECLARE @TVReview2 TABLE ([Title] nvarchar(300), [Rating] decimal(3,1), [Content] nvarchar(4000));
    INSERT INTO @TVReview2 ([Title],[Rating],[Content]) VALUES
        (N'Game of Thrones', 9.0, NULL),
        (N'Toilet-Bound Hanako-kun', 8.2, NULL),
        (N'Takopi''s Original Sin', 8.9, NULL),
        (N'Gravity Falls', 9.5, NULL),
        (N'Star vs. the Forces of Evil', 8.0, NULL),
        (N'Bloodhounds', 8.7, NULL);

    IF @User1 IS NOT NULL
    BEGIN
        INSERT INTO dbo.TVShowReviews ([Rating],[Content],[TVShowId],[UserId])
        SELECT r.[Rating], r.[Content], t.[Id], @User1
        FROM @TVReview1 r JOIN dbo.TVShows t ON t.[Title]=r.[Title]
        WHERE NOT EXISTS (SELECT 1 FROM dbo.TVShowReviews x WHERE x.[TVShowId]=t.[Id] AND x.[UserId]=@User1);
    END;

    IF @User2 IS NOT NULL
    BEGIN
        INSERT INTO dbo.TVShowReviews ([Rating],[Content],[TVShowId],[UserId])
        SELECT r.[Rating], r.[Content], t.[Id], @User2
        FROM @TVReview2 r JOIN dbo.TVShows t ON t.[Title]=r.[Title]
        WHERE NOT EXISTS (SELECT 1 FROM dbo.TVShowReviews x WHERE x.[TVShowId]=t.[Id] AND x.[UserId]=@User2);
    END;

    IF @User1 IS NOT NULL
    BEGIN
        INSERT INTO dbo.EpisodeReviews ([Rating],[Content],[EpisodeId],[UserId])
        SELECT CAST(9.5 AS decimal(3,1)), N'Excellent episode for demo review data.', ep.[Id], @User1
        FROM dbo.Episodes ep
        JOIN dbo.Seasons s ON s.[Id]=ep.[SeasonId]
        JOIN dbo.TVShows t ON t.[Id]=s.[TVShowId]
        WHERE t.[Title]=N'Game of Thrones' AND s.[SeasonNumber]=6 AND ep.[EpisodeNumber]=9
          AND NOT EXISTS (SELECT 1 FROM dbo.EpisodeReviews r WHERE r.[EpisodeId]=ep.[Id] AND r.[UserId]=@User1);

        INSERT INTO dbo.EpisodeReviews ([Rating],[Content],[EpisodeId],[UserId])
        SELECT CAST(9.6 AS decimal(3,1)), N'A great mystery-comedy opener.', ep.[Id], @User1
        FROM dbo.Episodes ep
        JOIN dbo.Seasons s ON s.[Id]=ep.[SeasonId]
        JOIN dbo.TVShows t ON t.[Id]=s.[TVShowId]
        WHERE t.[Title]=N'Gravity Falls' AND s.[SeasonNumber]=1 AND ep.[EpisodeNumber]=1
          AND NOT EXISTS (SELECT 1 FROM dbo.EpisodeReviews r WHERE r.[EpisodeId]=ep.[Id] AND r.[UserId]=@User1);
    END;

    IF @User1 IS NOT NULL
    BEGIN
        -- Watchlist: movies
        INSERT INTO dbo.WatchlistItems ([UserId],[MovieId],[TVShowId])
        SELECT @User1, m.[Id], NULL FROM dbo.Movies m
        WHERE m.[Title] IN (N'The Prestige',N'Interstellar')
          AND NOT EXISTS (SELECT 1 FROM dbo.WatchlistItems w WHERE w.[UserId]=@User1 AND w.[MovieId]=m.[Id]);

        -- Watchlist: TV shows
        INSERT INTO dbo.WatchlistItems ([UserId],[MovieId],[TVShowId])
        SELECT @User1, NULL, t.[Id] FROM dbo.TVShows t
        WHERE t.[Title] IN (N'Takopi''s Original Sin',N'Bloodhounds')
          AND NOT EXISTS (SELECT 1 FROM dbo.WatchlistItems w WHERE w.[UserId]=@User1 AND w.[TVShowId]=t.[Id]);

        -- History: movies
        INSERT INTO dbo.WatchHistoryItems ([UserId],[MovieId],[TVShowId])
        SELECT @User1, m.[Id], NULL FROM dbo.Movies m
        WHERE m.[Title] IN (N'Iron Man',N'The Dark Knight')
          AND NOT EXISTS (SELECT 1 FROM dbo.WatchHistoryItems h WHERE h.[UserId]=@User1 AND h.[MovieId]=m.[Id]);

        -- History: TV shows
        INSERT INTO dbo.WatchHistoryItems ([UserId],[MovieId],[TVShowId])
        SELECT @User1, NULL, t.[Id] FROM dbo.TVShows t
        WHERE t.[Title] IN (N'Game of Thrones',N'Gravity Falls')
          AND NOT EXISTS (SELECT 1 FROM dbo.WatchHistoryItems h WHERE h.[UserId]=@User1 AND h.[TVShowId]=t.[Id]);
    END;

    COMMIT TRANSACTION;

    PRINT N'MovieVerse demo seed completed successfully.';
    PRINT N'If no reviews/watchlist/history were created, register at least one user and run this script again.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

-- Quick sanity checks
SELECT N'Genres' AS [TableName], COUNT(*) AS [RowCount] FROM dbo.Genres
UNION ALL SELECT N'Actors', COUNT(*) FROM dbo.Actors
UNION ALL SELECT N'Directors', COUNT(*) FROM dbo.Directors
UNION ALL SELECT N'Writers', COUNT(*) FROM dbo.Writers
UNION ALL SELECT N'Movies', COUNT(*) FROM dbo.Movies
UNION ALL SELECT N'TVShows', COUNT(*) FROM dbo.TVShows
UNION ALL SELECT N'Seasons', COUNT(*) FROM dbo.Seasons
UNION ALL SELECT N'Episodes', COUNT(*) FROM dbo.Episodes
UNION ALL SELECT N'MovieReviews', COUNT(*) FROM dbo.MovieReviews
UNION ALL SELECT N'TVShowReviews', COUNT(*) FROM dbo.TVShowReviews
UNION ALL SELECT N'EpisodeReviews', COUNT(*) FROM dbo.EpisodeReviews
UNION ALL SELECT N'WatchlistItems', COUNT(*) FROM dbo.WatchlistItems
UNION ALL SELECT N'WatchHistoryItems', COUNT(*) FROM dbo.WatchHistoryItems;
