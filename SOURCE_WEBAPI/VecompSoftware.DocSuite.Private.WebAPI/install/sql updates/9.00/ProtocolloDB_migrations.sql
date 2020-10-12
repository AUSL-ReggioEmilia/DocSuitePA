/****************************************************************************************************************************************
* Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)  *
* <DBProtocollo, varchar(255), DSProtocollo>  --> Settare il nome del DB di protocollo.                  *
* <DBPratiche, varchar(255), DSPratiche>  --> Se esiste il DB di Pratiche settare il nome.              *
* <DBAtti, varchar(255), DSAtti>      --> Se esiste il DB di Atti settare il nome.            *
* <EXIST_DB_ATTI, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva  *
* <EXIST_DB_PRATICHE, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
* <EXIST_DB_PROTOCOLLO, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
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
PRINT 'Versionamento database alla 9.00'
GO

EXEC dbo.VersioningDatabase N'9.00',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT 'ALTER SQL Function [dbo].[Tenants_FX_UserTenants]'
GO

ALTER FUNCTION [webapiprivate].[Tenants_FX_UserTenants]
(​
	@UserName nvarchar(256), 
	@Domain nvarchar(256)
​)
RETURNS TABLE
AS
RETURN
(
	WITH
	MySecurityGroups AS (
		SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
	)
​
	SELECT  T.IdTenant AS IdTenantModel, T.IdTenantAOO, T.TenantName, T.CompanyName, T.StartDate, T.EndDate, T.Note, T.RegistrationUser, T.RegistrationDate, T.LastChangedDate, T.LastChangedUser, T.Timestamp
	FROM Tenants T​
	WHERE EXISTS (SELECT 1
		FROM [dbo].[ContainerGroup] CG​
		INNER JOIN [dbo].[TenantContainers] TC ON TC.EntityShortId = CG.idContainer ​
		WHERE EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
		AND TC.IdTenant = T.IdTenant)  ​
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
PRINT 'ALTER SQL Function [dbo].[DocumentUnit_FX_FascicolableDocumentUnits]'
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_FascicolableDocumentUnits]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset,
	@IncludeThreshold bit,
	@ThresholdFrom datetimeoffset,
	@ExcludeLinked bit,
	@IdTenantAOO uniqueidentifier
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
           TAOO.[IdTenantAOO] as TenantAOO_IdTenantAOO,
		   TAOO.[Name] as TenantAOO_Name,
		   CT.idCategory AS Category_IdCategory,
		   CT.Name AS Category_Name,
		   C.idContainer AS Container_IdContainer,
		   C.Name AS Container_Name
	FROM [cqrs].[DocumentUnits] DU
	INNER JOIN [dbo].[TenantAOO] TAOO on TAOO.IdTenantAOO = DU.IdTenantAOO
	INNER JOIN [dbo].[Category] CT on DU.idCategory = CT.idCategory
	INNER JOIN [dbo].[Container] C on DU.idContainer = C.idContainer
	INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
	LEFT OUTER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
	
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DU.IdDocumentUnit = DUR.IdDocumentUnit
	LEFT OUTER JOIN [dbo].[Role] RL on DUR.UniqueIdRole = RL.UniqueId
	LEFT OUTER JOIN [dbo].[RoleGroup] RG on RL.idRole = RG.idRole
	LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup

	WHERE DU.IdTenantAOO = @IdTenantAOO AND ( (@IncludeThreshold = 0 AND DU.RegistrationDate BETWEEN @DateFrom AND @DateTo) OR
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
		   TAOO.[IdTenantAOO],
		   TAOO.[Name],
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
PRINT 'ALTER SQL Function [dbo].[DocumentUnit_FX_FascicleDocumentUnits]'
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]
(
    @FascicleId uniqueidentifier,
	@IdFascicleFolder uniqueidentifier,
	@IdTenantAOO uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(    
    WITH FascicleDocumentUnits AS
	(
		SELECT FP.IdDocumentUnit, FP.ReferenceType
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
      ,TAOO.[IdTenantAOO] as TenantAOO_IdTenantAOO
      ,TAOO.[Name] as TenantAOO_Name
      ,DU.[RegistrationUser]
      ,DU.[RegistrationDate]
	  ,FDU.ReferenceType
	  ,DU.IdUDSRepository
	  ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
	FROM cqrs.DocumentUnits DU
	INNER JOIN FascicleDocumentUnits FDU ON FDU.IdDocumentUnit = DU.IdDocumentUnit
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	INNER JOIN [dbo].[TenantAOO] TAOO on TAOO.IdTenantAOO = DU.IdTenantAOO
	WHERE DU.IdTenantAOO = @IdTenantAOO
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
	  ,TAOO.[IdTenantAOO] as TenantAOO_IdTenantAOO
      ,TAOO.[Name] as TenantAOO_Name
      ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
       FROM cqrs.DocumentUnits DU
       INNER JOIN [dbo].FascicleDocumentUnits FDU ON FDU.IdDocumentUnit = DU.IdDocumentUnit
       INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
       INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	   INNER JOIN [dbo].[TenantAOO] TAOO on TAOO.IdTenantAOO = DU.IdTenantAOO
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
PRINT N'ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle]'
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
	@IncludeChildClassification bit,
    @IdTenantAOO uniqueidentifier
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
		
	WHERE   DU.IdTenantAOO = @IdTenantAOO AND (@Year IS NULL OR DU.Year = @Year) 
			 	AND (@Number IS NULL OR DU.Title like  '____/%' +  REPLACE(@Number, '|', '/'))
				AND (@DocumentUnitName IS NULL OR DU.DocumentUnitName = @DocumentUnitName OR (@DocumentUnitName = 'Archivio' AND Environment > 99) OR (@DocumentUnitName = 'Serie documentale' AND Environment = 4))
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
PRINT 'ALTER SQL Function [dbo].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle]'
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle] (
  @UserName NVARCHAR (256), 
  @Domain NVARCHAR (256), 
  @FascicleId UNIQUEIDENTIFIER, 
  @Year SMALLINT, 
  @Number NVARCHAR (256), 
  @DocumentUnitName NVARCHAR (256), 
  @CategoryId SMALLINT, 
  @ContainerId SMALLINT, 
  @Subject NVARCHAR (256), 
  @IncludeChildClassification BIT, 
  @Skip INT, 
  @Top INT,
  @IdTenantAOO UNIQUEIDENTIFIER
) RETURNS TABLE AS RETURN WITH MySecurityGroups AS (
  SELECT 
    IdGroup 
  FROM 
    [dbo].[UserSecurityGroups](@UserName, @Domain)
), 
MyCategory AS (
  SELECT 
    TOP 1 C.IdCategory 
  FROM 
    [dbo].[Category] AS C 
    INNER JOIN [dbo].[Fascicles] AS F ON F.IdCategory = C.IdCategory 
  WHERE 
    F.IdFascicle = @FascicleId 
  GROUP BY 
    C.IdCategory
), 
MyCategoryFascicles AS (
  SELECT 
    CF.IdCategory 
  FROM 
    [dbo].[CategoryFascicles] AS CF 
    INNER JOIN [dbo].[Category] AS C ON C.idCategory = CF.IdCategory 
  WHERE 
    (
      EXISTS (
        SELECT 
          MyCategory.IdCategory 
        FROM 
          MyCategory 
        WHERE 
          CF.IdCategory = MyCategory.IdCategory 
          AND CF.FascicleType = 1
      )
    ) 
    OR (
      EXISTS (
        SELECT 
          MyCategory.IdCategory 
        FROM 
          MyCategory 
        WHERE 
          MyCategory.IdCategory IN (
            SELECT 
              Value 
            FROM 
              [dbo].[SplitString](C.FullIncrementalPath, '|')
          ) 
          AND CF.FascicleType = 0
      )
    ) 
  GROUP BY 
    CF.IdCategory
), 
CategoryChildren AS (
  SELECT 
    CC.IdCategory 
  FROM 
    [dbo].Category AS CC 
  WHERE 
    (
      @IncludeChildClassification = 0 
      AND CC.IdCategory = @CategoryId
    ) 
    OR (
      @IncludeChildClassification = 1 
      AND (
        CC.FullIncrementalPath LIKE '%|' + CONVERT (
          VARCHAR (10), 
          @CategoryId
        ) + '|%' 
        OR CC.IdCategory = @CategoryId
      )
    ) 
  GROUP BY 
    CC.IdCategory
), 
MydocumentUnits AS (
  SELECT 
    T.IdDocumentUnit, 
    T.rownum 
  FROM 
    (
      SELECT 
        DU.[IdDocumentUnit], 
        row_number() OVER (
          ORDER BY 
            DU.[IdDocumentUnit]
        ) AS rownum 
      FROM 
        cqrs.DocumentUnits AS DU 
        INNER JOIN [dbo].[Container] AS C ON DU.IdContainer = C.IdContainer 
        INNER JOIN [dbo].[Category] AS CT ON DU.IdCategory = CT.IdCategory 
        LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] AS DUR ON DUR.IdDocumentUnit = DU.IdDocumentUnit 
      WHERE 
		DU.IdTenantAOO = @IdTenantAOO
        AND (
          @Year IS NULL 
          OR DU.Year = @Year
        ) 
        AND (
          @Number IS NULL 
          OR DU.Title LIKE '____/%' + REPLACE(@Number, '|', '/')
        ) 
        AND (
          @DocumentUnitName IS NULL 
          OR DU.DocumentUnitName = @DocumentUnitName 
          OR (
            @DocumentUnitName = 'Archivio' 
            AND Environment > 99
          ) 
          OR (
            @DocumentUnitName = 'Serie documentale' 
            AND Environment = 4
          )
        ) 
        AND (
          @CategoryId IS NULL 
          OR EXISTS (
            SELECT 
              TOP 1 CC.IdCategory 
            FROM 
              CategoryChildren AS CC 
            WHERE 
              DU.IdCategory = CC.IdCategory
          )
        ) 
        AND (
          @ContainerId IS NULL 
          OR DU.IdContainer = @ContainerId
        ) 
        AND (
          @Subject IS NULL 
          OR DU.Subject LIKE '%' + @Subject + '%'
        ) 
        AND (
          (
            EXISTS (
              SELECT 
                TOP 1 IdFascicle 
              FROM 
                [dbo].[Fascicles] 
              WHERE 
                IdFascicle = @FascicleId 
                AND FascicleType IN (4, 1)
            ) 
            AND (
              EXISTS (
                SELECT 
                  TOP 1 CG.idContainerGroup 
                FROM 
                  [dbo].[ContainerGroup] AS CG 
                WHERE 
                  CG.IdContainer = DU.IdContainer 
                  AND EXISTS (
                    SELECT 
                      1 
                    FROM 
                      MySecurityGroups AS SG 
                    WHERE 
                      SG.IdGroup = CG.IdGroup
                  ) 
                  AND (
                    (
                      DU.Environment = 1 
                      AND (CG.Rights LIKE '__1%')
                    ) 
                    OR (
                      DU.Environment = 2 
                      AND (CG.ResolutionRights LIKE '__1%')
                    ) 
                    OR (
                      DU.Environment = 3 
                      AND (
                        CG.DocumentSeriesRights LIKE '__1%'
                      )
                    ) 
                    OR (
                      (
                        DU.Environment = 7 
                        OR DU.Environment > 99
                      ) 
                      AND (CG.UDSRights LIKE '__1%')
                    )
                  )
              ) 
              OR EXISTS (
                SELECT 
                  TOP 1 RG.idRole 
                FROM 
                  [dbo].[RoleGroup] AS RG 
                  INNER JOIN Role AS R ON RG.idRole = R.idRole 
                WHERE 
                  R.UniqueId = DUR.UniqueIdRole 
                  AND (
                    (
                      DU.Environment = 1 
                      AND (RG.ProtocolRights LIKE '1%')
                    ) 
                    OR (
                      DU.Environment = 2 
                      AND (RG.ResolutionRights LIKE '1%')
                    ) 
                    OR (
                      DU.Environment = 3 
                      AND (
                        RG.DocumentSeriesRights LIKE '1%'
                      )
                    ) 
                    OR (
                      (
                        DU.Environment = 7 
                        OR DU.Environment > 99
                      ) 
                      AND (
                        RG.DocumentSeriesRights LIKE '1%'
                      )
                    )
                  ) 
                  AND EXISTS (
                    SELECT 
                      1 
                    FROM 
                      MySecurityGroups AS SG 
                    WHERE 
                      SG.IdGroup = RG.IdGroup
                  )
              )
            )
          ) 
          OR (
            NOT EXISTS (
              SELECT 
                TOP 1 IdFascicle 
              FROM 
                [dbo].[Fascicles] 
              WHERE 
                IdFascicle = @FascicleId 
                AND FascicleType IN (4, 1)
            ) 
            AND NOT EXISTS (
              SELECT 
                CF.IdCategoryFascicle 
              FROM 
                [dbo].[CategoryFascicles] AS CF 
                INNER JOIN [dbo].[Fascicles] AS F ON F.IdCategory = CF.IdCategory 
              WHERE 
                CF.FascicleType != 1 
                AND F.IdFascicle = @FascicleId 
                AND CF.DSWEnvironment = DU.Environment
            )
          )
        ) 
        AND (
          DU.IdFascicle IS NULL 
          OR DU.IdFascicle != @FascicleId
        ) 
      GROUP BY 
        DU.[IdDocumentUnit]
    ) AS T 
  WHERE 
    T.rownum > @Skip 
    AND T.rownum <= @Top
) 
SELECT 
  DU.[IdDocumentUnit] AS UniqueId, 
  DU.[IdFascicle], 
  DU.[EntityId], 
  DU.[Year], 
  CAST (DU.[Number] AS VARCHAR) AS Number, 
  DU.[Title], 
  DU.[Subject], 
  DU.[DocumentUnitName], 
  DU.[Environment], 
  DU.[RegistrationUser], 
  DU.[RegistrationDate], 
  DU.[IdUDSRepository], 
  TAOO.[IdTenantAOO] as TenantAOO_IdTenantAOO,
  TAOO.[Name] as TenantAOO_Name,
  CT.idCategory AS Category_IdCategory, 
  CT.Name AS Category_Name, 
  C.idContainer AS Container_IdContainer, 
  C.Name AS Container_Name, 
  (
    SELECT 
      CAST (
        COUNT(1) AS BIT
      ) 
    FROM 
      MyCategoryFascicles 
    WHERE 
      MyCategoryFascicles.IdCategory = CT.IdCategory
  ) AS IsFascicolable 
FROM 
  cqrs.DocumentUnits AS DU 
  INNER JOIN [dbo].[Container] AS C ON DU.IdContainer = C.IdContainer 
  INNER JOIN [dbo].[Category] AS CT ON DU.IdCategory = CT.IdCategory 
  INNER JOIN [dbo].[TenantAOO] AS TAOO ON TAOO.IdTenantAOO = DU.IdTenantAOO
WHERE 
  EXISTS (
    SELECT 
      MydocumentUnits.[IdDocumentUnit] 
    FROM 
      MydocumentUnits 
    WHERE 
      DU.[IdDocumentUnit] = MydocumentUnits.IdDocumentUnit
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
PRINT 'ALTER SQL Function [dbo].[DocumentUnit_FX_AllowedDocumentUnits]'
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_AllowedDocumentUnits] 
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset,
	@IdTenantAOO uniqueidentifier
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
      ,TAOO.[IdTenantAOO] as TenantAOO_IdTenantAOO
      ,TAOO.[Name] as TenantAOO_Name
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
	INNER JOIN [dbo].[TenantAOO] TAOO on TAOO.IdTenantAOO = DU.IdTenantAOO
	WHERE DU.IdTenantAOO = @IdTenantAOO AND DU.RegistrationDate BETWEEN @DateFrom AND @DateTo 
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Protocol_FX_AuthorizedProtocols]';
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
	  ,P.IdCategoryAPI
	  ,P.idContainer
      ,P.RegistrationDate As RegistrationDate
      ,P.DocumentCode As DocumentCode
      ,P.idStatus As IdStatus
      ,P.[Object]
	  ,P.idType As IdType
	  ,P.idDocument As IdDocument	 
	  ,P.UniqueId AS UniqueId
	  ,P.IdTenantAOO AS IdTenantAOO
	FROM [dbo].[Protocol] P
	INNER JOIN [dbo].[ProtocolRole] PR on P.Year = PR.Year AND P.Number = PR.Number
	INNER JOIN [dbo].[Role] R on PR.idRole = R.idRole 
	INNER JOIN [dbo].[RoleGroup] RG on R.idRole = RG.idRole
	WHERE 
	P.RegistrationDate between @DateFrom AND @DateTo 
	AND (RG.ProtocolRights like '1%') 
	AND EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = RG.IdGroup)
GROUP BY P.Year, P.Number, P.IdCategoryAPI, P.idContainer, P.RegistrationDate, P.DocumentCode, P.idStatus, P.[Object], P.idType, P.idDocument, P.UniqueId, P.IdTenantAOO)

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
	  ,TAOO.[IdTenantAOO] as TenantAOO_IdTenantAOO
      ,TAOO.[Name] as TenantAOO_Name
FROM ProtocolTableValued P
		INNER JOIN [dbo].[Type] T on P.idType = T.idType
		INNER JOIN [dbo].[Category] CT on P.IdCategoryAPI = CT.idCategory
		INNER JOIN [dbo].[TenantAOO] TAOO on TAOO.IdTenantAOO = P.IdTenantAOO
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
PRINT N'Create procedure [dbo].[Parameter_Insert]';
GO

CREATE PROCEDURE [dbo].[Parameter_Insert] 
    @UniqueId uniqueidentifier,
	@Incremental int,
	@LastUsedYear smallint, 
    @LastUsedNumber int, 
    @Locked bit, 
    @Password nvarchar(256),
    @LastChangedUser nvarchar(256),
    @LastChangedDate datetimeoffset(7),
    @LastUsedidCategory smallint,
    @LastUsedidRecipient smallint,
    @LastUsedidContainer smallint,
    @Version smallint,
    @LastUsedidDistributionList smallint,
    @DomainName nvarchar(256),
    @AlternativePassword nvarchar(256),
    @ServiceField nvarchar(256),
    @LastUsedidRole smallint,
    @LastUsedIdRoleUser smallint,
    @LastUsedidResolution int,
    @LastUsedResolutionYear smallint,
    @LastUsedResolutionNumber smallint,
    @LastUsedBillNumber smallint,
    @LastUsedYearReg smallint,
    @LastUsedNumberReg int,
    @RegistrationDate datetimeoffset(7), 
    @RegistrationUser nvarchar(256), 
    @IdTenantAOO uniqueidentifier
AS

	DECLARE @EntityId INT, @LastUsedIncremental INT

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION ParameterInsert

	SELECT top(1) @LastUsedIncremental = Incremental FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Parameter] ORDER BY Incremental DESC
	IF(@LastUsedIncremental is null)
	BEGIN
		SET @LastUsedIncremental = 0
	END

	SET @EntityId = @LastUsedIncremental + 1

	INSERT INTO  <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Parameter] ([Incremental],[LastUsedYear],[LastUsedNumber]
           ,[Locked],[Password],[LastChangedUser],[LastChangedDate],[LastUsedidCategory],[LastUsedidRecipient],[LastUsedidContainer]
           ,[Version],[LastUsedidDistributionList],[DomainName],[AlternativePassword],[ServiceField],[LastUsedidRole],[LastUsedIdRoleUser]
           ,[LastUsedidResolution],[LastUsedResolutionYear],[LastUsedResolutionNumber],[LastUsedBillNumber],[LastUsedYearReg],[LastUsedNumberReg]
           ,[UniqueId],[RegistrationDate],[RegistrationUser],[IdTenantAOO])
       VALUES (@EntityId, @LastUsedYear, @LastUsedNumber, @Locked, @Password,@LastChangedUser,@LastChangedDate,@LastUsedidCategory,@LastUsedidRecipient
               ,@LastUsedidContainer,@Version,@LastUsedidDistributionList,@DomainName,@AlternativePassword,@ServiceField,@LastUsedidRole
               ,@LastUsedIdRoleUser,@LastUsedidResolution,@LastUsedResolutionYear,@LastUsedResolutionNumber,@LastUsedBillNumber,@LastUsedYearReg,@LastUsedNumberReg,
	           @UniqueId,@RegistrationDate, @RegistrationUser, @IdTenantAOO)

	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN	 
		INSERT INTO <DBAtti, varchar(255), DBAtti>.[dbo].[Parameter] ([Incremental],[LastUsedYear],[LastUsedNumber]
           ,[Locked],[Password],[LastChangedUser],[LastChangedDate],[LastUsedidCategory],[LastUsedidRecipient],[LastUsedidContainer]
           ,[Version],[LastUsedidDistributionList],[DomainName],[AlternativePassword],[ServiceField],[LastUsedidRole],[LastUsedIdRoleUser]
           ,[LastUsedidResolution],[LastUsedResolutionYear],[LastUsedResolutionNumber],[LastUsedBillNumber],[LastUsedYearReg],[LastUsedNumberReg]
           ,[UniqueId],[RegistrationDate],[RegistrationUser],[IdTenantAOO])
       VALUES (@EntityId, @LastUsedYear, @LastUsedNumber, @Locked, @Password,@LastChangedUser,@LastChangedDate,@LastUsedidCategory,@LastUsedidRecipient
               ,@LastUsedidContainer,@Version,@LastUsedidDistributionList,@DomainName,@AlternativePassword,@ServiceField,@LastUsedidRole
               ,@LastUsedIdRoleUser,@LastUsedidResolution,@LastUsedResolutionYear,@LastUsedResolutionNumber,@LastUsedBillNumber,@LastUsedYearReg,@LastUsedNumberReg,
	           @UniqueId,@RegistrationDate, @RegistrationUser, @IdTenantAOO)
		END
	
	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 		
		BEGIN	 
		INSERT INTO <DBPratiche, varchar(255), DBPratiche>.[dbo].[Parameter] ([Incremental],[LastUsedYear],[LastUsedNumber]
           ,[Locked],[Password],[LastChangedUser],[LastChangedDate],[LastUsedidCategory],[LastUsedidRecipient],[LastUsedidContainer]
           ,[Version],[LastUsedidDistributionList],[DomainName],[AlternativePassword],[ServiceField],[LastUsedidRole],[LastUsedIdRoleUser]
           ,[LastUsedidResolution],[LastUsedResolutionYear],[LastUsedResolutionNumber],[LastUsedBillNumber],[LastUsedYearReg],[LastUsedNumberReg]
           ,[UniqueId],[RegistrationDate],[RegistrationUser],[IdTenantAOO])
       VALUES (@EntityId, @LastUsedYear, @LastUsedNumber, @Locked, @Password,@LastChangedUser,@LastChangedDate,@LastUsedidCategory,@LastUsedidRecipient
               ,@LastUsedidContainer,@Version,@LastUsedidDistributionList,@DomainName,@AlternativePassword,@ServiceField,@LastUsedidRole
               ,@LastUsedIdRoleUser,@LastUsedidResolution,@LastUsedResolutionYear,@LastUsedResolutionNumber,@LastUsedBillNumber,@LastUsedYearReg,@LastUsedNumberReg,
	           @UniqueId,@RegistrationDate, @RegistrationUser, @IdTenantAOO)
		END

	COMMIT TRANSACTION ParameterInsert

	SELECT @UniqueId as UniqueId, [Timestamp]
    FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Parameter]
    WHERE Incremental = @EntityId
																	   
	END TRY

	BEGIN CATCH 
		 ROLLBACK TRANSACTION ParameterInsert

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
PRINT N'Create procedure [dbo].[Parameter_Update]';
GO

CREATE PROCEDURE [dbo].[Parameter_Update] 
    @UniqueId uniqueidentifier,
    @Incremental int,
	@LastUsedYear smallint, 
    @LastUsedNumber int, 
    @Locked bit, 
    @Password nvarchar(256),
    @LastChangedUser nvarchar(256),
    @LastChangedDate datetimeoffset(7),
    @LastUsedidCategory smallint,
    @LastUsedidRecipient smallint,
    @LastUsedidContainer smallint,
    @Version smallint,
    @LastUsedidDistributionList smallint,
    @DomainName nvarchar(256),
    @AlternativePassword nvarchar(256),
    @ServiceField nvarchar(256),
    @LastUsedidRole smallint,
    @LastUsedIdRoleUser smallint,
    @LastUsedidResolution int,
    @LastUsedResolutionYear smallint,
    @LastUsedResolutionNumber smallint,
    @LastUsedBillNumber smallint,
    @LastUsedYearReg smallint,
    @LastUsedNumberReg int,
    @RegistrationDate datetimeoffset(7), 
    @RegistrationUser nvarchar(256), 
    @IdTenantAOO uniqueidentifier,
	@Timestamp_Original timestamp
AS

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION ParameterUpdate
	BEGIN TRY	

	UPDATE  <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Parameter] SET
        [LastUsedYear] = @LastUsedYear, [LastUsedNumber] = @LastUsedNumber, [Locked] = @Locked, [Password] = @Password, [LastChangedUser] = @LastChangedUser,
        [LastChangedDate] = @LastChangedDate, [LastUsedidCategory] = @LastUsedidCategory, [LastUsedidRecipient] = @LastUsedidRecipient, [LastUsedidContainer] = @LastUsedidContainer,
        [Version] = @Version, [LastUsedidDistributionList] = @LastUsedidDistributionList, [DomainName] = @DomainName, [AlternativePassword] = @AlternativePassword,
        [ServiceField] = @ServiceField, [LastUsedidRole] = @LastUsedidRole, [LastUsedIdRoleUser] = @LastUsedIdRoleUser,[LastUsedidResolution] = @LastUsedidResolution,
        [LastUsedResolutionYear] = LastUsedResolutionYear, [LastUsedResolutionNumber] = @LastUsedResolutionNumber, [LastUsedBillNumber] = @LastUsedBillNumber,
        [LastUsedYearReg] = @LastUsedYearReg, [LastUsedNumberReg] = @LastUsedNumberReg
    WHERE [Incremental] = @Incremental

	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN	 
            UPDATE <DBAtti, varchar(255), DBAtti>.[dbo].[Parameter] SET
                [LastUsedYear] = @LastUsedYear, [LastUsedNumber] = @LastUsedNumber, [Locked] = @Locked, [Password] = @Password, [LastChangedUser] = @LastChangedUser,
                [LastChangedDate] = @LastChangedDate, [LastUsedidCategory] = @LastUsedidCategory, [LastUsedidRecipient] = @LastUsedidRecipient, [LastUsedidContainer] = @LastUsedidContainer,
                [Version] = @Version, [LastUsedidDistributionList] = @LastUsedidDistributionList, [DomainName] = @DomainName, [AlternativePassword] = @AlternativePassword,
                [ServiceField] = @ServiceField, [LastUsedidRole] = @LastUsedidRole, [LastUsedIdRoleUser] = @LastUsedIdRoleUser,[LastUsedidResolution] = @LastUsedidResolution,
                [LastUsedResolutionYear] = LastUsedResolutionYear, [LastUsedResolutionNumber] = @LastUsedResolutionNumber, [LastUsedBillNumber] = @LastUsedBillNumber,
                [LastUsedYearReg] = @LastUsedYearReg, [LastUsedNumberReg] = @LastUsedNumberReg
            WHERE [Incremental] = @Incremental
        END
	
	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 		
		BEGIN	 
            UPDATE <DBPratiche, varchar(255), DBPratiche>.[dbo].[Parameter] SET
                [LastUsedYear] = @LastUsedYear, [LastUsedNumber] = @LastUsedNumber, [Locked] = @Locked, [Password] = @Password, [LastChangedUser] = @LastChangedUser,
                [LastChangedDate] = @LastChangedDate, [LastUsedidCategory] = @LastUsedidCategory, [LastUsedidRecipient] = @LastUsedidRecipient, [LastUsedidContainer] = @LastUsedidContainer,
                [Version] = @Version, [LastUsedidDistributionList] = @LastUsedidDistributionList, [DomainName] = @DomainName, [AlternativePassword] = @AlternativePassword,
                [ServiceField] = @ServiceField, [LastUsedidRole] = @LastUsedidRole, [LastUsedIdRoleUser] = @LastUsedIdRoleUser,[LastUsedidResolution] = @LastUsedidResolution,
                [LastUsedResolutionYear] = LastUsedResolutionYear, [LastUsedResolutionNumber] = @LastUsedResolutionNumber, [LastUsedBillNumber] = @LastUsedBillNumber,
                [LastUsedYearReg] = @LastUsedYearReg, [LastUsedNumberReg] = @LastUsedNumberReg
            WHERE [Incremental] = @Incremental
        END

	SELECT [Incremental],[LastUsedYear],[LastUsedNumber],[Locked],[Password],[LastChangedUser],[LastChangedDate],[LastUsedidCategory],[LastUsedidRecipient]
      ,[LastUsedidContainer],[Version],[LastUsedidDistributionList],[DomainName],[AlternativePassword],[ServiceField],[LastUsedidRole],[LastUsedIdRoleUser]
      ,[LastUsedidResolution],[LastUsedResolutionYear],[LastUsedResolutionNumber],[LastUsedBillNumber],[LastUsedYearReg],[LastUsedNumberReg],[UniqueId]
      ,[RegistrationDate],[RegistrationUser],[Timestamp],[IdTenantAOO]
	FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Parameter]
	WHERE [UniqueId] = @UniqueId

	COMMIT TRANSACTION ParameterUpdate																	   
	END TRY

	BEGIN CATCH 
		 ROLLBACK TRANSACTION ParameterUpdate

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
PRINT N'Create procedure [dbo].[Parameter_Delete]';
GO

CREATE PROCEDURE [dbo].[Parameter_Delete] 
    @UniqueId uniqueidentifier, 
    @IdTenantAOO uniqueidentifier,
	@Timestamp_Original timestamp
AS

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION ParameterDelete

	DELETE FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Parameter] 
    WHERE [UniqueId] = @UniqueId

	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN	 
            DELETE FROM <DBAtti, varchar(255), DBAtti>.[dbo].[Parameter]
            WHERE [UniqueId] = @UniqueId
        END
	
	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 			
		BEGIN	 
            DELETE FROM <DBPratiche, varchar(255), DBPratiche>.[dbo].[Parameter]
            WHERE [UniqueId] = @UniqueId
        END

	COMMIT TRANSACTION ParameterDelete																	   
	END TRY

	BEGIN CATCH 
		 ROLLBACK TRANSACTION ParameterDelete

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
PRINT N'Alter procedure [dbo].[Category_Insert]';
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
	   @IdMetadataRepository uniqueidentifier,
       @IdTenantAOO uniqueidentifier
AS 

	DECLARE @EntityShortId smallint, @LastUsedIdCategory smallint, @ParentFullCode nvarchar(255)
	SELECT @LastUsedIdCategory = [LastUsedIdCategory] FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Parameter]
	SET @EntityShortId = @LastUsedIdCategory + 1
	SET @FullIncrementalPath = @EntityShortId
    SET @FullCode =  RIGHT('0000' + cast(@Code as nvarchar(4)), 4)
	IF (@Code = 0)
	BEGIN
		SET @FullCode = ''
	END

	IF(@idParent is not null)
		BEGIN
		SET @ParentFullCode = (SELECT TOP 1 FullCode FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Category] where idCategory = @idParent)
		SET @FullIncrementalPath = (SELECT TOP 1 FullIncrementalPath FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Category] where idCategory = @idParent) + '|' + cast(@EntityShortId as nvarchar(256))
		SET @FullCode = @ParentFullCode + '|' + @FullCode
		END
	

    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION CategoryInsert
	
	--Inserimento classificatore in db Protocollo
	UPDATE <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
	INSERT INTO <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
             [LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository], [IdTenantAOO]) 
             
    VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate, 
		   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository, @IdTenantAOO)
    
	SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Category]

	IF( (CAST('True'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
	    --Inserimento classificatore in db Atti
       	UPDATE <DBAtti, varchar(255), DBAtti>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
		INSERT INTO <DBAtti, varchar(255), DBAtti>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
				[LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository], [IdTenantAOO]) 
             
		VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate, 
			   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository, @IdTenantAOO)

		SELECT [FullSearchComputed] FROM <DBAtti, varchar(255), DBAtti>.[dbo].[Category]
    END

	IF( (CAST('True'  AS BIT) = CAST('True' AS BIT)))
    BEGIN 
        --Inserimento classificatore in db Pratiche
       	UPDATE <DBPratiche, varchar(255), DBPratiche>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
		INSERT INTO <DBPratiche, varchar(255), DBPratiche>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
				[LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository], [IdTenantAOO]) 
             
		VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate,  
			   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository, @IdTenantAOO)

		SELECT [FullSearchComputed] FROM <DBPratiche, varchar(255), DBPratiche>.[dbo].[Category]
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
PRINT N'Alter procedure [dbo].[Category_Update]';
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
	@IdMetadataRepository uniqueidentifier,
    @IdTenantAOO uniqueidentifier
AS 

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE 	
	BEGIN TRY
	BEGIN TRANSACTION CategoryUpdate	

	--Aggiornamento classificatore in Protocollo
	UPDATE <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Category] SET [Name] = @Name, 
		[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
		[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
		[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
		[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository, [IdTenantAOO] = @IdTenantAOO Where [idCategory] = @idCategory
    SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Category]

	IF( (CAST('True'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		--Aggiornamento classificatore in Atti
		UPDATE <DBAtti, varchar(255), DBAtti>.[dbo].[Category] SET [Name] = @Name, 
			[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
			[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
			[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
			[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository, [IdTenantAOO] = @IdTenantAOO Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBAtti, varchar(255), DBAtti>.[dbo].[Category]
	END

	IF( (CAST('True'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		--Aggiornamento classificatore in Pratiche
		UPDATE <DBPratiche, varchar(255), DBPratiche>.[dbo].[Category] SET [Name] = @Name, 
			[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
			[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
			[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
			[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository, [IdTenantAOO] = @IdTenantAOO Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBPratiche, varchar(255), DBPratiche>.[dbo].[Category]
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
PRINT N'ALTER FUNCTION [webapiprivate].[Category_FX_FindCategories]'
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
	@Container smallint,
    @IdTenantAOO uniqueidentifier
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
        IdTenantAOO = @IdTenantAOO
		AND IsActive = 1 
		AND StartDate <= GETDATE()
        AND Code <> 0 
		AND 
		(
			EndDate IS NULL 
			OR EndDate > GETDATE()
		)
		AND 
		(
            (
                (
                    @LoadRoot = 1 
                    AND 
                    (
                        C.idParent IS NULL
                        OR
                        (
                            C.idParent = 
                            (
                                SELECT TOP 1 idCategory
                                FROM Category CP
                                WHERE CP.IdTenantAOO = @IdTenantAOO
                                AND CP.Code = 0
                            )
                        )
                    )
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
                            OR @FascicleType NOT IN (1,2)
                        ) 
                        OR 
                        (
                            @HasFascicleInsertRights = 1 
                            AND @FascicleType IN (1,2) 
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
			WHEN @FascicleType IS NULL THEN CAST(0 AS BIT) 
			WHEN (@FascicleType = 1 OR @FascicleType = 2) THEN
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
            ELSE CAST(0 AS BIT) 
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
PRINT N'Alter function [Contact_FX_FindContacts]';
GO

ALTER FUNCTION [webapiprivate].[Contact_FX_FindContacts]
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@FullSearchComputed nvarchar(256),
	@ApplyAuthorizations bit,
	@ExcludeRoleContacts bit,
	@ParentId smallint,
	@ParentToExclude smallint,
	@AVCPParentId smallint,
	@FascicleParentId smallint,
	@TenantId uniqueidentifier
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
	AND
	(
	    @TenantId IS NULL OR
		EXISTS(
			SELECT TOP 1 1 
			FROM [TenantContacts] TC
			WHERE C.Incremental = TC.EntityId AND TC.IdTenant = @TenantId
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
PRINT N'ALTER FUNCTION [webapiprivate].[Processes_FX_FindProcesses]';
GO

ALTER FUNCTION [webapiprivate].[Processes_FX_FindProcesses]
(   
    @UserName NVARCHAR(256), 
    @Domain NVARCHAR(256),
    @Name NVARCHAR(256),
    @DossierId UNIQUEIDENTIFIER,
    @CategoryId smallint,
    @LoadOnlyMy BIT
)
RETURNS TABLE 
AS
RETURN 
(
    WITH MySecurityGroups AS (
            SELECT IdGroup
            FROM   [dbo].[UserSecurityGroups](@UserName, @Domain)
    )
    SELECT
       P.[IdProcess] AS UniqueId
      ,P.[IdCategory] AS Category_IdCategory
      ,P.[IdDossier]
      ,P.[Name]
      ,P.[FascicleType]
      ,P.[StartDate]
      ,P.[EndDate]
      ,P.[Note]
      ,P.[RegistrationUser]
      ,P.[RegistrationDate]
      ,P.[LastChangedDate]
      ,P.[LastChangedUser]
      ,P.[Timestamp]
	  ,P.[ProcessType]
	  ,C.[Name] AS Category_Name
	  ,C.[Code] AS Category_Code
      ,(
            SELECT TOP 1 CAST(1 AS BIT) 
            FROM DossierFolders DF  
            WHERE DF.IdDossier = P.IdDossier 
        ) AS HasDossierFolders
FROM
    Processes AS P
	INNER JOIN Category C on P.IdCategory = C.idCategory
WHERE
    (@Name IS NULL OR (@Name IS NOT NULL AND P.Name like '%'+@Name+'%')) AND 
    (@CategoryId IS NULL OR (@CategoryId IS NOT NULL AND P.IdCategory = @CategoryId)) AND
    (@DossierId IS NULL OR (@DossierId IS NOT NULL AND P.IdDossier = @DossierId)) AND
    (@LoadOnlyMy IS NULL OR @LoadOnlyMy = 0 OR 
        (@LoadOnlyMy IS NOT NULL AND @LoadOnlyMy = 1 AND EXISTS (SELECT TOP 1 1 
            FROM ProcessRoles PR 
            INNER JOIN RoleGroup RG ON PR.IdRole = RG.idRole
            INNER JOIN MySecurityGroups MSG ON RG.idGroup = MSG.IdGroup
            WHERE PR.IdProcess = P.IdProcess AND 
            ( RG.ProtocolRights <>'00000000000000000000' OR RG.ResolutionRights <>'00000000000000000000') OR RG.DocumentSeriesRights <>'00000000000000000000')))
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
PRINT N'ALTER FUNCTION [webapiprivate].[DossierFolders_FX_FindProcessFolders]';
GO

ALTER FUNCTION [webapiprivate].[DossierFolders_FX_FindProcessFolders]
(   
    @UserName NVARCHAR(256), 
    @Domain NVARCHAR(256),
    @Name NVARCHAR(256),
    @ProcessId UNIQUEIDENTIFIER,
    @LoadOnlyActive BIT,
    @LoadOnlyMy BIT
)
RETURNS TABLE 
AS
RETURN 
(
    WITH MySecurityGroups AS (
            SELECT IdGroup
            FROM   [dbo].[UserSecurityGroups](@UserName, @Domain)
    )
    SELECT
       DF.[IdDossierFolder]
      ,DF.[IdDossier]
      ,DF.[IdFascicle]
      ,DF.[IdCategory]
      ,DF.[Name]
      ,DF.[Status]
      ,DF.[JsonMetadata]
      ,DF.[RegistrationUser]
      ,DF.[RegistrationDate]
      ,DF.[LastChangedUser]
      ,DF.[LastChangedDate]
      ,DF.[Timestamp]
      ,DF.[DossierFolderNode]
      ,DF.[DossierFolderPath]
      ,DF.[DossierFolderLevel]
      ,DF.[DossierFolderParentNode]
      ,DF.[ParentInsertId]
FROM
    DossierFolders AS DF
    INNER JOIN Processes P ON DF.IdDossier = P.IdDossier 
WHERE
    P.IdProcess = @ProcessId AND
	DF.DossierFolderLevel = 1 AND
    (@Name IS NULL OR (@Name IS NOT NULL AND DF.Name like '%'+@Name+'%')) AND 
    (@LoadOnlyActive IS NULL OR @LoadOnlyActive = 0 OR (@LoadOnlyActive IS NOT NULL AND DF.Status <> 4)) AND 
    (@LoadOnlyMy IS NULL OR @LoadOnlyMy = 0 OR 
        (@LoadOnlyMy IS NOT NULL AND @LoadOnlyMy = 1 AND EXISTS (SELECT TOP 1 1 
            FROM DossierFolderRoles DFR 
            INNER JOIN RoleGroup RG ON DFR.IdRole = RG.idRole
            INNER JOIN MySecurityGroups MSG ON RG.idGroup = MSG.IdGroup
            WHERE DFR.IdDossierFolder = DF.IdDossierFolder AND 
            ( RG.ProtocolRights <>'00000000000000000000' OR RG.ResolutionRights <>'00000000000000000000') OR RG.DocumentSeriesRights <>'00000000000000000000')))
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
PRINT N'ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@idCategory smallint,
	@Name nvarchar(256),
	@HasProcess BIT

)
RETURNS TABLE
AS
RETURN
(
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
left join dbo.DossierFolders DF on DF.IdFascicle = F.IdFascicle
left join dbo.Processes P on  P.IdDossier = DF.IdDossier 
INNER JOIN [dbo].[Category] F_C ON F.idCategory = F_C.idCategory
WHERE (@idCategory = 0 OR  F.IdCategory = @idCategory) 
	    AND ((@Name is NOT null AND ( F.Title like '%'+@Name+'%' OR F.Object like '%'+@Name+'%')) OR (@Name Is null))
		AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, F.IdFascicle)) = 1)
		AND (@HasProcess IS NULL OR (@HasProcess = 0 AND P.IdProcess IS NULL) OR (@HasProcess = 1 AND P.IdProcess IS NOT NULL))
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