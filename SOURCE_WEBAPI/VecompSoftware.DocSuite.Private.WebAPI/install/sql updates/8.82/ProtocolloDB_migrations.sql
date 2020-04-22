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
PRINT 'Versionamento database alla 8.82'
GO

EXEC dbo.VersioningDatabase N'8.82',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################

PRINT N'Modifica SQLFUNCTION [webapiprivate].[DocumentUnit_FX_FascicolableDocumentUnits]';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_FascicolableDocumentUnits]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset,
	@IncludeThreshold bit,
	@ThresholdFrom datetimeoffset,
	@ExcludeLinked bit
)
RETURNS TABLE
AS
RETURN
(
	WITH 
	
	MySecurityGroups AS (
		SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
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
			   AND (
					(@ExcludeLinked = 0 AND DU.[IdFascicle] IS NULL)
					OR
					(@ExcludeLinked = 1 AND NOT EXISTS (SELECT TOP 1 1 FROM dbo.FascicleDocumentUnits FDU WHERE FDU.IdDocumentUnit = DU.[IdDocumentUnit]))
				   )
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Category_FX_CategorySubFascicles]';
GO

ALTER FUNCTION [webapiprivate].[Category_FX_CategorySubFascicles]
	(
		@IdCategory SMALLINT
	)
RETURNS TABLE 
AS
RETURN 
    WITH     
       SubCategories AS       
             (SELECT C.idCategory,C.FullIncrementalPath,CF.IdCategoryFascicle,CF.FascicleType
         FROM   Category AS C
             INNER JOIN CategoryFascicles AS CF ON CF.idcategory = C.idCategory
         WHERE  C.fullincrementalpath LIKE '%' + CAST (@IdCategory AS NVARCHAR) + '|%'),
       ToExcludeSubFascicles AS       
             (SELECT CC.idCategory
        FROM   SubCategories AS SC
             INNER JOIN Category AS CC ON CC.FullIncrementalPath LIKE '%' + CAST (SC.idCategory AS NVARCHAR) + '|%'
        WHERE  FascicleType = 1)
    SELECT   IdCategoryFascicle, idCategory AS IdCategory
    FROM     SubCategories
    WHERE    idCategory NOT IN (SELECT idCategory FROM ToExcludeSubFascicles) 
                     AND FascicleType = 0
    GROUP BY IdCategoryFascicle, idCategory
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Modifica Stored Procedure [dbo].[FascicleFolder_Update]';
GO

ALTER PROCEDURE [dbo].[FascicleFolder_Update] 
       @IdFascicleFolder uniqueidentifier,        
       @IdFascicle uniqueidentifier, 
       @IdCategory smallint,
       @Name nvarchar(256), 
       @Status smallint,
       @Typology smallint,
	   @RegistrationDate datetimeoffset(7),
       @RegistrationUser nvarchar(256), 
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256),
	   @ParentInsertId uniqueidentifier,
	   @Timestamp_Original timestamp
	AS
		
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
		
	BEGIN TRY
		DECLARE @parentNode hierarchyid, @childNode hierarchyid, @node hierarchyid, @nNode hierarchyid

		BEGIN TRANSACTION UpdateFascicleFolder

		SELECT @node = [FascicleFolderNode] FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @IdFascicleFolder
		IF @ParentInsertId IS NOT NULL
		BEGIN
			SELECT @parentNode = [FascicleFolderNode] FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @ParentInsertId

			SELECT @nNode = @parentNode.GetDescendant(MAX([FascicleFolderNode]), NULL)   
			FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @parentNode

			UPDATE [dbo].[FascicleFolders]
			SET [FascicleFolderNode] = [FascicleFolderNode].GetReparentedValue(@node, @nNode)
			WHERE [FascicleFolderNode].IsDescendantOf(@node) = 1

			SET @node = @nNode
		END

		UPDATE [dbo].[FascicleFolders] SET [IdFascicle] = @IdFascicle, [IdCategory] = @IdCategory, [Name] = @Name, [Status] = @Status, 
		[Typology] = @Typology, [LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser,
		[FascicleFolderNode] = @node
		WHERE [IdFascicleFolder] = @IdFascicleFolder

        SELECT [FascicleFolderNode],[IdFascicleFolder] as UniqueId,[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
        [Name],[Typology],[FascicleFolderPath],[FascicleFolderLevel],[FascicleFolderParentNode],[ParentInsertId],[Timestamp] 
		FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @IdFascicleFolder
		
		COMMIT TRANSACTION UpdateFascicleFolder
	END TRY
		
	BEGIN CATCH 
		ROLLBACK TRANSACTION UpdateFascicleFolder
		
		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Error Code: '+cast(@ErrorNumber as nvarchar(max))+' Message: '+ ERROR_MESSAGE();

		 RAISERROR (@ErrorMessage, -- Message text.  
               @ErrorSeverity, -- Severity.  
               @ErrorState, -- State.  			   
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