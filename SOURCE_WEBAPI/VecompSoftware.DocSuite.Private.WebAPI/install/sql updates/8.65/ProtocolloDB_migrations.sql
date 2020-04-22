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
PRINT 'DROP FUNCTION [webapiprivate].[Protocol_FX_ParentWithCategoryFascicle]';
GO
DROP FUNCTION [webapiprivate].[Protocol_FX_ParentWithCategoryFascicle]
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
PRINT 'Create FUNCTION [webapiprivate].[Dossiers_FX_GetDossierContacts]';
GO

CREATE FUNCTION [webapiprivate].[Dossiers_FX_GetDossierContacts]
(
	@IdDossier uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(

WITH tblChild AS
(
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, 1 as IsSelected
        FROM [dbo].[DossierContacts] DC 
		INNER JOIN [dbo].[Contact] C ON C.Incremental = DC.IdContact AND DC.IdDossier = @IdDossier
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

--#############################################################################

PRINT 'CREATE FUNCTION [webapiprivate].[Dossiers_FX_AuthorizedDossiers]';
GO

CREATE FUNCTION [webapiprivate].[Dossiers_FX_AuthorizedDossiers](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Skip int,
	@Top int,
	@Year smallint,
	@Number smallint,
	@Subject nvarchar(255),
	@ContainerId smallint
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
			Group by Dossier.IdDossier, Dossier.Year, Dossier.Number) T where T.rownum > @Skip AND T.rownum <= @Top
		)

SELECT D.IdDossier, D.Year, D.Number, D.Subject, D.RegistrationDate, D.StartDate, D.EndDate,
	   C.idContainer as Container_Id, C.Name as Container_Name,
	   R.idRole as Role_IdRole, R.Name as Role_Name, Contact.Incremental as Contact_Incremenental, Contact.Description as Contact_Description
FROM Dossiers D
left join DossierContacts DC ON DC.IdDossier = D.IdDossier
left join dbo.Contact Contact on DC.IdContact = Contact.Incremental
inner join dbo.Container C on D.IdContainer = C.idContainer
inner join dbo.DossierRoles DR on D.IdDossier = DR.IdDossier and DR.RoleAuthorizationType = 0
inner join dbo.Role R on DR.IdRole = R.idRole
WHERE exists (SELECT * FROM MyDossiers WHERE D.IdDossier = MyDossiers.IdDossier)
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

PRINT 'CREATE FUNCTION [webapiprivate].[Dossiers_FX_CountAuthorizedDossiers]';
GO

CREATE FUNCTION [webapiprivate].[Dossiers_FX_CountAuthorizedDossiers](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Year smallint,
	@Number smallint,
	@Subject nvarchar(255),
	@ContainerId smallint 
)
RETURNS INT
AS
	BEGIN
	DECLARE @CountDossiers INT;
	WITH 	
	MySecurityGroups AS (
		SELECT SG.IdGroup 
		FROM [dbo].[SecurityGroups] SG 
		INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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
PRINT 'CREAZIONE SQL- FUNCTION [webapiprivate].[Dossiers_FX_HasManageableRight]';
GO
CREATE FUNCTION [webapiprivate].[Dossiers_FX_HasManageableRight] 
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
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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
PRINT 'CREAZIONE SQL- FUNCTION [webapiprivate].[Dossiers_FX_HasViewableRight]';
GO

CREATE FUNCTION [webapiprivate].[Dossiers_FX_HasViewableRight] 
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
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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
PRINT 'CREAZIONE SQL-FUNCTION [webapiprivate].[Dossiers_FX_HasInsertRight]';
GO

CREATE FUNCTION [webapiprivate].[Dossiers_FX_HasInsertRight]
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
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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

PRINT 'CREATE FUNCTION [webapiprivate].[Dossiers_FX_HasModifyRight]';
GO

CREATE FUNCTION [webapiprivate].[Dossiers_FX_HasModifyRight]
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
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain= @Domain
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
--############################################################################

PRINT 'CREATE FUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]';
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@idCategory smallint
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
		AND f.EndDate IS NULL
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
PRINT 'Create FUNCTION [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]';
GO

CREATE FUNCTION [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]
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
			INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
			WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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
			AND DU.Environment IN (SELECT DSWEnvironment FROM [dbo].[CategoryFascicles] CF
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
PRINT 'Create FUNCTION [webapiprivate].[Category_FX_CategorySubFascicles]';
GO

CREATE FUNCTION [webapiprivate].[Category_FX_CategorySubFascicles] (
	@IdCategory smallint
	)
	RETURNS TABLE
AS 
	RETURN
	(
		WITH
		MyCategories AS (
		  SELECT IdCategory FROM [dbo].[Category] C
		  WHERE @IdCategory in (SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
		  GROUP BY IdCategory
		)
	  
	  SELECT [CF].[IdCategoryFascicle]
		    ,[CF].[IdCategory]
	    FROM [dbo].[CategoryFascicles] CF
		WHERE [idCategory] IN (select MyCategories.IdCategory from MyCategories)
		AND [FascicleType] = 0
		GROUP BY [CF].[IdCategoryFascicle]
		        ,[CF].[idCategory]
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
PRINT 'Create FUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle]';
GO


CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle](
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
			AND DU.Environment IN (SELECT DSWEnvironment FROM [dbo].[CategoryFascicles] CF
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