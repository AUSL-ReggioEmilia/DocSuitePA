PRINT 'Creazione dello schema webapipublic';
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'webapipublic')
BEGIN
       EXEC('CREATE SCHEMA webapipublic');
END
GO
PRINT 'Creazione dello schema webapiprivate';
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'webapiprivate')
BEGIN
       EXEC('CREATE SCHEMA webapiprivate');
END
GO


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

--############################################################################
PRINT 'Rinominazione funzione SQL: MassimarioScarto_AllChildrenByParent';
GO

DROP FUNCTION [dbo].[MassimarioScarto_AllChildrenByParent]
GO

CREATE FUNCTION [webapiprivate].[MassimarioScarto_FX_AllChildrenByParent]
(	
	@parentId uniqueidentifier,
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(	 
	SELECT distinct [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp]
	FROM [dbo].[MassimariScarto]
	WHERE [MassimarioScartoNode].GetAncestor(1) = (SELECT [MassimarioScartoNode]  
	FROM [dbo].[MassimariScarto]
	WHERE [IdMassimarioScarto] = @parentId)
	AND ((@includeLogicalDelete = 0 AND ([EndDate] is null OR [EndDate] > GETDATE()) AND [Status] = 1) OR (@includeLogicalDelete = 1 AND [Status] in (0,1)))
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
PRINT 'Rinominazione funzione SQL: MassimarioScarto_RootChildren';
GO

DROP FUNCTION [dbo].[MassimarioScarto_RootChildren]
GO

CREATE FUNCTION [webapiprivate].[MassimarioScarto_FX_RootChildren]
(	
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(	 
	SELECT distinct [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp]
	FROM [dbo].[MassimariScarto]
	WHERE [MassimarioScartoNode].GetAncestor(1) = (SELECT [MassimarioScartoNode]  
	FROM [dbo].[MassimariScarto]
	WHERE [IdMassimarioScarto] = (SELECT [IdMassimarioScarto] FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode] = hierarchyid::GetRoot()))
	AND ((@includeLogicalDelete = 0 AND ([EndDate] is null OR [EndDate] > GETDATE()) AND [Status] = 1) OR (@includeLogicalDelete = 1 AND [Status] in (0,1)))
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
PRINT 'Eliminazione funzione SQL: MassimarioScarto_GetMassimari';
GO

DROP FUNCTION [dbo].[MassimarioScarto_GetMassimari]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'Rinominazione funzione SQL: MassimarioScarto_GetMassimariFull';
GO

DROP FUNCTION [dbo].[MassimarioScarto_GetMassimariFull]
GO

CREATE FUNCTION [webapiprivate].[MassimarioScarto_FX_FilteredMassimari]
(	
	@name nvarchar(256),
	@fullCode nvarchar(256) = NULL,
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT distinct M1.[MassimarioScartoNode],M1.[IdMassimarioScarto] as UniqueId,M1.[RegistrationDate],M1.[RegistrationUser],M1.[LastChangedDate],M1.[LastChangedUser],M1.[Status],
		M1.[Name],M1.[Code],M1.[FullCode],M1.[Note],M1.[ConservationPeriod],M1.[StartDate],M1.[EndDate],M1.[MassimarioScartoPath],M1.[MassimarioScartoLevel],M1.[MassimarioScartoParentPath],M1.[FakeInsertId],M1.[Timestamp]
    FROM (SELECT * FROM [dbo].[MassimariScarto]
			WHERE (@name IS NOT NULL AND @name != '' AND [Name] like '%'+@name+'%')
			OR (@fullCode IS NOT NULL AND [FullCode] = @fullCode)) M2,
    [dbo].[MassimariScarto] M1
    WHERE 
	[M2].[MassimarioScartoNode].IsDescendantOf(M1.MassimarioScartoNode) = 1
	AND M1.MassimarioScartoNode != hierarchyid::GetRoot()
	AND ((@includeLogicalDelete = 0 AND (M1.[EndDate] is null OR M1.[EndDate] > GETDATE()) AND M1.[Status] = 1) OR (@includeLogicalDelete = 1 AND M1.[Status] in (0,1)))
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
--############################################################################
PRINT 'Rinominazione funzione SQL: FascicleDocumentUnits';
GO

DROP FUNCTION [dbo].[FascicleDocumentUnits]
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]
(
    @FascicleId uniqueidentifier
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
	  ,COALESCE(FP.ReferenceType, FR.ReferenceType, FD.ReferenceType) as ReferenceType
	  ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN dbo.FascicleProtocols FP ON FP.UniqueIdProtocol = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleResolutions FR ON FR.UniqueIdResolution = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleDocumentSeriesItems FD ON FD.UniqueIdDocumentSeriesItem = DU.IdDocumentUnit
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE 
	(FP.UniqueIdProtocol IS NOT NULL AND FP.IdFascicle = @FascicleId)
	OR (FR.UniqueIdResolution IS NOT NULL AND FR.IdFascicle = @FascicleId)
	OR (FD.UniqueIdDocumentSeriesItem IS NOT NULL AND FD.IdFascicle = @FascicleId)

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
--############################################################################
PRINT 'Rinominazione funzione SQL: AuthorizedDocuments';
GO

DROP FUNCTION [dbo].[AuthorizedDocuments]
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_AllowedDocumentUnits] 
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
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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
				OR ((DU.Environment = 4 OR DU.Environment > 100) AND (RG.DocumentSeriesRights like '1%'))) 
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
--############################################################################
PRINT N'Rinominazione funzione DocumentUnit_HasViewableDocument';
GO

DROP FUNCTION [dbo].[DocumentUnit_HasViewableDocument]
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_HasVisibilityRight] 
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
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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
					OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))))

	 OR exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
				where  RG.IdRoleGroup = DUR.UniqueIdRole AND
				MSG.IdGroup IS NOT NULL AND ((DU.Environment = 1 AND (RG.ProtocolRights like '__1%'))
				OR (DU.Environment = 2 AND (RG.ResolutionRights like '__1%'))
				OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '__1%')))
				 ))
    
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
--############################################################################
PRINT N'Rinominazione funzione AvailableFasciclesFromProtocol';
GO
DROP FUNCTION [dbo].[AvailableFasciclesFromProtocol]
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromProtocol]
(
	@UniqueIdProtocol uniqueidentifier
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
		F.RegistrationUser,
		F.RegistrationDate			
	FROM Fascicles F 
		LEFT JOIN FascicleProtocols FP on F.IdFascicle = FP.IdFascicle AND FP.UniqueIdProtocol = @UniqueIdProtocol
		LEFT JOIN Protocol P on P.UniqueId = @UniqueIdProtocol
		LEFT JOIN Category C on C.IdCategory = P.IdCategoryAPI
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 1 AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = P.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 1 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 1 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FP.IdFascicleProtocol IS NULL
		AND F.EndDate IS NULL
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
		F.RegistrationUser,
		F.RegistrationDate
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
--############################################################################
PRINT N'Rinominazione funzione PeriodicFasciclesFromDocumentUnit';
GO
DROP FUNCTION [dbo].[PeriodicFasciclesFromDocumentUnit]
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_PeriodicFasciclesFromDocumentUnit]
(
	@UniqueIdUD uniqueidentifier,
	@Environment smallint
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
		F.RegistrationUser,
		F.RegistrationDate			
	FROM Fascicles F 
		LEFT JOIN cqrs.DocumentUnits D on D.IdDocumentUnit = @UniqueIdUD
		LEFT JOIN Category C on C.IdCategory = D.IdCategory
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 1 AND CF.FascicleType in (2 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE ((F.IdCategory = D.IdCategory or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 1 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = @Environment and FascicleType = 2
			ORDER BY IdCategory DESC))))
			AND ((D.RegistrationDate BETWEEN F.StartDate AND F.EndDate) 
					OR (F.EndDate IS NULL AND D.RegistrationDate >= F.StartDate))
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
--############################################################################
PRINT N'Rinominazione funzione GetFascicleContactModel';
GO

DROP FUNCTION [dbo].[GetFascicleContactModels]
GO

CREATE FUNCTION [webapipublic].[Fascicles_FX_FascicleContactModels]
(
	@IdFascicle uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(

WITH tblChild AS
(
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, 1 as IsSelected
        FROM [dbo].[FascicleContacts] FC 
		INNER JOIN [dbo].[Contact] C ON C.Incremental = FC.IdContact AND FC.IdFascicle = @IdFascicle
		GROUP BY C.UniqueId, C.Incremental, C.IncrementalFather, C.IdContactType, C.[Description]
    UNION ALL
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, 0 as IsSelected
		FROM [dbo].[Contact] C 
		INNER JOIN tblChild ON C.Incremental = tblChild.IncrementalFather
),
Results As
(SELECT UniqueId, Incremental as IdContact, IdContactType AS ContactType, [Description], 
		(SELECT TOP 1 I.UniqueId FROM tblChild I WHERE I.Incremental = tblChild.IncrementalFather) AS UniqueIdFather, IsSelected
	FROM tblChild
)
SELECT UniqueId, IdContact, ContactType, [Description], UniqueIdFather, CAST(MAX(IsSelected) as bit) AS IsSelected
FROM Results
GROUP BY UniqueId, IdContact, ContactType, [Description], UniqueIdFather
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
--############################################################################
PRINT N'Rinominazione funzione AvailableFasciclesFromResolution';
GO

DROP FUNCTION [dbo].[AvailableFasciclesFromResolution]
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromResolution]
(
	@UniqueIdResolution uniqueidentifier
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
		F.RegistrationUser,
		F.RegistrationDate						
	FROM Fascicles F 
		LEFT JOIN FascicleResolutions FR on F.IdFascicle = FR.IdFascicle AND FR.UniqueIdResolution = @UniqueIdResolution
		LEFT JOIN Resolution R on R.UniqueId = @UniqueIdResolution
		LEFT JOIN Category C on C.IdCategory = R.IdCategoryAPI
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 2 AND CF.FascicleType in (1, 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = R.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 1 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 1 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FR.IdFascicleResolution IS NULL
		AND F.EndDate IS NULL
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
		F.RegistrationUser,
		F.RegistrationDate
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

--############################################################################
PRINT N'Rinominazione funzione FascicleGenericDocumentUnits';
GO

DROP FUNCTION [dbo].[FascicleGenericDocumentUnits]
GO

CREATE FUNCTION [webapipublic].[DocumentUnit_FX_FascicleDocumentUnits]
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
         ,COALESCE(FP.ReferenceType, FR.ReferenceType, FD.ReferenceType) as ReferenceType
         ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
       FROM cqrs.DocumentUnits DU
       LEFT OUTER JOIN dbo.FascicleProtocols FP ON FP.UniqueIdProtocol = DU.IdDocumentUnit
       LEFT OUTER JOIN dbo.FascicleUDS UDS ON UDS.IdUDS = DU.IdDocumentUnit
       LEFT OUTER JOIN dbo.FascicleResolutions FR ON FR.UniqueIdResolution = DU.IdDocumentUnit
       LEFT OUTER JOIN dbo.FascicleDocumentSeriesItems FD ON FD.UniqueIdDocumentSeriesItem = DU.IdDocumentUnit
       INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
       INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
       WHERE 
       ((FP.UniqueIdProtocol IS NOT NULL AND FP.IdFascicle = @FascicleId)
       OR (FR.UniqueIdResolution IS NOT NULL AND FR.IdFascicle = @FascicleId)
       OR (UDS.IdUDS IS NOT NULL AND UDS.IdFascicle = @FascicleId)
       OR (FD.UniqueIdDocumentSeriesItem IS NOT NULL AND FD.IdFascicle = @FascicleId))
       AND DU.[DocumentUnitName] + DU.Title like '%'+@FilterNameTitle+'%'
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
--############################################################################
PRINT 'Eliminata Funzione in DocumentUnits FascicolableDocumentUnits'
GO

DROP FUNCTION [dbo].[FascicolableDocumentUnits]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--############################################################################
PRINT 'Rinominata Funzione in DocumentUnits FascicolableDocumentUnitsSecurityUser'
GO

DROP FUNCTION [dbo].[FascicolableDocumentUnitsSecurityUser]
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_FascicolableDocumentUnits]
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
		INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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
--############################################################################
PRINT 'Rinominare funzione in Commons GetHierarcyCode'
GO

DROP FUNCTION [dbo].[CategoryHierarcyCode]
GO


CREATE FUNCTION [webapipublic].[Category_FX_HierarcyCode](@IdCategory AS SMALLINT)
    RETURNS NVARCHAR(1000) AS
BEGIN
	DECLARE @Names NVARCHAR(1000) 
	SELECT @Names = COALESCE(@Names + '.','') + CAST(Code AS NVARCHAR(5))
	FROM [dbo].[SplitString]((SELECT TOP 1 FullIncrementalPath FROM [dbo].[Category] WHERE IdCategory = @IdCategory),'|') INNER JOIN [dbo].[Category] ON [Value] = IdCategory 
	ORDER BY [FullCode]
    RETURN @Names
END;
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--############################################################################
PRINT 'Rinominata Funzione in Commons CategoryHierarcyDescription'
GO


DROP FUNCTION [dbo].[CategoryHierarcyDescription]
GO

CREATE FUNCTION [webapipublic].[Category_FX_HierarcyDescription](@IdCategory as smallint)
    RETURNS NVARCHAR(2000) AS
BEGIN
	DECLARE @Names NVARCHAR(2000) 
	SELECT @Names = COALESCE(@Names + ', ','') + CAST(Code AS NVARCHAR(5)) + '.' + CAST(Name AS NVARCHAR(256))
	FROM [dbo].[SplitString]((SELECT TOP 1 FullIncrementalPath FROM [dbo].[Category] WHERE IdCategory = @IdCategory),'|') INNER JOIN [dbo].[Category] on [Value] = IdCategory 
	ORDER BY [FullCode]

    RETURN @Names
END;
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END
--#############################################################################
PRINT 'Rinominata SQL function AuthorizedProtocols'
GO

DROP FUNCTION [dbo].[AuthorizedProtocols]
GO

CREATE FUNCTION [webapiprivate].[Protocol_FX_AuthorizedProtocols]
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
WITH ProtocolTableValued AS(
SELECT P.Year As Year
      ,P.Number As Number
	  ,P.idCategory
	  ,P.idContainer
      ,P.RegistrationDate As RegistrationDate
      ,P.DocumentCode As DocumentCode
      ,P.idStatus As IdStatus
      ,P.[Object]
	  ,P.idType As IdType
	  ,P.idDocument As IdDocument	 
	  ,P.UniqueId AS UniqueId 
	FROM [dbo].[Protocol] P
	INNER JOIN [dbo].[ProtocolRole] PR on P.Year = PR.Year AND P.Number = PR.Number
	INNER JOIN [dbo].[Role] R on PR.idRole = R.idRole 
	INNER JOIN [dbo].[RoleGroup] RG on R.idRole = RG.idRole
	INNER JOIN [dbo].[SecurityGroups] SG on RG.idGroup = SG.idGroup
	INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE 
	P.RegistrationDate between @DateFrom AND @DateTo 
	AND (RG.ProtocolRights like '1%')
	AND (SU.Account = @UserName AND SU.UserDomain = @Domain) 
GROUP BY P.Year, P.Number, P.idCategory, P.idContainer, P.RegistrationDate, P.DocumentCode, P.idStatus, P.[Object], P.idType, P.idDocument, P.UniqueId)
SELECT P.*, T.[idType] As ProtocolType_IdType
	  ,T.[Description] As ProtocolType_Description
	  ,CT.idCategory as Category_IdCategory
	  ,CT.Name as Category_Name
	  ,C.idContainer as Container_IdContainer
	  ,C.Name as Container_Name
	  ,PC.[IDContact] As ProtocolContact_IDContact
	  ,CN.[Description] As ProtocolContact_Description
	  ,PCM.[Incremental] As ProtocolContactManual_Incremental
	  ,PCM.[Description] As ProtocolContactManual_Description
FROM ProtocolTableValued P
		INNER JOIN [dbo].[Type] T on P.idType = T.idType
		INNER JOIN [dbo].[Category] CT on P.idCategory = CT.idCategory
		LEFT OUTER JOIN [dbo].[ProtocolContact] PC on P.Year = PC.Year AND P.Number = PC.Number
			LEFT OUTER JOIN [dbo].[Contact] CN on PC.IDContact = CN.Incremental
		LEFT OUTER JOIN [dbo].[ProtocolContactManual] PCM on P.Year = PCM.Year AND P.Number = PCM.Number	
		INNER JOIN [dbo].[Container] C on P.idContainer = C.idContainer);


GO
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END


--#############################################################################
PRINT 'Rinominare SQL func GetProtocolContactModel'
GO

DROP FUNCTION [dbo].[GetProtocolContactModels]
GO

CREATE FUNCTION [webapipublic].[ProtocolContact_FX_ProtocolContacts]
(
	@ProtocolYear smallint, @ProtocolNumber int
)
RETURNS TABLE 
AS
RETURN 
(
WITH tblChild AS
(
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, PC.ComunicationType, PC.[Type], 1 as IsSelected
        FROM [dbo].[ProtocolContact] PC 
		INNER JOIN [dbo].[Contact] C ON C.Incremental = PC.IDContact AND PC.Year = @ProtocolYear  and PC.Number = @ProtocolNumber
		GROUP BY C.UniqueId, C.Incremental, C.IncrementalFather, C.IdContactType, C.[Description], PC.ComunicationType, PC.[Type]
    UNION ALL
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, tblChild.ComunicationType, NULL, 0 as IsSelected
		FROM [dbo].[Contact] C 
		INNER JOIN tblChild ON C.Incremental = tblChild.IncrementalFather
),
Results As
(SELECT UniqueId, Incremental as IdContact, IdContactType AS ContactType, [Description], (SELECT TOP 1 I.UniqueId FROM tblChild I WHERE I.Incremental = tblChild.IncrementalFather) AS UniqueIdFather, 
	ComunicationType, [Type], IsSelected
    FROM tblChild
UNION ALL
SELECT UniqueId, NULL, 'M'+PCM.IdContactType,PCM.[Description],NULL,PCM.ComunicationType,PCM.[Type],CAST(1 as bit) AS IsSelected
	FROM ProtocolContactManual PCM
	WHERE PCM.[Year] = @ProtocolYear AND PCM.[Number] = @ProtocolNumber
)
SELECT UniqueId, IdContact, ContactType, [Description], UniqueIdFather, ComunicationType, [Type], CAST(MAX(IsSelected) as bit) AS IsSelected
FROM Results
GROUP BY UniqueId, IdContact, ContactType, [Description], UniqueIdFather, ComunicationType, [Type]
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
PRINT'Rinominata in collaborazioni:ToVisionSign in Collaboration_FX_CollaborationsSigning'
GO

DROP FUNCTION [dbo].[ToVisionSignCollaborations]
GO

CREATE FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsSigning](
	@UserName nvarchar(255),
	@IsRequired bit)
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration			
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			right outer join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration				
				and CC_CS.SignUser = @UserName
				and CC_CS.IsActive = 1
				and ((@IsRequired is null and CC_CS.IsRequired in (1,0)) or (@IsRequired is not null and CC_CS.IsRequired = @IsRequired))
		WHERE			
			Collaboration.IdStatus = 'IN'
			and Collaboration.IdCollaboration not in (SELECT CA.idCollaborationChild
												 FROM   dbo.CollaborationAggregate CA)
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
PRINT 'Rinominata sqlFunc AtVisionSign in ProposedCollaborations'
GO

DROP FUNCTION [dbo].[AtVisionSignCollaborations]
GO

CREATE FUNCTION [webapiprivate].[Collaboration_FX_ProposedCollaborations](
	@UserName nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration			
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			right outer join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration				
		WHERE			
			Collaboration.IdStatus in ('IN', 'DL') and Collaboration.IdStatus is not null
			and Collaboration.RegistrationUser = @UserName
)


GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
BEGIN
INSERT INTO #tmpErrors (Error) VALUES (1);
BEGIN TRANSACTION;
END
GO

--#############################################################################
PRINT 'Drop sqlFunc ODGCollaborations '
GO

DROP FUNCTION [dbo].[ODGCollaborations]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
BEGIN
INSERT INTO #tmpErrors (Error) VALUES (1);
BEGIN TRANSACTION;
END
GO



--#############################################################################
PRINT'Drop SQL Func ODGFIgCHECKEDCOLLABORATIONS'
GO

DROP FUNCTION [dbo].[ODGFlgCheckedCollaborations]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################

PRINT'Rinominata  SQL Func CurrentActivityAllCollaborations'
GO

DROP FUNCTION [dbo].[CurrentActivityAllCollaborations]
GO

CREATE FUNCTION [webapiprivate].[Collaboration_FX_AllUserCollaborations](
	@UserName nvarchar(255),
	@Signers string_list_tbltype READONLY)
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			inner join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration
		WHERE ((CC_CS.SignUser = @UserName AND CC_CS.IsActive <> 1) AND Collaboration.IdStatus NOT IN ('PT', 'DP')) 
		      OR (Collaboration.IdStatus = 'IN' AND CC_CS.SignUser IN (SELECT val FROM @Signers))
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
--############################################################################
PRINT 'Rinominata sql Func CurrentActivityActiveCollaborations'
GO

DROP FUNCTION [dbo].[CurrentActivityActiveCollaborations]
GO


CREATE FUNCTION [webapiprivate].[Collaboration_FX_ActiveUserCollaborations](
	@UserName nvarchar(255),
	@Signers string_list_tbltype READONLY)
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			inner join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration
		WHERE ((CC_CS.SignUser = @UserName AND CC_CS.IsActive = 1) 
					AND Collaboration.IdStatus <> 'PT') 
		      OR (Collaboration.IdStatus = 'IN' AND CC_CS.SignUser IN (SELECT val FROM @Signers))
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
PRINT'Rinominata sql function IsCheckedOutCollaborations'
GO

DROP FUNCTION [dbo].[IsCheckedOutCollaborations]
GO

CREATE FUNCTION [webapiprivate].[Collaboration_FX_CheckedOutCollaborations](
	@UserName nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration			
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
		WHERE	C_CV.CheckedOut = 1 AND C_CV.CheckOutUser = @UserName
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

--############################################################################
 PRINT'Rinominare sqlFunc ProtocolAdmissionCollaborations'
 GO
 DROP FUNCTION [dbo].[ProtocolAdmissionCollaborations]
 GO
 
 
 CREATE FUNCTION [webapiprivate].[Collaboration_FX_ProtocolAdmissionCollaborations](
 	@UserName nvarchar(255))
 	RETURNS TABLE
 AS 
 	RETURN
 	(
 		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
 			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
 			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
 			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
 			   Collaboration.LastChangedDate,
 			   
 			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
 			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
 			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
 			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
 			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
 		
 		FROM   dbo.Collaboration Collaboration
 			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration			
 			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
 			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
 			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
 			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
 			inner join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration
 		WHERE	(Collaboration.RegistrationUser = @UserName OR CC_CS.SignUser = @UserName) AND Collaboration.IdStatus = 'DP'
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
 PRINT'SQL func CurrentActivityPastCollaborations RINOMINATA in Collaboration_FX_AlreadySignedCollaborations'
 GO
 
 DROP FUNCTION [dbo].[CurrentActivityPastCollaborations]
 GO
 
 CREATE FUNCTION [webapiprivate].[Collaboration_FX_AlreadySignedCollaborations](
 	@UserName nvarchar(255))
 	RETURNS TABLE
 AS 
 	RETURN
 	(
 		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
 			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
 			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
 			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
 			   Collaboration.LastChangedDate,
 			   
 			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
 			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
 			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
 			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
 			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
 		FROM   dbo.Collaboration Collaboration
 			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
 			inner join dbo.CollaborationSigns C_CSM on Collaboration.IdCollaboration = C_CSM.IdCollaboration AND C_CSM.IsActive = 1
 			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
 			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
 			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
 			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
 		WHERE ((C_CS.SignUser = @UserName AND C_CS.IsActive <> 1 AND C_CS.Incremental < C_CSM.Incremental) 
 					AND Collaboration.IdStatus <> 'PT')
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
 PRINT'Rinominare sql func ToManageCollaborations in Collaboration_FX_CollaborationManaging'
 GO

 DROP FUNCTION [dbo].[ToManageCollaborations]
 GO
 
 
 CREATE FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsManaging](
 	@UserName nvarchar(255))
 	RETURNS TABLE
 AS 
 	RETURN
 	(
 		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
 			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
 			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
 			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
 			   Collaboration.LastChangedDate,
 			   
 			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
 			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
 			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
 			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
 			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
 		FROM   dbo.Collaboration Collaboration
 			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
 			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
 			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
 			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
 			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
 		WHERE  Collaboration.IdStatus = 'DP'
 			   and (1 = (SELECT TOP (1) 1
 						   FROM   dbo.CollaborationUsers C_CU
 						   WHERE  C_CU.IdCollaboration = Collaboration.IdCollaboration
 								  and C_CU.Account = @UserName
 								  and C_CU.DestinationType = 'P')
 					and 1 = 1
 					 OR EXISTS (SELECT Account
 								FROM   RoleUser
 								WHERE  Type = 'S'
 									   AND Enabled = 1
 									   AND Account = @UserName
 									   AND IdRole IN (SELECT IdRole
 													  FROM   CollaborationUsers
 													  WHERE  IdCollaboration = Collaboration.IdCollaboration
 															 AND DestinationType = 'S'
 															 and DestinationFirst = 1)
 									   AND Account NOT IN (SELECT Account
 														   FROM   CollaborationUsers
 														   WHERE  IdCollaboration = Collaboration.IdCollaboration
 																  AND DestinationType = 'P'
 																  and DestinationFirst = 1)))
 			   and Collaboration.DocumentType in ('P', 'D', 'A', 'O', 'S', 'U', 'W')
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
 PRINT'Rinominata function RegisteredCollaborations'
 GO
 
 DROP FUNCTION [dbo].[RegisteredCollaborations]
 GO
 
 CREATE FUNCTION [webapiprivate].[Collaboration_FX_RegisteredCollaborations](
 	@UserName nvarchar(255), 
 	@DateFrom datetimeoffset,
 	@DateTo datetimeoffset)
 	RETURNS TABLE
 AS 
 	RETURN
 	(
 		WITH CollaborationTableValue AS(
             SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
                      Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
                      Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
                      Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
                      Collaboration.LastChangedDate, Collaboration.idDocumentSeriesItem                                          
             FROM dbo.Collaboration Collaboration                                   
 			left outer join dbo.CollaborationSigns s4_ on Collaboration.IdCollaboration = s4_.IdCollaboration and (s4_.SignUser = @UserName)
             left outer join dbo.CollaborationUsers u5_ on Collaboration.IdCollaboration = u5_.IdCollaboration and (u5_.Account = @UserName)
             left outer join dbo.RoleUser RU_C1 on Collaboration.RegistrationUser = RU_C1.Account AND RU_C1.Account = @UserName AND RU_C1.[Enabled] = 1   
             left outer join dbo.CollaborationUsers RU_C2 on Collaboration.IdCollaboration = RU_C2.IdCollaboration and RU_C2.DestinationType = 'S'
             left outer join dbo.RoleUser RU_C2_S on RU_C2.IdRole = RU_C2_S.IdRole and RU_C2_S.Account = @UserName AND RU_C2_S.[Enabled] = 1 
             left outer join dbo.CollaborationSigns RU_C3 on Collaboration.IdCollaboration = RU_C3.IdCollaboration
             left outer join dbo.RoleUser RU_C3_S on RU_C3.SignUser = RU_C3_S.Account and RU_C3_S.Account = @UserName AND RU_C3_S.[Enabled] = 1                   
             WHERE Collaboration.RegistrationDate between @DateFrom and @DateTo AND
             (1 = 0 or Collaboration.RegistrationUser = @UserName
                               or s4_.IdCollaborationSign is not null
                               or u5_.IdCollaborationUser is not null
                               or RU_C1.IdRole is not null
                               or RU_C2_S.Incremental is not null
                               or RU_C3_S.Incremental is not null)       
                               and Collaboration.IdStatus = 'PT' and Collaboration.DocumentType in ('P', 'D', 'A', 'O', 'S', 'U', 'W')
 			GROUP BY Collaboration.IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
                      Collaboration.MemorandumDate, Collaboration.Object, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
                      Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
                      Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
                      Collaboration.LastChangedDate, Collaboration.idDocumentSeriesItem
 			)
 
             SELECT Collaboration.*, C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
 					C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
                     C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
                     r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
                     dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
 			FROM CollaborationTableValue Collaboration
             inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
             inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
             inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
             left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
             left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
             left outer join dbo.Protocol p1_ on Collaboration.Year = p1_.Year and Collaboration.Number = p1_.Number
             WHERE (p1_.RegistrationDate between @DateFrom and @DateTo and p1_.idStatus = 0)
 						or (r2_.ProposeDate between @DateFrom and @DateTo and r2_.idStatus = 0)
                         or (dsi3_.RegistrationDate between @DateFrom and @DateTo and dsi3_.Status in (1, 3))   
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

PRINT 'Rinominare sqlFunc GetProtocolRoleModels'
GO

DROP FUNCTION [dbo].[GetProtocolRoleModels]
GO

CREATE FUNCTION [webapipublic].[ProtocolRole_FX_ProtocolRoles]
(
	@ProtocolYear smallint, @ProtocolNumber int
)
RETURNS TABLE 
AS
RETURN 
(
WITH tblChild AS
(
    SELECT R.UniqueId, R.IdRole, R.Name, R.idRoleFather, PR.DistributionType, PR.[Type], 1 as IsAuthorized
        FROM [dbo].[ProtocolRole] PR 
		INNER JOIN [dbo].[Role] R ON R.idRole = PR.idRole AND PR.Year = @ProtocolYear  and PR.Number = @ProtocolNumber
		GROUP BY R.UniqueId, R.IdRole, R.idRoleFather, R.Name, PR.DistributionType, PR.[Type]
    UNION ALL
    SELECT R.UniqueId, R.IdRole, R.Name, R.idRoleFather, NULL, NULL,  0 as IsAuthorized
		FROM [dbo].[Role] R 
		INNER JOIN tblChild ON R.IdRole = tblChild.idRoleFather
),
Results AS 
(SELECT UniqueId, IdRole, Name, (SELECT TOP 1 I.UniqueId FROM tblChild I WHERE I.idRole = tblChild.idRoleFather) AS UniqueIdFather, 
	DistributionType, [Type], IsAuthorized
    FROM tblChild)

SELECT UniqueId, IdRole, Name, UniqueIdFather, DistributionType, [Type], CAST(MAX(IsAuthorized) as bit) AS IsAuthorized
    FROM Results
	GROUP BY UniqueId, IdRole, Name, UniqueIdFather, DistributionType, [Type]
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


PRINT 'CREATE FUNCTION [dbo].[Resolution_FX_AlboPretorioResolutions]'
GO


IF object_id('dbo.Resolution_FX_AlboPretorioResolutions') IS NOT NULL
BEGIN
    DROP FUNCTION [dbo].[Resolution_FX_AlboPretorioResolutions]
END
GO

CREATE FUNCTION [webapipublic].[Resolution_FX_AlboPretorioResolutions]
(
	@OnlinePublicationInterval SMALLINT,
	@IdType TINYINT
)
RETURNS TABLE
AS RETURN
(
	SELECT R.idResolution as EntityId,
	       R.Year,
		   R.Number,
		   R.UniqueId,
		   R.ServiceNumber,
		   R.Object,
		   R.InclusiveNumber,
		   R.AdoptionDate,
		   R.PublishingDate,
		   R.EffectivenessDate,
		   R.idStatus as Status,
		   R.idType as IdType,	
		   C.Code as ProposerCode,
		   C.Description as ProposerDescription
	FROM Resolution R 
	LEFT OUTER JOIN ResolutionContact RC on RC.UniqueIdResolution = R.UniqueId AND RC.ComunicationType = 'P'
	LEFT OUTER JOIN Contact C on C.Incremental = RC.idContact
	WHERE R.PublishingDate IS NOT NULL AND R.idType = @IdType AND R.idStatus = 0 
	AND	(SELECT CASE 
				   WHEN R.PublishingDate = R.WebPublicationDate then R.PublishingDate
				   WHEN R.WebPublicationDate is null then R.PublishingDate
				   WHEN R.WebPublicationDate is not null and R.WebPublicationDate > R.PublishingDate and R.WebPublicationDate > getdate() then getdate()
				   WHEN R.WebPublicationDate is not null and R.WebPublicationDate > R.PublishingDate and R.WebPublicationDate <= getdate() then R.WebPublicationDate
		 END) between dateadd(day,-@OnlinePublicationInterval,getdate()) and getdate()
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