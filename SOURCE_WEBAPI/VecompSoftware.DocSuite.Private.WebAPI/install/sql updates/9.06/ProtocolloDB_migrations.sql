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
PRINT 'Versionamento database alla 9.06'
GO

EXEC dbo.VersioningDatabase N'9.06',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT N'Creata SQL Function [dbo].[Fascicles_FX_CountAuthorizedFasciclesFromDocumentUnit]';
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_CountAuthorizedFasciclesFromDocumentUnit]
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@UniqueId uniqueidentifier
)
RETURNS INT
AS
BEGIN
    DECLARE @CountFascicles AS INT;
    SELECT @CountFascicles = COUNT(DISTINCT(F.IdFascicle))
	FROM Fascicles F 
		INNER JOIN FascicleDocumentUnits FP on F.IdFascicle = FP.IdFascicle AND FP.IdDocumentUnit = @UniqueId
		AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, F.IdFascicle)) = 1)

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
PRINT N'Creata SQL Function [dbo].[Fascicles_FX_AuthorizedFasciclesFromDocumentUnit]';
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_AuthorizedFasciclesFromDocumentUnit]
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@UniqueId uniqueidentifier
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
		INNER JOIN FascicleDocumentUnits FP on F.IdFascicle = FP.IdFascicle AND FP.IdDocumentUnit = @UniqueId
		AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, F.IdFascicle)) = 1)
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
PRINT N'[webapiprivate].[Collaboration_FX_CollaborationsDeletationSigning]'
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsDeletationSigning](
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
PRINT N'Modifica SQL Function [dbo].[Fascicles_FX_AuthorizedFascicles]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AuthorizedFascicles]
(@UserName NVARCHAR (255), 
@Domain NVARCHAR (255), 
@Skip INT, 
@Top INT, 
@Year SMALLINT, 
@StartDateFrom DATETIMEOFFSET, 
@StartDateTo DATETIMEOFFSET, 
@EndDateFrom DATETIMEOFFSET, 
@EndDateTo DATETIMEOFFSET, 
@FascicleStatus INT, 
@Manager NVARCHAR (256), 
@Name NVARCHAR (256), 
@ViewConfidential BIT, 
@ViewAccessible BIT, 
@Subject NVARCHAR (256), 
@Note NVARCHAR (256), 
@Rack NVARCHAR (256), 
@IdMetadataRepository NVARCHAR (256), 
@MetadataValue NVARCHAR (256),
@Classifications NVARCHAR (256),
@IncludeChildClassifications BIT, 
@Roles NVARCHAR (MAX), 
@Container SMALLINT, 
@ApplySecurity BIT, 
@MetadataValues keyvalue_tbltype READONLY,
@ViewOnlyClosable BIT,
@ThresholdDate DATETIMEOFFSET)
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
							AND (@ViewOnlyClosable IS NULL
								OR (@ViewOnlyClosable IS NOT NULL 
									AND @ViewOnlyClosable = 1 
									AND Fascicle.EndDate IS NULL
									AND Fascicle.LastChangedDate < @ThresholdDate))
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
                             AND (NOT EXISTS (SELECT TOP 1 1 FROM @MetadataValues)
								  OR EXISTS (SELECT TOP 1 1 FROM
												MetadataValues MV2 
												WHERE MV2.IdFascicle NOT IN 
												(
													SELECT IdFascicle FROM 
													(
														SELECT MV.*, 
														(
															SELECT COUNT(IdMetadataValue) 
															FROM MetadataValues MV1
															WHERE MV1.IdMetadataValue = MV.IdMetadataValue 
															AND MV1.Name = MTVP.KeyName 
															AND (
																(MV.PropertyType <> 1 OR (MV.PropertyType = 1 and MV.ValueString like '%'+MTVP.Value+'%'))
																AND 
																(
																	MV.PropertyType <> 2 OR (
																	MV.PropertyType = 2 
																	and MV.ValueInt >= (SELECT TOP 1 CAST(Value as int) FROM SplitString(MTVP.Value,'|'))
																	and 
																	(
																		(
																			(SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) = 2
																			and MV.ValueInt <= (SELECT CAST(Value AS int) FROM (SELECT ROW_NUMBER() OVER(ORDER BY CAST(Value as int)) AS sequencenumber, Value 
																													FROM SplitString(MTVP.Value,'|')) AS T WHERE sequencenumber = 2)
																		)
																		OR ((SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) <> 2)
																	))
																)
																AND 
																(
																	MV.PropertyType <> 4 OR (
																	MV.PropertyType = 4 
																	and MV.ValueDate >= (SELECT TOP 1 CAST(Value as dateTime) FROM SplitString(MTVP.Value,'|'))
																	and 
																	(
																		(
																			(SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) = 2
																			and MV.ValueDate <= (SELECT CAST(Value as datetime) FROM (SELECT ROW_NUMBER() OVER(ORDER BY CAST(Value as DateTime)) AS sequencenumber, Value 
																													 FROM SplitString(MTVP.Value,'|')) AS T WHERE sequencenumber = 2)
																		)
																		OR ((SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) <> 2)
																	))
																)
																AND 
																(
																	MV.PropertyType <> 8 OR (
																	MV.PropertyType = 8
																	and MV.ValueDouble >= (SELECT TOP 1 CAST(Value as float) FROM SplitString(MTVP.Value,'|'))
																	and 
																	(
																		(
																			(SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) = 2
																			and MV.ValueDouble <= (SELECT CAST(Value AS float) FROM (SELECT ROW_NUMBER() OVER(ORDER BY CAST(Value as float)) AS sequencenumber, Value 
																													FROM SplitString(MTVP.Value,'|')) AS T WHERE sequencenumber = 2)
																		)
																		OR ((SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) <> 2)
																	))
																)																
																AND (MV.PropertyType <> 16 OR (MV.PropertyType = 16 and (MV.ValueBoolean = CAST(MTVP.Value as bit) OR MTVP.Value = '')))
																AND (MV.PropertyType <> 32 OR (MV.PropertyType = 32 and (MV.ValueGuid = CAST(MTVP.Value as uniqueidentifier) OR MTVP.Value = '')))
															)
														) as SearchEvaluated FROM MetadataValues MV
														INNER JOIN @MetadataValues MTVP ON MTVP.KeyName = MV.Name
														WHERE MV.IdFascicle = Fascicle.IdFascicle
													) as TCalculated
													WHERE TCalculated.SearchEvaluated = 0
												) and MV2.IdFascicle = Fascicle.IdFascicle))
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
		   F.StartDate,
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
           F_CON.Description AS Contact_Description,
		   F.MetadataValues,
		   F.MetadataDesigner,
		   F.DSWEnvironment,
		   F.Note,
		   F.Conservation,
		   F.LastChangedDate
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
(@UserName NVARCHAR (255), 
@Domain NVARCHAR (255), 
@Year SMALLINT, 
@StartDateFrom DATETIMEOFFSET, 
@StartDateTo DATETIMEOFFSET, 
@EndDateFrom DATETIMEOFFSET, 
@EndDateTo DATETIMEOFFSET,
@FascicleStatus INT,
@Manager NVARCHAR (256),
@Name NVARCHAR (256),
@ViewConfidential BIT,
@ViewAccessible BIT, 
@Subject NVARCHAR (256), 
@Note NVARCHAR (256), 
@Rack NVARCHAR (256),
@IdMetadataRepository NVARCHAR (256),
@MetadataValue NVARCHAR (256), 
@Classifications NVARCHAR (256),
@IncludeChildClassifications BIT,
@Roles NVARCHAR (MAX), 
@Container SMALLINT, 
@ApplySecurity BIT, 
@MetadataValues keyvalue_tbltype READONLY,
@ViewOnlyClosable BIT,
@ThresholdDate DATETIMEOFFSET)
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
		   AND (@ViewOnlyClosable IS NULL
				OR (@ViewOnlyClosable IS NOT NULL 
					AND @ViewOnlyClosable = 1 
					AND Fascicle.EndDate IS NULL
					AND Fascicle.LastChangedDate < @ThresholdDate))
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
           AND (NOT EXISTS (SELECT TOP 1 1 FROM @MetadataValues)
								  OR EXISTS (SELECT TOP 1 1 FROM
												MetadataValues MV2 
												WHERE MV2.IdFascicle NOT IN 
												(
													SELECT IdFascicle FROM 
													(
														SELECT MV.*, 
														(
															SELECT COUNT(IdMetadataValue) 
															FROM MetadataValues MV1
															WHERE MV1.IdMetadataValue = MV.IdMetadataValue 
															AND MV1.Name = MTVP.KeyName 
															AND (
																(MV.PropertyType <> 1 OR (MV.PropertyType = 1 and MV.ValueString like '%'+MTVP.Value+'%'))
																AND 
																(
																	MV.PropertyType <> 2 OR (
																	MV.PropertyType = 2 
																	and MV.ValueInt >= (SELECT TOP 1 CAST(Value as int) FROM SplitString(MTVP.Value,'|'))
																	and 
																	(
																		(
																			(SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) = 2
																			and MV.ValueInt <= (SELECT CAST(Value AS int) FROM (SELECT ROW_NUMBER() OVER(ORDER BY CAST(Value as int)) AS sequencenumber, Value 
																													FROM SplitString(MTVP.Value,'|')) AS T WHERE sequencenumber = 2)
																		)
																		OR ((SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) <> 2)
																	))
																)
																AND 
																(
																	MV.PropertyType <> 4 OR (
																	MV.PropertyType = 4 
																	and MV.ValueDate >= (SELECT TOP 1 CAST(Value as dateTime) FROM SplitString(MTVP.Value,'|'))
																	and 
																	(
																		(
																			(SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) = 2
																			and MV.ValueDate <= (SELECT CAST(Value as datetime) FROM (SELECT ROW_NUMBER() OVER(ORDER BY CAST(Value as DateTime)) AS sequencenumber, Value 
																													 FROM SplitString(MTVP.Value,'|')) AS T WHERE sequencenumber = 2)
																		)
																		OR ((SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) <> 2)
																	))
																)
																AND 
																(
																	MV.PropertyType <> 8 OR (
																	MV.PropertyType = 8
																	and MV.ValueDouble >= (SELECT TOP 1 CAST(Value as float) FROM SplitString(MTVP.Value,'|'))
																	and 
																	(
																		(
																			(SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) = 2
																			and MV.ValueDouble <= (SELECT CAST(Value AS float) FROM (SELECT ROW_NUMBER() OVER(ORDER BY CAST(Value as float)) AS sequencenumber, Value 
																													FROM SplitString(MTVP.Value,'|')) AS T WHERE sequencenumber = 2)
																		)
																		OR ((SELECT COUNT(Value) FROM SplitString(MTVP.Value,'|')) <> 2)
																	))
																)																
																AND (MV.PropertyType <> 16 OR (MV.PropertyType = 16 and (MV.ValueBoolean = CAST(MTVP.Value as bit) OR MTVP.Value = '')))
																AND (MV.PropertyType <> 32 OR (MV.PropertyType = 32 and (MV.ValueGuid = CAST(MTVP.Value as uniqueidentifier) OR MTVP.Value = '')))
															)
														) as SearchEvaluated FROM MetadataValues MV
														INNER JOIN @MetadataValues MTVP ON MTVP.KeyName = MV.Name
														WHERE MV.IdFascicle = Fascicle.IdFascicle
													) as TCalculated
													WHERE TCalculated.SearchEvaluated = 0
												) and MV2.IdFascicle = Fascicle.IdFascicle))
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
PRINT N'Modificata SQL Function [webapiprivate].[Category_FX_GeAvailablePeriodicCategoryFascicles]';
GO

ALTER FUNCTION [webapiprivate].[Category_FX_GeAvailablePeriodicCategoryFascicles]
(
	@idCategory smallint
)
RETURNS TABLE
AS
RETURN
(	 
	  SELECT [CF].[IdCategoryFascicle] as UniqueId
      ,[CF].[IdCategory]
      ,[CF].[IdFasciclePeriod]
      ,[CF].[FascicleType]
      ,[CF].[Manager]
      ,[CF].[DSWEnvironment]
	  ,[CF].[CustomActions]
      ,[CF].[RegistrationUser]
      ,[CF].[RegistrationDate]
      ,[CF].[LastChangedUser]
      ,[CF].[LastChangedDate]
      ,[CF].[Timestamp]
	    FROM [dbo].[CategoryFascicles] CF
		WHERE [idCategory] = @idCategory 
		AND [FascicleType] = 2  AND NOT EXISTS (SELECT DSWEnvironment FROM [dbo].[Fascicles] 
															WHERE [idCategory] = @idCategory AND [FascicleType] = 2 
																AND [StartDate] < GETUTCDATE() AND ([EndDate] > GETUTCDATE() OR [EndDate] IS NULL)
																AND [CF].[DSWEnvironment] = [DSWEnvironment])
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
PRINT N'Modificata SQL Function [webapiprivate].[Collaboration_FX_CollaborationsManaging]';
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsManaging](
 	@UserName nvarchar(255))
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
 			left outer join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
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