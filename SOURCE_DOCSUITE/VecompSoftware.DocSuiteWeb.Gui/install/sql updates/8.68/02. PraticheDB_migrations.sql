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
PRINT N'Aggiunta della colonna Timestamp alla tabella Contact';
GO

ALTER TABLE [dbo].[Contact] ADD [Timestamp] TIMESTAMP NOT NULL
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
PRINT N'Modificata la colonna [Account] alla tabella [dbo].[RoleUser]' 
GO

ALTER TABLE [dbo].[RoleUser] ALTER COLUMN [Account] nvarchar(256) NOT NULL
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
PRINT N'Creazione indice [IX_RoleUser_Type_Enabled]';
GO

CREATE NONCLUSTERED INDEX [IX_RoleUser_Type_Enabled]
ON [dbo].[RoleUser] ([Type],[Enabled])
INCLUDE ([idRole],[Account])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
   BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
   BEGIN
        INSERT INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
   END
GO

--#############################################################################
PRINT  N'Creazione indice [IX_RoleUser_idRole_Type_Enabled]';
GO

CREATE NONCLUSTERED INDEX [IX_RoleUser_idRole_Type_Enabled]
ON [dbo].[RoleUser] ([idRole],[Type],[Enabled])
INCLUDE ([Account])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
   BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
   BEGIN
        INSERT INTO #tmpErrors (Error) VALUES (1);
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