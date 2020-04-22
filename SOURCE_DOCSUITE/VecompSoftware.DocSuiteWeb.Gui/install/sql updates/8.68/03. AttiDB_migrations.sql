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
PRINT N'Aggiunta della colonna Timestamp alla tabella FileResolution';
GO

ALTER TABLE [dbo].[FileResolution] ADD [Timestamp] TIMESTAMP NOT NULL
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
PRINT N'Aggiunta della colonna UniqueIdResolution alla tabella FileResolution';
GO

ALTER TABLE [dbo].[FileResolution] ADD [UniqueIdResolution] [uniqueidentifier] NULL
GO


UPDATE FR SET FR.[UniqueIdResolution] = R.uniqueid
FROM dbo.FileResolution AS FR
INNER JOIN dbo.Resolution R ON R.idResolution = FR.idResolution
WHERE fr.[UniqueIdResolution] IS NULL
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

PRINT N'DROP ODG STRUCTURES';
GO

DROP TABLE [dbo].[ResolutionODGTasks]
GO
DROP TABLE [dbo].[ResolutionODGTaskDetails]
GO
DROP TABLE [dbo].[ResolutionODGDetails]
GO
DROP TABLE [dbo].[ResolutionODG]

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
PRINT N'Aggiunta della colonna [UniqueIdResolution] alla tabella [dbo].[ResolutionLog]';
GO

ALTER TABLE [dbo].[ResolutionLog] ADD [UniqueIdResolution] [uniqueidentifier] NULL
GO

UPDATE RL SET RL.[UniqueIdResolution] = R.uniqueid
FROM dbo.ResolutionLog AS RL
INNER JOIN dbo.Resolution R ON R.idResolution = RL.idResolution
WHERE RL.[UniqueIdResolution] IS NULL
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
PRINT N'Aggiunta delle colonna [UniqueId] alla tabella [dbo].[ResolutionLog]';
GO

ALTER TABLE [dbo].[ResolutionLog] ADD [UniqueId] uniqueidentifier NULL
GO

UPDATE [dbo].[ResolutionLog] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[ResolutionLog] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
GO

CREATE UNIQUE INDEX [IX_ResolutionLog_UniqueId] ON [dbo].[ResolutionLog]([UniqueId] ASC);
GO

ALTER TABLE [dbo].[ResolutionLog] ADD CONSTRAINT [DF_ResolutionLog_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
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
PRINT N'Aggiunta delle colonna [Timestamp] alla tabella [dbp].[ResolutionLog]';
GO

ALTER TABLE [dbo].[ResolutionLog] ADD [Timestamp] TIMESTAMP NOT NULL
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