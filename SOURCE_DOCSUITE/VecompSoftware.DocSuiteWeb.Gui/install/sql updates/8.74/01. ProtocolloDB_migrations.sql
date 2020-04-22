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
PRINT 'Versionamento database alla 8.74'
GO

EXEC dbo.VersioningDatabase N'8.74'
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
PRINT N'Aggiunta colonna Type alla tabella [dbo].[ProtocolHighlightUsers]';
GO

ALTER TABLE [dbo].[ProtocolHighlightUsers] ADD [Type] SMALLINT NULL
GO
UPDATE [dbo].[ProtocolHighlightUsers] SET [Type] = 2
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
PRINT 'Creazione della tabella DocumentUnitUsers'
GO

CREATE TABLE [cqrs].[DocumentUnitUsers](
[IdDocumentUnitUser] [uniqueidentifier] NOT NULL,
[IdDocumentUnit] [uniqueidentifier] NOT NULL,
[Account] [nvarchar](256) NOT NULL,
[AuthorizationType][smallint] NOT NULL, 
[RegistrationUser] [nvarchar](256) NOT NULL,
[RegistrationDate] [datetimeoffset](7) NOT NULL,
[LastChangedDate] [datetimeoffset](7) NULL,
[LastChangedUser] [nvarchar](256) NULL,
[Timestamp] [timestamp] NOT NULL,
CONSTRAINT [PK_DocumentUnitUser] PRIMARY KEY NONCLUSTERED  
(
	[IdDocumentUnitUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [cqrs].[DocumentUnitUsers]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitUsers_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO

ALTER TABLE [cqrs].[DocumentUnitUsers] CHECK CONSTRAINT [FK_DocumentUnitUsers_DocumentUnits]
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
PRINT N'Aggiunto clustered indice [IX_DocumentUnitUsers_RegistrationDate] in DocumentUnitUsers';
GO

CREATE CLUSTERED INDEX [IX_DocumentUnits_RegistrationDate]
    ON [cqrs].[DocumentUnitUsers]([RegistrationDate] ASC);
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
PRINT N'Modificata colonna Rights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [Rights] CHAR(20) NOT NULL
GO
UPDATE ContainerGroup SET Rights = SUBSTRING(Rights, 0, 11) + '0000000000'
WHERE LEN(Rights) = 10
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
PRINT N'Modificata colonna ResolutionRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [ResolutionRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET ResolutionRights = SUBSTRING(ResolutionRights, 0, 11) + '0000000000'
WHERE LEN(ResolutionRights) = 10
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
PRINT N'Modificata colonna DocumentRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [DocumentRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET DocumentRights = SUBSTRING(DocumentRights, 0, 11) + '0000000000'
WHERE LEN(DocumentRights) = 10
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
PRINT N'Modificata colonna DocumentSeriesRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [DocumentSeriesRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET DocumentSeriesRights = SUBSTRING(DocumentSeriesRights, 0, 11) + '0000000000'
WHERE LEN(DocumentSeriesRights) = 10
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
PRINT N'Modificata colonna DeskRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [DeskRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET DeskRights = SUBSTRING(DeskRights, 0, 11) + '0000000000'
WHERE LEN(DeskRights) = 10
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
PRINT N'Modificata colonna UDSRights della tabella [dbo].[ContainerGroup]';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [UDSRights] CHAR(20) NULL
GO
UPDATE ContainerGroup SET UDSRights = SUBSTRING(UDSRights, 0, 11) + '0000000000'
WHERE LEN(UDSRights) = 10
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
PRINT N'Modificata colonna [ProtocolRights] della tabella [dbo].[CategoryGroup]';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [ProtocolRights] CHAR(20) NOT NULL
GO
UPDATE CategoryGroup SET ProtocolRights = SUBSTRING(ProtocolRights, 0, 6) + '000000000000000'
WHERE LEN(ProtocolRights) = 5
GO
UPDATE CategoryGroup SET ProtocolRights = SUBSTRING(ProtocolRights, 0, 11) + '0000000000'
WHERE LEN(ProtocolRights) = 10
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
PRINT N'Modificata colonna [ResolutionRights] della tabella [dbo].[CategoryGroup]';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [ResolutionRights] CHAR(20) NULL
GO
UPDATE CategoryGroup SET ResolutionRights = SUBSTRING(ResolutionRights, 0, 6) + '000000000000000'
WHERE LEN(ResolutionRights) = 5
GO
UPDATE CategoryGroup SET ResolutionRights = SUBSTRING(ResolutionRights, 0, 11) + '0000000000'
WHERE LEN(ResolutionRights) = 10
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
PRINT N'Modificata colonna [DocumentRights] della tabella [dbo].[CategoryGroup]';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [DocumentRights] CHAR(20) NULL
GO
UPDATE CategoryGroup SET DocumentRights = SUBSTRING(DocumentRights, 0, 6) + '000000000000000'
WHERE LEN(DocumentRights) = 5
GO
UPDATE CategoryGroup SET DocumentRights = SUBSTRING(DocumentRights, 0, 11) + '0000000000'
WHERE LEN(DocumentRights) = 10
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
PRINT N'Modificata colonna [ProtocolRights] della tabella [dbo].[RoleGroup]';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [ProtocolRights] CHAR(20) NULL
GO
UPDATE RoleGroup SET ProtocolRights = SUBSTRING(ProtocolRights, 0, 11) + '0000000000'
WHERE LEN(ProtocolRights) = 10
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
PRINT N'Modificata colonna [ResolutionRights] della tabella [dbo].[RoleGroup]';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [ResolutionRights] CHAR(20) NULL
GO
UPDATE RoleGroup SET ResolutionRights = SUBSTRING(ResolutionRights, 0, 11) + '0000000000'
WHERE LEN(ResolutionRights) = 10
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
PRINT N'Modificata colonna [DocumentRights] della tabella [dbo].[RoleGroup]';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [DocumentRights] CHAR(20) NULL
GO
UPDATE RoleGroup SET DocumentRights = SUBSTRING(DocumentRights, 0, 11) + '0000000000'
WHERE LEN(DocumentRights) = 10
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
PRINT N'Modificata colonna [DocumentSeriesRights] della tabella [dbo].[RoleGroup]';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [DocumentSeriesRights] CHAR(20) NULL
GO
UPDATE RoleGroup SET DocumentSeriesRights = SUBSTRING(DocumentSeriesRights, 0, 11) + '0000000000'
WHERE LEN(DocumentSeriesRights) = 10
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
PRINT N'Modificata colonna [Description] della tabella [dbo].[PrivacyLevels]';
GO

ALTER TABLE [dbo].[PrivacyLevels] ALTER COLUMN [Description] nvarchar(MAX) not null
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
PRINT N'Aggiunta colonna [Colour] della tabella [dbo].[PrivacyLevels]';
GO

ALTER TABLE [dbo].[PrivacyLevels] ADD [Colour] nvarchar(7) null
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
PRINT N'Creata tabella [dbo].[FascicleFolders]'
GO

CREATE TABLE [dbo].[FascicleFolders](
	[IdFascicleFolder] [uniqueidentifier] NOT NULL,
	[IdFascicle] [uniqueidentifier] NULL,
	[IdCategory] [smallint] NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Status] [smallint] NOT NULL,
	[Typology] [smallint] NOT NULL,  
	[FascicleFolderNode] [hierarchyid] not null,
	[FascicleFolderPath] AS ([FascicleFolderNode].[ToString]()) PERSISTED,
	[FascicleFolderLevel] AS ([FascicleFolderNode].[GetLevel]()) PERSISTED,
	[FascicleFolderParentNode] AS ([FascicleFolderNode].[GetAncestor](1)) PERSISTED,
	[ParentInsertId] [uniqueidentifier] NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
CONSTRAINT [PK_FascicleFolders] PRIMARY KEY NONCLUSTERED 
(
	[FascicleFolderNode] ASC

)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FascicleFolders]  WITH CHECK ADD  CONSTRAINT [FK_FascicleFolders_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO

ALTER TABLE [dbo].[FascicleFolders] CHECK CONSTRAINT [FK_FascicleFolders_Fascicles]
GO

ALTER TABLE [dbo].[FascicleFolders]  WITH CHECK ADD  CONSTRAINT [FK_FascicleFolders_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([idCategory])
GO

ALTER TABLE [dbo].[FascicleFolders] CHECK CONSTRAINT [FK_FascicleFolders_Category]
GO

CREATE CLUSTERED INDEX [IX_FascicleFolders_FascicleFolderLevel_FascicleFolderNode] ON [dbo].[FascicleFolders]
(FascicleFolderLevel, FascicleFolderNode) 
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FascicleFolders_IdFascicleFolder] ON [dbo].[FascicleFolders]
(
	[IdFascicleFolder] ASC
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
PRINT N'Aggiunta colonna [IdFascicleFolder] nella tabella [dbo].[FascicleProtocols]'
GO

ALTER TABLE [dbo].[FascicleProtocols] ADD [IdFascicleFolder] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[FascicleProtocols] WITH CHECK ADD CONSTRAINT [FK_FascicleProtocols_FascicleFolders] FOREIGN KEY([IdFascicleFolder])
REFERENCES [dbo].[FascicleFolders] ([IdFascicleFolder])
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
PRINT N'Aggiunta colonna [IdFascicleFolder] nella tabella [dbo].[FascicleResolutions]'
GO

ALTER TABLE [dbo].[FascicleResolutions] ADD [IdFascicleFolder] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[FascicleResolutions] WITH CHECK ADD CONSTRAINT [FK_FascicleResolutions_FascicleFolders] FOREIGN KEY([IdFascicleFolder])
REFERENCES [dbo].[FascicleFolders] ([IdFascicleFolder])
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
PRINT N'Aggiunta colonna [IdFascicleFolder] nella tabella [dbo].[FascicleUDS]'
GO

ALTER TABLE [dbo].[FascicleUDS] ADD [IdFascicleFolder] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[FascicleUDS] WITH CHECK ADD CONSTRAINT [FK_FascicleUDS_FascicleFolders] FOREIGN KEY([IdFascicleFolder])
REFERENCES [dbo].[FascicleFolders] ([IdFascicleFolder])
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
PRINT N'Aggiunta colonna [IdFascicleFolder] nella tabella [dbo].[FascicleDocumentSeriesItems]'
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] ADD [IdFascicleFolder] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] WITH CHECK ADD CONSTRAINT [FK_FascicleDocumentSeriesItems_FascicleFolders] FOREIGN KEY([IdFascicleFolder])
REFERENCES [dbo].[FascicleFolders] ([IdFascicleFolder])
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
PRINT N'Aggiunta colonna [IdFascicleFolder] nella tabella [dbo].[FascicleDocuments]'
GO

ALTER TABLE [dbo].[FascicleDocuments] ADD [IdFascicleFolder] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[FascicleDocuments] WITH CHECK ADD CONSTRAINT [FK_FascicleDocuments_FascicleFolders] FOREIGN KEY([IdFascicleFolder])
REFERENCES [dbo].[FascicleFolders] ([IdFascicleFolder])
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
PRINT N'Inserimento nodo radice nella [dbo].[FascicleFolders]'
GO

INSERT INTO [dbo].[FascicleFolders]
(IdFascicleFolder, IdFascicle, IdCategory, Name, Status, Typology, RegistrationUser, RegistrationDate, FascicleFolderNode)
VALUES
(newid(), null, null, 'DSW ROOT', 1, 1,'anonymous_api', sysdatetimeoffset(), 0x)


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
PRINT N'Modificata colonna [LogDescription] della tabella [dbo].[TableLog]';
GO

ALTER TABLE [dbo].[TableLog] ALTER COLUMN [LogDescription] NVARCHAR(MAX) NOT NULL
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
PRINT N'Creata colonna [Hash] della tabella [dbo].[TableLog]';
GO

ALTER TABLE [dbo].[TableLog] ADD [Hash] [nchar](64) null
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
PRINT N'Creata colonna [Hash] della tabella [dbo].[DocumentSeriesItemLog]';
GO

ALTER TABLE [dbo].[DocumentSeriesItemLog] ADD [Hash] [nchar](64) null
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
PRINT N'Creata colonna [Hash] della tabella [dbo].[DossierLogs]';
GO

ALTER TABLE [dbo].[DossierLogs] ADD [Hash] [nchar](64) null
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
PRINT N'Creata colonna [Hash] della tabella [dbo].[FascicleLogs]';
GO

ALTER TABLE [dbo].[FascicleLogs] ADD [Hash] [nchar](64) null
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
PRINT N'Creata colonna [Hash] della tabella [dbo].[PecMailLog]';
GO

ALTER TABLE [dbo].[PECMailLog] ADD [Hash] [nchar](64) null
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
PRINT N'Creata colonna [Hash] della tabella [dbo].[ProtocolLog]';
GO

ALTER TABLE [dbo].[ProtocolLog] ADD [Hash] [nchar](64) null
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
PRINT N'Creata colonna [Hash] della tabella [dbo].[uds.UDSLogs]';
GO

ALTER TABLE uds.UDSLogs ADD [Hash] [nchar](64) null
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
PRINT N'Creato schema [conservation]';
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'conservation')
BEGIN
	EXEC('CREATE SCHEMA conservation');
END
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
PRINT N'Creata tabella [conservation].[Conservations]';
GO

CREATE TABLE [conservation].[Conservations](
	[IdConservation] [uniqueidentifier] NOT NULL,
	[EntityType] [nvarchar](256) NOT NULL,
	[Status] [smallint] NOT NULL,
	[Message] [nvarchar](MAX) NULL,
	[Type] [smallint] NOT NULL,
	[SendDate] [datetimeoffset](7) NULL,
	[Uri] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_Conservation] PRIMARY KEY NONCLUSTERED 
(
	[IdConservation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
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
PRINT N'Creato indice [IX_Conservation_RegistationDate] nella tabella [conservation].[Conservations]';
GO

CREATE CLUSTERED INDEX [IX_Conservation_RegistationDate]
	ON [conservation].[Conservations]([RegistrationDate] ASC);
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
PRINT N'ALTER TABLE PECMailLog ADD UniqueId uniqueidentifier not null default(newid())';
GO

ALTER TABLE PECMailLog ADD UniqueId uniqueidentifier not null default(newid());

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
PRINT N'Creato indice [IX_FascicleLog_LogDate] nella tabella [dbo].[FascicleLogs]';
GO

CREATE CLUSTERED INDEX [IX_FascicleLog_LogDate]
	ON [dbo].[FascicleLogs]([LogDate] ASC);
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
PRINT N'Creato indice [IX_UDSLog_LogDate] nella tabella [uds].[UDSLogs]';
GO

CREATE CLUSTERED INDEX [IX_UDSLog_LogDate]
	ON [uds].[UDSLogs]([LogDate] ASC);
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