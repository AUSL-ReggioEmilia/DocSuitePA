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
PRINT N'ALTER FUNCTION [webapiprivate].[Fascicles_FX_HasInsertRight]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_HasInsertRight]
(@UserName NVARCHAR (256), @Domain NVARCHAR (256), @FascicleType INT)
RETURNS BIT
AS
BEGIN
    DECLARE @HasRight AS BIT;
    DECLARE @EmptyRights AS NVARCHAR (20);
    SET @EmptyRights = '00000000000000000000';
    WITH   MySecurityGroups
    AS     (SELECT IdGroup
            FROM   [dbo].[UserSecurityGroups](@UserName, @Domain))
    SELECT @HasRight = CAST (COUNT(1) AS BIT)
    FROM   [dbo].[Role] AS R
    WHERE  (EXISTS (SELECT 1
                    FROM   [dbo].[RoleUser] AS RU
                           INNER JOIN
                           [dbo].[CategoryFascicleRights] AS CFR
                           ON CFR.IdRole = RU.idRole
                           INNER JOIN
                           [dbo].[CategoryFascicles] AS CF
                           ON CFR.IdCategoryFascicle = CF.IdCategoryFascicle
                    WHERE  (@FascicleType = 1
                            OR @FascicleType = 2)
                           AND R.idRole = RU.idRole
                           AND RU.Account = @Domain + '\' + @UserName
                           AND Type IN ('RP', 'SP'))
            OR EXISTS (SELECT 1
                       FROM   [dbo].[RoleGroup] AS RG
                       WHERE  @FascicleType = 4
                              AND RG.IdRole = R.IdRole
                              AND EXISTS (SELECT 1
                                          FROM   MySecurityGroups AS SG
                                          WHERE  SG.IdGroup = RG.IdGroup)
                              AND ((RG.ProtocolRights <> @EmptyRights)
                                   OR (RG.ResolutionRights <> @EmptyRights)
                                   OR (RG.DocumentRights <> @EmptyRights)
                                   OR (RG.DocumentSeriesRights <> @EmptyRights)))
            OR EXISTS (SELECT  1
					   FROM dbo.ContainerGroup CG
					   INNER JOIN [dbo].[UserSecurityGroups](@UserName,@Domain) SG ON SG.IdGroup = CG.IdGroup
					   INNER JOIN [dbo].[Container] C on CG.idContainer=C.idContainer and C.IsActive = 1 
					   WHERE CG.FascicleRights LIKE '1%'));
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
PRINT N'Alter function [webapiprivate].[Category_FX_FindCategories]'
GO

ALTER FUNCTION [webapiprivate].[Category_FX_FindCategories]
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@Name nvarchar(256),
	@LoadRoot bit,
	@ParentId smallint,
	@ParentAllDescendants bit,
	@FullCode nvarchar(256),
	@FascicleFilterEnabled bit,
	@FascicleType smallint,	
	@HasFascicleInsertRights bit,
	@Manager nvarchar(256),
	@Secretary nvarchar(256),
	@Role smallint,
	@Container smallint
)
RETURNS TABLE 
AS
RETURN 
(
	WITH FoundCategories AS (
	SELECT DISTINCT C.idCategory 
	FROM [dbo].[Category] C 
		LEFT OUTER JOIN [CategoryFascicles] CF ON C.idCategory = CF.idCategory 
		LEFT OUTER JOIN [CategoryFascicleRights] CFR ON CFR.IdCategoryFascicle = CF.IdCategoryFascicle 
	WHERE
		IsActive = 1 
		AND StartDate <= GETDATE() 
		AND 
		(
			EndDate IS NULL 
			OR EndDate > GETDATE()
		)
		AND 
		(
			@LoadRoot = 1 
			AND C.idParent IS NULL
		)
		OR 
		(
			(
				@LoadRoot IS NULL 
				OR @LoadRoot = 0
			) 
			AND 
			(
				@ParentId IS NULL 
				OR 
				(
					(
						(
							(
								@ParentAllDescendants IS NULL 
								OR @ParentAllDescendants = 0
							) 
							AND C.idParent = @ParentId
						) 
						OR 
						(
							@ParentAllDescendants = 1 
							AND C.FullIncrementalPath LIKE CAST(@ParentId AS NVARCHAR) + '|%'
						)
					) 
				)
			)
			AND 
			(
				@FullCode IS NULL 
				OR C.FullCode = @FullCode
			)
			AND 
			(
				@Name IS NULL 
				OR C.Name LIKE '%' + @Name + '%'
			)
			AND 
			(
				(@FascicleFilterEnabled IS NULL OR @FascicleFilterEnabled = 0) 
				OR 
				(
					@FascicleFilterEnabled = 1 
					AND 
					(
						(
							@HasFascicleInsertRights IS NULL 
							OR @HasFascicleInsertRights = 0 
							OR @FascicleType <> 1
						) 
						OR 
						(
							@HasFascicleInsertRights = 1 
							AND @FascicleType = 1 
							AND 
							(
								EXISTS 
								(
									SELECT 1 
									FROM [dbo].[RoleUser] RU 
									WHERE
										RU.idRole = CFR.IdRole 
										AND RU.Account = @Domain + '\' + @UserName
										AND Type IN ('RP','SP')
								)
								OR EXISTS 
								(
									SELECT 1 
									FROM SecurityGroups SG 
										INNER JOIN RoleGroup RG ON RG.idGroup = SG.idGroup 
									WHERE
										RG.idRole = CFR.IdRole 
										AND SG.AllUsers = 1 
								)
							)
						)
					)
					AND 
					(
						@FascicleType IS NULL 
						OR 
						(
							CF.IdCategoryFascicle IS NOT NULL 
							AND CF.FascicleType = @FascicleType
						)
					)
					AND 
					(
						@Manager IS NULL 
						OR EXISTS 
						(
							SELECT 1 
							FROM [dbo].[RoleUser] RU 
							WHERE
								CFR.idRole = RU.idRole 
								AND RU.Account = @Manager 
								AND Type = 'RP' 
						)
					)
					AND 
					(
						@Secretary IS NULL 
						OR EXISTS 
						(
							SELECT 1 
							FROM [dbo].[RoleUser] RU 
							WHERE
								CFR.idRole = RU.idRole 
								AND RU.Account = @Secretary 
								AND Type = 'SP'
						)
					)
					AND 
					(
						@Role IS NULL 
						OR 
						(
							CFR.IdCategoryFascicleRight IS NOT NULL 
							AND CFR.IdRole = @Role
						)
					)
					AND 
					(
						@Container IS NULL 
						OR 
						(
							EXISTS 
							(
								SELECT 1 
								FROM CategoryFascicleRights CFR2 
								WHERE
									CFR2.IdCategoryFascicle = CF.IdCategoryFascicle 
									AND CFR2.IdContainer = @Container 
							)
							OR NOT EXISTS 
							(
								SELECT 1 
								FROM CategoryFascicleRights CFR2 
								WHERE
									CFR2.IdCategoryFascicle = CF.IdCategoryFascicle 
									AND CFR2.IdContainer IS NOT NULL 
							)
						)
					)
				)
			)
		)
	)

	SELECT
	[Category].[idCategory] AS IdCategory,
	[Category].[Name],
	[Category].[isActive],
	[Category].[Code],
	[Category].[FullIncrementalPath],
	[Category].[FullCode],
	[Category].[RegistrationDate],
	[Category].[RegistrationUser],
	[Category].[UniqueId],
	[Category].[StartDate],
	[Category].[EndDate],
	[CategoryParent].[idCategory] AS CategoryParent_IdCategory,
	(
		SELECT TOP 1 CAST(1 AS BIT) 
		FROM Category C_TMP 
		WHERE
			C_TMP.idParent = Category.idCategory 
	)
	AS HasChildren,
	(
		CASE
			@FascicleType 
			WHEN NULL THEN CAST(0 AS BIT) 
			WHEN 1 THEN
			( 
				SELECT DISTINCT CAST(1 AS BIT) 
				FROM [dbo].[CategoryFascicles] CF 
					INNER JOIN [dbo].[CategoryFascicleRights] CFR ON CFR.IdCategoryFascicle = CF.IdCategoryFascicle 
				WHERE
					CF.idCategory = Category.idCategory 
					AND 
					(
						EXISTS 
						(
							SELECT 1 
							FROM [dbo].[RoleUser] RU 
							WHERE
								RU.idRole = CFR.IdRole 
								AND 
								(
									RU.Account = @Domain + '\' + @UserName
									AND Type IN ('RP','SP')
								)
						)
						OR EXISTS 
						(
							SELECT 1 
							FROM SecurityGroups SG 
								INNER JOIN RoleGroup RG ON RG.idGroup = SG.idGroup 
							WHERE
								RG.idRole = CFR.IdRole 
								AND SG.AllUsers = 1 
						)
					)
			) 
			WHEN 2 THEN
			( 
				SELECT DISTINCT CAST(1 AS BIT) 
				FROM [dbo].[CategoryFascicles] CF 
				WHERE
					CF.idCategory = Category.idCategory 
					AND CF.FascicleType = @FascicleType 
			) 
		END
	)
	AS HasFascicleDefinition 
FROM
	Category 
	INNER JOIN FoundCategories FC ON FC.idCategory = Category.idCategory 
	LEFT OUTER JOIN Category CategoryParent ON CategoryParent.idCategory = Category.idParent 
WHERE
	Category.isActive = 1
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
PRINT N'Create function [webapiprivate].[Container_FX_FascicleInsertAuthorized]'
GO

CREATE FUNCTION [webapiprivate].[Container_FX_FascicleInsertAuthorized]
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@CategoryId smallint,
	@FascicleType smallint
)
RETURNS TABLE 
AS
RETURN 
(
	WITH CategoryRights AS (
		SELECT
			CFR.IdCategoryFascicleRight,
			CFR.IdContainer 
		FROM CategoryFascicleRights CFR 
			INNER JOIN CategoryFascicles CF ON CF.idCategoryFascicle = CFR.idCategoryFascicle AND CF.IdCategory = @CategoryId 
		WHERE
			CFR.IdContainer IS NOT NULL 
			AND 
			(
				(
					@FascicleType = 1 
					AND CF.DSWEnvironment = 0
				) 
				OR 
				(
					@FascicleType = 2 
					AND CF.DSWEnvironment > 0
				)
			)
		GROUP BY
			CFR.IdCategoryFascicleRight,
			CFR.IdContainer
	)

	SELECT
	CAST([C].[idContainer] AS int) AS IdContainer,
	[C].[Name],
	[C].[Note],
	[C].[UniqueId] 
	FROM Container C 
		INNER JOIN ContainerGroup CG ON CG.idContainer = C.idContainer 
		INNER JOIN SecurityUsers SU ON SU.idGroup = CG.idGroup 
		LEFT OUTER JOIN CategoryRights CR ON CR.IdContainer = C.idContainer 
	WHERE
		C.isActive = 1 
		AND SU.Account = @UserName 
		AND SU.UserDomain = @Domain 
		AND CG.FascicleRights LIKE '1%' 
		AND 
		(
			@CategoryId IS NULL 
			OR 
			(
				@CategoryId IS NOT NULL 
				AND NOT EXISTS 
				(
					SELECT TOP 1 1 
					FROM CategoryRights 
				)
			)
			OR 
			(
				@CategoryId IS NOT NULL 
				AND CR.IdCategoryFascicleRight IS NOT NULL 
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
PRINT N'Alter sql function [webapiprivate].[Fascicle_FX_HasDocumentVisibilityRight]'
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

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Fascicles] F
       WHERE F.IdFascicle = @IdFascicle
       AND F.VisibilityType = 1
       AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, @IdFascicle)) = 1)
       
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
PRINT N'Alter function [webapiprivate].[Category_FX_CategorySubFascicles]'
GO

ALTER FUNCTION [webapiprivate].[Category_FX_CategorySubFascicles] (
	@IdCategory smallint
)
RETURNS TABLE
AS 
RETURN
(
	WITH SubCategories AS 
	(
		SELECT C.idCategory, 
			C.FullIncrementalPath,
			CF.IdCategoryFascicle,
			CF.FascicleType 
		FROM Category C 
			INNER JOIN CategoryFascicles CF ON CF.idcategory = C.idCategory 
		WHERE
			C.fullincrementalpath LIKE '%'+ CAST(@IdCategory AS NVARCHAR) +'|%' 
	),
	ToExcludeSubFascicles AS 
	(
		SELECT CC.idCategory 
		FROM SubCategories SC 
			LEFT OUTER JOIN Category CC ON CC.FullIncrementalPath LIKE '%' + CAST(SC.idCategory AS NVARCHAR) + '|%' 
		WHERE
			FascicleType = 1 
	)
	  
	SELECT IdCategoryFascicle
		,idCategory AS IdCategory
	FROM SubCategories 
		WHERE
			idCategory NOT IN 
			(
				SELECT idCategory 
				FROM ToExcludeSubFascicles
			)
			AND FascicleType = 0
	GROUP BY IdCategoryFascicle
		    ,idCategory
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
PRINT N'Alter sql function [webapiprivate].[FascicleFolder_FX_AllChildrenByParent]'
GO
 
ALTER FUNCTION [webapiprivate].[FascicleFolder_FX_AllChildrenByParent] (
	@IdFascicleFolder uniqueidentifier
	)
	RETURNS TABLE
AS 
	RETURN
	(	  
	SELECT [FF].[IdFascicleFolder]
			,[FF].[Name]
			,[FF].[Status]
			,[FF].[Typology]
			,[FF].[IdFascicle] as Fascicle_IdFascicle
			,[FF].[IdCategory] as Category_IdCategory	
			,CAST((SELECT COUNT(IdFascicleFolder) AS Result
		from FascicleFolders 
		WHERE
		IdFascicleFolder = @IdFascicleFolder
		AND (EXISTS (SELECT 1 FROM FascicleDocumentUnits WHERE idfasciclefolder = FF.IdFascicleFolder)			
			)) AS BIT) as HasDocuments
			,CAST(COUNT(FFF.IdFascicleFolder) AS BIT) as HasChildren
			, [FF].[FascicleFolderLevel]	
	   FROM FascicleFolders FF 
	   LEFT OUTER JOIN FascicleFolders AS FFF
	   ON FF.FascicleFolderNode = FFF.FascicleFolderNode.GetAncestor(1)
	   WHERE FF.FascicleFolderNode.GetAncestor(1) = (SELECT TOP 1 FascicleFolderNode 
												 FROM FascicleFolders 
												 WHERE IdFascicleFolder = @IdFascicleFolder)				 
		GROUP BY [FF].[IdFascicleFolder]
		        ,[FF].[Name]
				,[FF].[Status]
				,[FF].[Typology]
				,[FF].[IdFascicle]
				,[FF].[IdCategory]	
				,[FF].[FascicleFolderLevel]
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
PRINT N'Alter sql function [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]'
GO
ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]
(
    @FascicleId uniqueidentifier,
	@IdFascicleFolder uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(    
    WITH FascicleDocumentUnits AS
	(
		SELECT FP.IdDocumentUnit, FP.ReferenceType, NULL AS IdUDSRepository
		FROM  dbo.FascicleDocumentUnits FP
		WHERE FP.IdFascicle = @FascicleId AND ((@IdFascicleFolder IS NOT NULL AND FP.IdFascicleFolder = @IdFascicleFolder) OR (@IdFascicleFolder IS NULL))
	
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
	  ,FDU.ReferenceType
	  ,FDU.IdUDSRepository
	  ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
	FROM cqrs.DocumentUnits DU
	INNER JOIN FascicleDocumentUnits FDU ON FDU.IdDocumentUnit = DU.IdDocumentUnit
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
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
PRINT N'Alter sql function [webapiprivate].[Fascicles_FX_AvailableFasciclesFromDocumentUnit]'

GO
CREATE FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromDocumentUnit]
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@UniqueId uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(

	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate			
	FROM Fascicles F 
		LEFT JOIN FascicleDocumentUnits FP on F.IdFascicle = FP.IdFascicle AND FP.IdDocumentUnit = @UniqueId
		LEFT JOIN cqrs.DocumentUnits P on P.IdDocumentUnit = @UniqueId
		LEFT JOIN Category C on C.IdCategory = P.IdCategory
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 0 AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = P.IdCategory or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 0 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 0 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FP.IdDocumentUnit IS NULL
		AND F.EndDate IS NULL
		AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, F.IdFascicle)) = 1)
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate
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

PRINT N'ALTER FUNCTION [webapipublic].[DocumentUnit_FX_FascicleDocumentUnits]'
GO

ALTER FUNCTION [webapipublic].[DocumentUnit_FX_FascicleDocumentUnits]
(
    @FascicleId uniqueidentifier,
    @FilterNameTitle nvarchar(256)
)
RETURNS TABLE
AS
RETURN
(    
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
      ,FDU.ReferenceType
      ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
       FROM cqrs.DocumentUnits DU
       INNER JOIN [dbo].FascicleDocumentUnits FDU ON FDU.IdDocumentUnit = DU.IdDocumentUnit
       INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
       INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
       WHERE DU.[DocumentUnitName] + DU.Title like '%'+@FilterNameTitle+'%'
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
PRINT N'ALTER FUNCTION [webapiprivate].[FascicleFolder_FX_GetParent]'
GO

ALTER FUNCTION [webapiprivate].[FascicleFolder_FX_GetParent] (
	@IdFascicleFolder uniqueidentifier
)
RETURNS TABLE
AS 
	RETURN
	(
	WITH Ancestor AS 
	(
		SELECT FascicleFolderNode.GetAncestor(1) AS Ancestor
		FROM FascicleFolders 
		WHERE IdFasciclefolder=@IdFascicleFolder
	)
	SELECT [FF].[IdFascicleFolder]
			,[FF].[Name]
			,[FF].[Status]
			,[FF].[Typology]
			,[FF].[IdFascicle] as Fascicle_IdFascicle
			,[FF].[IdCategory] as Category_IdCategory	
			,cast((CASE 
				WHEN COUNT(FD.IdFascicleDocument) + COUNT(FDU.IdFascicleDocumentUnit) = 0 THEN 0
				ELSE 1 END) as bit) as HasDocuments
			,cast(1 as bit) as HasChildren
			,[FF].[FascicleFolderLevel]	
	   FROM FascicleFolders FF
	   INNER JOIN Ancestor A ON FF.FascicleFolderNode = A.Ancestor 
	   LEFT JOIN FascicleDocuments FD ON FD.IdFascicleFolder = FF.IdFascicleFolder
	   LEFT JOIN FascicleDocumentUnits FDU ON FDU.IdFascicleFolder = FF.IdFascicleFolder
	   WHERE FF.FascicleFolderLevel > 1
			 
		GROUP BY [FF].[IdFascicleFolder]
		        ,[FF].[Name]
				,[FF].[Status]
				,[FF].[Typology]
				,[FF].[IdFascicle]
				,[FF].[IdCategory]	
				,[FF].[FascicleFolderLevel]
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
PRINT N'DROP FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromProtocol]'
GO

DROP FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromProtocol]

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'DROP FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromResolution]'
GO

DROP FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromResolution]

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'DROP FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]'
GO

DROP FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'ALTER FUNCTION [webapiprivate].[Fascicles_FX_HasManageableRight]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_HasManageableRight]
(
       @UserName nvarchar(256), 
       @Domain nvarchar(256),
       @IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
       declare @HasRight bit;
       declare @EmptyRights nvarchar(20);
	   SET @EmptyRights = '00000000000000000000';

       WITH
       MySecurityGroups AS (
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
       ),
	   MyResponsibleRoles AS (
	    SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = @IdFascicle AND RoleAuthorizationType = 0
	   )

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Fascicles] F
       WHERE F.IdFascicle = @IdFascicle
       AND ( exists (select 1
                           from [dbo].[RoleGroup] RG
                           where RG.IdRole in (select idRole from MyResponsibleRoles)
						   AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)  
						   AND ((RG.ProtocolRights <> @EmptyRights)
                             OR (RG.ResolutionRights <> @EmptyRights)
                             OR (RG.DocumentRights <> @EmptyRights)
                             OR (RG.DocumentSeriesRights <> @EmptyRights))
                           )
             OR 
                exists (select 1 
						from [dbo].[RoleUser] RU
						INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
						INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
						where CF.IdCategory = F.IdCategory
						and RU.Account=@Domain+'\'+@UserName
						and F.FascicleType IN (1, 2) and RU.[Type] in ('RP', 'SP')
						and (
								(exists(select 1 from MyResponsibleRoles) and (RU.idRole in (select idRole from MyResponsibleRoles))) OR 
								(not exists(select 1 from MyResponsibleRoles))
							)
                       )
			OR
				exists (
						select 1
						from [dbo].[Contact] C
						INNER JOIN [dbo].[FascicleContacts] FC on FC.IdContact = C.Incremental
						where FC.IdFascicle = F.IdFascicle AND C.SearchCode = @Domain+'\'+@UserName
					   )
			OR
			   (NOT exists ( SELECT 1 
									  FROM [dbo].[FascicleRoles] FR
									  WHERE FR.IdFascicle = F.IdFascicle AND FR.RoleAuthorizationType = 0)
				AND exists (
						select 1
						from [dbo].[ContainerGroup] CG
						where CG.idContainer = F.IdContainer
						AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
						AND (CG.FascicleRights LIKE '1%' OR CG.FascicleRights LIKE '_1%')
					   )
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
--#############################################################################
PRINT N'ALTER FUNCTION [webapiprivate].[Fascicles_FX_HasViewableRight]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_HasViewableRight]
(
       @UserName nvarchar(256), 
       @Domain nvarchar(256),
       @IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
       declare @HasRight bit;
       declare @EmptyRights nvarchar(20);
	   SET @EmptyRights = '00000000000000000000';

       WITH
       MySecurityGroups AS (
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
       ),
	   MyResponsibleRoles AS (
	    SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = @IdFascicle AND RoleAuthorizationType = 0 and IsMaster = 1
	   )

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Fascicles] F
       WHERE F.IdFascicle = @IdFascicle
       AND ( exists (select 1
                     FROM MySecurityGroups SG
                     INNER JOIN [dbo].[RoleGroup] RG on SG.IdGroup = RG.IdGroup
                     INNER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = F.IdFascicle AND RG.IdRole = FR.IdRole
                     WHERE (
								(F.FascicleType = 4 AND FR.RoleAuthorizationType = 0) OR
								(F.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 1) OR
								(F.FascicleType = 2 AND FR.RoleAuthorizationType = 0 AND IsMaster = 1) OR
								(F.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 0 AND IsMaster = 0)
							)
					 AND ((RG.ProtocolRights <> @EmptyRights)
                        OR (RG.ResolutionRights <> @EmptyRights)
                        OR (RG.DocumentRights <> @EmptyRights)
                        OR (RG.DocumentSeriesRights <> @EmptyRights))
                    )
             OR 
                exists (select 1 
						from [dbo].[RoleUser] RU
						INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
						INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
						where CF.IdCategory = F.IdCategory
						and RU.Account=@Domain+'\'+@UserName
						and F.FascicleType = 1 and RU.[Type] in ('RP', 'SP')
						and (
								(exists(select 1 from MyResponsibleRoles) and (RU.idRole in (select idRole from MyResponsibleRoles))) OR 
								(not exists(select 1 from MyResponsibleRoles))
							)
                       )

			OR
				(NOT exists ( SELECT 1 
							  FROM [dbo].[FascicleRoles] FR
							  WHERE FR.IdFascicle = F.IdFascicle AND FR.RoleAuthorizationType = 0
							)
				AND exists (
							SELECT 1
							FROM [dbo].[ContainerGroup] CG
							where CG.idContainer = F.IdContainer
							AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
							AND CG.FascicleRights LIKE '__1%'
						 )
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
PRINT N'ALTER FUNCTION [webapiprivate].[Fascicles_FX_CountAuthorizedFascicles]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_CountAuthorizedFascicles](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Year smallint,
	@StartDateFrom datetimeoffset,
	@StartDateTo datetimeoffset,
	@EndDateFrom datetimeoffset,
	@EndDateTo datetimeoffset,
	@FascicleStatus int,
	@Manager nvarchar(256),
	@Name nvarchar(256),
	@ViewConfidential bit,
	@ViewAccessible bit,
	@Subject nvarchar(256),
	@Note nvarchar(256),
	@Rack nvarchar(256),
	@IdMetadataRepository nvarchar(256),
	@MetadataValue nvarchar (256),
	@Classifications nvarchar(256),
	@IncludeChildClassifications bit,
	@Roles nvarchar(MAX),
	@Container smallint,
	@ApplySecurity bit
)
RETURNS INT
AS
	BEGIN
	DECLARE @CountFascicles INT;
	WITH 	
	MySecurityGroups AS (
		SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
	)
	
	SELECT @CountFascicles = COUNT(DISTINCT Fascicle.IdFascicle)
			 FROM dbo.Fascicles Fascicle
			 WHERE (@Year is null or Fascicle.Year = @Year)
			 AND (@StartDateFrom is null or Fascicle.StartDate >= @StartDateFrom)
			 AND (@StartDateTo is null or Fascicle.StartDate <= @StartDateTo)
			 AND (@EndDateFrom is null or Fascicle.EndDate >= @StartDateFrom)
			 AND (@EndDateTo is null or Fascicle.EndDate <= @EndDateTo)
			 AND (@FascicleStatus is null or 
					((@FascicleStatus = 2 and Fascicle.EndDate is not null and Fascicle.EndDate <= GETUTCDATE())
					 or (@FascicleStatus = 1 and Fascicle.EndDate is null and Fascicle.StartDate <= GETUTCDATE()))
				 )
			 AND (@Manager is null or exists (SELECT 1 FROM FascicleContacts FC
												inner join Contact C on C.Incremental = FC.IdContact
												WHERE C.FullIncrementalPath = @Manager AND FC.IdFascicle = Fascicle.IdFascicle)
				 )
			 AND (@Name is null or Fascicle.Name like '%'+@Name+'%')
			 AND ((@ViewConfidential is null and @ViewAccessible is null)
					or ((@ViewConfidential is null and @ViewAccessible is not null) and (Fascicle.VisibilityType = 1))
					or ((@ViewConfidential is not null and @ViewAccessible is null) and (Fascicle.VisibilityType = 0))
					or ((@ViewConfidential is not null and @ViewAccessible is not null) and (Fascicle.VisibilityType in (0,1)))
				 )
			 AND (@Subject is null or Fascicle.Object like '%'+@Subject+'%')
			 AND (@Rack is null or Fascicle.Rack like '%'+@Rack+'%')
			 AND (@Note is null or Fascicle.Note like '%'+@Note+'%')
			 AND (@IdMetadataRepository is null or Fascicle.IdMetadataRepository = @IdMetadataRepository)
			 AND (@MetadataValue is null or Fascicle.MetadataValues like '%'+@MetadataValue+'%')
			 AND (@Classifications is null or ((@IncludeChildClassifications = 1 and exists (SELECT 1 FROM Category
																							 WHERE IdCategory = Fascicle.IdCategory 
																							 AND FullIncrementalPath like '%'+@Classifications))
												or ((@IncludeChildClassifications is null or @IncludeChildClassifications = 0) and exists (SELECT 1 FROM Category
																							 WHERE IdCategory = Fascicle.IdCategory 
																							 AND FullIncrementalPath = @Classifications))
											  )
				 )
			 AND (@Container is null or Fascicle.IdContainer = @Container)
			 AND (@Roles is null or exists (SELECT 1 FROM FascicleRoles FR
															   WHERE FR.IdFascicle = Fascicle.IdFascicle
															   AND FR.IdRole IN (SELECT CAST([Value] AS smallint) FROM dbo.SplitString(@Roles, '|')))
				 )
			 AND (
					(@ApplySecurity is null or @ApplySecurity = 0)
					OR 
					 (@ApplySecurity = 1 
					  AND 
						(exists (select 1
								 FROM MySecurityGroups SG
								 INNER JOIN [dbo].[RoleGroup] RG on SG.IdGroup = RG.IdGroup
								 INNER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = Fascicle.IdFascicle AND RG.IdRole = FR.IdRole
								 WHERE (
										 (Fascicle.FascicleType = 4 AND FR.RoleAuthorizationType = 0) OR
										 (Fascicle.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 1) OR
										 (Fascicle.FascicleType = 2 AND FR.RoleAuthorizationType = 0 AND IsMaster = 1) OR
										 (Fascicle.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 0 AND IsMaster = 0)
									   )
								   AND ((RG.ProtocolRights <> '00000000000000000000')
									 OR (RG.ResolutionRights <> '00000000000000000000')
									 OR (RG.DocumentRights <> '00000000000000000000')
									 OR (RG.DocumentSeriesRights <> '00000000000000000000'))
							   )
						 OR 
							exists (select 1 
									from [dbo].[RoleUser] RU
									INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
									INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
									where CF.IdCategory = Fascicle.IdCategory
									and RU.Account=@Domain+'\'+@UserName
									and Fascicle.FascicleType = 1
									and RU.[Type] in ('RP', 'SP')
									and (
											NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[FascicleRoles] WHERE IdFascicle = Fascicle.IdFascicle AND RoleAuthorizationType = 0 AND IsMaster = 1)
											OR (RU.idRole in ((SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = Fascicle.IdFascicle AND RoleAuthorizationType = 0 AND IsMaster = 1))))
										)
						OR
							(NOT exists ( SELECT 1 
											FROM [dbo].[FascicleRoles] FR
											WHERE FR.IdFascicle = Fascicle.IdFascicle AND FR.RoleAuthorizationType = 0)
								AND exists (
									SELECT 1
									FROM [dbo].[ContainerGroup] CG
									where CG.idContainer = Fascicle.IdContainer
									AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
									AND CG.FascicleRights LIKE '__1%'
									)
							)
						)
					)
				)

	RETURN @CountFascicles
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
PRINT N'ALTER FUNCTION [webapiprivate].[Fascicles_FX_AuthorizedFascicles]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AuthorizedFascicles](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Skip int,
	@Top int,
	@Year smallint,
	@StartDateFrom datetimeoffset,
	@StartDateTo datetimeoffset,
	@EndDateFrom datetimeoffset,
	@EndDateTo datetimeoffset,
	@FascicleStatus int,
	@Manager nvarchar(256),
	@Name nvarchar(256),
	@ViewConfidential bit,
	@ViewAccessible bit,
	@Subject nvarchar(256),
	@Note nvarchar(256),
	@Rack nvarchar(256),
	@IdMetadataRepository nvarchar(256),
	@MetadataValue nvarchar (256),
	@Classifications nvarchar(256),
	@IncludeChildClassifications bit,
	@Roles nvarchar(MAX),
	@Container smallint,
	@ApplySecurity bit	
)
RETURNS TABLE
AS
RETURN
	(
		WITH 	
		MySecurityGroups AS (
			SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
		),
		MyFascicles AS (
			select IdFascicle from
			(select Fascicle.IdFascicle, row_number() over(order by Fascicle.RegistrationDate ASC, Fascicle.Title ASC) AS rownum
			 FROM dbo.Fascicles Fascicle
			 WHERE (@Year is null or Fascicle.Year = @Year)
			 AND (@StartDateFrom is null or Fascicle.StartDate >= @StartDateFrom)
			 AND (@StartDateTo is null or Fascicle.StartDate <= @StartDateTo)
			 AND (@EndDateFrom is null or Fascicle.EndDate >= @StartDateFrom)
			 AND (@EndDateTo is null or Fascicle.EndDate <= @EndDateTo)
			 AND (@FascicleStatus is null or 
					((@FascicleStatus = 2 and Fascicle.EndDate is not null and Fascicle.EndDate <= GETUTCDATE())
					 or (@FascicleStatus = 1 and Fascicle.EndDate is null and Fascicle.StartDate <= GETUTCDATE()))
				 )
			 AND (@Manager is null or exists (SELECT 1 FROM FascicleContacts FC
												inner join Contact C on C.Incremental = FC.IdContact
												WHERE C.FullIncrementalPath = @Manager AND FC.IdFascicle = Fascicle.IdFascicle)
				 )
			 AND (@Name is null or Fascicle.Name like '%'+@Name+'%')
			 AND ((@ViewConfidential is null and @ViewAccessible is null)
					or ((@ViewConfidential is null and @ViewAccessible is not null) and (Fascicle.VisibilityType = 1))
					or ((@ViewConfidential is not null and @ViewAccessible is null) and (Fascicle.VisibilityType = 0))
					or ((@ViewConfidential is not null and @ViewAccessible is not null) and (Fascicle.VisibilityType in (0,1)))
				 )
			 AND (@Subject is null or Fascicle.Object like '%'+@Subject+'%')
			 AND (@Rack is null or Fascicle.Rack like '%'+@Rack+'%')
			 AND (@Note is null or Fascicle.Note like '%'+@Note+'%')
			 AND (@IdMetadataRepository is null or Fascicle.IdMetadataRepository = @IdMetadataRepository)
			 AND (@MetadataValue is null or Fascicle.MetadataValues like '%'+@MetadataValue+'%')
			 AND (@Classifications is null or ((@IncludeChildClassifications = 1 and exists (SELECT 1 FROM Category
																							 WHERE IdCategory = Fascicle.IdCategory 
																							 AND FullIncrementalPath like '%'+@Classifications))
												or ((@IncludeChildClassifications is null or @IncludeChildClassifications = 0) and exists (SELECT 1 FROM Category
																							 WHERE IdCategory = Fascicle.IdCategory 
																							 AND FullIncrementalPath = @Classifications))
											  )
				 )
			 AND (@Roles is null or exists (SELECT 1 FROM FascicleRoles FR
													 WHERE FR.IdFascicle = Fascicle.IdFascicle
													       AND FR.IdRole IN (SELECT CAST([Value] AS smallint) FROM dbo.SplitString(@Roles, '|')))
				 )
			 AND (@Container is null or Fascicle.IdContainer = @Container)
			 AND ((@ApplySecurity is null or @ApplySecurity = 0) or (@ApplySecurity = 1 AND 
					(EXISTS (SELECT 1
							 FROM MySecurityGroups SG
                             INNER JOIN [dbo].[RoleGroup] RG on SG.IdGroup = RG.IdGroup
                             INNER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = Fascicle.IdFascicle AND RG.IdRole = FR.IdRole
                             WHERE (
									 (Fascicle.FascicleType = 4 AND FR.RoleAuthorizationType = 0) OR
									 (Fascicle.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 1) OR
									 (Fascicle.FascicleType = 2 AND FR.RoleAuthorizationType = 0 AND IsMaster = 1) OR
									 (Fascicle.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 0 AND IsMaster = 0)
								   )
						     AND ((RG.ProtocolRights <> '00000000000000000000')
                              OR (RG.ResolutionRights <> '00000000000000000000')
                              OR (RG.DocumentRights <> '00000000000000000000')
                              OR (RG.DocumentSeriesRights <> '00000000000000000000'))
                           )
					 OR 
						exists (SELECT 1 
								FROM [dbo].[RoleUser] RU
								INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
								INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
								where CF.IdCategory = Fascicle.IdCategory
								AND RU.Account=@Domain+'\'+@UserName
								AND Fascicle.FascicleType = 1
								AND RU.[Type] in ('RP', 'SP')
								AND (
										NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[FascicleRoles] WHERE IdFascicle = Fascicle.IdFascicle AND RoleAuthorizationType = 0 AND IsMaster = 1)
										OR (RU.idRole in ((SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = Fascicle.IdFascicle AND RoleAuthorizationType = 0 AND IsMaster = 1))))
									)
					OR
						(NOT exists ( SELECT 1 
										FROM [dbo].[FascicleRoles] FR
										WHERE FR.IdFascicle = Fascicle.IdFascicle AND FR.RoleAuthorizationType = 0)
							AND exists (
								SELECT 1
								FROM [dbo].[ContainerGroup] CG
								where CG.idContainer = Fascicle.IdContainer
								AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
								AND CG.FascicleRights LIKE '__1%'
								)
							)
					)
				))
			Group by Fascicle.IdFascicle, Fascicle.RegistrationDate, Fascicle.Title) T 
			where T.rownum > @Skip AND T.rownum <= @Top			
		)

SELECT F.IdFascicle AS UniqueId, F.Year, F.Number, F.EndDate, F.Title, F.Name, F.Object as FascicleObject, F.Manager,
	   F.IdCategory, F.FascicleType, F.VisibilityType, F.RegistrationUser, F.RegistrationDate, F_C.IdCategory as Category_idCategory,
	   F_C.Name as Category_Name, FC.IdContact AS Contact_Incremental, F_CON.Description AS Contact_Description
FROM Fascicles F
INNER JOIN MyFascicles MF ON MF.IdFascicle = F.IdFascicle
INNER JOIN Category F_C on F_C.idCategory = F.IdCategory
LEFT OUTER JOIN FascicleContacts FC ON FC.IdFascicle = F.IdFascicle
LEFT OUTER JOIN Contact F_CON ON F_CON.Incremental = FC.IdContact
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