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
PRINT 'Versionamento database alla 8.51'
GO

EXEC dbo.VersioningDatabase N'8.51'
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
PRINT N'Aggiunge la colonna UriSharepoint nella tabella Protocol';
GO
ALTER TABLE [dbo].[Role]  ADD [UriSharepoint] nvarchar(256)
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
--#############################################################################
PRINT N'Aggiunge la tabella CollaborationAggregate nel database di protocollo';
GO
CREATE TABLE [dbo].[CollaborationAggregate](
	[IdCollaborationAggregate] [uniqueidentifier] NOT NULL,
	[idCollaborationFather] [numeric](10, 0) NOT NULL,
	[idCollaborationChild] [numeric](10, 0) NOT NULL,
	[CollaborationDocumentType] [char](1) NOT NULL,
 CONSTRAINT [PK_CollaborationAggregate] PRIMARY KEY CLUSTERED 
(
	[IdCollaborationAggregate] ASC
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
PRINT N'Aggiunge la tabella FK [idCollaborationFather] nella tebella [CollaborationAggregate] verso [Collaboration] ';
GO
ALTER TABLE [dbo].[CollaborationAggregate]  WITH CHECK ADD  CONSTRAINT [FK_CollaborationAggregate_idCollaborationFather] FOREIGN KEY([idCollaborationFather])
REFERENCES [dbo].[Collaboration] ([IdCollaboration])
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
PRINT N'Aggiunge la tabella FK [idCollaborationFather] nella tebella [CollaborationAggregate] verso [Collaboration] ';
GO
ALTER TABLE [dbo].[CollaborationAggregate]  WITH CHECK ADD  CONSTRAINT [FK_CollaborationAggregate_idCollaborationChild] FOREIGN KEY([idCollaborationChild])
REFERENCES [dbo].[Collaboration] ([IdCollaboration])
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
PRINT N'ALTER TABLE [dbo].[CollaborationLog] ALTER COLUMN [SystemUser] nvarchar(256) NULL';
GO
	ALTER TABLE [dbo].[CollaborationLog] ALTER COLUMN [SystemUser] nvarchar(256) NULL
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
--############################################################################
PRINT N'ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [RegistrationUser] nvarchar(256) NULL';
GO
DROP INDEX [IX_Collaboration_DocumentType_IdStatus_RegistrationUser] ON [dbo].[Collaboration]
GO
	ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [RegistrationUser] nvarchar(256) NULL
GO
CREATE NONCLUSTERED INDEX [IX_Collaboration_DocumentType_IdStatus_RegistrationUser] ON [dbo].[Collaboration]
(
	[DocumentType] ASC,
	[IdStatus] ASC,
	[RegistrationUser] ASC
)
INCLUDE ( 	[IdCollaboration]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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
PRINT N'ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [LastChangedUser] nvarchar(256) NULL';
GO
	ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [LastChangedUser] nvarchar(256) NULL
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
PRINT N'ALTER TABLE [dbo].[CollaborationVersioning] ALTER COLUMN [CheckOutUser] nvarchar(256) NULL';
GO
	ALTER TABLE [dbo].[CollaborationVersioning] ALTER COLUMN [CheckOutUser] nvarchar(256) NULL
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
PRINT N'ALTER TABLE [dbo].[CollaborationVersioning] ALTER COLUMN [RegistrationUser] nvarchar(256) NOT NULL';
GO
	ALTER TABLE [dbo].[CollaborationVersioning] ALTER COLUMN [RegistrationUser] nvarchar(256) NOT NULL
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
PRINT N'ALTER TABLE [dbo].[CollaborationVersioning] ALTER COLUMN [RegistrationName] nvarchar(256) NOT NULL';
GO
	ALTER TABLE [dbo].[CollaborationVersioning] ALTER COLUMN [RegistrationName] nvarchar(256) NOT NULL
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
PRINT N'ALTER TABLE [dbo].[ProtocolDraft]  ALTER COLUMN [Data] ntext';
GO
ALTER TABLE [dbo].[PECMailAttachment] ALTER COLUMN [AttachmentStream] text null
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
PRINT N'ALTER TABLE [dbo].[ProtocolDraft]  ALTER COLUMN [Data] ntext';
GO
ALTER TABLE [dbo].[ProtocolDraft]  ALTER COLUMN [Data] ntext
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
PRINT 'Creazione tabella ContainerProperties';
GO
CREATE TABLE [dbo].[ContainerProperties](
	[IdContainerProperty] [uniqueidentifier] NOT NULL,
	[idContainer] [smallint] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[ContainerType] [smallint] NOT NULL,
	[ValueInt] [bigint] NULL,
	[ValueDate] [datetime] NULL,
	[ValueDouble] [float] NULL,
	[ValueBoolean] [bit] NULL,
	[ValueGuid] [uniqueidentifier] NULL,
	[ValueString] [nvarchar](max) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL
 CONSTRAINT [PK_ContainerProperties] PRIMARY KEY CLUSTERED 
(
	[IdContainerProperty] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[ContainerProperties]  WITH CHECK ADD  CONSTRAINT [FK_ContainerProperties_Container] FOREIGN KEY([idContainer])
REFERENCES [dbo].[Container] ([idContainer])
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ContainerProperties_UniqueIndex] ON [dbo].[ContainerProperties]
(
	[idContainer] ASC,
	[Name] ASC
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
PRINT 'ALTER TABLE [dbo].[SecurityGroups] ADD AllUsers [bit] NOT NULL default (0)';

GO
ALTER TABLE [dbo].[SecurityGroups] ADD AllUsers [bit] NOT NULL default (0)
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

PRINT 'DROP Primary Key CategoryGroup'
GO

declare @table_name nvarchar(256)
declare @Command  nvarchar(1000)

set @table_name = N'CategoryGroup'

select TOP 1 @Command = 'ALTER TABLE ' + @table_name + ' DROP CONSTRAINT ' + i.name 
 FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
WHERE   i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID) = @table_name

PRINT @Command

execute (@Command)
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
PRINT N'Starting rebuilding table [dbo].[CategoryGroup]...';
GO

CREATE CLUSTERED INDEX [IX_CategoryGroup_RegistationDate]
    ON [dbo].[CategoryGroup]([RegistrationDate] ASC);
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
PRINT N'Starting rebuilding table [dbo].[CategoryGroup]...';
GO

ALTER TABLE [dbo].[CategoryGroup] ADD [IdCategoryGroup] UNIQUEIDENTIFIER NULL
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

PRINT N'Creating [dbo].[FK_CategoryGroup_Category]...';
GO

ALTER TABLE [dbo].[CategoryGroup]  WITH CHECK ADD  CONSTRAINT [FK_CategoryGroup_Category] FOREIGN KEY([idCategory])
REFERENCES [dbo].[Category] ([idCategory])
GO

ALTER TABLE [dbo].[CategoryGroup] CHECK CONSTRAINT [FK_CategoryGroup_Category]
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

PRINT N'Creating [dbo].[CategoryGroup].[IX_CategoryGroup_GroupName_idCategory]...';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CategoryGroup_GroupName_idCategory]
    ON [dbo].[CategoryGroup]([GroupName] ASC, [idCategory] ASC);
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

PRINT 'DROP Primary Key ContainerGroup'
GO

declare @table_name nvarchar(256)
declare @Command  nvarchar(1000)

set @table_name = N'ContainerGroup'

select TOP 1 @Command = 'ALTER TABLE ' + @table_name + ' DROP CONSTRAINT ' + i.name 
 FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
WHERE   i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID) = @table_name

PRINT @Command

execute (@Command)
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
PRINT N'ALTER TABLE [dbo].[ContainerGroup] ADD [IdContainerGroup] UNIQUEIDENTIFIER NULL';
GO

ALTER TABLE [dbo].[ContainerGroup] ADD [IdContainerGroup] UNIQUEIDENTIFIER NULL
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

PRINT N'Starting rebuilding table [dbo].[ContainerGroup]...';
GO

CREATE CLUSTERED INDEX [IX_ContainerGroup_RegistationDate]
    ON [dbo].[ContainerGroup]([RegistrationDate] ASC);
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

PRINT N'Creating [dbo].[FK_ContainerGroup_Container]...';
GO

ALTER TABLE [dbo].[ContainerGroup]  WITH CHECK ADD  CONSTRAINT [FK_ContainerGroup_Container] FOREIGN KEY([idContainer])
REFERENCES [dbo].[Container] ([idContainer])
GO

ALTER TABLE [dbo].[ContainerGroup] CHECK CONSTRAINT [FK_ContainerGroup_Container]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
BEGIN
INSERT INTO #tmpErrors (Error) VALUES (1);
BEGIN TRANSACTION;
END
GO

--#############################################################################

PRINT N'Creating [dbo].[ContainerGroup].[IX_ContainerGroup_GroupName_idContainer]...';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ContainerGroup_GroupName_idContainer]
    ON [dbo].[ContainerGroup]([GroupName] ASC, [idContainer] ASC);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
BEGIN
INSERT INTO #tmpErrors (Error) VALUES (1);
BEGIN TRANSACTION;
END
GO
--#############################################################################

PRINT 'DROP Primary Key RoleGroup'
GO

declare @table_name nvarchar(256)
declare @Command  nvarchar(1000)

set @table_name = N'RoleGroup'

select TOP 1 @Command = 'ALTER TABLE ' + @table_name + ' DROP CONSTRAINT ' + i.name 
 FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
WHERE   i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID) = @table_name

PRINT @Command

execute (@Command)
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

PRINT N'ALTER TABLE [dbo].[RoleGroup] ADD [IdRoleGroup] UNIQUEIDENTIFIER NULL';
GO

ALTER TABLE [dbo].[RoleGroup] ADD [IdRoleGroup] UNIQUEIDENTIFIER NULL
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

PRINT N'Starting rebuilding table [dbo].[RoleGroup]...';
GO

CREATE CLUSTERED INDEX [IX_RoleGroup_RegistationDate]
    ON [dbo].[RoleGroup]([RegistrationDate] ASC);
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

PRINT N'Creating [dbo].[FK_RoleGroup_Role]...';
GO

ALTER TABLE [dbo].[RoleGroup]  WITH CHECK ADD  CONSTRAINT [FK_RoleGroup_Role] FOREIGN KEY([idRole])
REFERENCES [dbo].[Role] ([idRole])
GO

ALTER TABLE [dbo].[RoleGroup] CHECK CONSTRAINT [FK_RoleGroup_Role]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
BEGIN
INSERT INTO #tmpErrors (Error) VALUES (1);
BEGIN TRANSACTION;
END
GO

--#############################################################################

PRINT N'Creating [dbo].[RoleGroup].[IX_RoleGroup_GroupName_idRole]...';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RoleGroup_GroupName_idRole]
    ON [dbo].[RoleGroup]([GroupName] ASC, [idRole] ASC);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
BEGIN
INSERT INTO #tmpErrors (Error) VALUES (1);
BEGIN TRANSACTION;
END
GO
--#############################################################################

PRINT 'DROP Primary Key CollaborationVersioning'
GO

declare @table_name nvarchar(256)
declare @Command  nvarchar(1000)

set @table_name = N'CollaborationVersioning'

select TOP 1 @Command = 'ALTER TABLE ' + @table_name + ' DROP CONSTRAINT ' + i.name 
 FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
WHERE   i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID) = @table_name

PRINT @Command

execute (@Command)

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
PRINT N'ALTER TABLE [dbo].[CollaborationVersioning] ADD [IdCollaborationVersioning] UNIQUEIDENTIFIER  DEFAULT NEWSEQUENTIALID() NOT NULL';
GO

ALTER TABLE [dbo].[CollaborationVersioning] ADD [IdCollaborationVersioning] UNIQUEIDENTIFIER  DEFAULT NEWSEQUENTIALID() NOT NULL
GO
ALTER TABLE [dbo].[CollaborationVersioning] ADD CONSTRAINT PK_CollaborationVersioning PRIMARY KEY NONCLUSTERED ([IdCollaborationVersioning] ASC)
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
PRINT N'ALTER COLUMN CheckOutDate';
GO

ALTER TABLE [CollaborationVersioning] ALTER COLUMN [CheckOutDate] DATETIMEOFFSET (7) NULL
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
PRINT N'ALTER COLUMN RegistrationDate';
GO

ALTER TABLE [CollaborationVersioning] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET (7) NOT NULL
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
PRINT N'ALTER COLUMN IsActive';
GO

ALTER TABLE [CollaborationVersioning] ALTER COLUMN [IsActive] bit NULL
GO
EXEC sp_rename 'dbo.CollaborationVersioning.isActive', 'IsActive', 'COLUMN';
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
PRINT N'ALTER COLUMN CheckedOut';
GO

ALTER TABLE [CollaborationVersioning] ALTER COLUMN [CheckedOut] bit NULL
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

PRINT N'Starting rebuilding table [dbo].[CollaborationVersioning]...';
GO

CREATE CLUSTERED INDEX [IX_CollaborationVersioning_RegistationDate]
    ON [dbo].[CollaborationVersioning]([RegistrationDate] ASC);
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

PRINT N'Creating [dbo].[CollaborationVersioning].[IX_CollaborationVersioning_IdCollaboration_CollaborationIncremental_Incremental]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CollaborationVersioning_IdCollaboration_CollaborationIncremental_Incremental]
    ON [dbo].[CollaborationVersioning]([IdCollaboration] ASC, [CollaborationIncremental] ASC, [Incremental] ASC);


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END

GO

--#############################################################################

PRINT N'Creating [dbo].[FK_CollaborationVersioning_Collaboration]...';


GO
ALTER TABLE [dbo].[CollaborationVersioning] WITH CHECK
    ADD CONSTRAINT [FK_CollaborationVersioning_Collaboration] FOREIGN KEY ([IdCollaboration]) REFERENCES [dbo].[Collaboration] ([IdCollaboration]);
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
		PRINT 'Verificare Se in CollaborationVersioning siano presenti Id di Collaborazioni non presenti in dbo.Collaboration'
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END

GO
--#############################################################################

PRINT 'Creazione colonna UniqueId nullable nella tabella Category'
GO

ALTER TABLE [dbo].[Category] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella Contact'
GO

ALTER TABLE [dbo].[Contact] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella ContactPlaceName'
GO

ALTER TABLE [dbo].[ContactPlaceName] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella ContactTitle'
GO

ALTER TABLE [dbo].[ContactTitle] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella Container'
GO

ALTER TABLE [dbo].[Container] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella Location'
GO

ALTER TABLE [dbo].[Location] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella Role'
GO

ALTER TABLE [dbo].[Role] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella SecurityGroups'
GO

ALTER TABLE [dbo].[SecurityGroups] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella SecurityUsers'
GO

ALTER TABLE [dbo].[SecurityUsers] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'ALTER IdCollaborationSign CollaborationSigns'
GO
ALTER TABLE [dbo].[CollaborationSigns] DROP CONSTRAINT [PK_CollaborationSigns]
GO
EXEC sp_rename 'dbo.CollaborationSigns.IdCollaborationSigns', 'IdCollaborationSign', 'COLUMN';
GO
ALTER TABLE [dbo].[CollaborationSigns] ADD  CONSTRAINT [PK_CollaborationSigns] PRIMARY KEY NONCLUSTERED 
(
	[IdCollaborationSign] ASC
)
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

PRINT 'ALTER IsActive CollaborationSigns'
GO

ALTER TABLE [CollaborationSigns] ALTER COLUMN IsActive bit null
GO
EXEC sp_rename 'dbo.CollaborationSigns.isActive', 'IsActive', 'COLUMN';
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

PRINT 'ALTER IsRequired CollaborationSigns'
GO

ALTER TABLE [CollaborationSigns] ALTER COLUMN IsRequired bit null
GO
EXEC sp_rename 'dbo.CollaborationSigns.isRequired', 'IsRequired', 'COLUMN';
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

PRINT 'ALTER IdCollaborationUser CollaborationUsers'
GO
ALTER TABLE [dbo].[CollaborationUsers] DROP CONSTRAINT [PK_CollaborationUsers]
GO
EXEC sp_rename 'dbo.CollaborationUsers.IdCollaborationUsers', 'IdCollaborationUser', 'COLUMN';
GO
ALTER TABLE [dbo].[CollaborationUsers] ADD  CONSTRAINT [PK_CollaborationUsers] PRIMARY KEY NONCLUSTERED 
(
	[IdCollaborationUser] ASC
)
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

PRINT 'ALTER DestinationFirst CollaborationUsers'
GO

ALTER TABLE [CollaborationUsers] ALTER COLUMN DestinationFirst bit null
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

PRINT 'Cancellazione indice [XAKName] dalla tabella Role'

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Role]') AND name = N'XAKName')
	DROP INDEX [XAKName] ON [dbo].[Role] WITH ( ONLINE = OFF )
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

PRINT 'Cancella la colonna [RegistrationUser] nella tabella [CollaborationVersioning]';
GO
ALTER TABLE [dbo].[CollaborationVersioning] DROP COLUMN [RegistrationUser]
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
PRINT 'Rinomino la colonna [RegistrationName] nella tabella [CollaborationVersioning]';
GO
EXECUTE sp_rename '[dbo].[CollaborationVersioning].[RegistrationName]', 'RegistrationUser', 'COLUMN';
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

PRINT 'Creazione ForeignKey nella tabella MessageAttachment'
GO

ALTER TABLE [dbo].[MessageAttachment] WITH CHECK
  ADD CONSTRAINT [FK_MessageAttachment_Message] 
	FOREIGN KEY ([IDMessage]) 
	REFERENCES [dbo].[Message] ([IDMessage]);
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

PRINT 'Creazione ForeignKey nella tabella MessageContact'
GO

ALTER TABLE [dbo].[MessageContact] WITH CHECK
  ADD CONSTRAINT [FK_MessageContact_Message] 
	FOREIGN KEY ([IDMessage]) 
	REFERENCES [dbo].[Message] ([IDMessage]);
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

PRINT 'Creazione ForeignKey nella tabella MessageContactEmail'
GO

ALTER TABLE [dbo].[MessageContactEmail] WITH CHECK
  ADD CONSTRAINT [FK_MessageContactEmail_MessageContact] 
	FOREIGN KEY ([IDMessageContact]) 
	REFERENCES [dbo].[MessageContact] ([IDMessageContact]);
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


PRINT 'Creazione ForeignKey nella tabella MessageEmail'
GO

ALTER TABLE [dbo].[MessageEmail] WITH CHECK
  ADD CONSTRAINT [FK_MessageEmail_Message] 
	FOREIGN KEY ([IDMessage]) 
	REFERENCES [dbo].[Message] ([IDMessage]);
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


PRINT 'Creazione ForeignKey nella tabella MessageLog'
GO

ALTER TABLE [dbo].[MessageLog] WITH CHECK
  ADD CONSTRAINT [FK_MessageLog_Message] 
	FOREIGN KEY ([IDMessage]) 
	REFERENCES [dbo].[Message] ([IDMessage]);
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

PRINT 'Conversione IdCollaboration nella tabella CollaborationLog'
GO

ALTER TABLE [dbo].[CollaborationLog] ALTER COLUMN [IdCollaboration] NUMERIC(10,0) null
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

PRINT 'Creazione ForeignKey nella tabella CollaborationLog'
GO

ALTER TABLE [dbo].[CollaborationLog] WITH CHECK
  ADD CONSTRAINT [FK_CollaborationLog_Collaboration] 
	FOREIGN KEY ([IdCollaboration]) 
	REFERENCES [dbo].[Collaboration] ([IdCollaboration]);
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

PRINT 'Creazione ForeignKey nella tabella Role'
GO

ALTER TABLE [dbo].[Role] WITH CHECK
  ADD CONSTRAINT [FK_Role_DocmLocation_Location] 
	FOREIGN KEY ([DocmLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione ForeignKey nella tabella Role'
GO

ALTER TABLE [dbo].[Role] WITH CHECK
  ADD CONSTRAINT [FK_Role_ProtLocation_Location] 
	FOREIGN KEY ([ProtLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione ForeignKey nella tabella Role'
GO

ALTER TABLE [dbo].[Role] WITH CHECK
  ADD CONSTRAINT [FK_Role_ReslLocation_Location] 
	FOREIGN KEY ([ReslLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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

PRINT 'Creazione ForeignKey nella tabella SecurityGroups'
GO

ALTER TABLE [dbo].[SecurityGroups] WITH CHECK
  ADD CONSTRAINT [FK_SecurityGroups_SecurityGroupsFather] 
	FOREIGN KEY ([idGroupFather]) 
	REFERENCES [dbo].[SecurityGroups] ([idGroup]);
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

PRINT 'Creazione ForeignKey nella tabella SecurityUsers'
GO

ALTER TABLE [dbo].[SecurityUsers] WITH CHECK
  ADD CONSTRAINT [FK_SecurityUsers_SecurityGroups] 
	FOREIGN KEY ([idGroup]) 
	REFERENCES [dbo].[SecurityGroups] ([idGroup]);
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


PRINT 'Creazione ForeignKey nella tabella CollaborationUsers'
GO

ALTER TABLE [dbo].[CollaborationUsers] WITH CHECK
  ADD CONSTRAINT [FK_CollaborationUsers_Role] 
	FOREIGN KEY ([idRole]) 
	REFERENCES [dbo].[Role] ([idRole]);
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

PRINT 'Creazione ForeignKey nella tabella Container'
GO

ALTER TABLE [dbo].[Container] WITH CHECK
  ADD CONSTRAINT [FK_Container_ProtLocation_Location] 
	FOREIGN KEY ([ProtLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione ForeignKey nella tabella Container'
GO

ALTER TABLE [dbo].[Container] WITH CHECK
  ADD CONSTRAINT [FK_Container_DocmLocation_Location] 
	FOREIGN KEY ([DocmLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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

PRINT 'Creazione ForeignKey nella tabella Container'
GO

ALTER TABLE [dbo].[Container] WITH CHECK
  ADD CONSTRAINT [FK_Container_ReslLocation_Location] 
	FOREIGN KEY ([ReslLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione PrimaryKey nella tabella ProtocolCheckLog'
GO
ALTER TABLE [dbo].[ProtocolCheckLog] ALTER COLUMN [IdProtocolCheckLog] int not null
GO
ALTER TABLE [dbo].[ProtocolCheckLog] WITH CHECK
  ADD CONSTRAINT [PK_ProtocolCheckLog] 
	PRIMARY KEY CLUSTERED 
	([IdProtocolCheckLog]);
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


PRINT 'Creazione PrimaryKey nella tabella ProtocolJournalLog'
GO
ALTER TABLE [dbo].[ProtocolJournalLog] ALTER COLUMN [IdProtocolJournalLog] int not null
GO
ALTER TABLE [dbo].[ProtocolJournalLog] WITH CHECK
  ADD CONSTRAINT [PK_ProtocolJournalLog] 
	PRIMARY KEY CLUSTERED 
	([IdProtocolJournalLog]);
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

PRINT 'Creazione ForeignKey nella tabella Protocol'
GO

ALTER TABLE [dbo].[Protocol] WITH CHECK
  ADD CONSTRAINT [FK_Protocol_ProtocolCheckLog] 
	FOREIGN KEY ([IdProtocolCheckLog]) 
	REFERENCES [dbo].[ProtocolCheckLog] ([IdProtocolCheckLog]);
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

PRINT 'Creazione ForeignKey nella tabella Protocol'
GO

ALTER TABLE [dbo].[Protocol] WITH CHECK
  ADD CONSTRAINT [FK_Protocol_ProtocolJournalLog] 
	FOREIGN KEY ([IdProtocolJournalLog]) 
	REFERENCES [dbo].[ProtocolJournalLog] ([IdProtocolJournalLog]);
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


PRINT 'Creazione ForeignKey nella tabella PECMail'
GO

ALTER TABLE [dbo].[PECMail] WITH CHECK
  ADD CONSTRAINT [FK_PECMail_Location] 
	FOREIGN KEY ([IDLocation]) 
	REFERENCES [dbo].[Location] ([idLocation]);
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

PRINT 'Creazione ForeignKey nella tabella PECMailBox'
GO

ALTER TABLE [dbo].[PECMailBox] WITH CHECK
  ADD CONSTRAINT [FK_PECMailBox_Location] 
	FOREIGN KEY ([idLocation]) 
	REFERENCES [dbo].[Location] ([idLocation]);
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

PRINT 'Creazione ForeignKey nella tabella OChartItem'
GO

ALTER TABLE [dbo].[OChartItem] WITH CHECK
  ADD CONSTRAINT [FK_OChartItem_OChartItemParent] 
	FOREIGN KEY ([IdParent]) 
	REFERENCES [dbo].[OChartItem] ([Id]);
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

PRINT 'Creazione colonna UniqueId nullable nella tabella Message'
GO
-- 1 
ALTER TABLE [dbo].[Message] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella Message'
GO
-- 2 
UPDATE [dbo].[Message]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[Message]
    ADD CONSTRAINT [DF_Message_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[Message] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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




PRINT 'Creazione colonna UniqueId nullable nella tabella MessageAttachment'
GO
-- 1 
ALTER TABLE [dbo].[MessageAttachment] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella MessageAttachment'
GO
-- 2 
UPDATE [dbo].[MessageAttachment]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[MessageAttachment]
    ADD CONSTRAINT [DF_MessageAttachment_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[MessageAttachment] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella MessageContact'
GO
-- 1 
ALTER TABLE [dbo].[MessageContact] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella MessageContact'
GO
-- 2 
UPDATE [dbo].[MessageContact]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[MessageContact]
    ADD CONSTRAINT [DF_MessageContact_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[MessageContact] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella MessageContactEmail'
GO
-- 1 
ALTER TABLE [dbo].[MessageContactEmail] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella MessageContactEmail'
GO
-- 2 
UPDATE [dbo].[MessageContactEmail]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[MessageContactEmail]
    ADD CONSTRAINT [DF_MessageContactEmail_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[MessageContactEmail] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella MessageEmail'
GO
-- 1 
ALTER TABLE [dbo].[MessageEmail] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella MessageEmail'
GO
-- 2 
UPDATE [dbo].[MessageEmail]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[MessageEmail]
    ADD CONSTRAINT [DF_MessageEmail_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[MessageEmail] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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




PRINT 'Creazione colonna UniqueId nullable nella tabella MessageLog'
GO
-- 1 
ALTER TABLE [dbo].[MessageLog] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella MessageLog'
GO
-- 2 
UPDATE [dbo].[MessageLog]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[MessageLog]
    ADD CONSTRAINT [DF_MessageLog_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[MessageLog] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella Collaboration'
GO
-- 1 
ALTER TABLE [dbo].[Collaboration] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella Collaboration'
GO
-- 2 
UPDATE [dbo].[Collaboration]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[Collaboration]
    ADD CONSTRAINT [DF_Collaboration_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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


PRINT 'Creazione colonna UniqueId nullable nella tabella CollaborationLog'
GO
-- 1 
ALTER TABLE [dbo].[CollaborationLog] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella CollaborationLog'
GO
-- 2 
UPDATE [dbo].[CollaborationLog]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[CollaborationLog]
    ADD CONSTRAINT [DF_CollaborationLog_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[CollaborationLog] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella Protocol'
GO
-- 1 
ALTER TABLE [dbo].[Protocol] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella Protocol'
GO
-- 2 
UPDATE [dbo].[Protocol]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[Protocol]
    ADD CONSTRAINT [DF_Protocol_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[Protocol] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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


PRINT 'Creazione colonna UniqueId nullable nella tabella Type'
GO
-- 1 
ALTER TABLE [dbo].[Type] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella Type'
GO
-- 2 
UPDATE [dbo].[Type]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[Type]
    ADD CONSTRAINT [DF_Type_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[Type] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella PECMail'
GO
-- 1 
ALTER TABLE [dbo].[PECMail] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella PECMail'
GO
-- 2 
UPDATE [dbo].[PECMail]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[PECMail]
    ADD CONSTRAINT [DF_PECMail_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[PECMail] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella PECMailBox'
GO
-- 1 
ALTER TABLE [dbo].[PECMailBox] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella PECMailBox'
GO
-- 2 
UPDATE [dbo].[PECMailBox]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[PECMailBox]
    ADD CONSTRAINT [DF_PECMailBox_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[PECMailBox] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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
--#############################################################################
PRINT 'Creazione delle tabelle di Workflow...'
GO

/****** Object:  Table [dbo].[WorkflowActivities] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowActivities](
	[IdWorkflowActivity] [uniqueidentifier] NOT NULL,
	[IdWorkflowInstance] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NULL,
	[ActivityType] [smallint] NOT NULL,
	[Status] [smallint] NOT NULL,
	[DueDate] [datetimeoffset](7) NULL,
	[Duration] [tinyint] NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_WorkflowActivities] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowActivity] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkflowActivityLogs] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkflowActivityLogs](
	[IdWorkflowActivityLog] [uniqueidentifier] NOT NULL,
	[IdWorkflowActivity] [uniqueidentifier] NOT NULL,
	[LogDate] [datetimeoffset](7) NOT NULL,
	[SystemComputer] [nvarchar](256) NOT NULL,
	[SystemUser] [nvarchar](256) NOT NULL,
	[LogType] [smallint] NOT NULL,
	[LogDescription] [varchar](2000) NOT NULL,
	[Severity] [smallint] NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_WorkflowActivityLogs] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowActivityLog] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkflowInstances] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowInstances](
	[IdWorkflowInstance] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[InstanceId] [uniqueidentifier] NULL,
	[Status] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_WorkflowInstance] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowInstance] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkflowProperties] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowProperties](
	[IdWorkflowProperty] [uniqueidentifier] NOT NULL,
	[IdWorkflowActivity] [uniqueidentifier] NULL,
	[IdWorkflowInstance] [uniqueidentifier] NULL,
	[Name] [nvarchar](256) NOT NULL,
	[WorkflowType] [smallint] NOT NULL,
	[PropertyType] [smallint] NOT NULL,
	[ValueInt] [bigint] NULL,
	[ValueDate] [datetime] NULL,
	[ValueDouble] [float] NULL,
	[ValueBoolean] [bit] NULL,
	[ValueGuid] [uniqueidentifier] NULL,
	[ValueString] [nvarchar](max) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_WorkflowProperties] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowProperty] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkflowRepositories] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowRepositories](
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Version] [nvarchar](5) NOT NULL,
	[ActiveFrom] [datetimeoffset](7) NOT NULL,
	[ActiveTo] [datetimeoffset](7) NULL,
	[Xaml] [xml] NOT NULL,
	[Json] [nvarchar](max) NOT NULL,
	[Status] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL
 CONSTRAINT [PK_WorkflowRepositories] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_WorkflowRepositories_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[WorkflowActivityLogs] ADD  CONSTRAINT [DF_WorkflowActivityLogs_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]
GO

ALTER TABLE [dbo].[WorkflowActivities]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActivities_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[WorkflowActivities] CHECK CONSTRAINT [FK_WorkflowActivities_WorkflowInstances]
GO

ALTER TABLE [dbo].[WorkflowActivityLogs]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActivityLogs_WorkflowActivities] FOREIGN KEY([IdWorkflowActivity])
REFERENCES [dbo].[WorkflowActivities] ([IdWorkflowActivity])
GO

ALTER TABLE [dbo].[WorkflowActivityLogs] CHECK CONSTRAINT [FK_WorkflowActivityLogs_WorkflowActivities]
GO

ALTER TABLE [dbo].[WorkflowInstances]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowInstances_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO

ALTER TABLE [dbo].[WorkflowInstances] CHECK CONSTRAINT [FK_WorkflowInstances_WorkflowRepositories]
GO

ALTER TABLE [dbo].[WorkflowProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowProperties_WorkflowActivities] FOREIGN KEY([IdWorkflowActivity])
REFERENCES [dbo].[WorkflowActivities] ([IdWorkflowActivity])
GO

ALTER TABLE [dbo].[WorkflowProperties] CHECK CONSTRAINT [FK_WorkflowProperties_WorkflowActivities]
GO

ALTER TABLE [dbo].[WorkflowProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowProperties_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[WorkflowProperties] CHECK CONSTRAINT [FK_WorkflowProperties_WorkflowInstances]
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
PRINT 'Creazione delle tabelle di UDS...'
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'uds')
BEGIN
	EXEC('CREATE SCHEMA uds');
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [uds].[UDSSchemaRepositories](
	[IdUDSSchemaRepository] [uniqueidentifier] NOT NULL,
	[SchemaXML] [xml] NULL,
	[Version] [smallint] NOT NULL,
	[ActiveDate] [datetimeoffset](7) NOT NULL,
	[ExpiredDate] [datetimeoffset](7) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[Timestamp] [timestamp] NULL,
 CONSTRAINT [PK_UDSSchemaRepositories] PRIMARY KEY CLUSTERED 
(
	[IdUDSSchemaRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [uds].[UDSRepositories](
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdUDSSchemaRepository] [uniqueidentifier] NOT NULL,
	[IdContainer] [smallint] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[SequenceCurrentYear] [smallint] NOT NULL,
	[SequenceCurrentNumber] [int] NOT NULL,
	[ModuleXML] [xml] NOT NULL,
	[Version] [smallint] NOT NULL,
	[ActiveDate] [datetimeoffset](7) NOT NULL,
	[Status] [smallint] NOT NULL,
	[ExpiredDate] [datetimeoffset](7) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[Timestamp] [timestamp] NULL,
 CONSTRAINT [PK_UDSRepositories] PRIMARY KEY CLUSTERED 
(
	[IdUDSRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [uds].[UDSRepositories]  WITH CHECK ADD  CONSTRAINT [FK_UDSRepositories_UDSSchemaRepositories] FOREIGN KEY([IdUDSSchemaRepository])
REFERENCES [uds].[UDSSchemaRepositories] ([IdUDSSchemaRepository])
GO

ALTER TABLE [uds].[UDSRepositories] CHECK CONSTRAINT [FK_UDSRepositories_UDSSchemaRepositories]
GO

ALTER TABLE [uds].[UDSRepositories]  WITH CHECK ADD  CONSTRAINT [FK_UDSRepositories_Container] FOREIGN KEY([IdContainer])
REFERENCES [dbo].[Container] ([idContainer])
GO

ALTER TABLE [uds].[UDSRepositories] CHECK CONSTRAINT [FK_UDSRepositories_Container]
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
PRINT 'Inserimento parametro TenantId...'
GO

IF NOT EXISTS(SELECT * FROM ParameterEnv WHERE KeyName = 'TenantId')

begin
    INSERT INTO ParameterEnv(KeyName, KeyValue) VALUES ('TenantId', NEWID())
end

ELSE
BEGIN
 PRINT 'Colonna TenantId gia modificata nella tabella ParameterEnv'
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
PRINT 'Creazione colonne UDSLocation e UDSRights...'
GO

ALTER TABLE dbo.Container ADD UDSLocation [smallint] NULL
GO

ALTER TABLE dbo.ContainerGroup ADD UDSRights [char](10) NULL
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
PRINT 'Creazione colonne IdUDS e IdUDSRepository in Protocol...'
GO

ALTER TABLE dbo.Protocol ADD IdUDS [uniqueidentifier] NULL
GO

ALTER TABLE dbo.Protocol ADD IdUDSRepository [uniqueidentifier] NULL
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
PRINT 'Creazione tabella OChartItemWorkflows...'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OChartItemWorkflows](
	[IdOChartItemWorkflow] [uniqueidentifier] NOT NULL,
	[IdOChartItem] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
 CONSTRAINT [PK_OChartItemWorkflows] PRIMARY KEY CLUSTERED 
(
	[IdOChartItemWorkflow] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[OChartItemWorkflows]  WITH CHECK ADD  CONSTRAINT [FK_OChartItemWorkflows_OChartItem] FOREIGN KEY([IdOChartItem])
REFERENCES [dbo].[OChartItem] ([Id])
GO
ALTER TABLE [dbo].[OChartItemWorkflows] CHECK CONSTRAINT [FK_OChartItemWorkflows_OChartItem]
GO
ALTER TABLE [dbo].[OChartItemWorkflows]  WITH CHECK ADD  CONSTRAINT [FK_OChartItemWorkflows_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
ALTER TABLE [dbo].[OChartItemWorkflows] CHECK CONSTRAINT [FK_OChartItemWorkflows_WorkflowRepositories]
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
PRINT 'Creazione tabella WorkflowAuthorizations...'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowAuthorizations](
	[IdWorkflowAuthorization] [uniqueidentifier] NOT NULL,
	[IdWorkflowActivity] [uniqueidentifier] NOT NULL,
	[Account] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
 CONSTRAINT [PK_WorkflowAuthorizations] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowAuthorization] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkflowAuthorizations]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowAuthorizations_WorkflowActivities] FOREIGN KEY([IdWorkflowActivity])
REFERENCES [dbo].[WorkflowActivities] ([IdWorkflowActivity])
GO
ALTER TABLE [dbo].[WorkflowAuthorizations] CHECK CONSTRAINT [FK_WorkflowAuthorizations_WorkflowActivities]
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
PRINT 'Creazione tabella WorkflowRoles...'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowRoles](
	[IdWorkflowRole] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
 CONSTRAINT [PK_WorkflowRoles] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkflowRoles]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowRoles_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([idRole])
GO
ALTER TABLE [dbo].[WorkflowRoles] CHECK CONSTRAINT [FK_WorkflowRoles_Role]
GO
ALTER TABLE [dbo].[WorkflowRoles]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowRoles_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
ALTER TABLE [dbo].[WorkflowRoles] CHECK CONSTRAINT [FK_WorkflowRoles_WorkflowRepositories]
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
PRINT 'Creazione colonne IdUDSRepository e IdOChartItemWorkflow in RoleUser...'
GO

ALTER TABLE dbo.RoleUser ADD IdUDSRepository [uniqueidentifier] NULL
GO

ALTER TABLE dbo.RoleUser ADD IdOChartItemWorkflow [uniqueidentifier] NULL
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

PRINT 'Modifica tabella CollaborationSigns colonna RegistrationUser not nullable'
GO

ALTER TABLE [dbo].[CollaborationSigns] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL

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

PRINT 'Modifica tabella CollaborationSigns colonna RegistrationDate not nullable'
GO

DROP INDEX [IX_CollaborationSigns_RegistationDate] ON [dbo].[CollaborationSigns] WITH ( ONLINE = OFF )
GO

ALTER TABLE [dbo].[CollaborationSigns] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET(7) NOT NULL
GO

CREATE CLUSTERED INDEX [IX_CollaborationSigns_RegistationDate] ON [dbo].[CollaborationSigns]
(
	[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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

PRINT 'Modifica tabella CollaborationUsers colonna RegistrationDate not nullable'
GO

DROP INDEX [IX_CollaborationUsers_RegistationDate] ON [dbo].[CollaborationUsers] WITH ( ONLINE = OFF )
GO

ALTER TABLE [dbo].[CollaborationUsers] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET(7) NOT NULL

CREATE CLUSTERED INDEX [IX_CollaborationUsers_RegistationDate] ON [dbo].[CollaborationUsers]
(
	[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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

PRINT 'Modifica tabella CollaborationUsers colonna RegistrationUser not nullable'
GO

ALTER TABLE [dbo].[CollaborationUsers] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL

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

PRINT 'ALTER TABLE [dbo].[Role] ADD [TenantId] [uniqueidentifier] NULL'
GO

ALTER TABLE [dbo].[Role] ADD [TenantId] [uniqueidentifier] NULL
GO
UPDATE [dbo].[Role] SET [TenantId] = 'F916C0BA-69CF-4589-9B13-DBA7E97CFB8F'
GO
ALTER TABLE [dbo].[Role] ALTER COLUMN [TenantId] [uniqueidentifier] NOT NULL
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

PRINT 'Aggiunta colonna IdRoleTenant alla tabella Role'
GO

ALTER TABLE [dbo].[Role]
ADD [IdRoleTenant] int NULL
GO

UPDATE [dbo].[Role]
SET [IdRoleTenant] = [idRole]
GO

ALTER TABLE [dbo].[Role]
ALTER COLUMN [IdRoleTenant] smallint NOT NULL
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

PRINT 'Aggiunta colonna TenantId alla tabella SecurityGroups'
GO

ALTER TABLE [dbo].[SecurityGroups]
ADD [TenantId] [uniqueidentifier] NULL
GO

UPDATE [dbo].[SecurityGroups] 
SET [TenantId] = 'F916C0BA-69CF-4589-9B13-DBA7E97CFB8F'
GO

ALTER TABLE [dbo].[SecurityGroups]
ALTER COLUMN [TenantId] [uniqueidentifier] NOT NULL

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

PRINT 'Aggiunta colonna IdSecurityGroupTenant alla tabella SecurityGroups'
GO

ALTER TABLE [dbo].[SecurityGroups]
ADD [IdSecurityGroupTenant] int NULL
GO

UPDATE [dbo].[SecurityGroups]
SET [IdSecurityGroupTenant] = [idGroup]
GO

ALTER TABLE [dbo].[SecurityGroups]
ALTER COLUMN [IdSecurityGroupTenant] int NOT NULL
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


PRINT 'Aggiunto indice [IX_Role_IdRoleTenant_TenantId] alla tabella Role'
GO

CREATE UNIQUE INDEX [IX_Role_IdRoleTenant_TenantId] ON [dbo].[Role]
(
[IdRoleTenant] ASC,
[TenantId] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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


PRINT 'Aggiunto indice [IX_SecurityGroups_IdSecurityGroupTenant_TenantId] alla tabella SecurityGroups'
GO

CREATE UNIQUE INDEX [IX_SecurityGroups_IdSecurityGroupTenant_TenantId] ON [dbo].[SecurityGroups]
(
[IdSecurityGroupTenant] ASC,
[TenantId] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
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