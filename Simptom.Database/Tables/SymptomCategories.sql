CREATE TABLE [dbo].[SymptomCategories]
(
	[ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(),
    [Name] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [AK_SymptomCategories_Name] UNIQUE ([Name])
)
