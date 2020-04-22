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
PRINT 'Versionamento database alla 8.58'
GO

EXEC dbo.VersioningDatabase N'8.58'
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
PRINT 'ALTER TABLE PECMail ALTER COLUMN MailSubject nvarchar(4000) null';
 GO
 
 ALTER TABLE PECMail ALTER COLUMN MailSubject nvarchar(4000) null
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
 PRINT 'ALTER TABLE PECMail ALTER COLUMN MailBody nvarchar(max) null';
 GO
 
 ALTER TABLE PECMail ALTER COLUMN MailBody nvarchar(max) null
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
PRINT 'Aggiungo la colonna [IdInternalActivity] nella tabella [WorkflowRoleMappings]';
GO

ALTER TABLE [dbo].[WorkflowRoleMappings] ADD [IdInternalActivity] nvarchar(256) NULL
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
PRINT 'Aggiungo la colonna [AccountName] nella tabella [WorkflowRoleMappings]';
GO

ALTER TABLE [dbo].[WorkflowRoleMappings] ADD [AccountName] nvarchar(256) NULL
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
PRINT 'Aggiungo la colonna [CustomActivities] nella tabella [WorkflowRepositories]';
GO

ALTER TABLE [dbo].[WorkflowRepositories] ADD [CustomActivities] nvarchar(max) NULL
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
PRINT 'Elimino indice [IX_WorkflowRoleMappings_IdWorkflowRepository_IdRole_MappingTag] nella tabella [WorkflowRoleMappings]';
GO

DROP INDEX [IX_WorkflowRoleMappings_IdWorkflowRepository_IdRole_MappingTag] ON [dbo].[WorkflowRoleMappings]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_WorkflowRoleMappings_IdWorkflowRepository_IdRole_MappingTag_IdInternalActivity] ON [dbo].[WorkflowRoleMappings]
(
	[IdWorkflowRepository] ASC,
	[IdRole] ASC,
	[MappingTag] ASC,
	[IdInternalActivity] ASC
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
PRINT 'Modifico la colonna [Xaml] nella tabella [WorkflowRepositories]';
GO

ALTER TABLE [dbo].[WorkflowRepositories] ALTER COLUMN [Xaml] [xml] NULL
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
PRINT 'Modifico la colonna [Json] nella tabella [WorkflowRepositories]';
GO

ALTER TABLE [dbo].[WorkflowRepositories] ALTER COLUMN [Json] [nvarchar](max) NULL
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
PRINT 'Modifico la colonna [IdRole] nella tabella [WorkflowRoleMappings]';
GO

ALTER TABLE [dbo].[WorkflowRoleMappings] ALTER COLUMN [IdRole] [smallint] NULL
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
PRINT 'Aggiunto constraint [CHK_WorkflowRepositories_Xaml_Json] in [WorkflowRepositories]';
GO

ALTER TABLE [dbo].[WorkflowRepositories] 
ADD CONSTRAINT [CHK_WorkflowRepositories_Xaml_Json] CHECK (NOT ((Xaml IS NULL) AND (Json IS NULL OR Json = '')) AND NOT ((Xaml IS NOT NULL) AND (Json IS NOT NULL OR Json <> '')));
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

PRINT 'Modifico la colonna [Program] nella tabella [ProtocolLog]';
GO

ALTER TABLE [dbo].[ProtocolLog] ALTER COLUMN [Program] nvarchar(15) not null
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
PRINT 'Creazione tabella [WorkflowInstanceLogs]'
GO

CREATE TABLE [dbo].[WorkflowInstanceLogs](
	[IdWorkflowInstanceLog] [uniqueidentifier] NOT NULL,
	[IdWorkflowInstance] [uniqueidentifier] NOT NULL,
	[LogDate] [datetimeoffset](7) NOT NULL,
	[SystemComputer] [nvarchar](256) NOT NULL,
	[SystemUser] [nvarchar](256) NOT NULL,
	[LogType] [smallint] NOT NULL,
	[LogDescription] [nvarchar](max) NOT NULL,
	[Severity] [smallint] NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_WorkflowInstanceLogs] PRIMARY KEY NONCLUSTERED 
(
	[IdWorkflowInstanceLog] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO

ALTER TABLE [dbo].[WorkflowInstanceLogs]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowInstanceLogs_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[WorkflowInstanceLogs] CHECK CONSTRAINT [FK_WorkflowInstanceLogs_WorkflowInstances]
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

PRINT 'Aggiunto clustered indice [IX_WorkflowInstanceLogs_LogDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowInstanceLogs_LogDate]
    ON [dbo].[WorkflowInstanceLogs]([LogDate] ASC);
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
PRINT 'Creazione tabella [WorkflowInstanceRoles]'
GO

CREATE TABLE [dbo].[WorkflowInstanceRoles](
	[IdWorkflowInstanceRole] [uniqueidentifier] NOT NULL,
	[IdWorkflowInstance] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,

 CONSTRAINT [PK_WorkflowInstanceRoles] PRIMARY KEY NONCLUSTERED 
(
	[IdWorkflowInstanceRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkflowInstanceRoles] ADD  CONSTRAINT [DF_WorkflowInstanceRoles_IdWorkflowInstanceRole]  DEFAULT (newsequentialid()) FOR [IdWorkflowInstanceRole]
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles] ADD  CONSTRAINT [DF_WorkflowInstanceRoles_RegistrationDate]  DEFAULT (getdate()) FOR [RegistrationDate]
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowInstanceRoles_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([idRole])
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles] CHECK CONSTRAINT [FK_WorkflowInstanceRoles_Role]
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowInstanceRoles_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[WorkflowInstanceRoles] CHECK CONSTRAINT [FK_WorkflowInstanceRoles_WorkflowInstances]
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
PRINT 'Aggiunto clustered indice [IX_WorkflowInstanceRoles_RegistrationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowInstanceRoles_RegistrationDate]
    ON [dbo].[WorkflowInstanceRoles]([RegistrationDate] ASC);
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
PRINT 'Aggiunta colonna [ContactId] nella tabella [PosteOnlineAccount]';
GO

ALTER TABLE [dbo].[PosteOnlineAccount] ADD [ContactId] [int] null
GO

ALTER TABLE [dbo].[PosteOnlineAccount] WITH CHECK ADD CONSTRAINT [FK_PosteOnlineAccount_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([Incremental])
GO

ALTER TABLE[dbo].[PosteOnlineAccount] CHECK CONSTRAINT [FK_PosteOnlineAccount_Contact]
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