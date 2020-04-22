/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<DBProtocollo, varcahr(50), DBProtocollo>  --> Settare il nome del DB di protocollo.				        						*
*	<DBPratiche, varcahr(50), DBPratiche>  --> Se esiste il DB di Pratiche settare il nome.					    					*
*	<DBAtti, varcahr(50), DBAtti>			   --> Se esiste il DB di Atti settare il nome.												*
*	<ESISTE_DB_ATTI, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva		*
*	<ESISTE_DB_PRATICHE, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
*****************************************************************************************************************************************/

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
PRINT 'Creazione StoredProcedure per inserimento Category ';
GO

CREATE PROCEDURE [dbo].[Category_Insert] 
	   @idCategory smallint,
       @Name nvarchar(100),
       @idParent smallint, 
       @isActive tinyint,
       @Code smallint, 
	   @FullIncrementalPath nvarchar(256),
	   @FullCode nvarchar(256), 
       @RegistrationUser nvarchar(256),
	   @RegistrationDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
	   @LastChangedDate datetimeoffset(7),
       @UniqueId uniqueidentifier,
	   @IdMassimarioScarto uniqueidentifier,
	   @IdCategorySchema uniqueidentifier,
	   @StartDate datetimeoffset(7),
	   @EndDate datetimeoffset(7),
	   @IdMetadataRepository uniqueidentifier
AS 

	DECLARE @EntityShortId smallint, @LastUsedIdCategory smallint, @ParentFullCode nvarchar(255)
	SELECT @LastUsedIdCategory = [LastUsedIdCategory] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter]
	SET @EntityShortId = @LastUsedIdCategory + 1
	SET @FullIncrementalPath = @EntityShortId
	SET @FullCode =  RIGHT('0000' + cast(@Code as nvarchar(4)), 4)

	IF(@idParent is not null)
		BEGIN
		SET @ParentFullCode = (SELECT TOP 1 FullCode FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category] where idCategory = @idParent)
		SET @FullIncrementalPath = (SELECT TOP 1 FullIncrementalPath FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category] where idCategory = @idParent) + '|' + cast(@EntityShortId as nvarchar(256))
		SET @FullCode = @ParentFullCode + '|' + @FullCode
		END
	

    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION CategoryInsert
	
	--Inserimento classificatore in db Protocollo
	UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
	INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
             [LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository]) 
             
    VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate, 
		   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository)
    
	SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]

	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
	    --Inserimento classificatore in db Atti
       	UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
		INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
				[LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository]) 
             
		VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate, 
			   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository)

		SELECT [FullSearchComputed] FROM <DBAtti, varchar(50), DBAtti>.[dbo].[Category]
    END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
    BEGIN 
        --Inserimento classificatore in db Pratiche
       	UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
		INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
				[LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository]) 
             
		VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate,  
			   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository)

		SELECT [FullSearchComputed] FROM <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]
    END
 
	COMMIT TRANSACTION CategoryInsert
	SELECT @EntityShortId as idCategory
	END TRY

	BEGIN CATCH 
	print ERROR_MESSAGE() 
    ROLLBACK TRANSACTION CategoryInsert
	END CATCH
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Creazione StoredProcedure per modifica Category';
GO

CREATE PROCEDURE [dbo].[Category_Update] 
	@idCategory smallint, 
    @Name nvarchar(100),
	@idParent smallint, 
    @isActive tinyint,
    @Code smallint,
    @FullIncrementalPath nvarchar(256),
	@FullCode nvarchar(256), 
    @RegistrationUser nvarchar(256),
	@RegistrationDate datetimeoffset(7),
    @LastChangedUser nvarchar(256), 
	@LastChangedDate datetimeoffset(7),
    @UniqueId uniqueidentifier,
	@IdMassimarioScarto uniqueidentifier,
	@IdCategorySchema uniqueidentifier,
	@StartDate datetimeoffset(7),
	@EndDate datetimeoffset(7),
	@IdMetadataRepository uniqueidentifier
AS 

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE 	
	BEGIN TRANSACTION CategoryUpdate
	BEGIN TRY

	--Aggiornamento classificatore in Protocollo
	UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category] SET [Name] = @Name, 
		[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
		[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
		[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
		[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository Where [idCategory] = @idCategory
    SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]

	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		--Aggiornamento classificatore in Atti
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Category] SET [Name] = @Name, 
			[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
			[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
			[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
			[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBAtti, varchar(50), DBAtti>.[dbo].[Category]
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		--Aggiornamento classificatore in Pratiche
		UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category] SET [Name] = @Name, 
			[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
			[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
			[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
			[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]
	END
	COMMIT TRANSACTION CategoryUpdate
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION CategoryUpdate
	END CATCH
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Creazione StoredProcedure per Eliminazione Category';
GO

CREATE PROCEDURE [dbo].[Category_Delete] 
	@idCategory smallint, 
	@idParent smallint, 
	@IdMassimarioScarto uniqueidentifier,
	@IdCategorySchema uniqueidentifier,
	@IdMetadataRepository uniqueidentifier
AS 
	DECLARE @LastChangedDate datetimeoffset(7) = GETDATE()

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION CategoryDelete
	
	BEGIN TRY

	--Cancellazione logica contenitore
		UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]  SET [LastChangedDate] = @LastChangedDate, [isActive] = 0 Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]
	
	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Category]  SET [LastChangedDate] = @LastChangedDate, [isActive] = 0 Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBAtti, varchar(50), DBAtti>.[dbo].[Category]
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]  SET [LastChangedDate] = @LastChangedDate, [isActive] = 0 Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]
	END
	COMMIT TRANSACTION CategoryDelete
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION CategoryDelete
	END CATCH
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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