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
PRINT 'Versionamento database alla 9.04'
GO

EXEC dbo.VersioningDatabase N'9.04',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT 'Create user table type keyvalue_tbltype'
GO

CREATE TYPE keyvalue_tbltype AS TABLE (KeyName nvarchar(256), Value nvarchar(max));  
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
(@UserName NVARCHAR (255), @Domain NVARCHAR (255), @Skip INT, @Top INT, @Year SMALLINT, @StartDateFrom DATETIMEOFFSET, @StartDateTo DATETIMEOFFSET, @EndDateFrom DATETIMEOFFSET, @EndDateTo DATETIMEOFFSET, @FascicleStatus INT, @Manager NVARCHAR (256), @Name NVARCHAR (256), @ViewConfidential BIT, @ViewAccessible BIT, @Subject NVARCHAR (256), @Note NVARCHAR (256), @Rack NVARCHAR (256), @IdMetadataRepository NVARCHAR (256), @MetadataValue NVARCHAR (256), @Classifications NVARCHAR (256), @IncludeChildClassifications BIT, @Roles NVARCHAR (MAX), @Container SMALLINT, @ApplySecurity BIT, @MetadataValues keyvalue_tbltype READONLY)
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
		   F.Conservation
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
(@UserName NVARCHAR (255), @Domain NVARCHAR (255), @Year SMALLINT, @StartDateFrom DATETIMEOFFSET, @StartDateTo DATETIMEOFFSET, @EndDateFrom DATETIMEOFFSET, @EndDateTo DATETIMEOFFSET, @FascicleStatus INT, @Manager NVARCHAR (256), @Name NVARCHAR (256), @ViewConfidential BIT, @ViewAccessible BIT, @Subject NVARCHAR (256), @Note NVARCHAR (256), @Rack NVARCHAR (256), @IdMetadataRepository NVARCHAR (256), @MetadataValue NVARCHAR (256), @Classifications NVARCHAR (256), @IncludeChildClassifications BIT, @Roles NVARCHAR (MAX), @Container SMALLINT, @ApplySecurity BIT, @MetadataValues keyvalue_tbltype READONLY)
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
	@MetadataValue nvarchar (255),
	@StartDateFrom datetimeoffset,
	@StartDateTo datetimeoffset,
	@EndDateFrom datetimeoffset,
	@EndDateTo datetimeoffset,
	@MetadataValues keyvalue_tbltype READONLY
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
			   AND (@MetadataValue is null or EXISTS (SELECT 1 FROM dbo.SplitString(Dossier.MetadataValues, ',')
											 WHERE Value like '%"Value":%' and Value like '%'+@MetadataValue+'%'))
			   AND (NOT EXISTS (SELECT TOP 1 1 FROM @MetadataValues)
								  OR EXISTS (SELECT TOP 1 1 FROM
												MetadataValues MV2 
												WHERE MV2.IdDossier NOT IN 
												(
													SELECT IdDossier FROM 
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
														WHERE MV.IdDossier = Dossier.idDossier
													) as TCalculated
													WHERE TCalculated.SearchEvaluated = 0
												) and MV2.IdDossier = Dossier.idDossier))
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
	@MetadataValue nvarchar(255),
	@MetadataValues keyvalue_tbltype READONLY
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
			AND (@MetadataValue is null or EXISTS (SELECT 1 FROM dbo.SplitString(Dossier.MetadataValues, ',')
											 WHERE Value like '%"Value":%' and Value like '%'+@MetadataValue+'%'))
			   AND (NOT EXISTS (SELECT TOP 1 1 FROM @MetadataValues)
								  OR EXISTS (SELECT TOP 1 1 FROM
												MetadataValues MV2 
												WHERE MV2.IdDossier NOT IN 
												(
													SELECT IdDossier FROM 
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
														WHERE MV.IdDossier = Dossier.idDossier
													) as TCalculated
													WHERE TCalculated.SearchEvaluated = 0
												) and MV2.IdDossier = Dossier.idDossier))	

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
		   CAST(DU.[Number] as nvarchar) AS [Number],
		   DU.[Title],
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
		   DU.[Number],
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