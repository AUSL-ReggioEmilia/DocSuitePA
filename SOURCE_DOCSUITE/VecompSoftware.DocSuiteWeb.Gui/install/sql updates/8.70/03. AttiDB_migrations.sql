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
PRINT 'Versionamento database alla 8.70'
GO

EXEC dbo.VersioningDatabase N'8.70'
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
PRINT 'Aggiunta colonna [IdMetadataRepository] in Category'
GO

ALTER TABLE [dbo].[Category] ADD [IdMetadataRepository] uniqueidentifier null
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
PRINT 'Aggiunta colonna [ManageSecureDocument] in [Container]'
GO

ALTER TABLE [dbo].[Container] ADD [ManageSecureDocument] bit null
GO

UPDATE [dbo].[Container] SET [ManageSecureDocument] = 0
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [ManageSecureDocument] [bit] NOT NULL
GO

ALTER TABLE [dbo].[Container] ADD CONSTRAINT [DF_Container_ManageSecureDocument] 
	DEFAULT 0 FOR [ManageSecureDocument];
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
PRINT 'Aggiunta colonna [PrivacyLevel] in [Container]'
GO

ALTER TABLE [dbo].[Container] ADD [PrivacyLevel] INT  NULL
GO

UPDATE [dbo].[Container] SET [PrivacyLevel] = 0
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [PrivacyLevel] INT  NOT NULL
GO

ALTER TABLE [dbo].[Container] ADD CONSTRAINT [DF_Container_PrivacyLevel]
DEFAULT 0 FOR [PrivacyLevel]
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
PRINT 'Aggiunta colonna [PrivacyLevel] in [ContainerGroup]'
GO

ALTER TABLE [dbo].[ContainerGroup] ADD [PrivacyLevel] INT  NULL
GO

UPDATE [dbo].[ContainerGroup] SET [PrivacyLevel] = 0
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [PrivacyLevel] INT  NOT NULL
GO

ALTER TABLE [dbo].[ContainerGroup] ADD CONSTRAINT [DF_ContainerGroup_PrivacyLevel]
DEFAULT 0 FOR [PrivacyLevel]
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
PRINT 'Aggiunta colonna [PrivacyEnabled] in [Container]'
GO

ALTER TABLE [dbo].[Container] ADD [PrivacyEnabled] bit  NULL
GO

UPDATE [dbo].[Container] SET [PrivacyEnabled] = 0
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [PrivacyEnabled] [bit]  NOT NULL
GO

ALTER TABLE [dbo].[Container] ADD CONSTRAINT [DF_Container_PrivacyEnabled]
DEFAULT 0 FOR [PrivacyEnabled]
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