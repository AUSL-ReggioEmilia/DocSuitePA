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
PRINT 'Versionamento database alla 8.74'
GO

EXEC dbo.VersioningDatabase N'8.74'
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
PRINT N'Modificata colonna Rights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [Rights] CHAR(20) NOT NULL
GO
UPDATE ContainerGroup SET Rights = SUBSTRING(Rights, 0, 11) + '0000000000'
WHERE LEN(Rights) = 10
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
PRINT N'Modificata colonna ResolutionRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [ResolutionRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET ResolutionRights = SUBSTRING(ResolutionRights, 0, 11) + '0000000000'
WHERE LEN(ResolutionRights) = 10
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
PRINT N'Modificata colonna DocumentRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [DocumentRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET DocumentRights = SUBSTRING(DocumentRights, 0, 11) + '0000000000'
WHERE LEN(DocumentRights) = 10
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
PRINT N'Modificata colonna DocumentSeriesRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [DocumentSeriesRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET DocumentSeriesRights = SUBSTRING(DocumentSeriesRights, 0, 11) + '0000000000'
WHERE LEN(DocumentSeriesRights) = 10
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
PRINT N'Modificata colonna DeskRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [DeskRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET DeskRights = SUBSTRING(DeskRights, 0, 11) + '0000000000'
WHERE LEN(DeskRights) = 10
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
PRINT N'Modificata colonna UDSRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [UDSRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET UDSRights = SUBSTRING(UDSRights, 0, 11) + '0000000000'
WHERE LEN(UDSRights) = 10
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
PRINT N'Modificata colonna [ProtocolRights] della tabella [dbo].[CategoryGroup]';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [ProtocolRights] CHAR(20) NOT NULL
GO
UPDATE CategoryGroup SET ProtocolRights = SUBSTRING(ProtocolRights, 0, 6) + '000000000000000'
WHERE LEN(ProtocolRights) = 5
GO
UPDATE CategoryGroup SET ProtocolRights = SUBSTRING(ProtocolRights, 0, 11) + '0000000000'
WHERE LEN(ProtocolRights) = 10
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
PRINT N'Modificata colonna [ResolutionRights] della tabella [dbo].[CategoryGroup]';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [ResolutionRights] CHAR(20) NULL
GO
UPDATE CategoryGroup SET ResolutionRights = SUBSTRING(ResolutionRights, 0, 6) + '000000000000000'
WHERE LEN(ResolutionRights) = 5
GO
UPDATE CategoryGroup SET ResolutionRights = SUBSTRING(ResolutionRights, 0, 11) + '0000000000'
WHERE LEN(ResolutionRights) = 10
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
PRINT N'Modificata colonna [DocumentRights] della tabella [dbo].[CategoryGroup]';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [DocumentRights] CHAR(20) NULL
GO
UPDATE CategoryGroup SET DocumentRights = SUBSTRING(DocumentRights, 0, 6) + '000000000000000'
WHERE LEN(DocumentRights) = 5
GO
UPDATE CategoryGroup SET DocumentRights = SUBSTRING(DocumentRights, 0, 11) + '0000000000'
WHERE LEN(DocumentRights) = 10
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
PRINT N'Modificata colonna [ProtocolRights] della tabella [dbo].[RoleGroup]';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [ProtocolRights] CHAR(20) NULL
GO
UPDATE RoleGroup SET ProtocolRights = SUBSTRING(ProtocolRights, 0, 11) + '0000000000'
WHERE LEN(ProtocolRights) = 10
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
PRINT N'Modificata colonna [ResolutionRights] della tabella [dbo].[RoleGroup]';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [ResolutionRights] CHAR(20) NULL
GO
UPDATE RoleGroup SET ResolutionRights = SUBSTRING(ResolutionRights, 0, 11) + '0000000000'
WHERE LEN(ResolutionRights) = 10
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
PRINT N'Modificata colonna [DocumentRights] della tabella [dbo].[RoleGroup]';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [DocumentRights] CHAR(20) NULL
GO
UPDATE RoleGroup SET DocumentRights = SUBSTRING(DocumentRights, 0, 11) + '0000000000'
WHERE LEN(DocumentRights) = 10
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
PRINT N'Modificata colonna [DocumentSeriesRights] della tabella [dbo].[RoleGroup]';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [DocumentSeriesRights] CHAR(20) NULL
GO
UPDATE RoleGroup SET DocumentSeriesRights = SUBSTRING(DocumentSeriesRights, 0, 11) + '0000000000'
WHERE LEN(DocumentSeriesRights) = 10
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