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
PRINT 'Versionamento database alla 8.51'
GO

EXEC dbo.VersioningDatabase N'8.51'
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

PRINT 'Modifica tabella CollaborationSigns colonna RegistrationUser not nullable'
GO

ALTER TABLE [dbo].[CollaborationSigns] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL

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

PRINT 'Modifica tabella CollaborationSigns colonna RegistrationDate not nullable'
GO

DROP INDEX [IX_CollaborationSigns_RegistationDate] ON [dbo].[CollaborationSigns] WITH ( ONLINE = OFF )
GO

ALTER TABLE [dbo].[CollaborationSigns] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET(7) NOT NULL
GO

CREATE CLUSTERED INDEX [IX_CollaborationSigns_RegistationDate] ON [dbo].[CollaborationSigns]
(
	[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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

PRINT 'Modifica tabella CollaborationUsers colonna RegistrationDate not nullable'
GO

DROP INDEX [IX_CollaborationUsers_RegistationDate] ON [dbo].[CollaborationUsers] WITH ( ONLINE = OFF )
GO

ALTER TABLE [dbo].[CollaborationUsers] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET(7) NOT NULL

CREATE CLUSTERED INDEX [IX_CollaborationUsers_RegistationDate] ON [dbo].[CollaborationUsers]
(
	[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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

PRINT 'Modifica tabella CollaborationUsers colonna RegistrationUser not nullable'
GO

ALTER TABLE [dbo].[CollaborationUsers] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL

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

PRINT 'ALTER TABLE [dbo].[Role] ADD [TenantId] [uniqueidentifier] NULL'
GO

ALTER TABLE [dbo].[Role] ADD [TenantId] [uniqueidentifier] NULL
GO
UPDATE [dbo].[Role] SET [TenantId] = '<immettere CurrentTenantId>'
GO
ALTER TABLE [dbo].[Role] ALTER COLUMN [TenantId] [uniqueidentifier] NOT NULL
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

PRINT 'Aggiunta colonna IdRoleTenant alla tabella Role'
GO

ALTER TABLE [dbo].[Role]
ADD [IdRoleTenant] int NULL
GO

UPDATE [dbo].[Role]
SET [IdRoleTenant] = [idRole]
GO

ALTER TABLE [dbo].[Role]
ALTER COLUMN [IdRoleTenant] smallint NOT NULL
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

PRINT 'Aggiunta colonna TenantId alla tabella SecurityGroups'
GO

ALTER TABLE [dbo].[SecurityGroups]
ADD [TenantId] [uniqueidentifier] NULL
GO

UPDATE [dbo].[SecurityGroups] 
SET [TenantId] = '<immettere CurrentTenantId>'
GO

ALTER TABLE [dbo].[SecurityGroups]
ALTER COLUMN [TenantId] [uniqueidentifier] NOT NULL

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

PRINT 'Aggiunta colonna IdSecurityGroupTenant alla tabella SecurityGroups'
GO

ALTER TABLE [dbo].[SecurityGroups]
ADD [IdSecurityGroupTenant] int NULL
GO

UPDATE [dbo].[SecurityGroups]
SET [IdSecurityGroupTenant] = [idGroup]
GO

ALTER TABLE [dbo].[SecurityGroups]
ALTER COLUMN [IdSecurityGroupTenant] int NOT NULL
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


PRINT 'Aggiunto indice [IX_Role_IdRoleTenant_TenantId] alla tabella Role'
GO

CREATE UNIQUE INDEX [IX_Role_IdRoleTenant_TenantId] ON [dbo].[Role]
(
[IdRoleTenant] ASC,
[TenantId] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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


PRINT 'Aggiunto indice [IX_SecurityGroups_IdSecurityGroupTenant_TenantId] alla tabella SecurityGroups'
GO

CREATE UNIQUE INDEX [IX_SecurityGroups_IdSecurityGroupTenant_TenantId] ON [dbo].[SecurityGroups]
(
[IdSecurityGroupTenant] ASC,
[TenantId] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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