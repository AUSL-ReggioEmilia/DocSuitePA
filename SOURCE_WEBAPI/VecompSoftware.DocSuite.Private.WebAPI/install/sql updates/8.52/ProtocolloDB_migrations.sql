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
PRINT 'CREATE FUNCTION [dbo].[GetCategoryFromProtocol]'
GO

CREATE FUNCTION [dbo].[GetCategoryFromProtocol](@year as smallint, @number as int, @protocolIdCategory as smallint)
    RETURNS smallint AS
BEGIN
    DECLARE @idSubCategory smallint
    
    SELECT @idSubCategory = idSubCategory 
    FROM [dbo].[AdvancedProtocol]
    WHERE [Year] = @year and Number = @number
    
    RETURN CASE WHEN isnull(@idSubCategory, 0) = 0 THEN  @protocolIdCategory ELSE @idSubCategory END
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
--#############################################################################
PRINT 'ALTER TABLE Protocol ADD APICategoryId'
GO

ALTER TABLE [dbo].[Protocol] ADD [IdCategoryAPI] AS [dbo].[GetCategoryFromProtocol]([Year], [Number], [idCategory])

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
PRINT 'CREATE FUNCTION [dbo].[GetResolutionFromCollaboration]'
GO

CREATE FUNCTION [dbo].[GetResolutionFromCollaboration](@idResolution as int)
    RETURNS uniqueidentifier AS
BEGIN
    DECLARE @uniqueId uniqueidentifier
    
    SELECT @uniqueId = UniqueId 
    FROM [dbo].[Resolution]
    WHERE idResolution = @idResolution
    
    RETURN @uniqueId
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
--#############################################################################
PRINT 'ALTER TABLE Collaboration ADD [ResolutionUniqueId]'
GO

ALTER TABLE Collaboration ADD [ResolutionUniqueId] AS ([dbo].[GetResolutionFromCollaboration]([idResolution])) 
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
PRINT 'CREATE FUNCTION [dbo].[FascicolableDocumentUnitsSecurityUser]'
GO

CREATE FUNCTION [dbo].[FascicolableDocumentUnitsSecurityUser]
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
    ),

    ProtocolTableValued AS(
        SELECT
          'Protocollo' AS DocumentUnitName
          ,P.[Year] AS [Year]
          ,CAST(right('0000000'+convert(varchar(7), P.Number), 7) AS NVARCHAR(255)) AS Number
          ,NULL AS EntityId
          ,p.IdCategoryAPI AS IdCategory
          ,P.idContainer
          ,P.RegistrationUser AS RegistrationUser
          ,P.RegistrationDate AS RegistrationDate
          ,P.[Object] AS [Subject]
          ,P.UniqueId AS UniqueId 
        FROM [dbo].[Protocol] P
        
        INNER JOIN [dbo].[Container] C on P.IdContainer = C.IdContainer
        INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
        LEFT OUTER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup

        LEFT OUTER JOIN [dbo].[ProtocolRole] PR on P.Year = PR.Year AND P.Number = PR.Number
        LEFT OUTER JOIN [dbo].[Role] R on PR.idRole = R.idRole 
        LEFT OUTER JOIN [dbo].[RoleGroup] RG on R.idRole = RG.idRole
        LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup

        LEFT OUTER JOIN [dbo].[FascicleProtocols] FP on FP.UniqueIdProtocol = P.UniqueId AND ReferenceType = 0
        WHERE ( (@IncludeThreshold = 0 AND P.RegistrationDate BETWEEN @DateFrom AND @DateTo) OR
                ( @IncludeThreshold = 1 AND ( P.RegistrationDate BETWEEN @ThresholdFrom AND CAST(getdate()-60 AS DATE) OR 
                                              P.RegistrationDate BETWEEN @DateFrom AND @DateTo))
               )
               AND ( (C_MSG.IdGroup IS NOT NULL AND CG.Rights like '1%') OR 
                     ( PR.idRole IS NULL OR 
                      (PR.idRole IS NOT NULL AND (RG.ProtocolRights like '1%') AND MSG.IdGroup IS NOT NULL)
                     ) 
                   )
               AND NOT (C_MSG.IdGroup IS NULL AND MSG.IdGroup IS NULL)
               AND FP.IdFascicleProtocol IS NULL
        GROUP BY P.[Year], P.Number, P.IdCategoryAPI, P.idContainer, P.RegistrationUser, P.RegistrationDate, P.[Object], P.UniqueId),

    ResolutionTableValued AS(
        SELECT
            RT.Description AS DocumentUnitName
            ,R.[Year] AS Year
            ,COALESCE(R.ServiceNumber, CAST(R.Number AS nvarchar(255))) AS Number
            ,R.IdResolution AS EntityId
            ,COALESCE(R.idSubCategory, R.idCategory) AS IdCategory
            ,R.idContainer
            ,R.AdoptionUser AS RegistrationUser
            ,R.AdoptionDate AS RegistrationDate
            ,R.[Object] AS [Subject]
            ,R.UniqueId AS UniqueId 
        FROM [dbo].[Resolution] R
        INNER JOIN [dbo].[ResolutionType] RT on R.idType = RT.idType
        
        INNER JOIN [dbo].[Container] C on R.IdContainer = C.IdContainer
        INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
        LEFT OUTER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup

        LEFT OUTER JOIN [dbo].[ResolutionRole] RR on R.idResolution = RR.idResolution
        LEFT OUTER JOIN [dbo].[Role] RL on RR.idRole = RL.idRole 
        LEFT OUTER JOIN [dbo].[RoleGroup] RG on RL.idRole = RG.idRole
        LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup

        LEFT OUTER JOIN [dbo].[FascicleResolutions] FP on FP.UniqueIdResolution = R.UniqueId AND ReferenceType = 0
        WHERE ( (@IncludeThreshold = 0 AND R.AdoptionDate BETWEEN @DateFrom AND @DateTo) OR
                ( @IncludeThreshold = 1 AND ( R.AdoptionDate BETWEEN @ThresholdFrom AND CAST(getdate()-60 AS DATE) OR 
                                              R.AdoptionDate BETWEEN @DateFrom AND @DateTo))
               )
               AND ( (C_MSG.IdGroup IS NOT NULL AND CG.ResolutionRights like '1%') OR 
                     ( RR.idRole IS NULL OR 
                      (RR.idRole IS NOT NULL AND (RG.ResolutionRights like '1%') AND MSG.IdGroup IS NOT NULL)
                     ) 
                   )
               AND NOT (C_MSG.IdGroup IS NULL AND MSG.IdGroup IS NULL)
               AND FP.IdFascicleResolution IS NULL
        GROUP BY RT.Description, R.[Year], COALESCE(R.ServiceNumber,CAST(R.Number AS nvarchar(255))), R.IdResolution, COALESCE(R.idSubCategory, R.idCategory), R.idContainer, R.AdoptionUser, R.AdoptionDate, R.[Object], R.UniqueId)

SELECT UD.*
      ,CT.idCategory AS Category_IdCategory
      ,CT.Name AS Category_Name
      ,C.idContainer AS Container_IdContainer
      ,C.Name AS Container_Name
FROM (SELECT * 
      FROM ProtocolTableValued 
      UNION 
      SELECT * 
      FROM ResolutionTableValued) UD 
INNER JOIN [dbo].[Category] CT on UD.idCategory = CT.idCategory
INNER JOIN [dbo].[Container] C on UD.idContainer = C.idContainer);

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
PRINT 'CREATE FUNCTION [dbo].[AvailableFasciclesFromProtocol]'
GO

CREATE FUNCTION [dbo].[AvailableFasciclesFromProtocol]
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
        WHERE F.IdCategory = P.IdCategoryAPI AND FP.IdFascicleProtocol IS NULL
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

--#############################################################################
PRINT 'CREATE FUNCTION [dbo].[AvailableFasciclesFromResolution]'
GO

CREATE FUNCTION [dbo].[AvailableFasciclesFromResolution]
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
		WHERE F.IdCategory = R.IdCategory AND FR.IdFascicleResolution IS NULL
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

--#############################################################################
PRINT 'CREATE FUNCTION [dbo].[FascicleDocumentUnits]'
GO

CREATE FUNCTION [dbo].[FascicleDocumentUnits]
(
    @FascicleId uniqueidentifier,
	@ConfigurationName nvarchar(256)
)
RETURNS TABLE
AS
RETURN
(
    WITH 

    ProtocolTableValued AS (
        SELECT 
            P.UniqueId,
            NULL as EntityId,
            'Protocollo' as DocumentUnitName,
            P.[Year],
            CAST(right('0000000'+convert(varchar(7), P.Number), 7) AS NVARCHAR(255)) AS Number,
            FP.ReferenceType,
            CAST(P.[Year] AS varchar) + '/' + CAST(right('0000000'+convert(varchar(7), P.Number), 7) AS varchar) as Title,
            P.RegistrationDate,
            P.RegistrationUser,
            P.[Object] as [Subject],
            P.IdCategoryAPI as IdCategory,
            P.IdContainer,
			F.IdFascicle,
			1 as Environment 
		FROM [dbo].[FascicleProtocols] FP
		INNER JOIN [dbo].[Protocol] P on P.UniqueId = FP.UniqueIdProtocol
		LEFT JOIN [dbo].[FascicleProtocols] F on F.UniqueIdProtocol = FP.UniqueIdProtocol AND F.IdFascicleProtocol != FP.IdFascicleProtocol AND F.ReferenceType = 0 AND FP.ReferenceType = 1
		WHERE FP.IdFascicle = @FascicleId),
    
    ResolutionTableValued AS (
        SELECT
            R.Uniqueid as UniqueId,
            R.IdResolution as EntityId,
            TM.[Description] as DocumentUnitName,
            R.[Year],
            COALESCE(R.ServiceNumber, CAST(R.Number as nvarchar(255))) AS Number,
            FR.ReferenceType,
            CAST(R.[Year] AS varchar) + '/' + COALESCE(R.ServiceNumber, CAST(right(+convert(varchar(7), R.Number), 7) AS varchar)) as Title,
            R.AdoptionDate As RegistrationDate,
            R.AdoptionUser As RegistrationUser,
            R.[Object] as [Subject],
            COALESCE(R.idSubCategory, R.idCategory) as IdCategory,
            R.IdContainer,
			F.IdFascicle,
			2 as Environment 
		FROM FascicleResolutions FR
		INNER JOIN Resolution R on R.UniqueId = FR.UniqueIdResolution
		INNER JOIN TabMaster TM on TM.ResolutionType = R.idType AND TM.Configuration = @ConfigurationName
		LEFT JOIN [dbo].[FascicleResolutions] F on F.UniqueIdResolution = FR.UniqueIdResolution AND F.IdFascicleResolution != FR.IdFascicleResolution AND F.ReferenceType = 0 AND FR.ReferenceType = 1
		WHERE FR.IdFascicle = @FascicleId)

    --ArchiveTableValued AS (
    --	SELECT 
    --		NULL as UniqueId,
    --		dsi.Id as EntityId,
    --		'Archivio' as DocumentUnitName,
    --		CAST(dsi.Year AS smallint) as Year,
    --		dsi.Number,
    --		fdsi.ReferenceType,
    --		'' as Title,
    --		dsi.RegistrationDate,
    --		dsi.RegistrationUser,
    --		dsi.Subject,
    --		dsi.IdCategory,
    --		null as IdContainer
    --	FROM FascicleDocumentSeriesItems fdsi
    --	INNER JOIN DocumentSeriesItem dsi on dsi.Id = fdsi.IdDocumentSeriesItem
    --	WHERE fdsi.IdFascicle = @FascicleId)
    

SELECT UD.*
      ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
FROM (SELECT * 
      FROM ProtocolTableValued 
      UNION 
      SELECT * 
      FROM ResolutionTableValued) UD 
INNER JOIN [dbo].[Category] CT on UD.idCategory = CT.idCategory
INNER JOIN [dbo].[Container] C on UD.idContainer = C.idContainer);
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
PRINT 'CREATE FUNCTION [dbo].[FascicolableDocumentUnits]'
GO

CREATE FUNCTION [dbo].[FascicolableDocumentUnits]
(
    @DateFrom datetimeoffset,
    @DateTo datetimeoffset,
    @IncludeThreshold bit,
    @ThresholdFrom datetimeoffset,
    @RoleNames xml,
    @TenantId as uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
    WITH 
    
    MyRolesProtocol AS (
        SELECT R.idRoleTenant as IdRole
        FROM [Role] R
        INNER JOIN [RoleGroup] RG ON R.IdRole = RG.IdRole
        INNER JOIN @RoleNames.nodes('/ArrayOfDomainGroupModel/DomainGroupModel/Name/text()') as x ( y ) on RG.GroupName =  x.y.value('.','varchar(100)')
        WHERE R.TenantId = @TenantId AND RG.ProtocolRights like '1%' and R.IsActive = 1
        GROUP BY R.idRoleTenant
    ),
    MyRolesResolution AS (
        SELECT R.idRoleTenant as IdRole
        FROM [Role] R
        INNER JOIN [RoleGroup] RG ON R.IdRole = RG.IdRole
        INNER JOIN @RoleNames.nodes('/ArrayOfDomainGroupModel/DomainGroupModel/Name/text()') as x ( y ) on RG.GroupName =  x.y.value('.','varchar(100)')
        WHERE R.TenantId = @TenantId AND RG.ResolutionRights like '1%' and R.IsActive = 1
        GROUP BY R.idRoleTenant
    ),

    ProtocolTableValued AS(
    SELECT
          'Protocollo' AS DocumentUnitName
          ,P.[Year] AS Year
		  ,CAST(right('0000000'+convert(varchar(7), P.Number), 7) AS NVARCHAR(255)) AS Number
          ,NULL AS EntityId
          ,p.IdCategoryAPI as IdCategory
          ,P.idContainer
          ,P.RegistrationUser As RegistrationUser
          ,P.RegistrationDate As RegistrationDate
          ,P.[Object] AS [Subject]
          ,P.UniqueId AS UniqueId 
        FROM [dbo].[Protocol] P
        
        INNER JOIN [dbo].[Container] C on P.IdContainer = C.IdContainer
        INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
        LEFT OUTER JOIN @RoleNames.nodes('/ArrayOfDomainGroupModel/DomainGroupModel/Name/text()') as x ( y ) on CG.GroupName =  x.y.value('.','varchar(255)')

        LEFT OUTER JOIN [dbo].[ProtocolRole] PR on P.Year = PR.Year AND P.Number = PR.Number
        LEFT OUTER JOIN MyRolesProtocol R_MSG on PR.IdRole = R_MSG.IdRole

        LEFT OUTER JOIN [dbo].[FascicleProtocols] FP on FP.UniqueIdProtocol = P.UniqueId AND ReferenceType = 0
        WHERE ( (@IncludeThreshold = 0 AND P.RegistrationDate BETWEEN @DateFrom AND @DateTo) OR
                ( @IncludeThreshold = 1 AND ( P.RegistrationDate BETWEEN @ThresholdFrom AND CAST(getdate()-60 AS DATE) OR 
                                              P.RegistrationDate BETWEEN @DateFrom AND @DateTo))
               )
               AND ( (x.y.value('.','varchar(255)') IS NOT NULL AND CG.Rights like '1%') OR
                     (x.y.value('.','varchar(255)') IS NULL AND R_MSG.IdRole IS NOT NULL) OR
                     (x.y.value('.','varchar(255)') IS NOT NULL AND R_MSG.IdRole IS NOT NULL)
                   )
               AND NOT (R_MSG.IdRole IS NULL AND x.y.value('.','varchar(255)') IS NULL)

               AND FP.IdFascicleProtocol IS NULL
        GROUP BY P.[Year], P.Number, P.IdCategoryAPI, P.idContainer, P.RegistrationUser, P.RegistrationDate, P.[Object], P.UniqueId),

    ResolutionTableValued AS (
        SELECT
              RT.Description AS DocumentUnitName
              ,R.[Year] AS Year
              ,COALESCE(R.ServiceNumber, CAST(R.Number as nvarchar(255))) AS Number
              ,R.IdResolution AS EntityId
              ,COALESCE(R.idSubCategory, R.idCategory) as IdCategory
              ,R.idContainer
              ,R.AdoptionUser As RegistrationUser
              ,R.AdoptionDate As RegistrationDate
              ,R.[Object] AS [Subject]
              ,R.UniqueId AS UniqueId 
            FROM [dbo].[Resolution] R
            INNER JOIN [dbo].[ResolutionType] RT on R.idType = RT.idType
        
            INNER JOIN [dbo].[Container] C on R.IdContainer = C.IdContainer
            INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
            LEFT OUTER JOIN @RoleNames.nodes('/ArrayOfDomainGroupModel/DomainGroupModel/Name/text()') as x ( y ) on CG.GroupName =  x.y.value('.','varchar(255)')

            LEFT OUTER JOIN [dbo].[ResolutionRole] RR on R.idResolution = RR.idResolution
            LEFT OUTER JOIN MyRolesResolution R_MSG on RR.IdRole = R_MSG.IdRole

            LEFT OUTER JOIN [dbo].[FascicleResolutions] FP on FP.UniqueIdResolution = R.UniqueId AND ReferenceType = 0
            WHERE ( (@IncludeThreshold = 0 AND R.AdoptionDate BETWEEN @DateFrom AND @DateTo) OR
                    ( @IncludeThreshold = 1 AND ( R.AdoptionDate BETWEEN @ThresholdFrom AND CAST(getdate()-60 AS DATE) OR 
                                                  R.AdoptionDate BETWEEN @DateFrom AND @DateTo))
                   )
                   AND ( (x.y.value('.','varchar(255)') IS NOT NULL AND CG.ResolutionRights like '1%') OR
                         (x.y.value('.','varchar(255)') IS NULL AND R_MSG.IdRole IS NOT NULL) OR
                         (x.y.value('.','varchar(255)') IS NOT NULL AND R_MSG.IdRole IS NOT NULL)
                       )
                   AND NOT (R_MSG.IdRole IS NULL AND x.y.value('.','varchar(255)') IS NULL)

                   AND FP.IdFascicleResolution IS NULL
            GROUP BY RT.Description, R.[Year], COALESCE(R.ServiceNumber,CAST(R.Number as nvarchar(255))), R.IdResolution, COALESCE(R.idSubCategory, R.idCategory), R.idContainer, R.AdoptionUser, R.AdoptionDate, R.[Object], R.UniqueId)

SELECT UD.*
      ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
FROM (SELECT * 
      FROM ProtocolTableValued 
      UNION 
      SELECT * 
      FROM ResolutionTableValued) UD 
     INNER JOIN [dbo].[Category] CT on UD.idCategory = CT.idCategory
     INNER JOIN [dbo].[Container] C on UD.idContainer = C.idContainer);

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