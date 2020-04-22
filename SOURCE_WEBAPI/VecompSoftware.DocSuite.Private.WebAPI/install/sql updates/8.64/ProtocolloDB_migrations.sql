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
PRINT 'CREATE FUNCTION SQL Fascicles_FX_AvailableFasciclesFromUDS';
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]
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
		LEFT JOIN FascicleUDS FU on F.IdFascicle=FU.IdFascicle and FU.IdUDS = @UniqueIdUD
		LEFT JOIN cqrs.DocumentUnits D on D.IdDocumentUnit = @UniqueIdUD
		LEFT JOIN Category C on C.IdCategory = D.IdCategory
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment =  @Environment AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
WHERE (F.IdCategory = D.IdCategory or (CF2.FascicleType = 0 and CF2.DSWEnvironment = @Environment and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = @Environment and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FU.IdFascicleUDS IS NULL
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
PRINT 'ALTER FUNCTION SQL DocumentUnit_FX_FascicleDocumentUnits';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]
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
	  ,COALESCE(FP.ReferenceType, FR.ReferenceType, FD.ReferenceType, FU.ReferenceType) as ReferenceType
	  ,FU.IdUDSRepository as IdUDSRepository
	  ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN dbo.FascicleProtocols FP ON FP.UniqueIdProtocol = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleResolutions FR ON FR.UniqueIdResolution = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleDocumentSeriesItems FD ON FD.UniqueIdDocumentSeriesItem = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleUDS FU ON FU.IdUDS = DU.IdDocumentUnit
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE 
	(FP.UniqueIdProtocol IS NOT NULL AND FP.IdFascicle = @FascicleId)
	OR (FR.UniqueIdResolution IS NOT NULL AND FR.IdFascicle = @FascicleId)
	OR (FD.UniqueIdDocumentSeriesItem IS NOT NULL AND FD.IdFascicle = @FascicleId)
	OR (FU.IdUDS IS NOT NULL AND FU.IdFascicle = @FascicleId)

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
PRINT N'Rinominazione funzione AvailableFasciclesFromProtocol con aggiunta della colonna VisibilityType';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromProtocol]
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
		F.VisibilityType,
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
PRINT N'Modifica funzione PeriodicFasciclesFromDocumentUnit con aggiunta colonna VisibilityType';
GO
ALTER FUNCTION [webapiprivate].[Fascicles_FX_PeriodicFasciclesFromDocumentUnit]
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
		F.VisibilityType,
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
PRINT N'Modifica funzione AvailableFasciclesFromResolution con aggiunta colonna VisibilityType';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromResolution]
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
		F.VisibilityType,
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
PRINT N'Modifica funzione AvailableFasciclesFromUDS con aggiunta colonna VisibilityType';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]
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
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate			
	FROM Fascicles F
		LEFT JOIN FascicleUDS FU on F.IdFascicle=FU.IdFascicle and FU.IdUDS = @UniqueIdUD
		LEFT JOIN cqrs.DocumentUnits D on D.IdDocumentUnit = @UniqueIdUD
		LEFT JOIN Category C on C.IdCategory = D.IdCategory
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment =  @Environment AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
WHERE (F.IdCategory = D.IdCategory or (CF2.FascicleType = 0 and CF2.DSWEnvironment = @Environment and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = @Environment and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FU.IdFascicleUDS IS NULL
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
PRINT 'ALTER FUNCTION SQL DocumentUnit_FX_HasVisibilityRight';
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
				)))
    
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
PRINT 'DROP indice IX_Fascicles_Year_IdCategory_Number sulla tabella Fascicles';
GO

DROP INDEX [IX_Fascicles_Year_IdCategory_Number] ON [dbo].[Fascicles]
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
PRINT N'Modifica funzione [webapiprivate].[Fascicles_FX_AvailableFasciclesFromResolution]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromResolution]
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
		AND (NOT F.FascicleType = 4)
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
--#############################################################################
PRINT N'Modifica funzione [webapiprivate].[Fascicles_FX_AvailableFasciclesFromProtocol]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromProtocol]
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
		F.VisibilityType,
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
		AND (NOT F.FascicleType = 4)
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
PRINT N'Modifica funzione [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]
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
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate			
	FROM Fascicles F
		LEFT JOIN FascicleUDS FU on F.IdFascicle=FU.IdFascicle and FU.IdUDS = @UniqueIdUD
		LEFT JOIN cqrs.DocumentUnits D on D.IdDocumentUnit = @UniqueIdUD
		LEFT JOIN Category C on C.IdCategory = D.IdCategory
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment =  @Environment AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
WHERE (F.IdCategory = D.IdCategory or (CF2.FascicleType = 0 and CF2.DSWEnvironment = @Environment and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = @Environment and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FU.IdFascicleUDS IS NULL
		AND F.EndDate IS NULL
		AND (NOT F.FascicleType = 4)
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
PRINT N'Creata funzione [webapiprivate].[Fascicle_FX_IsCurrentUserManagerOnActivityFascicle]';
GO

CREATE FUNCTION [webapiprivate].[Fascicle_FX_IsCurrentUserManagerOnActivityFascicle]
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
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Description = @Description 
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
PRINT N'Create function [webapiprivate].[Fascicle_FX_HasDocumentVisibilityRight]';
GO

CREATE FUNCTION [webapiprivate].[Fascicle_FX_HasDocumentVisibilityRight]
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
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
        GROUP BY SG.IdGroup
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM [dbo].[Fascicles] F
	LEFT OUTER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = F.IdFascicle
	WHERE F.IdFascicle = @IdFascicle
	AND F.VisibilityType = 1
	AND exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
				INNER JOIN Role R on RG.idRole = R.idRole
				where  R.IdRole = FR.IdRole AND FR.RoleAuthorizationType = 1 AND
				MSG.IdGroup IS NOT NULL AND ((RG.ProtocolRights <> @EmptyRights)
				OR (RG.ResolutionRights <> @EmptyRights)
				OR (RG.DocumentRights <> @EmptyRights)
				OR (RG.DocumentSeriesRights <> @EmptyRights))
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
PRINT N'Creata funzione [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle]';
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle] 
(	
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@FascicleId uniqueidentifier,
	@Year smallint = null,
	@Number int = null,
	@DocumentUnitName nvarchar(256) = null,
	@CategoryId smallint = null,
	@Subject nvarchar(256) = null
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
	FROM cqrs.DocumentUnits DU
	INNER JOIN [dbo].[Container] C on DU.IdContainer = C.IdContainer
	INNER JOIN [dbo].[Category] CT on DU.IdCategory = CT.IdCategory
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
	WHERE (@Year IS NULL OR DU.Year = @Year) 
	AND (@Number IS NULL OR DU.Number = @Number)
	AND (@DocumentUnitName IS NULL OR DU.DocumentUnitName = @DocumentUnitName)
	AND (@CategoryId IS NULL OR DU.IdCategory = @CategoryId)
	AND (@Subject IS NULL OR DU.Subject like '%'+@Subject+'%')
	AND ((exists (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType = 4)
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
		OR (not exists (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType = 4) 
			AND DU.Environment IN (SELECT DSWEnvironment FROM [dbo].[CategoryFascicles] CF
						  INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = CF.IdCategory
						  WHERE CF.FascicleType = 1 AND F.IdFascicle = @FascicleId)))
	GROUP BY DU.[IdDocumentUnit]
		  ,DU.[IdFascicle]
		  ,DU.[EntityId]
		  ,DU.[Year]
		  ,DU.[Number]
		  ,DU.[Title]
		  ,DU.[Subject]
		  ,DU.[DocumentUnitName]
		  ,DU.[Environment]
		  ,DU.[RegistrationUser]
		  ,DU.[RegistrationDate]
		  ,DU.[IdUDSRepository]
		  ,CT.idCategory
		  ,CT.Name
		  ,C.idContainer
		  ,C.Name
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