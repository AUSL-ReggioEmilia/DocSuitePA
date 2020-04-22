--#############################################################################
PRINT N'Modificata colonna [LogDescription] in [dbo].[ProtocolLog]';
GO

ALTER TABLE [dbo].[ProtocolLog]  ALTER COLUMN [LogDescription] NVARCHAR (Max) NOT NULL
GO

--#############################################################################
PRINT N'Modificata colonna [LogDescription] in [dbo].[DocumentSeriesItemLog]';
GO

ALTER TABLE [dbo].[DocumentSeriesItemLog]  ALTER COLUMN [LogDescription] NVARCHAR (Max) NOT NULL
GO
--#############################################################################