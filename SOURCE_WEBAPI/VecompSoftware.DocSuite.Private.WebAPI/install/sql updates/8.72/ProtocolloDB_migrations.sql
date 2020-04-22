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
PRINT N'Modificata funzione [DocumentUnit_FX_HasVisibilityRight]';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_HasVisibilityRight] 
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@IdDocumentUnit uniqueidentifier
)
RETURNS BIT
AS
BEGIN
    declare @HasRight bit;

	WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
	WHERE DU.IdDocumentUnit = @IdDocumentUnit
	
	and ( exists ( select top 1 CG.idContainerGroup
				 from [dbo].[ContainerGroup] CG 
				 INNER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
				 where CG.IdContainer = DU.IdContainer AND 
				 C_MSG.IdGroup IS NOT NULL AND ((DU.Environment = 1 AND (CG.Rights like '__1%'))
					OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					OR (DU.Environment > 99 AND (CG.UDSRights like '__1%'))
					))

	 OR exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
				INNER JOIN Role R on RG.idRole = R.idRole
				where  R.UniqueId = DUR.UniqueIdRole AND
				MSG.IdGroup IS NOT NULL AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
				OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
				OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
				OR (DU.Environment > 99 AND (RG.DocumentSeriesRights like '1%'))
				))
	OR (DU.Environment = 1 and exists (select top 1 IdProtocolHighlightUser from ProtocolHighlightUsers PHU where PHU.UniqueIdProtocol = DU.IdDocumentUnit AND Account= @Domain+'\'+@UserName))
	OR (DU.Environment = 2 and exists (select top 1 idResolution from Resolution R where R.UniqueId = @IdDocumentUnit and R.EffectivenessDate is not null and R.EffectivenessUser is not null))
	)
    
	RETURN @HasRight;

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
PRINT 'Modificata sql function [dbo].[ DocumentUnit_FX_AllowedDocumentUnits]';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_AllowedDocumentUnits] 
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset
)
RETURNS TABLE 
AS
RETURN 
(
WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    )

	SELECT distinct DU.[IdDocumentUnit] as UniqueId
	  ,DU.[IdFascicle]
	  ,DU.[EntityId]
	  ,DU.[Year]
	  ,CAST(DU.[Number] as varchar) as Number
	  ,DU.[Title]
	  ,DU.[Subject]
	  ,DU.[DocumentUnitName]
	  ,DU.[Environment]
	  ,DU.[RegistrationUser]
	  ,DU.[RegistrationDate]
	  ,DU.[IdUDSRepository]
	  ,DU.idCategory as Category_IdCategory
	  ,CT.Name as Category_Name
	  ,DU.idContainer as Container_IdContainer
	  ,C.Name as Container_Name
	  ,(SELECT TOP 1 DocumentName 
	   FROM cqrs.DocumentUnitChains CUC 
	   WHERE CUC.IdDocumentUnit = DU.IdDocumentUnit AND CUC.DocumentName is not null AND CUC.DocumentName <> ''
	   ORDER BY CUC.RegistrationDate DESC) as DocumentUnitChain_DocumentName
	 FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
    LEFT OUTER JOIN [dbo].[Role] R on DUR.UniqueIdRole = R.UniqueId
    LEFT OUTER JOIN [dbo].[RoleGroup] RG on R.idRole = RG.idRole
    LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE DU.RegistrationDate BETWEEN @DateFrom AND @DateTo 
	AND ( (DUR.UniqueIdRole IS NOT NULL
				AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
				OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
				OR ((DU.Environment = 4 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
				AND MSG.IdGroup IS NOT NULL)
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
PRINT 'Modificata sql function [dbo].[ DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle]';
GO
ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle] 
(	
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@FascicleId uniqueidentifier,
	@Year smallint = null,
	@Number int = null,
	@DocumentUnitName nvarchar(256) = null,
	@CategoryId smallint = null,
	@Subject nvarchar(256) = null,
	@Skip int,
	@Top int
)
RETURNS TABLE 
AS
RETURN 
(
	WITH 	
	MySecurityGroups AS (
		SELECT SG.IdGroup 
		FROM [dbo].[SecurityGroups] SG 
		LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
		GROUP BY SG.IdGroup
	), 	
	MyCategory AS (
		SELECT TOP 1 C.IdCategory
		FROM [dbo].[Category] C 
		INNER JOIN [dbo].[Fascicles] F on F.IdCategory = C.IdCategory
		WHERE F.IdFascicle = @FascicleId
		GROUP BY C.IdCategory
	), 	
	MyCategoryFascicles AS (
		SELECT CF.IdCategory
		FROM [dbo].[CategoryFascicles] CF 
		INNER JOIN [dbo].[Category] C on C.idCategory = CF.IdCategory
		WHERE (exists (select MyCategory.IdCategory from MyCategory where CF.IdCategory = MyCategory.IdCategory and CF.FascicleType = 1))
			  OR (exists (select MyCategory.IdCategory from MyCategory where MyCategory.IdCategory in (SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|')) and CF.FascicleType = 0))
		GROUP BY CF.IdCategory
	),
	
	MydocumentUnits AS (
			select T.IdDocumentUnit, T.rownum from
			(select DU.[IdDocumentUnit], row_number() over(order by DU.[IdDocumentUnit]) as rownum 
			 FROM cqrs.DocumentUnits DU
			 	INNER JOIN [dbo].[Container] C on DU.IdContainer = C.IdContainer
				INNER JOIN [dbo].[Category] CT on DU.IdCategory = CT.IdCategory
				LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
			 WHERE (@Year IS NULL OR DU.Year = @Year) 
			 	AND (@Number IS NULL OR DU.Number = @Number)
				AND (@DocumentUnitName IS NULL OR DU.DocumentUnitName = @DocumentUnitName)
				AND (@CategoryId IS NULL OR DU.IdCategory = @CategoryId)
				AND (@Subject IS NULL OR DU.Subject like '%'+@Subject+'%')
				AND ((exists (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1))
				  AND (
				     exists (select top 1 CG.idContainerGroup
					 from [dbo].[ContainerGroup] CG 
					 INNER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
					 where CG.IdContainer = DU.IdContainer AND C_MSG.IdGroup IS NOT NULL
					 AND (
					 (DU.Environment = 1 AND (CG.Rights like '__1%'))
					 OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					 OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					 OR ((DU.Environment = 7 OR DU.Environment > 99) AND (CG.UDSRights like '__1%'))
					)) 
					 OR exists (select top 1 RG.idRole
						from [dbo].[RoleGroup] RG
						INNER JOIN Role R on RG.idRole = R.idRole
						INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
						where  R.UniqueId = DUR.UniqueIdRole
							   AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
							   OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
						       OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
						       OR ((DU.Environment = 7 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
						       AND MSG.IdGroup IS NOT NULL)
				 )
		    )
			OR (not exists (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1)) 
			AND DU.Environment IN (SELECT CF.DSWEnvironment FROM [dbo].[CategoryFascicles] CF
						  INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = CF.IdCategory
						  WHERE CF.FascicleType = 1 AND F.IdFascicle = @FascicleId)))
			AND (DU.IdFascicle is null OR DU.IdFascicle != @FascicleId)
			Group by DU.[IdDocumentUnit]) T where T.rownum > @Skip AND T.rownum <= @Top
		)

	SELECT DU.[IdDocumentUnit] as UniqueId
		  ,DU.[IdFascicle]
		  ,DU.[EntityId]
		  ,DU.[Year]
		  ,CAST(DU.[Number] as varchar) as Number
		  ,DU.[Title]
		  ,DU.[Subject]
		  ,DU.[DocumentUnitName]
		  ,DU.[Environment]
		  ,DU.[RegistrationUser]
		  ,DU.[RegistrationDate]
		  ,DU.[IdUDSRepository]
		  ,CT.idCategory AS Category_IdCategory
		  ,CT.Name AS Category_Name
		  ,C.idContainer AS Container_IdContainer
		  ,C.Name AS Container_Name
		  ,(select CAST(COUNT(1) AS BIT) from MyCategoryFascicles where MyCategoryFascicles.IdCategory = CT.IdCategory) as IsFascicolable
		  from cqrs.DocumentUnits DU
	INNER JOIN [dbo].[Container] C on DU.IdContainer = C.IdContainer
	INNER JOIN [dbo].[Category] CT on DU.IdCategory = CT.IdCategory
where exists (select MydocumentUnits.[IdDocumentUnit] from MydocumentUnits where DU.[IdDocumentUnit] = MydocumentUnits.IdDocumentUnit)
);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Modificata sql function [dbo].[ DocumentUnit_FX_FascicolableDocumentUnits]';
GO
ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_FascicolableDocumentUnits]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset,
	@IncludeThreshold bit,
	@ThresholdFrom datetimeoffset
)
RETURNS TABLE
AS
RETURN
(
	WITH 
	
	MySecurityGroups AS (
		SELECT SG.IdGroup 
		FROM [dbo].[SecurityGroups] SG 
		LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
		GROUP BY SG.IdGroup
	)

   
	SELECT DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title] AS [Number],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit] as [UniqueId],
		   CT.idCategory AS Category_IdCategory,
		   CT.Name AS Category_Name,
		   C.idContainer AS Container_IdContainer,
		   C.Name AS Container_Name
	FROM [cqrs].[DocumentUnits] DU
	INNER JOIN [dbo].[Category] CT on DU.idCategory = CT.idCategory
	INNER JOIN [dbo].[Container] C on DU.idContainer = C.idContainer
	INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
	LEFT OUTER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
	
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DU.IdDocumentUnit = DUR.IdDocumentUnit
	LEFT OUTER JOIN [dbo].[Role] RL on DUR.UniqueIdRole = RL.UniqueId
	LEFT OUTER JOIN [dbo].[RoleGroup] RG on RL.idRole = RG.idRole
	LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup

	WHERE ( (@IncludeThreshold = 0 AND DU.RegistrationDate BETWEEN @DateFrom AND @DateTo) OR
				( @IncludeThreshold = 1 AND ( DU.RegistrationDate BETWEEN @ThresholdFrom AND CAST(getdate()-60 AS DATE) OR 
											  DU.RegistrationDate BETWEEN @DateFrom AND @DateTo))
			   )
			   AND ( (C_MSG.IdGroup IS NOT NULL AND (CASE Environment 
													WHEN 1 THEN CG.Rights 
													WHEN 2 THEN CG.ResolutionRights 
													WHEN 3 THEN '0'
													WHEN 4 THEN CG.DocumentSeriesRights
													WHEN 5 THEN '0'
													ELSE CG.UDSRights
													END) like '1%') OR 
					 ( DUR.UniqueIdRole IS NULL OR 
					  (DUR.UniqueIdRole IS NOT NULL AND (CASE Environment 
													WHEN 1 THEN RG.ProtocolRights 
													WHEN 2 THEN RG.ResolutionRights 
													WHEN 3 THEN '0'
													WHEN 4 THEN RG.DocumentSeriesRights
													ELSE '0'
													END like '1%') AND MSG.IdGroup IS NOT NULL)
					 ) 
				   )
			   AND NOT (C_MSG.IdGroup IS NULL AND MSG.IdGroup IS NULL)
			   AND DU.[IdFascicle] IS NULL
			   AND DU.Environment in (1,2)
	GROUP BY DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit],
		   CT.idCategory,
		   CT.Name,
		   C.idContainer,
		   C.Name
);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Modificata sql function [dbo].[ Dossiers_FX_AuthorizedDossiers]';
GO
ALTER FUNCTION [webapiprivate].[Dossiers_FX_AuthorizedDossiers](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Skip int,
	@Top int,
	@Year smallint,
	@Number smallint,
	@Subject nvarchar(255),
	@Note nvarchar(255),
	@ContainerId smallint,
	@StartDateFrom datetimeoffset,
	@StartDateTo datetimeoffset,
	@EndDateFrom datetimeoffset,
	@EndDateTo datetimeoffset
)
RETURNS TABLE
AS
RETURN
	(
		WITH 	
		MySecurityGroups AS (
			SELECT SG.IdGroup 
			FROM [dbo].[SecurityGroups] SG 
			LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
			WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
			OR SG.AllUsers = 1
			GROUP BY SG.IdGroup
		),
		MyDossiers AS (
			select * from
			(select Dossier.IdDossier, row_number() over(order by Dossier.Year, Dossier.Number) as rownum 
			 FROM dbo.Dossiers Dossier
			 inner join dbo.Container C on Dossier.IdContainer = C.idContainer
			 inner join dbo.DossierRoles DR on Dossier.IdDossier = DR.IdDossier and DR.RoleAuthorizationType = 0
			 inner join dbo.Role R on DR.IdRole = R.idRole
			 WHERE  (
			   exists (select top 1 CG.IdContainerGroup
						from dbo.ContainerGroup CG
						INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
						where CG.IdContainer = Dossier.IdContainer and C_MSG.IdGroup is not null
							  and CG.DocumentRights like '___1%')
			   or exists (select top 1 RG.idRole
						from dbo.RoleGroup RG
						INNER JOIN Role R on RG.idRole = R.idRole
						INNER JOIN MySecurityGroups MSG on RG.idGroup = MSG.idGroup
						where R.idRole = DR.IdRole and MSG.IdGroup is not null
							  and RG.DocumentRights like '1%')
			   )
			   AND (@Year is null or Dossier.Year = @Year)
			   AND (@Number is null or Dossier.Number = @Number)
			   AND (@Subject is null or Dossier.Subject like '%'+@Subject+'%')
			   AND (@ContainerId is null or C.idContainer = @ContainerId)
			   AND (@StartDateFrom is null or Dossier.StartDate >= @StartDateFrom)
			   AND (@StartDateTo is null or Dossier.StartDate <= @StartDateTo)
			   AND (@EndDateFrom is null or Dossier.EndDate >= @StartDateFrom)
			   AND (@EndDateTo is null or Dossier.EndDate <= @EndDateTo)
			   AND (@Note is null or Dossier.Note like '%'+@Note+'%')
			Group by Dossier.IdDossier, Dossier.Year, Dossier.Number) T where T.rownum > @Skip AND T.rownum <= @Top
		)

select D.IdDossier, D.Year, D.Number, D.Subject, D.RegistrationDate, D.StartDate, D.EndDate,
	   C.idContainer as Container_Id, C.Name as Container_Name,
	   R.idRole as Role_IdRole, R.Name as Role_Name, Contact.Incremental as Contact_Incremenental, Contact.Description as Contact_Description
from Dossiers D
left join DossierContacts DC ON DC.IdDossier = D.IdDossier
left join dbo.Contact Contact on DC.IdContact = Contact.Incremental
inner join dbo.Container C on D.IdContainer = C.idContainer
inner join dbo.DossierRoles DR on D.IdDossier = DR.IdDossier and DR.RoleAuthorizationType = 0
inner join dbo.Role R on DR.IdRole = R.idRole
where exists (select * from MyDossiers where D.IdDossier = MyDossiers.IdDossier)
);
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Modificata sql function [dbo].[ Fascicles_FX_AvailableFascicles]';
GO
ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@idCategory smallint,
	@Name nvarchar(256)

)
RETURNS TABLE
AS
RETURN
(
WITH
MySecurityGroups AS (
	SELECT SG.IdGroup 
	FROM [dbo].[SecurityGroups] SG 
	LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
	OR SG.AllUsers = 1
	GROUP BY SG.IdGroup
)

SELECT F.IdFascicle AS UniqueId,
		F.Year,
		F.Number,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.IdCategory,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate,
		F_C.IdCategory as Category_idCategory,
		F_C.Name as Category_Name		
FROM Fascicles F
INNER JOIN [dbo].[Category] F_C ON F.idCategory = F_C.idCategory
LEFT OUTER JOIN [dbo].[FascicleRoles] FR ON FR.idFascicle = F.idFascicle	
	WHERE (@idCategory = 0 OR  F.IdCategory = @idCategory) 
	    AND ((@Name is NOT null AND ( F.Title like '%'+@Name+'%' OR F.Object like '%'+@Name+'%')) OR (@Name Is null))
		AND((F.FascicleType = 1 AND
				EXISTS (SELECT TOP 1 CG.IdCategory FROM [dbo].[CategoryGroup] CG
				INNER JOIN MySecurityGroups C_MSG ON CG.idgroup = C_MSG.idGroup
				WHERE CG.ProtocolRights LIKE '____1' AND CG.idcategory = F_C.idcategory				
			))
			OR (F.FascicleType = 4 AND FR.RoleAuthorizationType = 0 AND
				EXISTS (SELECT TOP 1 R.idRole from [dbo].[Role] R
				INNER JOIN [dbo].[RoleGroup] RG ON R.idRole = RG.idRole
				INNER JOIN MySecurityGroups R_MSG ON RG.idGroup = R_MSG.idGroup
				WHERE FR.idrole = R.idRole AND R.IsActive = 1 
					)
				)
			)	
GROUP BY F.IdFascicle,
		 F.Year,
		 F.Number,
		 F.EndDate,
		 F.Title,
		 F.Name,
		 F.Object,
		 F.Manager,
		 F.IdCategory,
		 F.FascicleType,
		 F.VisibilityType,
		 F.RegistrationUser,
		 F.RegistrationDate,
		 F_C.IdCategory,
		 F_C.Name
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
PRINT 'Modificata sql function [dbo].[UDSRepository_FX_InsertableRepositoriesByTypology]';
GO

ALTER FUNCTION [webapiprivate].[UDSRepository_FX_InsertableRepositoriesByTypology](
	@UserName nvarchar(256),
	@Domain nvarchar(256),
	@IDUDSTypology uniqueidentifier,
	@PECAnnexedEnabled bit
)
RETURNS TABLE
AS
RETURN
(
WITH
MySecurityGroups AS (
	SELECT SG.IdGroup 
	FROM [dbo].[SecurityGroups] SG 
	LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
	OR SG.AllUsers = 1
	GROUP BY SG.IdGroup
)

SELECT R.IdUDSRepository,
	   R.Name,
	   R.IdContainer as Container_IdContainer,
	   R.Version,
	   R.Status,
	   R.ActiveDate,
	   R.ExpiredDate, 
	   R.RegistrationUser,
	   R.RegistrationDate,
	   R.LastChangedUser,
	   R.LastChangedDate,
	   R.SequenceCurrentNumber,
	   R.SequenceCurrentYear,
	   R.DSWEnvironment,
	   R.Alias
FROM uds.UDSRepositories R
WHERE R.Status = 2 AND R.ActiveDate <= getutcdate() AND (R.ExpiredDate is null OR R.ExpiredDate >= getutcdate())  
	  AND EXISTS (select top 1 CG.IdContainerGroup
						from dbo.ContainerGroup CG
						INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
						where CG.IdContainer = R.IdContainer and C_MSG.IdGroup is not null
							  and CG.UDSRights like '1%')
	  AND (@IDUDSTypology is null OR EXISTS (select top 1 RT.IdUDSRepositoryTypology
											 from uds.UDSRepositoryTypologies RT
											 where RT.IdUDSTypology = @IDUDSTypology and RT.IdUDSRepository = R.IdUDSRepository))
      AND (@PECAnnexedEnabled is null OR @PECAnnexedEnabled = 0 
	       OR (R.ModuleXML.exist('/*[(@PECEnabled) eq true()]') = 1 AND R.ModuleXML.exist('/*/*[local-name()=''Documents'']/*[local-name()=''DocumentAnnexed'']') = 1))
GROUP BY R.IdUDSRepository,
	     R.Name,
	     R.IdContainer,
	     R.Version,
	     R.Status,
		 R.ActiveDate,
		 R.ExpiredDate, 
		 R.RegistrationUser,
		 R.RegistrationDate,
		 R.LastChangedUser,
		 R.LastChangedDate,
	     R.SequenceCurrentNumber,
	     R.SequenceCurrentYear,
	     R.DSWEnvironment,
	     R.Alias
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
PRINT 'Modificata sql function [dbo].[UDSRepository_FX_ViewableRepositoriesByTypology]';
GO

ALTER FUNCTION [webapiprivate].[UDSRepository_FX_ViewableRepositoriesByTypology](
	@UserName nvarchar(256),
	@Domain nvarchar(256),
	@IDUDSTypology uniqueidentifier,
	@PECAnnexedEnabled bit
)
RETURNS TABLE
AS
RETURN
(
WITH
MySecurityGroups AS (
	SELECT SG.IdGroup 
	FROM [dbo].[SecurityGroups] SG 
	LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
	OR SG.AllUsers = 1
	GROUP BY SG.IdGroup
)

SELECT R.IdUDSRepository,
	   R.Name,
	   R.IdContainer as Container_IdContainer,
	   R.Version,
	   R.Status,
	   R.ActiveDate,
	   R.ExpiredDate, 
	   R.RegistrationUser,
	   R.RegistrationDate,
	   R.LastChangedUser,
	   R.LastChangedDate,
	   R.SequenceCurrentNumber,
	   R.SequenceCurrentYear,
	   R.DSWEnvironment,
	   R.Alias
FROM uds.UDSRepositories R
WHERE R.Status = 2 AND R.ActiveDate <= getutcdate() AND (R.ExpiredDate is null OR R.ExpiredDate >= getutcdate())  
	  AND EXISTS (select top 1 CG.IdContainerGroup
						from dbo.ContainerGroup CG
						INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
						where CG.IdContainer = R.IdContainer and C_MSG.IdGroup is not null
							  and (CG.UDSRights like '__1%' or CG.UDSRights like '___1%'))
	  AND (@IDUDSTypology is null OR EXISTS (select top 1 RT.IdUDSRepositoryTypology
											 from uds.UDSRepositoryTypologies RT
											 where RT.IdUDSTypology = @IDUDSTypology and RT.IdUDSRepository = R.IdUDSRepository))
      AND (@PECAnnexedEnabled is null OR @PECAnnexedEnabled = 0 
	       OR (R.ModuleXML.exist('/*[(@PECEnabled) eq true()]') = 1 AND R.ModuleXML.exist('/*/*[local-name()=''Documents'']/*[local-name()=''DocumentAnnexed'']') = 1))
GROUP BY R.IdUDSRepository,
	     R.Name,
	     R.IdContainer,
	     R.Version,
	     R.Status,
		 R.ActiveDate,
		 R.ExpiredDate, 
		 R.RegistrationUser,
		 R.RegistrationDate,
		 R.LastChangedUser,
		 R.LastChangedDate,
	     R.SequenceCurrentNumber,
	     R.SequenceCurrentYear,
	     R.DSWEnvironment,
	     R.Alias
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
PRINT 'Modificata sql function [dbo].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]';
GO
ALTER FUNCTION [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@Environment integer,
	@AnyEnvironment tinyint
)
RETURNS TABLE
AS
RETURN
(
		WITH 	
		MySecurityGroups AS (
			SELECT SG.IdGroup 
			FROM [dbo].[SecurityGroups] SG 
			LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
			WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
			OR SG.AllUsers = 1
			GROUP BY SG.IdGroup
		)

SELECT WR.[IdWorkflowRepository] AS UniqueId
      ,WR.[Name]
      ,WR.[Version]
      ,WR.[ActiveFrom]
      ,WR.[ActiveTo]
	  --Xaml = '' per ragioni di performance
      ,'' as Xaml
      ,WR.[Status]
      ,WR.[RegistrationUser]
      ,WR.[RegistrationDate]
      ,WR.[LastChangedUser]
      ,WR.[LastChangedDate]
	  ,WR.[Json]
	  ,WR.[CustomActivities]
	  ,WR.[DSWEnvironment]
	  ,WR.[Timestamp]
	FROM [dbo].[WorkflowRepositories] WR
	LEFT OUTER JOIN [dbo].[WorkflowRoles] WRR on WR.IdWorkflowRepository = WRR.IdWorkflowRepository	
	WHERE (WRR.IdWorkflowRepository IS NULL Or (
		EXISTS (SELECT TOP 1 RG.IdRole
				FROM dbo.RoleGroup RG
				INNER JOIN Role R on RG.idRole = R.idRole
				INNER JOIN  MySecurityGroups MSG ON RG.idGroup = MSG.idGroup
				WHERE R.idRole = WRR.IdRole and MSG.IdGroup IS NOT NULL
				AND ((WR.DSWEnvironment = 0)
				OR (WR.DSWEnvironment = 1 AND RG.ProtocolRights like '1%')
				OR (WR.DSWEnvironment = 2 AND RG.ResolutionRights like '1%')
				OR ((WR.DSWEnvironment = 4 OR WR.DSWEnvironment = 8 OR WR.DSWEnvironment >= 100)
					AND RG.DocumentSeriesRights like '1%')
				))
		)) AND WR.Status = 1 
			AND ((WR.DSWEnvironment = @Environment) 
				OR ( (WR.DSWEnvironment = 0 AND @AnyEnvironment = 1)))

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
PRINT 'Modificata sql function [dbo].[ DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle]';
GO
ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle](
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@FascicleId uniqueidentifier,
	@Year smallint = null,
	@Number int = null,
	@DocumentUnitName nvarchar(256) = null,
	@CategoryId smallint = null,
	@Subject nvarchar(256) = null
)
RETURNS INT
AS
	BEGIN
	DECLARE @CountDocumentUnits INT;
	WITH 	
	MySecurityGroups AS (
		SELECT SG.IdGroup 
		FROM [dbo].[SecurityGroups] SG 
		LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
		GROUP BY SG.IdGroup
	)
	
	SELECT @CountDocumentUnits = COUNT(DISTINCT DU.IdDocumentUnit)

	FROM cqrs.DocumentUnits DU
			 	INNER JOIN [dbo].[Container] C on DU.IdContainer = C.IdContainer
				INNER JOIN [dbo].[Category] CT on DU.IdCategory = CT.IdCategory
				LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
		
	WHERE      (@Year IS NULL OR DU.Year = @Year) 
			 	AND (@Number IS NULL OR DU.Number = @Number)
				AND (@DocumentUnitName IS NULL OR DU.DocumentUnitName = @DocumentUnitName)
				AND (@CategoryId IS NULL OR DU.IdCategory = @CategoryId)
				AND (@Subject IS NULL OR DU.Subject like '%'+@Subject+'%')
				AND ((exists (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1))
				  AND (
				     exists (select top 1 CG.idContainerGroup
					 from [dbo].[ContainerGroup] CG 
					 INNER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
					 where CG.IdContainer = DU.IdContainer AND C_MSG.IdGroup IS NOT NULL
					 AND (
					 (DU.Environment = 1 AND (CG.Rights like '__1%'))
					 OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					 OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					 OR ((DU.Environment = 7 OR DU.Environment > 99) AND (CG.UDSRights like '__1%'))
					)) 
					 OR exists (select top 1 RG.idRole
						from [dbo].[RoleGroup] RG
						INNER JOIN Role R on RG.idRole = R.idRole
						INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
						where  R.UniqueId = DUR.UniqueIdRole
							   AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
							   OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
						       OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
						       OR ((DU.Environment = 7 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
						       AND MSG.IdGroup IS NOT NULL)
				 )
		    )
			OR (not exists (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1)) 
			AND DU.Environment IN (SELECT CF.DSWEnvironment FROM [dbo].[CategoryFascicles] CF
						  INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = CF.IdCategory
						  WHERE CF.FascicleType = 1 AND F.IdFascicle = @FascicleId)))
			AND (DU.IdFascicle is null OR DU.IdFascicle != @FascicleId)

	RETURN @CountDocumentUnits
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
PRINT 'Modificata sql function [dbo].[Dossiers_FX_CountAuthorizedDossiers]';
GO
ALTER FUNCTION [webapiprivate].[Dossiers_FX_CountAuthorizedDossiers](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Year smallint null,
	@Number smallint null,
	@Subject nvarchar(255),
	@ContainerId smallint null
)
RETURNS INT
AS
	BEGIN
	DECLARE @CountDossiers INT;
	WITH 	
	MySecurityGroups AS (
		SELECT SG.IdGroup 
		FROM [dbo].[SecurityGroups] SG 
		LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
		GROUP BY SG.IdGroup
	)
	
	SELECT @CountDossiers = COUNT(DISTINCT Dossier.IdDossier)
	FROM dbo.Dossiers Dossier
	left join dbo.DossierContacts DC on Dossier.IdDossier = Dc.IdDossier
	left join dbo.Contact Contact on DC.IdContact = Contact.Incremental
	inner join dbo.Container C on Dossier.IdContainer = C.idContainer
	inner join dbo.DossierRoles DR on Dossier.IdDossier = DR.IdDossier and DR.RoleAuthorizationType = 0
	inner join dbo.Role R on DR.IdRole = R.idRole
		
	WHERE  (
			exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
					where CG.IdContainer = Dossier.IdContainer and C_MSG.IdGroup is not null
							and CG.DocumentRights like '___1%')
			or exists (select top 1 RG.idRole
					   from dbo.RoleGroup RG
					   INNER JOIN Role R on RG.idRole = R.idRole
					   INNER JOIN MySecurityGroups MSG on RG.idGroup = MSG.idGroup
					   where R.idRole = DR.IdRole and MSG.IdGroup is not null
							and RG.DocumentRights like '1%')
			)
			and (@Year is null or Dossier.Year = @Year)
		    and (@Number is null or Dossier.Number = @Number)
			and (@Subject is null or Dossier.Subject like '%'+@Subject+'%')
			and (@ContainerId is null or C.idContainer = @ContainerId)

	RETURN @CountDossiers
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
PRINT 'Modificata sql function [dbo].[ Dossiers_FX_HasInsertRight]';
GO
ALTER FUNCTION [webapiprivate].[Dossiers_FX_HasInsertRight]
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256)
)
RETURNS BIT 
AS
BEGIN
	declare @HasRight bit;

	WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	
	FROM dbo.ContainerGroup CG	
	WHERE
	exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					INNER JOIN [dbo].[Container] C on CG.idContainer=C.idContainer and C.IsActive = 1 AND C.DocmLocation IS NOT NULL
					INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
					where C_MSG.IdGroup IS NOT NULL AND CG.DocumentRights LIKE '1%')
			
	RETURN @HasRight
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
PRINT 'Modificata sql function [dbo].[Dossiers_FX_HasManageableRight]';
GO
ALTER FUNCTION [webapiprivate].[Dossiers_FX_HasManageableRight] 
(
    @UserName nvarchar(256), 
    @Domain nvarchar(256),
	@IdDossier uniqueidentifier
)
RETURNS BIT
AS
BEGIN
    declare @HasRight bit;
	
		WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM dbo.Dossiers D
	LEFT OUTER JOIN [dbo].[DossierRoles] DR on DR.IdDossier = D.IdDossier
	WHERE D.IdDossier = @IdDossier
	and (	exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
				INNER JOIN Role R on RG.idRole = R.idRole
				where  R.IdRole = DR.IdRole AND DR.RoleAuthorizationType=0 AND
				MSG.IdGroup IS NOT NULL  AND (RG.DocumentRights like '1%')
				)
				or exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
					where CG.IdContainer = D.IdContainer and C_MSG.IdGroup is not null
							and (CG.DocumentRights like '1%' OR CG.DocumentRights like '_1%'))
			)

				RETURN @HasRight;
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
PRINT 'Modificata sql function [dbo].[Dossiers_FX_HasModifyRight]';
GO
ALTER FUNCTION [webapiprivate].[Dossiers_FX_HasModifyRight]
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@IdDossier uniqueidentifier
)

RETURNS BIT 
AS
BEGIN
	declare @HasRight bit;

	WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain= @Domain
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    )
	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM dbo.Dossiers D
	WHERE D.IdDossier = @IdDossier
	and exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
					INNER JOIN Container C on CG.IdContainer = C.IdContainer and C.isActive = 1 and C.DocmLocation IS NOT NULL
					where CG.IdContainer = D.IdContainer and C_MSG.IdGroup is not null
							and CG.DocumentRights like '_1%')
			

			
	RETURN @HasRight
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
PRINT 'Modificata sql function [dbo].[Dossiers_FX_HasViewableRight]';
GO
ALTER FUNCTION [webapiprivate].[Dossiers_FX_HasViewableRight] 
(
    @UserName nvarchar(256), 
    @Domain nvarchar(256),
	@IdDossier uniqueidentifier
)
RETURNS BIT
AS
BEGIN
    declare @HasRight bit;

	WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM dbo.Dossiers D
	LEFT OUTER JOIN [dbo].[DossierRoles] DR on DR.IdDossier = D.IdDossier
	WHERE D.IdDossier = @IdDossier
	and (	exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
				INNER JOIN Role R on RG.idRole = R.idRole
				where  R.IdRole = DR.IdRole AND
				MSG.IdGroup IS NOT NULL AND (RG.DocumentRights like '1%')
				)
			or exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
					where CG.IdContainer = D.IdContainer and C_MSG.IdGroup is not null
							and CG.DocumentRights like '___1%')
			)

		RETURN @HasRight;
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
PRINT 'Modificata sql function [dbo].[Fascicle_FX_HasDocumentVisibilityRight]';
GO
ALTER FUNCTION [webapiprivate].[Fascicle_FX_HasDocumentVisibilityRight]
(
       @UserName nvarchar(256), 
       @Domain nvarchar(256),
       @IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
       declare @HasRight bit;
       declare @EmptyRights nvarchar(10);
       set @EmptyRights = '0000000000';

       WITH
       MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    )

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Fascicles] F
       WHERE F.IdFascicle = @IdFascicle
       AND F.VisibilityType = 1
       AND ( exists (select top 1 RG.idRole
                           from [dbo].[RoleGroup] RG
                           INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
                           INNER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = F.IdFascicle
                           where  RG.IdRole = FR.IdRole AND FR.RoleAuthorizationType in (0, 1) AND
                           MSG.IdGroup IS NOT NULL AND ((RG.ProtocolRights <> @EmptyRights)
                           OR (RG.ResolutionRights <> @EmptyRights)
                           OR (RG.DocumentRights <> @EmptyRights)
                           OR (RG.DocumentSeriesRights <> @EmptyRights))
                           )
             OR 
                exists (select top 1 CG.idCategory
                           from [dbo].[CategoryGroup] CG
                           INNER JOIN MySecurityGroups MSG on CG.IdGroup = MSG.IdGroup
                           where F.IdCategory = CG.IdCategory AND MSG.IdGroup IS NOT NULL AND CG.ProtocolRights like '__1%'
                           )
       )
       
       RETURN @HasRight

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
PRINT 'Modificata sql function [dbo].[Fascicle_FX_IsCurrentUserManagerOnActivityFascicle]';
GO
ALTER FUNCTION [webapiprivate].[Fascicle_FX_IsCurrentUserManagerOnActivityFascicle]
(
    @Description nvarchar(256),
	@IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
	declare @IsManager bit;
	WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Description = @Description 
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    )

	SELECT  @IsManager = CAST(COUNT(1) AS BIT)
	FROM [dbo].[Fascicles] F
	LEFT OUTER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = F.IdFascicle
	WHERE F.IdFascicle = @IdFascicle
	AND exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
				INNER JOIN Role R on RG.idRole = R.idRole
				where  R.IdRole = FR.IdRole AND FR.RoleAuthorizationType = 0 AND
				MSG.IdGroup IS NOT NULL 
				)
	
	RETURN @IsManager
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
PRINT N'Modifica StoredProcedure [Contact_Insert] ';
GO

ALTER PROCEDURE [dbo].[Contact_Insert] 
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
	@FullIncrementalPath varchar (255)
	
AS

	DECLARE @LastUsedIdContact INT, @EntityId INT, @ContactId uniqueidentifier, @FullIncrementalFatherPath varchar(255)

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION ContactInsert

	SELECT top(1) @LastUsedIdContact = Incremental FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Contact] ORDER BY Incremental DESC
	IF(@LastUsedIdContact is null)
	BEGIN
		SET @LastUsedIdContact = 0
	END

	SET @EntityId = @LastUsedIdContact + 1
	SET @ContactId = newid()
	SET @FullIncrementalPath = @EntityId

	SET @FullIncrementalFatherPath = (SELECT FullIncrementalPath FROM dbo.Contact WHERE [Incremental] = @IncrementalFather)
	IF(@FullIncrementalFatherPath IS NOT NULL)
	BEGIN
		SET @FullIncrementalPath = @FullIncrementalFatherPath + '|' + CAST(@EntityId AS VARCHAR(50))
	END

	INSERT INTO  <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
                                                                       [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
																	   [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
																	   [UniqueId],[Language],[Nationality],[BirthPlace]
																	  )
       VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
	           @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
			   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
	           @ContactId,@Language, @Nationality, @BirthPlace
			)

	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN	 
		INSERT INTO <DBAtti, varcahr(50), DBAtti>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
																		   [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
																		   [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
																		   [UniqueId],[Language],[Nationality],[BirthPlace]
																		  )
		   VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
				   @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
				   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
				   @ContactId,@Language, @Nationality, @BirthPlace
				)
		END
	
	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 	
		BEGIN	 
		INSERT INTO <DBPratiche, varcahr(50), DBPratiche>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
																		   [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
																		   [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
																		   [UniqueId],[Language],[Nationality],[BirthPlace]
																		  )
		   VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
				   @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
				   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
				   @ContactId,@Language, @Nationality, @BirthPlace
				)
		END

	COMMIT TRANSACTION ContactInsert

	SELECT [Incremental] as EntityId,[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
																		   [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail] as CertifiedMail,[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
																		   [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
																		   [UniqueId],[Language],[Nationality],[BirthPlace],[Timestamp] FROM <DBProtocollo, varchar(50), DBProtocollo>.dbo.Contact WHERE Incremental = @EntityId
																	   
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
			@ErrorSeverity, 
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

PRINT N'Modifica StoredProcedure [Contact_Update]';
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
		@Timestamp_Original timestamp
	AS 


		SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
		BEGIN TRANSACTION ContactUpdate
		
	BEGIN TRY
		
		--Aggiornamento contatto
		UPDATE  <DBProtocollo, varcahr(50), DBProtocollo>.[dbo].[Contact] SET 
			[IdContactType]=@IdContactType,  [IncrementalFather] = @IncrementalFather,[Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
			[City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
			[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
			[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
			[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace Where [Incremental] = @Incremental
			
		
		IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN
			
			UPDATE  <DBAtti, varcahr(50), DBAtti>.[dbo].[Contact] SET 
			[IdContactType] = @IdContactType,[IncrementalFather] = @IncrementalFather, [Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
			[City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
			[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
			[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
			[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace Where [Incremental] = @Incremental 

		END

		IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
			BEGIN 
				UPDATE <DBPratiche, varcahr(50), DBPratiche>.[dbo].[Contact] SET 
					[IdContactType] = @IdContactType, [IncrementalFather] = @IncrementalFather, [Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
					[City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
					[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
					[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
					[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace Where [Incremental] = @Incremental 

			END

		COMMIT TRANSACTION ContactUpdate
	
		SELECT [Incremental] as EntityId,[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
                                                                       [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail] as CertifiedMail,[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
																	   [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
																	   [UniqueId],[Language],[Nationality],[BirthPlace],[Timestamp] FROM <DBProtocollo, varcahr(50), DBProtocollo>.[dbo].[Contact] WHERE Incremental = @Incremental
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
			@ErrorSeverity, 
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

PRINT N'Modifica StoredProcedure [Contact_Delete]';
GO

	ALTER PROCEDURE [dbo].[Contact_Delete] 
		@Incremental int,
		@Timestamp_Original timestamp,
		@IdPlaceName smallint,
		@idRole smallint,
		@idRoleRootContact smallint,
		@IdTitle smallint
	AS
 
	DECLARE @LastChangedDate datetimeoffset(7)

	SET @LastChangedDate = GETUTCDATE()

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION ContactDelete
	
	BEGIN TRY

		----Cancellazione logica contatto
			UPDATE <DBProtocollo, varcahr(50), DBProtocollo>.[dbo].[Contact]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [Incremental] = @Incremental
		
		IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN
			UPDATE  <DBAtti, varcahr(50), DBAtti>.[dbo].[Contact]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [Incremental] = @Incremental
		END

		IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
		BEGIN 
			UPDATE <DBPratiche, varcahr(50), DBPratiche>.[dbo].[Contact]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [Incremental] = @Incremental
		END

		COMMIT TRANSACTION ContactDelete
	END TRY
	
	BEGIN CATCH 
		ROLLBACK TRANSACTION ContactDelete
		
		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		RAISERROR 
			(
			@ErrorSeverity, 
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

PRINT N'Modifica StoredProcedure [DossierFolder_Insert]';
GO

	ALTER PROCEDURE [dbo].[DossierFolder_Insert] 
	@IdDossierFolder uniqueidentifier, 
	@IdDossier uniqueidentifier,
	@IdFascicle uniqueidentifier, 
	@IdCategory smallint,
	@Name nvarchar(256), 
    @Status smallint,
	@JsonMetadata nvarchar(max), 
	@RegistrationDate datetimeoffset(7),
    @RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256), 
	@ParentInsertId uniqueidentifier
	AS
	
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	
	
	BEGIN TRY
		BEGIN TRANSACTION InsertDossierFolder
		-- Recupero il parent node
		IF @ParentInsertId IS NOT NULL
			BEGIN
				SELECT @parentNode = [DossierFolderNode] FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @ParentInsertId
			END
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[DossierFolders]  
				           WHERE [DossierFolderNode] = hierarchyid::GetRoot())
				BEGIN
					SET @parentNode = hierarchyid::GetRoot()
				END	
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[DossierFolders]  
				           WHERE [DossierFolderNode] = (select MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() AND @IdDossier is not null and [IdDossier] = @IdDossier))
				BEGIN
					SET @parentNode = (select MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() AND [IdDossier] = @IdDossier)
					PRINT @parentNode.ToString()
				END	
			END

		-- Recupero il max node in base al parent node
		SELECT @maxNode = MAX([DossierFolderNode]) FROM [dbo].[DossierFolders] WHERE [DossierFolderNode].GetAncestor(1) = @parentNode;

		IF @ParentInsertId IS NOT NULL			
			BEGIN
				SET @node = @parentNode.GetDescendant(@maxNode, NULL)
			END			
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[DossierFolders] WHERE [DossierFolderNode] = hierarchyid::GetRoot() 
							OR [DossierFolderNode] = (select MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() and @IdDossier is not null and [IdDossier] = @IdDossier))
				BEGIN
					SET @node = @parentNode.GetDescendant(@maxNode, NULL)
					PRINT @node.ToString()
				END	
				ELSE
				BEGIN
					SET @node = hierarchyid::GetRoot()
					PRINT @node.ToString()
				END
			END
	
		
		INSERT INTO [dbo].[DossierFolders]([DossierFolderNode],[IdDossierFolder],[IdDossier],[IdFascicle],[IdCategory],[Name],[Status],[JsonMetadata],
		[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
		VALUES (@node, @IdDossierFolder, @IdDossier, @IdFascicle, @IdCategory, @Name, @Status, @JsonMetadata, @RegistrationDate, @RegistrationUser, NULL, NULL)

		SELECT [DossierFolderNode],[IdDossierFolder] AS UniqueId,[IdDossier],[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[JsonMetadata],[DossierFolderPath],[DossierFolderLevel],[DossierFolderParentNode],[ParentInsertId],[Timestamp] 
		FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder

		COMMIT TRANSACTION InsertDossierFolder
	END TRY
	
	BEGIN CATCH 
		ROLLBACK TRANSACTION InsertDossierFolder
		
		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		RAISERROR 
			(
			@ErrorSeverity, 
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

PRINT N'Modifica StoredProcedure [DossierFolder_Update]';
GO

	ALTER PROCEDURE [dbo].[DossierFolder_Update] 
       @IdDossierFolder uniqueidentifier, 
       @IdDossier uniqueidentifier,
       @IdFascicle uniqueidentifier, 
       @IdCategory smallint,
       @Name nvarchar(256), 
       @Status smallint,
       @JsonMetadata nvarchar(max),
	   @RegistrationDate datetimeoffset(7),
       @RegistrationUser nvarchar(256), 
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256),
	   @ParentInsertId uniqueidentifier,
	   @Timestamp_Original timestamp
	AS
		
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
		
	BEGIN TRY
		BEGIN TRANSACTION UpdateDossierFolder
		UPDATE [dbo].[DossierFolders] SET [IdFascicle] = @IdFascicle, [IdCategory] = @IdCategory, [Name] = @Name, [Status] = @Status, 
												[JsonMetadata] = @JsonMetadata, [LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser
			  WHERE [IdDossierFolder] = @IdDossierFolder

              SELECT [DossierFolderNode],[IdDossierFolder] as UniqueId,[IdDossier],[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
              [Name],[JsonMetadata],[DossierFolderPath],[DossierFolderLevel],[DossierFolderParentNode],[ParentInsertId],[Timestamp] 
			  FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder
		
		COMMIT TRANSACTION UpdateDossierFolder
	END TRY
		
	BEGIN CATCH 
		ROLLBACK TRANSACTION UpdateDossierFolder
		
		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		RAISERROR 
			(
			@ErrorSeverity, 
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

PRINT N'Modifica StoredProcedure [DossierFolder_Delete] ';
GO

	ALTER  PROCEDURE [dbo].[DossierFolder_Delete] 
       @IdDossierFolder uniqueidentifier, 
       @IdDossier uniqueidentifier,
       @IdFascicle uniqueidentifier, 
       @IdCategory smallint,
	   @Timestamp_Original timestamp
	AS
		
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  	
	
	BEGIN TRY
		BEGIN TRANSACTION DeleteDossierFolder
		IF EXISTS (SELECT TOP 1 [IdDossierFolder] FROM [DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder)
		 BEGIN 
              DELETE [dbo].[DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder
	     END 
		
		
		COMMIT TRANSACTION DeleteDossierFolder
	END TRY
		
	BEGIN CATCH 
		ROLLBACK TRANSACTION DeleteDossierFolder
		
		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		RAISERROR 
			(
			@ErrorSeverity, 
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