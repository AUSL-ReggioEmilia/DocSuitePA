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
PRINT 'Versionamento database alla 8.66'
GO

EXEC dbo.VersioningDatabase N'8.66'
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
PRINT N'Aggiunta colonna [AuthorizationType] in  [dbo].[WorkflowInstanceRoles]';
GO

ALTER TABLE  [dbo].[WorkflowInstanceRoles]
ADD [AuthorizationType] [smallint] NULL
GO

UPDATE [dbo].[WorkflowInstanceRoles] SET [AuthorizationType] = 1
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles] ALTER COLUMN [AuthorizationType] [smallint] NOT NULL
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

PRINT 'Aggiunta colonna [Timestamp] nella tabella [WorkflowInstanceRoles]';
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles] ADD [Timestamp] TIMESTAMP NOT NULL
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
PRINT N'Creazione della colonna [RegistrationUser] nella tabella [WorkflowInstanceRoles]';
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles] ADD [RegistrationUser] [nvarchar](256) NULL;
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

PRINT N'Creazione della colonna [LastChangedDate] nella tabella [WorkflowInstanceRoles]';
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles] ADD [LastChangedDate] [datetimeoffset](7) NULL;
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

PRINT N'Creazione della colonna [LastChangedUser] nella tabella [WorkflowInstanceRoles]';
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles] ADD [LastChangedUser] [nvarchar](256) NULL;
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
PRINT 'Creazione della tabella [dbo].[FascicleWorkflowInstances]'
GO

CREATE TABLE [dbo].[FascicleWorkflowInstances](
	[IdFascicleWorkflowInstance] [uniqueidentifier] NOT NULL,
	[IdFascicle] [uniqueidentifier] NOT NULL,
	[IdWorkflowInstance] [uniqueidentifier] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_FascicleWorkflowInstances] PRIMARY KEY NONCLUSTERED 
(
	[IdFascicleWorkflowInstance] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FascicleWorkflowInstances] ADD  DEFAULT (newsequentialid()) FOR [IdFascicleWorkflowInstance]
GO

ALTER TABLE [dbo].[FascicleWorkflowInstances] ADD  CONSTRAINT [DF_FascicleWorkflowInstances_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]
GO

ALTER TABLE [dbo].[FascicleWorkflowInstances]  WITH CHECK ADD  CONSTRAINT [FK_FascicleWorkflowInstances_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO

ALTER TABLE [dbo].[FascicleWorkflowInstances] CHECK CONSTRAINT [FK_FascicleWorkflowInstances_Fascicles]
GO

ALTER TABLE [dbo].[FascicleWorkflowInstances]  WITH CHECK ADD  CONSTRAINT [FK_FascicleWorkflowInstances_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[FascicleWorkflowInstances] CHECK CONSTRAINT [FK_FascicleWorkflowInstances_WorkflowInstances]
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
PRINT 'Aggiunto constraint per la creazione di un nuovo idDossierWorkflowInstance nella tabella [dbo].[DossierWorkflowInstances]'
GO

ALTER TABLE [dbo].[DossierWorkflowInstances] ADD  DEFAULT (newsequentialid()) FOR [IdDossierWorkflowInstance]
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
PRINT 'Aggiunto constraint sulla colonna RegistrationDate nella tabella [dbo].[DossierWorkflowInstances]'
GO

ALTER TABLE [dbo].[DossierWorkflowInstances] ADD  CONSTRAINT [DF_DossierWorkflowInstances_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]
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
PRINT 'Cancellazione colonna RegistrationUser nella tabella [dbo].[DossierWorkflowInstances]'
GO

ALTER TABLE [dbo].[DossierWorkflowInstances] DROP COLUMN [RegistrationUser]
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
PRINT 'Cancellazione colonna LastChangedUser nella tabella [dbo].[DossierWorkflowInstances]'
GO

ALTER TABLE [dbo].[DossierWorkflowInstances] DROP COLUMN [LastChangedUser]
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
PRINT 'Cancellazione colonna LastChangedDate nella tabella [dbo].[DossierWorkflowInstances]'
GO

ALTER TABLE [dbo].[DossierWorkflowInstances] DROP COLUMN [LastChangedDate]
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

PRINT N'Aggiunte colonne [DossierFolderNode], [DossierFolderPath], [DossierFolderLevel], [DossierFolderParentNode], [FakeInsertId] in [dbo].[DossierFolders]';
GO

ALTER TABLE [dbo].[DossierFolders] ADD [DossierFolderNode] [hierarchyid] not null
GO

ALTER TABLE [dbo].[DossierFolders] ADD [DossierFolderPath] AS ([DossierFolderNode].[ToString]()) PERSISTED
GO

ALTER TABLE [dbo].[DossierFolders] ADD [DossierFolderLevel] AS ([DossierFolderNode].[GetLevel]()) PERSISTED
GO

ALTER TABLE [dbo].[DossierFolders] ADD [DossierFolderParentNode] AS ([DossierFolderNode].[GetAncestor](1)) PERSISTED
GO

ALTER TABLE [dbo].[DossierFolders] ADD [ParentInsertId] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[DossierFolders] ALTER COLUMN [IdDossier] uniqueidentifier NULL
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
PRINT N'Modificata Primary Key in [dbo].[DossierFolders]';
GO

ALTER TABLE [dbo].[DossierFolderRoles] DROP CONSTRAINT [FK_DossierFolderRoles_DossierFolders]
GO

ALTER TABLE [dbo].[DossierComments] DROP CONSTRAINT [FK_DossierComments_DossierFolders]
GO

ALTER TABLE [dbo].[DossierFolders] DROP CONSTRAINT [PK_DossierFolders]
GO

ALTER TABLE [dbo].[DossierFolders] ADD CONSTRAINT [PK_DossierFolders] PRIMARY KEY NONCLUSTERED 
(
	[DossierFolderNode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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

PRINT N'Creato indice [IX_DossierFolders_DossierFolderLevel_DossierFolderNode] in [dbo].[DossierFolders]';
GO

DROP INDEX [IX_DossierFolders_RegistrationDate] ON [dbo].[DossierFolders] WITH ( ONLINE = OFF )
GO

CREATE CLUSTERED INDEX [IX_DossierFolders_DossierFolderLevel_DossierFolderNode] ON [dbo].[DossierFolders]
(DossierFolderLevel, DossierFolderNode) 
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
PRINT  N'Creato indice [IX_DossierFolders_IdDossierFolder] in [dbo].[DossierFolders]'
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DossierFolders_IdDossierFolder] ON [dbo].[DossierFolders]
(
	[IdDossierFolder] ASC
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
PRINT N'Aggiunta colonna [IdDossierFolder] in [dbo].[DossierLogs]';
GO

ALTER TABLE  [dbo].[DossierLogs]
ADD [IdDossierFolder] [uniqueidentifier] NULL
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
PRINT N'Aggiunta relazione tra [dbo].[DossierLogs] e [dbo].[DossierFolders]'
GO

ALTER TABLE [dbo].[DossierLogs] with check ADD CONSTRAINT [FK_DossierLogs_DossierFolders] 
FOREIGN KEY ([IdDossierFolder]) REFERENCES [dbo].[DossierFolders] ([IdDossierFolder])
GO

ALTER TABLE [dbo].[DossierLogs] CHECK CONSTRAINT [FK_DossierLogs_DossierFolders]
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
PRINT N'Aggiunta relazione tra [dbo].[DossierFolderRoles] e [dbo].[DossierFolders]'
GO

ALTER TABLE [dbo].[DossierFolderRoles] with check ADD CONSTRAINT [FK_DossierFolderRoles_DossierFolders] 
FOREIGN KEY ([IdDossierFolder]) REFERENCES [dbo].[DossierFolders] ([IdDossierFolder])
GO

ALTER TABLE [dbo].[DossierFolderRoles] CHECK CONSTRAINT [FK_DossierFolderRoles_DossierFolders]
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
PRINT N'Cambio tipologia colonna [LogType] in [dbo].[DossierLogs]'
GO

ALTER TABLE [dbo].[DossierLogs] ALTER COLUMN [LogType] int not null
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