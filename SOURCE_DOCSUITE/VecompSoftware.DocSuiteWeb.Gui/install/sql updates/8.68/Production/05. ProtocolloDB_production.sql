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
PRINT N'Modificata colonna [LogDescription] in [dbo].[ProtocolLog]';
GO

ALTER TABLE [dbo].[ProtocolLog]  ALTER COLUMN [LogDescription] NVARCHAR (Max)  NOT NULL
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
PRINT N'Aggiunta della colonna [UniqueIdDocumentSeriesItem] alla tabella [dbo].[DocumentSeriesItemLog]' 
GO


UPDATE DSIL SET DSIL.[UniqueIdDocumentSeriesItem] = DSI.UniqueId
FROM [dbo].[DocumentSeriesItemLog] AS DSIL 
INNER JOIN [dbo].[DocumentSeriesItem] DSI ON DSI.Id = DSIL.IdDocumentSeriesItem
WHERE DSIL.[UniqueIdDocumentSeriesItem] IS NULL
GO

ALTER TABLE [dbo].[DocumentSeriesItemLog] ALTER COLUMN [UniqueIdDocumentSeriesItem] [uniqueidentifier] NOT NULL
GO

ALTER TABLE [dbo].[DocumentSeriesItemLog] WITH CHECK ADD CONSTRAINT [FK_DocumentSeriesItemLog_DocumentSeriesItem] FOREIGN KEY (UniqueIdDocumentSeriesItem)
REFERENCES [dbo].[DocumentSeriesItem] ([UniqueId])
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