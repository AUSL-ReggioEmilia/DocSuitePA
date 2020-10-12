/****************************************************************************************************************************************
* Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)  *
* <DBProtocollo, varcahr(255), DSProtocollo>  --> Settare il nome del DB di protocollo.                  *
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
PRINT 'Versionamento database alla 8.87'
GO

EXEC dbo.VersioningDatabase N'8.87',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT 'DROP FUNCTION [dbo].[IsSQL2012Compatible]'
GO
DROP FUNCTION [dbo].[IsSQL2012Compatible]
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
PRINT 'CREATE FUNCTION [dbo].[CurrentDatabaseVersion]'
GO

CREATE FUNCTION [dbo].[CurrentDatabaseVersion]()
RETURNS TINYINT
AS
BEGIN
    DECLARE @CurrentVersion AS TINYINT
    SELECT @CurrentVersion = compatibility_level
    FROM sys.databases
    WHERE name='<DBProtocollo, varchar(255), DBProtocollo>'
    return @CurrentVersion
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
PRINT 'CREATE FUNCTION [webapiprivate].[Role_FX_FindRoles]'
GO

IF ([dbo].[CurrentDatabaseVersion]()) >= 140
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Role_FX_FindRoles]
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
        R.UniqueId <> ''00000000-0000-0000-0000-000000000000'' AND
        R.isActive = 1 AND 
        (@Name IS NULL OR (@Name IS NOT NULL AND R.Name like ''%''+@Name+''%'')) AND 
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
                    (@Environment = 1 AND RG.ProtocolRights <>''00000000000000000000'') OR
                    (@Environment = 2 AND RG.ResolutionRights <>''00000000000000000000'') OR
                    ((@Environment = 4 OR @Environment>= 100) AND RG.DocumentSeriesRights <>''00000000000000000000'')))))
    ),
    AllParentIds AS (
        SELECT DISTINCT [Value]
        FROM SplitString((SELECT STRING_AGG(CAST(REPLACE(A.FullIncrementalPath,''|''+CAST(A.IdRole as NVARCHAR(6)),'''') AS [nvarchar](MAX)),''|'') 
                         FROM Results A
                         WHERE @LoadAlsoParent IS NOT NULL AND @LoadAlsoParent = 1),''|'')
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
)'		
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Role_FX_FindRoles]
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
        R.UniqueId <> ''00000000-0000-0000-0000-000000000000'' AND
        R.isActive = 1 AND 
        (@Name IS NULL OR (@Name IS NOT NULL AND R.Name like ''%''+@Name+''%'')) AND 
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
                    (@Environment = 1 AND RG.ProtocolRights <>''00000000000000000000'') OR
                    (@Environment = 2 AND RG.ResolutionRights <>''00000000000000000000'') OR
                    ((@Environment = 4 OR @Environment>= 100) AND RG.DocumentSeriesRights <>''00000000000000000000'')))))
    ),
    AllParentIds AS (
        SELECT DISTINCT [Value]
        FROM SplitString(STUFF(REPLACE((SELECT ''#!'' + LTRIM(RTRIM(REPLACE(A.FullIncrementalPath,''|''+CAST(A.IdRole as NVARCHAR(6)),''''))) AS ''data()'' 
                         FROM Results A
                         WHERE @LoadAlsoParent IS NOT NULL AND @LoadAlsoParent = 1
                         FOR XML PATH('''')),'' #!'',''|''), 1, 2, ''''),''|'')
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
)'
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
PRINT N'CREATE FUNCTION [webapiprivate].[Processes_FX_FindProcesses]'
GO
CREATE FUNCTION [webapiprivate].[Processes_FX_FindProcesses]
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
PRINT N'CREATE FUNCTION [webapiprivate].[DossierFolders_FX_FindProcessFolders]'
GO
CREATE FUNCTION [webapiprivate].[DossierFolders_FX_FindProcessFolders]
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
PRINT N'ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_CanBeFascicolable]';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_CanBeFascicolable](
	@FascicleIdCategory smallint,
	@FascicleEnvironment int,
	@FascicleType smallint,
	@IdDocumentUnit uniqueidentifier,
	@Environment int,
	@CategoryId smallint
)
RETURNS BIT
AS
BEGIN
	DECLARE @CanBeFascicolable BIT
	DECLARE @List NVARCHAR(MAX)
	SELECT @List = FullIncrementalPath FROM Category WHERE  IdCategory = @CategoryId;
	
	WITH   
	SplitedValues AS
		(SELECT [Number], [Value] = LTRIM(RTRIM(SUBSTRING(@List, [Number], CHARINDEX('|', @List + '|', [Number]) - [Number])))
		 FROM (SELECT Number = ROW_NUMBER() OVER (ORDER BY name)
			   FROM sys.all_objects) AS x
			   WHERE Number <= LEN(@List) AND SUBSTRING('|' + @List, [Number], LEN('|')) = '|'
		 ),
	 AllCategories AS
		 (SELECT C.Value AS IdCategory, C.Number, CF.DSWEnvironment, CF.FascicleType
		  FROM SplitedValues C
		  INNER JOIN CategoryFascicles AS CF ON C.Value = CF.IdCategory
		 ),
	 Categories AS
		 (SELECT A.IdCategory, A.Number, A.DSWEnvironment, A.FascicleType
		  FROM AllCategories A
		  WHERE A.Number >= (SELECT MAX(I.Number) FROM AllCategories I WHERE I.FascicleType = 1)
		 )

	SELECT @CanBeFascicolable = cast(count(1) as bit)
	FROM Categories C
	WHERE NOT EXISTS (SELECT 1 FROM FascicleDocumentUnits WHERE IdDocumentUnit = @IdDocumentUnit) AND @FascicleIdCategory IN (SELECT IdCategory FROM Categories) AND 
		  (
			   (@FascicleType = 1 AND C.FascicleType in (0,1) AND NOT EXISTS ( SELECT 1 FROM Categories CI WHERE CI.DSWEnvironment = @Environment AND CI.IdCategory = @CategoryId AND CI.FascicleType = 2)) 
			OR (@FascicleType = 2 AND C.FascicleType = 2 AND C.DSWEnvironment = @Environment AND @Environment = @FascicleEnvironment AND C.IdCategory = @CategoryId)
		  )

	RETURN @CanBeFascicolable
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
PRINT N'ALTER FUNCTION [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]';
GO

ALTER FUNCTION [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@Environment integer,
	@AnyEnvironment tinyint,
	@DocumentRequired tinyint,
    @ShowOnlyNoInstanceWorkflows tinyint
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
	LEFT OUTER JOIN [dbo].[WorkflowRoles] WRR ON WR.IdWorkflowRepository = WRR.IdWorkflowRepository	
	WHERE (WRR.IdWorkflowRepository IS NULL 
            OR (
                EXISTS (SELECT TOP 1 RG.IdRole
                        FROM dbo.RoleGroup RG
                        INNER JOIN Role R on RG.idRole = R.idRole
                        WHERE R.idRole = WRR.IdRole 
                        AND EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = RG.IdGroup)
                        AND ((WR.DSWEnvironment = 0)
                            OR (WR.DSWEnvironment = 1 AND RG.ProtocolRights like '1%')
                            OR (WR.DSWEnvironment = 2 AND RG.ResolutionRights like '1%')
                            OR ((WR.DSWEnvironment = 4 OR WR.DSWEnvironment = 8 OR WR.DSWEnvironment >= 100) AND RG.DocumentSeriesRights like '1%')
                            )
                        )
                )
        ) 
        AND WR.Status = 1 
        AND (
                ((@DocumentRequired IS NULL OR @DocumentRequired = 0) AND NOT EXISTS (SELECT 1 
                                                                                  FROM WorkflowEvaluationProperties WFP 
                                                                                  WHERE WFP.IdWorkflowRepository = WR.IdWorkflowRepository 
                                                                                        AND WFP.Name = '_dsw_v_Fascicle_DocumentRequired' AND WFP.ValueBoolean = 1)
                )  
             OR (@DocumentRequired = 1 AND EXISTS (SELECT 1
                                                   FROM WorkflowEvaluationProperties WFP 
                                                   WHERE WFP.IdWorkflowRepository = WR.IdWorkflowRepository 
                                                         AND WFP.Name = '_dsw_v_Fascicle_DocumentRequired' AND WFP.ValueBoolean = 1)
                )
            ) 
         AND (
                ((@ShowOnlyNoInstanceWorkflows IS NULL OR @ShowOnlyNoInstanceWorkflows = 0) AND NOT EXISTS (SELECT 1 
                                                                                  FROM WorkflowEvaluationProperties WFP 
                                                                                  WHERE WFP.IdWorkflowRepository = WR.IdWorkflowRepository 
                                                                                        AND WFP.Name = '_dsw_v_Workflow_NoInstanceRequired' AND WFP.ValueBoolean = 1)
                )  
             OR (@ShowOnlyNoInstanceWorkflows = 1 AND EXISTS (SELECT 1
                                                   FROM WorkflowEvaluationProperties WFP 
                                                   WHERE WFP.IdWorkflowRepository = WR.IdWorkflowRepository 
                                                         AND WFP.Name = '_dsw_v_Workflow_NoInstanceRequired' AND WFP.ValueBoolean = 1)
                )
            )             
        AND ((WR.DSWEnvironment = @Environment) OR ( (WR.DSWEnvironment = 0 AND @AnyEnvironment = 1)))

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
PRINT N'ALTER FUNCTION [dbo].[UserSecurityGroups]';
GO

ALTER FUNCTION [dbo].[UserSecurityGroups](
	@UserName nvarchar(255),
	@Domain nvarchar(255)
)
RETURNS TABLE
AS
RETURN
(
	SELECT SG.IdGroup 
	FROM [dbo].[SecurityGroups] SG 
	INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
	GROUP BY SG.IdGroup
	UNION 
	SELECT IdGroup
	FROM [dbo].[SecurityGroups] SG
	WHERE SG.AllUsers = 1
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
PRINT N'CREATE NONCLUSTERED INDEX [IX_SecurityGroups_AllUsers]';
GO

CREATE NONCLUSTERED INDEX [IX_SecurityGroups_AllUsers]
ON [dbo].[SecurityGroups] ([AllUsers])

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
PRINT N'CREATE NONCLUSTERED INDEX [IX_SecurityUsers_[UserDomain]';
GO

CREATE NONCLUSTERED INDEX [IX_SecurityUsers_[UserDomain]
ON [dbo].[SecurityUsers] ([UserDomain])
INCLUDE ([idGroup],[Account])

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
PRINT N'DROP FUNCTION [webapiprivate].[Contact_FX_GetContacParents]';
GO

DROP FUNCTION [webapiprivate].[Contact_FX_GetContacParents]
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
PRINT N'CREATE FUNCTION [webapiprivate].[Contact_FX_GetContactParents]';
GO

CREATE FUNCTION [webapiprivate].[Contact_FX_GetContactParents]
(	
	@IdContact int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT V.[Index], C.[Incremental] As IdContact,C.[IncrementalFather] AS ContactParent_IdContact,C.[IdContactType] AS ContactType,REPLACE(C.[Description],'|',' ') AS Description,C.[Code]
	  ,C.[EMailAddress] AS Email,C.[CertifydMail] AS CertifiedMail,C.[Note],[C].[RegistrationDate],[C].[FiscalCode]
	FROM [Contact] C
	INNER JOIN (SELECT CAST(ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS int) AS [Index], [Value] FROM SplitString(
		(
			SELECT FullIncrementalPath FROM Contact WHERE Incremental = @IdContact), '|'
		)) AS V ON V.[Value] = C.Incremental
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