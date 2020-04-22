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
PRINT N'Creazione funzione [webapiprivate].[UDSRepository_FX_AvailableCQRSPublishedUDSRepositories]';
GO

CREATE FUNCTION [webapiprivate].[UDSRepository_FX_AvailableCQRSPublishedUDSRepositories](
	@IDUDSTypology uniqueidentifier,
	@Name nvarchar(255),
	@Alias nvarchar(4),
	@ContainerId smallint

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
	  AND (@IDUDSTypology is null OR NOT EXISTS (select top 1 RT.IdUDSRepositoryTypology
											 from uds.UDSRepositoryTypologies RT
											 where RT.IdUDSTypology = @IDUDSTypology and RT.IdUDSRepository = R.IdUDSRepository))
      AND (R.ModuleXML.exist('/*[(@DocumentUnitSynchronizeEnabled) eq true()]') = 1)
	  AND (@Name is null or R.Name like '%'+@Name+'%')
	  AND (@Alias is null or R.Alias like '%'+@Alias+'%')
	  AND (@ContainerId is null or R.IdContainer = @ContainerId)
	  AND R.Version = (select MAX(UR.Version) from uds.UDSRepositories UR where UR.IdUDSRepository = R.IdUDSRepository)
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
PRINT N'Creazione funzione [webapiprivate].[Category_FX_GeAvailablePeriodicCategoryFascicles]';
GO

CREATE FUNCTION [webapiprivate].[Category_FX_GeAvailablePeriodicCategoryFascicles]
(
	@idCategory smallint
)
RETURNS TABLE
AS
RETURN
(	 
	  SELECT [CF].[IdCategoryFascicle] as UniqueId
      ,[CF].[IdCategory]
      ,[CF].[IdFasciclePeriod]
      ,[CF].[FascicleType]
      ,[CF].[Manager]
      ,[CF].[DSWEnvironment]
      ,[CF].[RegistrationUser]
      ,[CF].[RegistrationDate]
      ,[CF].[LastChangedUser]
      ,[CF].[LastChangedDate]
      ,[CF].[Timestamp]
	    FROM [dbo].[CategoryFascicles] CF
		WHERE [idCategory] = @idCategory 
		AND [FascicleType] = 2  AND NOT EXISTS (SELECT DSWEnvironment FROM [dbo].[Fascicles] 
															WHERE [idCategory] = @idCategory AND [FascicleType] = 2 
																AND [StartDate] < GETUTCDATE() AND ([EndDate] > GETUTCDATE() OR [EndDate] IS NULL)
																AND [CF].[DSWEnvironment] = [DSWEnvironment])
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
PRINT N'Creazione store procedure [dbo].[DossierFolder_SP_PropagateAuthorizationToDescendants]';
GO

create procedure [dbo].[DossierFolder_SP_PropagateAuthorizationToDescendants]
	@IdParent uniqueidentifier,
	@IdDossier uniqueidentifier,
	@AuthorizationType smallint,
	@AuthorizationTypeDescription nvarchar(256),
    @RegistrationUser nvarchar(256),
	@System nvarchar(30)
as
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
	BEGIN TRANSACTION PropagateDescendants

		declare @childDossierFolderId as uniqueidentifier;
		declare @folderName as nvarchar(256);
		declare @descriptionLog as nvarchar(max);
		declare @idFascicle as uniqueidentifier = null;
		declare @fascicleName as nvarchar(max);

		declare @ParentRoles table(
			idRole smallint not null,
			Name nvarchar(100) not null
		);
		insert into @ParentRoles
		select dfr.idRole, r.Name from dossierfolderroles dfr inner join role r on dfr.idRole = r.idRole
		where iddossierfolder = @idParent and roleAuthorizationType = @AuthorizationType

		declare @TempRoles table(
			idRole smallint not null
		)

		declare @fetch_children_cursor as int;

		declare children_cursor cursor
		for(
			select idDossierfolder, df.Name, df.idFascicle, f.Title+'-'+f.Object from DossierFolders df
			left outer join fascicles f on f.idFascicle = df.idfascicle
			where df.DossierFolderNode.IsDescendantOf((select DossierFolderNode from DossierFolders where idDossierFolder = @IdParent))=1
			and df.idDossierFolder <>  @IdParent
		);

		open children_cursor
		fetch next from children_cursor into @childDossierFolderId, @folderName, @idFascicle, @fascicleName
		select @fetch_children_cursor = @@FETCH_STATUS
		while @fetch_children_cursor = 0
		begin
			delete from @TempRoles
			insert into @TempRoles
			select idRole from @ParentRoles
								except
								select idRole from DossierFolderRoles dfr
								where dfr.iddossierFolder = @childDossierFolderId
								and dfr.roleauthorizationtype = @AuthorizationType

			insert into DossierFolderRoles
			select newid(), @childDossierFolderId, IdRole, @AuthorizationType, 1, 1, @RegistrationUser, sysdatetimeoffset(), null, null, null
			from @TempRoles; 

	
			insert into DossierLogs
			select newid(), @IdDossier, sysdatetimeoffset(), @System, @RegistrationUser, 512, 'Autorizzata la cartella '+ @folderName +' al settore '+ pr.Name + ' ('+ cast(pr.idRole as nvarchar) +') per competenza ('+@AuthorizationTypeDescription+')', null, null, null, null, @childDossierFolderId
			from @ParentRoles pr
			where idRole in (select idRole from @TempRoles); 

	
			delete from @TempRoles
	
			insert into @TempRoles
			select idRole from @ParentRoles pr
				where pr.IdRole not in (
					select idRole from fascicleroles fr
					left outer join dossierfolders df on df.idfascicle = fr.idfascicle
					where iddossierfolder = @childDossierFolderId
					and df.idFascicle = @idFascicle		
					and RoleAuthorizationType = @AuthorizationType
					and ismaster = 0
				)

			if(@idFascicle is not null)
			begin
				insert into FascicleRoles
				select newId(), @idFascicle, idrole, 1, @RegistrationUser, sysdatetimeoffset(), null, null, null, 0
				from @TempRoles

				insert into FascicleLogs
				select newid(), @idFascicle, sysdatetimeoffset(), @System, @RegistrationUser, 4096, 'Aggiunta autorizzazione'+ @AuthorizationTypeDescription+' al settore '+pr.Name+'('+cast(pr.idrole as nvarchar)+')', null, null, null, null
				from @ParentRoles pr
				where pr.IdRole in (select idrole from @TempRoles)
	
				insert into DossierLogs
				select newid(), @iddossier, sysdatetimeoffset(), @System, @RegistrationUser, 512, 'Aggiunta autorizzazione al settore '+pr.Name+'('+cast(pr.idrole as nvarchar)+') nel fascicolo '+@fascicleName, null, null, null, null, @childDossierFolderId
				from @ParentRoles pr
				where pr.IdRole in (select idrole from @TempRoles)
			end

			insert into DossierLogs
			select newid(), @IdDossier, sysdatetimeoffset(), @System, @RegistrationUser, 256, 'Rimossa Autorizzazione '+ @AuthorizationTypeDescription+' al settore '+r.Name+'('+cast(r.idrole as nvarchar)+') alla cartella '+@folderName, null, null, null, null, @childDossierFolderId
			from DossierFolderRoles dfr
			inner join role r on r.idrole = dfr.idrole
			where dfr.iddossierFolder = @childDossierFolderId
			and dfr.roleauthorizationtype = @AuthorizationType
			and ismaster = 1
			and dfr.idrole not in (select idrole from @ParentRoles)

			delete from DossierFolderRoles
			where idrole not in (select idrole from @ParentRoles)
			and roleauthorizationtype = @AuthorizationType
			and ismaster = 1
			and iddossierfolder = @childDossierFolderId
	
			insert into FascicleLogs
			select newid(), @idFascicle, sysdatetimeoffset(), @System, @RegistrationUser, 4096, 'Rimossa autorizzazione '+@AuthorizationTypeDescription+' al settore '+r.Name+ '('+cast(fr.IdRole as nvarchar)+')', null, null, null, null
			from FascicleRoles fr inner join Role r on r.idRole = fr.IdRole
			where fr.idRole not in (select idRole from @ParentRoles)
			and idFascicle in (select idFascicle from dossierFolders where IdDossierFolder = @childDossierFolderId)
			and roleauthorizationtype = @AuthorizationType

			delete from FascicleRoles
			where idRole not in (select idRole from @ParentRoles)
			and idFascicle in (select idFascicle from dossierFolders where IdDossierFolder = @childDossierFolderId)
			and roleauthorizationtype = @AuthorizationType

			fetch next from children_cursor into @childDossierFolderId, @folderName, @idFascicle, @fascicleName
			select @fetch_children_cursor = @@FETCH_STATUS
		end	

		close children_cursor
		deallocate children_cursor
	COMMIT TRANSACTION PropagateDescendants
END TRY	

BEGIN CATCH
	ROLLBACK TRANSACTION PropagateDescendants

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
PRINT 'Modifica FUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle]';
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
		INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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
PRINT 'Modifica FUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle]';
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
		INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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