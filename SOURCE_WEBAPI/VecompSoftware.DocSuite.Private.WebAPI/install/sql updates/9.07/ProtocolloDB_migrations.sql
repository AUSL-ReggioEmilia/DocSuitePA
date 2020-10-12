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
PRINT 'Versionamento database alla 9.07'
GO

EXEC dbo.VersioningDatabase N'9.07',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT N'Modifica SQL Function [dbo].[Fascicles_FX_AuthorizedFascicles] ADD PARAMETER @Title';
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
@ThresholdDate DATETIMEOFFSET,
@Title NVARCHAR(256))
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
							 AND
							 (@Title IS NULL
							  OR Fascicle.Title LIKE '%' + @Title + '%')
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
PRINT N'Modifica SQL Function [dbo].[Fascicles_FX_AuthorizedFascicles] RETURN TenantAOO Name and Id';
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
@ThresholdDate DATETIMEOFFSET,
@Title NVARCHAR(256))
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
							 AND
							 (@Title IS NULL
							  OR Fascicle.Title LIKE '%' + @Title + '%')
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
		   F.LastChangedDate,
		   C_TAOO.IdTenantAOO AS TenantAOO_IdTenantAOO,
		   C_TAOO.Name AS TenantAOO_Name
    FROM   Fascicles AS F
           INNER JOIN
           MyFascicles AS MF
           ON MF.IdFascicle = F.IdFascicle
           INNER JOIN
           Category AS F_C
           ON F_C.idCategory = F.IdCategory
		   INNER JOIN
		   TenantAOO AS C_TAOO
		   ON F_C.IdTenantAOO = C_TAOO.IdTenantAOO
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
PRINT N'Modifica SQL Function [dbo].[Fascicles_FX_CountAuthorizedFascicles] ADD PARAMETER @Title';
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
@ThresholdDate DATETIMEOFFSET,
@Title NVARCHAR(256))
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
		   AND (@Title IS NULL
				OR Fascicle.Title LIKE '%' + @Title + '%')
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
PRINT N'Creata funzione [webapiprivate].[DossierFolder_FX_GetAllParentsOfFascicle]' 
GO

CREATE FUNCTION [webapiprivate].[DossierFolder_FX_GetAllParentsOfFascicle] (
	@IdDossier uniqueidentifier,
	@IdFascicle uniqueidentifier
	)
	RETURNS TABLE
AS
	RETURN
		(
		WITH results AS
		(
		    SELECT IdDossierFolder
				  ,Name
				  ,Status
				  ,IdDossier AS Dossier_IdDossier
				  ,IdFascicle AS Fascicle_IdFascicle
				  ,IdCategory AS Category_IdCategory
				  ,DossierFolderNode
		    FROM DossierFolders 
		    WHERE IdDossier = @IdDossier AND IdFascicle = @IdFascicle
		    UNION ALL
		    SELECT DF.IdDossierFolder
				  ,DF.Name
				  ,DF.Status
				  ,DF.IdDossier AS Dossier_IdDossier
				  ,DF.IdFascicle AS Fascicle_IdFascicle
				  ,DF.IdCategory AS Category_IdCategory
				  ,DF.DossierFolderNode
		    FROM DossierFolders DF INNER JOIN results r ON r.DossierFolderNode.GetAncestor(1) = DF.DossierFolderNode
			WHERE DF.DossierFolderNode.GetAncestor(1) IS NOT NULL
		)
		SELECT * FROM results
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
PRINT N'Alter sql function [webapiprivate].[FascicleFolder_FX_AllChildrenByParent]'
GO

ALTER FUNCTION [webapiprivate].[FascicleFolder_FX_AllChildrenByParent] (
	@IdFascicleFolder uniqueidentifier
	)
	RETURNS TABLE
AS 
	RETURN
	(	  
		SELECT [FF].[IdFascicleFolder]
			,[FF].[Name]
			,[FF].[Status]
			,[FF].[Typology]
			,[FF].[IdFascicle] as Fascicle_IdFascicle
			,[FF].[IdCategory] as Category_IdCategory
			,[FD].[IdFascicleDocument] as Document_IdFascicleDocument
			,[FD].[IdArchiveChain] as Document_IdArchiveChain
			,[FD].[ChainType] as Document_ChainType
			,CAST((SELECT COUNT(IdFascicleFolder) AS Result
		from FascicleFolders 
		WHERE
		IdFascicleFolder = @IdFascicleFolder
		AND (EXISTS (SELECT 1 FROM FascicleDocumentUnits WHERE idfasciclefolder = FF.IdFascicleFolder)			
			)) AS BIT) as HasDocuments
			,CAST(COUNT(FFF.IdFascicleFolder) AS BIT) as HasChildren
			, [FF].[FascicleFolderLevel]	
	   FROM FascicleFolders FF 
	   LEFT OUTER JOIN FascicleFolders AS FFF
	   ON FF.FascicleFolderNode = FFF.FascicleFolderNode.GetAncestor(1)
	   LEFT OUTER JOIN FascicleDocuments AS FD
	   ON FF.IdFascicleFolder = FD.IdFascicleFolder
	   WHERE FF.FascicleFolderNode.GetAncestor(1) = (SELECT TOP 1 FascicleFolderNode 
												 FROM FascicleFolders 
												 WHERE IdFascicleFolder = @IdFascicleFolder)		
		GROUP BY [FF].[IdFascicleFolder]
		        ,[FF].[Name]
				,[FF].[Status]
				,[FF].[Typology]
				,[FF].[IdFascicle]
				,[FF].[IdCategory]	
				,[FF].[FascicleFolderLevel]
				,[FD].[IdFascicleDocument]
				,[FD].[IdArchiveChain]
				,[FD].[ChainType]
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
PRINT N'Alter function [webapiprivate].[Role_FX_FindRoles]' 
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
                    ((@Environment = 3 OR @Environment = 9) AND RG.DocumentRights <>'00000000000000000000') OR
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
PRINT N'Alter function [webapiprivate].[Dossiers_FX_GetDossierContacts]' 
GO

ALTER FUNCTION [webapiprivate].[Dossiers_FX_GetDossierContacts]
(
	@IdDossier uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
    SELECT C.UniqueId, C.Incremental as IdContact, C.IdContactType as ContactType, C.[Description], C.IncrementalFather, cast(1 as bit) as IsSelected
        FROM [dbo].[DossierContacts] DC 
		INNER JOIN [dbo].[Contact] C ON C.Incremental = DC.IdContact
		WHERE DC.IdDossier = @IdDossier
		GROUP BY C.UniqueId, C.Incremental, C.IncrementalFather, C.IdContactType, C.[Description]
)
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