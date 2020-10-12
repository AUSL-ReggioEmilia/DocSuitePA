
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
PRINT 'Versionamento database alla 9.01'
GO

EXEC dbo.VersioningDatabase N'9.01',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT 'CREATE PROCEDURE [dbo].[DossierFolder_Clone_Hierarchy] '
GO
CREATE PROCEDURE [dbo].[DossierFolder_Clone_Hierarchy] 
    @IdDossier uniqueidentifier
    AS  
    DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid

    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
    
    BEGIN TRY
        BEGIN TRANSACTION UpdateDossierFolderHierarchy
        
        DECLARE @IdDossierFolder uniqueidentifier;      
        DECLARE @Status smallint;
        DECLARE @Name  nvarchar(256);
        DECLARE @JsonMetadata nvarchar(max);
        
        DECLARE Hierarchy_Cursor CURSOR FAST_FORWARD FORWARD_ONLY FOR 
                SELECT  IdDossierFolder, Status, JsonMetadata, Name FROM [dbo].[DossierFolders]
                WHERE IdDossier = @IdDossier and JsonMetadata is not null
                ORDER BY Name asc;
        
        OPEN Hierarchy_Cursor

        FETCH NEXT FROM Hierarchy_Cursor INTO @IdDossierFolder, @Status, @JsonMetadata, @Name

        WHILE @@FETCH_STATUS = 0
            BEGIN                   
            -- Recupero il parent node
            IF @JsonMetadata <> 'parent'
                BEGIN
                    SELECT @parentNode = [DossierFolderNode] FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @JsonMetadata
                END
            ELSE 
            BEGIN
                IF EXISTS(SELECT TOP 1 * FROM [dbo].[DossierFolders]  
                           WHERE [DossierFolderNode] = hierarchyid::GetRoot())
                BEGIN
                    SET @parentNode = hierarchyid::GetRoot()
                END 
                IF EXISTS(SELECT TOP 1 * FROM [dbo].[DossierFolders]  
                           WHERE [DossierFolderNode] = (select MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() AND @IdDossier is not null and [IdDossier] = @IdDossier))
                BEGIN
                    SET @parentNode = (select MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() AND [IdDossier] = @IdDossier)
                    PRINT @parentNode.ToString()
                END 
            END 
            
            -- Recupero il max node in base al parent node
            SELECT @maxNode = MAX([DossierFolderNode]) FROM [dbo].[DossierFolders] WHERE [DossierFolderNode].GetAncestor(1) = @parentNode;

            IF @JsonMetadata <> 'parent'        
                BEGIN
                    SET @node = @parentNode.GetDescendant(@maxNode, NULL)
                END         
            ELSE 
            BEGIN
                IF EXISTS(SELECT TOP 1 * FROM [dbo].[DossierFolders] WHERE [DossierFolderNode] = hierarchyid::GetRoot() 
                            OR [DossierFolderNode] = (select MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() and @IdDossier is not null and [IdDossier] = @IdDossier))
                BEGIN
                    SET @node = @parentNode.GetDescendant(@maxNode, NULL)
                    PRINT @node.ToString()
                END 
                ELSE
                BEGIN
                    SET @node = hierarchyid::GetRoot()
                    PRINT @node.ToString()
                END
            END
            
            UPDATE [dbo].[DossierFolders] SET [DossierFolderNode]=@node, [Status] = @Status, [JsonMetadata] = NULL, [Name]= right(@Name, len(@Name) - charindex('-', @Name))
            WHERE [IdDossierFolder] = @IdDossierFolder

            SELECT [DossierFolderNode],[IdDossierFolder] AS UniqueId,[IdDossier],[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
            [Name],[JsonMetadata],[DossierFolderPath],[DossierFolderLevel],[DossierFolderParentNode],[ParentInsertId],[Timestamp] 
            FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder

            FETCH NEXT FROM Hierarchy_Cursor INTO @IdDossierFolder, @Status, @JsonMetadata, @Name
        END 
        
        CLOSE Hierarchy_Cursor
        DEALLOCATE Hierarchy_Cursor
        
        
        COMMIT TRANSACTION UpdateDossierFolderHierarchy
    END TRY
    
    BEGIN CATCH 
        ROLLBACK TRANSACTION UpdateDossierFolderHierarchy
        
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
PRINT N'Modifica SQL Function [dbo].[Fascicles_FX_AuthorizedFascicles]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AuthorizedFascicles]
(@UserName NVARCHAR (255), @Domain NVARCHAR (255), @Skip INT, @Top INT, @Year SMALLINT, @StartDateFrom DATETIMEOFFSET, @StartDateTo DATETIMEOFFSET, @EndDateFrom DATETIMEOFFSET, @EndDateTo DATETIMEOFFSET, @FascicleStatus INT, @Manager NVARCHAR (256), @Name NVARCHAR (256), @ViewConfidential BIT, @ViewAccessible BIT, @Subject NVARCHAR (256), @Note NVARCHAR (256), @Rack NVARCHAR (256), @IdMetadataRepository NVARCHAR (256), @MetadataValue NVARCHAR (256), @Classifications NVARCHAR (256), @IncludeChildClassifications BIT, @Roles NVARCHAR (MAX), @Container SMALLINT, @ApplySecurity BIT)
RETURNS TABLE 
AS
RETURN 
    WITH   MySecurityGroups
   AS     (SELECT IdGroup
            FROM   [dbo].[UserSecurityGroups](@UserName, @Domain)),
           MyFascicles
    AS     (SELECT IdFascicle
            FROM   (SELECT   Fascicle.IdFascicle,
                             row_number() OVER (ORDER BY Fascicle.RegistrationDate ASC, Fascicle.Title ASC) AS rownum
                    FROM     dbo.Fascicles AS Fascicle
                    WHERE    (@Year IS NULL
                              OR Fascicle.Year = @Year)
                             AND (@StartDateFrom IS NULL
                                  OR Fascicle.StartDate >= @StartDateFrom)
                             AND (@StartDateTo IS NULL
                                  OR Fascicle.StartDate <= @StartDateTo)
                             AND (@EndDateFrom IS NULL
                                  OR Fascicle.EndDate >= @StartDateFrom)
                             AND (@EndDateTo IS NULL
                                  OR Fascicle.EndDate <= @EndDateTo)
                             AND (@FascicleStatus IS NULL
                                  OR ((@FascicleStatus = 2
                                       AND Fascicle.EndDate IS NOT NULL
                                       AND Fascicle.EndDate <= GETUTCDATE())
                                      OR (@FascicleStatus = 1
                                          AND Fascicle.EndDate IS NULL
                                          AND Fascicle.StartDate <= GETUTCDATE())))
                             AND (@Manager IS NULL
                                  OR EXISTS (SELECT 1
                                             FROM   FascicleContacts AS FC
                                                    INNER JOIN
                                                    Contact AS C
                                                    ON C.Incremental = FC.IdContact
                                             WHERE  C.FullIncrementalPath = @Manager
                                                    AND FC.IdFascicle = Fascicle.IdFascicle))
                             AND (@Name IS NULL
                                  OR Fascicle.Name LIKE '%' + @Name + '%')
                             AND ((@ViewConfidential IS NULL
                                   AND @ViewAccessible IS NULL)
                                  OR ((@ViewConfidential IS NULL
                                       AND @ViewAccessible IS NOT NULL)
                                      AND (Fascicle.VisibilityType = 1))
                                  OR ((@ViewConfidential IS NOT NULL
                                       AND @ViewAccessible IS NULL)
                                      AND (Fascicle.VisibilityType = 0))
                                  OR ((@ViewConfidential IS NOT NULL
                                       AND @ViewAccessible IS NOT NULL)
                                      AND (Fascicle.VisibilityType IN (0, 1))))
                             AND (@Subject IS NULL
                                  OR Fascicle.Object LIKE '%' + @Subject + '%')
                             AND (@Rack IS NULL
                                  OR Fascicle.Rack LIKE '%' + @Rack + '%')
                             AND (@Note IS NULL
                                  OR Fascicle.Note LIKE '%' + @Note + '%')
                             AND (@IdMetadataRepository IS NULL
                                  OR Fascicle.IdMetadataRepository = @IdMetadataRepository)
                             AND (@MetadataValue IS NULL
                                  OR EXISTS (SELECT 1 FROM dbo.SplitString(Fascicle.MetadataValues, ',')
											 WHERE Value like '%"Value":%' and Value like '%'+@MetadataValue+'%'))								  
                             AND (@Classifications IS NULL
                                  OR ((@IncludeChildClassifications = 1
                                       AND EXISTS (SELECT 1
                                                   FROM   Category
                                                   WHERE  IdCategory = Fascicle.IdCategory
                                                          AND FullIncrementalPath LIKE '%' + @Classifications))
                                      OR ((@IncludeChildClassifications IS NULL
                                           OR @IncludeChildClassifications = 0)
                                          AND EXISTS (SELECT 1
                                                      FROM   Category
                                                      WHERE  IdCategory = Fascicle.IdCategory
                                                             AND FullIncrementalPath = @Classifications))))
                             AND (@Roles IS NULL
                                  OR EXISTS (SELECT 1
                                             FROM   FascicleRoles AS FR
                                             WHERE  FR.IdFascicle = Fascicle.IdFascicle
                                                    AND FR.IdRole IN (SELECT CAST ([Value] AS SMALLINT)
                                                                      FROM   dbo.SplitString(@Roles, '|'))))
                             AND (@Container IS NULL
                                  OR Fascicle.IdContainer = @Container)
                             AND ((@ApplySecurity IS NULL
                                   OR @ApplySecurity = 0)
                                  OR (@ApplySecurity = 1
                                      AND (EXISTS (SELECT 1
                                                   FROM   MySecurityGroups AS SG
                                                          INNER JOIN
                                                          [dbo].[RoleGroup] AS RG
                                                          ON SG.IdGroup = RG.IdGroup
                                                          INNER JOIN
                                                          [dbo].[FascicleRoles] AS FR
                                                          ON FR.IdFascicle = Fascicle.IdFascicle
                                                             AND RG.IdRole = FR.IdRole
                                                   WHERE  ((Fascicle.FascicleType = 4
                                                            AND FR.RoleAuthorizationType = 0)
                                                           OR (Fascicle.FascicleType IN (1, 2)
                                                               AND FR.RoleAuthorizationType = 1)
                                                           OR (Fascicle.FascicleType = 2
                                                               AND FR.RoleAuthorizationType = 0
                                                               AND IsMaster = 1)
                                                           OR (Fascicle.FascicleType IN (1, 2)
                                                               AND FR.RoleAuthorizationType = 0
                                                               AND IsMaster = 0)
                                                                                                        OR (Fascicle.IdContainer is not null and Fascicle.FascicleType = 1  AND FR.RoleAuthorizationType = 0 AND IsMaster = 1))
                                                          AND ((RG.ProtocolRights <> '00000000000000000000')
                                                               OR (RG.ResolutionRights <> '00000000000000000000')
                                                               OR (RG.DocumentRights <> '00000000000000000000')
                                                               OR (RG.DocumentSeriesRights <> '00000000000000000000')))
                                           OR EXISTS (SELECT 1
                                                      FROM   [dbo].[RoleUser] AS RU
                                                             INNER JOIN
                                                             [dbo].[CategoryFascicleRights] AS CFR
                                                             ON CFR.IdRole = RU.idRole
                                                             INNER JOIN
                                                             [dbo].[CategoryFascicles] AS CF
                                                             ON CFR.IdCategoryFascicle = CF.IdCategoryFascicle
                                                      WHERE  CF.IdCategory = Fascicle.IdCategory
                                                             AND RU.Account = @Domain + '\' + @UserName
                                                             AND Fascicle.FascicleType = 1
                                                             AND RU.[Type] IN ('RP', 'SP')
                                                             AND (NOT EXISTS (SELECT TOP 1 1
                                                                              FROM   [dbo].[FascicleRoles]
                                                                              WHERE  IdFascicle = Fascicle.IdFascicle
                                                                                     AND RoleAuthorizationType = 0
                                                                                     AND IsMaster = 1)
                                                                  OR (RU.idRole IN ((SELECT idRole
                                                                                     FROM   [dbo].[FascicleRoles]
                                                                                     WHERE  IdFascicle = Fascicle.IdFascicle
                                                                                            AND RoleAuthorizationType = 0
                                                                                            AND IsMaster = 1)))))
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
                    GROUP BY Fascicle.IdFascicle, Fascicle.RegistrationDate, Fascicle.Title) AS T
            WHERE  T.rownum > @Skip
                   AND T.rownum <= @Top)
    SELECT F.IdFascicle AS UniqueId,
           F.Year,
           F.Number,
           F.EndDate,
           F.Title,
           F.Name,
           F.Object AS FascicleObject,
           F.Manager,
           F.IdCategory,
           F.FascicleType,
           F.VisibilityType,
           F.RegistrationUser,
           F.RegistrationDate,
           F_C.IdCategory AS Category_idCategory,
           F_C.Name AS Category_Name,
           FC.IdContact AS Contact_Incremental,
           F_CON.Description AS Contact_Description
    FROM   Fascicles AS F
           INNER JOIN
           MyFascicles AS MF
           ON MF.IdFascicle = F.IdFascicle
           INNER JOIN
           Category AS F_C
           ON F_C.idCategory = F.IdCategory
           LEFT OUTER JOIN
           FascicleContacts AS FC
           ON FC.IdFascicle = F.IdFascicle
           LEFT OUTER JOIN
           Contact AS F_CON
           ON F_CON.Incremental = FC.IdContact
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
PRINT N'Modifica SQL Function [dbo].[Fascicles_FX_CountAuthorizedFascicles]';
GO


ALTER FUNCTION [webapiprivate].[Fascicles_FX_CountAuthorizedFascicles]
(@UserName NVARCHAR (255), @Domain NVARCHAR (255), @Year SMALLINT, @StartDateFrom DATETIMEOFFSET, @StartDateTo DATETIMEOFFSET, @EndDateFrom DATETIMEOFFSET, @EndDateTo DATETIMEOFFSET, @FascicleStatus INT, @Manager NVARCHAR (256), @Name NVARCHAR (256), @ViewConfidential BIT, @ViewAccessible BIT, @Subject NVARCHAR (256), @Note NVARCHAR (256), @Rack NVARCHAR (256), @IdMetadataRepository NVARCHAR (256), @MetadataValue NVARCHAR (256), @Classifications NVARCHAR (256), @IncludeChildClassifications BIT, @Roles NVARCHAR (MAX), @Container SMALLINT, @ApplySecurity BIT)
RETURNS INT
AS
BEGIN
    DECLARE @CountFascicles AS INT;
    WITH   MySecurityGroups
    AS     (SELECT IdGroup
            FROM   [dbo].[UserSecurityGroups](@UserName, @Domain))
    SELECT @CountFascicles = COUNT(DISTINCT Fascicle.IdFascicle)
    FROM   dbo.Fascicles AS Fascicle
    WHERE  (@Year IS NULL
            OR Fascicle.Year = @Year)
           AND (@StartDateFrom IS NULL
                OR Fascicle.StartDate >= @StartDateFrom)
           AND (@StartDateTo IS NULL
                OR Fascicle.StartDate <= @StartDateTo)
           AND (@EndDateFrom IS NULL
                OR Fascicle.EndDate >= @StartDateFrom)
           AND (@EndDateTo IS NULL
                OR Fascicle.EndDate <= @EndDateTo)
           AND (@FascicleStatus IS NULL
                OR ((@FascicleStatus = 2
                     AND Fascicle.EndDate IS NOT NULL
                     AND Fascicle.EndDate <= GETUTCDATE())
                    OR (@FascicleStatus = 1
                        AND Fascicle.EndDate IS NULL
                        AND Fascicle.StartDate <= GETUTCDATE())))
           AND (@Manager IS NULL
                OR EXISTS (SELECT 1
                           FROM   FascicleContacts AS FC
                                  INNER JOIN
                                  Contact AS C
                                  ON C.Incremental = FC.IdContact
                           WHERE  C.FullIncrementalPath = @Manager
                                  AND FC.IdFascicle = Fascicle.IdFascicle))
           AND (@Name IS NULL
                OR Fascicle.Name LIKE '%' + @Name + '%')
           AND ((@ViewConfidential IS NULL
                 AND @ViewAccessible IS NULL)
                OR ((@ViewConfidential IS NULL
                     AND @ViewAccessible IS NOT NULL)
                    AND (Fascicle.VisibilityType = 1))
                OR ((@ViewConfidential IS NOT NULL
                     AND @ViewAccessible IS NULL)
                    AND (Fascicle.VisibilityType = 0))
                OR ((@ViewConfidential IS NOT NULL
                     AND @ViewAccessible IS NOT NULL)
                    AND (Fascicle.VisibilityType IN (0, 1))))
           AND (@Subject IS NULL
                OR Fascicle.Object LIKE '%' + @Subject + '%')
           AND (@Rack IS NULL
                OR Fascicle.Rack LIKE '%' + @Rack + '%')
           AND (@Note IS NULL
                OR Fascicle.Note LIKE '%' + @Note + '%')
           AND (@IdMetadataRepository IS NULL
                OR Fascicle.IdMetadataRepository = @IdMetadataRepository)
           AND (@MetadataValue IS NULL
                 OR EXISTS (SELECT 1 FROM dbo.SplitString(Fascicle.MetadataValues, ',')
											 WHERE Value like '%"Value":%' and Value like '%'+@MetadataValue+'%'))	
           AND (@Classifications IS NULL
                OR ((@IncludeChildClassifications = 1
                     AND EXISTS (SELECT 1
                                 FROM   Category
                                 WHERE  IdCategory = Fascicle.IdCategory
                                        AND FullIncrementalPath LIKE '%' + @Classifications))
                    OR ((@IncludeChildClassifications IS NULL
                         OR @IncludeChildClassifications = 0)
                        AND EXISTS (SELECT 1
                                    FROM   Category
                                    WHERE  IdCategory = Fascicle.IdCategory
                                           AND FullIncrementalPath = @Classifications))))
           AND (@Container IS NULL
                OR Fascicle.IdContainer = @Container)
          AND (@Roles IS NULL
                OR EXISTS (SELECT 1
                           FROM   FascicleRoles AS FR
                           WHERE  FR.IdFascicle = Fascicle.IdFascicle
                                  AND FR.IdRole IN (SELECT CAST ([Value] AS SMALLINT)
                                                    FROM   dbo.SplitString(@Roles, '|'))))
           AND ((@ApplySecurity IS NULL
                 OR @ApplySecurity = 0)
                OR (@ApplySecurity = 1
                    AND (EXISTS (SELECT 1
                                 FROM   MySecurityGroups AS SG
                                        INNER JOIN
                                        [dbo].[RoleGroup] AS RG
                                        ON SG.IdGroup = RG.IdGroup
                                        INNER JOIN
                                        [dbo].[FascicleRoles] AS FR
                                        ON FR.IdFascicle = Fascicle.IdFascicle
                                           AND RG.IdRole = FR.IdRole
                                 WHERE  ((Fascicle.FascicleType = 4
                                          AND FR.RoleAuthorizationType = 0)
                                         OR (Fascicle.FascicleType IN (1, 2)
                                             AND FR.RoleAuthorizationType = 1)
                                         OR (Fascicle.FascicleType = 2
                                             AND FR.RoleAuthorizationType = 0
                                             AND IsMaster = 1)
                                         OR (Fascicle.FascicleType IN (1, 2)
                                             AND FR.RoleAuthorizationType = 0
                                             AND IsMaster = 0)
                                                                   OR (Fascicle.IdContainer is not null and Fascicle.FascicleType = 1  AND FR.RoleAuthorizationType = 0 AND IsMaster = 1))
                                        AND ((RG.ProtocolRights <> '00000000000000000000')
                                             OR (RG.ResolutionRights <> '00000000000000000000')
                                             OR (RG.DocumentRights <> '00000000000000000000')
                                             OR (RG.DocumentSeriesRights <> '00000000000000000000')))
                         OR EXISTS (SELECT 1
                                    FROM   [dbo].[RoleUser] AS RU
                                           INNER JOIN
                                           [dbo].[CategoryFascicleRights] AS CFR
                                           ON CFR.IdRole = RU.idRole
                                           INNER JOIN
                                           [dbo].[CategoryFascicles] AS CF
                                           ON CFR.IdCategoryFascicle = CF.IdCategoryFascicle
                                    WHERE  CF.IdCategory = Fascicle.IdCategory
                                           AND RU.Account = @Domain + '\' + @UserName
                                           AND Fascicle.FascicleType = 1
                                           AND RU.[Type] IN ('RP', 'SP')
                                           AND (NOT EXISTS (SELECT TOP 1 1
                                                            FROM   [dbo].[FascicleRoles]
                                                            WHERE  IdFascicle = Fascicle.IdFascicle
                                                                   AND RoleAuthorizationType = 0
                                                                   AND IsMaster = 1)
                                                OR (RU.idRole IN ((SELECT idRole
                                                                   FROM   [dbo].[FascicleRoles]
                                                                   WHERE  IdFascicle = Fascicle.IdFascicle
                                                                          AND RoleAuthorizationType = 0
                                                                          AND IsMaster = 1)))))
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
    RETURN @CountFascicles;
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
PRINT N'ALTER SQL Function [webapiprivate].[Tenants_FX_UserTenants]';
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
	WHERE EXISTS 
    (
        SELECT 1
		FROM [dbo].[ContainerGroup] CG​
		INNER JOIN [dbo].[TenantContainers] TC ON TC.EntityShortId = CG.idContainer ​
		WHERE EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
		AND TC.IdTenant = T.IdTenant
    )  ​
    OR EXISTS 
    (
        SELECT 1
		FROM [dbo].[RoleGroup] RG​
		INNER JOIN [dbo].[TenantRoles] TR ON TR.EntityShortId = RG.idRole ​
		WHERE EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)
		AND TR.IdTenant = T.IdTenant
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