CREATE TABLE [dbo].[ActivityCategories]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(),
    [Name] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [AK_ActivityCategories_Name] UNIQUE ([Name])
)
