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
PRINT 'Versionamento database alla 8.81'
GO

EXEC dbo.VersioningDatabase N'8.81',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT N'ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle]';
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
  @Top INT
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
        (
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