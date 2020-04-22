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
PRINT 'Versionamento database alla 8.53'
GO

EXEC dbo.VersioningDatabase N'8.53'
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

PRINT 'Creazione colonna UniqueId nullable nella tabella ResolutionRole'
GO
-- 1 
ALTER TABLE [dbo].[ResolutionRole] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella ResolutionRole'
GO
-- 2 
UPDATE [dbo].[ResolutionRole]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[ResolutionRole]
    ADD CONSTRAINT [DF_ResolutionRole_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[ResolutionRole] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT N'Aggiunto unique indice [IX_ResolutionRole_UniqueId] in ResolutionRole';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ResolutionRole_UniqueId]
    ON [dbo].[ResolutionRole]([UniqueId] ASC);
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

PRINT 'Creazione colonna UniqueId nullable nella tabella FileResolution'
GO
-- 1 
ALTER TABLE [dbo].[FileResolution] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella FileResolution'
GO
-- 2 
UPDATE [dbo].[FileResolution]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[FileResolution]
    ADD CONSTRAINT [DF_FileResolution_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[FileResolution] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT N'Aggiunto unique indice [IX_FileResolution_UniqueId] in FileResolution';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FileResolution_UniqueId]
    ON [dbo].[FileResolution]([UniqueId] ASC);
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


PRINT N'Modificata colonna [Conservation] in Container';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [Conservation] smallint null
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

PRINT N'Modificata colonna [isActive] in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [isActive] tinyint null
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

PRINT N'Modificata colonna [Collapsed] in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [Collapsed] tinyint null
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