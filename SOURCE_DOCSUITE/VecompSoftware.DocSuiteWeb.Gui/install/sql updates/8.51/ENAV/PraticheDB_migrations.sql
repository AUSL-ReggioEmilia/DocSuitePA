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
PRINT N'ALTER TABLE [dbo].[Document] ALTER COLUMN [Manager] nvarchar(256) NULL';
GO
	ALTER TABLE [dbo].[Document] ALTER COLUMN [Manager] nvarchar(256) NULL
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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
INSERT INTO #tmpErrors (Error) VALUES (1);
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
INSERT INTO #tmpErrors (Error) VALUES (1);
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
  ADD CONSTRAINT [PK_SecurityGroups] 
	PRIMARY KEY CLUSTERED 
	([idGroup]);
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
PRINT 'Add colonne UDSLocation e UDSRights...'
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