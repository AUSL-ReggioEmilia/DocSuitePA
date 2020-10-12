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
PRINT 'Versionamento database alla 9.03'
GO

EXEC dbo.VersioningDatabase N'9.03',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
	SELECT  T.IdTenant AS IdTenantModel, T.IdTenantAOO, T.TenantTypology, T.TenantName, T.CompanyName, T.StartDate, T.EndDate, T.Note, T.RegistrationUser, T.RegistrationDate, T.LastChangedDate, T.LastChangedUser, T.Timestamp
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
PRINT N'ALTER FUNCTION [webapiprivate].[Role_FX_FindRoles]';
GO

ALTER FUNCTION [webapiprivate].[Role_FX_FindRoles]
(   
    @UserName NVARCHAR(256), 
    @Domain NVARCHAR(256),
    @Name NVARCHAR(256),
    @ParentId SMALLINT,
    @ServiceCode NVARCHAR(256),
    @TenantId UNIQUEIDENTIFIER,
    @Environment integer,
    @LoadOnlyRoot BIT,
    @LoadOnlyMy BIT,
    @LoadAlsoParent BIT
)
RETURNS TABLE 
AS
RETURN 
(
    WITH MySecurityGroups AS (
            SELECT IdGroup
            FROM   [dbo].[UserSecurityGroups](@UserName, @Domain)
    ),
    Results AS (
        SELECT
        R.[idRole] AS IdRole
        ,R.[IdRoleTenant]
        ,R.[UniqueId]
        ,R.[idRoleFather] AS RoleParent_IdRole
        ,R.[Name]
        ,R.[isActive] AS IsActive
        ,R.[isChanged]
        ,R.[TenantId]
        ,R.[FullIncrementalPath]
        ,R.[EMailAddress]
        ,R.[UriSharepoint]
        ,R.[ServiceCode]
        ,R.[Collapsed]
        ,R.[ActiveFrom]
        ,R.[ActiveTo]
        ,R.[RegistrationUser]
        ,R.[RegistrationDate]
        ,R.[LastChangedUser]
        ,R.[LastChangedDate]
        ,(
                SELECT TOP 1 CAST(1 AS BIT) 
                FROM Role R_Children 
                WHERE R_Children.idRoleFather = R.idRole 
        ) AS HasChildren
        ,CAST(1 AS BIT) AS IsRealResult 
    FROM
        [Role] AS R
    WHERE
        R.UniqueId <> '00000000-0000-0000-0000-000000000000' AND
		R.TenantId <> '00000000-0000-0000-0000-000000000000' AND
        R.isActive = 1 AND 
        (@Name IS NULL OR (@Name IS NOT NULL AND R.Name like '%'+@Name+'%')) AND 
        (@ParentId IS NULL OR (@ParentId IS NOT NULL AND R.idRoleFather = @ParentId)) AND
        (@ServiceCode IS NULL OR (@ServiceCode IS NOT NULL AND R.ServiceCode = @ServiceCode)) AND
        (@TenantId IS NULL OR (@TenantId IS NOT NULL AND EXISTS (SELECT TOP 1 1 FROM TenantRoles TR WHERE TR.IdTenant = @TenantId AND TR.EntityShortId = R.IdRole))) AND
        (@LoadOnlyRoot IS NULL OR @LoadOnlyRoot = 0 OR 
            (@LoadOnlyRoot IS NOT NULL AND @LoadOnlyRoot = 1 AND R.idRoleFather IS NULL)) AND
        (@LoadOnlyMy IS NULL OR @LoadOnlyMy = 0 OR 
            (@LoadOnlyMy IS NOT NULL AND @LoadOnlyMy = 1 AND EXISTS (SELECT TOP 1 1 
                FROM RoleGroup RG
                INNER JOIN MySecurityGroups MSG ON RG.idGroup = MSG.IdGroup
                WHERE RG.idRole = R.idRole AND (@Environment = 0 OR 
                    (@Environment = 1 AND RG.ProtocolRights <>'00000000000000000000') OR
                    (@Environment = 2 AND RG.ResolutionRights <>'00000000000000000000') OR
                    ((@Environment = 4 OR @Environment>= 100) AND RG.DocumentSeriesRights <>'00000000000000000000')))))
    ),
    AllParentIds AS (
        SELECT DISTINCT [Value]
        FROM SplitString(STUFF(REPLACE((SELECT '#!' + LTRIM(RTRIM(REPLACE(A.FullIncrementalPath,'|'+CAST(A.IdRole as NVARCHAR(6)),''))) AS 'data()' 
                         FROM Results A
                         WHERE @LoadAlsoParent IS NOT NULL AND @LoadAlsoParent = 1
                         FOR XML PATH('')),' #!','|'), 1, 2, ''),'|')
    )
    SELECT [IdRole]
        ,[IdRoleTenant]
        ,[UniqueId]
        ,RoleParent_IdRole
        ,[Name]
        ,IsActive
        ,[isChanged]
        ,[TenantId]
        ,[FullIncrementalPath]
        ,[EMailAddress]
        ,[UriSharepoint]
        ,[ServiceCode]
        ,[Collapsed]
        ,[ActiveFrom]
        ,[ActiveTo]
        ,[RegistrationUser]
        ,[RegistrationDate]
        ,[LastChangedUser]
        ,[LastChangedDate]
        ,[HasChildren]
        ,[IsRealResult] 
    FROM Results
    UNION 
    SELECT R.[idRole] AS IdRole
        ,R.[IdRoleTenant]
        ,R.[UniqueId]
        ,R.[idRoleFather] AS RoleParent_IdRole
        ,R.[Name]
        ,R.[isActive] AS IsActive
        ,R.[isChanged]
        ,R.[TenantId]
        ,R.[FullIncrementalPath]
        ,R.[EMailAddress]
        ,R.[UriSharepoint]
        ,R.[ServiceCode]
        ,R.[Collapsed]
        ,R.[ActiveFrom]
        ,R.[ActiveTo]
        ,R.[RegistrationUser]
        ,R.[RegistrationDate]
        ,R.[LastChangedUser]
        ,R.[LastChangedDate]
        ,CAST(1 AS BIT) AS HasChildren
        ,CAST(0 AS BIT) AS IsRealResult 
    FROM
        [Role] AS R
        WHERE R.idRole IN (SELECT * FROM AllParentIds) AND NOT R.idRole IN (SELECT idRole FROM Results)
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
PRINT N'ALTER PROCEDURE [dbo].[DossierFolder_Insert]';
GO

ALTER PROCEDURE [dbo].[DossierFolder_Insert] 
	@IdDossierFolder uniqueidentifier, 
	@IdDossier uniqueidentifier,
	@IdFascicle uniqueidentifier, 
	@IdCategory smallint,
	@Name nvarchar(256), 
    @Status smallint,
	@JsonMetadata nvarchar(max), 
	@RegistrationDate datetimeoffset(7),
    @RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256), 
	@ParentInsertId uniqueidentifier
	AS
	
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	
	
	BEGIN TRY
		BEGIN TRANSACTION InsertDossierFolder
		-- Recupero il parent node
        SELECT @parentNode = [DossierFolderNode] FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @ParentInsertId

        IF @parentNode IS NULL
            BEGIN
                    SELECT @parentNode = MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() AND [IdDossier] = @IdDossier
            END
        
        IF @parentNode IS NULL
            BEGIN
                    SET @parentNode = hierarchyid::GetRoot()
                    SET @IdDossierFolder = @IdDossier 
            END

		-- Recupero il max node in base al parent node
		SELECT @maxNode = MAX([DossierFolderNode]) FROM [dbo].[DossierFolders] WHERE [DossierFolderNode].GetAncestor(1) = @parentNode;
		SET @node = @parentNode.GetDescendant(@maxNode, NULL)	
		
		INSERT INTO [dbo].[DossierFolders]([DossierFolderNode],[IdDossierFolder],[IdDossier],[IdFascicle],[IdCategory],[Name],[Status],[JsonMetadata],
		    [RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
		VALUES (@node, @IdDossierFolder, @IdDossier, @IdFascicle, @IdCategory, @Name, @Status, @JsonMetadata, @RegistrationDate, @RegistrationUser, NULL, NULL)

		COMMIT TRANSACTION InsertDossierFolder

		SELECT [DossierFolderNode],[IdDossierFolder] AS UniqueId,[IdDossier],[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[JsonMetadata],[DossierFolderPath],[DossierFolderLevel],[DossierFolderParentNode],[ParentInsertId],[Timestamp] 
		FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder

	END TRY
	
	BEGIN CATCH 
		ROLLBACK TRANSACTION InsertDossierFolder
		
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
PRINT N'ALTER PROCEDURE [dbo].[FascicleFolder_Insert] ';
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
       
       
	   DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid
       
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

		    -- Recupero il max node in base al parent node
		    SELECT @maxNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @parentNode;
            SET @node = @parentNode.GetDescendant(@maxNode, NULL)   

            INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
                [RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
            VALUES (@node, @IdFascicleFolder, @IdFascicle, @IdCategory, @Name, @Status, @Typology, @RegistrationDate, @RegistrationUser, NULL, NULL)
                    
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
          OR (
			DU.Title LIKE '____/%' + REPLACE(@Number, '|', '/')
			OR DU.Title LIKE '%' + REPLACE(@Number, '|', '/') + '%/____'
			OR DU.Title LIKE '%' + REPLACE(@Number, '|', '/') + '%/%/____'
			OR DU.Title LIKE '____/%/' + REPLACE(@Number, '|', '/')
		  )
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
PRINT N'CREATE FUNCTION [dbo].[Collaboration_FX_CollaborationsDeletationSigning]';
GO
CREATE FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsDeletationSigning](
	@Signers string_list_tbltype READONLY,
	@IsRequired bit)
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate, Collaboration.TemplateName,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate, C_CS.IsAbsent as CollaborationSign_IsAbsent,
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
				and CC_CS.SignUser IN (SELECT val FROM @Signers)
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
PRINT N'CREATE FUNCTION [webapiprivate].[UserDomain_FX_UserRights]';
GO
CREATE FUNCTION [webapiprivate].[UserDomain_FX_UserRights]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
    @RoleGroupPECRightEnabled bit
)
RETURNS TABLE
AS
RETURN
(
	WITH 
	
	MySecurityGroups AS (
		SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
	),
    MyContainerGroups AS (
		SELECT CG.Rights,CG.DeskRights,CG.DocumentRights,CG.DocumentSeriesRights,CG.FascicleRights,CG.ResolutionRights,CG.UDSRights
        FROM [dbo].[Container] C
        INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
        WHERE EXISTS (SELECT 1 from MySecurityGroups MSG where CG.IdGroup = MSG.IdGroup) AND C.isActive = 1
        GROUP BY CG.Rights,CG.DeskRights,CG.DocumentRights,CG.DocumentSeriesRights,CG.FascicleRights,CG.ResolutionRights,CG.UDSRights
	),
    MyRoleGroups AS (
		SELECT RG.DocumentRights,RG.DocumentSeriesRights,RG.FascicleRights,RG.ResolutionRights,RG.ProtocolRights
        FROM [dbo].[Role] R
        INNER JOIN [dbo].[RoleGroup] RG on RG.idRole = R.idRole
        WHERE EXISTS (SELECT 1 from MySecurityGroups MSG WHERE RG.IdGroup = MSG.IdGroup) AND R.isActive = 1
		AND (R.UniqueId <> '00000000-0000-0000-0000-000000000000' AND R.TenantId <> '00000000-0000-0000-0000-000000000000')
        GROUP BY RG.DocumentRights,RG.DocumentSeriesRights,RG.FascicleRights,RG.ResolutionRights,RG.ProtocolRights
	),
    MyRoleUsers AS (
		SELECT [type] 
        FROM [dbo].[RoleUser] 
        WHERE Account = @Domain+'\'+@UserName and [Enabled] = 1
        GROUP BY [type]
	)
   
   SELECT A.Environment,
		   CAST(MAX(A.HasInsertable) AS BIT) AS HasInsertable,
		   CAST(MAX(A.HasViewable) AS BIT) AS HasViewable,
		   CAST(null AS BIT) AS HasSignerRole,
		   CAST(null AS BIT) AS HasSecretaryRole,
		   CAST(null AS BIT) AS HasFascicleResponsibleRole,
		   CAST(null AS BIT) AS HasFascicleSecretaryRole,
		   CAST(null AS BIT) AS HasManagerRole
	FROM (SELECT CAST(1 AS int) AS Environment,
		         CAST(CASE WHEN Rights like '1%' THEN 1 ELSE 0 END AS smallint) AS HasInsertable,
		         CAST(CASE WHEN Rights like '__1%' THEN 1 ELSE 0 END AS smallint) AS HasViewable
	    FROM MyContainerGroups
        WHERE Rights <> '00000000000000000000') A
    GROUP BY A.Environment
    UNION ALL
    SELECT A.Environment,
		   CAST(MAX(A.HasInsertable) AS BIT) AS HasInsertable,
		   CAST(MAX(A.HasViewable) AS BIT) AS HasViewable,
		   CAST(null AS BIT) AS HasSignerRole,
		   CAST(null AS BIT) AS HasSecretaryRole,
		   CAST(null AS BIT) AS HasFascicleResponsibleRole,
		   CAST(null AS BIT) AS HasFascicleSecretaryRole,
		   CAST(null AS BIT) AS HasManagerRole
	FROM ( SELECT CAST(2 AS int) AS Environment,
                  CAST(CASE WHEN ResolutionRights like '1%' THEN 1 ELSE 0 END AS smallint) AS HasInsertable,
                  CAST(CASE WHEN ResolutionRights like '__1%' THEN 1 ELSE 0 END AS smallint) AS HasViewable
            FROM MyContainerGroups
            WHERE ResolutionRights <> '00000000000000000000') A
    GROUP BY A.Environment
    UNION ALL
    SELECT A.Environment,
		   CAST(MAX(A.HasInsertable) AS BIT) AS HasInsertable,
		   CAST(MAX(A.HasViewable) AS BIT) AS HasViewable,
		   CAST(null AS BIT) AS HasSignerRole,
		   CAST(null AS BIT) AS HasSecretaryRole,
		   CAST(null AS BIT) AS HasFascicleResponsibleRole,
		   CAST(null AS BIT) AS HasFascicleSecretaryRole,
		   CAST(null AS BIT) AS HasManagerRole
	FROM (SELECT CAST(4 AS int) AS Environment,
            CAST(CASE WHEN DocumentSeriesRights like '1%' THEN 1 ELSE 0 END AS smallint) AS HasInsertable,
            CAST(CASE WHEN DocumentSeriesRights like '__1%' THEN 1 ELSE 0 END AS smallint) AS HasViewable
        FROM MyContainerGroups
        WHERE DocumentSeriesRights <> '00000000000000000000') A
    GROUP BY A.Environment
    UNION ALL
    SELECT A.Environment,
		   CAST(MAX(A.HasInsertable) AS BIT) AS HasInsertable,
		   CAST(null AS BIT) AS HasViewable,
		   CAST(null AS BIT) AS HasSignerRole,
		   CAST(null AS BIT) AS HasSecretaryRole,
		   CAST(null AS BIT) AS HasFascicleResponsibleRole,
		   CAST(null AS BIT) AS HasFascicleSecretaryRole,
		   CAST(null AS BIT) AS HasManagerRole
	FROM (SELECT CAST(5 AS int) AS Environment,
                 CAST(CASE WHEN DeskRights like '1%' THEN 1 ELSE 0 END AS smallint) AS HasInsertable
          FROM MyContainerGroups
          WHERE DeskRights <> '00000000000000000000') A
    GROUP BY A.Environment
    UNION ALL
    SELECT A.Environment,
		   CAST(MAX(A.HasInsertable) AS BIT) AS HasInsertable,
		   CAST(MAX(A.HasViewable) AS BIT) AS HasViewable,
		   CAST(null AS BIT) AS HasSignerRole,
		   CAST(null AS BIT) AS HasSecretaryRole,
		   CAST(null AS BIT) AS HasFascicleResponsibleRole,
		   CAST(null AS BIT) AS HasFascicleSecretaryRole,
		   CAST(null AS BIT) AS HasManagerRole
	FROM (SELECT CAST(7 AS int) AS Environment,
                CAST(CASE WHEN UDSRights like '1%' THEN 1 ELSE 0 END AS smallint) AS HasInsertable,
                CAST(CASE WHEN UDSRights like '__1%' THEN 1 ELSE 0 END AS smallint) AS HasViewable
            FROM MyContainerGroups
            WHERE UDSRights <> '00000000000000000000') A
    GROUP BY A.Environment
    UNION ALL
    SELECT A.Environment,
		   CAST(null AS BIT) AS HasInsertable,
		   CAST(null AS BIT) AS HasViewable,
		   CAST(null AS BIT) AS HasSignerRole,
		   CAST(null AS BIT) AS HasSecretaryRole,
		   CAST(MAX(A.HasFascicleResponsibleRole) AS BIT) AS HasFascicleResponsibleRole,
		   CAST(MAX(A.HasFascicleSecretaryRole) AS BIT) AS HasFascicleSecretaryRole,
		   CAST(null AS BIT) AS HasManagerRole
	FROM (SELECT CAST(8 AS int) AS Environment,
                 CAST(CASE WHEN EXISTS (select 1 from MyRoleUsers where [Type] = 'RP') THEN 1 ELSE 0 END AS smallint) AS HasFascicleResponsibleRole,
                 CAST(CASE WHEN EXISTS (select 1 from MyRoleUsers where [Type] = 'SP') THEN 1 ELSE 0 END AS smallint) AS HasFascicleSecretaryRole
          FROM MyRoleUsers) A
    GROUP BY A.Environment
    UNION ALL
    SELECT A.Environment,
		   CAST(MAX(A.HasInsertable) AS BIT) AS HasInsertable,
		   CAST(MAX(A.HasViewable) AS BIT) AS HasViewable,
		   CAST(null AS BIT) AS HasSignerRole,
		   CAST(null AS BIT) AS HasSecretaryRole,
		   CAST(null AS BIT) AS HasFascicleResponsibleRole,
		   CAST(null AS BIT) AS HasFascicleSecretaryRole,
		   CAST(null AS BIT) AS HasManagerRole
	FROM (SELECT CAST(9 AS int) AS Environment,
                 CAST(CASE WHEN DocumentRights like '1%' THEN 1 ELSE 0 END AS smallint) AS HasInsertable,
                 CAST(CASE WHEN DocumentRights like '__1%' THEN 1 ELSE 0 END AS smallint) AS HasViewable
           FROM MyContainerGroups
           WHERE DocumentRights <> '00000000000000000000') A
    GROUP BY A.Environment
    UNION ALL
    SELECT A.Environment,
		   CAST(MAX(A.HasInsertable) AS BIT) AS HasInsertable,
		   CAST(MAX(A.HasViewable) AS BIT) AS HasViewable,
		   CAST(null AS BIT) AS HasSignerRole,
		   CAST(null AS BIT) AS HasSecretaryRole,
		   CAST(null AS BIT) AS HasFascicleResponsibleRole,
		   CAST(null AS BIT) AS HasFascicleSecretaryRole,
		   CAST(null AS BIT) AS HasManagerRole
	FROM (SELECT CAST(11 AS int) AS Environment,
                 CAST(CASE WHEN @RoleGroupPECRightEnabled =1 
                            THEN CASE WHEN ProtocolRights like '__1%' THEN 1 ELSE 0 END 
                            ELSE CASE WHEN ProtocolRights like '1%' THEN 1 ELSE 0 END 
                      END AS smallint) AS HasInsertable,
                 CAST(CASE WHEN @RoleGroupPECRightEnabled =1 
                            THEN CASE WHEN ProtocolRights like '__1%' THEN 1 ELSE 0 END 
                            ELSE CASE WHEN ProtocolRights like '1%' THEN 1 ELSE 0 END 
                      END AS smallint) AS HasViewable
            FROM MyRoleGroups
            WHERE ProtocolRights <> '00000000000000000000') A
    GROUP BY A.Environment 
    UNION ALL
    SELECT A.Environment,
		   CAST(1 AS BIT) AS HasInsertable,
		   CAST(null AS BIT) AS HasViewable,
		   CAST(MAX(A.HasSignerRole) AS BIT) AS HasSignerRole,
		   CAST(MAX(A.HasSecretaryRole) AS BIT) AS HasSecretaryRole,
		   CAST(null AS BIT) AS HasFascicleResponsibleRole,
		   CAST(null AS BIT) AS HasFascicleSecretaryRole,
		   CAST(null AS BIT) AS HasManagerRole
	FROM (SELECT CAST(12 AS int) AS Environment,
                CAST(CASE WHEN EXISTS (select 1 from MyRoleUsers where [Type] IN ('D', 'V')) THEN 1 ELSE 0 END AS smallint) AS HasSignerRole,
                CAST(CASE WHEN EXISTS (select 1 from MyRoleUsers where [Type] = 'S') THEN 1 ELSE 0 END AS smallint) AS HasSecretaryRole
            FROM MyRoleUsers) A
    GROUP BY A.Environment 
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