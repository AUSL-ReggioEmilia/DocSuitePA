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
PRINT 'Versionamento database alla 8.65'
GO

EXEC dbo.VersioningDatabase N'8.65'
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
PRINT 'Creazione della tabella [Dossiers]'
GO

CREATE TABLE [dbo].[Dossiers]
(
	[IdDossier] [uniqueidentifier] NOT NULL,
	[IdContainer] [smallint] NOT NULL,
	[Year] [smallint] NOT NULL,
	[Number] [int] NOT NULL,
	[Subject] [nvarchar] (512) NOT NULL,
	[Note] [nvarchar] (1000) NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset] (7) NULL,
	[JsonMetadata] [nvarchar] (max) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
CONSTRAINT [PK_Dossiers] PRIMARY KEY NONCLUSTERED
(
	[IdDossier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_Dossiers_RegistrationDate] 
	ON [dbo].[Dossiers] ([RegistrationDate] ASC)
	GO

ALTER TABLE [dbo].[Dossiers] WITH CHECK ADD CONSTRAINT [FK_Dossiers_Container] FOREIGN KEY (IdContainer)
REFERENCES [dbo].[Container] ([IdContainer])
GO

ALTER TABLE [dbo].[Dossiers] CHECK CONSTRAINT [FK_Dossiers_Container]
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
PRINT N'Creazione della tabella [dbo].[DossierLogs]';
GO

CREATE TABLE [dbo].[DossierLogs](
	[IdDossierLog] [uniqueidentifier] NOT NULL,
	[IdDossier] [uniqueidentifier] NOT NULL,
	[LogDate] [datetimeoffset](7) NOT NULL,
	[SystemComputer] [nvarchar](30) NOT NULL,
	[SystemUser] [nvarchar](256) NOT NULL,
	[LogType] [smallint] NOT NULL,
	[LogDescription] [nvarchar](max) NOT NULL,
	[Severity] [smallint] NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DossierLogs] PRIMARY KEY NONCLUSTERED 
(
	[IdDossierLog] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[DossierLogs]  WITH CHECK ADD  CONSTRAINT [FK_DossierLogs_Dossiers] FOREIGN KEY([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[DossierLogs] CHECK CONSTRAINT [FK_DossierLogs_Dossiers]
GO

CREATE CLUSTERED INDEX [IX_DossierLogs_LogDate] ON [dbo].[DossierLogs]([LogDate] ASC);
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
PRINT 'Creazione della tabella [dbo].[DossierContacts]'
GO

CREATE TABLE [dbo].[DossierContacts](
	[IdDossierContact] [uniqueidentifier] NOT NULL,
	[IdContact] [int] NOT NULL,
	[IdDossier][uniqueidentifier] NOT NULL,
    [RegistrationDate] [dateTimeOffset](7) NULL,
	      
 CONSTRAINT [PK_DossierContacts] PRIMARY KEY NONCLUSTERED 
(
	[IdDossierContact] ASC
  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DossierContacts] ADD  CONSTRAINT [DF_DossierContacts_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]
GO

CREATE CLUSTERED INDEX [IX_DossierContacts_RegistrationDate]
    ON [dbo].[DossierContacts]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[DossierContacts] ADD DEFAULT (newsequentialid()) FOR [IdDossierContact]
GO

ALTER TABLE [dbo].[DossierContacts]  WITH CHECK ADD  CONSTRAINT [FK_DossierContacts_Contact] FOREIGN KEY([IdContact])
REFERENCES [dbo].[Contact] ([Incremental])
GO

ALTER TABLE [dbo].[DossierContacts] CHECK CONSTRAINT [FK_DossierContacts_Contact]
GO

ALTER TABLE [dbo].[DossierContacts]  WITH CHECK ADD  CONSTRAINT [FK_DossierContacts_Dossiers] FOREIGN KEY([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[DossierContacts] CHECK CONSTRAINT [FK_DossierContacts_Dossiers]
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
PRINT 'Creazione della tabella [dbo].[DossierMessages]'
GO

CREATE TABLE [dbo].[DossierMessages](
	[IdDossierMessage] [uniqueidentifier] NOT NULL,
    [IdMessage] [int] NOT NULL,
	[IdDossier] [uniqueidentifier] NOT NULL,
    [RegistrationDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_DossierMessages] PRIMARY KEY NONCLUSTERED 
(
	[IdDossierMessage] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DossierMessages] ADD  CONSTRAINT [DF_DossierMessages_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]
GO

CREATE CLUSTERED INDEX [IX_DossierMessages_RegistrationDate]
    ON [dbo].[DossierMessages]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[DossierMessages]  WITH CHECK ADD  CONSTRAINT [FK_DossierMessages_Dossier] FOREIGN KEY([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[DossierMessages]  WITH CHECK ADD  CONSTRAINT [FK_DossierMessages_Message] FOREIGN KEY([IdMessage])
REFERENCES [dbo].[Message] ([IDMessage])
GO

ALTER TABLE [dbo].[DossierMessages] CHECK CONSTRAINT [FK_DossierMessages_Message]
GO

ALTER TABLE [dbo].[DossierMessages] CHECK CONSTRAINT [FK_DossierMessages_Dossier]
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
PRINT 'Creazione della tabella [dbo].[DossierDocuments]'
GO

CREATE TABLE [dbo].[DossierDocuments](
	[IdDossierDocument] [uniqueidentifier] NOT NULL,
	[IdDossier] [uniqueidentifier] NOT NULL,
	[IdArchiveChain] [uniqueidentifier] NOT NULL,
	[ChainType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DossierDocuments] PRIMARY KEY NONCLUSTERED 
(
	[IdDossierDocument] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DossierDocuments]  WITH CHECK ADD  CONSTRAINT [FK_DossierDocuments_Dossiers] FOREIGN KEY([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[DossierDocuments] CHECK CONSTRAINT [FK_DossierDocuments_Dossiers]
GO

CREATE CLUSTERED INDEX [IX_DossierDocuments_RegistationDate]
    ON [dbo].[DossierDocuments]([RegistrationDate] ASC);
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
PRINT N'CREAZIONE DELLA TABELLA [DossierRoles]'
GO

CREATE TABLE [dbo].[DossierRoles](
	[IdDossierRole] [uniqueidentifier] NOT NULL,
	[IdDossier] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[RoleAuthorizationType] [smallint] NOT NULL,
	[IsMaster] [bit] NOT NULL,
	[Status] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
CONSTRAINT [PK_DossierRoles] PRIMARY KEY NONCLUSTERED
(
	[IdDossierRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON ) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_DossierRoles_RegistrationDate] 
	ON [dbo].[DossierRoles] ([RegistrationDate] ASC)
	GO

ALTER TABLE [dbo].[DossierRoles] WITH CHECK ADD CONSTRAINT [FK_DossierRoles_Dossiers] FOREIGN KEY ([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[DossierRoles] CHECK CONSTRAINT [FK_DossierRoles_Dossiers]
GO

ALTER TABLE [dbo].[DossierRoles] WITH CHECK ADD CONSTRAINT [FK_DossierRoles_Role] FOREIGN KEY ([IdRole])
REFERENCES [dbo].[Role]([IdRole])
GO

ALTER TABLE [dbo].[DossierRoles] CHECK CONSTRAINT [FK_DossierRoles_Role]
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

PRINT 'Creazione della tabella [dbo].[DossierFolders]'
GO

CREATE TABLE [dbo].[DossierFolders](
	[IdDossierFolder] [uniqueidentifier] NOT NULL,
	[IdDossier] [uniqueidentifier] NOT NULL,
	[IdFascicle] [uniqueidentifier] NULL,
	[IdCategory] [smallint] NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Status] [smallint] NOT NULL,
    [JsonMetadata] [nvarchar](max) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DossierFolders] PRIMARY KEY NONCLUSTERED 
(
	[IdDossierFolder] ASC

)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DossierFolders]  WITH CHECK ADD  CONSTRAINT [FK_DossierFolders_Dossiers] FOREIGN KEY([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[DossierFolders] CHECK CONSTRAINT [FK_DossierFolders_Dossiers]
GO

ALTER TABLE [dbo].[DossierFolders]  WITH CHECK ADD  CONSTRAINT [FK_DossierFolders_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO

ALTER TABLE [dbo].[DossierFolders] CHECK CONSTRAINT [FK_DossierFolders_Fascicles]
GO

ALTER TABLE [dbo].[DossierFolders]  WITH CHECK ADD  CONSTRAINT [FK_DossierFolders_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([idCategory])
GO

ALTER TABLE [dbo].[DossierFolders] CHECK CONSTRAINT [FK_DossierFolders_Category]
GO

CREATE CLUSTERED INDEX [IX_DossierFolders_RegistrationDate] ON [dbo].[DossierFolders] ([RegistrationDate] ASC)
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
PRINT 'Creazione della tabella [dbo].[DossierFolderRoles]'
GO

CREATE TABLE [dbo].[DossierFolderRoles](
	[IdDossierFolderRole] [uniqueidentifier] NOT NULL,
	[IdDossierFolder] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[RoleAuthorizationType] [smallint] NOT NULL,
	[Status][smallint] NOT NULL,
	[IsMaster][bit] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
	 CONSTRAINT [PK_DossierFolderRoles] PRIMARY KEY NONCLUSTERED 
(
	[IdDossierFolderRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE CLUSTERED INDEX [IX_DossierFolderRoles_RegistrationDate]
    ON [dbo].[DossierFolderRoles]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[DossierFolderRoles]  WITH CHECK ADD  CONSTRAINT [FK_DossierFolderRoles_DossierFolders] FOREIGN KEY([IdDossierFolder])
REFERENCES [dbo].[DossierFolders] ([IdDossierFolder])
GO

ALTER TABLE [dbo].[DossierFolderRoles] CHECK CONSTRAINT [FK_DossierFolderRoles_DossierFolders]
GO

ALTER TABLE [dbo].[DossierFolderRoles]  WITH CHECK ADD  CONSTRAINT [FK_DossierFolderRoles_Roles] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([idRole])
GO

ALTER TABLE [dbo].[DossierFolderRoles] CHECK CONSTRAINT [FK_DossierFolderRoles_Roles]
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
PRINT 'Creazione della tabella [dbo].[DossierComments]'
GO

CREATE TABLE [dbo].[DossierComments](
	[IdDossierComment] [uniqueidentifier] NOT NULL,
	[IdDossier] [uniqueidentifier] NOT NULL,
	[IdDossierFolder] [uniqueidentifier] NULL,
	[Author] [nvarchar](256) NOT NULL,
	[Comment] [nvarchar](max) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
CONSTRAINT [PK_DossierComments] PRIMARY KEY NONCLUSTERED 
(
	[IdDossierComment] ASC

)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]

CREATE CLUSTERED INDEX [IX_DossierComments_RegistrationDate] 
	ON [dbo].[DossierComments] ([RegistrationDate] ASC)
	GO

	ALTER TABLE [dbo].[DossierComments] WITH CHECK ADD CONSTRAINT [FK_DossierComments_Dossiers] FOREIGN KEY ([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[DossierComments] CHECK CONSTRAINT [FK_DossierComments_Dossiers]

ALTER TABLE [dbo].[DossierComments] WITH CHECK ADD CONSTRAINT [FK_DossierComments_DossierFolders] FOREIGN KEY ([IdDossierFolder])
REFERENCES [dbo].[DossierFolders] ([IdDossierFolder])
GO

ALTER TABLE [dbo].[DossierComments] CHECK CONSTRAINT [FK_DossierComments_DossierFolders]

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
PRINT N'Aggiunta colonna [DSWEnvironment] in  [dbo].[WorkflowRepositories]';
GO

ALTER TABLE  [dbo].[WorkflowRepositories]
ADD [DSWEnvironment] int null
GO

UPDATE [dbo].[WorkflowRepositories] SET [DSWEnvironment] = 0
GO

ALTER TABLE [dbo].[WorkflowRepositories] ALTER COLUMN [DSWEnvironment] int NOT NULL
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
PRINT 'Creazione della tabella [dbo].[DossierWorkflowInstances]'
GO

CREATE TABLE [dbo].[DossierWorkflowInstances](
	[IdDossierWorkflowInstance] [uniqueidentifier] NOT NULL,
	[IdDossier] [uniqueidentifier] NOT NULL,
	[IdWorkflowInstance] [uniqueidentifier] NOT NULL,	
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DossierWorkflowInstances] PRIMARY KEY NONCLUSTERED 
(
	[IdDossierWorkflowInstance] ASC

)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DossierWorkflowInstances]  WITH CHECK ADD  CONSTRAINT [FK_DossierWorkflowInstances_Dossiers] FOREIGN KEY([IdDossier])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO

ALTER TABLE [dbo].[DossierWorkflowInstances] CHECK CONSTRAINT [FK_DossierWorkflowInstances_Dossiers]
GO

ALTER TABLE [dbo].[DossierWorkflowInstances]  WITH CHECK ADD  CONSTRAINT [FK_DossierWorkflowInstances_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DossierWorkflowInstances_IdWorkflowInstance] ON [dbo].[DossierWorkflowInstances]
(       
       [IdWorkflowInstance] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DossierWorkflowInstances] CHECK CONSTRAINT [FK_DossierWorkflowInstances_WorkflowInstances]
GO

CREATE CLUSTERED INDEX [IX_DossierWorkflowInstances_RegistrationDate] ON [dbo].[DossierWorkflowInstances] ([RegistrationDate] ASC)
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
PRINT N'Aggiunta della colonna [IsHandler] alla tabella [dbo].[WorkflowAuthorizations]'

ALTER TABLE [dbo].[WorkflowAuthorizations]
	ADD [IsHandler] [bit] NULL
GO

UPDATE [dbo].[WorkflowAuthorizations]
	SET IsHandler = 0
GO

ALTER TABLE [dbo].[WorkflowAuthorizations]
	ALTER COLUMN [IsHandler] [bit] NOT NULL
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
PRINT N'Aggiunta della colonna [AssignUser] alla tabella [cqrs].[DocumentUnitRoles]'

ALTER TABLE [cqrs].[DocumentUnitRoles]
	ADD [AssignUser] [nvarchar](256) NULL
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
PRINT N'Aggiunta colonna [BirthPlace] in [dbo].[Contact]';
GO

ALTER TABLE [dbo].[Contact]
ADD [BirthPlace] [nvarchar](256) NULL
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
PRINT N'Aggiunta colonna [BirthPlace] in [dbo].[ProtocolContactManual]';
GO

ALTER TABLE [dbo].[ProtocolContactManual]
ADD [BirthPlace] [nvarchar](256) NULL
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
PRINT N'Aggiunta colonna [BirthPlace] in [dbo].[TemplateProtocolContactManual]';
GO

ALTER TABLE [dbo].[TemplateProtocolContactManual]
ADD [BirthPlace] [nvarchar](256) NULL
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
PRINT 'Aggiunta colonna IsMaster nella tabella FascicleRoles'
GO

ALTER TABLE [dbo].[FascicleRoles] ADD IsMaster [bit] NULL
GO

UPDATE [dbo].[FascicleRoles] SET [IsMaster] = 0
GO

ALTER TABLE [dbo].[FascicleRoles] ALTER COLUMN [IsMaster] [bit] NOT NULL
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