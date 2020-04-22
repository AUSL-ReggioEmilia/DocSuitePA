SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;
GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
--#############################################################################
PRINT 'Versionamento database alla 8.68'
GO

EXEC dbo.VersioningDatabase N'8.68'
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Colonna [UniqueIdResolution] NOT NULL nella tabella [dbo].[FileResolution] e aggiunta relazione foreign key'
GO

UPDATE FR SET FR.[UniqueIdResolution] = R.uniqueid
FROM dbo.FileResolution AS FR
INNER JOIN dbo.Resolution R ON R.idResolution = FR.idResolution
WHERE fr.[UniqueIdResolution] IS NULL
Go

ALTER TABLE [dbo].[FileResolution] ALTER COLUMN [UniqueIdResolution] [uniqueidentifier] NOT NULL
GO

ALTER TABLE [dbo].[FileResolution] WITH CHECK ADD CONSTRAINT [FK_FileResolution_Resolution] FOREIGN KEY (UniqueIdResolution)
REFERENCES [dbo].[Resolution] ([UniqueId])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Colonna [UniqueIdResolution] NOT NULL nella tabella [dbo].[ResolutionLog] e aggiunta relazione foreign key'
GO

UPDATE RL SET RL.[UniqueIdResolution] = R.uniqueid
FROM [dbo].[ResolutionLog] AS RL
INNER JOIN dbo.Resolution R ON R.idResolution = RL.idResolution
WHERE RL.[UniqueIdResolution] IS NULL
Go

ALTER TABLE [dbo].[ResolutionLog] ALTER COLUMN [UniqueIdResolution] [uniqueidentifier] NOT NULL
GO

ALTER TABLE [dbo].[ResolutionLog] WITH CHECK ADD CONSTRAINT [FK_ResolutionLog_Resolution] FOREIGN KEY (UniqueIdResolution)
REFERENCES [dbo].[Resolution] ([UniqueId])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
	PRINT N'The transacted portion of the database update succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT N'The transacted portion of the database update FAILED.'
GO
DROP TABLE #tmpErrors
GO