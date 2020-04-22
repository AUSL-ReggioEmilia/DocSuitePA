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
		DECLARE @sql nvarchar(500) = N'USE '+@CurrentDBName+'; SELECT @path = path_locator FROM '+@FileTableName+' WHERE name = @StorageAreaName'
		exec sp_executesql @sql, N'@StorageAreaName nvarchar(256), @path HIERARCHYID OUTPUT', @StorageAreaName, @path=@path OUTPUT
		IF (@path IS NULL)
		BEGIN
			exec ('USE '+@CurrentDBName+'; INSERT INTO '+@FileTableName+'(name, is_directory, is_archive) VALUES ('''+@StorageAreaName+''', 1, 0)')
			exec sp_executesql @sql, N'@StorageAreaName nvarchar(256), @path HIERARCHYID OUTPUT', @StorageAreaName, @path=@path OUTPUT
		END

		SELECT @newpath = convert(HIERARCHYID, @path.ToString()     +
		CONVERT(VARCHAR(20), CONVERT(BIGINT, SUBSTRING(CONVERT(BINARY(16), NEWID()), 1, 6))) + '.' +
		CONVERT(VARCHAR(20), CONVERT(BIGINT, SUBSTRING(CONVERT(BINARY(16), NEWID()), 7, 6))) + '.' +
		CONVERT(VARCHAR(20), CONVERT(BIGINT, SUBSTRING(CONVERT(BINARY(16), NEWID()), 13, 4))) + '/');
	
		DECLARE @docId uniqueidentifier = NEWID()
		DECLARE @sql_insert nvarchar(500) = N'USE '+@CurrentDBName+'; INSERT INTO '+@FileTableName+'(stream_id, name, file_stream, path_locator) VALUES (@DocId, @Name, @Content, @newpath)'
		exec sp_executesql @sql_insert, N'@DocId uniqueidentifier, @Name nvarchar(256), @Content varbinary(max), @newpath HIERARCHYID', @DocId, @Name, @Content, @newpath

		DECLARE @sql_exist_document nvarchar(500) = N'USE '+@CurrentDBName+'; SELECT @row_count = COUNT(*) FROM Document'+@StorageName+'Maps WHERE IdDocument = @IdDocument AND DocumentType = @DocumentType'
		DECLARE @row_count INT
		exec sp_executesql @sql_exist_document, N'@IdDocument uniqueidentifier, @DocumentType int, @row_count INT OUTPUT', @IdDocument, @DocumentType, @row_count=@row_count OUTPUT

		DECLARE @sql_manage_document nvarchar(500)
		IF (@row_count = 0)
		BEGIN
			DECLARE @tmp_id uniqueidentifier = NEWID()
			SET @sql_manage_document = N'USE '+@CurrentDBName+'; INSERT INTO Document'+@StorageName+'Maps(Id, IdDocument, IdChain, IdFileTableDocument, DocumentType) VALUES (@tmp_id, @IdDocument, @IdChain, @DocId, @DocumentType)'
			exec sp_executesql @sql_manage_document, N'@tmp_id uniqueidentifier, @DocId uniqueidentifier, @IdDocument uniqueidentifier, @IdChain uniqueidentifier, @DocumentType int', @tmp_id, @docId, @IdDocument, @IdChain, @DocumentType
		END
		ELSE
		BEGIN
			SET @sql_manage_document = N'USE '+@CurrentDBName+'; UPDATE Document'+@StorageName+'Maps SET IdFileTableDocument = @DocId WHERE IdDocument = @IdDocument AND DocumentType = @DocumentType'
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