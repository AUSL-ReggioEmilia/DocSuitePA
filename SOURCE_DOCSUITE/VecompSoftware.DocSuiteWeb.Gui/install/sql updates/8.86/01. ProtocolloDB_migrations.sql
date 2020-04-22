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
PRINT 'Versionamento database alla 8.86'
GO

EXEC dbo.VersioningDatabase N'8.86',N'DSW Version','MigrationDate'
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
PRINT N'Creazione Tabella Processes'
GO
CREATE TABLE [dbo].[Processes](
	[IdProcess] [uniqueidentifier] NOT NULL,
	[IdCategory] [smallint] NOT NULL,
	[IdDossier] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[FascicleType] [int] NOT NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7),
	[Note] [nvarchar](4000) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL
CONSTRAINT [PK_Processes] PRIMARY KEY NONCLUSTERED
	(
	[IdProcess] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_Processes_RegistrationDate] 
	ON [dbo].[Processes] ([RegistrationDate] ASC)
GO

ALTER TABLE [dbo].[Processes]  WITH CHECK ADD  CONSTRAINT [FK_Processes_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([IdCategory])
GO

ALTER TABLE [dbo].[Processes]  WITH CHECK ADD  CONSTRAINT [FK_Processes_Dossier] FOREIGN KEY([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

CREATE UNIQUE INDEX [IX_Processes_Name] 
	ON [dbo].[Processes] ([Name])
GO

ALTER TABLE [dbo].[Processes]
ADD CONSTRAINT DF_Processes_FascicleType
DEFAULT 1 FOR FascicleType
GO

ALTER TABLE [dbo].[Processes]
ADD CONSTRAINT DF_Processes_StartDate
DEFAULT GETUTCDATE() FOR [StartDate]
GO

ALTER TABLE [dbo].[Processes]  WITH CHECK 
ADD  CONSTRAINT [CHK_Processes_StartDate_EndDate] 
CHECK  (NOT [StartDate]>=[EndDate])
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
PRINT N'Creazione Tabella ProcessFascicleTemplates'
GO
CREATE TABLE [dbo].[ProcessFascicleTemplates](
	[IdProcessFascicleTemplate] [uniqueidentifier] NOT NULL,
	[IdProcess] [uniqueidentifier] NOT NULL,
	[IdDossierFolder] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[JsonModel] [nvarchar](MAX) NOT NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL
CONSTRAINT [PK_ProcessFascicleTemplates] PRIMARY KEY NONCLUSTERED
	(
	[IdProcessFascicleTemplate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_ProcessFascicleTemplates_RegistrationDate] 
	ON [dbo].[ProcessFascicleTemplates] ([RegistrationDate] ASC)
GO

ALTER TABLE [dbo].[ProcessFascicleTemplates]  WITH CHECK ADD  CONSTRAINT [FK_ProcessFascicleTemplates_Processes] FOREIGN KEY([IdProcess])
REFERENCES [dbo].[Processes] ([IdProcess])
GO

ALTER TABLE [dbo].[ProcessFascicleTemplates]  WITH CHECK ADD  CONSTRAINT [FK_ProcessFascicleTemplates_DossierFolders] FOREIGN KEY([IdDossierFolder])
REFERENCES [dbo].[DossierFolders] ([IdDossierFolder])
GO

ALTER TABLE [dbo].[ProcessFascicleTemplates]
ADD CONSTRAINT DF_ProcessFascicleTemplates_StartDate
DEFAULT GETUTCDATE() FOR [StartDate]
GO

ALTER TABLE [dbo].[ProcessFascicleTemplates]  WITH CHECK 
ADD  CONSTRAINT [CHK_ProcessFascicleTemplates_StartDate_EndDate] 
CHECK  (NOT [StartDate]>=[EndDate])
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
PRINT N'Creazione Tabella ProcessFascicleWorkflowRepositories'
GO
CREATE TABLE [dbo].[ProcessFascicleWorkflowRepositories](
	[IdProcessFascicleWorkflowRepository] [uniqueidentifier] NOT NULL,
	[IdProcess] [uniqueidentifier] NOT NULL,
	[IdDossierFolder] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL

CONSTRAINT [PK_ProcessFascicleWorkflowRepositories] PRIMARY KEY NONCLUSTERED
	(
	[IdProcessFascicleWorkflowRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_ProcessFascicleWorkflowRepositories_RegistrationDate] 
	ON [dbo].[ProcessFascicleWorkflowRepositories] ([RegistrationDate] ASC)
GO

ALTER TABLE [dbo].[ProcessFascicleWorkflowRepositories]  WITH CHECK ADD  CONSTRAINT [FK_ProcessFascicleWorkflowRepositories_Processes] FOREIGN KEY([IdProcess])
REFERENCES [dbo].[Processes] ([IdProcess])
GO

ALTER TABLE [dbo].[ProcessFascicleWorkflowRepositories]  WITH CHECK ADD  CONSTRAINT [FK_ProcessFascicleWorkflowRepositories_DossierFolders] FOREIGN KEY([IdDossierFolder])
REFERENCES [dbo].[DossierFolders] ([IdDossierFolder])
GO

ALTER TABLE [dbo].[ProcessFascicleWorkflowRepositories]  WITH CHECK ADD  CONSTRAINT [FK_ProcessFascicleWorkflowRepositories_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
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
PRINT N'Creazione Tabella ProcessRoles'
GO
CREATE TABLE [dbo].[ProcessRoles](
	[IdProcessRole] [uniqueidentifier] NOT NULL,
	[IdProcess] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[Timestamp] [timestamp] NOT NULL

CONSTRAINT [PK_ProcessRoles] PRIMARY KEY NONCLUSTERED
	(
	[IdProcessRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_ProcessRoles_RegistrationDate] 
	ON [dbo].[ProcessRoles] ([RegistrationDate] ASC)
GO

ALTER TABLE [dbo].[ProcessRoles]  WITH CHECK ADD  CONSTRAINT [FK_ProcessRoles_Processes] FOREIGN KEY([IdProcess])
REFERENCES [dbo].[Processes] ([IdProcess])
GO

ALTER TABLE [dbo].[ProcessRoles]  WITH CHECK ADD  CONSTRAINT [FK_ProcessRoles_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([IdRole])
GO

ALTER TABLE [dbo].[ProcessRoles] 
ADD  DEFAULT (newid()) FOR [IdProcessRole]
GO

ALTER TABLE [dbo].[ProcessRoles] 
ADD  DEFAULT (getutcdate()) FOR [RegistrationDate]
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
PRINT N'Aggiungere la colonna IdProcessFascicleTemplate nella tabella Fascicles'
GO
ALTER TABLE [dbo].[Fascicles]
ADD [IdProcessFascicleTemplate] [uniqueidentifier] NULL
CONSTRAINT [FK_Fascicles_ProcessFascicleTemplates] FOREIGN KEY([IdProcessFascicleTemplate]) REFERENCES [dbo].[ProcessFascicleTemplates] ([IdProcessFascicleTemplate])
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
PRINT 'ALTER TABLE [dbo].[PosteOnLineAccount] ADD [ExtendedProperties]'
GO
  
ALTER TABLE [dbo].[PosteOnLineAccount] ADD ExtendedProperties nvarchar(max)
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
PRINT 'ALTER TABLE [dbo].[PosteOnLineContact] ADD [ExtendedProperties]'
GO
ALTER TABLE [dbo].[PosteOnLineContact] ADD ExtendedProperties nvarchar(max)
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

PRINT N'CREATE TABLE [dbo].[TenantContacts]';
GO
CREATE TABLE [dbo].[TenantContacts] (
    [IdTenantContact][uniqueidentifier] NOT NULL,
	[IdTenant] [uniqueidentifier] NOT NULL,
    [EntityId] [int] NOT NULL,
    [RegistrationDate] [datetimeoffset](7) NOT NULL,
    [Timestamp] [timestamp] NOT NULL
	
	CONSTRAINT [PK_TenantContacts] PRIMARY KEY NONCLUSTERED
	(
		[IdTenantContact] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

ALTER TABLE [dbo].[TenantContacts]  WITH CHECK ADD  CONSTRAINT [FK_TenantContact_Tenant] FOREIGN KEY([IdTenant])
REFERENCES [dbo].[Tenants] ([IdTenant])
GO

ALTER TABLE [dbo].[TenantContacts]  WITH CHECK ADD  CONSTRAINT [FK_TenantContact_Contact] FOREIGN KEY([EntityId])
REFERENCES [dbo].[Contact] ([Incremental])
GO

ALTER TABLE [dbo].[TenantContacts]
  ADD CONSTRAINT RegistrationDate_default
  DEFAULT getutcdate() FOR [RegistrationDate];
GO

ALTER TABLE [dbo].[TenantContacts]
  ADD CONSTRAINT IdTenantContact_default
  DEFAULT NEWID() FOR [IdTenantContact];
GO

CREATE CLUSTERED INDEX [IX_TenantContacts_RegistrationDate] 
	ON [dbo].[TenantContacts] ([RegistrationDate] ASC)
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
