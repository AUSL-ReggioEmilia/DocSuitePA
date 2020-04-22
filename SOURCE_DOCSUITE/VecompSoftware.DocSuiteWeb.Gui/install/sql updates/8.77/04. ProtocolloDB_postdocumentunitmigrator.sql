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
PRINT N'ALTER TABLE [dbo].[Protocol] DROP COLUMNS IdUDS AND IdUDSRepository';

ALTER TABLE [dbo].[Protocol] DROP COLUMN [IdUDS]
GO

declare @Drop_udsrepository_constraint_command nvarchar(1000)

SELECT TOP 1 @Drop_udsrepository_constraint_command = 'ALTER TABLE Protocol DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'Protocol' AND c.Name = N'IdUDSRepository'

PRINT @Drop_udsrepository_constraint_command
EXECUTE (@Drop_udsrepository_constraint_command)
GO

ALTER TABLE [dbo].[Protocol] DROP COLUMN [IdUDSRepository]
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
PRINT N'ALTER TABLE [dbo].[Collaboration] DROP COLUMNS IdUDS AND IdUDSRepository';

ALTER TABLE [dbo].[Collaboration] DROP COLUMN [IdUDS]
GO

ALTER TABLE [dbo].[Collaboration] DROP COLUMN [IdUDSRepository]
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