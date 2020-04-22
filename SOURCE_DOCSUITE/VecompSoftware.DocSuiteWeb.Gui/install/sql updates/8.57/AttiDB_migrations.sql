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
PRINT 'Versionamento database alla 8.57'
GO

EXEC dbo.VersioningDatabase N'8.57'
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ResolutionContact]';
GO

ALTER TABLE [dbo].[ResolutionContact] ADD [UniqueId] [uniqueidentifier] null
GO

UPDATE [dbo].[ResolutionContact] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[ResolutionContact] ADD CONSTRAINT [DF_ResolutionContact_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

ALTER TABLE [dbo].[ResolutionContact] ALTER COLUMN [UniqueId] [uniqueidentifier] not null
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

PRINT 'Aggiunta colonne [LastChangedDate], [LastChangedUser], [Timestamp] nella tabella [ResolutionContact]';
GO

ALTER TABLE [dbo].[ResolutionContact] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ResolutionContact] ADD [LastChangedUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[ResolutionContact] ADD [Timestamp] TIMESTAMP not null
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
