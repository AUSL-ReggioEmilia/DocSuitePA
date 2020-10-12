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
PRINT 'Versionamento database alla 9.03'
GO

EXEC dbo.VersioningDatabase N'9.03',N'DSW Version','MigrationDate'
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
PRINT 'ALTER TABLE [dbo].[Location] DROP COLUMN [DocumentServer]'
GO
IF EXISTS (SELECT 1
               FROM   INFORMATION_SCHEMA.COLUMNS
               WHERE  TABLE_NAME = 'Location'
                      AND COLUMN_NAME = 'DocumentServer'
                      AND TABLE_SCHEMA='DBO')
  BEGIN
      ALTER TABLE [dbo].[Location] DROP COLUMN [DocumentServer]
  END
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
PRINT 'ALTER TABLE [dbo].[Location] DROP COLUMN [ConservationServer]'
GO
IF EXISTS (SELECT 1
               FROM   INFORMATION_SCHEMA.COLUMNS
               WHERE  TABLE_NAME = 'Location'
                      AND COLUMN_NAME = 'ConservationServer'
                      AND TABLE_SCHEMA='DBO')
  BEGIN
      ALTER TABLE [dbo].[Location] DROP COLUMN [ConservationServer]
  END
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