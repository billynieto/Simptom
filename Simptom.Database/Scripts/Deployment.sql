--This is my database deplyment/update script.  This is NOT IDEAL, but due to the version
--of SQL Server I'm using (SQL Express), DACPAC files are not supported.  Therefore I must
--use the outdated method of doing this manually.  This has complicated this project and
--added time for actually developing something that works.

DROP TABLE [dbo].[FlareUps]
DROP TABLE [dbo].[Participations]

GO

DROP TABLE [dbo].[Activities]
DROP TABLE [dbo].[ActivityCategories]
DROP TABLE [dbo].[SymptomCategories]
DROP TABLE [dbo].[Symptoms]
DROP TABLE [dbo].[Users]

GO

CREATE TABLE [dbo].[Activities]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(), 
    [Name] NVARCHAR(50) NOT NULL, 
    [CategoryID] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [AK_Activities_Name] UNIQUE ([Name])
)

CREATE TABLE [dbo].[ActivityCategories]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(),
    [Name] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [AK_ActivityCategories_Name] UNIQUE ([Name])
)

CREATE TABLE [dbo].[FlareUps]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(), 
    [ExperiencedOn] DATETIME2 NOT NULL, 
    [UserID] UNIQUEIDENTIFIER NOT NULL, 
    [SymptomID] UNIQUEIDENTIFIER NOT NULL, 
    [Severity] FLOAT NOT NULL
)

CREATE TABLE [dbo].[Participations]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(),
    [ActivityID] UNIQUEIDENTIFIER NOT NULL, 
    [PerformedOn] DATETIME2 NOT NULL, 
    [Severity] FLOAT NOT NULL,
    [UserID] UNIQUEIDENTIFIER NOT NULL
)

CREATE TABLE [dbo].[SymptomCategories]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(),
    [Name] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [AK_SymptomCategories_Name] UNIQUE ([Name])
)

CREATE TABLE [dbo].[Symptoms]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(),
    [Name] NVARCHAR(50) NOT NULL, 
    [CategoryID] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [AK_Symptoms_Name] UNIQUE ([Name])
)

CREATE TABLE [dbo].[Users]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(), 
    [Name] NVARCHAR(50) NOT NULL, 
    [Password] NVARCHAR(50) NOT NULL, 
)

GO

INSERT INTO ActivityCategories ([Name]) VALUES
	('Excercise'),
	('Ingestion'),
	('Sedentary')

INSERT INTO SymptomCategories ([Name]) VALUES
	('Impairement'),
	('Pain'),
	('Nuerological')

INSERT INTO Users ([Name], [Password]) VALUES
	('User', 'Password')

GO

INSERT INTO Activities ([Name], CategoryID) VALUES
	('Test', (SELECT TOP 1 ID FROM ActivityCategories))

INSERT INTO Symptoms ([Name], CategoryID) VALUES
	('Test', (SELECT TOP 1 ID FROM SymptomCategories))

GO

DECLARE @Today AS DATETIME
SET @Today = GETDATE()
SET @Today = CAST(FORMAT(@Today, 'yyyy-MM-dd') + 'T' + FORMAT(@Today, 'HH:mm') + ':00.000' AS DATETIME)

INSERT INTO FlareUps (ExperiencedOn, UserID, SymptomID, Severity) VALUES
	(@Today, (SELECT TOP 1 ID FROM Users), (SELECT TOP 1 ID FROM Symptoms), 0.45)

INSERT INTO Participations (ActivityID, PerformedOn, Severity, UserID) VALUES
	((SELECT TOP 1 ID FROM Activities), @Today, 0.75, (SELECT TOP 1 ID FROM Users))