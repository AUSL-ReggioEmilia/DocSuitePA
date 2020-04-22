/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<UTENTE_DEFAULT, varchar(256),>	--> Settare il nome dell'utente.
*	<FIRST_PRIVACY_LEVEL_DESCRIPTION, varchar(256), Riservato> --> Settare Descrizione per il primo livello di privacy
*****************************************************************************************************************************************/

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

ALTER TABLE [dbo].[Category] WITH CHECK ADD CONSTRAINT [FK_Category_MetadataRepository] FOREIGN KEY([IdMetadataRepository])
REFERENCES [dbo].[MetadataRepositories] ([IdMetadataRepository])
GO

ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_MetadataRepository]
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
PRINT 'Aggiunta colonna [MetadataValues] in [Fascicles]'
GO

ALTER TABLE [dbo].[Fascicles] ADD [MetadataValues] nvarchar(max) null
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
PRINT 'Aggiunta colonna [PrivacyLevel] in [UserLog]'
GO

ALTER TABLE [dbo].[UserLog] ADD [PrivacyLevel] INT NULL
GO

UPDATE [dbo].[UserLog] SET [PrivacyLevel] = 0
GO

ALTER TABLE [dbo].[UserLog] ALTER COLUMN [PrivacyLevel] INT NOT NULL
GO

ALTER TABLE [dbo].[UserLog] ADD CONSTRAINT [DF_UserLog_PrivacyLevel]
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
PRINT 'Creata tabella [dbo].[PrivacyLevels]'
GO

CREATE TABLE [dbo].[PrivacyLevels](
	[IdPrivacyLevel] [uniqueidentifier] NOT NULL,
	[Level] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL DEFAULT 1,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL
 CONSTRAINT [PK_PrivacyLevels] PRIMARY KEY NONCLUSTERED 
(
	[IdPrivacyLevel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_PrivacyLevels_RegistrationDate] 
	ON [dbo].[PrivacyLevels] ([RegistrationDate] ASC)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PrivacyLevels_Level]
    ON [dbo].[PrivacyLevels]([Level] ASC);
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
PRINT 'Inseriti i valori di default nella tabella [dbo].[PrivacyLevels]'
GO

INSERT INTO [dbo].[PrivacyLevels] (IdPrivacyLevel, Level, Description, RegistrationUser, RegistrationDate) values (newid(), 0, 'Pubblico', '<UTENTE_DEFAULT, varchar(256), ''>', sysdatetimeoffset())
GO

INSERT INTO [dbo].[PrivacyLevels] (IdPrivacyLevel, Level, Description, RegistrationUser, RegistrationDate) values (newid(), 1, '<FIRST_PRIVACY_LEVEL_DESCRIPTION, varchar(256), Riservato>', '<UTENTE_DEFAULT, varchar(256), ''>', sysdatetimeoffset())
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
PRINT 'Eliminata colonna [IdLocation] in [TemplateDocumentRepositories]'
GO

ALTER TABLE [dbo].[TemplateDocumentRepositories] DROP CONSTRAINT FK_TemplateDocumentRepositories_Location
GO

ALTER TABLE	[dbo].[TemplateDocumentRepositories] DROP COLUMN IdLocation
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