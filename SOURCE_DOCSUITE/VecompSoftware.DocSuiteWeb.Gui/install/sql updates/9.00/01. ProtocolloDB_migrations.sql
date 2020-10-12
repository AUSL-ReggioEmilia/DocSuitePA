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
PRINT 'Versionamento database alla 9.00'
GO

EXEC dbo.VersioningDatabase N'9.00',N'DSW Version','MigrationDate'
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[AdvancedProtocol]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE AdvancedProtocol DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'AdvancedProtocol' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[AdvancedProtocol]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE AdvancedProtocol DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'AdvancedProtocol' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[AdvancedProtocol]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE AdvancedProtocol DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'AdvancedProtocol'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key Year from table [dbo].[AdvancedProtocol]'
GO

declare @Drop_year_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON AdvancedProtocol'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'AdvancedProtocol' and c.name='Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_uniqueindex_command
	EXECUTE (@Drop_year_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key Number from table [dbo].[AdvancedProtocol]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON AdvancedProtocol'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'AdvancedProtocol' and c.name='Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[AdvancedProtocol]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON AdvancedProtocol'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'AdvancedProtocol' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolContact]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolContact DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolContact' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolContact]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolContact DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolContact' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolContact]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolContact DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolContact'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key Year from table [dbo].[ProtocolContact]'
GO

declare @Drop_year_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolContact'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolContact' and c.name='Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_uniqueindex_command
	EXECUTE (@Drop_year_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key Number from table [dbo].[ProtocolContact]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolContact'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolContact' and c.name='Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[ProtocolContact]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolContact'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolContact' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolContactIssue]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolContactIssue DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolContactIssue'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolContactManual]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolContactManual DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolContactManual' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolContactManual]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolContactManual DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolContactManual' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[ProtocolContactManual]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolContactManual'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolContactManual' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolContactManual]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolContactManual DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolContactManual'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolDocumentSeriesItems]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolDocumentSeriesItems DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolDocumentSeriesItems' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolDocumentSeriesItems]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolDocumentSeriesItems DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolDocumentSeriesItems' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key Year from table [dbo].[ProtocolDocumentSeriesItems]'
GO

declare @Drop_year_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolDocumentSeriesItems'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolDocumentSeriesItems' and c.name='Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_uniqueindex_command
	EXECUTE (@Drop_year_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key Number from table [dbo].[ProtocolDocumentSeriesItems]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolDocumentSeriesItems'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolDocumentSeriesItems' and c.name='Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolDraft]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolDraft DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolDraft' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolDraft]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolDraft DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolDraft' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[ProtocolDraft]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolDraft'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolDraft' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolDraft]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolDraft DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolDraft'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolImport]'
GO

IF (EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'ProtocolImport'))
BEGIN
    declare @Drop_year_constraint_command nvarchar(1000)

	DECLARE _cursor CURSOR FOR   
	SELECT 'ALTER TABLE ProtocolImport DROP CONSTRAINT ' + fk.name 
	FROM [sys].[foreign_key_columns] AS fkc 
	INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
	INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
	INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
	WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolImport' AND c.Name = N'Year'
	  
	OPEN _cursor  
	  
	FETCH NEXT FROM _cursor   
	INTO @Drop_year_constraint_command  
	  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		PRINT @Drop_year_constraint_command
		EXECUTE (@Drop_year_constraint_command)

		FETCH NEXT FROM _cursor   
		INTO @Drop_year_constraint_command  
	END   
	CLOSE _cursor;  
	DEALLOCATE _cursor;
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
PRINT 'Drop foreign key Number from table [dbo].[ProtocolImport]'
GO

IF (EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'ProtocolImport'))
BEGIN
    declare @Drop_number_constraint_command nvarchar(1000)

	DECLARE _cursor CURSOR FOR   
	SELECT 'ALTER TABLE ProtocolImport DROP CONSTRAINT ' + fk.name 
	FROM [sys].[foreign_key_columns] AS fkc 
	INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
	INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
	INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
	WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolImport' AND c.Name = N'Number'
	  
	OPEN _cursor  
	  
	FETCH NEXT FROM _cursor   
	INTO @Drop_number_constraint_command  
	  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		PRINT @Drop_number_constraint_command
		EXECUTE (@Drop_number_constraint_command)

		FETCH NEXT FROM _cursor   
		INTO @Drop_number_constraint_command  
	END   
	CLOSE _cursor;  
	DEALLOCATE _cursor; 
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
PRINT 'Drop foreign key Year from table [dbo].[ProtocolFolder]'
GO

IF (EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'ProtocolFolder'))
BEGIN
    declare @Drop_year_constraint_command nvarchar(1000)

	DECLARE _cursor CURSOR FOR   
	SELECT 'ALTER TABLE ProtocolFolder DROP CONSTRAINT ' + fk.name 
	FROM [sys].[foreign_key_columns] AS fkc 
	INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
	INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
	INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
	WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolFolder' AND c.Name = N'Year'
	  
	OPEN _cursor  
	  
	FETCH NEXT FROM _cursor   
	INTO @Drop_year_constraint_command  
	  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		PRINT @Drop_year_constraint_command
		EXECUTE (@Drop_year_constraint_command)

		FETCH NEXT FROM _cursor   
		INTO @Drop_year_constraint_command  
	END   
	CLOSE _cursor;  
	DEALLOCATE _cursor;
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
PRINT 'Drop foreign key Number from table [dbo].[ProtocolFolder]'
GO

IF (EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'TheSchema' 
                 AND  TABLE_NAME = 'TheTable'))
BEGIN
    declare @Drop_number_constraint_command nvarchar(1000)

	DECLARE _cursor CURSOR FOR   
	SELECT 'ALTER TABLE ProtocolFolder DROP CONSTRAINT ' + fk.name 
	FROM [sys].[foreign_key_columns] AS fkc 
	INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
	INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
	INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
	WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolFolder' AND c.Name = N'Number'
	  
	OPEN _cursor  
	  
	FETCH NEXT FROM _cursor   
	INTO @Drop_number_constraint_command  
	  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		PRINT @Drop_number_constraint_command
		EXECUTE (@Drop_number_constraint_command)

		FETCH NEXT FROM _cursor   
		INTO @Drop_number_constraint_command  
	END   
	CLOSE _cursor;  
	DEALLOCATE _cursor; 
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
PRINT 'Drop foreign key Year from table [dbo].[ProtocolLinks]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolLinks DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolLinks' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolLinks]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolLinks DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolLinks' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key YearSon from table [dbo].[ProtocolLinks]'
GO

declare @Drop_yearson_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolLinks DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolLinks' AND c.Name = N'YearSon'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_yearson_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_yearson_constraint_command
	EXECUTE (@Drop_yearson_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_yearson_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key NumberSon from table [dbo].[ProtocolLinks]'
GO

declare @Drop_numberson_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolLinks DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolLinks' AND c.Name = N'NumberSon'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_numberson_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_numberson_constraint_command
	EXECUTE (@Drop_numberson_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_numberson_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolLinks]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolLinks DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolLinks'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[ProtocolLinks]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolLinks'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolLinks' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolLog]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolLog DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolLog' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolLog]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolLog DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolLog' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[ProtocolLog]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolLog'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolLog' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolLog]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolLog DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolLog'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolMessage]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolMessage DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolMessage' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolMessage]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolMessage DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolMessage' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[ProtocolMessage]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolMessage'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolMessage' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolMessage]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolMessage DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolMessage'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolParer]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolParer DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolParer' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolParer]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolParer DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolParer' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolParer]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolParer DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolParer'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[ProtocolParer]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolParer'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolParer' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolRecipient]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolRecipient DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolRecipient' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolRecipient]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolRecipient DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolRecipient' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolRecipient]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolRecipient DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolRecipient'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolRejectedRoles]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolRejectedRoles DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolRejectedRoles' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolRejectedRoles]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolRejectedRoles DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolRejectedRoles' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolRoleUser]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolRoleUser DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolRoleUser' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolRoleUser]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolRoleUser DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolRoleUser' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key idRole from table [dbo].[ProtocolRoleUser]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolRoleUser DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolRoleUser' AND c.Name = N'idRole'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolRoleUser]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolRoleUser DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolRoleUser'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[ProtocolRoleUser]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolRoleUser'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolRoleUser' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolRole]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolRole DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolRole' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolRole]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolRole DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolRole' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolRole]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolRole DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolRole'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key Year from table [dbo].[ProtocolRole]'
GO

declare @Drop_year_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolRole'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolRole' and c.name='Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_uniqueindex_command
	EXECUTE (@Drop_year_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key Number from table [dbo].[ProtocolRole]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolRole'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolRole' and c.name='Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop unique index key UniqueId from table [dbo].[ProtocolRole]'
GO

declare @Drop_number_uniqueindex_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'DROP INDEX ' + d.name + ' ON ProtocolRole'
FROM [sys].[tables] t 
JOIN [sys].[indexes] d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1 and d.is_primary_key=0
JOIN [sys].[index_columns] ic on d.index_id=ic.index_id and ic.object_id=t.object_id
JOIN [sys].[columns] c on ic.column_id = c.column_id  and c.object_id=t.object_id
WHERE t.name = 'ProtocolRole' and c.name='UniqueId'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_uniqueindex_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_uniqueindex_command
	EXECUTE (@Drop_number_uniqueindex_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_uniqueindex_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolTransfert]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolTransfert DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolTransfert' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolTransfert]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolTransfert DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolTransfert' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[ProtocolTransfert]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE ProtocolTransfert DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'ProtocolTransfert'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[ProtocolUsers]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolUsers DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolUsers' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[ProtocolUsers]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE ProtocolUsers DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'ProtocolUsers' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Year from table [dbo].[TaskHeaderProtocol]'
GO

declare @Drop_year_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE TaskHeaderProtocol DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'TaskHeaderProtocol' AND c.Name = N'Year'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_year_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_year_constraint_command
	EXECUTE (@Drop_year_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_year_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key Number from table [dbo].[TaskHeaderProtocol]'
GO

declare @Drop_number_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE TaskHeaderProtocol DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'TaskHeaderProtocol' AND c.Name = N'Number'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_number_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_number_constraint_command
	EXECUTE (@Drop_number_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_number_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop foreign key IdUDSRepository from table [dbo].[PECMail]'
GO

declare @Drop_repository_constraint_command nvarchar(1000)

DECLARE _cursor CURSOR FOR   
SELECT 'ALTER TABLE PECMail DROP CONSTRAINT ' + fk.name 
FROM [sys].[foreign_key_columns] AS fkc 
INNER JOIN [sys].[foreign_keys] AS fk ON fk.object_id = fkc.constraint_object_id
INNER JOIN [sys].[tables] AS t ON fkc.parent_object_id = t.object_id
INNER JOIN [sys].[columns] AS c ON  c.OBJECT_ID = fkc.PARENT_OBJECT_ID and fkc.parent_column_id = c.column_id
WHERE  OBJECT_NAME(t.OBJECT_ID) = N'PECMail' AND c.Name = N'IdUDSRepository'
  
OPEN _cursor  
  
FETCH NEXT FROM _cursor   
INTO @Drop_repository_constraint_command  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	PRINT @Drop_repository_constraint_command
	EXECUTE (@Drop_repository_constraint_command)

    FETCH NEXT FROM _cursor   
    INTO @Drop_repository_constraint_command  
END   
CLOSE _cursor;  
DEALLOCATE _cursor; 

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop primary key from table [dbo].[Protocol]'
GO

declare @Drop_pk_constraint_command nvarchar(1000)

SELECT @Drop_pk_constraint_command = 'ALTER TABLE Protocol DROP CONSTRAINT ' + name 
FROM [sys].[key_constraints]
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'Protocol'
  
PRINT @Drop_pk_constraint_command
EXECUTE (@Drop_pk_constraint_command);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_Protocol into table [dbo].[Protocol] for column [UniqueId]'
GO

ALTER TABLE [dbo].[Protocol]
ADD CONSTRAINT PK_Protocol PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_Protocol_RegistrationDate into table [dbo].[Protocol] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_Protocol_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[Protocol]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_Protocol_RegistrationDate] ON [dbo].[Protocol]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Add column IdTenantAOO into table [dbo].[Protocol]'
GO

ALTER TABLE [dbo].[Protocol]
ADD IdTenantAOO UNIQUEIDENTIFIER NULL;
GO

UPDATE [dbo].[Protocol] SET IdTenantAOO = NEWID()
GO

ALTER TABLE [dbo].[Protocol]
ALTER COLUMN IdTenantAOO UNIQUEIDENTIFIER NOT NULL;
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create unique index IX_Protocol_Year_Number_IdTenantAOO into table [dbo].[Protocol]'
GO

CREATE UNIQUE INDEX [IX_Protocol_Year_Number_IdTenantAOO] 
	ON [dbo].[Protocol] ([Year],[Number],[IdTenantAOO])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_AdvancedProtocol into table [dbo].[AdvancedProtocol] for column [UniqueId]'
GO

ALTER TABLE [dbo].[AdvancedProtocol]
ADD CONSTRAINT PK_AdvancedProtocol PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_AdvancedProtocol_Protocol into table [dbo].[AdvancedProtocol] for column [UniqueIdProtocol]'
GO

ALTER TABLE [dbo].[AdvancedProtocol]  WITH CHECK ADD CONSTRAINT [FK_AdvancedProtocol_Protocol] FOREIGN KEY([UniqueIdProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[AdvancedProtocol] CHECK CONSTRAINT [FK_AdvancedProtocol_Protocol]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create unique index IX_AdvancedProtocol_UniqueIdProtocol into table [dbo].[AdvancedProtocol]'
GO

CREATE UNIQUE INDEX [IX_AdvancedProtocol_UniqueIdProtocol] 
	ON [dbo].[AdvancedProtocol] ([UniqueIdProtocol])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_AdvancedProtocol_RegistrationDate into table [dbo].[AdvancedProtocol] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_AdvancedProtocol_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[AdvancedProtocol]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_AdvancedProtocol_RegistrationDate] ON [dbo].[AdvancedProtocol]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Create primary key PK_ProtocolContact into table [dbo].[ProtocolContact] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolContact]
ADD CONSTRAINT PK_ProtocolContact PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create unique index IX_ProtocolContact_UniqueIdProtocol_IdContact_ComunicationType into table [dbo].[ProtocolContact]'
GO

CREATE UNIQUE INDEX [IX_ProtocolContact_UniqueIdProtocol_IdContact_ComunicationType] 
	ON [dbo].[ProtocolContact] ([UniqueIdProtocol],[IdContact],[ComunicationType])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolContact_RegistrationDate into table [dbo].[ProtocolContact] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolContact_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolContact]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolContact_RegistrationDate] ON [dbo].[ProtocolContact]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Add column UniqueId into table [dbo].[ProtocolContactIssue]'
GO

ALTER TABLE [dbo].[ProtocolContactIssue]
ADD UniqueId UNIQUEIDENTIFIER NOT NULL DEFAULT (newid())
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column UniqueIdProtocol into table [dbo].[ProtocolContactIssue]'
GO

ALTER TABLE [dbo].[ProtocolContactIssue]
ADD UniqueIdProtocol UNIQUEIDENTIFIER NULL
GO

UPDATE PCI SET UniqueIdProtocol = P.UniqueId   
FROM [dbo].[ProtocolContactIssue] PCI
INNER JOIN [dbo].[Protocol] P ON P.Year = PCI.Year AND P.Number = PCI.Number
GO

ALTER TABLE [dbo].[ProtocolContactIssue]
ALTER COLUMN UniqueIdProtocol UNIQUEIDENTIFIER NOT NULL
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolContactIssue into table [dbo].[ProtocolContactIssue] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolContactIssue]
ADD CONSTRAINT PK_ProtocolContactIssue PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_ProtocolContactIssue_Protocol into table [dbo].[ProtocolContactIssue] for column [UniqueIdProtocol]'
GO

ALTER TABLE [dbo].[ProtocolContactIssue]  WITH CHECK ADD CONSTRAINT [FK_ProtocolContactIssue_Protocol] FOREIGN KEY([UniqueIdProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolContactIssue] CHECK CONSTRAINT [FK_ProtocolContactIssue_Protocol]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolContactIssue_RegistrationDate into table [dbo].[ProtocolContactIssue] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolContactIssue_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolContactIssue]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolContactIssue_RegistrationDate] ON [dbo].[ProtocolContactIssue]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Create unique index IX_ProtocolContactIssue_UniqueIdProtocol_IdContact into table [dbo].[ProtocolContactIssue]'
GO

CREATE UNIQUE INDEX [IX_ProtocolContactIssue_UniqueIdProtocol_IdContact] 
	ON [dbo].[ProtocolContactIssue] ([UniqueIdProtocol],[IdContact])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolContactManual into table [dbo].[ProtocolContactManual] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolContactManual]
ADD CONSTRAINT PK_ProtocolContactManual PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolContactManual_RegistrationDate into table [dbo].[ProtocolContactManual] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolContactManual_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolContactManual]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolContactManual_RegistrationDate] ON [dbo].[ProtocolContactManual]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Create unique index IX_ProtocolContactManual_UniqueIdProtocol_Incremental into table [dbo].[ProtocolContactManual]'
GO

CREATE UNIQUE INDEX [IX_ProtocolContactManual_UniqueIdProtocol_Incremental] 
	ON [dbo].[ProtocolContactManual] ([UniqueIdProtocol],[Incremental])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create unique index IX_ProtocolDocumentSeriesItems_UniqueIdProtocol_IdDocumentSeriesItem into table [dbo].[ProtocolDocumentSeriesItems]'
GO

CREATE UNIQUE INDEX [IX_ProtocolDocumentSeriesItems_UniqueIdProtocol_IdDocumentSeriesItem] 
	ON [dbo].[ProtocolDocumentSeriesItems] ([UniqueIdProtocol],[IdDocumentSeriesItem])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolDraft into table [dbo].[ProtocolDraft] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolDraft]
ADD CONSTRAINT PK_ProtocolDraft PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolDraft_RegistrationDate into table [dbo].[ProtocolDraft] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolDraft_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolDraft]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolDraft_RegistrationDate] ON [dbo].[ProtocolDraft]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Add column UniqueIdProtocol into table [dbo].[ProtocolDraft]'
GO

ALTER TABLE [dbo].[ProtocolDraft]
ADD UniqueIdProtocol UNIQUEIDENTIFIER NULL
GO

UPDATE PCI SET UniqueIdProtocol = P.UniqueId   
FROM [dbo].[ProtocolDraft] PCI
INNER JOIN [dbo].[Protocol] P ON P.Year = PCI.Year AND P.Number = PCI.Number
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_ProtocolDraft_Protocol into table [dbo].[ProtocolDraft] for column [UniqueIdProtocol]'
GO

ALTER TABLE [dbo].[ProtocolDraft]  WITH CHECK ADD CONSTRAINT [FK_ProtocolDraft_Protocol] FOREIGN KEY([UniqueIdProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolDraft] CHECK CONSTRAINT [FK_ProtocolDraft_Protocol]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolLinks into table [dbo].[ProtocolLinks] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolLinks]
ADD CONSTRAINT PK_ProtocolLinks PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolLinks_RegistrationDate into table [dbo].[ProtocolLinks] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolLinks_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolLinks]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolLinks_RegistrationDate] ON [dbo].[ProtocolLinks]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Create unique index IX_ProtocolLinks_UniqueIdProtocolParent_UniqueIdProtocolSon into table [dbo].[ProtocolLinks]'
GO

CREATE UNIQUE INDEX [IX_ProtocolLinks_UniqueIdProtocolParent_UniqueIdProtocolSon] 
	ON [dbo].[ProtocolLinks] ([UniqueIdProtocolParent],[UniqueIdProtocolSon])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolLog into table [dbo].[ProtocolLog] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolLog]
ADD CONSTRAINT PK_ProtocolLog PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolMessage into table [dbo].[ProtocolMessage] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolMessage]
ADD CONSTRAINT PK_ProtocolMessage PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolMessage_RegistrationDate into table [dbo].[ProtocolMessage] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolMessage_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolMessage]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolMessage_RegistrationDate] ON [dbo].[ProtocolMessage]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Create primary key PK_ProtocolParer into table [dbo].[ProtocolParer] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolParer]
ADD CONSTRAINT PK_ProtocolParer PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolParer_RegistrationDate into table [dbo].[ProtocolParer] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolParer_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolParer]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolParer_RegistrationDate] ON [dbo].[ProtocolParer]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Create unique index IX_ProtocolParer_UniqueIdProtocol into table [dbo].[ProtocolParer]'
GO

CREATE UNIQUE INDEX [IX_ProtocolParer_UniqueIdProtocol] 
	ON [dbo].[ProtocolParer] ([UniqueIdProtocol])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column UniqueId into table [dbo].[ProtocolRecipient]'
GO

ALTER TABLE [dbo].[ProtocolRecipient]
ADD UniqueId UNIQUEIDENTIFIER NOT NULL DEFAULT (newid())
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolRecipient into table [dbo].[ProtocolRecipient] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolRecipient]
ADD CONSTRAINT PK_ProtocolRecipient PRIMARY KEY CLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column UniqueIdProtocol into table [dbo].[ProtocolRecipient]'
GO

ALTER TABLE [dbo].[ProtocolRecipient]
ADD UniqueIdProtocol UNIQUEIDENTIFIER NULL
GO

UPDATE PCI SET UniqueIdProtocol = P.UniqueId   
FROM [dbo].[ProtocolRecipient] PCI
INNER JOIN [dbo].[Protocol] P ON P.Year = PCI.Year AND P.Number = PCI.Number
GO

ALTER TABLE [dbo].[ProtocolRecipient]
ALTER COLUMN UniqueIdProtocol UNIQUEIDENTIFIER NOT NULL
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_ProtocolRecipient_Protocol into table [dbo].[ProtocolRecipient] for column [UniqueIdProtocol]'
GO

ALTER TABLE [dbo].[ProtocolRecipient]  WITH CHECK ADD CONSTRAINT [FK_ProtocolRecipient_Protocol] FOREIGN KEY([UniqueIdProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolRecipient] CHECK CONSTRAINT [FK_ProtocolRecipient_Protocol]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create unique index IX_ProtocolRecipient_UniqueIdProtocol_idRecipient into table [dbo].[ProtocolRecipient]'
GO

CREATE UNIQUE INDEX [IX_ProtocolRecipient_UniqueIdProtocol_idRecipient] 
	ON [dbo].[ProtocolRecipient] ([UniqueIdProtocol],[idRecipient])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolRole into table [dbo].[ProtocolRole] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolRole]
ADD CONSTRAINT PK_ProtocolRole PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolRole_RegistrationDate into table [dbo].[ProtocolRole] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolRole_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolRole]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolRole_RegistrationDate] ON [dbo].[ProtocolRole]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Create unique index IX_ProtocolRole_UniqueIdProtocol_idRole into table [dbo].[ProtocolRole]'
GO

CREATE UNIQUE INDEX [IX_ProtocolRole_UniqueIdProtocol_idRole] 
	ON [dbo].[ProtocolRole] ([UniqueIdProtocol],[idRole])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolRoleUser into table [dbo].[ProtocolRoleUser] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolRoleUser]
ADD CONSTRAINT PK_ProtocolRoleUser PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolRoleUser_RegistrationDate into table [dbo].[ProtocolRoleUser] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolRoleUser_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolRoleUser]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolRoleUser_RegistrationDate] ON [dbo].[ProtocolRoleUser]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Create unique index IX_ProtocolRoleUser_UniqueIdProtocol_idRole_GroupName_UserName into table [dbo].[ProtocolRoleUser]'
GO

CREATE UNIQUE INDEX [IX_ProtocolRoleUser_UniqueIdProtocol_idRole_GroupName_UserName] 
	ON [dbo].[ProtocolRoleUser] ([UniqueIdProtocol],[idRole],[GroupName],[UserName])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column IdProtocolRole into table [dbo].[ProtocolRoleUser]'
GO

ALTER TABLE [dbo].[ProtocolRoleUser]
ADD IdProtocolRole UNIQUEIDENTIFIER NULL
GO

UPDATE PRU SET IdProtocolRole = PR.UniqueId   
FROM [dbo].[ProtocolRoleUser] PRU
INNER JOIN [dbo].[ProtocolRole] PR ON PR.UniqueIdProtocol = PRU.UniqueIdProtocol AND PR.idRole = PRU.idRole
GO

ALTER TABLE [dbo].[ProtocolRoleUser]
ALTER COLUMN IdProtocolRole UNIQUEIDENTIFIER NOT NULL
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_ProtocolRoleUser_ProtocolRole into table [dbo].[ProtocolRoleUser] for column [IdProtocolRole]'
GO

ALTER TABLE [dbo].[ProtocolRoleUser]  WITH CHECK ADD CONSTRAINT [FK_ProtocolRoleUser_ProtocolRole] FOREIGN KEY([IdProtocolRole])
REFERENCES [dbo].[ProtocolRole] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolRoleUser] CHECK CONSTRAINT [FK_ProtocolRoleUser_ProtocolRole]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Alter column [Account] from table [ProtocolRoleUser]'
GO

ALTER TABLE [dbo].[ProtocolRoleUser]
ALTER COLUMN [Account] NVARCHAR(256) NOT NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column UniqueId into table [dbo].[ProtocolTransfert]'
GO

ALTER TABLE [dbo].[ProtocolTransfert]
ADD UniqueId UNIQUEIDENTIFIER NOT NULL DEFAULT (newid())
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column UniqueIdProtocol into table [dbo].[ProtocolTransfert]'
GO

ALTER TABLE [dbo].[ProtocolTransfert]
ADD UniqueIdProtocol UNIQUEIDENTIFIER NULL
GO

UPDATE PCI SET UniqueIdProtocol = P.UniqueId   
FROM [dbo].[ProtocolTransfert] PCI
INNER JOIN [dbo].[Protocol] P ON P.Year = PCI.Year AND P.Number = PCI.Number
GO

ALTER TABLE [dbo].[ProtocolTransfert]
ALTER COLUMN UniqueIdProtocol UNIQUEIDENTIFIER NOT NULL
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create primary key PK_ProtocolTransfert into table [dbo].[ProtocolTransfert] for column [UniqueId]'
GO

ALTER TABLE [dbo].[ProtocolTransfert]
ADD CONSTRAINT PK_ProtocolTransfert PRIMARY KEY NONCLUSTERED (UniqueId);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_ProtocolTransfert_Protocol into table [dbo].[ProtocolTransfert] for column [UniqueIdProtocol]'
GO

ALTER TABLE [dbo].[ProtocolTransfert]  WITH CHECK ADD CONSTRAINT [FK_ProtocolTransfert_Protocol] FOREIGN KEY([UniqueIdProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolTransfert] CHECK CONSTRAINT [FK_ProtocolTransfert_Protocol]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create index IX_ProtocolTransfert_RegistrationDate into table [dbo].[ProtocolTransfert] for column [RegistrationDate]'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes  WHERE name='IX_ProtocolTransfert_RegistrationDate' AND object_id = OBJECT_ID('[dbo].[ProtocolTransfert]'))
BEGIN
    CREATE CLUSTERED INDEX [IX_ProtocolTransfert_RegistrationDate] ON [dbo].[ProtocolTransfert]
	(
		[RegistrationDate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Create unique index IX_ProtocolTransfert_UniqueIdProtocol into table [dbo].[ProtocolTransfert]'
GO

CREATE UNIQUE INDEX [IX_ProtocolTransfert_UniqueIdProtocol] 
	ON [dbo].[ProtocolTransfert] ([UniqueIdProtocol])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column UniqueIdProtocol into table [dbo].[TaskHeaderProtocol]'
GO

ALTER TABLE [dbo].[TaskHeaderProtocol]
ADD UniqueIdProtocol UNIQUEIDENTIFIER NULL
GO

UPDATE PCI SET UniqueIdProtocol = P.UniqueId   
FROM [dbo].[TaskHeaderProtocol] PCI
INNER JOIN [dbo].[Protocol] P ON P.Year = PCI.Year AND P.Number = PCI.Number
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create unique index IX_TaskHeaderProtocol_UniqueIdProtocol into table [dbo].[TaskHeaderProtocol]'
GO

CREATE UNIQUE INDEX [IX_TaskHeaderProtocol_UniqueIdProtocol] 
	ON [dbo].[TaskHeaderProtocol] ([UniqueIdProtocol])

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column IdDocumentUnit into table [dbo].[PECMail]'
GO

ALTER TABLE [dbo].[PECMail]
ADD IdDocumentUnit UNIQUEIDENTIFIER NULL
GO

UPDATE PM SET IdDocumentUnit = DU.IdDocumentUnit
FROM [cqrs].[DocumentUnits] DU
INNER JOIN [dbo].[PECMail] PM ON PM.Year = DU.Year AND PM.Number = DU.Number AND PM.DocumentUnitType = 1
WHERE DU.Environment = 1
GO

UPDATE PM SET IdDocumentUnit = DU.IdDocumentUnit
FROM [cqrs].[DocumentUnits] DU
INNER JOIN [dbo].[PECMail] PM ON PM.Year = DU.Year AND PM.Number = DU.Number AND PM.DocumentUnitType = 7
WHERE DU.Environment >= 100
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_PECMail_DocumentUnits into table [dbo].[PECMail] for column [IdDocumentUnit]'
GO

ALTER TABLE [dbo].[PECMail]  WITH CHECK ADD CONSTRAINT [FK_PECMail_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO

ALTER TABLE [dbo].[PECMail] CHECK CONSTRAINT [FK_PECMail_DocumentUnits]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop column IdUDS from table [dbo].[PECMail]'
GO

ALTER TABLE [dbo].[PECMail] DROP COLUMN IdUDS
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop column IdUDSRepository from table [dbo].[PECMail]'
GO

ALTER TABLE [dbo].[PECMail] DROP COLUMN IdUDSRepository
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop column DocumentUnitType from table [dbo].[PECMail]'
GO

ALTER TABLE [dbo].[PECMail] DROP COLUMN DocumentUnitType
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column IdDocumentUnit into table [dbo].[PosteOnLineRequest]'
GO

ALTER TABLE [dbo].[PosteOnLineRequest]
ADD IdDocumentUnit UNIQUEIDENTIFIER NULL
GO

UPDATE PL SET IdDocumentUnit = DU.IdDocumentUnit
FROM [cqrs].[DocumentUnits] DU
INNER JOIN [dbo].[PosteOnLineRequest] PL ON PL.ProtocolYear = DU.Year AND PL.ProtocolNumber = DU.Number
WHERE DU.Environment = 1
GO

ALTER TABLE [dbo].[PosteOnLineRequest]
ALTER COLUMN IdDocumentUnit UNIQUEIDENTIFIER NOT NULL;
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_PosteOnLineRequest_DocumentUnits into table [dbo].[PosteOnLineRequest] for column [IdDocumentUnit]'
GO

ALTER TABLE [dbo].[PosteOnLineRequest]  WITH CHECK ADD CONSTRAINT [FK_PosteOnLineRequest_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO

ALTER TABLE [dbo].[PosteOnLineRequest] CHECK CONSTRAINT [FK_PosteOnLineRequest_DocumentUnits]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop column ProtocolYear from table [dbo].[PosteOnLineRequest]'
GO

ALTER TABLE [dbo].[PosteOnLineRequest] DROP COLUMN ProtocolYear
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Drop column ProtocolNumber from table [dbo].[PosteOnLineRequest]'
GO

ALTER TABLE [dbo].[PosteOnLineRequest] DROP COLUMN ProtocolNumber
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column IdTenantAOO into table [cqrs].[DocumentUnits]'
GO

ALTER TABLE [cqrs].[DocumentUnits]
ADD IdTenantAOO UNIQUEIDENTIFIER NULL;
GO

UPDATE [cqrs].[DocumentUnits] SET IdTenantAOO = NEWID()
GO

ALTER TABLE [cqrs].[DocumentUnits]
ALTER COLUMN IdTenantAOO UNIQUEIDENTIFIER NOT NULL;
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column IdDocumentUnit into table [dbo].[Collaboration]'
GO

ALTER TABLE [dbo].[Collaboration]
ADD IdDocumentUnit UNIQUEIDENTIFIER NULL
GO

UPDATE C SET IdDocumentUnit = DU.IdDocumentUnit
FROM [cqrs].[DocumentUnits] DU
INNER JOIN [dbo].[Collaboration] C ON C.Year = DU.Year AND C.Number = DU.Number
WHERE DU.Environment = 1 AND C.DocumentType IN ('P','W','U')
GO

UPDATE C SET IdDocumentUnit = DU.IdDocumentUnit
FROM [cqrs].[DocumentUnits] DU
INNER JOIN [dbo].[Collaboration] C ON C.IdResolution = DU.EntityId
WHERE DU.Environment = 2 AND C.DocumentType IN ('D','A')
GO

UPDATE C SET IdDocumentUnit = DU.IdDocumentUnit
FROM [cqrs].[DocumentUnits] DU
INNER JOIN [dbo].[Collaboration] C ON C.idDocumentSeriesItem = DU.EntityId
WHERE DU.Environment = 4 AND C.DocumentType = 'S'
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column UniqueIdSourceProtocol into table [dbo].[Collaboration]'
GO

ALTER TABLE [dbo].[Collaboration]
ADD UniqueIdSourceProtocol UNIQUEIDENTIFIER NULL
GO

UPDATE C SET UniqueIdSourceProtocol = P.UniqueId
FROM [dbo].[Protocol] P
INNER JOIN [dbo].[Collaboration] C ON C.SourceProtocolYear = P.Year AND C.SourceProtocolNumber = P.Number
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_Collaboration_DocumentUnits into table [dbo].[Collaboration] for column [IdDocumentUnit]'
GO

ALTER TABLE [dbo].[Collaboration]  WITH CHECK ADD CONSTRAINT [FK_Collaboration_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO

ALTER TABLE [dbo].[Collaboration] CHECK CONSTRAINT [FK_Collaboration_DocumentUnits]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create foreign key FK_Collaboration_Protocol into table [dbo].[Collaboration] for column [UniqueIdSourceProtocol]'
GO

ALTER TABLE [dbo].[Collaboration]  WITH CHECK ADD CONSTRAINT [FK_Collaboration_Protocol] FOREIGN KEY([UniqueIdSourceProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[Collaboration] CHECK CONSTRAINT [FK_Collaboration_Protocol]
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column IdTenantAOO into table [dbo].[Parameter]'
GO

ALTER TABLE [dbo].[Parameter]
ADD IdTenantAOO UNIQUEIDENTIFIER NULL;
GO

UPDATE [dbo].[Parameter] SET IdTenantAOO = NEWID()
GO

ALTER TABLE [dbo].[Parameter]
ALTER COLUMN IdTenantAOO UNIQUEIDENTIFIER NOT NULL;
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column IdTenantAOO into table [dbo].[Tenants]'
GO

ALTER TABLE [dbo].[Tenants]
ADD IdTenantAOO UNIQUEIDENTIFIER NULL;
GO

UPDATE [dbo].[Tenants] SET IdTenantAOO = NEWID()
GO

ALTER TABLE [dbo].[Tenants]
ALTER COLUMN IdTenantAOO UNIQUEIDENTIFIER NOT NULL;
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Add column IdTenantAOO into table [dbo].[ProtocolJournalLog]'
GO

ALTER TABLE [dbo].[ProtocolJournalLog]
ADD IdTenantAOO UNIQUEIDENTIFIER NULL;
GO

UPDATE [dbo].[ProtocolJournalLog] SET IdTenantAOO = NEWID()
GO

ALTER TABLE [dbo].[ProtocolJournalLog]
ALTER COLUMN IdTenantAOO UNIQUEIDENTIFIER NOT NULL;
GO

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'ALTER SQL Function [dbo].[GetDiarioUnificatoDetails]'
GO

ALTER FUNCTION [dbo].[GetDiarioUnificatoDetails](@IdTipologia INT, @DataDal DATETIME,  @DataAl DATETIME, @User NVARCHAR(256), @Riferimento1 AS INT = NULL, @Riferimento2 AS INT = NULL,  @Riferimento3 AS UNIQUEIDENTIFIER = NULL, @IdTenantAOO AS UNIQUEIDENTIFIER)
RETURNS TABLE 
AS
RETURN 
(
	SELECT O.IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol,
	L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[CollaborationLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Collaborazione')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Collaborazione') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.LogDate BETWEEN @DataDal AND @DataAl	
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IdCollaboration = ISNULL(@Riferimento1, O.IdCollaboration) )
		
	UNION 

	SELECT CAST(NULL AS INT) AS IdCollaboration, O.IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol,
	L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[DocumentSeriesItemLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Serie Documentali/Archivi')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Serie Documentali/Archivi') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.LogDate BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IdDocumentSeriesItem = ISNULL(@Riferimento1, O.IdDocumentSeriesItem) )
		
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, O.IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol,
	L.IdLogKind as [Type], LogDate, CAST(O.[Type] AS VARCHAR(256)) AS LogType, O.[Description] AS LogDescription, O.SystemUser AS [User], O.Severity,
	CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[MessageLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Posta Elettronica')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Posta Elettronica') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IDMessage = ISNULL(@Riferimento1, O.IDMessage) )

	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, O.IDMail AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol,
	L.IdLogKind as [Type], O.[Date] AS LogDate, O.[Type] AS LogType, O.[Description] As LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[LogKind] L
	INNER JOIN [dbo].[PECMailLog] O ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'PEC')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'PEC') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[Date] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IDMail = ISNULL(@Riferimento1, O.IDMail) )
	
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, O.UniqueIdProtocol,
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)) COLLATE SQL_Latin1_General_CP1_CI_AS, O.[LogDescription] COLLATE SQL_Latin1_General_CP1_CI_AS, O.SystemUser COLLATE SQL_Latin1_General_CP1_CI_AS AS [User], O.Severity,
	CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[ProtocolLog] O 
    INNER JOIN [dbo].[Protocol] P ON P.UniqueId = O.UniqueIdProtocol
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo')
	WHERE P.IdTenantAOO = @IdTenantAOO
    AND (@IdTipologia IS NULL OR @IdTipologia = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo'))
	AND L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo')
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND ( O.[Year] = ISNULL(@Riferimento1, O.[Year]) AND O.[Number] = ISNULL(@Riferimento2, O.[Number]))
 
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, O.IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol,
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[ResolutionLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Atti')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Atti') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IdResolution = ISNULL(@Riferimento1, O.IdResolution) )

	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	[Year] AS DocmYear, Number AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol,
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[DocumentLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Pratiche')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Pratiche') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.[Year] = ISNULL(@Riferimento1, O.[Year]) AND O.[Number] = ISNULL(@Riferimento2, O.[Number]) )

	UNION

	SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, UniqueIdProtocol,
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.[User] AS [User], O.Severity, UDSId, IdUDSRepository
	FROM (
		SELECT O.IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
		CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol,
		L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
		FROM [dbo].[CollaborationLog] O
		INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma')
		WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma') = @IdTipologia)
		AND O.SystemUser = @User
		AND O.LogDate BETWEEN @DataDal AND @DataAl	
		AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
		AND ( O.IdCollaboration = ISNULL(@Riferimento1, O.IdCollaboration) )
	)O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma') = @IdTipologia)
	AND O.[User] = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)	
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
PRINT 'ALTER SQL Function [dbo].[GetDiarioUnificatoDetailsFiltered]'
GO

ALTER FUNCTION [dbo].[GetDiarioUnificatoDetailsFiltered](@IdTipologia INT, @DataDal DATETIME,  @DataAl DATETIME, @User NVARCHAR(256), @Riferimento1 AS INT = NULL, @Riferimento2 AS INT = NULL, @Subject NVARCHAR(MAX), @Riferimento3 AS UNIQUEIDENTIFIER = NULL, @IdTenantAOO AS UNIQUEIDENTIFIER)
RETURNS TABLE 
AS
RETURN 
(
	SELECT O.IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol, 
	L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[CollaborationLog] O
	INNER JOIN [dbo].[Collaboration] E ON E.IdCollaboration = O.IdCollaboration 
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Collaborazione')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Collaborazione') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.LogDate BETWEEN @DataDal AND @DataAl	
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[Object] LIKE '%'+@Subject+'%'
	AND ( O.IdCollaboration = ISNULL(@Riferimento1, O.IdCollaboration) )
		
	UNION 

	SELECT CAST(NULL AS INT) AS IdCollaboration, O.IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol, 
	L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[DocumentSeriesItemLog] O
	INNER JOIN [dbo].[DocumentSeriesItem] E ON E.Id = O.IdDocumentSeriesItem
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Serie Documentali/Archivi')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Serie Documentali/Archivi') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.LogDate BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[Subject] LIKE '%'+@Subject+'%'
	AND ( O.IdDocumentSeriesItem = ISNULL(@Riferimento1, O.IdDocumentSeriesItem) )
		
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, O.IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol, 
	L.IdLogKind as [Type], LogDate, CAST(O.[Type] AS VARCHAR(256)) AS LogType, O.[Description] AS LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[MessageLog] O
	INNER JOIN [dbo].[Message] E ON E.IDMessage = O.IDMessage 
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Posta Elettronica')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Posta Elettronica') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND (SELECT TOP 1 M.Subject FROM [dbo].[MessageEmail] M WHERE M.IDMessage = O.IDMessage) LIKE '%'+@Subject+'%'
	AND ( O.IDMessage = ISNULL(@Riferimento1, O.IDMessage) )

	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, O.IDMail AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol, 
	L.IdLogKind as [Type], O.[Date] AS LogDate, O.[Type] AS LogType, O.[Description] As LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[LogKind] L
	INNER JOIN [dbo].[PECMailLog] O ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'PEC')
	INNER JOIN [dbo].[PECMail] E ON E.IDPECMail = O.IDMail
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'PEC') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[Date] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[MailSubject] LIKE '%'+@Subject+'%'
	AND ( O.IDMail = ISNULL(@Riferimento1, O.IDMail) )
	
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, O.[UniqueIdProtocol],
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)) COLLATE SQL_Latin1_General_CP1_CI_AS, O.[LogDescription] COLLATE SQL_Latin1_General_CP1_CI_AS, O.SystemUser COLLATE SQL_Latin1_General_CP1_CI_AS AS [User], O.Severity,
	CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL aS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[ProtocolLog] O 
	INNER JOIN [dbo].[Protocol] E ON E.[Year] = O.[Year] AND E.[Number] = O.[Number]
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo')
	WHERE E.IdTenantAOO = @IdTenantAOO 
    AND (@IdTipologia IS NULL OR @IdTipologia = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo'))
	AND L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo')
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND E.[Object] LIKE '%'+@Subject+'%'
	AND ( O.[Year] = ISNULL(@Riferimento1, O.[Year]) AND O.[Number] = ISNULL(@Riferimento2, O.[Number]))
 
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, O.IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol, 
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[ResolutionLog] O
	INNER JOIN [dbo].[Resolution] E ON E.idResolution = O.idResolution 
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Atti')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Atti') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[Object] LIKE '%'+@Subject+'%'
	AND ( O.IdResolution = ISNULL(@Riferimento1, O.IdResolution) )

	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	O.[Year] AS DocmYear, O.Number AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol, 
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL aS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[DocumentLog] O
	INNER JOIN [dbo].[Document] E ON E.[Year] = O.[Year] AND E.[Number] = O.[Number]
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Pratiche')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Pratiche') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[Object] LIKE '%'+@Subject+'%'
	AND ( O.[Year] = ISNULL(@Riferimento1, O.[Year]) AND O.[Number] = ISNULL(@Riferimento2, O.[Number]) )

	UNION

	SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, UniqueIdProtocol, 
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.[User] AS [User], O.Severity, UDSId, IdUDSRepository
	FROM (
		SELECT O.IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
		CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS [uniqueidentifier]) AS UniqueIdProtocol, 
		L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL aS UNIQUEIDENTIFIER) IdUDSRepository
		FROM [dbo].[CollaborationLog] O
		INNER JOIN [dbo].[Collaboration] E ON E.IdCollaboration = O.IdCollaboration 
		INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma')
		WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma') = @IdTipologia)
		AND O.SystemUser = @User
		AND O.LogDate BETWEEN @DataDal AND @DataAl	
		AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
		AND E.[Object] LIKE '%'+@Subject+'%'
		AND ( O.IdCollaboration = ISNULL(@Riferimento1, O.IdCollaboration) )
	)O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma') = @IdTipologia)
	AND O.[User] = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)	
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
PRINT 'ALTER SQL Function [dbo].[GetDiarioUnificatoTestata]'
GO

ALTER PROCEDURE [dbo].[GetDiarioUnificatoTestata](@IdTipologia INT = NULL, @DataDal DATETIME,  @DataAl DATETIME, @User NVARCHAR(256), @Subject NVARCHAR(MAX) = NULL, @IdTenantAOO UNIQUEIDENTIFIER)
AS
BEGIN
	SET NOCOUNT ON;
	IF @Subject IS NULL 
		BEGIN 
			-- Testate
			SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, UniqueIdProtocol,
			[Type],LogDate,LogType,LogDescription,[User],Severity, UDSId, IdUDSRepository, ROW_NUMBER() OVER(ORDER BY [LogDate] DESC, [Type]) AS [Id]
			FROM 
			(
				SELECT *, ROW_NUMBER() OVER(PARTITION BY [Type], IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, UniqueIdProtocol, UDSId, IdUDSRepository ORDER BY LogDate DESC ) AS [Ranking]
				FROM dbo.GetDiarioUnificatoDetails(@IdTipologia,@DataDal,@DataAl,@User, NULL, NULL, NULL, @IdTenantAOO)
			)A	
			WHERE  A.Ranking = 1
		END
	ELSE
		BEGIN
			-- Testate
		SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, UniqueIdProtocol,
		[Type],LogDate,LogType,LogDescription,[User],Severity, UDSId, IdUDSRepository, ROW_NUMBER() OVER(ORDER BY [LogDate] DESC, [Type]) AS [Id]
		FROM 
		(
			SELECT *, ROW_NUMBER() OVER(PARTITION BY [Type], IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, UniqueIdProtocol, UDSId, IdUDSRepository ORDER BY LogDate DESC ) AS [Ranking]
			FROM dbo.GetDiarioUnificatoDetailsFiltered(@IdTipologia,@DataDal,@DataAl,@User, NULL, NULL, @Subject, null, @IdTenantAOO)
		)A
		WHERE A.Ranking = 1

		END
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
PRINT 'ALTER SQL Function [dbo].[GetDiarioUnificatoDettaglio]'
GO

ALTER PROCEDURE [dbo].[GetDiarioUnificatoDettaglio](@IdTipologia INT, @DataDal DATETIME,  @DataAl DATETIME, @User NVARCHAR(256), @Riferimento1 AS INT = NULL, @Riferimento2 AS INT = NULL, @Riferimento3 AS UNIQUEIDENTIFIER = NULL, @IdTenantAOO AS UNIQUEIDENTIFIER)
AS
BEGIN
	SET NOCOUNT ON;

	-- Dettaglio
	SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear,DocmNumber,UniqueIdProtocol,
	[Type],LogDate,LogType,LogDescription,[User],Severity, UDSId, IdUDSRepository, ROW_NUMBER() OVER(ORDER BY [LogDate] DESC) AS [Id]
	FROM dbo.GetDiarioUnificatoDetails(@IdTipologia,@DataDal,@DataAl,@User, @Riferimento1, @Riferimento2, @Riferimento3, @IdTenantAOO)
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
PRINT 'ALTER TABLE [dbo].[PosteOnLineRequest] ADD [ExtendedProperties]'
GO
ALTER TABLE [dbo].[PosteOnLineRequest] ADD ExtendedProperties nvarchar(max) NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'ALTER TABLE [dbo].[PosteOnLineRequest] ADD [Timestamp]'
GO
ALTER TABLE PosteOnLineRequest ADD  [Timestamp] timestamp
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'ALTER TABLE [dbo].[PosteOnLineRequest] ALTER COLUMN [RegistrationDate] datetimeoffset'
GO
ALTER TABLE [dbo].[PosteOnLineRequest] ALTER COLUMN RegistrationDate datetimeoffset NOT NULL
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'ALTER TABLE [dbo].[PosteOnLineRequest] ALTER COLUMN [LastChangedDate] datetimeoffset'
GO
ALTER TABLE [dbo].[PosteOnLineRequest] ALTER COLUMN LastChangedDate datetimeoffset NULL
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'ALTER TABLE [dbo].[PosteOnLineContact] ALTER COLUMN [RegistrationDate] datetimeoffset'
GO
ALTER TABLE [dbo].[PosteOnLineContact] ALTER COLUMN RegistrationDate datetimeoffset NOT NULL
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'ALTER TABLE [dbo].[PosteOnLineContact] ALTER COLUMN [LastChangedDate] datetimeoffset'
GO
ALTER TABLE [dbo].[PosteOnLineContact] ALTER COLUMN LastChangedDate datetimeoffset NULL
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'CREATE UNIQUE INDEX [UX_TenantContacts_IdTenant_EntityShortId] ON [dbo].[TenantContacts]'
GO
CREATE UNIQUE INDEX [UX_TenantContacts_IdTenant_EntityShortId] ON [dbo].[TenantContacts]
(
	[IdTenant] ASC,
	[EntityId] ASC
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
PRINT N'Creazione Tabella TenantAOO'
GO

CREATE TABLE [dbo].[TenantAOO](
       [IdTenantAOO] [uniqueidentifier] NOT NULL,
       [Name] [nvarchar](256) NOT NULL,
       [Note] [nvarchar](4000) NULL,
	   [CategorySuffix] [nvarchar](20) NULL,
       [RegistrationUser] [nvarchar](256) NOT NULL,
       [RegistrationDate] [datetimeoffset](7) NOT NULL,
       [LastChangedDate] [datetimeoffset](7) NULL,
       [LastChangedUser] [nvarchar](256) NULL,
       [Timestamp] [timestamp] NOT NULL,

CONSTRAINT [PK_TenantAOO] PRIMARY KEY NONCLUSTERED
(
       [IdTenantAOO] ASC

)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

) ON [PRIMARY]

GO

CREATE CLUSTERED INDEX [IX_TenantAOO_RegistrationDate] 
ON [dbo].[TenantAOO] ([RegistrationDate] ASC)

GO
ALTER TABLE [dbo].[TenantAOO] ADD  CONSTRAINT [UX_TenantAOO_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
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
PRINT 'Add column IdTenant into table [dbo].[WorkflowActivities]'
GO

ALTER TABLE [dbo].[WorkflowActivities]
ADD IdTenant UNIQUEIDENTIFIER NULL;
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--################################################################################
PRINT N'Add column IdTenantAOO into table [dbo].[Category]';
GO

ALTER TABLE [dbo].[Category] 
ADD IdTenantAOO UNIQUEIDENTIFIER NULL;
GO

UPDATE [dbo].[Category] SET IdTenantAOO = NEWID();
GO

ALTER TABLE [dbo].[Category]
ALTER COLUMN IdTenantAOO UNIQUEIDENTIFIER NOT NULL;
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################

PRINT N'ALTER TABLE [dbo].[WorkflowRoles]'
GO
ALTER TABLE [dbo].[WorkflowRoles] DROP CONSTRAINT [PK_WorkflowRoles]
GO

ALTER TABLE [dbo].[WorkflowRoles] ADD  CONSTRAINT [PK_WorkflowRoles]  DEFAULT (newid()) FOR [IdWorkflowRole]
GO

ALTER TABLE [dbo].[WorkflowRoles] DROP CONSTRAINT [DF_WorkflowRoles_RegistrationDate]
GO

ALTER TABLE [dbo].[WorkflowRoles] ADD  CONSTRAINT [DF_WorkflowRoles_RegistrationDate] DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]
GO

ALTER TABLE [dbo].[WorkflowRoles] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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
