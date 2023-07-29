SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE DATABASE Library;
GO

USE Library;
GO

CREATE LOGIN lib_user WITH PASSWORD = 'passw0rd!';
GO

CREATE USER lib_user FOR LOGIN lib_user;
GO

EXEC sp_addrolemember 'db_datareader', 'lib_user';
EXEC sp_addrolemember 'db_datawriter', 'lib_user';
GO

CREATE TABLE [dbo].[User](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](250) NOT NULL,
	[LastName] [nvarchar](250) NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Author](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[MiddleName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Author] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Book](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[AuthorID] [bigint] NOT NULL,
 CONSTRAINT [PK_Book] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Book]  WITH CHECK ADD  CONSTRAINT [FK_Book_Author] FOREIGN KEY([AuthorID])
REFERENCES [dbo].[Author] ([ID])
GO

ALTER TABLE [dbo].[Book] CHECK CONSTRAINT [FK_Book_Author]
GO

CREATE TABLE [dbo].[BooksTaken](
	[BookID] [bigint] NOT NULL,
	[UserID] [bigint] NOT NULL,
	[DateTaken] [datetime] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BooksTaken]  WITH CHECK ADD  CONSTRAINT [FK_BooksTaken_Book] FOREIGN KEY([BookID])
REFERENCES [dbo].[Book] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BooksTaken] CHECK CONSTRAINT [FK_BooksTaken_Book]
GO

ALTER TABLE [dbo].[BooksTaken]  WITH CHECK ADD  CONSTRAINT [FK_BooksTaken_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BooksTaken] CHECK CONSTRAINT [FK_BooksTaken_User]
GO

/*
----------------------------------------
Usage:
EXEC dbo.p_PopulateTestData
----------------------------------------
*/
CREATE PROCEDURE dbo.p_PopulateTestData
	
AS
BEGIN
	
	SET NOCOUNT ON;

	/*** Users **************************/
	DECLARE @tblUsers AS TABLE (
		[ID] [bigint] NOT NULL,
		[FirstName] [nvarchar](250) NOT NULL,
		[LastName] [nvarchar](250) NOT NULL,
		[Email] [nvarchar](250) NOT NULL
	)

	INSERT INTO @tblUsers
	SELECT 1, 'James', 'Hetfield', 'coolguy@metal.com' UNION
	SELECT 2, 'Lars', 'Ulrich', 'drummer001@gmail.com' UNION
	SELECT 3, 'Angus', 'Young', 'dynamite@tnt.com.au' UNION
	SELECT 4, 'Brian', 'Johnson', 'brian.j.73@yahoo.com' UNION
	SELECT 5, 'John Michael', 'Osbourne', 'perry.mason@musichits.com' 

	SET IDENTITY_INSERT dbo.[User] ON

	MERGE dbo.[User] t 
	USING @tblUsers  s
	ON t.ID = s.ID
	WHEN MATCHED 
		THEN UPDATE SET
			t.FirstName = s.FirstName,
			t.LastName = s.LastName,
			t.Email = s.Email
	WHEN NOT MATCHED BY TARGET
		THEN INSERT (ID, FirstName, LastName, Email)
		VALUES (s.ID, s.FirstName, s.LastName, s.Email)

    WHEN NOT MATCHED BY SOURCE 
		THEN DELETE;

	SET IDENTITY_INSERT dbo.[User] OFF

	/*** Authors **************************/
	DECLARE @tblAuthors AS TABLE (
		[ID] [bigint] NOT NULL,
		[FirstName] [nvarchar](50) NOT NULL,
		[MiddleName] [nvarchar](50) NULL,
		[LastName] [nvarchar](50) NOT NULL
	)

	INSERT INTO @tblAuthors
	SELECT 1, 'Edgar', 'Allan', 'Poe' UNION
	SELECT 2, 'Ernest', 'Miller', 'Hemingway' UNION
	SELECT 3, 'Kenzaburo', NULL, 'Oe' UNION
	SELECT 4, 'Mark', NULL, 'Twain' UNION
	SELECT 5, 'Jorge', 'Leal', 'Amado' UNION
	SELECT 6, 'Octavio', NULL, 'Paz' UNION
	SELECT 7, 'Stanislaw', NULL, 'Lem' UNION
	SELECT 8, 'Adam', NULL, 'Mickiewicz' UNION
	SELECT 9, 'Henryk', NULL, 'Sienkiewicz' UNION
	SELECT 10, 'Friedrich', NULL, 'Nietzsche' 

	SET IDENTITY_INSERT dbo.[Author] ON

	MERGE dbo.[Author] t 
	USING @tblAuthors  s
	ON t.ID = s.ID
	WHEN MATCHED 
		THEN UPDATE SET
			t.FirstName = s.FirstName,
			t.LastName = s.LastName,
			t.MiddleName = s.MiddleName
	WHEN NOT MATCHED BY TARGET
		THEN INSERT (ID, FirstName, LastName, MiddleName)
		VALUES (s.ID, s.FirstName, s.LastName, s.MiddleName)

    WHEN NOT MATCHED BY SOURCE 
		THEN DELETE;

	SET IDENTITY_INSERT dbo.[Author] OFF

	/*** Books **************************/
	DECLARE @tblBooks AS TABLE (
		[ID] [bigint] NOT NULL,
		[Title] [nvarchar](250) NOT NULL,
		[Description] [nvarchar](1000),
		[AuthorID] [bigint] NOT NULL	)

	INSERT INTO @tblBooks
	SELECT 1, 'The Raven', 'A narrative poem by American writer Edgar Allan Poe. First published in January 1845, the poem is often noted for its musicality, stylized language, and supernatural atmosphere. It tells of a distraught lover who is paid a mysterious visit by a talking raven.', 1 UNION
	SELECT 2, 'The Tell-Tale Heart', 'A short story by American writer Edgar Allan Poe, first published in 1843. It is related by an unnamed narrator who endeavors to convince the reader of the narrator"s sanity while simultaneously describing a murder the narrator committed. The victim was an old man with a filmy pale blue "vulture-eye", as the narrator calls it.', 1 UNION
	
	SELECT 3, 'A Farewell to Arms', 'A Farewell to Arms is a novel by American writer Ernest Hemingway, set during the Italian campaign of World War I. First published in 1929, it is a first-person account of an American, Frederic Henry, serving as a lieutenant (Italian: tenente) in the ambulance corps of the Italian Army.', 2 UNION
	SELECT 4, 'For Whom the Bell Tolls', 'For Whom the Bell Tolls is a novel by Ernest Hemingway published in 1940. It tells the story of Robert Jordan, a young American volunteer attached to a Republican guerrilla unit during the Spanish Civil War.', 2 UNION
	SELECT 5, 'Across the River and Into the Trees', 'a novel by American writer Ernest Hemingway, published by Charles Scribner"s Sons in 1950, after first being serialized in Cosmopolitan magazine earlier that year. The title is derived from the last words of U.S. Civil War Confederate General Thomas J. "Stonewall" Jackson: “Let us cross over the river and rest under the shade of the trees.”', 2 UNION

	SELECT 6, 'The Day He Himself Shall Wipe My Tears Away', ' a novella by the Japanese author Kenzaburō Ōe, first published in Japanese in 1972. It has been translated into English by John Nathan and was published in 1977 together with Teach Us to Outgrow Our Madness, Prize Stock and Aghwee the Sky Monster.', 3 UNION
	SELECT 7, 'Football in the Year 1860', NULL, 3 UNION

	SELECT 8, 'The Adventures of Tom Sawyer', 'The Adventures of Tom Sawyer is an 1876 novel by Mark Twain about a boy growing up along the Mississippi River.', 4 UNION

	SELECT 9, 'Dona Flor and Her Two Husbands', ' a fantasy novel by Brazilian writer Jorge Amado, published in 1966; it was translated into English by Harriet de Onís in 1969.', 5 UNION
	SELECT 10, 'Showdown', 'The novel deals with the foundation of a community, Tocaia Grande ("big ambush" in Portuguese), in a fertile agricultural zone in the state of Bahia', 5 UNION
	SELECT 11, 'Red Field', 'Jorge Amado published Red Field in 1946. In 1945, Brazil had entered a period of “redemocratization” and Amado was elected federal deputy for São Paulo as a candidate of the Brazilian Communist Party.', 5 UNION

	SELECT 12, 'The Double Flame', NULL, 6 UNION

	SELECT 13, 'Solaris', 'Solaris is a 1961 science fiction novel by Polish writer Stanisław Lem. It follows a crew of scientists on a research station as they attempt to understand an extraterrestrial intelligence, which takes the form of a vast ocean on the titular alien planet. The novel is one of Lem"s best-known works.', 7 UNION
	SELECT 14, 'Fables for Robots', 'Fables for Robots is a series of humorous science fiction short stories by Polish writer Stanisław Lem, first printed in 1964.', 7 UNION
	SELECT 15, 'The Futurological Congress', 'a 1971 black humour science fiction novel by Polish author Stanisław Lem. It details the exploits of the hero of a number of his stories, Ijon Tichy, as he visits the Eighth World Futurological Congress at a Hilton Hotel in Costa Rica.', 7 UNION

	SELECT 16, 'Konrad Wallenrod', 'Konrad Wallenrod is an 1828 narrative poem, in Polish, by Adam Mickiewicz, set in the 14th-century Grand Duchy of Lithuania. ', 8 UNION

	SELECT 17, 'The Knights of the Cross', 'The Knights of the Cross or The Teutonic Knights is a 1900 historical novel written by the Polish Positivist writer and the 1905 Nobel laureate, Henryk Sienkiewicz.', 9 UNION
	SELECT 18, 'In Desert and Wilderness, The Story of a Lighthouse Keeper', NULL, 9 UNION
	SELECT 19, 'With Fire and Sword', 'a historical novel by the Polish author Henryk Sienkiewicz, published in 1884. It is the first volume of a series known to Poles as The Trilogy, followed by The Deluge and Fire in the Steppe.', 9 UNION

	SELECT 20, 'Beyond Good and Evil', 'Beyond Good and Evil: Prelude to a Philosophy of the Future is a book by philosopher Friedrich Nietzsche that covers ideas in his previous work Thus Spoke Zarathustra but with a more polemical approach.', 10 UNION
	SELECT 21, 'The Birth of Tragedy Out of the Spirit of Music', 'The Birth of Tragedy Out of the Spirit of Music is an 1872 work of dramatic theory by the German philosopher Friedrich Nietzsche. It was reissued in 1886 as The Birth of Tragedy, Or: Hellenism and Pessimism.', 10
	

	SET IDENTITY_INSERT dbo.[Book] ON

	MERGE dbo.[Book] t 
	USING @tblBooks  s
	ON t.ID = s.ID
	WHEN MATCHED 
		THEN UPDATE SET
			t.Title = s.Title,
			t.Description = s.Description,
			t.AuthorID = s.AuthorID
	WHEN NOT MATCHED BY TARGET
		THEN INSERT (ID, Title, Description, AuthorID)
		VALUES (s.ID, s.Title, s.Description, s.AuthorID)

    WHEN NOT MATCHED BY SOURCE 
		THEN DELETE;

	SET IDENTITY_INSERT dbo.[Book] OFF

	/*** BooksTaken **************************/
	DECLARE @tblBooksTaken AS TABLE (
		[UserID] [bigint] NOT NULL,
		[BookID] [bigint] NOT NULL,
		[DateTaken] [datetime] NOT NULL
	)

	INSERT INTO @tblBooksTaken
	SELECT 1, 2, '2023-05-01' UNION
	SELECT 1, 3, '2022-11-11' UNION
	SELECT 1, 3, '2023-01-12' UNION
	SELECT 2, 12, '2023-05-11' UNION
	SELECT 3, 17, '2023-06-11' UNION 
	SELECT 3, 9, '2023-06-11' UNION
	SELECT 5, 6, '2022-12-29'


	MERGE dbo.[BooksTaken] t 
	USING @tblBooksTaken  s
	ON t.UserID = s.UserID and t.BookID = s.BookID
	WHEN MATCHED 
		THEN UPDATE SET
			t.[DateTaken] = s.[DateTaken]
	WHEN NOT MATCHED BY TARGET
		THEN INSERT (UserID, BookID, [DateTaken])
		VALUES (s.UserID, s.BookID, s.[DateTaken])

    WHEN NOT MATCHED BY SOURCE 
		THEN DELETE;


END
GO

EXEC dbo.p_PopulateTestData