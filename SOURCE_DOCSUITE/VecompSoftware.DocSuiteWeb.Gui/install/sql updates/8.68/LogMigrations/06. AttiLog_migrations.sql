--#############################################################################
PRINT N'Modificata colonna [LogDescription] in [dbo].[ResolutionLog]';
GO

ALTER TABLE [dbo].[ResolutionLog]  ALTER COLUMN [LogDescription] NVARCHAR (Max)  NOT NULL
GO
--#############################################################################