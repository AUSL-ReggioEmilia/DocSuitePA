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
PRINT 'CREATE Stored Procedure SQL Storage_SP_InitializeSQL2014Storage';
GO

CREATE PROCEDURE [dbo].[Storage_SP_InitializeSQL2014Storage] 
	@StorageName nvarchar(256) = '',
	@StorageBasePath nvarchar(256) = ''
AS
BEGIN
	SET XACT_ABORT ON
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	DECLARE @FileTableSearchableDocumentsName nvarchar(256)
	DECLARE @FileTableAttributesName nvarchar(256)
	DECLARE @TableDocumentStorageMapName nvarchar(256)
	DECLARE @FileGroupAttributesName nvarchar(256)
	DECLARE @FileGroupSearchableDocumentsName nvarchar(256)
	DECLARE @CurrentDBName nvarchar(256)
	DECLARE @StorageFullTextCatalogName nvarchar(256)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF (@StorageName IS NULL OR @StorageName = '')
		THROW 50000, 'StorageName is null or empty', 1

	IF (@StorageBasePath IS NULL OR @StorageBasePath = '')
		THROW 50000, 'StorageBasePath is null or empty', 1
	
	SET @FileTableSearchableDocumentsName = 'ft_'+@StorageName+'_searchable_documents'
	SET @FileTableAttributesName = 'ft_'+@StorageName+'_searchable_attributes'
	SET @TableDocumentStorageMapName = 'Document'+@StorageName+'Maps'
	SET @FileGroupAttributesName = 'Biblos_FG_'+@StorageName+'_Searchable_Attributes'
	SET @FileGroupSearchableDocumentsName = 'Biblos_FG_'+@StorageName+'_SearchableDocuments'
	SELECT @CurrentDBName = DB_NAME()
	SET @StorageFullTextCatalogName = 'Biblos_FTCAT_'+@StorageName

	-- ADD FileGroup for Attributes FileTable
	IF NOT EXISTS (SELECT * FROM sys.filegroups WHERE name=@FileGroupAttributesName)
		exec ('ALTER DATABASE '+@CurrentDBName+' ADD FILEGROUP '+@FileGroupAttributesName+' CONTAINS FILESTREAM')

	IF NOT EXISTS (SELECT * FROM sys.database_files WHERE type=2 AND name=@FileGroupAttributesName)
		exec ('ALTER DATABASE '+@CurrentDBName+' ADD FILE (NAME = '''+@FileGroupAttributesName+''', FILENAME = '''+@StorageBasePath+'\'+@FileGroupAttributesName+''') TO FILEGROUP '+@FileGroupAttributesName)

	-- ADD FileGroup for Extracted Documents FileTable
	IF NOT EXISTS (SELECT * FROM sys.filegroups WHERE name=@FileGroupSearchableDocumentsName)
		exec ('ALTER DATABASE '+@CurrentDBName+' ADD FILEGROUP '+@FileGroupSearchableDocumentsName+' CONTAINS FILESTREAM')

	IF NOT EXISTS (SELECT * FROM sys.database_files WHERE type=2 AND name=@FileGroupSearchableDocumentsName)
		exec ('ALTER DATABASE '+@CurrentDBName+' ADD FILE (NAME = '''+@FileGroupSearchableDocumentsName+''', FILENAME = '''+@StorageBasePath+'\'+@FileGroupSearchableDocumentsName+''') TO FILEGROUP '+@FileGroupSearchableDocumentsName)

	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE name=@StorageFullTextCatalogName)
		exec ('CREATE FULLTEXT CATALOG '+@StorageFullTextCatalogName)

	BEGIN TRAN
	BEGIN TRY				
		-- CREATE FileTables
		IF NOT EXISTS(SELECT 1 FROM sys.Tables WHERE  Name = @FileTableSearchableDocumentsName AND Type = N'U')
		BEGIN
			exec ('CREATE TABLE '+@FileTableSearchableDocumentsName+' AS FILETABLE FILESTREAM_ON '+@FileGroupSearchableDocumentsName)
			exec ('CREATE UNIQUE INDEX UQ_StreamId ON '+@FileTableSearchableDocumentsName+'(stream_id);')
		END		

		IF NOT EXISTS(SELECT 1 FROM sys.Tables WHERE  Name = @FileTableAttributesName AND Type = N'U')
		BEGIN
			exec ('CREATE TABLE '+@FileTableAttributesName+' AS FILETABLE FILESTREAM_ON '+@FileGroupAttributesName)
			exec ('CREATE UNIQUE INDEX UQ_StreamId ON '+@FileTableAttributesName+'(stream_id);')		
		END			

		IF NOT EXISTS(SELECT 1 FROM sys.Tables WHERE  Name = @TableDocumentStorageMapName AND Type = N'U')
		BEGIN
			exec ('CREATE TABLE '+@TableDocumentStorageMapName+'(
					Id [uniqueidentifier] NOT NULL,
					IdDocument [uniqueidentifier] NOT NULL,
					IdChain [uniqueidentifier] NOT NULL,
					IdFileTableDocument [uniqueidentifier] NOT NULL,
					DocumentType [int] NOT NULL,
					RegistrationDate [datetimeoffset](7) NOT NULL
				 CONSTRAINT [PK_'+@TableDocumentStorageMapName+'] PRIMARY KEY NONCLUSTERED 
				(
					Id ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				) ON [PRIMARY]');

			exec ('ALTER TABLE '+@TableDocumentStorageMapName+' ADD  CONSTRAINT [DF_'+@TableDocumentStorageMapName+'_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]')

			exec ('CREATE CLUSTERED INDEX [IX_'+@TableDocumentStorageMapName+'_RegistrationDate] ON '+@TableDocumentStorageMapName+'
					(
						[RegistrationDate] ASC
					)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]')
		END
		COMMIT		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;			
		THROW	
	END CATCH

	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes where object_id = object_id(@FileTableSearchableDocumentsName))
	BEGIN
		--E' possibile creare fulltext index solo fuori dalla transazione
		exec ('CREATE FULLTEXT INDEX ON '+@FileTableSearchableDocumentsName+'(file_stream TYPE COLUMN file_type Language 1040)
			KEY INDEX UQ_StreamId 
			ON '+@StorageFullTextCatalogName)
	END

	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes where object_id = object_id(@FileTableAttributesName))
	BEGIN
		exec ('CREATE FULLTEXT INDEX ON '+@FileTableAttributesName+'(file_stream TYPE COLUMN file_type Language 1040)
			KEY INDEX UQ_StreamId 
			ON '+@StorageFullTextCatalogName)
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
PRINT 'CREATE Stored Procedure SQL Storage_SP_InitializeFullTextStorage';
GO

CREATE PROCEDURE [dbo].[Storage_SP_InitializeFullTextStorage] 
	@StorageName nvarchar(256) = '',
	@StorageBasePath nvarchar(256) = ''
AS
BEGIN
	SET XACT_ABORT ON
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	DECLARE @FileTableSearchableDocumentsName nvarchar(256)
	DECLARE @FileTableAttributesName nvarchar(256)
	DECLARE @TableDocumentStorageMapName nvarchar(256)
	DECLARE @FileGroupAttributesName nvarchar(256)
	DECLARE @FileGroupSearchableDocumentsName nvarchar(256)
	DECLARE @CurrentDBName nvarchar(256)
	DECLARE @StorageFullTextCatalogName nvarchar(256)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF (@StorageName IS NULL OR @StorageName = '')
		THROW 50000, 'StorageName is null or empty', 1

	IF (@StorageBasePath IS NULL OR @StorageBasePath = '')
		THROW 50000, 'StorageBasePath is null or empty', 1
	
	SET @FileTableSearchableDocumentsName = 'ft_'+@StorageName+'_searchable_documents'
	SET @FileTableAttributesName = 'ft_'+@StorageName+'_searchable_attributes'
	SET @TableDocumentStorageMapName = 'Document'+@StorageName+'Maps'
	SET @FileGroupAttributesName = 'Biblos_FG_'+@StorageName+'_SearchableAttributes'
	SET @FileGroupSearchableDocumentsName = 'Biblos_FG_'+@StorageName+'_SearchableDocuments'
	SELECT @CurrentDBName = DB_NAME()
	SET @StorageFullTextCatalogName = 'Biblos_FTCAT_'+@StorageName	

	-- ADD FileGroup for Attributes FileTable

	IF NOT EXISTS (SELECT * FROM sys.filegroups WHERE name=@FileGroupAttributesName)
		exec ('ALTER DATABASE '+@CurrentDBName+' ADD FILEGROUP '+@FileGroupAttributesName+' CONTAINS FILESTREAM')

	IF NOT EXISTS (SELECT * FROM sys.database_files WHERE type=2 AND name=@FileGroupAttributesName)
		exec ('ALTER DATABASE '+@CurrentDBName+' ADD FILE (NAME = '''+@FileGroupAttributesName+''', FILENAME = '''+@StorageBasePath+'\'+@FileGroupAttributesName+''') TO FILEGROUP '+@FileGroupAttributesName)

	-- ADD FileGroup for Extracted Documents FileTable
	IF NOT EXISTS (SELECT * FROM sys.filegroups WHERE name=@FileGroupSearchableDocumentsName)
		exec ('ALTER DATABASE '+@CurrentDBName+' ADD FILEGROUP '+@FileGroupSearchableDocumentsName+' CONTAINS FILESTREAM')

	IF NOT EXISTS (SELECT * FROM sys.database_files WHERE type=2 AND name=@FileGroupSearchableDocumentsName)
		exec ('ALTER DATABASE '+@CurrentDBName+' ADD FILE (NAME = '''+@FileGroupSearchableDocumentsName+''', FILENAME = '''+@StorageBasePath+'\'+@FileGroupSearchableDocumentsName+''') TO FILEGROUP '+@FileGroupSearchableDocumentsName)

	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE name=@StorageFullTextCatalogName)
		exec ('CREATE FULLTEXT CATALOG '+@StorageFullTextCatalogName)

	BEGIN TRAN
	BEGIN TRY				
		-- CREATE FileTables
		IF NOT EXISTS(SELECT 1 FROM sys.Tables WHERE  Name = @FileTableSearchableDocumentsName AND Type = N'U')
		BEGIN
			exec ('CREATE TABLE '+@FileTableSearchableDocumentsName+' AS FILETABLE FILESTREAM_ON '+@FileGroupSearchableDocumentsName)
			exec ('CREATE UNIQUE INDEX UQ_StreamId ON '+@FileTableSearchableDocumentsName+'(stream_id);')
		END		

		IF NOT EXISTS(SELECT 1 FROM sys.Tables WHERE  Name = @FileTableAttributesName AND Type = N'U')
		BEGIN
			exec ('CREATE TABLE '+@FileTableAttributesName+' AS FILETABLE FILESTREAM_ON '+@FileGroupAttributesName)
			exec ('CREATE UNIQUE INDEX UQ_StreamId ON '+@FileTableAttributesName+'(stream_id);')		
		END			

		IF NOT EXISTS(SELECT 1 FROM sys.Tables WHERE  Name = @TableDocumentStorageMapName AND Type = N'U')
		BEGIN
			exec ('CREATE TABLE '+@TableDocumentStorageMapName+'(
					Id [uniqueidentifier] NOT NULL,
					IdDocument [uniqueidentifier] NOT NULL,
					IdChain [uniqueidentifier] NOT NULL,
					IdFileTableDocument [uniqueidentifier] NOT NULL,
					DocumentType [int] NOT NULL,
					RegistrationDate [datetimeoffset](7) NOT NULL
				 CONSTRAINT [PK_'+@TableDocumentStorageMapName+'] PRIMARY KEY NONCLUSTERED 
				(
					Id ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				) ON [PRIMARY]');

			exec ('ALTER TABLE '+@TableDocumentStorageMapName+' ADD  CONSTRAINT [DF_'+@TableDocumentStorageMapName+'_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]')

			exec ('CREATE CLUSTERED INDEX [IX_'+@TableDocumentStorageMapName+'_RegistrationDate] ON '+@TableDocumentStorageMapName+'
					(
						[RegistrationDate] ASC
					)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]')
		END
		COMMIT		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;			
		THROW	
	END CATCH

	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes where object_id = object_id(@FileTableSearchableDocumentsName))
	BEGIN
		--E' possibile creare fulltext index solo fuori dalla transazione
		exec ('CREATE FULLTEXT INDEX ON '+@FileTableSearchableDocumentsName+'(file_stream TYPE COLUMN file_type Language 1040)
			KEY INDEX UQ_StreamId 
			ON '+@StorageFullTextCatalogName)
	END

	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes where object_id = object_id(@FileTableAttributesName))
	BEGIN
		exec ('CREATE FULLTEXT INDEX ON '+@FileTableAttributesName+'(file_stream TYPE COLUMN file_type Language 1040)
			KEY INDEX UQ_StreamId 
			ON '+@StorageFullTextCatalogName)
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
PRINT 'CREATE Stored Procedure SQL Storage_SP_InsertDocumentToStorage';
GO

CREATE PROCEDURE [dbo].[Storage_SP_InsertDocumentToStorage]
	@StorageName nvarchar(256) = '',
	@Name nvarchar(256) = '',
	@Content varbinary(MAX) = NULL,
	@StorageAreaName nvarchar(256) = '',
	@IdDocument uniqueidentifier = NULL,
	@IdChain uniqueidentifier = NULL,
	@DocumentType int = NULL
AS
BEGIN
	SET XACT_ABORT ON
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	DECLARE @path HIERARCHYID
	DECLARE @newpath HIERARCHYID
	DECLARE @CurrentDBName nvarchar(256)
	SET NOCOUNT ON;

	SELECT @CurrentDBName = DB_NAME()+'_FullText'
    IF (@StorageName IS NULL OR @StorageName = '')
		THROW 50000, 'StorageName is null or empty', 1

	IF (@Name IS NULL OR @Name = '')
		THROW 50000, 'Name is null or empty', 1

	IF (@StorageAreaName IS NULL OR @StorageAreaName = '')
		THROW 50000, 'StorageAreaName is null or empty', 1

	IF (@IdDocument IS NULL OR @IdDocument = '00000000-0000-0000-0000-000000000000')
		THROW 50000, 'IdDocument is null or empty', 1

	IF (@IdChain IS NULL OR @IdChain = '00000000-0000-0000-0000-000000000000')
		THROW 50000, 'IdChain is null or empty', 1

	IF (@DocumentType IS NULL)
		THROW 50000, 'DocumentType is null', 1

	IF (@Content IS NULL)
		SET @Content = CONVERT(varbinary(MAX), '')

	DECLARE @FileTableName nvarchar(256)
	IF (@DocumentType = 1)
		SET @FileTableName = 'ft_'+@StorageName+'_searchable_documents'
	ELSE IF (@DocumentType = 4)
		SET @FileTableName = 'ft_'+@StorageName+'_searchable_attributes'

	BEGIN TRAN
	BEGIN TRY
		DECLARE @sql nvarchar(500) = N'USE'+@CurrentDBName+'; SELECT @path = path_locator FROM '+@FileTableName+' WHERE name = @StorageAreaName'
		exec sp_executesql @sql, N'@StorageAreaName nvarchar(256), @path HIERARCHYID OUTPUT', @StorageAreaName, @path=@path OUTPUT
		IF (@path IS NULL)
		BEGIN
			exec ('USE'+@CurrentDBName+'; INSERT INTO '+@FileTableName+'(name, is_directory, is_archive) VALUES ('''+@StorageAreaName+''', 1, 0)')
			exec sp_executesql @sql, N'@StorageAreaName nvarchar(256), @path HIERARCHYID OUTPUT', @StorageAreaName, @path=@path OUTPUT
		END

		SELECT @newpath = convert(HIERARCHYID, @path.ToString()     +
		CONVERT(VARCHAR(20), CONVERT(BIGINT, SUBSTRING(CONVERT(BINARY(16), NEWID()), 1, 6))) + '.' +
		CONVERT(VARCHAR(20), CONVERT(BIGINT, SUBSTRING(CONVERT(BINARY(16), NEWID()), 7, 6))) + '.' +
		CONVERT(VARCHAR(20), CONVERT(BIGINT, SUBSTRING(CONVERT(BINARY(16), NEWID()), 13, 4))) + '/');
	
		DECLARE @docId uniqueidentifier = NEWID()
		DECLARE @sql_insert nvarchar(500) = N'USE'+@CurrentDBName+'; INSERT INTO '+@FileTableName+'(stream_id, name, file_stream, path_locator) VALUES (@DocId, @Name, @Content, @newpath)'
		exec sp_executesql @sql_insert, N'@DocId uniqueidentifier, @Name nvarchar(256), @Content varbinary(max), @newpath HIERARCHYID', @DocId, @Name, @Content, @newpath

		DECLARE @sql_exist_document nvarchar(500) = N'USE'+@CurrentDBName+'; SELECT @row_count = COUNT(*) FROM Document'+@StorageName+'Maps WHERE IdDocument = @IdDocument AND DocumentType = @DocumentType'
		DECLARE @row_count INT
		exec sp_executesql @sql_exist_document, N'@IdDocument uniqueidentifier, @DocumentType int, @row_count INT OUTPUT', @IdDocument, @DocumentType, @row_count=@row_count OUTPUT

		DECLARE @sql_manage_document nvarchar(500)
		IF (@row_count = 0)
		BEGIN
			DECLARE @tmp_id uniqueidentifier = NEWID()
			SET @sql_manage_document = N'USE'+@CurrentDBName+'; INSERT INTO Document'+@StorageName+'Maps(Id, IdDocument, IdChain, IdFileTableDocument, DocumentType) VALUES (@tmp_id, @IdDocument, @IdChain, @DocId, @DocumentType)'
			exec sp_executesql @sql_manage_document, N'@tmp_id uniqueidentifier, @DocId uniqueidentifier, @IdDocument uniqueidentifier, @IdChain uniqueidentifier, @DocumentType int', @tmp_id, @docId, @IdDocument, @IdChain, @DocumentType
		END
		ELSE
		BEGIN
			SET @sql_manage_document = N'USE'+@CurrentDBName+'; UPDATE Document'+@StorageName+'Maps SET IdFileTableDocument = @DocId WHERE IdDocument = @IdDocument AND DocumentType = @DocumentType'
			exec sp_executesql @sql_manage_document, N'@DocId uniqueidentifier, @IdDocument uniqueidentifier, @IdChain uniqueidentifier, @DocumentType int', @docId, @IdDocument, @IdChain, @DocumentType
		END
		COMMIT		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;			
		THROW	
	END CATCH
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
PRINT 'CREATE Stored Procedure SQL Storage_SP_DeleteDocumentFromStorage';
GO

CREATE PROCEDURE [dbo].[Storage_SP_DeleteDocumentFromStorage]
	@StorageName nvarchar(256) = '',
	@IdDocument uniqueidentifier = NULL
AS
BEGIN
	SET XACT_ABORT ON
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	DECLARE @FileTableSearchableDocumentsName nvarchar(256)
	DECLARE @FileTableAttributesName nvarchar(256)
	DECLARE @TableDocumentStorageMapName nvarchar(256)
	SET NOCOUNT ON;

    IF (@StorageName IS NULL OR @StorageName = '')
		THROW 50000, 'StorageName is null or empty', 1

	IF (@IdDocument IS NULL OR @IdDocument = '00000000-0000-0000-0000-000000000000')
		THROW 50000, 'IdDocument is null or empty', 1

	SET @FileTableSearchableDocumentsName = 'ft_'+@StorageName+'_searchable_documents'
	SET @FileTableAttributesName = 'ft_'+@StorageName+'_searchable_attributes'
	SET @TableDocumentStorageMapName = 'Document'+@StorageName+'Maps'

	DECLARE	@idFileTableDocument uniqueidentifier
	DECLARE @documentType int
	DECLARE @my_cursor CURSOR

	BEGIN TRAN
	BEGIN TRY		

		DECLARE @tmp_sql nvarchar(500) = N'SET @my_cursor = CURSOR FAST_FORWARD
               FOR
			   SELECT IdFileTableDocument, DocumentType FROM '+@TableDocumentStorageMapName+' WHERE IdDocument = @IdDocument
			   OPEN @my_cursor'

		exec sp_executesql @tmp_sql, N'@IdDocument uniqueidentifier, @my_cursor CURSOR OUTPUT', @IdDocument, @my_cursor OUTPUT

		FETCH NEXT FROM @my_cursor INTO @idFileTableDocument, @documentType
		WHILE @@FETCH_STATUS = 0   
		BEGIN  
			IF (@documentType = 1)
				exec ('DELETE FROM '+@FileTableSearchableDocumentsName+' WHERE stream_id = '''+@idFileTableDocument+'''');
			ELSE IF (@documentType = 4)
				exec ('DELETE FROM '+@FileTableAttributesName+' WHERE stream_id = '''+@idFileTableDocument+'''');

			FETCH NEXT FROM @my_cursor INTO @idFileTableDocument, @documentType
		END  

		CLOSE @my_cursor
		DEALLOCATE @my_cursor
		
		exec ('DELETE FROM '+@TableDocumentStorageMapName+' WHERE IdDocument = '''+@IdDocument+'''')
		COMMIT		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		BEGIN TRY
			--clean it up               
			CLOSE @my_cursor
			DEALLOCATE @my_cursor                                  
		END TRY
		BEGIN CATCH
			print 'cursor clean up error'
		END CATCH;
		THROW	
	END CATCH
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
PRINT 'CREATE Stored Procedure SQL SQL2014Storage_SP_DocumentById';
GO

CREATE PROCEDURE [dbo].[SQL2014Storage_SP_DocumentById] 
	@StorageName nvarchar(256),
	@IdDocument uniqueidentifier,
	@DocumentType int = NULL
AS
BEGIN	
	DECLARE @TableDocumentStorageMapName nvarchar(256)
	SET NOCOUNT ON;	

	IF (@StorageName IS NULL OR @StorageName = '')
		THROW 50000, 'StorageName is null or empty', 1

	IF (@DocumentType IS NULL)
		THROW 50000, 'DocumentType is null', 1

	DECLARE @FileTableName nvarchar(256)
	IF (@DocumentType = 1)
		SET @FileTableName = 'ft_'+@StorageName+'_searchable_documents'
	ELSE IF (@DocumentType = 4)
		SET @FileTableName = 'ft_'+@StorageName+'_searchable_attributes'

	SET @TableDocumentStorageMapName = 'Document'+@StorageName+'Maps'

	DECLARE @document_id uniqueidentifier
	DECLARE @sql_find nvarchar(500) = 'SELECT @document_id = IdFileTableDocument FROM '+@TableDocumentStorageMapName+' WHERE IdDocument = @IdDocument AND DocumentType = @DocumentType'
	exec sp_executesql @sql_find, N'@IdDocument uniqueidentifier, @DocumentType int, @document_id uniqueidentifier OUTPUT', @IdDocument, @DocumentType, @document_id = @document_id OUTPUT
	
    
	DECLARE @tsql nvarchar(MAX)
	DECLARE @results table([stream_id] uniqueidentifier
      ,[file_stream] varbinary(max)
      ,[name] nvarchar(255)
	  ,[path] nvarchar(2000))
	  
	DECLARE @tmp_sql nvarchar(500) = N'SELECT stream_id, file_stream, name, file_stream.PathName() FROM '+@FileTableName+' WHERE stream_id = '''+CONVERT(nvarchar(256), @document_id)+''' AND is_directory = 0'	
	INSERT INTO @results exec sp_executesql @tmp_sql
	SELECT TOP 1 * FROM @results
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
PRINT 'CREATE Stored Procedure SQL SQL2014Storage_SP_DocumentsByName';
GO

--TODO: DA RIVEDERE PERCHE NON ESISTE PIU UNA FILE TABLE DOCUMENTS

--CREATE PROCEDURE [dbo].[SQL2014Storage_SP_DocumentsByName] 
--	@StorageName nvarchar(256),
--	@StorageAreaName nvarchar(256),
--	@Name nvarchar(256)
--AS
--BEGIN	
--	DECLARE @FileTableDocumentsName nvarchar(256)
--	SET NOCOUNT ON;
	
--	IF (@StorageName IS NULL OR @StorageName = '')
--		THROW 50000, 'StorageName is null or empty', 1

--	IF (@Name IS NULL OR @Name = '')
--		THROW 50000, 'Name is null or empty', 1

--	SET @FileTableDocumentsName = 'ft_'+@StorageName+'_documents'

--	DECLARE @path_locator nvarchar(2000)
--	DECLARE @sql_find_folder nvarchar(500) = 'SELECT @path_locator = CONVERT(nvarchar(2000), path_locator) FROM '+@FileTableDocumentsName+' WHERE name = @StorageAreaName AND is_directory = 1'
--	exec sp_executesql @sql_find_folder, N'@StorageAreaName nvarchar(256), @path_locator nvarchar(2000) OUTPUT', @StorageAreaName, @path_locator = @path_locator OUTPUT
	
--	IF (@path_locator IS NULL)
--		SET @path_locator = hierarchyid::GetRoot().ToString()
    
--	DECLARE @tsql nvarchar(MAX)
--	DECLARE @results table([stream_id] uniqueidentifier
--      ,[file_stream] varbinary(max)
--      ,[name] nvarchar(255)
--	  ,[path] nvarchar(2000))
	  
--	DECLARE @tmp_sql nvarchar(500) = N'SELECT stream_id, file_stream, name, file_stream.PathName() FROM '+@FileTableDocumentsName+' WHERE name = '''+@Name+''' AND is_directory = 0 AND parent_path_locator = '''+@path_locator+''''	
--	INSERT INTO @results exec sp_executesql @tmp_sql
--	SELECT * FROM @results
--END
--GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'INSERT SQL2014Storage into StorageType table';
GO

INSERT INTO [dbo].[StorageType]([Type], [IdStorageType], [StorageAssembly], [StorageClassName]) VALUES ('Sql2014Server', NEWID(), 'BiblosDS.Library.Storage.SQL', 'SQL2014Storage')
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Create stored procedure Storage_SP_DeleteSearchableDocumentFromStorage';
GO

CREATE PROCEDURE [dbo].[Storage_SP_DeleteSearchableDocumentFromStorage]
	@StorageName nvarchar(256) = '',
	@IdDocument uniqueidentifier = NULL
AS
BEGIN
	SET XACT_ABORT ON
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	DECLARE @CurrentDBName nvarchar(256)
	DECLARE @FileTableSearchableDocumentsName nvarchar(256)
	DECLARE @FileTableAttributesName nvarchar(256)
	DECLARE @TableDocumentStorageMapName nvarchar(256)
	SET NOCOUNT ON;

	SELECT @CurrentDBName = DB_NAME()+'_FullText'
    IF (@StorageName IS NULL OR @StorageName = '')
		THROW 50000, 'StorageName is null or empty', 1

	IF (@IdDocument IS NULL OR @IdDocument = '00000000-0000-0000-0000-000000000000')
		THROW 50000, 'IdDocument is null or empty', 1

	SET @FileTableSearchableDocumentsName = @CurrentDBName+'.dbo.ft_'+@StorageName+'_searchable_documents'
	SET @FileTableAttributesName = @CurrentDBName+'.dbo.ft_'+@StorageName+'_searchable_attributes'
	SET @TableDocumentStorageMapName = @CurrentDBName+'.dbo.Document'+@StorageName+'Maps'

	DECLARE	@idFileTableDocument uniqueidentifier
	DECLARE @documentType int
	DECLARE @my_cursor CURSOR

	BEGIN TRAN
	BEGIN TRY		

		DECLARE @tmp_sql nvarchar(500) = N'SET @my_cursor = CURSOR FAST_FORWARD
               FOR
			   SELECT IdFileTableDocument, DocumentType FROM '+@TableDocumentStorageMapName+' WHERE IdDocument = @IdDocument
			   OPEN @my_cursor'

		exec sp_executesql @tmp_sql, N'@IdDocument uniqueidentifier, @my_cursor CURSOR OUTPUT', @IdDocument, @my_cursor OUTPUT

		FETCH NEXT FROM @my_cursor INTO @idFileTableDocument, @documentType
		WHILE @@FETCH_STATUS = 0   
		BEGIN  
			IF (@documentType = 1)
				exec ('DELETE FROM '+@FileTableSearchableDocumentsName+' WHERE stream_id = '''+@idFileTableDocument+'''');
			ELSE IF (@documentType = 4)
				exec ('DELETE FROM '+@FileTableAttributesName+' WHERE stream_id = '''+@idFileTableDocument+'''');

			FETCH NEXT FROM @my_cursor INTO @idFileTableDocument, @documentType
		END  

		CLOSE @my_cursor
		DEALLOCATE @my_cursor
		
		exec ('DELETE FROM '+@TableDocumentStorageMapName+' WHERE IdDocument = '''+@IdDocument+''' AND DocumentType in (1,4)')
		COMMIT		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		BEGIN TRY
			--clean it up               
			CLOSE @my_cursor
			DEALLOCATE @my_cursor                                  
		END TRY
		BEGIN CATCH
			print 'cursor clean up error'
		END CATCH;
		THROW	
	END CATCH
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