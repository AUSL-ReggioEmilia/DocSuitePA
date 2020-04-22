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
PRINT 'Migrazione tabella [TableLog]'
GO

CREATE TABLE [dbo].[tmp_ms_xx_TableLog] (
    [IdTableLog]      UNIQUEIDENTIFIER   NOT NULL,
    [EntityId]        INT                NULL,
    [EntityUniqueId]  UNIQUEIDENTIFIER   NULL,
    [TableName]       NVARCHAR (256)     NOT NULL,
    [LogDate]         DATETIMEOFFSET (7) NOT NULL,
    [SystemComputer]  NVARCHAR (256)     NOT NULL,
    [SystemUser]      NVARCHAR (256)     NOT NULL,
    [LogType]         SMALLINT           NOT NULL,
    [LogDescription]  NVARCHAR (4000)     NOT NULL,
   [LastChangedUser] NVARCHAR (256)     NULL,
    [LastChangedDate] DATETIMEOFFSET (7) NULL,
    [Timestamp]       ROWVERSION         NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_TableLog1] PRIMARY KEY NONCLUSTERED ([IdTableLog] ASC)
);

CREATE CLUSTERED INDEX [tmp_ms_xx_index_IX_TableLog_LogDate1]
    ON [dbo].[tmp_ms_xx_TableLog]([LogDate] ASC);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[TableLog])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_TableLog] ([IdTableLog], [EntityId], [LogDate], [SystemComputer], [SystemUser], [TableName], [LogType], [LogDescription])
        SELECT   NEWID(),
                           [IdRef],
                           [LogDate],
                 [SystemComputer],
                 [SystemUser],
                 [TableName],
                 CASE [LogType]
                                  WHEN 'IN' THEN 2
                                  WHEN 'DL' Then 0
								  When 'UP' THEN 1
                           END,
                 [LogDescription]
        FROM     [dbo].[TableLog]
        ORDER BY [LogDate] ASC;
    END

DROP TABLE [dbo].[TableLog];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_TableLog]', N'TableLog';

EXECUTE sp_rename N'[dbo].[TableLog].[tmp_ms_xx_index_IX_TableLog_LogDate1]', N'IX_TableLog_LogDate', N'INDEX';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_TableLog1]', N'PK_TableLog', N'OBJECT';

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
PRINT 'Drop and Create view [ResolutionContact]'
GO

DROP VIEW [dbo].[ResolutionContactEmpty]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ResolutionContactEmpty] AS SELECT TOP 0 
	CAST( NULL AS  [int] ) [idResolution],
	CAST( NULL AS  [int] ) [idContact],
	CAST( NULL AS  [char](1) ) [ComunicationType],
	CAST( NULL AS  [smallint] ) [Incremental],
	CAST( NULL AS  [varchar](20) ) [RegistrationUser],
	CAST( NULL AS  [datetime] ) [RegistrationDate],
	CAST( NULL AS  [uniqueidentifier]) [UniqueId],
	CAST( NULL AS  [timestamp]) [Timestamp],
	CAST( NULL AS  [varchar](30) ) [LastChangedUser],
	CAST( NULL AS  [datetimeoffset](7) ) [LastChangedDate]
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
