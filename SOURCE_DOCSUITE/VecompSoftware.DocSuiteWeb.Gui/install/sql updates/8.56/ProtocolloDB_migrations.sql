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
PRINT 'Versionamento database alla 8.56'
GO

EXEC dbo.VersioningDatabase N'8.56'
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
PRINT 'Creazione tabella WorkflowRoleMappings'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowRoleMappings](
	[IdWorkflowRoleMapping] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[MappingTag] [nvarchar](100) NOT NULL,
	[AuthorizationType] [smallint] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL	
 CONSTRAINT [PK_WorkflowRoleMappings] PRIMARY KEY NONCLUSTERED 
(
	[IdWorkflowRoleMapping] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkflowRoleMappings]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowRoleMappings_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([idRole])
GO
ALTER TABLE [dbo].[WorkflowRoleMappings] CHECK CONSTRAINT [FK_WorkflowRoleMappings_Role]
GO

ALTER TABLE [dbo].[WorkflowRoleMappings]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowRoleMappings_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
ALTER TABLE [dbo].[WorkflowRoleMappings] CHECK CONSTRAINT [FK_WorkflowRoleMappings_WorkflowRepositories]
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

PRINT 'Aggiunto clustered indice [IX_WorkflowRoleMappings_RegistrationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowRoleMappings_RegistrationDate]
    ON [dbo].[WorkflowRoleMappings]([RegistrationDate] ASC);
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
PRINT N'Aggiunto nonclustered indice [IX_WorkflowRoleMappings_IdWorkflowRepository_IdRole_MappingTag] in WorkflowRoleMapping';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_WorkflowRoleMappings_IdWorkflowRepository_IdRole_MappingTag] 
	ON [dbo].[WorkflowRoleMappings]([IdWorkflowrepository] ASC, [IdRole] ASC, [MappingTag] ASC)
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
PRINT 'Aggiungo la colonna [Subject] nella tabella [WorkflowActivities]';
GO

ALTER TABLE [dbo].[WorkflowActivities] ADD [Subject] nvarchar(500) NULL
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
PRINT N'Aggiungo la colonna IdWorkflowInstance';
GO

ALTER TABLE [dbo].[Collaboration] ADD [IdWorkflowInstance] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[Collaboration]  WITH CHECK ADD  CONSTRAINT [FK_Collaboration_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[Collaboration] CHECK CONSTRAINT [FK_Collaboration_WorkflowInstances]
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
PRINT N'Modifico la colonna IsActive della tabella ProtocolDraft';
GO

ALTER TABLE [dbo].[ProtocolDraft] ALTER COLUMN [IsActive] [bit] NOT NULL
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolDraft]';
GO

ALTER TABLE [dbo].[ProtocolDraft] ADD [UniqueId] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[ProtocolDraft] ADD CONSTRAINT [DF_ProtocolDraft_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

UPDATE [dbo].[ProtocolDraft] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL
GO

ALTER TABLE [dbo].[ProtocolDraft] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT 'Creato indice univoco [IX_ProtocolDraft_UniqueId] nella tabella [ProtocolDraft]';
GO

CREATE UNIQUE INDEX [IX_ProtocolDraft_UniqueId] ON [dbo].[ProtocolDraft]([UniqueId] ASC);
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

PRINT 'Aggiunta colonna [Timestamp] nella tabella [ProtocolDraft]';
GO

ALTER TABLE [dbo].[ProtocolDraft] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT 'Aggiunto clustered indice [IX_WorkflowRoleMappings_RegistrationDate]';
GO

ALTER TABLE [dbo].[WorkflowInstances] ADD [Json] [nvarchar](max) NULL;

GO

UPDATE [dbo].[WorkflowInstances] SET [Json] = (SELECT [Json] FROM [dbo].[WorkflowRepositories] WHERE [WorkflowInstances].[IdWorkflowRepository] = [WorkflowRepositories].[IdWorkflowRepository]);

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

PRINT 'Aggiunta colonna [UniqueIdDocumentSeriesItem] nella tabella [DocumentSeriesItemRole]';
GO

ALTER TABLE [DocumentSeriesItemRole] ADD [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NULL
GO

UPDATE DSIR SET DSIR.UniqueIdDocumentSeriesItem = DSI.UniqueId
FROM [dbo].[DocumentSeriesItemRole] AS DSIR
INNER JOIN [dbo].[DocumentSeriesItem] DSI ON DSI.Id = DSIR.IdDocumentSeriesItem
GO

ALTER TABLE [DocumentSeriesItemRole] ALTER COLUMN [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NOT NULL
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

PRINT 'Bonifica FOREIGN KEY [FK_FascicleDocumentSeriesItems_DocumentSeriesItem]';
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] DROP CONSTRAINT [FK_FascicleDocumentSeriesItems_DocumentSeriesItem]
GO

UPDATE [DocumentSeriesItem] SET [UniqueId] = newid()
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] ADD [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NOT NULL
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems]  WITH CHECK ADD  CONSTRAINT [FK_FascicleDocumentSeriesItems_DocumentSeriesItem] FOREIGN KEY([UniqueIdDocumentSeriesItem])
REFERENCES [dbo].[DocumentSeriesItem] ([UniqueId])
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] CHECK CONSTRAINT [FK_FascicleDocumentSeriesItems_DocumentSeriesItem]
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
PRINT N'Aggiunto unique index [IX_IdFascicle_IdDocumentSeriesItem_ReferenceType] in [FascicleDocumentSeriesItems]';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FascicleDocumentSeriesItems_IdFascicle_UniqueIdDocumentSeriesItem_ReferenceType] 
	ON [dbo].[FascicleDocumentSeriesItems]([IdFascicle] ASC, [UniqueIdDocumentSeriesItem] ASC, [ReferenceType] ASC)
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

PRINT N'Aggiunto unique index [IX_FascicleUDS_IdFascicle_IdUDS_ReferenceType] in [FascicleUDS]';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FascicleUDS_IdFascicle_IdUDS_ReferenceType] 
	ON [dbo].[FascicleUDS]([IdFascicle] ASC, [IdUDS] ASC, [ReferenceType] ASC)
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

PRINT N'Aggiunto constraint [CHK_UDSRepositories_Status_IdContainer] in [UDSRepositories]';
GO

ALTER TABLE [uds].[UDSRepositories] 
ADD CONSTRAINT [CHK_UDSRepositories_Status_IdContainer] CHECK (NOT(Status = 2 AND IdContainer IS NULL));
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