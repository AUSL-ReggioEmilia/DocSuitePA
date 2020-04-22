
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
PRINT 'Versionamento database alla 8.80'
GO

EXEC dbo.VersioningDatabase N'8.80'
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

PRINT N'ALTER TABLE [dbo].[WorkflowActivities] DROP COLUMN [Duration]';
GO
ALTER TABLE [dbo].[WorkflowActivities] DROP COLUMN [Duration]
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

PRINT N'Aggiungere la colonna IdArchiveChain, Priority, Note nella tabella WorkflowActivities';
GO
ALTER TABLE [dbo].[WorkflowActivities]
ADD [IdArchiveChain] [uniqueidentifier] NULL,
    [Priority] [smallint] NULL,
    [Note] [nvarchar](max) NULL
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

PRINT N'Aggiungere la colonna DocumentUnitReferenced nella tabella WorkflowActivities';
GO
ALTER TABLE [dbo].[WorkflowActivities]
ADD [DocumentUnitReferencedId] [uniqueidentifier] NULL
CONSTRAINT [FK_WorkflowActivities_DocumentUnits] FOREIGN KEY ([DocumentUnitReferencedId])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
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

PRINT N'CREATE TABLE [dbo].[FascicleDocumentUnits]';
GO
create table [dbo].[FascicleDocumentUnits](
    [IdFascicleDocumentUnit] [uniqueidentifier] NOT NULL,​
    [IdFascicle] [uniqueidentifier] NOT NULL,​ 
    [IdFascicleFolder] [uniqueidentifier] NOT NULL,​  
    [IdDocumentUnit] [uniqueidentifier] NOT NULL,​
    [ReferenceType] [smallint] NOT NULL,​
    [RegistrationUser] [nvarchar](256) NOT NULL,​
    [RegistrationDate] [datetimeoffset](7) NOT NULL,​
    [LastChangedUser] [nvarchar](256) NULL,​
    [LastChangedDate] [datetimeoffset](7) NULL,​
    [Timestamp] [timestamp] NOT NULL
    CONSTRAINT [PK_IdFascicleDocumentUnit] PRIMARY KEY NONCLUSTERED
	(
		[IdFascicleDocumentUnit] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
GO
PRINT N'Aggiungere la colonna IdFascicle nella tabella FascicleDocumentUnits'
GO
ALTER TABLE [dbo].[FascicleDocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_FascicleDocumentUnits_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
PRINT N'Aggiungere la colonna IdFascicleFolder nella tabella FascicleDocumentUnits'
GO
ALTER TABLE [dbo].[FascicleDocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_FascicleDocumentUnits_FascicleFolders] FOREIGN KEY([IdFascicleFolder])
REFERENCES [dbo].[FascicleFolders] ([IdFascicleFolder])
GO
PRINT N'Aggiungere la colonna IdDocumentUnit nella tabella FascicleDocumentUnits'
GO
ALTER TABLE [dbo].[FascicleDocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_FascicleDocumentUnits_CQRS.DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES CQRS.DocumentUnits ([IdDocumentUnit])
GO

CREATE CLUSTERED INDEX [IX_FascicleDocumentUnits_RegistrationDate] ON [dbo].[FascicleDocumentUnits]
(
	[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FascicleDocumentUnits_IdFascicle_IdDocumentUnit_IdFascicleFolder] ON [dbo].[FascicleDocumentUnits]
(
	[IdFascicle] ASC,
	[IdDocumentUnit] ASC,
	[IdFascicleFolder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FascicleDocumentUnits_IdFascicle_IdDocumentUnit_ReferenceType] ON [dbo].[FascicleDocumentUnits]
(
	[IdFascicle] ASC,
	[IdDocumentUnit] ASC,
	[ReferenceType] ASC
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
PRINT N'Create index [IX_UDSPECMails_IdUDS]';
GO

CREATE INDEX [IX_UDSPECMails_IdUDS] ON [uds].[UDSPECMails] ([IdUDS])
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
PRINT N'Create index [IX_UDSCollaborations_IdCollaboration]';
GO

CREATE INDEX [IX_UDSCollaborations_IdCollaboration] ON [uds].[UDSCollaborations] ([IdCollaboration])
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
PRINT N'CREATE INDEX [IX_UDSContacts_IdUDS] ON [uds].[UDSContacts] ([IdUDS])';
GO

CREATE INDEX [IX_UDSContacts_IdUDS] ON [uds].[UDSContacts] ([IdUDS])
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
PRINT N'CREATE NONCLUSTERED INDEX [IX_ContainerGroup_FascicleRights]';
GO

CREATE NONCLUSTERED INDEX [IX_ContainerGroup_FascicleRights]
ON [dbo].[ContainerGroup] ([FascicleRights])
INCLUDE ([idContainer],[idGroup])

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
PRINT N'CREATE NONCLUSTERED INDEX [IX_RoleGroup_Role]';
GO

CREATE NONCLUSTERED INDEX [IX_RoleGroup_Role]
ON [dbo].[RoleGroup] ([idRole])
INCLUDE ([ProtocolRights],[ResolutionRights],[DocumentRights],[DocumentSeriesRights],[idGroup])
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
PRINT N'CREATE NONCLUSTERED INDEX [IX_Fascicles_Year_StartDate]';
GO

CREATE NONCLUSTERED INDEX [IX_Fascicles_Year_StartDate]
ON [dbo].[Fascicles] ([Year],[StartDate])
INCLUDE ([IdCategory],[Object],[RegistrationDate],[IdFascicle],[FascicleType],[Title],[IdContainer])
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

