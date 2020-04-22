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
PRINT 'Versionamento database alla 8.77'
GO

EXEC dbo.VersioningDatabase N'8.77'
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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
PRINT N'ALTER TABLE [dbo].[AdvancedProtocol]';
GO
ALTER TABLE [dbo].[AdvancedProtocol] ADD 
	[UniqueId] UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
	[UniqueIdProtocol] UNIQUEIDENTIFIER NULL,
	[RegistrationDate] DATETIMEOFFSET(7) NOT NULL DEFAULT getutcdate(),
	[RegistrationUser] NVARCHAR(256) NULL,
	[LastChangedUser] NVARCHAR(256) NULL,
	[LastChangedDate] DATETIMEOFFSET(7) NULL,
	[Timestamp] Timestamp NOT NULL
GO

UPDATE AP SET AP.UniqueIdProtocol = P.UniqueId,
     AP.[RegistrationDate] = P.[RegistrationDate],
     AP.[RegistrationUser] = P.[RegistrationUser],
     AP.[LastChangedDate] = P.[LastChangedDate],
     AP.[LastChangedUser] = P.[LastChangedUser]
FROM [dbo].[AdvancedProtocol] AS AP
INNER JOIN [dbo].[Protocol] P ON P.[Year] = AP.[Year] and P.[Number] = AP.[Number]
WHERE AP.UniqueIdProtocol IS NULL
GO

ALTER TABLE [dbo].[AdvancedProtocol] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
GO
ALTER TABLE [dbo].[AdvancedProtocol] ALTER COLUMN [UniqueIdProtocol] UNIQUEIDENTIFIER NOT NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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
PRINT N'ALTER TABLE [dbo].[Parameter] ';
GO
ALTER TABLE [dbo].[Parameter] ADD 
	[UniqueId] UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
	[RegistrationDate] DATETIMEOFFSET(7) NOT NULL DEFAULT getutcdate(),
	[RegistrationUser] NVARCHAR(256) NOT NULL DEFAULT 'SYSTEM',
	[Timestamp] Timestamp NOT NULL

GO
ALTER TABLE [dbo].[Parameter] ALTER COLUMN [LastChangedDate] DATETIMEOFFSET(7) NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'ALTER TABLE [dbo].[PECMailBox] ADD [RulesetDefinition] NVARCHAR(max) NULL';
GO
ALTER TABLE [dbo].[PECMailBox] ADD [RulesetDefinition] NVARCHAR(max) NULL

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [dbo].[Protocol] ADD [IdSubCategory] SMALLINT NULL';
GO

ALTER TABLE [dbo].[Protocol] ADD [IdSubCategory] SMALLINT NULL
GO

ALTER TABLE [dbo].[Protocol]  WITH CHECK ADD CONSTRAINT [FK_Protocol_Category_SubCategory] FOREIGN KEY([IdSubCategory])
REFERENCES [dbo].[Category] ([idCategory])
GO

ALTER TABLE [dbo].[Protocol] CHECK CONSTRAINT [FK_Protocol_Category_SubCategory]
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
PRINT N'Migrate [dbo].[AdvancedProtocol].[IdSubCategory] to [dbo].[Protocol].[IdSubCategory]';
GO

UPDATE P
SET P.[IdSubCategory] = AP.[IdSubCategory]
FROM [dbo].[Protocol] P
INNER JOIN [dbo].[AdvancedProtocol] AP ON AP.[Year] = P.[Year] AND AP.Number = P.Number
WHERE AP.IdSubCategory IS NOT NULL
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
PRINT N'ALTER TABLE [dbo].[Protocol] ALTER [IdCategoryAPI]';
GO

ALTER TABLE [dbo].[Protocol] DROP COLUMN [IdCategoryAPI]
GO

ALTER TABLE [dbo].[Protocol] ADD [IdCategoryAPI] SMALLINT NULL
GO

UPDATE Protocol set IdCategoryAPI = (CASE WHEN ISNULL(IdSubCategory, 0) = 0 THEN idCategory ELSE IdSubCategory END)
GO

ALTER TABLE [dbo].[Protocol] ALTER COLUMN [IdCategoryAPI] SMALLINT NOT NULL
GO

ALTER TABLE [dbo].[Protocol]  WITH CHECK ADD CONSTRAINT [FK_Protocol_Category_CategoryAPI] FOREIGN KEY([IdCategoryAPI])
REFERENCES [dbo].[Category] ([idCategory])
GO

ALTER TABLE [dbo].[Protocol] CHECK CONSTRAINT [FK_Protocol_Category_CategoryAPI]
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
PRINT N'ALTER TABLE [dbo].[AdvancedProtocol] DROP COLUMN [idSubCategory]';
GO

declare @Drop_advancedprotocolcategory_constraint_command nvarchar(1000)

SELECT TOP 1 @Drop_advancedprotocolcategory_constraint_command = 'ALTER TABLE AdvancedProtocol DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'AdvancedProtocol' AND c.Name = N'idSubCategory'

PRINT @Drop_advancedprotocolcategory_constraint_command
EXECUTE (@Drop_advancedprotocolcategory_constraint_command)
GO

ALTER TABLE [dbo].[AdvancedProtocol] DROP COLUMN [idSubCategory];
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
PRINT N'ALTER TABLE [dbo].[Protocol] DROP COLUMN [idCategory]';
GO

declare @Drop_protocolcategory_constraint_command nvarchar(1000)

SELECT TOP 1 @Drop_protocolcategory_constraint_command = 'ALTER TABLE Protocol DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'Protocol' AND c.Name = N'idCategory'

PRINT @Drop_protocolcategory_constraint_command
EXECUTE (@Drop_protocolcategory_constraint_command)
GO

declare @Drop_protocolcategory_constraint_command nvarchar(1000)
SELECT  TOP 1 @Drop_protocolcategory_constraint_command = 'DROP INDEX ' +  ind.name + ' ON [dbo].[Protocol]'
      
FROM    sys.indexes ind
        INNER JOIN sys.index_columns ic
            ON ind.object_id = ic.object_id
               AND ind.index_id = ic.index_id
        INNER JOIN sys.columns col
            ON ic.object_id = col.object_id
               AND ic.column_id = col.column_id
        INNER JOIN sys.tables t
            ON ind.object_id = t.object_id
WHERE   t.is_ms_shipped = 0 and OBJECT_NAME(ind.object_id) = 'Protocol' and col.name='idCategory'
ORDER BY OBJECT_SCHEMA_NAME(ind.object_id) --SchemaName
      , OBJECT_NAME(ind.object_id) --ObjectName
      , ind.is_primary_key DESC
      , ind.is_unique DESC
      , ind.name --IndexName
      , ic.key_ordinal

PRINT @Drop_protocolcategory_constraint_command
EXECUTE (@Drop_protocolcategory_constraint_command)
GO

ALTER TABLE [dbo].[Protocol] DROP COLUMN [idCategory];
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
PRINT N'ALTER TABLE [dbo].[Protocol] DROP COLUMN [idSubCategory]';
GO

declare @Drop_protocolcategory_constraint_command nvarchar(1000)

SELECT TOP 1 @Drop_protocolcategory_constraint_command = 'ALTER TABLE Protocol DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'Protocol' AND c.Name = N'idSubCategory'

PRINT @Drop_protocolcategory_constraint_command
EXECUTE (@Drop_protocolcategory_constraint_command)
GO

ALTER TABLE [dbo].[Protocol] DROP COLUMN [idSubCategory];
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
PRINT N'ALTER TABLE [dbo].[PECMailAttachment]';
GO

ALTER TABLE [dbo].[PECMailAttachment] ADD 
       [UniqueId] UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
       [RegistrationDate] DATETIMEOFFSET(7) NOT NULL DEFAULT getutcdate(),
       [RegistrationUser] NVARCHAR(256) NULL,
       [LastChangedDate] DATETIMEOFFSET NULL,
       [LastChangedUser] NVARCHAR(256) NULL,
       [Timestamp] Timestamp NOT NULL
GO

UPDATE PCMA SET 
     PCMA.[RegistrationDate] = M.[RegistrationDate],
     PCMA.[RegistrationUser] = M.[RegistrationUser]
FROM [dbo].[PECMailAttachment] AS PCMA
INNER JOIN [dbo].[PECMail] M ON PCMA.[IDPECMail] = M.[IDPECMail] 
GO

ALTER TABLE [dbo].[PECMailAttachment] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'ALTER TABLE [dbo].[PECMailReceipt] ';
GO

ALTER TABLE [dbo].[PECMailReceipt] ADD 
       [UniqueId] UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
       [RegistrationDate] DATETIMEOFFSET(7) NOT NULL DEFAULT getutcdate(),
       [RegistrationUser] NVARCHAR(256) NULL,
       [LastChangedDate] DATETIMEOFFSET NULL,
       [LastChangedUser] NVARCHAR(256) NULL,
       [Timestamp] Timestamp NOT NULL
GO

UPDATE PCMR SET 
     PCMR.[RegistrationDate] = M.[RegistrationDate],
     PCMR.[RegistrationUser] = M.[RegistrationUser]
FROM [dbo].[PECMailReceipt] AS PCMR
INNER JOIN [dbo].[PECMail] M ON PCMR.[IDPECMail] = M.[IDPECMail] 
GO

UPDATE PCMR SET 
     PCMR.[RegistrationDate] = M.[RegistrationDate],
     PCMR.[RegistrationUser] = M.[RegistrationUser]
FROM [dbo].[PECMailReceipt] AS PCMR
INNER JOIN [dbo].[PECMail] M ON PCMR.[IDParent] = M.[IDPECMail] 
GO

ALTER TABLE [dbo].[PECMailReceipt] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'Creazione Tabella UDSContacts'
GO

CREATE TABLE [uds].[UDSContacts] (
	[IdUDSContact] [uniqueidentifier] NOT NULL,
	[IdUDS] [uniqueidentifier] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdContact] [int] NULL,
	[Environment] [int] NOT NULL,
	[ContactManual] nvarchar(max) NULL,
	[ContactType] [smallint] NOT NULL,
	[ContactLabel] nvarchar(256) NOT NULL,
	[RelationType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL
	CONSTRAINT [PK_UDSContacts] PRIMARY KEY NONCLUSTERED
	(
	[IdUDSContact] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_UDSContacts_RegistrationDate] 
	ON [uds].[UDSContacts] ([RegistrationDate] ASC)
GO

ALTER TABLE [uds].[UDSContacts]  WITH CHECK ADD  CONSTRAINT [FK_UDSContacts_UDSRepositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [uds].[UDSContacts] CHECK CONSTRAINT [FK_UDSContacts_UDSRepositories]
GO	

ALTER TABLE [uds].[UDSContacts]  WITH CHECK ADD  CONSTRAINT [FK_UDSContacts_Contact] FOREIGN KEY([IdContact])
REFERENCES [dbo].[Contact] ([Incremental])
GO

ALTER TABLE [uds].[UDSContacts] CHECK CONSTRAINT [FK_UDSContacts_Contact] 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Creazione Tabella UDSMessages'
GO

CREATE TABLE [uds].[UDSMessages] (
	[IdUDSMessage] [uniqueidentifier] NOT NULL,
	[IdUDS] [uniqueidentifier] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdMessage] [int] NOT NULL,
	[Environment] [int] NOT NULL,
	[RelationType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL
CONSTRAINT [PK_UDSMessages] PRIMARY KEY NONCLUSTERED
	(
	[IdUDSMessage] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_UDSMessages_RegistrationDate] 
	ON [uds].[UDSMessages] ([RegistrationDate] ASC)
GO

ALTER TABLE [uds].[UDSMessages]  WITH CHECK ADD  CONSTRAINT [FK_UDSMessages_UDSRepositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [uds].[UDSMessages] CHECK CONSTRAINT [FK_UDSMessages_UDSRepositories]
GO	

ALTER TABLE [uds].[UDSMessages]  WITH CHECK ADD  CONSTRAINT [FK_UDSMessages_Message] FOREIGN KEY([IdMessage])
REFERENCES [dbo].[Message] ([IDMessage])
GO

ALTER TABLE [uds].[UDSMessages] CHECK CONSTRAINT [FK_UDSMessages_Message]
GO	

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################

PRINT N'ALTER TABLE [uds].[UDSDocumentUnits] ';
GO

CREATE TABLE [uds].[UDSDocumentUnits](
	[IdUDSDocumentUnit] [uniqueidentifier] NOT NULL,
	[IdUDS] [uniqueidentifier] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdDocumentUnit] [uniqueidentifier] NULL,
	[Environment] [int] NOT NULL,
	[RelationType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
	 CONSTRAINT [PK_UDSDocumentUnits] PRIMARY KEY NONCLUSTERED 
(
 [IdUDSDocumentUnit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE CLUSTERED INDEX [IX_UDSDocumentUnits_RegistrationDate] 
	ON [uds].[UDSDocumentUnits] ([RegistrationDate] ASC)
GO

ALTER TABLE [uds].[UDSDocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_UDSDocumentUnits_UDSRepositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [uds].[UDSDocumentUnits] CHECK CONSTRAINT [FK_UDSDocumentUnits_UDSRepositories] 
GO	

ALTER TABLE [uds].[UDSDocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_UDSDocumentUnits_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO

ALTER TABLE [uds].[UDSDocumentUnits] CHECK CONSTRAINT [FK_UDSDocumentUnits_DocumentUnits] 
GO	

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'Creazione Tabella UDSPECMails'
GO

CREATE TABLE [uds].[UDSPECMails](
	[IdUDSPECMail] [uniqueidentifier] NOT NULL,
	[IdUDS] [uniqueidentifier] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdPECMail] [int] NOT NULL,
	[Environment] [int] NOT NULL,
	[RelationType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
	CONSTRAINT [PK_UDSPECMail] PRIMARY KEY NONCLUSTERED
	(
	[IdUDSPECMail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_UDSPECMails_RegistrationDate] 
	ON [uds].[UDSPECMails] ([RegistrationDate] ASC)
GO

ALTER TABLE [uds].[UDSPECMails] WITH CHECK ADD CONSTRAINT [FK_UDSPECMails_UDSRepositories] FOREIGN KEY ([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [uds].[UDSPECMails] CHECK CONSTRAINT [FK_UDSPECMails_UDSRepositories]
GO	

ALTER TABLE [uds].[UDSPECMails] WITH CHECK ADD CONSTRAINT [FK_UDSPECMails_PECMail] FOREIGN KEY ([IdPECMail])
REFERENCES [dbo].[PECMail] ([IdPECMail])
GO

ALTER TABLE [uds].[UDSPECMails] CHECK CONSTRAINT [FK_UDSPECMails_PECMail] 
GO	

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'Creazione Tabella UDSCollaborations'
GO

CREATE TABLE [uds].[UDSCollaborations](
	[IdUDSCollaboration] [uniqueidentifier] NOT NULL,
	[IdUDS] [uniqueidentifier] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdCollaboration] [numeric](10,0) NOT NULL,
	[Environment] [int] NOT NULL,
	[RelationType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
	CONSTRAINT [PK_UDSCollaboration] PRIMARY KEY NONCLUSTERED
	(
	[IdUDSCollaboration] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_UDSCollaborations_RegistrationDate] 
	ON [uds].[UDSCollaborations] ([RegistrationDate] ASC)
GO

ALTER TABLE [uds].[UDSCollaborations] WITH CHECK ADD CONSTRAINT [FK_UDSCollaborations_UDSRepositories] FOREIGN KEY ([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [uds].[UDSCollaborations] CHECK CONSTRAINT [FK_UDSCollaborations_UDSRepositories]
GO	

ALTER TABLE [uds].[UDSCollaborations] WITH CHECK ADD CONSTRAINT [FK_UDSCollaborations_Collaboration] FOREIGN KEY ([IdCollaboration])
REFERENCES [dbo].[Collaboration] ([IdCollaboration])
GO

ALTER TABLE [uds].[UDSCollaborations] CHECK CONSTRAINT [FK_UDSCollaborations_Collaboration] 
GO	

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [uds].[UDSRoles] ADD RelationType';
GO

ALTER TABLE [uds].[UDSRoles] ADD [RelationType] smallint NULL
GO

UPDATE [uds].[UDSRoles] SET [RelationType] = 13
GO

ALTER TABLE [uds].[UDSRoles] ALTER COLUMN [RelationType] smallint NOT NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [dbo].[PECMailBox]';
GO

ALTER TABLE [dbo].[PECMailBox] ADD
	[InvoiceType] smallint NULL,
	[HumanEnabled] bit NOT NULL DEFAULT 1
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'ALTER TABLE [dbo].[ProtocolContactManual] ';
GO

ALTER TABLE [dbo].[ProtocolContactManual] ADD 
       [SDIIdentification] NVARCHAR(256) NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'ALTER TABLE [dbo].[Contact]';
GO

ALTER TABLE [dbo].[Contact] ADD
	[SDIIdentification] nvarchar(256) null
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'ALTER TABLE [dbo].[PECMail]';
GO

ALTER TABLE [dbo].[PECMail] ADD
	[InvoiceStatus] smallint null DEFAULT 0
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [dbo].[PECMail]';
GO

ALTER TABLE [dbo].[PECMail] ALTER COLUMN [Checksum] NCHAR(64) NULL
GO
ALTER TABLE [dbo].[PECMail] ALTER COLUMN [HeaderChecksum] NCHAR(64) null
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [uds].[UDSRepositories] DROP CONSTRAINT [PK_UDSRepositories]';
GO

ALTER TABLE [uds].[UDSRepositories] DROP CONSTRAINT [PK_UDSRepositories]
GO

ALTER TABLE [uds].[UDSRepositories] ADD  CONSTRAINT [PK_UDSRepositories] PRIMARY KEY NONCLUSTERED  
(
	[IdUDSRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

PRINT N'SE IL DROP CONSTRAINT [PK_UDSRepositories] E RELATIVO ADD PRIMARY KEY NONCLUSTERED  VANNO IN ERRORE PER REFERENZE DI CHIAVI, PROCEDERE MANUALMENTE TRAMITE DESIGNER IN MODO TALE CHE L''INDICE PK_UDSRepositories DIVENTI "NOT CLUSTERED"';
GO

CREATE CLUSTERED INDEX [IX_UDSRepositories_RegistrationDate] ON [uds].[UDSRepositories]
(
	[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'UPDATE [uds].[UDSSchemaRepositories] SET [SchemaXML]';
GO

UPDATE [uds].[UDSSchemaRepositories] SET [SchemaXML] = '<xs:schema xmlns="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd" xmlns:mstns="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" id="SchemaUnitaDocumentariaSpecifica" targetNamespace="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd" elementFormDefault="qualified">
  <!-- Tipi base -->
  <xs:simpleType name="guid">
    <xs:annotation>
      <xs:documentation xml:lang="en">
        The representation of a GUID, generally the id of an element.
      </xs:documentation>
    </xs:annotation>
    <xs:restriction base="xs:token">
      <xs:pattern value="[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}" />
    </xs:restriction>
  </xs:simpleType>
  <xs:complexType name="BaseType">
    <xs:attribute name="Label" type="xs:token" use="required" />
    <xs:attribute name="ClientId" type="xs:token" use="optional" default="0" />
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required" />
    <xs:attribute name="Searchable" type="xs:boolean" use="required" />
    <xs:attribute name="ModifyEnabled" type="xs:boolean" use="required" />
  </xs:complexType>
  <xs:complexType name="DynamicBaseType">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:sequence>
          <xs:element name="Layout" type="LayoutPosition" minOccurs="0" maxOccurs="1" />
          <xs:element name="JavaScript" type="JavaScript" minOccurs="0" maxOccurs="1" />
          <xs:element name="CustomCSS" type="xs:string" minOccurs="0" maxOccurs="1" />
          <xs:element name="CSSClassName" type="xs:token" minOccurs="0" maxOccurs="1" />
        </xs:sequence>
        <xs:attribute name="Required" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name="FieldBaseType">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:attribute name="ColumnName" type="xs:token" use="required" />
        <xs:attribute name="HiddenField" type="xs:boolean" use="required" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
        <xs:attribute name="ResultPosition" type="xs:short" use="required" />
        <xs:attribute name="Format" type="xs:token" use="optional" />
        <xs:attribute name="ConservationPosition" type="xs:short" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Lista di documenti -->
  <xs:complexType name="Documents">
    <!--Tipo del documento:
        1) documento principale 
        2) documento allegato
        3) documento annesso
      -->
    <xs:all>
      <!--Almeno un documento deve essere inserito se viene creato un Documents-->
      <xs:element name="Document" type="Document" minOccurs="1" maxOccurs="1" />
      <xs:element name="DocumentAttachment" type="Document" minOccurs="0" maxOccurs="1" />
      <xs:element name="DocumentAnnexed" type="Document" minOccurs="0" maxOccurs="1" />
      <xs:element name="DocumentDematerialisation" type="Document" minOccurs="0" maxOccurs="1" />
    </xs:all>
  </xs:complexType>
  <!-- Definizione del classificatore  -->
  <xs:complexType name="Category">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:attribute name="IdCategory" use="required" />
        <xs:attribute name="UniqueId" type="xs:token" />
        <xs:attribute name="DefaultEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione del contenitore  -->
  <xs:complexType name="Container">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:attribute name="IdContainer" use="required" />
        <xs:attribute name="UniqueId" type="xs:token" />
        <xs:attribute name="CreateContainer" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un documento -->
  <xs:complexType name="Document">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:sequence>
          <xs:element name="Instances" type="DocumentInstance" minOccurs="0" maxOccurs="unbounded" />
        </xs:sequence>
        <xs:attribute name="BiblosArchive" type="xs:token" use="required" />
        <xs:attribute name="CreateBiblosArchive" type="xs:boolean" use="required" />
        <xs:attribute name="AllowMultiFile" type="xs:boolean" use="required" />
        <xs:attribute name="Deletable" type="xs:boolean" use="required" />
        <xs:attribute name="UploadEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ScannerEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="DematerialisationEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="SignEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="SignRequired" type="xs:boolean" use="required" />
        <xs:attribute name="CopyProtocol" type="xs:boolean" use="required" />
        <xs:attribute name="CopyResolution" type="xs:boolean" use="required" />
        <xs:attribute name="CopySeries" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione della serie documentale-->
  <xs:complexType name="DocumentInstance">
    <xs:attribute name="IdDocument" />
    <xs:attribute name="DocumentContent" type="xs:token" />
    <xs:attribute name="DocumentName" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione delle serie documentali-->
  <xs:complexType name="DocumentSeries">
    <xs:sequence minOccurs="0">
      <xs:element name="Instances" type="DocumentSeriesInstance" />
    </xs:sequence>
  </xs:complexType>
  <!-- Definizione della serie documentale-->
  <xs:complexType name="DocumentSeriesInstance">
    <xs:attribute name="IdDocumentSeries" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei contatti-->
  <xs:simpleType name="ContactType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="None" />
      <xs:enumeration value="Sender" />
      <xs:enumeration value="Recipient" />
      <xs:enumeration value="AccountAuthorization" />
      <xs:enumeration value="RoleAuthorization" />
    </xs:restriction>
  </xs:simpleType>
  <xs:complexType name="Contacts">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:sequence>
          <!-- Contatto -->
          <xs:element name="ContactInstances" type="ContactInstance" minOccurs="0" maxOccurs="unbounded" />
          <!-- Contatto Manuale-->
          <xs:element name="ContactManualInstances" type="ContactManualInstance" minOccurs="0" maxOccurs="unbounded" />
        </xs:sequence>
        <xs:attribute name="ContactType" type="ContactType" use="optional" />
        <xs:attribute name="ADEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="AddressBookEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ADDistributionListEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ManualEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ExcelImportEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="AllowMultiContact" type="xs:boolean" use="required" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
        <xs:attribute name="ResultPosition" type="xs:short" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione dei contatti-->
  <xs:complexType name="ContactInstance">
    <xs:attribute name="IdContact" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei contatti manuali-->
  <xs:complexType name="ContactManualInstance">
    <xs:attribute name="ContactDescription" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei Authorizations-->
  <xs:complexType name="Authorizations">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:sequence>
          <xs:element name="Instances" type="AuthorizationInstance" minOccurs="0" maxOccurs="unbounded" />
        </xs:sequence>
        <xs:attribute name="AllowMultiAuthorization" type="xs:boolean" use="required" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
        <xs:attribute name="ResultPosition" type="xs:short" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione della tipologia di autorizzazione-->
  <xs:simpleType name="AuthorizationType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="Responsible" />
      <xs:enumeration value="Accounted" />
      <xs:enumeration value="Consulted" />
      <xs:enumeration value="Informed" />
    </xs:restriction>
  </xs:simpleType>
  <!-- Definizione della tipologia di autorizzazione-->
  <xs:simpleType name="AuthorizationInstanceType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="Role" />
      <xs:enumeration value="User" />
    </xs:restriction>
  </xs:simpleType>
  <!-- Definizione di un Authorization-->
  <xs:complexType name="AuthorizationInstance">
    <xs:attribute name="IdAuthorization" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <xs:attribute name="AuthorizationType" type="AuthorizationType" use="required" />
    <xs:attribute name="AuthorizationInstanceType" type="AuthorizationInstanceType" use="required" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei messages-->
  <xs:complexType name="Messages">
    <xs:sequence>
      <xs:element name="Instances" type="MessageInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di un message-->
  <xs:complexType name="MessageInstance">
    <xs:attribute name="IdMessage" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione delle PECMails-->
  <xs:complexType name="PECMails">
    <xs:sequence>
      <xs:element name="Instances" type="PECInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di una pec-->
  <xs:complexType name="PECInstance">
    <xs:attribute name="IdPECMail" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei Protocols-->
  <xs:complexType name="Protocols">
    <xs:sequence>
      <xs:element name="Instances" type="ProtocolInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di un Protocol-->
  <xs:complexType name="ProtocolInstance">
    <xs:attribute name="IdProtocol" type="xs:token" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei Resolutions-->
  <xs:complexType name="Resolutions">
    <xs:sequence>
      <xs:element name="Instances" type="ResolutionInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di un Resolution-->
  <xs:complexType name="ResolutionInstance">
    <xs:attribute name="IdResolution" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione delle Collaborations-->
  <xs:complexType name="Collaborations">
    <xs:sequence>
      <xs:element name="Instances" type="CollaborationInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di una Collaboration-->
  <xs:complexType name="CollaborationInstance">
    <xs:attribute name="IdCollaboration" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione di un campo testuale -->
  <xs:complexType name="TextField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="Multiline" type="xs:boolean" use="required" />
        <xs:attribute name="IsSignature" type="xs:boolean" use="required" />
        <xs:attribute name="DefaultValue" type="xs:string" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:string" use="optional" />
        <xs:attribute name="Value" type="xs:string" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un campo data -->
  <xs:complexType name="DateField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultValue" type="xs:dateTime" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:dateTime" use="optional" />
        <xs:attribute name="Value" type="xs:dateTime" use="optional" />
        <xs:attribute name="RestrictedYear" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un campo booleano -->
  <xs:complexType name="BoolField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultValue" type="xs:boolean" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:boolean" use="optional" />
        <xs:attribute name="Value" type="xs:boolean" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un campo numerico -->
  <xs:complexType name="NumberField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultValue" type="xs:double" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:double" use="optional" />
        <xs:attribute name="Value" type="xs:double" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un campo enum -->
  <xs:complexType name="OptionEnum">
    <xs:sequence>
      <xs:element name="Option" type="xs:token" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
  </xs:complexType>
  <xs:complexType name="EnumField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:sequence>
          <xs:element name="Options" type="OptionEnum" minOccurs="1" maxOccurs="1">
            <!--<xs:unique name="Option_unique">
              <xs:selector xpath="mstns:Option" />
              <xs:field xpath="." />
            </xs:unique>-->
          </xs:element>
        </xs:sequence>
        <xs:attribute name="DefaultValue" type="xs:token" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:token" use="optional" />
        <xs:attribute name="Value" type="xs:token" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name="LookupField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultSearchValue" type="xs:string" use="optional" />
        <xs:attribute name="Value" type="xs:string" use="optional" />
        <xs:attribute name="MultipleValues" type="xs:boolean" />
        <xs:attribute name="LookupArchiveName" type="xs:token" />
        <xs:attribute name="LookupArchiveColumnName" type="xs:token" />
        <xs:attribute name="LookupValue" type="xs:string" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name="StatusType">
    <xs:simpleContent>
      <xs:extension base="xs:token">
        <xs:attribute name="IconPath" type="xs:token" use="required" />
        <xs:attribute name="MappingTag" type="xs:token" use="optional" />
        <xs:attribute name="TagValue" type="xs:token" use="optional" />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>
  <xs:complexType name="StatusEnum">
    <xs:sequence>
      <xs:element name="State" type="StatusType" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
  </xs:complexType>
  <xs:complexType name="StatusField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:sequence>
          <xs:element name="Options" type="StatusEnum" minOccurs="1" maxOccurs="1" />
        </xs:sequence>
        <xs:attribute name="DefaultValue" type="xs:token" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:token" use="optional" />
        <xs:attribute name="Value" type="xs:token" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione dei campi "BASE" di un singolo UDS -->
  <xs:complexType name="Section">
    <xs:choice minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Enum" type="EnumField" minOccurs="0" />
      <xs:element name="Text" type="TextField" minOccurs="0" />
      <xs:element name="Date" type="DateField" minOccurs="0" />
      <xs:element name="Number" type="NumberField" minOccurs="0" />
      <xs:element name="Bool" type="BoolField" minOccurs="0" />
      <xs:element name="Lookup" type="LookupField" minOccurs="0" />
      <xs:element name="Status" type="StatusField" minOccurs="0" />
    </xs:choice>
    <xs:attribute name="SectionLabel" type="xs:token" use="required" />
    <xs:attribute name="SectionId" type="xs:token" use="required" />
  </xs:complexType>
  <xs:complexType name="Metadata">
    <xs:sequence minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Sections" type="Section" minOccurs="1" maxOccurs="unbounded">
        <xs:unique name="Section_LabelUnique">
          <xs:selector xpath="./*" />
          <xs:field xpath="@Label" />
        </xs:unique>
        <xs:unique name="Section_ClientUnique">
          <xs:selector xpath="./*" />
          <xs:field xpath="@ClientId" />
        </xs:unique>
        <xs:unique name="Section_ColumnNameUnique">
          <xs:selector xpath="./*" />
          <xs:field xpath="@ColumnName" />
        </xs:unique>
      </xs:element>
    </xs:sequence>
  </xs:complexType>
  <!-- Definizione dei campi "RELAZIONE" di un singolo UDS verso entità esterne -->
  <xs:complexType name="Relations">
    <xs:choice minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Contacts" type="Contacts" minOccurs="0" />
      <xs:element name="Messages" type="Messages" minOccurs="0" />
      <xs:element name="PECMails" type="PECMails" minOccurs="0" />
      <xs:element name="Protocols" type="Protocols" minOccurs="0" />
      <xs:element name="Resolutions" type="Resolutions" minOccurs="0" />
      <xs:element name="Collaborations" type="Collaborations" minOccurs="0" />
    </xs:choice>
    <xs:attribute name="Label" type="xs:token" use="required" />
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required" />
  </xs:complexType>
  <!-- Definizione della sezione Javascript -->
  <xs:complexType name="JavaScriptSpecification">
    <xs:sequence>
      <xs:element name="FunctionName" type="xs:token" minOccurs="1" maxOccurs="1" />
      <xs:element name="EventType" type="JSEventType" minOccurs="1" maxOccurs="1" />
    </xs:sequence>
  </xs:complexType>
  <xs:complexType name="JavaScript">
    <xs:sequence>
      <xs:element name="Validation" type="JavaScriptSpecification" minOccurs="0" maxOccurs="1" />
      <xs:element name="Action" type="JavaScriptSpecification" minOccurs="0" maxOccurs="1" />
    </xs:sequence>
  </xs:complexType>
  <xs:simpleType name="JSEventType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="onchange" />
      <xs:enumeration value="onclick" />
      <xs:enumeration value="onmouseover" />
      <xs:enumeration value="onmouseout" />
      <xs:enumeration value="onkeydown" />
      <xs:enumeration value="onkeypress" />
      <xs:enumeration value="onkeyup" />
      <xs:enumeration value="onload" />
      <xs:enumeration value="onselect" />
      <xs:enumeration value="onsubmit" />
    </xs:restriction>
  </xs:simpleType>
  <!-- Definizione della sezione Layout -->
  <xs:complexType name="LayoutPosition">
    <xs:sequence>
      <xs:element name="PanelId" type="xs:token" minOccurs="1" maxOccurs="1" />
      <xs:element name="RowNumber" type="xs:unsignedInt" minOccurs="1" maxOccurs="1" />
      <xs:element name="CSS" type="xs:token" minOccurs="0" maxOccurs="1" />
    </xs:sequence>
  </xs:complexType>
  <xs:simpleType name="LayoutType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="GridOneColumn" />
      <xs:enumeration value="GridTwoColumn" />
      <xs:enumeration value="GridThreeColumn" />
    </xs:restriction>
  </xs:simpleType>
  <!-- In una unità documentaria sono necessari i seguenti campi:
       1) Title             -> unico per tutte le unità documentarie
       2) Subject           -> oggetto unità documentaria
       3) Category          -> classificatore
       4) Authorizations    -> diritti unità documentaria
       5) Documents-> inserisco dei documenti (1...N) principali, (1...N) annessi, (1...N) allegati
       6) Metadata -> i metadata possono essere dei campi semplici o dei collegamenti 1:N con elementi esterni.
                      DocumentSeries
                      Message
                      PEC
                      Protocol
                      Resolution
                      Pratiche
                      Contatti
  -->
  <xs:simpleType name="TitleType">
    <xs:restriction base="xs:token">
      <xs:minLength value="3" />
      <xs:maxLength value="55" />
    </xs:restriction>
  </xs:simpleType>
  <xs:simpleType name="AliasType">
    <xs:restriction base="xs:token">
      <xs:minLength value="2" />
      <xs:maxLength value="4" />
    </xs:restriction>
  </xs:simpleType>
  <xs:complexType name="SubjectType">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:attribute name="DefaultValue" type="xs:string" use="optional" />
        <xs:attribute name="Value" type="xs:string" use="optional" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:simpleType name="ProtocolDirectionType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="None" />
      <xs:enumeration value="In" />
      <xs:enumeration value="Out" />
      <xs:enumeration value="InternalOffice" />
    </xs:restriction>
  </xs:simpleType>
  <xs:element name="UnitaDocumentariaSpecifica">
    <xs:complexType>
      <xs:sequence minOccurs="1" maxOccurs="1">
        <xs:element name="Title" type="TitleType" minOccurs="1" maxOccurs="1" nillable="false" />
        <xs:element name="Alias" type="AliasType" minOccurs="1" maxOccurs="1" nillable="false" />
        <xs:element name="Subject" type="SubjectType" minOccurs="1" maxOccurs="1" />
        <xs:element name="Category" type="Category" minOccurs="1" maxOccurs="1" />
        <xs:element name="Container" type="Container" minOccurs="1" maxOccurs="1" />
        <xs:element name="Documents" type="Documents" minOccurs="1" maxOccurs="1" nillable="false" />
        <xs:element name="Authorizations" type="Authorizations" minOccurs="0" maxOccurs="1" />
        <xs:element name="Contacts" type="Contacts" minOccurs="0" maxOccurs="unbounded" />
        <xs:element name="Messages" type="Messages" minOccurs="0" maxOccurs="1" />
        <xs:element name="PECMails" type="PECMails" minOccurs="0" maxOccurs="1" />
        <xs:element name="Protocols" type="Protocols" minOccurs="0" maxOccurs="1" />
        <xs:element name="Resolutions" type="Resolutions" minOccurs="0" maxOccurs="1" />
        <xs:element name="Collaborations" type="Collaborations" minOccurs="0" maxOccurs="1" />
        <xs:element name="Metadata" type="Metadata" minOccurs="1" maxOccurs="1" />
        <xs:element name="CustomJavaScript" type="xs:string" minOccurs="0" maxOccurs="1" />
        <xs:element name="CustomCSS" type="xs:string" minOccurs="0" maxOccurs="1" />
      </xs:sequence>
      <xs:attribute name="WorkflowEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="DocumentUnitSynchronizeEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="DocumentUnitSynchronizeTitle" type="xs:token" use="optional" default="{Year}/{Number:0000000}" />
      <xs:attribute name="ProtocolEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="PECEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="PECButtonEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="MailButtonEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="MailRoleButtonEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="CancelMotivationRequired" type="xs:boolean" use="required" />
      <xs:attribute name="IncrementalIdentityEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="SignatureMetadataEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="Layout" type="LayoutType" use="required" />
      <xs:attribute name="ProtocolDirection" type="ProtocolDirectionType" use="optional" />
      <xs:attribute name="ConservationEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="HideRegistrationIdentifier" type="xs:boolean" use="required" />
      <xs:attribute name="UDSId" type="xs:token" use="optional" />
    </xs:complexType>
  </xs:element>
</xs:schema>'
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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