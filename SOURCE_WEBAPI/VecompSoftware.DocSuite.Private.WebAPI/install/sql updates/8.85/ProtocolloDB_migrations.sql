/****************************************************************************************************************************************
* Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)  *
* <DBProtocollo, varcahr(255), DSProtocollo>  --> Settare il nome del DB di protocollo.                  *
* <DBPratiche, varcahr(255), DSPratiche>  --> Se esiste il DB di Pratiche settare il nome.              *
* <DBAtti, varcahr(255), DSAtti>      --> Se esiste il DB di Atti settare il nome.            *
* <EXIST_DB_ATTI, varchar(255), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva  *
* <EXIST_DB_PRATICHE, varchar(255), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
* <EXIST_DB_PROTOCOLLO, varchar(255), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
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
PRINT 'ALTER PROCEDURE [dbo].[VersioningDatabase]'
GO

ALTER PROCEDURE [dbo].[VersioningDatabase]
	@Version AS NCHAR(5),
	@AppName AS NCHAR(30),
	@MigrateLabel AS NCHAR(30)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @DateNow AS NCHAR(23)
	SET @DateNow = CONVERT(NCHAR(23), SYSDATETIME(), 126)
	IF EXISTS( select * from sys.extended_properties Where class_desc = 'DATABASE' And name = @AppName)
	BEGIN
		-- Aggiornamento property
		EXEC sys.sp_updateextendedproperty @name = @AppName, @value = @Version;
	END
	ELSE
	BEGIN 
		-- Aggiunta property
		EXEC sys.sp_addextendedproperty @name = @AppName, @value = @Version;		
	END

	IF EXISTS( select * from sys.extended_properties Where class_desc = 'DATABASE' And name = @MigrateLabel)
	BEGIN
		EXEC sys.sp_updateextendedproperty @name = @MigrateLabel, @value = @DateNow;
	END
	ELSE
	BEGIN 
		-- Aggiunta property
		EXEC sys.sp_addextendedproperty @name = @MigrateLabel, @value = @DateNow;
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
PRINT 'Versionamento database alla 8.85'
GO

EXEC dbo.VersioningDatabase N'8.85',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Aggiungo colonna calcolata FullSearchComputed'
GO

ALTER TABLE [dbo].[Contact]
ADD [FullSearchComputed]  AS 
(
	(rtrim(ltrim(replace([Description],'|',' ')))+'|')
	+(rtrim(ltrim(coalesce(CONVERT([nvarchar](10),[BirthDate],(103)),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([Code],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([SearchCode],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([FiscalCode],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([Address],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([CivicNumber],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([ZipCode],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([City],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([CityCode],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([TelephoneNumber],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([FaxNumber],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([EmailAddress],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([CertifydMail],''),'')))+'|')
	+(rtrim(ltrim(coalesce(nullif([Nationality],''),'')))+'|')
	+rtrim(ltrim(coalesce(nullif([BirthPlace],''),'')))
) PERSISTED
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create SQL Function [webapiprivate].[Contact_FX_FindContacts]';
GO

CREATE FUNCTION [webapiprivate].[Contact_FX_FindContacts]
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@FullSearchComputed nvarchar(256),
	@ApplyAuthorizations bit,
	@ExcludeRoleContacts bit,
	@ParentId smallint,
	@ParentToExclude smallint,
	@AVCPParentId smallint,
	@FascicleParentId smallint
)
RETURNS TABLE 
AS
RETURN 
(
	WITH   MySecurityGroups
	AS (
			SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName, @Domain)
	   )

	SELECT C.[Incremental] As IdContact,C.[IncrementalFather] AS ContactParent_IdContact,C.[IdContactType] AS ContactType,REPLACE(C.[Description],'|',' ') AS Description,C.[Code]
	  ,C.[EMailAddress] AS Email,C.[CertifydMail] AS CertifiedMail,C.[Note],[C].[RegistrationDate],[C].[FiscalCode]
	FROM [Contact] C
	WHERE 
	(
		(
			@ApplyAuthorizations IS NULL 
			OR @ApplyAuthorizations = 0
		) OR
		@ApplyAuthorizations = 1 AND
		(
			EXISTS 
			(
				SELECT TOP 1 1
				FROM [RoleGroup] RG
				INNER JOIN [MySecurityGroups] MSG ON MSG.IdGroup = RG.IdGroup
				WHERE RG.ProtocolRights LIKE '1%' AND RG.idRole = C.IdRoleRootContact
			)
			OR C.IdRoleRootContact is null
		)
	)
	AND 
	(
		(@ExcludeRoleContacts IS NULL OR @ExcludeRoleContacts = 0) OR
		@ExcludeRoleContacts = 1 AND C.IdRoleRootContact IS NULL
	)
	AND 
	(
		@ParentId IS NULL OR 
		(
			C.FullIncrementalPath LIKE '%'+CAST(@ParentId AS nvarchar)+'|%' OR
			C.Incremental = @ParentId
		)
	)
	AND C.isActive = 1
	AND 
	(
		@ParentToExclude IS NULL OR
		(
			C.FullIncrementalPath NOT like '%'+CAST(@ParentToExclude AS nvarchar)+'|%' 
			AND C.Incremental <> @ParentToExclude
		)
	)
	AND 
	(
		@FullSearchComputed IS NULL OR
		C.FullSearchComputed like '%'+@FullSearchComputed+'%'
	)
	GROUP BY C.[Incremental],C.[IncrementalFather],C.[IdContactType],C.[Description],C.[Code]
	  ,C.[EMailAddress],C.[CertifydMail],C.[Note],C.[RegistrationDate],[C].[FiscalCode]
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
PRINT N'Create SQL Function [webapiprivate].[Contact_FX_GetContacParents]';
GO

CREATE FUNCTION [webapiprivate].[Contact_FX_GetContacParents]
(	
	@IdContact int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT C.[Incremental] As IdContact,C.[IncrementalFather] AS ContactParent_IdContact,C.[IdContactType] AS ContactType,REPLACE(C.[Description],'|',' ') AS Description,C.[Code]
	  ,C.[EMailAddress] AS Email,C.[CertifydMail] AS CertifiedMail,C.[Note],[C].[RegistrationDate],[C].[FiscalCode]
	FROM [Contact] C
	WHERE Incremental IN 
	(
		SELECT [Value] FROM SplitString(
		(
			SELECT FullIncrementalPath FROM Contact WHERE Incremental = @IdContact), '|'
		)
	)
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
PRINT N'Create SQL Function [webapiprivate].[Contact_FX_GetAuthorizedRoleContacts]';
GO

CREATE FUNCTION [webapiprivate].[Contact_FX_GetAuthorizedRoleContacts]
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@AddressBookAdministratorGroups nvarchar(MAX)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH   MySecurityGroups
	AS (
			SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName, @Domain)
	   )

	SELECT C.[Incremental] As IdContact,C.[IncrementalFather] AS ContactParent_IdContact,C.[IdContactType] AS ContactType,REPLACE(C.[Description],'|',' ') AS Description,C.[Code]
	  ,C.[EMailAddress] AS Email,C.[CertifydMail] AS CertifiedMail,C.[Note],[C].[RegistrationDate],[C].[FiscalCode]
	FROM [Contact] C
	WHERE 
	(
		EXISTS 
		(
			SELECT TOP 1 1
			FROM MySecurityGroups
			WHERE IdGroup IN 
			(
				SELECT CAST ([Value] AS SMALLINT)
				FROM dbo.SplitString(@AddressBookAdministratorGroups, '|')
			)
		)
		OR
		EXISTS 
		(
			SELECT TOP 1 1
			FROM [RoleGroup] RG
			INNER JOIN [MySecurityGroups] MSG ON MSG.IdGroup = RG.IdGroup
			WHERE RG.ProtocolRights LIKE '1%' AND RG.idRole = C.IdRoleRootContact
		)	
	)
	AND C.IdContactType = 'S' AND C.IdRoleRootContact IS NOT NULL
	GROUP BY C.[Incremental],C.[IncrementalFather],C.[IdContactType],C.[Description],C.[Code]
	  ,C.[EMailAddress],C.[CertifydMail],C.[Note],C.[RegistrationDate],[C].[FiscalCode]
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
PRINT N'Alter Stored Procedure [dbo].[Contact_Insert]';
GO

ALTER PROCEDURE [dbo].[Contact_Insert] 
    @UniqueId uniqueidentifier,
	@IdContactType char(1),
	@IncrementalFather int,
	@Description nvarchar (4000),
	@BirthDate datetime,
	@Code varchar (8),
	@SearchCode varchar (255),
	@FiscalCode varchar (16),
	@IdPlaceName smallint,
	@Address varchar (60),
	@CivicNumber char (10),
	@ZipCode char (20),
	@City varchar(50),
	@CityCode char(2),
	@TelephoneNumber varchar (50),
	@FaxNumber varchar (50),
	@EMailAddress nvarchar (256),
	@CertifydMail varchar(250),
	@Note varchar(255),
	@idRole smallint,
	@isActive tinyint,
    @isLocked tinyint,
	@isNotExpandable tinyint,
	@RegistrationUser nvarchar (256),
	@RegistrationDate datetimeoffset (7),
	@LastChangedUser nvarchar (256),
	@LastChangedDate datetimeoffset (7),
	@IdTitle int,
	@IdRoleRootContact smallint,
	@ActiveFrom datetime,
	@ActiveTo datetime,
	@isChanged smallint,
	@Language int,
	@Nationality nvarchar (256),
	@BirthPlace nvarchar (256),
	@FullIncrementalPath varchar (255),
	@SDIIdentification nvarchar (256)
	
AS

	DECLARE @isAttiEnable BIT
	DECLARE @isPraticheEnable BIT
	DECLARE @isProtocolloEnable BIT
	SET @isAttiEnable = CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT)
	SET @isPraticheEnable = CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT)
	SET @isProtocolloEnable = CAST('<EXIST_DB_PROTOCOLLO, varchar(255), True>' AS BIT)

	DECLARE @LastUsedIdContact INT, @EntityId INT, @FullIncrementalFatherPath varchar(255)

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION ContactInsert

	SELECT top(1) @LastUsedIdContact = Incremental FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Contact] ORDER BY Incremental DESC
	IF(@LastUsedIdContact is null)
	BEGIN
		SET @LastUsedIdContact = 0
	END

	SET @EntityId = @LastUsedIdContact + 1
	SET @FullIncrementalPath = @EntityId

	SET @FullIncrementalFatherPath = (SELECT FullIncrementalPath FROM dbo.Contact WHERE [Incremental] = @IncrementalFather)
	IF(@FullIncrementalFatherPath IS NOT NULL)
	BEGIN
		SET @FullIncrementalPath = @FullIncrementalFatherPath + '|' + CAST(@EntityId AS VARCHAR(50))
	END

	INSERT INTO  <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
         [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
		 [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
		 [UniqueId],[Language],[Nationality],[BirthPlace], [SDIIdentification])
       VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
	           @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
			   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
	           @UniqueId,@Language, @Nationality, @BirthPlace, @SDIIdentification
			)

	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN	 
		INSERT INTO <DBAtti, varchar(255), DBAtti>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
			[City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
			[RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
			[UniqueId],[Language],[Nationality],[BirthPlace], [SDIIdentification])
		   VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
				   @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
				   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
				   @UniqueId,@Language, @Nationality, @BirthPlace, @SDIIdentification)
		END
	
	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 		
		BEGIN	 
		INSERT INTO <DBPratiche, varchar(255), DBPratiche>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
			[City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
			[RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
			[UniqueId],[Language],[Nationality],[BirthPlace], [SDIIdentification])
		   VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
				   @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
				   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
				   @UniqueId,@Language, @Nationality, @BirthPlace, @SDIIdentification)
		END

	COMMIT TRANSACTION ContactInsert

	SELECT @EntityId as Incremental, [Timestamp]
    FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Contact]
    WHERE Incremental = @EntityId
																	   
	END TRY

	BEGIN CATCH 
		 ROLLBACK TRANSACTION ContactInsert

		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		RAISERROR 
			(
			@ErrorMessage, 
			1,               
			@ErrorNumber,    -- parameter: original error number.
			@ErrorSeverity,  -- parameter: original error severity.
			@ErrorState,     -- parameter: original error state.
			@ErrorProcedure, -- parameter: original error procedure name.
			@ErrorLine       -- parameter: original error line number.
			); 
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
PRINT N'Alter Stored Procedure [dbo].[Contact_Update]';
GO

ALTER PROCEDURE [dbo].[Contact_Update]
		@UniqueId uniqueidentifier,
		@Incremental int, 
		@IdContactType char(1),
		@IncrementalFather int,
		@Description nvarchar (4000),
		@BirthDate datetime,
		@Code varchar (8),
		@SearchCode varchar (255),
		@FiscalCode varchar (16),
		@IdPlaceName smallint,
		@Address varchar (60),
		@CivicNumber char (10),
		@ZipCode char (20),
		@City varchar(50),
		@CityCode char(2),
		@TelephoneNumber varchar (50),
		@FaxNumber varchar (50),
		@EMailAddress nvarchar (256),
		@CertifydMail varchar(250),
		@Note varchar(255),
		@idRole smallint,
		@isActive tinyint,
		@isLocked tinyint,
		@isNotExpandable tinyint,
		@FullIncrementalPath varchar (255), 
		@RegistrationUser nvarchar (256),
		@RegistrationDate datetimeoffset (7),
		@LastChangedUser nvarchar (256),
		@LastChangedDate datetimeoffset (7),
		@IdTitle int,
		@IdRoleRootContact smallint,
		@ActiveFrom datetime,
		@ActiveTo datetime,
		@isChanged smallint,
		@Language int,
		@Nationality nvarchar (256),
		@BirthPlace nvarchar (256),
		@Timestamp_Original timestamp,
		@SDIIdentification nvarchar (256)
	AS 

		DECLARE @isAttiEnable BIT
		DECLARE @isPraticheEnable BIT
		DECLARE @isProtocolloEnable BIT
		SET @isAttiEnable = CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT)
		SET @isPraticheEnable = CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT)
		SET @isProtocolloEnable = CAST('<EXIST_DB_PROTOCOLLO, varchar(255), True>' AS BIT)

		SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
		BEGIN TRANSACTION ContactUpdate
		
	BEGIN TRY
		
		--Aggiornamento contatto
		UPDATE  <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Contact] SET 
			[IdContactType]=@IdContactType,  [IncrementalFather] = @IncrementalFather,[Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
			[City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
			[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
			[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
			[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace, [SDIIdentification] = @SDIIdentification 
			WHERE [Incremental] = @Incremental
			
		
		IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN
			
			UPDATE  <DBAtti, varchar(255), DBAtti>.[dbo].[Contact] SET 
			[IdContactType] = @IdContactType,[IncrementalFather] = @IncrementalFather, [Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
			[City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
			[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
			[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
			[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace, [SDIIdentification] = @SDIIdentification 
			WHERE [Incremental] = @Incremental 

		END

		IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
			BEGIN 
				UPDATE <DBPratiche, varchar(255), DBPratiche>.[dbo].[Contact] SET 
					[IdContactType] = @IdContactType, [IncrementalFather] = @IncrementalFather, [Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
					[City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
					[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
					[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
					[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace, [SDIIdentification] = @SDIIdentification 
				WHERE [Incremental] = @Incremental 

			END

		COMMIT TRANSACTION ContactUpdate
	END TRY
	
	BEGIN CATCH 
		ROLLBACK TRANSACTION ContactUpdate
		
		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		RAISERROR 
			(
			@ErrorMessage, 
			1,               
			@ErrorNumber,    -- parameter: original error number.
			@ErrorSeverity,  -- parameter: original error severity.
			@ErrorState,     -- parameter: original error state.
			@ErrorProcedure, -- parameter: original error procedure name.
			@ErrorLine       -- parameter: original error line number.
			);
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
PRINT N'DROP FUNCTION [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated]';
GO
DROP FUNCTION [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated]

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'DROP FUNCTION [webapiprivate].[DocumentUnit_FX_IsOnlyReferenziable]';
GO
DROP FUNCTION [webapiprivate].[DocumentUnit_FX_IsOnlyReferenziable]

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_CanBeFascicolable]';
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_CanBeFascicolable](
	@FascicleIdCategory smallint,
	@FascicleEnvironment int,
	@FascicleType smallint,
	@IdDocumentUnit uniqueidentifier,
	@Environment int,
	@CategoryId smallint
)
RETURNS BIT
AS
BEGIN
	DECLARE @CanBeFascicolable BIT;
	WITH Categories AS
	(SELECT C.Value as IdCategory, CF.DSWEnvironment, CF.FascicleType
	 FROM [dbo].[SplitString]((SELECT FullIncrementalPath FROM Category WHERE IdCategory = @CategoryId), '|') AS C
	 INNER JOIN CategoryFascicles CF on C.Value = CF.IdCategory
	)

	SELECT @CanBeFascicolable = cast(count(1) as bit)
	FROM Categories C
	WHERE NOT EXISTS (SELECT 1 FROM FascicleDocumentUnits WHERE IdDocumentUnit = @IdDocumentUnit) AND @FascicleIdCategory IN (SELECT IdCategory FROM Categories) AND 
		  (
			   (@FascicleType = 1 AND C.FascicleType in (0,1) AND NOT EXISTS ( SELECT 1 FROM Categories CI WHERE CI.DSWEnvironment = @Environment AND CI.IdCategory = @CategoryId AND CI.FascicleType = 2)) 
			OR (@FascicleType = 2 AND C.FascicleType = 2 AND C.DSWEnvironment = @Environment AND @Environment = @FascicleEnvironment AND C.IdCategory = @CategoryId)
		  )

	RETURN @CanBeFascicolable
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
PRINT N'Alter stored procedure [dbo].[SecurityUsers_Insert]';
GO

ALTER PROCEDURE [dbo].[SecurityUsers_Insert] 
       @Account nvarchar(256), 
       @Description nvarchar(256),
       @UserDomain nvarchar(256),
	   @RegistrationUser nvarchar(256),
       @RegistrationDate datetimeoffset(7),
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
	   @UniqueId uniqueidentifier,
	   @IdGroup int
AS 

DECLARE @LastUsedIdUser INT, @EntityId INT, @SecurityUserId uniqueidentifier

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION SecurityUsersInsert

SELECT top(1) @LastUsedIdUser = IdUser FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[SecurityUsers] ORDER BY idUser DESC
IF(@LastUsedIdUser is null)
BEGIN
	SET @LastUsedIdUser = 0
END

SET @EntityId = @LastUsedIdUser + 1
SET @SecurityUserId = newid()

INSERT INTO  <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[SecurityUsers] ([idUser], [Account], [Description], [idGroup], [RegistrationUser], [RegistrationDate], [UserDomain], [UniqueId])
       VALUES (@EntityId , @Account, @Description, @IdGroup, @RegistrationUser, @RegistrationDate, @UserDomain, @SecurityUserId)

IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN	 
	INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[SecurityUsers] ([idUser], [Account], [Description], [idGroup], [RegistrationUser], [RegistrationDate], [UserDomain], [UniqueId])
       VALUES (@EntityId , @Account, @Description, @IdGroup, @RegistrationUser, @RegistrationDate, @UserDomain, @SecurityUserId)
	END
	
IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 	
	BEGIN	 
	INSERT INTO  <DBPratiche, varchar(50), DBPratiche>.[dbo].[SecurityUsers] ([idUser], [Account], [Description], [idGroup], [RegistrationUser], [RegistrationDate], [UserDomain], [UniqueId])
       VALUES (@EntityId , @Account, @Description, @IdGroup, @RegistrationUser, @RegistrationDate, @UserDomain, @SecurityUserId)
	END

COMMIT TRANSACTION SecurityUsersInsert
END TRY

BEGIN CATCH 
     ROLLBACK TRANSACTION SecurityUsersInsert

	 declare @ErrorNumber as int = ERROR_NUMBER()
	 declare @ErrorSeverity as int = ERROR_SEVERITY()
	 declare @ErrorMessage as nvarchar(4000)
	 declare @ErrorState as int = ERROR_STATE()
	 declare @ErrorLine as int = ERROR_LINE()
	 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

	 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

	 RAISERROR 
			(
			@ErrorMessage, 
			1,               
			@ErrorNumber,    -- parameter: original error number.
			@ErrorSeverity,  -- parameter: original error severity.
			@ErrorState,     -- parameter: original error state.
			@ErrorProcedure, -- parameter: original error procedure name.
			@ErrorLine       -- parameter: original error line number.
			); 
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
PRINT N'Alter stored procedure [dbo].[SecurityGroups_Insert]';
GO

ALTER PROCEDURE [dbo].[SecurityGroups_Insert] 
       @GroupName nvarchar(256), 
       @FullIncrementalPath nvarchar(256),
       @LogDescription nvarchar(256),
	   @RegistrationUser nvarchar(256),
       @RegistrationDate datetimeoffset(7),
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
	   @UniqueId uniqueidentifier,
	   @TenantId uniqueidentifier,
	   @IdSecurityGroupTenant int,
	   @AllUsers bit,
	   @idGroupFather int
AS

DECLARE @LastUsedIdGroup INT, @EntityId INT, @SecurityGroupId uniqueidentifier

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION SecurityGroupsInsert

SELECT top(1) @LastUsedIdGroup = IdGroup FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[SecurityGroups] ORDER BY idGroup DESC
IF(@LastUsedIdGroup is null)
BEGIN
	SET @LastUsedIdGroup = 0
END

SET @EntityId = @LastUsedIdGroup + 1
SET @SecurityGroupId = newid()
SET @FullIncrementalPath = @EntityId

IF(@idGroupFather IS NOT NULL)
BEGIN
	SET @FullIncrementalPath = @idGroupFather + '|' + @EntityId
END

INSERT INTO  <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[securitygroups] ([idGroup], [GroupName], [FullIncrementalPath], [idGroupFather], [RegistrationUser], [RegistrationDate], [AllUsers], [UniqueId], [TenantId], [IdSecurityGroupTenant])
       VALUES (@EntityId , @GroupName, @FullIncrementalPath, @idGroupFather, @RegistrationUser, @RegistrationDate, @AllUsers, @SecurityGroupId, @TenantId, @EntityId)

IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN	 
	INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[securitygroups] ([idGroup], [GroupName], [FullIncrementalPath], [idGroupFather], [RegistrationUser], [RegistrationDate], [AllUsers], [UniqueId], [TenantId], [IdSecurityGroupTenant])
       VALUES (@EntityId , @GroupName, @FullIncrementalPath, @idGroupFather, @RegistrationUser, @RegistrationDate, @AllUsers, @SecurityGroupId, @TenantId, @EntityId)
	END
	
IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 	
	BEGIN	 
	INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[securitygroups] ([idGroup], [GroupName], [FullIncrementalPath], [idGroupFather], [RegistrationUser], [RegistrationDate], [AllUsers], [UniqueId], [TenantId], [IdSecurityGroupTenant])
       VALUES (@EntityId , @GroupName, @FullIncrementalPath, @idGroupFather, @RegistrationUser, @RegistrationDate, @AllUsers, @SecurityGroupId, @TenantId, @EntityId)
	END

COMMIT TRANSACTION SecurityGroupsInsert
RETURN @EntityId
END TRY

BEGIN CATCH 
     ROLLBACK TRANSACTION SecurityGroupsInsert

	 declare @ErrorNumber as int = ERROR_NUMBER()
	 declare @ErrorSeverity as int = ERROR_SEVERITY()
	 declare @ErrorMessage as nvarchar(4000)
	 declare @ErrorState as int = ERROR_STATE()
	 declare @ErrorLine as int = ERROR_LINE()
	 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

	 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

	 RAISERROR 
			(
			@ErrorMessage, 
			1,               
			@ErrorNumber,    -- parameter: original error number.
			@ErrorSeverity,  -- parameter: original error severity.
			@ErrorState,     -- parameter: original error state.
			@ErrorProcedure, -- parameter: original error procedure name.
			@ErrorLine       -- parameter: original error line number.
			); 
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
PRINT 'Alter stored procedure [dbo].[MassimarioScarto_Update]'
GO

ALTER PROCEDURE [dbo].[MassimarioScarto_Update]
	@IdMassimarioScarto uniqueidentifier, 
	@RegistrationDate datetimeoffset(7),
	@RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256),
	@Status smallint,
	@Name nvarchar(256), 
	@Code smallint, 
	@Note nvarchar(1024), 
	@ConservationPeriod smallint, 
	@StartDate datetimeoffset(7), 
	@EndDate datetimeoffset(7),
	@FakeInsertId uniqueidentifier,
	@Timestamp_Original timestamp
AS
	DECLARE @version smallint
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRY
	BEGIN TRANSACTION MassimarioScartoUpdate
		-- Modifico i valori in MassimarioScarto
		UPDATE [dbo].[MassimariScarto] 
		SET [Name] = @Name, [Code] = @Code, [Note] = @Note, 
		[Status] = @Status, [ConservationPeriod] = @ConservationPeriod,
		[StartDate] = @StartDate, [EndDate] = @EndDate,
		[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser
		WHERE [IdMassimarioScarto] = @IdMassimarioScarto

		--Setto la EndDate verso i figli
		UPDATE [dbo].[MassimariScarto] SET [EndDate] = @EndDate
		WHERE [MassimarioScartoNode].GetAncestor(1) = (SELECT [MassimarioScartoNode]  
		FROM [dbo].[MassimariScarto]
		WHERE [IdMassimarioScarto] = @IdMassimarioScarto) AND ([EndDate] is null OR [EndDate] > @EndDate)
		
		SELECT [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp] FROM [dbo].[MassimariScarto] WHERE [IdMassimarioScarto] = @IdMassimarioScarto	
	COMMIT TRANSACTION MassimarioScartoUpdate
	END TRY

	BEGIN CATCH 
		 ROLLBACK TRANSACTION MassimarioScartoUpdate

		 declare @ErrorNumber as int = ERROR_NUMBER()
		 declare @ErrorSeverity as int = ERROR_SEVERITY()
		 declare @ErrorMessage as nvarchar(4000)
		 declare @ErrorState as int = ERROR_STATE()
		 declare @ErrorLine as int = ERROR_LINE()
		 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		 RAISERROR 
				(
				@ErrorMessage, 
				1,               
				@ErrorNumber,    -- parameter: original error number.
				@ErrorSeverity,  -- parameter: original error severity.
				@ErrorState,     -- parameter: original error state.
				@ErrorProcedure, -- parameter: original error procedure name.
				@ErrorLine       -- parameter: original error line number.
				); 
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
PRINT 'Alter stored procedure [dbo].[MassimarioScarto_Insert]'
GO

ALTER PROCEDURE [dbo].[MassimarioScarto_Insert] 
	@IdMassimarioScarto uniqueidentifier, 
	@RegistrationDate datetimeoffset(7),
	@RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256),
	@Status smallint,
	@Name nvarchar(256), 
	@Code smallint, 
	@Note nvarchar(1024), 
	@ConservationPeriod smallint, 
	@StartDate datetimeoffset(7), 
	@EndDate datetimeoffset(7),
	@FakeInsertId uniqueidentifier
AS
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRY
	BEGIN TRANSACTION MassimarioScartoInsert

		-- Recupero il parent node
		IF @FakeInsertId IS NOT NULL
			BEGIN
				SELECT @parentNode = [MassimarioScartoNode] FROM [dbo].[MassimariScarto] WHERE [IdMassimarioScarto] = @FakeInsertId
			END
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode] = hierarchyid::GetRoot())
				BEGIN
					SET @parentNode = hierarchyid::GetRoot()
				END	
			END

		-- Recupero il max node in base al parent node
		SELECT @maxNode = MAX([MassimarioScartoNode]) FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode].GetAncestor(1) = @parentNode;

		IF @FakeInsertId IS NOT NULL			
			BEGIN
				SET @node = @parentNode.GetDescendant(@maxNode, NULL)
			END			
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode] = hierarchyid::GetRoot())
				BEGIN
					SET @node = hierarchyid::GetRoot().GetDescendant(@maxNode, NULL)
					PRINT @node.ToString()
				END	
				ELSE
				BEGIN
					SET @node = hierarchyid::GetRoot()
					PRINT @node.ToString()
				END
			END
	
		
		INSERT INTO [dbo].[MassimariScarto]([MassimarioScartoNode],[IdMassimarioScarto],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[Note],[ConservationPeriod],[StartDate],[EndDate]) 
		VALUES (@node, @IdMassimarioScarto, @RegistrationDate, @RegistrationUser, NULL, NULL, @Status, @Name, @Code, @Note, @ConservationPeriod, @StartDate, @EndDate)

		SELECT [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp] FROM [dbo].[MassimariScarto] WHERE [IdMassimarioScarto] = @IdMassimarioScarto

	COMMIT TRANSACTION MassimarioScartoInsert
	END TRY

	BEGIN CATCH 
		 ROLLBACK TRANSACTION MassimarioScartoInsert

		 declare @ErrorNumber as int = ERROR_NUMBER()
		 declare @ErrorSeverity as int = ERROR_SEVERITY()
		 declare @ErrorMessage as nvarchar(4000)
		 declare @ErrorState as int = ERROR_STATE()
		 declare @ErrorLine as int = ERROR_LINE()
		 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		 RAISERROR 
				(
				@ErrorMessage, 
				1,               
				@ErrorNumber,    -- parameter: original error number.
				@ErrorSeverity,  -- parameter: original error severity.
				@ErrorState,     -- parameter: original error state.
				@ErrorProcedure, -- parameter: original error procedure name.
				@ErrorLine       -- parameter: original error line number.
				); 
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
PRINT N'Alter stored procedure [dbo].[ContainerGroups_Insert]';
GO

ALTER PROCEDURE [dbo].[ContainerGroups_Insert]
	   @idContainer smallint,
	   @Rights char(20),
       @GroupName varchar(255), 
       @ResolutionRights char(20),
       @DocumentRights char(20),
	   @RegistrationUser nvarchar(256),
	   @RegistrationDate datetimeoffset(7),
       @DocumentSeriesRights char(20),
       @idGroup int,
       @DeskRights char(20),
       @UDSRights char(20),
	   @PrivacyLevel int,
	   @FascicleRights char(20)
AS 
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION ContainerGroupInsert

	DECLARE @idContainerGroup uniqueidentifier
	SET @idContainerGroup = newid()

	     INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel], [FascicleRights])
         VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel, @FascicleRights)

		 IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		 BEGIN 
		  INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel], [FascicleRights])
          Values(@idContainer, @Rights, @idGroup, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel, @FascicleRights)
		END
		IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
		BEGIN 
			INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel], [FascicleRights])
			VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel, @FascicleRights)
		END
	
      COMMIT TRANSACTION ContainerGroupInsert
END TRY
BEGIN CATCH 	
	ROLLBACK TRANSACTION ContainerGroupInsert

	declare @ErrorNumber as int = ERROR_NUMBER()
		 declare @ErrorSeverity as int = ERROR_SEVERITY()
		 declare @ErrorMessage as nvarchar(4000)
		 declare @ErrorState as int = ERROR_STATE()
		 declare @ErrorLine as int = ERROR_LINE()
		 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		 RAISERROR 
				(
				@ErrorMessage, 
				1,               
				@ErrorNumber,    -- parameter: original error number.
				@ErrorSeverity,  -- parameter: original error severity.
				@ErrorState,     -- parameter: original error state.
				@ErrorProcedure, -- parameter: original error procedure name.
				@ErrorLine       -- parameter: original error line number.
				); 
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
PRINT 'Alter stored procedure [dbo].[Container_Update] ';
GO

ALTER PROCEDURE [dbo].[Container_Update] 
	@idContainer smallint, 
	@Name nvarchar(1000),
	@Note varchar(2000), 
	@DocmLocation smallint,
	@ProtLocation smallint,
	@ReslLocation smallint,
	@isActive tinyint,
	@Massive tinyint,
	@Conservation smallint,
	@RegistrationDate datetimeoffset(7),
    @RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256), 
	@DocumentSeriesAnnexedLocation smallint,
	@DocumentSeriesLocation smallint,
	@DocumentSeriesUnpublishedAnnexedLocation smallint,
	@ProtocolRejection tinyint,
	@ActiveFrom datetime,
	@ActiveTo datetime,
	@idArchive int,
	@Privacy tinyint,
	@HeadingFrontalino nvarchar(511),
	@HeadingLetter nvarchar(256),
	@ProtAttachLocation smallint,
	@idProtocolType smallint,
	@DeskLocation smallint,
	@UDSLocation smallint,
	@UniqueId uniqueidentifier,
	@AutomaticSecurityGroups tinyint,
	@PrefixSecurityGroupName nvarchar(256),
	@TenantId uniqueidentifier,
	@ContainerType smallint,
	@SecurityUserAccount nvarchar(256),
	@SecurityUserDisplayName nvarchar(256),
	@ManageSecureDocument bit,
	@PrivacyLevel int,
	@PrivacyEnabled bit
AS 

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION ContainerUpdate		

	--Aggiornamento contenitore
	UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation, [ManageSecureDocument] = @ManageSecureDocument, [PrivacyLevel] = @PrivacyLevel, [PrivacyEnabled] = @PrivacyEnabled  Where [idContainer] = @idContainer

	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN

		
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation, [ManageSecureDocument] = @ManageSecureDocument, [PrivacyLevel] = @PrivacyLevel, [PrivacyEnabled] = @PrivacyEnabled  Where [idContainer] = @idContainer
	END

	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
	UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation, [ManageSecureDocument] = @ManageSecureDocument, [PrivacyLevel] = @PrivacyLevel, [PrivacyEnabled] = @PrivacyEnabled  Where [idContainer] = @idContainer
	END
	COMMIT TRANSACTION ContainerUpdate
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION ContainerUpdate

		declare @ErrorNumber as int = ERROR_NUMBER()
		 declare @ErrorSeverity as int = ERROR_SEVERITY()
		 declare @ErrorMessage as nvarchar(4000)
		 declare @ErrorState as int = ERROR_STATE()
		 declare @ErrorLine as int = ERROR_LINE()
		 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		 RAISERROR 
				(
				@ErrorMessage, 
				1,               
				@ErrorNumber,    -- parameter: original error number.
				@ErrorSeverity,  -- parameter: original error severity.
				@ErrorState,     -- parameter: original error state.
				@ErrorProcedure, -- parameter: original error procedure name.
				@ErrorLine       -- parameter: original error line number.
				);
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
PRINT 'Alter stored procedure [dbo].[Container_Insert]';
GO

ALTER PROCEDURE [dbo].[Container_Insert] 
       @Name nvarchar(1000),
       @Note varchar(2000), 
       @DocmLocation smallint,
       @ProtLocation smallint,
       @ReslLocation smallint,
       @isActive tinyint,
       @Massive tinyint,
       @Conservation smallint,
       @RegistrationDate datetimeoffset(7),
       @RegistrationUser nvarchar(256),
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
       @DocumentSeriesAnnexedLocation smallint,
       @DocumentSeriesLocation smallint,
       @DocumentSeriesUnpublishedAnnexedLocation smallint,
       @ProtocolRejection tinyint,
       @ActiveFrom datetime,
       @ActiveTo datetime,
       @idArchive int,
       @Privacy tinyint,
       @HeadingFrontalino nvarchar(511),
       @HeadingLetter nvarchar(256),
       @ProtAttachLocation smallint,
       @idProtocolType smallint,
       @DeskLocation smallint,
       @UDSLocation smallint,
       @UniqueId uniqueidentifier,
       @AutomaticSecurityGroups tinyint,
       @PrefixSecurityGroupName nvarchar(256),
       @TenantId uniqueidentifier,
       @ContainerType smallint,
	   @SecurityUserAccount nvarchar(256),
	   @SecurityUserDisplayName nvarchar(256),
	   @ManageSecureDocument bit,
	   @PrivacyLevel int,
	   @PrivacyEnabled bit
AS 

       DECLARE @EntityShortId smallint, @LastUsedIdContainer smallint, @RightsFull nvarchar(20), @ResolutionRightsFull nvarchar(20), @DocumentRightsFull nvarchar(20), @DocumentSeriesRightsFull nvarchar(20), 
			   @DeskRightsFull nvarchar(20), @UDSRightsFull nvarchar(20),@RightsIns nvarchar(20), @ResolutionRightsIns nvarchar(20), @DocumentRightsIns nvarchar(20), @DocumentSeriesRightsIns nvarchar(20), 
			   @DeskRightsIns nvarchar(20), @UDSRightsIns nvarchar(20),@RightsVis nvarchar(20), @ResolutionRightsVis nvarchar(20), @DocumentRightsVis nvarchar(20), @DocumentSeriesRightsVis nvarchar(20), 
			   @DeskRightsVis nvarchar(20), @UDSRightsVis nvarchar(20), @SecurityUserName nvarchar(100), @SecurityUserDescription nvarchar(100), @SecurityUserDomain nvarchar(100), @SecurityGroupIdFull int, 
			   @SecurityGroupIdVis int, @SecurityGroupIdIns int, @SecurityGroupName nvarchar(256)


	SELECT  @LastUsedIdContainer = [LastUsedIdContainer] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter]

       --Ad oggi è implementata solo la gestione dei diritti degli archivi,
       --Per ottenere un corretto funzionamento con le altre UD occorre implementarne la gestione
       --diritti protocollo

       SET @RightsFull = 
             CASE 
                    WHEN @ContainerType = 1 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @RightsIns = 
             CASE 
                    WHEN @ContainerType = 1 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @RightsVis = 
             CASE 
                    WHEN @ContainerType = 1 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

             --diritti atti

       SET @ResolutionRightsFull = 
             CASE 
                    WHEN @ContainerType = 2 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @ResolutionRightsIns = 
             CASE 
                    WHEN @ContainerType = 2 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @ResolutionRightsVis = 
             CASE 
                    WHEN @ContainerType = 2 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

             --diritti pratiche

       SET @DocumentRightsFull = 
             CASE 
                    WHEN @ContainerType = 4 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentRightsIns = 
             CASE 
                    WHEN @ContainerType = 4 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentRightsVis = 
             CASE 
                    WHEN @ContainerType = 4 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END           

             --diritti serie documentali

       SET @DocumentSeriesRightsFull = 
             CASE 
                    WHEN @ContainerType = 8 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentSeriesRightsIns = 
             CASE 
                    WHEN @ContainerType = 8 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentSeriesRightsVis = 
             CASE 
                    WHEN @ContainerType = 8 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END    

             --diritti tavoli

       SET @DeskRightsFull = 
             CASE 
                    WHEN @ContainerType = 16 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DeskRightsIns = 
             CASE 
                    WHEN @ContainerType = 16 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DeskRightsVis = 
             CASE 
                    WHEN @ContainerType = 16 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END    

             -- diritti UDS
             
       SET @UDSRightsFull = 
             CASE 
                    WHEN @ContainerType = 32 THEN '11111111000000000000'
                    ELSE '00000000000000000000'
             END

       SET @UDSRightsIns = 
             CASE 
                    WHEN @ContainerType = 32 THEN '11111000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @UDSRightsVis = 
             CASE 
                    WHEN @ContainerType = 32 THEN '00110000000000000000'
                    ELSE '00000000000000000000'
             END    

	   IF(@SecurityUserAccount IS NOT NULL AND @SecurityUserAccount != '')
			BEGIN			
			SELECT TOP 1 @SecurityUserDomain = Value FROM dbo.SplitString(@SecurityUserAccount, '\')
			SELECT TOP 1 @SecurityUserName = Value FROM (SELECT Value, row_number() over(order by (select null)) as rownum FROM dbo.SplitString(@SecurityUserAccount, '\')) T where T.rownum > 1 AND T.rownum <= 2
			END

       SET @EntityShortId = @LastUsedIdContainer + 1

       SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
       BEGIN TRY
       BEGIN TRANSACTION ContainerInsert
       UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter] SET [LastUsedidContainer] = @EntityShortId

       --Inserimento contenitore
       INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
             [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
             [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
             [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId], [ManageSecureDocument], [PrivacyLevel], [PrivacyEnabled]) 
             
       VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation,   @RegistrationUser, @RegistrationDate, @DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
             @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId, @ManageSecureDocument, @PrivacyLevel, @PrivacyEnabled)

	   IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
       BEGIN
             update  <DBAtti, varchar(50), DBAtti>.[dbo].[Parameter] set [LastUsedidContainer] = @EntityShortId
             --Inserimento contenitore
             INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
                    [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
                    [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
                    [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId], [ManageSecureDocument], [PrivacyLevel], [PrivacyEnabled])  
             
             VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation,@RegistrationUser, @RegistrationDate, 
					@DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
                    @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId, @ManageSecureDocument, @PrivacyLevel, @PrivacyEnabled)

       END

	   IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
       BEGIN 
             update <DBPratiche, varchar(50), DBPratiche>.[dbo].[Parameter] set [LastUsedidContainer] = @EntityShortId

             --Inserimento contenitore
             INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
                    [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
                    [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
                    [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId], [ManageSecureDocument], [PrivacyLevel], [PrivacyEnabled]) 
             
             VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation, @RegistrationUser, @RegistrationDate, 
					@DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
                    @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId, @ManageSecureDocument, @PrivacyLevel, @PrivacyEnabled)

        END

	   IF(@AutomaticSecurityGroups = 1)
       BEGIN 
             --inserimento  gruppo con tutti i diritti
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_full'
             EXEC @SecurityGroupIdFull = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsFull, @SecurityGroupName, @ResolutionRightsFull, @DocumentRightsFull, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsFull, @SecurityGroupIdFull, @DeskRightsFull, @UDSRightsFull, @PrivacyLevel, '00000000000000000000'
             
			 --inserimento  gruppo con diritti di inserimento
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_ins'
             EXEC @SecurityGroupIdIns = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
			 EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsIns, @SecurityGroupName, @ResolutionRightsIns, @DocumentRightsIns, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsIns, @SecurityGroupIdIns, @DeskRightsIns, @UDSRightsIns, @PrivacyLevel, '00000000000000000000'
             
			 --inserimento gruppo con diritti di visualizzazione
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_vis'
             EXEC @SecurityGroupIdVis = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsVis, @SecurityGroupName, @ResolutionRightsVis, @DocumentRightsVis, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsVis, @SecurityGroupIdVis, @DeskRightsVis, @UDSRightsVis, @PrivacyLevel, '00000000000000000000'
       
			IF(@SecurityUserAccount IS NOT NULL AND @SecurityUserDomain IS NOT NULL AND @SecurityUserName IS NOT NULL)
			BEGIN			

			EXEC [dbo].[SecurityUsers_Insert] @SecurityUserName, @SecurityUserDisplayName, @SecurityUserDomain, @RegistrationUser, @RegistrationDate, null, null, null, @SecurityGroupIdFull 

			END
	   END


       COMMIT TRANSACTION ContainerInsert
       SELECT @EntityShortId as idContainer
       END TRY

       BEGIN CATCH 
          ROLLBACK TRANSACTION ContainerInsert

		  declare @ErrorNumber as int = ERROR_NUMBER()
		 declare @ErrorSeverity as int = ERROR_SEVERITY()
		 declare @ErrorMessage as nvarchar(4000)
		 declare @ErrorState as int = ERROR_STATE()
		 declare @ErrorLine as int = ERROR_LINE()
		 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		 RAISERROR 
				(
				@ErrorMessage, 
				1,               
				@ErrorNumber,    -- parameter: original error number.
				@ErrorSeverity,  -- parameter: original error severity.
				@ErrorState,     -- parameter: original error state.
				@ErrorProcedure, -- parameter: original error procedure name.
				@ErrorLine       -- parameter: original error line number.
				);
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
PRINT N'Alter stored procedure [dbo].[Container_Delete]';
GO

ALTER PROCEDURE [dbo].[Container_Delete] 
	@idContainer smallint, 
	@DocmLocation smallint,
	@ProtLocation smallint,
	@ReslLocation smallint,
	@idProtocolType smallint,
	@DeskLocation smallint,
	@DocumentSeriesAnnexedLocation smallint,
	@DocumentSeriesLocation smallint,
	@DocumentSeriesUnpublishedAnnexedLocation smallint,
	@ProtAttachLocation smallint,
	@UDSLocation smallint
AS 
	DECLARE @LastChangedDate datetimeoffset(7)

	SET @LastChangedDate = GETDATE()

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION ContainerDelete	

	--Cancellazione logica contenitore
		UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [idContainer] = @idContainer
	
	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Container]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [idContainer] = @idContainer
	END

	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [idContainer] = @idContainer
	END
	COMMIT TRANSACTION ContainerDelete
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION ContainerDelete

		declare @ErrorNumber as int = ERROR_NUMBER()
		 declare @ErrorSeverity as int = ERROR_SEVERITY()
		 declare @ErrorMessage as nvarchar(4000)
		 declare @ErrorState as int = ERROR_STATE()
		 declare @ErrorLine as int = ERROR_LINE()
		 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		 RAISERROR 
				(
				@ErrorMessage, 
				1,               
				@ErrorNumber,    -- parameter: original error number.
				@ErrorSeverity,  -- parameter: original error severity.
				@ErrorState,     -- parameter: original error state.
				@ErrorProcedure, -- parameter: original error procedure name.
				@ErrorLine       -- parameter: original error line number.
				);
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
PRINT 'Alter stored procedure [dbo].[Category_Update]';
GO

ALTER PROCEDURE [dbo].[Category_Update] 
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
	BEGIN TRY
	BEGIN TRANSACTION CategoryUpdate	

	--Aggiornamento classificatore in Protocollo
	UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category] SET [Name] = @Name, 
		[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
		[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
		[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
		[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository Where [idCategory] = @idCategory
    SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]

	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		--Aggiornamento classificatore in Atti
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Category] SET [Name] = @Name, 
			[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
			[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
			[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
			[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBAtti, varchar(50), DBAtti>.[dbo].[Category]
	END

	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
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

		declare @ErrorNumber as int = ERROR_NUMBER()
		 declare @ErrorSeverity as int = ERROR_SEVERITY()
		 declare @ErrorMessage as nvarchar(4000)
		 declare @ErrorState as int = ERROR_STATE()
		 declare @ErrorLine as int = ERROR_LINE()
		 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		 RAISERROR 
				(
				@ErrorMessage, 
				1,               
				@ErrorNumber,    -- parameter: original error number.
				@ErrorSeverity,  -- parameter: original error severity.
				@ErrorState,     -- parameter: original error state.
				@ErrorProcedure, -- parameter: original error procedure name.
				@ErrorLine       -- parameter: original error line number.
				);
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
PRINT 'Alter stored procedure [dbo].[Category_Insert]';
GO

ALTER PROCEDURE [dbo].[Category_Insert] 
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

	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
	    --Inserimento classificatore in db Atti
       	UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
		INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
				[LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository]) 
             
		VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate, 
			   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository)

		SELECT [FullSearchComputed] FROM <DBAtti, varchar(50), DBAtti>.[dbo].[Category]
    END

	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
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
		ROLLBACK TRANSACTION CategoryInsert

		declare @ErrorNumber as int = ERROR_NUMBER()
		 declare @ErrorSeverity as int = ERROR_SEVERITY()
		 declare @ErrorMessage as nvarchar(4000)
		 declare @ErrorState as int = ERROR_STATE()
		 declare @ErrorLine as int = ERROR_LINE()
		 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		 RAISERROR 
				(
				@ErrorMessage, 
				1,               
				@ErrorNumber,    -- parameter: original error number.
				@ErrorSeverity,  -- parameter: original error severity.
				@ErrorState,     -- parameter: original error state.
				@ErrorProcedure, -- parameter: original error procedure name.
				@ErrorLine       -- parameter: original error line number.
				);
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
PRINT N'Alter stored procedure [dbo].[Category_Delete]';
GO

ALTER PROCEDURE [dbo].[Category_Delete] 
	@idCategory smallint, 
	@idParent smallint, 
	@IdMassimarioScarto uniqueidentifier,
	@IdCategorySchema uniqueidentifier,
	@IdMetadataRepository uniqueidentifier
AS 
	DECLARE @LastChangedDate datetimeoffset(7) = GETDATE()

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION CategoryDelete		

	--Cancellazione logica contenitore
		UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]  SET [LastChangedDate] = @LastChangedDate, [isActive] = 0 Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]
	
	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Category]  SET [LastChangedDate] = @LastChangedDate, [isActive] = 0 Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBAtti, varchar(50), DBAtti>.[dbo].[Category]
	END

	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]  SET [LastChangedDate] = @LastChangedDate, [isActive] = 0 Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]
	END
	COMMIT TRANSACTION CategoryDelete
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION CategoryDelete

		declare @ErrorNumber as int = ERROR_NUMBER()
		 declare @ErrorSeverity as int = ERROR_SEVERITY()
		 declare @ErrorMessage as nvarchar(4000)
		 declare @ErrorState as int = ERROR_STATE()
		 declare @ErrorLine as int = ERROR_LINE()
		 declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		 SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		 RAISERROR 
				(
				@ErrorMessage, 
				1,               
				@ErrorNumber,    -- parameter: original error number.
				@ErrorSeverity,  -- parameter: original error severity.
				@ErrorState,     -- parameter: original error state.
				@ErrorProcedure, -- parameter: original error procedure name.
				@ErrorLine       -- parameter: original error line number.
				);
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
PRINT N'DROP FUNCTION [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated]';
GO
DROP FUNCTION [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated]

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'DROP FUNCTION [webapiprivate].[DocumentUnit_FX_IsOnlyReferenziable]';
GO
DROP FUNCTION [webapiprivate].[DocumentUnit_FX_IsOnlyReferenziable]

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_CanBeFascicolable]';
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_CanBeFascicolable](
	@FascicleIdCategory smallint,
	@FascicleEnvironment int,
	@FascicleType smallint,
	@IdDocumentUnit uniqueidentifier,
	@Environment int,
	@CategoryId smallint
)
RETURNS BIT
AS
BEGIN
	DECLARE @CanBeFascicolable BIT
	WITH Categories AS
	(SELECT C.Value as IdCategory, CF.DSWEnvironment, CF.FascicleType
	 FROM [dbo].[SplitString]((SELECT FullIncrementalPath FROM Category WHERE IdCategory = @CategoryId), '|') AS C
	 INNER JOIN CategoryFascicles CF on C.Value = CF.IdCategory
	)

	SELECT @CanBeFascicolable = cast(count(1) as bit)
	FROM Categories C
	WHERE NOT EXISTS (SELECT 1 FROM FascicleDocumentUnits WHERE IdDocumentUnit = @IdDocumentUnit) AND @FascicleIdCategory IN (SELECT IdCategory FROM Categories) AND 
		  (
			   (@FascicleType = 1 AND C.FascicleType in (0,1) AND NOT EXISTS ( SELECT 1 FROM Categories CI WHERE CI.DSWEnvironment = @Environment AND CI.IdCategory = @CategoryId AND CI.FascicleType = 2)) 
			OR (@FascicleType = 2 AND C.FascicleType = 2 AND C.DSWEnvironment = @Environment AND @Environment = @FascicleEnvironment AND C.IdCategory = @CategoryId)
		  )

	RETURN @CanBeFascicolable
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