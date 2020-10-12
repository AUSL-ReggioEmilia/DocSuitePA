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
PRINT 'Versionamento database alla 9.04'
GO

EXEC dbo.VersioningDatabase N'9.04',N'DSW Version','MigrationDate'
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
PRINT 'ALTER TABLE [dbo].[Role] ADD [Timestamp]'
GO

ALTER TABLE [dbo].[Role] ADD [Timestamp] timestamp NOT NULL
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
PRINT 'CREATE TABLE [dbo].[MetadataValues]'
GO

CREATE TABLE [dbo].[MetadataValues](
	[IdMetadataValue] [uniqueidentifier] NOT NULL,
	[IdFascicle] [uniqueidentifier] NULL,
	[IdDossier] [uniqueidentifier] NULL,
	[PropertyType] [smallint] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[ValueString] [nvarchar](MAX) NULL,
	[ValueInt] [bigint] NULL,
	[ValueDate] [datetime] NULL,
	[ValueDouble] [float] NULL,
	[ValueBoolean] [bit] NULL,
	[ValueGuid] [uniqueidentifier] NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_MetadataValues] PRIMARY KEY NONCLUSTERED 
(
	[IdMetadataValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_MetadataValues_RegistrationDate] 
	ON [dbo].[MetadataValues] ([RegistrationDate] ASC)
GO

ALTER TABLE [dbo].[MetadataValues]  WITH CHECK ADD  CONSTRAINT [FK_MetadataValues_Dossiers] FOREIGN KEY([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[MetadataValues] CHECK CONSTRAINT [FK_MetadataValues_Dossiers]
GO

ALTER TABLE [dbo].[MetadataValues]  WITH CHECK ADD  CONSTRAINT [FK_MetadataValues_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO

ALTER TABLE [dbo].[MetadataValues] CHECK CONSTRAINT [FK_MetadataValues_Fascicles]
GO

CREATE UNIQUE INDEX [IX_MetadataValues_IdFascicle_IdDossier_Name] 
	ON [dbo].[MetadataValues] ([IdFascicle],[IdDossier],[Name])
GO

ALTER TABLE [dbo].[MetadataValues]  WITH CHECK ADD  CONSTRAINT [CHK_MetadataValues_IdFascicle_IdDossier] 
CHECK  (([IdFascicle] IS NULL AND [IdDossier] IS NOT NULL) OR ([IdFascicle] IS NOT NULL AND [IdDossier] IS NULL))
GO

ALTER TABLE [dbo].[MetadataValues] CHECK CONSTRAINT [CHK_MetadataValues_IdFascicle_IdDossier]
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
PRINT 'CREATE TABLE [dbo].[MetadataValueContacts]'
GO

CREATE TABLE [dbo].[MetadataValueContacts](
	[IdMetadataValueContact] [uniqueidentifier] NOT NULL,
	[IdFascicle] [uniqueidentifier] NULL,
	[IdDossier] [uniqueidentifier] NULL,
	[Name] [nvarchar](256) NOT NULL,
	[IdContact] [int] NULL,
	[ContactManual] [nvarchar](MAX) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_MetadataValueContacts] PRIMARY KEY NONCLUSTERED 
(
	[IdMetadataValueContact] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_MetadataValueContacts_RegistrationDate] 
	ON [dbo].[MetadataValueContacts] ([RegistrationDate] ASC)
GO

ALTER TABLE [dbo].[MetadataValueContacts]  WITH CHECK ADD  CONSTRAINT [FK_MetadataValueContacts_Dossiers] FOREIGN KEY([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[MetadataValueContacts] CHECK CONSTRAINT [FK_MetadataValueContacts_Dossiers]
GO

ALTER TABLE [dbo].[MetadataValueContacts]  WITH CHECK ADD  CONSTRAINT [FK_MetadataValueContacts_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO

ALTER TABLE [dbo].[MetadataValueContacts] CHECK CONSTRAINT [FK_MetadataValueContacts_Fascicles]
GO

CREATE UNIQUE INDEX [IX_MetadataValueContacts_IdFascicle_IdDossier_Name] 
	ON [dbo].[MetadataValueContacts] ([IdFascicle],[IdDossier],[Name])
GO

ALTER TABLE [dbo].[MetadataValueContacts]  WITH CHECK ADD  CONSTRAINT [CHK_MetadataValueContacts_IdFascicle_IdDossier] 
CHECK  (([IdFascicle] IS NULL AND [IdDossier] IS NOT NULL) OR ([IdFascicle] IS NOT NULL AND [IdDossier] IS NULL))
GO

ALTER TABLE [dbo].[MetadataValueContacts] CHECK CONSTRAINT [CHK_MetadataValueContacts_IdFascicle_IdDossier]
GO

ALTER TABLE [dbo].[MetadataValueContacts]  WITH CHECK ADD  CONSTRAINT [CHK_MetadataValueContacts_IdContact_ContactManual] 
CHECK  (([IdContact] IS NULL AND [ContactManual] IS NOT NULL) OR ([IdContact] IS NOT NULL AND [ContactManual] IS NULL))
GO

ALTER TABLE [dbo].[MetadataValueContacts] CHECK CONSTRAINT [CHK_MetadataValueContacts_IdContact_ContactManual]
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
PRINT 'Rename column MetadataValues to MetadataDesigner into [dbo].[Fascicles]'
GO

EXEC sp_rename 'dbo.Fascicles.MetadataValues', 'MetadataDesigner', 'COLUMN';
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
PRINT 'Rename column JsonMetadata to MetadataDesigner into [dbo].[Dossiers]'
GO

EXEC sp_rename 'dbo.Dossiers.JsonMetadata', 'MetadataDesigner', 'COLUMN';
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
PRINT 'ALTER TABLE [dbo].[Fascicles] ADD [MetadataValues]'
GO

ALTER TABLE [dbo].[Fascicles] ADD [MetadataValues] [nvarchar](MAX) NULL
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
PRINT 'ALTER TABLE [dbo].[Dossiers] ADD [MetadataValues]'
GO

ALTER TABLE [dbo].[Dossiers] ADD [MetadataValues] [nvarchar](MAX) NULL
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
PRINT 'DROP AND CREATE UNIQUE NONCLUSTERED INDEX [IX_Processes_Name_IdCategory] ON [dbo].[Processes]'
GO

DROP INDEX [IX_Processes_Name] ON [dbo].[Processes]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Processes_Name_IdCategory] ON [dbo].[Processes]
(
	[Name] ASC,
	[IdCategory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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