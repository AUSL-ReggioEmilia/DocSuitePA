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
PRINT N'CREATE NONCLUSTERED INDEX [IX_DocumentUnits_Year]';
GO

CREATE NONCLUSTERED INDEX [IX_DocumentUnits_Year]
ON [cqrs].[DocumentUnits] ([Year])
INCLUDE ([IdDocumentUnit],[IdContainer],[IdFascicle],[Environment])
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
PRINT N'Creata SQLFUNCTION [dbo].[UserSecurityGroups]';
GO

CREATE FUNCTION [dbo].[UserSecurityGroups](
	@UserName nvarchar(255),
	@Domain nvarchar(255)
)
RETURNS TABLE
AS
RETURN
(
	SELECT SG.IdGroup 
	FROM [dbo].[SecurityGroups] SG 
	LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
	OR SG.AllUsers = 1
	GROUP BY SG.IdGroup
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
PRINT N'Modificata SQLFUNCTION [webapiprivate].[DocumentUnit_FX_AllowedDocumentUnits]';
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
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE DU.RegistrationDate BETWEEN @DateFrom AND @DateTo 
	AND ( (DUR.UniqueIdRole IS NOT NULL
				AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
				OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
				OR ((DU.Environment = 4 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
				AND EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = RG.IdGroup))
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

PRINT N'Modificata SQLFUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle] ';
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
		 SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
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
					 WHERE CG.IdContainer = DU.IdContainer 
					 AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
					 AND (
					 (DU.Environment = 1 AND (CG.Rights like '__1%'))
					 OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					 OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					 OR ((DU.Environment = 7 OR DU.Environment > 99) AND (CG.UDSRights like '__1%'))
					)) 
					 OR EXISTS (SELECT top 1 RG.idRole
						FROM [dbo].[RoleGroup] RG
						INNER JOIN Role R ON RG.idRole = R.idRole
						WHERE  R.UniqueId = DUR.UniqueIdRole
							   AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
							   OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
						       OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
						       OR ((DU.Environment = 7 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
						       AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup))
				 )
		    )
            OR (NOT EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1)) 
			AND NOT EXISTS (SELECT CF.IdCategoryFascicle FROM [dbo].[CategoryFascicles] CF
						  INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = CF.IdCategory
						  WHERE CF.FascicleType != 1 AND F.IdFascicle = @FascicleId AND CF.DSWEnvironment = DU.Environment)))
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[DocumentUnit_FX_FascicolableDocumentUnits]';
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Dossiers_FX_AuthorizedDossiers]';
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
	@JsonMetadata nvarchar (255),
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
			SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
		),
		MyDossiers AS (
			select IdDossier from
			(select Dossier.IdDossier, row_number() over(order by Dossier.Year, Dossier.Number) as rownum 
			 FROM dbo.Dossiers Dossier
			 inner join dbo.Container C on Dossier.IdContainer = C.idContainer
			 inner join dbo.DossierRoles DR on Dossier.IdDossier = DR.IdDossier and DR.RoleAuthorizationType = 0
			 inner join dbo.Role R on DR.IdRole = R.idRole
			 WHERE  (
			   exists (select top 1 CG.IdContainerGroup
						from dbo.ContainerGroup CG
						where CG.IdContainer = Dossier.IdContainer and EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
							  and CG.DocumentRights like '___1%')
			   or exists (select top 1 RG.idRole
						from dbo.RoleGroup RG
						INNER JOIN Role R on RG.idRole = R.idRole
						where R.idRole = DR.IdRole and EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)
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
			   AND (@JsonMetadata is null or Dossier.JsonMetadata like '%'+@JsonMetadata+'%')
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
where exists (select 1 from MyDossiers where D.IdDossier = MyDossiers.IdDossier)
);
GO
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]';
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
	SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
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
				WHERE CG.ProtocolRights LIKE '____1' AND CG.idcategory = F_C.idcategory
					  AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)		
			))
			OR (F.FascicleType = 4 AND FR.RoleAuthorizationType = 0 AND
				EXISTS (SELECT TOP 1 R.idRole from [dbo].[Role] R
				INNER JOIN [dbo].[RoleGroup] RG ON R.idRole = RG.idRole
				WHERE FR.idrole = R.idRole AND R.IsActive = 1 
				      AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromProtocol]';
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
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 0 AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = P.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 0 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 0 and FascicleType = 1
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

PRINT N'Modifica SQLFUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromResolution]';
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
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 0 AND CF.FascicleType in (1, 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = R.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 0 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 0 and FascicleType = 1
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]
(
	@UniqueIdUD uniqueidentifier
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
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 0 AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
WHERE (F.IdCategory = D.IdCategory or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 0 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 0 and FascicleType = 1
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Fascicles_FX_PeriodicFasciclesFromDocumentUnit]';
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
	WITH 	
	DocumentUnit AS (
		SELECT TOP 1 D.IdDocumentUnit, D.IdCategory, D.RegistrationDate
		FROM [cqrs].[DocumentUnits] D 
		WHERE D.IdDocumentUnit = @UniqueIdUD
	)
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
	WHERE F.FascicleType = 2 AND F.IdCategory = (SELECT IdCategory FROM DocumentUnit)
	      AND (EXISTS (SELECT TOP 1 CF.IdCategoryFascicle FROM CategoryFascicles CF
				   WHERE CF.IdCategory = (SELECT IdCategory FROM DocumentUnit) AND CF.DSWEnvironment = @Environment AND CF.FascicleType = 2))
		  AND EXISTS (SELECT TOP 1 D.IdDocumentUnit FROM DocumentUnit D WHERE (D.RegistrationDate BETWEEN F.StartDate AND F.EndDate) OR (F.EndDate IS NULL AND D.RegistrationDate >= F.StartDate))
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Protocol_FX_AuthorizedProtocols]';
GO

ALTER FUNCTION [webapiprivate].[Protocol_FX_AuthorizedProtocols]
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
	WHERE 
	P.RegistrationDate between @DateFrom AND @DateTo 
	AND (RG.ProtocolRights like '1%') 
	AND EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = RG.IdGroup)
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[TemplateCollaboration_FX_AuthorizedTemplates]';
GO

ALTER FUNCTION [webapiprivate].[TemplateCollaboration_FX_AuthorizedTemplates]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256)
)
RETURNS TABLE
AS
RETURN
(
SELECT TC.[IdTemplateCollaboration] AS UniqueId
      ,TC.[Name]
      ,TC.[Status]
      ,TC.[DocumentType]
      ,TC.[IdPriority]
      ,TC.[Object]
      ,TC.[Note]
      ,TC.[IsLocked]
      ,TC.[WSManageable]
      ,TC.[WSDeletable]
      ,TC.[RegistrationUser]
      ,TC.[RegistrationDate]
      ,TC.[LastChangedUser]
      ,TC.[LastChangedDate]
	FROM [dbo].[TemplateCollaborations] TC
	LEFT OUTER JOIN [dbo].[TemplateCollaborationRoles] TCR on TC.IdTemplateCollaboration = TCR.IdTemplateCollaboration
	LEFT OUTER JOIN [dbo].[Role] R on TCR.idRole = R.idRole 
	LEFT OUTER JOIN [dbo].[RoleGroup] RG on R.idRole = RG.idRole
	WHERE (TCR.IdTemplateCollaboration IS NULL OR (((TC.DocumentType = 'P' AND RG.ProtocolRights like '1%')
	OR ((TC.DocumentType = 'A' OR TC.DocumentType = 'D') AND RG.ResolutionRights like '1%')
	OR ((TC.DocumentType = 'S' OR TC.DocumentType = 'UDS' OR ISNUMERIC(TC.DocumentType) = 1) AND RG.DocumentSeriesRights like '1%'))
	AND EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = RG.IdGroup)))
	AND TC.Status = 1
GROUP BY TC.[IdTemplateCollaboration]
      ,TC.[Name]
      ,TC.[Status]
      ,TC.[DocumentType]
      ,TC.[IdPriority]
      ,TC.[Object]
      ,TC.[Note]
      ,TC.[IsLocked]
      ,TC.[WSManageable]
      ,TC.[WSDeletable]
      ,TC.[RegistrationUser]
      ,TC.[RegistrationDate]
      ,TC.[LastChangedUser]
      ,TC.[LastChangedDate])
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[UDSRepository_FX_InsertableRepositoriesByTypology]';
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
						where CG.IdContainer = R.IdContainer 
						      and EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = CG.IdGroup)
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]';
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
				WHERE R.idRole = WRR.IdRole 
				AND EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = RG.IdGroup)
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
PRINT N'Modifica SQLFUNCTION ';
GO


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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle]';
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
		SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
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
					 WHERE CG.IdContainer = DU.IdContainer 
					 AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
					 AND (
					 (DU.Environment = 1 AND (CG.Rights like '__1%'))
					 OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					 OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					 OR ((DU.Environment = 7 OR DU.Environment > 99) AND (CG.UDSRights like '__1%'))
					)) 
					 OR exists (SELECT top 1 RG.idRole
						FROM [dbo].[RoleGroup] RG
						INNER JOIN Role R on RG.idRole = R.idRole
						WHERE  R.UniqueId = DUR.UniqueIdRole
							   AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
							   OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
						       OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
						       OR ((DU.Environment = 7 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
						       AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup))
				 )
		    )
			OR (NOT EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1)) 
			AND NOT EXISTS (SELECT CF.IdCategoryFascicle FROM [dbo].[CategoryFascicles] CF
						  INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = CF.IdCategory
						  WHERE CF.FascicleType != 1 AND F.IdFascicle = @FascicleId AND CF.DSWEnvironment = DU.Environment)))
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[DocumentUnit_FX_HasVisibilityRight]';
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
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
	WHERE DU.IdDocumentUnit = @IdDocumentUnit
	
	and ( exists ( select top 1 CG.idContainerGroup
				 from [dbo].[ContainerGroup] CG 
				 where CG.IdContainer = DU.IdContainer  
				 AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
				 AND ((DU.Environment = 1 AND (CG.Rights like '__1%'))
				   OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
				   OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
				   OR (DU.Environment > 99 AND (CG.UDSRights like '__1%'))
					))

	 OR exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN Role R on RG.idRole = R.idRole
				where  R.UniqueId = DUR.UniqueIdRole 
				AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup) 
				AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
				  OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
				  OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
				  OR (DU.Environment > 99 AND (RG.DocumentSeriesRights like '1%'))
				))
	OR (DU.Environment = 1 and exists (select top 1 IdProtocolUser from ProtocolUsers PU where PU.UniqueIdProtocol = DU.IdDocumentUnit AND PU.Account= @Domain+'\'+@UserName AND PU.Type in (1,2)))
	OR (DU.Environment = 2 and exists (select top 1 idResolution from Resolution R where R.UniqueId = @IdDocumentUnit and R.EffectivenessDate is not null and R.EffectivenessUser is not null))
	OR (DU.Environment > 99 and exists (select top 1 IdDocumentUnitUser from cqrs.DocumentUnitUsers DUU where DUU.IdDocumentUnit = @IdDocumentUnit and DUU.Account = @Domain+'\'+@UserName))
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Dossiers_FX_CountAuthorizedDossiers]';
GO

ALTER FUNCTION [webapiprivate].[Dossiers_FX_CountAuthorizedDossiers](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Year smallint,
	@Number smallint,
	@Subject nvarchar(255),
	@ContainerId smallint,
	@MetadataRepositoryId uniqueidentifier,
	@MetadataValue nvarchar(255)
)
RETURNS INT
AS
	BEGIN
	DECLARE @CountDossiers INT;
	WITH 	
	MySecurityGroups AS (
		SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
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
					where CG.IdContainer = Dossier.IdContainer and EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
							and CG.DocumentRights like '___1%')
			or exists (select top 1 RG.idRole
					   from dbo.RoleGroup RG
					   INNER JOIN Role R on RG.idRole = R.idRole
					   where R.idRole = DR.IdRole and EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)
							and RG.DocumentRights like '1%')
			)
			and (@Year is null or Dossier.Year = @Year)
		    and (@Number is null or Dossier.Number = @Number)
			and (@Subject is null or Dossier.Subject like '%'+@Subject+'%')
			and (@ContainerId is null or C.idContainer = @ContainerId)
			and (@MetadataRepositoryId is null or Dossier.IdMetadataRepository= @MetadataRepositoryId)
			 AND(@MetadataValue is null or Dossier.JsonMetadata like '%'+@MetadataValue+'%')

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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Dossiers_FX_HasInsertRight]';
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
	SELECT  @HasRight = CAST(COUNT(1) AS BIT)	
	FROM dbo.ContainerGroup CG	
	WHERE
	exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					INNER JOIN [dbo].[Container] C on CG.idContainer=C.idContainer and C.IsActive = 1 AND C.DocmLocation IS NOT NULL
					where EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = CG.IdGroup) 
					      AND CG.DocumentRights LIKE '1%')
			
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Dossiers_FX_HasManageableRight]';
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
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM dbo.Dossiers D
	LEFT OUTER JOIN [dbo].[DossierRoles] DR on DR.IdDossier = D.IdDossier
	WHERE D.IdDossier = @IdDossier
	and (	exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN Role R on RG.idRole = R.idRole
				where  R.IdRole = DR.IdRole AND DR.RoleAuthorizationType=0 AND
				EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup) AND (RG.DocumentRights like '1%')
				)
				or exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					where CG.IdContainer = D.IdContainer and EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Dossiers_FX_HasModifyRight]';
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
	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM dbo.Dossiers D
	WHERE D.IdDossier = @IdDossier
	and exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					INNER JOIN Container C on CG.IdContainer = C.IdContainer and C.isActive = 1 and C.DocmLocation IS NOT NULL
					where CG.IdContainer = D.IdContainer 
					and EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = CG.IdGroup) 
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Dossiers_FX_HasViewableRight]';
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
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM dbo.Dossiers D
	LEFT OUTER JOIN [dbo].[DossierRoles] DR on DR.IdDossier = D.IdDossier
	WHERE D.IdDossier = @IdDossier
	and (	exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN Role R on RG.idRole = R.idRole
				where R.IdRole = DR.IdRole AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup) 
				      AND (RG.DocumentRights like '1%')
				)
			or exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					where CG.IdContainer = D.IdContainer and EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Fascicle_FX_HasDocumentVisibilityRight]';
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
       set @EmptyRights = '00000000000000000000';

       WITH
       MySecurityGroups AS (
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
    )

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Fascicles] F
       WHERE F.IdFascicle = @IdFascicle
       AND F.VisibilityType = 1
       AND ( exists (select top 1 RG.idRole
                           from [dbo].[RoleGroup] RG
                           INNER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = F.IdFascicle
                           where  RG.IdRole = FR.IdRole AND FR.RoleAuthorizationType in (0, 1) 
						   AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)  
						   AND ((RG.ProtocolRights <> @EmptyRights)
                             OR (RG.ResolutionRights <> @EmptyRights)
                             OR (RG.DocumentRights <> @EmptyRights)
                             OR (RG.DocumentSeriesRights <> @EmptyRights))
                           )
             OR 
                exists (select top 1 CG.idCategory
                           from [dbo].[CategoryGroup] CG
                           where F.IdCategory = CG.IdCategory 
						   AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
						   AND CG.ProtocolRights like '__1%'
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[WorkflowRepository_FX_HasAuthorizedWorkflowRepositories]';
GO

ALTER FUNCTION [webapiprivate].[WorkflowRepository_FX_HasAuthorizedWorkflowRepositories]
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
SELECT @HasWorkflows = CAST(COUNT(1) AS BIT)
FROM [dbo].[WorkflowRepositories] WR
LEFT OUTER JOIN [dbo].[WorkflowRoles] WRR on WR.IdWorkflowRepository = WRR.IdWorkflowRepository	
WHERE (WRR.IdWorkflowRepository IS NULL Or (
		EXISTS (SELECT TOP 1 RG.IdRole
				FROM dbo.RoleGroup RG
				WHERE RG.idRole = WRR.IdRole AND EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = RG.IdGroup)
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
PRINT N'Modifica SQLFUNCTION [webapiprivate].[Fascicles_FX_IsProcedureSecretary]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_IsProcedureSecretary](
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
       and exists (select top 1 CG.IdGroup 
	               from dbo.CategoryGroup CG 
	               where CG.idCategory = @CategoryId 
				   and exists (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = CG.IdGroup)) 
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
PRINT N'Cancellata SQLFUNCTION [webapiprivate].[Category_FX_ParentWithCategoryFascicle]';
GO

DROP FUNCTION [webapiprivate].[Category_FX_ParentWithCategoryFascicle]
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

PRINT N'Modificata store procedure [dbo].[FascicleSubFolder_Insert]';
GO


ALTER PROCEDURE [dbo].[FascicleSubFolder_Insert]
@IdFascicle uniqueidentifier,
@ParentNode hierarchyid,
@IdParentCategory smallint,
@IdParentFolder uniqueidentifier,
@RegistrationUser nvarchar(256)
AS
DECLARE @categoryFascicleId smallint,@idcategory smallint,@node hierarchyid,@childNode hierarchyid, @name nvarchar(256), @idFascicleFolder uniqueidentifier

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  

BEGIN TRY
		BEGIN TRANSACTION FascicleSubFolderInsert
		DECLARE category_cursor CURSOR LOCAL READ_ONLY FOR 
			SELECT C.name, C.idcategory FROM category C 
			WHERE C.idparent = @IdParentCategory and 
				exists (select F.IdCategoryFascicle FROM CategoryFascicles F where F.FascicleType = 0 and F.IdCategory = C.IdCategory)
		
		OPEN category_cursor
		FETCH NEXT FROM category_cursor INTO @name, @idcategory  
		WHILE @@FETCH_STATUS = 0  
		BEGIN
			SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @ParentNode;
			SET @node = @ParentNode.GetDescendant(@childNode, NULL)
		    SET @idFascicleFolder = NEWID()
			
			INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
				[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
			VALUES (@node, @idFascicleFolder, @IdFascicle,@idcategory, @name, 1, 2, getutcdate(), @RegistrationUser, NULL, NULL)
			EXEC [dbo].[FascicleSubFolder_Insert] @IdFascicle, @node, @idcategory, @idFascicleFolder , @RegistrationUser
			FETCH NEXT FROM category_cursor INTO  @name, @idcategory  
		END   
		CLOSE category_cursor;  
		DEALLOCATE category_cursor;  
		
		COMMIT TRANSACTION FascicleSubFolderInsert		
		
END TRY 
BEGIN CATCH 
ROLLBACK TRANSACTION FascicleSubFolderInsert
		
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
PRINT N'Modificata store procedure [dbo].[FascicleFolder_Insert] ';
GO

ALTER PROCEDURE [dbo].[FascicleFolder_Insert] 
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
	@ParentInsertId uniqueidentifier
	AS
	
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid, @allNode hierarchyid, @rootNode hierarchyid, @childNode hierarchyid,@insertiNode hierarchyid,@fascicleNode hierarchyid,@level smallint,@idSubFolder uniqueidentifier

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	
	
	BEGIN TRY
		BEGIN TRANSACTION InsertFascicleFolder
		-- Recupero il parent node
	    SELECT @parentNode = [FascicleFolderNode] FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @ParentInsertId
		
		IF @parentNode IS NULL
			BEGIN
				SELECT @parentNode = MAX([FascicleFolderNode]) from [dbo].[FascicleFolders] where [FascicleFolderNode].GetAncestor(1) = hierarchyid::GetRoot() AND [IdFascicle] = @IdFascicle
			END
		
		IF @parentNode IS NULL
			BEGIN
				SET @parentNode = hierarchyid::GetRoot()
				SET @IdFascicleFolder = @IdFascicle 
			END

		IF @ParentInsertId IS NOT NULL			
			BEGIN
			    SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @parentNode;
				SET @node = @parentNode.GetDescendant(@childNode, NULL)
				
				
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
				[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@node, @IdFascicleFolder, @IdFascicle,@IdCategory, @Name, @Status, @Typology, @RegistrationDate, @RegistrationUser, NULL, NULL)
				
			END	
		ELSE
			BEGIN
			    SELECT @maxNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @parentNode;
				SET @allNode = @parentNode.GetDescendant(@maxNode, NULL)
				PRINT @allNode.ToString()
				
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
													[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@allNode, @IdFascicle, @IdFascicle, NULL, @Name, @Status, 0, @RegistrationDate, @RegistrationUser, NULL, NULL)
				SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @allNode;
				
				SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @allNode;
				SET @fascicleNode = @allNode.GetDescendant(@childNode, NULL)
				SET @idSubFolder = NEWID()
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
													[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@fascicleNode, @idSubFolder, @IdFascicle, @IdCategory, 'Fascicolo', @Status, 1, @RegistrationDate, @RegistrationUser, NULL, NULL)
				
				EXEC [dbo].[FascicleSubFolder_Insert] @IdFascicle, @fascicleNode, @IdCategory, @idSubFolder, @RegistrationUser 
			END
	    COMMIT TRANSACTION InsertFascicleFolder

		SELECT [FascicleFolderNode],[IdFascicleFolder] AS UniqueId,[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
			[Name],[Typology],[FascicleFolderPath],[FascicleFolderLevel],[FascicleFolderParentNode],[ParentInsertId],[Timestamp] 
		FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @IdFascicleFolder
		
		
	END TRY 
	BEGIN CATCH 
		ROLLBACK TRANSACTION InsertFascicleFolder
		
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
PRINT N'ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]';
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
		SELECT FP.UniqueIdProtocol AS IdDocumentUnit, FP.ReferenceType, NULL AS IdUDSRepository
		FROM  dbo.FascicleProtocols FP
		WHERE FP.IdFascicle = @FascicleId AND ((@IdFascicleFolder IS NOT NULL AND FP.IdFascicleFolder = @IdFascicleFolder) OR (@IdFascicleFolder IS NULL))
		UNION ALL
		SELECT FR.UniqueIdResolution AS IdDocumentUnit, FR.ReferenceType, NULL AS IdUDSRepository
		FROM  dbo.FascicleResolutions FR
		WHERE FR.IdFascicle = @FascicleId AND ((@IdFascicleFolder IS NOT NULL AND FR.IdFascicleFolder = @IdFascicleFolder) OR (@IdFascicleFolder IS NULL))
		UNION ALL
		SELECT FU.IdUDS AS IdDocumentUnit, FU.ReferenceType, FU.IdUDSRepository AS IdUDSRepository
		FROM  dbo.FascicleUDS FU
		WHERE FU.IdFascicle = @FascicleId AND ((@IdFascicleFolder IS NOT NULL AND FU.IdFascicleFolder = @IdFascicleFolder) OR (@IdFascicleFolder IS NULL))
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
PRINT N'ALTER FUNCTION [webapipublic].[DocumentUnit_FX_FascicleDocumentUnits]';
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
	WITH FascicleDocumentUnits AS
	(
		SELECT FP.UniqueIdProtocol AS IdDocumentUnit, FP.ReferenceType, NULL AS IdUDSRepository
		FROM  dbo.FascicleProtocols FP
		WHERE FP.IdFascicle = @FascicleId
		UNION ALL
		SELECT FR.UniqueIdResolution AS IdDocumentUnit, FR.ReferenceType, NULL AS IdUDSRepository
		FROM  dbo.FascicleResolutions FR
		WHERE FR.IdFascicle = @FascicleId
		UNION ALL
		SELECT FU.IdUDS AS IdDocumentUnit, FU.ReferenceType, FU.IdUDSRepository AS IdUDSRepository
		FROM  dbo.FascicleUDS FU
		WHERE FU.IdFascicle = @FascicleId
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
      ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
       FROM cqrs.DocumentUnits DU
       INNER JOIN FascicleDocumentUnits FDU ON FDU.IdDocumentUnit = DU.IdDocumentUnit
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
PRINT N'CREATE FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringSeriesRole]';
GO

CREATE FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringSeriesRole]
(
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset
)
RETURNS TABLE
AS
RETURN 
(
	WITH 
		ActivePublished AS 
		(
			SELECT RR.[Name] AS [Role], S.[Name] AS DocumentSeries, S.Id AS IdDocumentSeries, Count(*) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesItemRole R 
			INNER JOIN [Role] RR ON RR.IdRole = R.IdRole 
			INNER JOIN DocumentSeriesItem I ON I.Id = R.IdDocumentSeriesItem 
			INNER JOIN DocumentSeries S ON I.IdDocumentSeries = S.Id 
		    WHERE R.LinkType = 0 AND I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
				  (RetireDate IS NULL OR (RetireDate IS NOT NULL AND RetireDate > getdate())) AND 
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo
			GROUP BY RR.[Name], S.[Name], S.Id
		),
		Inserted AS 
		(
			SELECT RR.[Name] AS [Role], S.[Name] AS DocumentSeries, S.Id AS IdDocumentSeries, Count(*) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesItemRole R 
			INNER JOIN [Role] RR ON RR.IdRole = R.IdRole 
			INNER JOIN DocumentSeriesItem I ON I.Id = R.IdDocumentSeriesItem 
			INNER JOIN DocumentSeries S ON I.IdDocumentSeries = S.Id 
		    WHERE R.LinkType = 0 AND I.[Status] = 1 AND 
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo
			GROUP BY RR.[Name], S.[Name], S.Id
		),
		Published AS 
		(
			SELECT RR.[Name] AS [Role], S.[Name] AS DocumentSeries, S.Id AS IdDocumentSeries, Count(*) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesItemRole R 
			INNER JOIN [Role] RR ON RR.IdRole = R.IdRole 
			INNER JOIN DocumentSeriesItem I ON I.Id = R.IdDocumentSeriesItem 
			INNER JOIN DocumentSeries S ON I.IdDocumentSeries = S.Id 
		    WHERE R.LinkType = 0 AND I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo
			GROUP BY RR.[Name], S.[Name], S.Id
		),
		Updated AS 
		(
			SELECT RR.[Name] AS [Role], S.[Name] AS DocumentSeries, S.Id AS IdDocumentSeries, Count(*) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesItemRole R 
			INNER JOIN [Role] RR ON RR.IdRole = R.IdRole 
			INNER JOIN DocumentSeriesItem I ON I.Id = R.IdDocumentSeriesItem 
			INNER JOIN DocumentSeries S ON I.IdDocumentSeries = S.Id 
		    WHERE R.LinkType = 0 AND I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND I.LastChangedDate IS NOT NULL AND  
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo
			GROUP BY RR.[Name], S.[Name], S.Id
		),
		Canceled AS 
		(
			SELECT RR.[Name] AS [Role], S.[Name] AS DocumentSeries, S.Id AS IdDocumentSeries, Count(*) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesItemRole R 
			INNER JOIN [Role] RR ON RR.IdRole = R.IdRole 
			INNER JOIN DocumentSeriesItem I ON I.Id = R.IdDocumentSeriesItem 
			INNER JOIN DocumentSeries S ON I.IdDocumentSeries = S.Id 
		    WHERE R.LinkType = 0 AND I.[Status] = 3 AND 
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo
			GROUP BY RR.[Name], S.[Name], S.Id
		),
		Retired AS 
		(
			SELECT RR.[Name] AS [Role], S.[Name] AS DocumentSeries, S.Id AS IdDocumentSeries, Count(*) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesItemRole R 
			INNER JOIN [Role] RR ON RR.IdRole = R.IdRole 
			INNER JOIN DocumentSeriesItem I ON I.Id = R.IdDocumentSeriesItem 
			INNER JOIN DocumentSeries S ON I.IdDocumentSeries = S.Id 
		    WHERE R.LinkType = 0 AND I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
				  (RetireDate IS NOT NULL AND RetireDate BETWEEN @DateFrom AND @DateTo)
			GROUP BY RR.[Name], S.[Name], S.Id
		)

SELECT AP.[Role], AP.DocumentSeries, AP.IdDocumentSeries,
	   IsNull(AP.[Counter], 0) AS ActivePublished, 
	   IsNull(I.[Counter], 0) AS Inserted, 
	   IsNull(P.[Counter], 0) AS Published, 
	   IsNull(U.[Counter], 0) AS Updated,
	   IsNull(C.[Counter], 0) AS Canceled,
	   IsNull(R.[Counter], 0) AS Retired,
	   AP.LastUpdated
FROM ActivePublished AP 
LEFT JOIN Inserted I ON AP.[Role] = I.[Role] AND AP.DocumentSeries = I.DocumentSeries
LEFT JOIN Published P ON AP.[Role] = P.[Role] AND AP.DocumentSeries = P.DocumentSeries
LEFT JOIN Updated U ON AP.[Role] = U.[Role] AND AP.DocumentSeries = U.DocumentSeries
LEFT JOIN Canceled C ON AP.[Role] = C.[Role] AND AP.DocumentSeries = C.DocumentSeries
LEFT JOIN Retired R ON AP.[Role] = R.[Role] AND AP.DocumentSeries = R.DocumentSeries
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
PRINT N'CREATE FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringQualitySummary]';
GO

CREATE FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringQualitySummary]
(
	@IdDocumentSeries int,
	@DateFROM datetimeoffset,
	@DateTo datetimeoffset
)
RETURNS TABLE
AS
RETURN 
(
	WITH 
		Published AS 
		(
			SELECT S.[Name] AS DocumentSeries, RR.[Name] AS [Role], S.Id AS IdDocumentSeries, RR.IdRole, count(I.Id) AS [Counter]
			FROM DocumentSeries S 
			INNER JOIN DocumentSeriesItem I ON I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			LEFT JOIN [Role] RR ON RR.IdRole = R.IdRole 
			WHERE S.PublicationEnabled = 1 AND (R.Id IS NULL OR (R.Id IS NOT NULL AND R.LinkType = 0)) AND 
			      (@IdDocumentSeries IS NULL OR (@IdDocumentSeries IS NOT NULL AND S.Id = @IdDocumentSeries)) AND 
				  I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
				  I.RegistrationDate BETWEEN @DateFROM AND @DateTo
			GROUP BY S.[Name], RR.[Name], S.Id, RR.IdRole
		),
		WithoutDocuments AS 
		(
		    SELECT S.[Name] AS DocumentSeries, RR.[Name] AS [Role], S.Id AS IdDocumentSeries, RR.IdRole, count(I.Id) AS [Counter]
			FROM DocumentSeries S 
			INNER JOIN DocumentSeriesItem I ON I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			LEFT JOIN [Role] RR ON RR.IdRole = R.IdRole 
			WHERE S.PublicationEnabled = 1 AND (R.Id IS NULL OR (R.Id IS NOT NULL AND R.LinkType = 0)) AND 
			      (@IdDocumentSeries IS NULL OR (@IdDocumentSeries IS NOT NULL AND S.Id = @IdDocumentSeries)) AND 
				  I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
				  I.RegistrationDate BETWEEN @DateFROM AND @DateTo AND
				  ('00000000-0000-0000-0000-000000000000' = ANY (SELECT IdAnnexed) AND ('00000000-0000-0000-0000-000000000000' = ANY (SELECT IdUnpublishedAnnexed)) AND (0 = ANY (SELECT HasMainDocument)))
			GROUP BY S.[Name], RR.[Name], S.Id, RR.IdRole
		),
		PublishedFromResolutions AS 
		(
			SELECT S.[Name] AS DocumentSeries, RR.[Name] AS [Role], S.Id AS IdDocumentSeries, RR.IdRole, count(I.Id) AS [Counter]
			FROM DocumentSeries S 
			INNER JOIN DocumentSeriesItem I ON I.IdDocumentSeries = S.Id 
			INNER JOIN ResolutionDocumentSeriesItem A ON I.Id = A.IdDocumentSeriesItem 
			LEFT JOIN  DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			LEFT JOIN [Role] RR ON RR.IdRole = R.IdRole 
			WHERE S.PublicationEnabled = 1 AND (R.Id IS NULL OR (r.Id IS NOT NULL AND R.LinkType = 0)) AND 
			      (@IdDocumentSeries IS NULL OR (@IdDocumentSeries IS NOT NULL AND S.Id = @IdDocumentSeries)) AND 
				  I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND
				  I.RegistrationDate BETWEEN @DateFROM AND @DateTo
			GROUP BY S.[Name], RR.[Name], S.Id, RR.IdRole
		),
		PublishedFromProtocols AS 
		(
			SELECT S.[Name] AS DocumentSeries, RR.[Name] AS [Role], S.Id AS IdDocumentSeries, RR.IdRole, Count(*) AS [Counter]
			FROM DocumentSeries S 
			INNER JOIN DocumentSeriesItem I ON I.IdDocumentSeries = S.Id 
			INNER JOIN ProtocolDocumentSeriesItems A ON I.Id = A.IdDocumentSeriesItem 
			LEFT JOIN  DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			LEFT JOIN [Role] RR ON RR.IdRole = R.IdRole 
			WHERE S.PublicationEnabled = 1 AND (R.Id IS NULL OR (r.Id IS NOT NULL AND R.LinkType = 0)) AND 
			      (@IdDocumentSeries IS NULL OR (@IdDocumentSeries IS NOT NULL AND S.Id = @IdDocumentSeries)) AND 
				  I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND
				  I.RegistrationDate BETWEEN @DateFROM AND @DateTo
			GROUP BY S.[Name], RR.[Name], S.Id, RR.IdRole
		)

SELECT P.DocumentSeries, IsNull(P.[Role], '(Senza settore)') AS [Role],P.IdDocumentSeries, P.IdRole,
	   IsNull(P.[Counter], 0) AS Published, 
	   IsNull(A.[Counter], 0) AS FromResolution, 
	   IsNull(I.[Counter], 0) AS FromProtocol, 
	   IsNull(P.[Counter], 0) - IsNull(A.[Counter], 0) - IsNull(I.[Counter],0) AS WithoutLink,
	   IsNull(D.[Counter], 0) AS WithoutDocument
FROM Published P 
LEFT JOIN PublishedFromResolutions A ON P.DocumentSeries = A.DocumentSeries AND P.[Role] = A.[Role]
LEFT JOIN PublishedFromProtocols I ON P.DocumentSeries = I.DocumentSeries AND P.[Role] = I.[Role]
LEFT JOIN WithoutDocuments D ON P.DocumentSeries = D.DocumentSeries AND P.[Role] = D.[Role]
)
GO

CREATE NONCLUSTERED INDEX [IX_DocumentSeriesItemRole_IdRole_LinkType]
ON [dbo].[DocumentSeriesItemRole] ([IdRole],[LinkType])
INCLUDE ([IdDocumentSeriesItem])
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
PRINT N'CREATE FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringQualityDetails]';
GO

CREATE FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringQualityDetails]
(
	@IdDocumentSeries int,
	@IdRole smallint,
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset
)
RETURNS TABLE
AS
RETURN 
(
	WITH 
		Published AS 
		(
			SELECT I.Id AS IdDocumentSeriesItem, CAST(I.year AS nvarchar(4))+'/'+RIGHT('0000000'+CAST(i.Number AS nvarchar(7)), 7) AS Identifier, year AS [Year], Number AS [Number], 1 AS [Counter]
			FROM DocumentSeriesItem I 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			WHERE  I.Status = 1 AND I.PublishingDate IS NOT NULL AND I.IdDocumentSeries = @IdDocumentSeries AND 
				   ((R.Id IS NULL AND @IdRole IS NULL) OR (R.Id IS NOT NULL AND R.LinkType = 0 AND R.IdRole = @IdRole)) AND
				   I.RegistrationDate BETWEEN @DateFrom AND @DateTo
		),
		WithoutDocuments AS 
		(
			SELECT I.Id AS IdDocumentSeriesItem, CAST(I.year AS nvarchar(4))+'/'+RIGHT('0000000'+CAST(i.Number AS nvarchar(7)), 7) AS Identifier, year AS [Year], Number AS [Number], 1 AS [Counter]
			FROM DocumentSeriesItem I 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			WHERE  I.Status = 1 AND I.PublishingDate IS NOT NULL AND I.IdDocumentSeries = @IdDocumentSeries AND 
				   ((R.Id IS NULL AND @IdRole IS NULL) OR (R.Id IS NOT NULL AND R.LinkType = 0 AND R.IdRole = @IdRole)) AND
				   I.RegistrationDate BETWEEN @DateFrom AND @DateTo AND
				   ('00000000-0000-0000-0000-000000000000' = ANY (SELECT IdAnnexed) AND ('00000000-0000-0000-0000-000000000000' = ANY (SELECT IdUnpublishedAnnexed)) AND (0 = ANY (SELECT HasMainDocument)))
		),
		PublishedFromResolutions AS 
		(
			SELECT I.Id AS IdDocumentSeriesItem, CAST(I.year AS nvarchar(4))+'/'+RIGHT('0000000'+CAST(i.Number AS nvarchar(7)), 7) AS Identifier,year AS [Year], Number AS [Number], 1 AS [Counter]
			FROM DocumentSeriesItem I 
			INNER JOIN ResolutionDocumentSeriesItem A ON I.Id = a.IdDocumentSeriesItem 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			WHERE  I.Status = 1 AND I.PublishingDate IS NOT NULL AND I.IdDocumentSeries = @IdDocumentSeries AND 
				   ((R.Id IS NULL AND @IdRole IS NULL) OR (R.Id IS NOT NULL AND R.LinkType = 0 AND R.IdRole = @IdRole)) AND
				   I.RegistrationDate BETWEEN @DateFrom AND @DateTo
		),
		PublishedFromProtocols AS 
		(
			SELECT I.Id AS IdDocumentSeriesItem, CAST(I.year AS nvarchar(4))+'/'+RIGHT('0000000'+CAST(i.Number AS nvarchar(7)), 7) AS Identifier,I.year AS [Year], I.Number AS [Number], 1 AS [Counter]
			FROM DocumentSeriesItem I 
			INNER JOIN ProtocolDocumentSeriesItems A ON I.Id = A.IdDocumentSeriesItem 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			WHERE  I.Status = 1 AND I.PublishingDate IS NOT NULL AND I.IdDocumentSeries = @IdDocumentSeries AND 
			       ((R.Id IS NULL AND @IdRole IS NULL) OR (R.Id IS NOT NULL AND R.LinkType = 0 AND R.IdRole = @IdRole)) AND
				   I.RegistrationDate BETWEEN @DateFrom AND @DateTo
		)

SELECT P.IdDocumentSeriesItem, P.Identifier, P.[Year], P.[Number], 
	   IsNull(P.[Counter], 0) AS Published, 
	   IsNull(A.[Counter], 0) AS FromResolution, 
	   IsNull(I.[Counter], 0) AS FromProtocol, 
	   IsNull(P.[Counter], 0) - IsNull(A.[Counter], 0) - IsNull(I.[Counter],0) AS WithoutLink,
	   IsNull(D.[Counter], 0) AS WithoutDocument
FROM Published P 
LEFT JOIN PublishedFromResolutions A ON P.IdDocumentSeriesItem = A.IdDocumentSeriesItem 
LEFT JOIN PublishedFromProtocols I ON P.IdDocumentSeriesItem = I.IdDocumentSeriesItem
LEFT JOIN WithoutDocuments D ON P.IdDocumentSeriesItem = D.IdDocumentSeriesItem
)
GO


CREATE NONCLUSTERED INDEX [IX_DocumentSeriesItem_IdDocumentSeries,Status,RegistrationDate,PublishingDate]
ON [dbo].[DocumentSeriesItem] ([IdDocumentSeries],[Status],[RegistrationDate],[PublishingDate])
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
PRINT N'CREATE FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringSeriesSection]';
GO

CREATE FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringSeriesSection]
(
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset
)
RETURNS TABLE
AS
RETURN 
(
	WITH 
	    Labels AS ( 
		    SELECT F.[Name] AS Family, S.[Name] AS Series
			FROM DocumentSeriesFamily F 
			INNER JOIN DocumentSeries S on S.IdDocumentSeriesFamily = F.Id 
			WHERE S.PublicationEnabled = 1
		),
		ActivePublished AS 
		(
			SELECT F.[Name] AS Family, S.[Name] AS Series, IsNull(B.[Description], '') AS SubSection, count(I.Id) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesFamily F 
			INNER JOIN DocumentSeries S on S.IdDocumentSeriesFamily = F.Id 
			LEFT JOIN DocumentSeriesItem I on I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesSubsection B on I.IdDocumentSeriesSubsection = B.Id
		    WHERE S.PublicationEnabled = 1 AND (I.Id IS NULL OR (I.Id IS NOT NULL AND I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
			      (I.RetireDate IS NULL OR (I.RetireDate IS NOT NULL AND I.RetireDate > getdate())) AND
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo))
			GROUP BY F.[Name], S.[Name], IsNull(B.[Description], '')
		),
		Inserted AS 
		(
			SELECT F.[Name] AS Family, S.[Name] AS Series, IsNull(B.[Description], '') AS SubSection, count(I.Id) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesFamily F 
			INNER JOIN DocumentSeries S on S.IdDocumentSeriesFamily = F.Id 
			LEFT JOIN DocumentSeriesItem I on I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesSubsection B on I.IdDocumentSeriesSubsection = B.Id
		    WHERE S.PublicationEnabled = 1 AND (I.Id IS NULL OR (I.Id IS NOT NULL AND I.[Status] = 1 AND
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo))
			GROUP BY F.[Name], S.[Name], IsNull(B.[Description], '')
		),
		Published AS 
		(
		    SELECT F.[Name] AS Family, S.[Name] AS Series, IsNull(B.[Description], '') AS SubSection, count(I.Id) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesFamily F 
			INNER JOIN DocumentSeries S on S.IdDocumentSeriesFamily = F.Id 
			LEFT JOIN DocumentSeriesItem I on I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesSubsection B on I.IdDocumentSeriesSubsection = B.Id
		    WHERE S.PublicationEnabled = 1 AND (I.Id IS NULL OR (I.Id IS NOT NULL AND I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo))
			GROUP BY F.[Name], S.[Name], IsNull(B.[Description], '')
		),
		Updated AS 
		(
		    SELECT F.[Name] AS Family, S.[Name] AS Series, IsNull(B.[Description], '') AS SubSection, count(I.Id) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesFamily F 
			INNER JOIN DocumentSeries S on S.IdDocumentSeriesFamily = F.Id 
			LEFT JOIN DocumentSeriesItem I on I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesSubsection B on I.IdDocumentSeriesSubsection = B.Id
		    WHERE S.PublicationEnabled = 1 AND (I.Id IS NULL OR (I.Id IS NOT NULL AND I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND I.LastChangedDate IS NOT NULL AND  
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo))
			GROUP BY F.[Name], S.[Name], IsNull(B.[Description], '')
		),
		Canceled AS 
		(
		    SELECT F.[Name] AS Family, S.[Name] AS Series, IsNull(B.[Description], '') AS SubSection, count(I.Id) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesFamily F 
			INNER JOIN DocumentSeries S on S.IdDocumentSeriesFamily = F.Id 
			LEFT JOIN DocumentSeriesItem I on I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesSubsection B on I.IdDocumentSeriesSubsection = B.Id
		    WHERE S.PublicationEnabled = 1 AND (I.Id IS NULL OR (I.Id IS NOT NULL AND I.[Status] = 3 AND
				  I.RegistrationDate BETWEEN @DateFrom AND @DateTo))
			GROUP BY F.[Name], S.[Name], IsNull(B.[Description], '')
		),
		Retired AS 
		(
		    SELECT F.[Name] AS Family, S.[Name] AS Series, IsNull(B.[Description], '') AS SubSection, count(I.Id) AS [Counter], max(i.LastChangedDate) AS LastUpdated
			FROM DocumentSeriesFamily F 
			INNER JOIN DocumentSeries S on S.IdDocumentSeriesFamily = F.Id 
			LEFT JOIN DocumentSeriesItem I on I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesSubsection B on I.IdDocumentSeriesSubsection = B.Id
		    WHERE S.PublicationEnabled = 1 AND (I.Id IS NULL OR (I.Id IS NOT NULL AND I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
				  (I.RetireDate IS NOT NULL AND I.RetireDate BETWEEN @DateFrom AND @DateTo)))
			GROUP BY F.[Name], S.[Name], IsNull(B.[Description], '')
		)

SELECT LBL.Family, LBL.Series, IsNull(AP.SubSection,'') AS SubSection,
	   IsNull(AP.[Counter], 0) AS ActivePublished, 
	   IsNull(I.[Counter], 0) AS Inserted, 
	   IsNull(P.[Counter], 0) AS Published, 
	   IsNull(U.[Counter], 0) AS Updated,
	   IsNull(C.[Counter], 0) AS Canceled,
	   IsNull(R.[Counter], 0) AS Retired,
	   AP.LastUpdated
FROM Labels LBL
LEFT JOIN ActivePublished AP ON AP.Family = LBL.Family AND AP.Series = LBL.Series AND AP.SubSection = ''
LEFT JOIN Inserted I ON AP.Family = I.Family AND AP.Series = I.Series AND AP.SubSection = I.SubSection
LEFT JOIN Published P ON AP.Family = P.Family AND AP.Series = P.Series AND AP.SubSection = P.SubSection
LEFT JOIN Updated U ON AP.Family = U.Family AND AP.Series = U.Series AND AP.SubSection = U.SubSection
LEFT JOIN Canceled C ON AP.Family = C.Family AND AP.Series = C.Series AND AP.SubSection = C.SubSection
LEFT JOIN Retired R ON AP.Family = R.Family AND AP.Series = R.Series AND AP.SubSection = R.SubSection
)
GO
CREATE NONCLUSTERED INDEX [IX_DocumentSeriesItem_IdDocumentSeries]
ON [dbo].[DocumentSeriesItem] ([IdDocumentSeries])
INCLUDE ([Id],[RegistrationDate],[LastChangedDate],[PublishingDate],[RetireDate],[IdDocumentSeriesSubsection],[Status])
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
PRINT N'Creato script SQL per eliminare le cartelle di fascicolo di tipo Inserto, migrare le catene documentali degli inserti nella cartella Fascicolo e migrare la nuova tipologia sottofascicolo = 2';
GO

WITH t (IdFascicleFolderFascicle, IdFascicle)
AS
(
	SELECT IdFascicleFolder, IdFascicle from fasciclefolders
	where Typology = 1 and IdFascicle is not null AND FascicleFolderlevel= 2
)

UPDATE fd SET idfasciclefolder = tt.IdFascicleFolderFascicle 
FROM fascicledocuments fd
INNER JOIN fascicles f on f.idfascicle = fd.idfascicle
INNER JOIN fasciclefolders ff on ff.IdFascicleFolder = fd.idfasciclefolder
INNER JOIN t tt on tt.idfascicle = f.idfascicle
WHERE ff.typology = 2 
 
DELETE FROM fasciclefolders 
WHERE Typology=2 

UPDATE FascicleFolders SET Typology=2 WHERE Typology=4


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
PRINT 'Modificata SQL FUNCTION [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated]';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated](
	@IdDocumentUnit uniqueidentifier,
	@CategoryId smallint
)
RETURNS BIT
AS
BEGIN
	DECLARE @IsAlreadyFascicolated BIT;
	SELECT @IsAlreadyFascicolated = cast(count(1) as bit)
	FROM cqrs.DocumentUnits
	WHERE idDocumentUnit = @IdDocumentUnit
	AND idCategory IN (SELECT idCategory FROM categoryFascicles CF
						WHERE idCategory IN (SELECT idcategory FROM category C 
											WHERE @CategoryId IN (SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
											)
						AND [fascicleType] IN (0,1,2))
	AND NOT idFascicle IS NULL
	RETURN @IsAlreadyFascicolated
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
PRINT N'Modificata SQL FUNCTION [webapiprivate].[DocumentUnit_FX_IsOnlyReferenziable]';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_IsOnlyReferenziable](
	@IdDocumentUnit uniqueidentifier,
	@CategoryId smallint
)
RETURNS BIT
AS
BEGIN
	DECLARE @IsOnlyReferenziable BIT;
	select @IsOnlyReferenziable = cast(count(1) as bit)
	from cqrs.documentunits DU
	where (((select [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated](@IdDocumentUnit, DU.IdCategory)) = 1)
			OR (@CategoryId not in (SELECT Value FROM [dbo].[SplitString]((select FullIncrementalPath from category where idcategory = DU.IdCategory), '|'))))	
	AND DU.iddocumentunit = @IdDocumentUnit 
	RETURN @IsOnlyReferenziable
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
PRINT N'Modificata STORE PROCEDURE [dbo].[DossierFolder_SP_PropagateAuthorizationToDescendants]';
GO

ALTER procedure [dbo].[DossierFolder_SP_PropagateAuthorizationToDescendants]
--parametri store
	@IdParent uniqueidentifier,
	@IdDossier uniqueidentifier,
	@AuthorizationType smallint,
	@AuthorizationTypeDescription nvarchar(256),
    @RegistrationUser nvarchar(256),
	@System nvarchar(30)
AS
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
	BEGIN TRANSACTION PropagateDescendants

		DECLARE @childDossierFolderId as uniqueidentifier;
		DECLARE @folderName as nvarchar(256);
		DECLARE @descriptionLog as nvarchar(max);
		DECLARE @idFascicle as uniqueidentifier = null;
		DECLARE @fascicleName as nvarchar(max);

		DECLARE @ParentRoles table(
			idRole smallint not null,
			Name nvarchar(100) not null
		);
		insert into @ParentRoles
		select dfr.idRole, r.Name from dossierfolderroles dfr inner join role r on dfr.idRole = r.idRole
		where iddossierfolder = @idParent and roleAuthorizationType = @AuthorizationType

		declare @TempRoles table(
			idRole smallint not null
		)

		--fetch cursore
		declare @fetch_children_cursor as int;

		--cursore delle cartelle figlie
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

			--pulisco la variabile tabella dei settori da aggiungere
			delete from @TempRoles
			--inserisco i ruoli da aggiungere
			insert into @TempRoles
			select idRole from @ParentRoles
								except
								select idRole from DossierFolderRoles dfr
								where dfr.iddossierFolder = @childDossierFolderId
								and dfr.roleauthorizationtype = @AuthorizationType

			--inserisco i settori del padre alla cartella figlia	
			insert into DossierFolderRoles
			select newid(), @childDossierFolderId, IdRole, @AuthorizationType, 1, 1, @RegistrationUser, sysdatetimeoffset(), null, null, null
			from @TempRoles; 

			--inserisco i log dell'aggiunta dei settori del padre alla cartella figlia	
			insert into DossierLogs
			select newid(), @IdDossier, sysdatetimeoffset(), @System, @RegistrationUser, 512, 'Autorizzata la cartella '+ @folderName +' al settore '+ pr.Name + ' ('+ cast(pr.idRole as nvarchar) +') per competenza ('+@AuthorizationTypeDescription+')', null, null, null, null, @childDossierFolderId,@IdDossier
			from @ParentRoles pr
			where idRole in (select idRole from @TempRoles); 

			--pulisco la variabile tabella dei settori da aggiungere
			delete from @TempRoles
			--ripopolo la variabile tabella con i settori da aggiungere ai fascicoli
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
				--inserisco i settori anche nei fascicoli
				insert into FascicleRoles
				select newId(), @idFascicle, idrole, 1, @RegistrationUser, sysdatetimeoffset(), null, null, null, 0
				from @TempRoles

				--inserisco i log nei log dei fascicoli dell'aggiunta dei settori
				insert into FascicleLogs
				select newid(), @idFascicle, sysdatetimeoffset(), @System, @RegistrationUser, 4096, 'Aggiunta autorizzazione'+ @AuthorizationTypeDescription+' al settore '+pr.Name+'('+cast(pr.idrole as nvarchar)+')', null, null, null, null,@IdDossier
				from @ParentRoles pr
				where pr.IdRole in (select idrole from @TempRoles)
	
				--inserisco i log nei log dei dossier dell'aggiunta dei settori
				insert into DossierLogs
				select newid(), @iddossier, sysdatetimeoffset(), @System, @RegistrationUser, 512, 'Aggiunta autorizzazione al settore '+pr.Name+'('+cast(pr.idrole as nvarchar)+') nel fascicolo '+@fascicleName, null, null, null, null, @childDossierFolderId,@IdDossier
				from @ParentRoles pr
				where pr.IdRole in (select idrole from @TempRoles)
			end

			--RIMOZIONE SETTORI NON DEL PADRE
			--prima si inseriscono i log della rimozione dei settori dalla cartella
			insert into DossierLogs
			select newid(), @IdDossier, sysdatetimeoffset(), @System, @RegistrationUser, 256, 'Rimossa Autorizzazione '+ @AuthorizationTypeDescription+' al settore '+r.Name+'('+cast(r.idrole as nvarchar)+') alla cartella '+@folderName, null, null, null, null, @childDossierFolderId,@IdDossier
			from DossierFolderRoles dfr
			inner join role r on r.idrole = dfr.idrole
			where dfr.iddossierFolder = @childDossierFolderId
			and dfr.roleauthorizationtype = @AuthorizationType
			and ismaster = 1
			and dfr.idrole not in (select idrole from @ParentRoles)

			--poi si rimuovono i settori della cartella figlia non nella cartella madre
			delete from DossierFolderRoles
			where idrole not in (select idrole from @ParentRoles)
			and roleauthorizationtype = @AuthorizationType
			and ismaster = 1
			and iddossierfolder = @childDossierFolderId
	
			--log della rimozione dei settori non del padre dai fascicoli
			insert into FascicleLogs
			select newid(), @idFascicle, sysdatetimeoffset(), @System, @RegistrationUser, 4096, 'Rimossa autorizzazione '+@AuthorizationTypeDescription+' al settore '+r.Name+ '('+cast(fr.IdRole as nvarchar)+')', null, null, null, null,@IdDossier
			from FascicleRoles fr inner join Role r on r.idRole = fr.IdRole
			where fr.idRole not in (select idRole from @ParentRoles)
			and idFascicle in (select idFascicle from dossierFolders where IdDossierFolder = @childDossierFolderId)
			and roleauthorizationtype = @AuthorizationType

			--la rimozione dei settori non del padre dai fascicoli
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