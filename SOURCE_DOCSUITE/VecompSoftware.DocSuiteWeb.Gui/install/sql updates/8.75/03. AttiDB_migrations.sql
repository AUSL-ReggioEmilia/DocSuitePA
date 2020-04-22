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
PRINT 'Versionamento database alla 8.75'
GO

EXEC dbo.VersioningDatabase N'8.75'
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [dbo].[Role] DROP COLUMN [DocmLocation]'
GO
ALTER TABLE [dbo].[Role] DROP CONSTRAINT [FK_Role_DocmLocation_Location]
GO
ALTER TABLE [dbo].[Role] DROP COLUMN [DocmLocation] 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [dbo].[Role] DROP COLUMN [ReslLocation]'
GO
ALTER TABLE [dbo].[Role] DROP CONSTRAINT [FK_Role_ReslLocation_Location]
GO
ALTER TABLE [dbo].[Role] DROP COLUMN [ReslLocation] 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [dbo].[Role] DROP COLUMN [ProtLocation]'
GO
ALTER TABLE [dbo].[Role] DROP CONSTRAINT [FK_Role_ProtLocation_Location]
GO
ALTER TABLE [dbo].[Role] DROP COLUMN [ProtLocation] 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [dbo].[ResolutionKinds] ADD [Timestamp]'
GO

ALTER TABLE [dbo].[ResolutionKinds] ADD [Timestamp] timestamp NOT NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'DROP ResolutionKind CONSTRAINT from Resolution table'
GO

declare @Drop_resolution_constraint_command nvarchar(1000)

SELECT TOP 1 @Drop_resolution_constraint_command = 'ALTER TABLE Resolution DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'Resolution' AND c.Name = N'IdResolutionKind'

PRINT @Drop_resolution_constraint_command
EXECUTE (@Drop_resolution_constraint_command)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'DROP ResolutionKind CONSTRAINT from ResolutionKindDocumentSeries table'
GO

declare @Drop_resolutionkinddocumentseries_constraint_command nvarchar(1000)

SELECT TOP 1 @Drop_resolutionkinddocumentseries_constraint_command = 'ALTER TABLE ResolutionKindDocumentSeries DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ResolutionKindDocumentSeries' AND c.Name = N'IdResolutionKind'

PRINT @Drop_resolutionkinddocumentseries_constraint_command
EXECUTE (@Drop_resolutionkinddocumentseries_constraint_command)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'DROP ResolutionKind Primary key CONSTRAINT'
GO

declare @Drop_resolutionkinds_primarykey_command  nvarchar(1000)

SELECT TOP 1 @Drop_resolutionkinds_primarykey_command = 'ALTER TABLE ResolutionKinds DROP CONSTRAINT ' + i.name 
FROM [sys].[indexes] AS i 
INNER JOIN [sys].[index_columns] AS ic ON  i.OBJECT_ID = ic.OBJECT_ID AND i.index_id = ic.index_id
WHERE   i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID) = N'ResolutionKinds'

PRINT @Drop_resolutionkinds_primarykey_command
EXECUTE (@Drop_resolutionkinds_primarykey_command)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE ResolutionKinds ADD PrimaryKey CONSTRAINT'
GO

ALTER TABLE [dbo].[ResolutionKinds] ADD CONSTRAINT PK_ResolutionKinds PRIMARY KEY NONCLUSTERED ([IdResolutionKind] ASC)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'CREATE INDEX [IX_ResolutionKinds_RegistrationDate] ON TABLE ResolutionKinds'
GO

CREATE CLUSTERED INDEX [IX_ResolutionKinds_RegistrationDate]
    ON [dbo].[ResolutionKinds]([RegistrationDate] ASC);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE Resolution ADD ForeignKey CONSTRAINT'
GO

ALTER TABLE [dbo].[Resolution]  WITH CHECK ADD  CONSTRAINT [FK_Resolution_ResolutionKinds] FOREIGN KEY([IdResolutionKind])
REFERENCES [dbo].[ResolutionKinds] ([IdResolutionKind])
GO

ALTER TABLE [dbo].[Resolution] CHECK CONSTRAINT [FK_Resolution_ResolutionKinds]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE ResolutionKindDocumentSeries ADD ForeignKey CONSTRAINT'
GO

ALTER TABLE [dbo].[ResolutionKindDocumentSeries]  WITH CHECK ADD  CONSTRAINT [FK_ResolutionKindDocumentSeries_ResolutionKinds] FOREIGN KEY([IdResolutionKind])
REFERENCES [dbo].[ResolutionKinds] ([IdResolutionKind])
GO

ALTER TABLE [dbo].[ResolutionKindDocumentSeries] CHECK CONSTRAINT [FK_ResolutionKindDocumentSeries_ResolutionKinds]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'DROP IsActive CONSTRAINT from ResolutionKinds table'
GO

declare @Drop_resolutionkinds_isactive_constraint_command  nvarchar(1000)

SELECT TOP 1 @Drop_resolutionkinds_isactive_constraint_command = 'ALTER TABLE ResolutionKinds DROP CONSTRAINT ' + dc.name 
FROM [sys].[default_constraints] AS dc 
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = dc.PARENT_OBJECT_ID
WHERE  OBJECT_NAME(c.OBJECT_ID) = N'ResolutionKinds' AND c.Name = N'IsActive'

PRINT @Drop_resolutionkinds_isactive_constraint_command
EXECUTE (@Drop_resolutionkinds_isactive_constraint_command)

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE ResolutionKinds ADD IsActive CONSTRAINT'
GO

ALTER TABLE [dbo].[ResolutionKinds] ALTER COLUMN [IsActive] [bit] NOT NULL
GO

ALTER TABLE [dbo].[ResolutionKinds] ADD CONSTRAINT DF_ResolutionKinds_IsActive  DEFAULT ((1)) FOR [IsActive]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'DROP ResolutionKindDocumentSeries primary key CONSTRAINT'
GO

declare @Drop_resolutionkinddocumentseries_primarykey_command  nvarchar(1000)

SELECT TOP 1 @Drop_resolutionkinddocumentseries_primarykey_command = 'ALTER TABLE ResolutionKindDocumentSeries DROP CONSTRAINT ' + i.name 
FROM [sys].[indexes] AS i 
INNER JOIN [sys].[index_columns] AS ic ON  i.OBJECT_ID = ic.OBJECT_ID AND i.index_id = ic.index_id
WHERE   i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID) = N'ResolutionKindDocumentSeries'

PRINT @Drop_resolutionkinddocumentseries_primarykey_command
EXECUTE (@Drop_resolutionkinddocumentseries_primarykey_command)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE ResolutionKinds ADD PrimaryKey CONSTRAINT'
GO

ALTER TABLE [dbo].[ResolutionKindDocumentSeries] ADD CONSTRAINT PK_ResolutionKindDocumentSeries PRIMARY KEY NONCLUSTERED ([IdResolutionKindDocumentSeries] ASC)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE [dbo].[ResolutionKindDocumentSeries] ADD [Timestamp]'
GO

ALTER TABLE [dbo].[ResolutionKindDocumentSeries] ADD [Timestamp] timestamp NOT NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE ResolutionKindDocumentSeries add auditable columns'
GO

ALTER TABLE [dbo].[ResolutionKindDocumentSeries] ADD [RegistrationDate] [datetimeoffset](7) NULL
GO

ALTER TABLE [dbo].[ResolutionKindDocumentSeries] ADD [RegistrationUser] [nvarchar](256) NULL
GO

ALTER TABLE [dbo].[ResolutionKindDocumentSeries] ADD [LastChangedDate] [datetimeoffset](7) NULL
GO

ALTER TABLE [dbo].[ResolutionKindDocumentSeries] ADD [LastChangedUser] [nvarchar](256) NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'CREATE INDEX [IX_ResolutionKindDocumentSeries_RegistrationDate] ON TABLE ResolutionKindDocumentSeries'
GO

CREATE CLUSTERED INDEX [IX_ResolutionKindDocumentSeries_RegistrationDate]
    ON [dbo].[ResolutionKindDocumentSeries]([RegistrationDate] ASC);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER TABLE ResolutionKindDocumentSeries ADD [IdDocumentSeriesConstraint]'
GO

ALTER TABLE [dbo].[ResolutionKindDocumentSeries] ADD [IdDocumentSeriesConstraint] [uniqueidentifier] NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'CREATE INDEX [IX_ResolutionDocumentSeriesItem_IdDocumentSeriesItem]'
GO

CREATE NONCLUSTERED INDEX [IX_ResolutionDocumentSeriesItem_IdDocumentSeriesItem]
ON [dbo].[ResolutionDocumentSeriesItem] ([IdDocumentSeriesItem])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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