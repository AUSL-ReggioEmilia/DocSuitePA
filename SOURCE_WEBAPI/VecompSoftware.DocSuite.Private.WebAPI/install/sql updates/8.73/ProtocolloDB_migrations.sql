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

PRINT N'Modifica SQLFUNCTION [webapiprivate].[Dossiers_FX_AuthorizedDossiers] ';
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
	@IdMetadataRepository nvarchar(255),
	@MetadataValue nvarchar (255),
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
			   AND (@IdMetadataRepository is null or Dossier.IdMetadataRepository = @IdMetadataRepository)
			   AND (@MetadataValue is null or Dossier.JsonMetadata like '%'+@MetadataValue+'%')
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle] ';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle] 
(	
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@FascicleId uniqueidentifier,
	@Year smallint,
	@Number nvarchar(256),
	@DocumentUnitName nvarchar(256),
	@CategoryId smallint,
	@ContainerId smallint,
	@Subject nvarchar(256),
	@IncludeChildClassification bit,
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
		LEFT JOIN [dbo].[SecurityUsers] SU ON SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
		GROUP BY SG.IdGroup
	), 	
	MyCategory AS (
		SELECT TOP 1 C.IdCategory
		FROM [dbo].[Category] C 
		INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = C.IdCategory
		WHERE F.IdFascicle = @FascicleId
		GROUP BY C.IdCategory
	), 	
	MyCategoryFascicles AS (
		SELECT CF.IdCategory
		FROM [dbo].[CategoryFascicles] CF 
		INNER JOIN [dbo].[Category] C ON C.idCategory = CF.IdCategory
		WHERE (EXISTS (SELECT MyCategory.IdCategory FROM MyCategory WHERE CF.IdCategory = MyCategory.IdCategory and CF.FascicleType = 1))
			  OR (EXISTS (SELECT MyCategory.IdCategory FROM MyCategory WHERE MyCategory.IdCategory in (SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|')) and CF.FascicleType = 0))
		GROUP BY CF.IdCategory
	),
	CategoryChildren AS (
		SELECT CC.IdCategory
		FROM [dbo].Category CC
		WHERE (@IncludeChildClassification = 0 AND CC.IdCategory = @CategoryId ) OR ( @IncludeChildClassification = 1 AND (CC.FullIncrementalPath like CONVERT(varchar(10), @CategoryId) +'|%' OR CC.IdCategory = @CategoryId))
				GROUP BY CC.IdCategory
	),
	
	MydocumentUnits AS (
			SELECT T.IdDocumentUnit, T.rownum FROM
			(SELECT DU.[IdDocumentUnit], row_number() over(order by DU.[IdDocumentUnit]) as rownum 
			 FROM cqrs.DocumentUnits DU
			 	INNER JOIN [dbo].[Container] C ON DU.IdContainer = C.IdContainer
				INNER JOIN [dbo].[Category] CT ON DU.IdCategory = CT.IdCategory
				LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR ON DUR.IdDocumentUnit = DU.IdDocumentUnit
			 WHERE (@Year IS NULL OR DU.Year = @Year) 
			 	AND (@Number IS NULL OR DU.Title like  '____/%' + REPLACE(@Number, '|', '/'))
				AND (@DocumentUnitName IS NULL OR DU.DocumentUnitName = @DocumentUnitName)
				AND (@CategoryId IS NULL OR EXISTS (SELECT TOP 1 CC.IdCategory FROM CategoryChildren CC WHERE DU.IdCategory = CC.IdCategory))
				AND (@ContainerId IS NULL OR DU.IdContainer = @ContainerId)
				AND (@Subject IS NULL OR DU.Subject like '%'+@Subject+'%')
				AND ((EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1))
				  AND (
				     EXISTS (SELECT top 1 CG.idContainerGroup
					 FROM [dbo].[ContainerGroup] CG 
					 INNER JOIN MySecurityGroups C_MSG ON CG.IdGroup = C_MSG.IdGroup
					 WHERE CG.IdContainer = DU.IdContainer AND C_MSG.IdGroup IS NOT NULL
					 AND (
					 (DU.Environment = 1 AND (CG.Rights like '__1%'))
					 OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					 OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					 OR ((DU.Environment = 7 OR DU.Environment > 99) AND (CG.UDSRights like '__1%'))
					)) 
					 OR EXISTS (SELECT top 1 RG.idRole
						FROM [dbo].[RoleGroup] RG
						INNER JOIN Role R ON RG.idRole = R.idRole
						INNER JOIN MySecurityGroups MSG ON RG.IdGroup = MSG.IdGroup
						WHERE  R.UniqueId = DUR.UniqueIdRole
							   AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
							   OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
						       OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
						       OR ((DU.Environment = 7 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
						       AND MSG.IdGroup IS NOT NULL)
				 )
		    )
			OR (NOT EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1)) 
			AND DU.Environment IN (SELECT CF.DSWEnvironment FROM [dbo].[CategoryFascicles] CF
						  INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = CF.IdCategory
						  WHERE CF.FascicleType = 1 AND F.IdFascicle = @FascicleId)))
			AND (DU.IdFascicle IS NULL OR DU.IdFascicle != @FascicleId)
			Group by DU.[IdDocumentUnit]) T WHERE T.rownum > @Skip AND T.rownum <= @Top
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
		  ,(SELECT CAST(COUNT(1) AS BIT) from MyCategoryFascicles where MyCategoryFascicles.IdCategory = CT.IdCategory) as IsFascicolable
		  from cqrs.DocumentUnits DU
	INNER JOIN [dbo].[Container] C ON DU.IdContainer = C.IdContainer
	INNER JOIN [dbo].[Category] CT ON DU.IdCategory = CT.IdCategory
WHERE EXISTS (SELECT MydocumentUnits.[IdDocumentUnit] from MydocumentUnits where DU.[IdDocumentUnit] = MydocumentUnits.IdDocumentUnit)
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle] ';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle](
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@FascicleId uniqueidentifier,
	@Year smallint,
	@Number nvarchar(256),
	@DocumentUnitName nvarchar(256),
	@CategoryId smallint,
	@ContainerId smallint,
	@Subject nvarchar(256),
	@IncludeChildClassification bit
)
RETURNS INT
AS
	BEGIN
	DECLARE @CountDocumentUnits INT;
WITH 	
	MySecurityGroups AS (
		SELECT SG.IdGroup 
		FROM [dbo].[SecurityGroups] SG 
		LEFT JOIN [dbo].[SecurityUsers] SU ON SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
		GROUP BY SG.IdGroup
	),
	CategoryChildren AS (
		SELECT CC.IdCategory
		FROM [dbo].Category CC
		WHERE (@IncludeChildClassification = 0 AND CC.IdCategory = @CategoryId ) OR ( @IncludeChildClassification = 1 AND (CC.FullIncrementalPath like CONVERT(varchar(10), @CategoryId) +'|%' OR CC.IdCategory = @CategoryId))
				GROUP BY CC.IdCategory
	)
	
	SELECT @CountDocumentUnits = COUNT(DISTINCT DU.IdDocumentUnit)

	FROM cqrs.DocumentUnits DU
			 	INNER JOIN [dbo].[Container] C ON DU.IdContainer = C.IdContainer
				INNER JOIN [dbo].[Category] CT ON DU.IdCategory = CT.IdCategory
				LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR ON DUR.IdDocumentUnit = DU.IdDocumentUnit
		
	WHERE      (@Year IS NULL OR DU.Year = @Year) 
			 	AND (@Number IS NULL OR DU.Title like  '____/%' +  REPLACE(@Number, '|', '/'))
				AND (@DocumentUnitName IS NULL OR DU.DocumentUnitName = @DocumentUnitName)
				AND (@CategoryId IS NULL OR EXISTS ( SELECT TOP 1 CC.IdCategory FROM CategoryChildren CC WHERE DU.IdCategory = CC.IdCategory))
				AND (@ContainerId IS NULL OR DU.IdContainer = @ContainerId)
				AND (@Subject IS NULL OR DU.Subject like '%'+@Subject+'%')
				AND ((EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1))
				  AND (
				     EXISTS (SELECT top 1 CG.idContainerGroup
					 FROM [dbo].[ContainerGroup] CG 
					 INNER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
					 WHERE CG.IdContainer = DU.IdContainer AND C_MSG.IdGroup IS NOT NULL
					 AND (
					 (DU.Environment = 1 AND (CG.Rights like '__1%'))
					 OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					 OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					 OR ((DU.Environment = 7 OR DU.Environment > 99) AND (CG.UDSRights like '__1%'))
					)) 
					 OR exists (SELECT top 1 RG.idRole
						FROM [dbo].[RoleGroup] RG
						INNER JOIN Role R on RG.idRole = R.idRole
						INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
						WHERE  R.UniqueId = DUR.UniqueIdRole
							   AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
							   OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
						       OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
						       OR ((DU.Environment = 7 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
						       AND MSG.IdGroup IS NOT NULL)
				 )
		    )
			OR (NOT EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1)) 
			AND DU.Environment IN (SELECT CF.DSWEnvironment FROM [dbo].[CategoryFascicles] CF
						  INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = CF.IdCategory
						  WHERE CF.FascicleType = 1 AND F.IdFascicle = @FascicleId)))
			AND (DU.IdFascicle IS NULL OR DU.IdFascicle != @FascicleId)

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
PRINT N'Creazione StoredProcedure [dbo].[ContainerProperty_Insert] ';
GO

CREATE PROCEDURE [dbo].[ContainerProperty_Insert]
	@IdContainerProperty uniqueidentifier,
	@idContainer smallint,
	@Name nvarchar(256),
    @ContainerType smallint,
    @ValueInt bigint,
    @ValueDate datetime,
    @ValueDouble float,
    @ValueBoolean bit,
    @ValueGuid uniqueidentifier,
    @ValueString nvarchar(max),
	@RegistrationDate datetimeoffset(7),
    @RegistrationUser nvarchar(256),
    @LastChangedDate datetimeoffset(7),
    @LastChangedUser nvarchar(256)
AS
BEGIN
	
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
    BEGIN TRY
    BEGIN TRANSACTION ContainerPropertyInsert		
		INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[ContainerProperties] ([IdContainerProperty],[idContainer],[Name]
           ,[ContainerType],[ValueInt],[ValueDate],[ValueDouble],[ValueBoolean],[ValueGuid],[ValueString]
           ,[RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate])
		VALUES
           (@IdContainerProperty,@idContainer,@Name,@ContainerType,@ValueInt,@ValueDate,@ValueDouble
           ,@ValueBoolean,@ValueGuid,@ValueString,@RegistrationUser,@RegistrationDate,@LastChangedUser
           ,@LastChangedDate)

		IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN
			INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[ContainerProperties] ([IdContainerProperty],[idContainer],[Name]
			   ,[ContainerType],[ValueInt],[ValueDate],[ValueDouble],[ValueBoolean],[ValueGuid],[ValueString]
			   ,[RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate])
			VALUES
			   (@IdContainerProperty,@idContainer,@Name,@ContainerType,@ValueInt,@ValueDate,@ValueDouble
			   ,@ValueBoolean,@ValueGuid,@ValueString,@RegistrationUser,@RegistrationDate,@LastChangedUser
			   ,@LastChangedDate)
		END

		IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
		BEGIN 
			INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[ContainerProperties] ([IdContainerProperty],[idContainer],[Name]
			   ,[ContainerType],[ValueInt],[ValueDate],[ValueDouble],[ValueBoolean],[ValueGuid],[ValueString]
			   ,[RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate])
			VALUES
			   (@IdContainerProperty,@idContainer,@Name,@ContainerType,@ValueInt,@ValueDate,@ValueDouble
			   ,@ValueBoolean,@ValueGuid,@ValueString,@RegistrationUser,@RegistrationDate,@LastChangedUser
			   ,@LastChangedDate)
		END

	COMMIT TRANSACTION ContainerPropertyInsert
    SELECT @IdContainerProperty as idContainerProperty
    END TRY

    BEGIN CATCH 
		ROLLBACK TRANSACTION ContainerPropertyInsert
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
PRINT N'Creazione StoredProcedure [dbo].[ContainerProperty_Update] ';
GO

CREATE PROCEDURE [dbo].[ContainerProperty_Update]
	@IdContainerProperty uniqueidentifier,
	@idContainer smallint,
	@Name nvarchar(256),
    @ContainerType smallint,
    @ValueInt bigint,
    @ValueDate datetime,
    @ValueDouble float,
    @ValueBoolean bit,
    @ValueGuid uniqueidentifier,
    @ValueString nvarchar(max),
	@RegistrationDate datetimeoffset(7),
    @RegistrationUser nvarchar(256),
    @LastChangedDate datetimeoffset(7),
    @LastChangedUser nvarchar(256)
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
    BEGIN TRY
    BEGIN TRANSACTION ContainerPropertyUpdate		
		UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[ContainerProperties] SET [idContainer] = @idContainer,[Name] = @Name
           ,[ContainerType] = @ContainerType,[ValueInt] = @ValueInt,[ValueDate] = @ValueDate,[ValueDouble] = @ValueDouble
		   ,[ValueBoolean] = @ValueBoolean,[ValueGuid] = @ValueGuid,[ValueString] = @ValueString
           ,[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate
		WHERE IdContainerProperty = @IdContainerProperty

		IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN
			UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[ContainerProperties] SET [idContainer] = @idContainer,[Name] = @Name
			   ,[ContainerType] = @ContainerType,[ValueInt] = @ValueInt,[ValueDate] = @ValueDate,[ValueDouble] = @ValueDouble
			   ,[ValueBoolean] = @ValueBoolean,[ValueGuid] = @ValueGuid,[ValueString] = @ValueString
			   ,[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate
			WHERE IdContainerProperty = @IdContainerProperty
		END

		IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
		BEGIN 
			UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[ContainerProperties] SET [idContainer] = @idContainer,[Name] = @Name
			   ,[ContainerType] = @ContainerType,[ValueInt] = @ValueInt,[ValueDate] = @ValueDate,[ValueDouble] = @ValueDouble
			   ,[ValueBoolean] = @ValueBoolean,[ValueGuid] = @ValueGuid,[ValueString] = @ValueString
			   ,[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate
			WHERE IdContainerProperty = @IdContainerProperty
		END

	COMMIT TRANSACTION ContainerPropertyUpdate
    END TRY

    BEGIN CATCH 
		ROLLBACK TRANSACTION ContainerPropertyUpdate
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
PRINT N'Creazione StoredProcedure [dbo].[ContainerProperty_Delete] ';
GO

CREATE PROCEDURE [dbo].[ContainerProperty_Delete]
	@IdContainerProperty uniqueidentifier,
	@idContainer smallint
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
    BEGIN TRY
    BEGIN TRANSACTION ContainerPropertyDelete		
		DELETE FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[ContainerProperties]
		WHERE IdContainerProperty = @IdContainerProperty

		IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN
			DELETE FROM <DBAtti, varchar(50), DBAtti>.[dbo].[ContainerProperties]
			WHERE IdContainerProperty = @IdContainerProperty
		END

		IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
		BEGIN 
			DELETE FROM <DBPratiche, varchar(50), DBPratiche>.[dbo].[ContainerProperties]
			WHERE IdContainerProperty = @IdContainerProperty
		END

	COMMIT TRANSACTION ContainerPropertyDelete
    END TRY

    BEGIN CATCH 
		ROLLBACK TRANSACTION ContainerPropertyDelete
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
PRINT 'Creata funzione [webapiprivate].[Fascicles_FX_IsProcedureSecretary]'
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_IsProcedureSecretary](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@CategoryId smallint
)
RETURNS BIT
AS
BEGIN
DECLARE @IsSecretary BIT;
SELECT @IsSecretary = CAST(COUNT(1) AS BIT)
FROM   dbo.RoleUser RU
WHERE  RU.Type = 'SP'
       and RU.Account = @Domain + '\' + @UserName
       and exists (select top 1 SU.idUser
                                 FROM   dbo.SecurityUsers SU
                                        inner join dbo.SecurityGroups SG
                                          on SU.idGroup = SG.idGroup
                                        inner join dbo.CategoryGroup CG
                                          on SG.idGroup = CG.idGroup
                                 WHERE  CG.IdCategory = -31848 and SU.Description = @UserName and SU.UserDomain = @Domain)
RETURN @IsSecretary
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
PRINT 'Creata funzione [webapiprivate].[WorkflowRepository_FX_HasAuthorizedWorkflowRepositories]'
GO

CREATE FUNCTION [webapiprivate].[WorkflowRepository_FX_HasAuthorizedWorkflowRepositories]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@Environment integer,
	@AnyEnvironment tinyint
)
RETURNS BIT
AS
BEGIN
DECLARE @HasWorkflows BIT;
		WITH 	
		MySecurityGroups AS (
			SELECT SG.IdGroup 
			FROM [dbo].[SecurityGroups] SG 
			LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
			WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
			OR SG.AllUsers = 1
			GROUP BY SG.IdGroup
		)

SELECT @HasWorkflows = CAST(COUNT(1) AS BIT)
FROM [dbo].[WorkflowRepositories] WR
LEFT OUTER JOIN [dbo].[WorkflowRoles] WRR on WR.IdWorkflowRepository = WRR.IdWorkflowRepository	
WHERE (WRR.IdWorkflowRepository IS NULL Or (
		EXISTS (SELECT TOP 1 RG.IdRole
				FROM dbo.RoleGroup RG
				INNER JOIN  MySecurityGroups MSG ON RG.idGroup = MSG.idGroup
				WHERE RG.idRole = WRR.IdRole and MSG.IdGroup IS NOT NULL
				AND ((WR.DSWEnvironment = 0)
				OR (WR.DSWEnvironment = 1 AND RG.ProtocolRights like '1%')
				OR (WR.DSWEnvironment = 2 AND RG.ResolutionRights like '1%')
				OR ((WR.DSWEnvironment = 4 OR WR.DSWEnvironment = 8 OR WR.DSWEnvironment >= 100)
					AND RG.DocumentSeriesRights like '1%')
				))
		)) AND WR.Status = 1 
			AND ((WR.DSWEnvironment = @Environment) 
				OR ( (WR.DSWEnvironment = 0 AND @AnyEnvironment = 1)))
RETURN @HasWorkflows;
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
PRINT 'Modificata funzione [webapiprivate].[UDSRepository_FX_ViewableRepositoriesByTypology]'
GO

ALTER FUNCTION [webapiprivate].[UDSRepository_FX_ViewableRepositoriesByTypology](
	@IDUDSTypology uniqueidentifier,
	@PECAnnexedEnabled bit
)
RETURNS TABLE
AS
RETURN
(
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