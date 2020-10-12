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
PRINT 'Versionamento database alla 9.09'
GO

EXEC dbo.VersioningDatabase N'9.09',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT N'Modifica SQL Function [dbo].[Dossiers_FX_AuthorizedDossiers] ADD PARAMETERS @IdDossier, @DossierType AND @Status';
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
	@MetadataValues keyvalue_tbltype READONLY,
	@IdCategory smallint,
	@DossierType smallint,
	@Status smallint
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
			 inner join dbo.DossierRoles DR on Dossier.IdDossier = DR.IdDossier
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
			   AND (@IdCategory is null or Dossier.IdCategory = @IdCategory)
			   AND (@DossierType is null or Dossier.DossierType = @DossierType)
			   AND (@Status is null or Dossier.Status = @Status)
			Group by Dossier.IdDossier, Dossier.Year, Dossier.Number) T where T.rownum > @Skip AND T.rownum <= @Top + @Skip
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
PRINT N'Modifica SQL Function [dbo].[Dossiers_FX_CountAuthorizedDossiers] ADD PARAMETERS @IdDossier, @DossierType AND @Status';
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
	@MetadataValues keyvalue_tbltype READONLY,
	@IdCategory smallint,
	@DossierType smallint,
	@Status smallint
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
	inner join dbo.DossierRoles DR on Dossier.IdDossier = DR.IdDossier
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
			   AND (@IdCategory is null or Dossier.IdCategory = @IdCategory)
			   AND (@DossierType is null or Dossier.DossierType = @DossierType)
			   AND (@Status is null or Dossier.Status = @Status)

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
PRINT N'Modifica SQL Function [dbo].[Fascicles_FX_AuthorizedFascicles] UPDATE rownum';
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
@Title NVARCHAR(256),
@IsManager BIT,
@IsSecretary BIT)
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
                                    AND @ViewOnlyClosable = 0)
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
												WHERE MV2.IdFascicle = Fascicle.IdFascicle
												AND (EXISTS 
													(SELECT TOP 1 1 from @MetadataValues MTVP where MTVP.KeyName = MV2.Name)
												) 
												AND MV2.IdFascicle NOT IN 
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
												)))
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
							 AND (@IsManager IS NULL OR @IsManager = 0
								  OR (@IsManager = 1 AND (SELECT [webapiprivate].[Fascicles_FX_IsManager] (@UserName, @Domain, Fascicle.IdFascicle)) = 1))
							 AND (@IsSecretary IS NULL OR @IsSecretary = 0
								  OR (@IsSecretary = 1 AND (SELECT [webapiprivate].[Fascicles_FX_IsProcedureSecretary] (@UserName, @Domain, Fascicle.IdCategory)) = 1))
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
                   AND T.rownum <= @Top + @Skip)
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
PRINT 'Create [webapiprivate].[RoleUser_FX_AllSecretariesFromDossier]'
GO

CREATE FUNCTION  [webapiprivate].[RoleUser_FX_AllSecretariesFromDossier]
(   
    @IdDossier UNIQUEIDENTIFIER
)
RETURNS TABLE 
AS
RETURN 
(
    WITH
    FascicleIdRoles
    AS
    (
        SELECT DISTINCT FascicleRoles.IdRole
        FROM [dbo].DossierFolders DossierFolders
        INNER JOIN [dbo].FascicleRoles FascicleRoles ON DossierFolders.IdFascicle = FascicleRoles.IdFascicle
        WHERE DossierFolders.IdFascicle is not null
            and FascicleRoles.RoleAuthorizationType = 0
            and FascicleRoles.IsMaster = 1
            and DossierFolders.IdDossier = @IdDossier
    )

   SELECT 
    RoleUser.[Type],
    RoleUser.[Description],
    RoleUser.[Account],
    RoleUser.[Enabled],
    RoleUser.[Email],
    RoleUser.[IsMainRole],
    RoleUser.[DSWEnvironment],
    RoleUser.[idRole] FROM [dbo].RoleUser RoleUser
        WHERE RoleUser.Type = 'SP'
        AND RoleUser.Email is not null and RoleUser.Email <> ''
        AND RoleUser.Enabled = 1
        AND RoleUser.IsMainRole = 1
        AND RoleUser.idRole IN (SELECT * FROM FascicleIdRoles)
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
PRINT 'Alter [webapiprivate].[Dossiers_FX_CountAuthorizedDossiers]'
GO

ALTER FUNCTION [webapiprivate].[Dossiers_FX_CountAuthorizedDossiers](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Year smallint,
	@Number smallint,
	@Subject nvarchar(255),
    @Note nvarchar(255),
	@ContainerId smallint,
	@MetadataRepositoryId uniqueidentifier,
	@MetadataValue nvarchar(255),
    @StartDateFrom datetimeoffset,
	@StartDateTo datetimeoffset,
	@EndDateFrom datetimeoffset,
	@EndDateTo datetimeoffset,
	@MetadataValues keyvalue_tbltype READONLY,
	@IdCategory smallint,
	@DossierType smallint,
	@Status smallint
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
	inner join dbo.DossierRoles DR on Dossier.IdDossier = DR.IdDossier
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
            AND (@StartDateFrom is null or Dossier.StartDate >= @StartDateFrom)
			AND (@StartDateTo is null or Dossier.StartDate <= @StartDateTo)
			AND (@EndDateFrom is null or Dossier.EndDate >= @StartDateFrom)
			AND (@EndDateTo is null or Dossier.EndDate <= @EndDateTo)
            AND (@Note is null or Dossier.Note like '%'+@Note+'%')
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
			   AND (@IdCategory is null or Dossier.IdCategory = @IdCategory)
			   AND (@DossierType is null or Dossier.DossierType = @DossierType)
			   AND (@Status is null or Dossier.Status = @Status)

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
PRINT 'Alter [webapiprivate].[Dossiers_FX_AuthorizedDossiers] - fixing @EndDateFrom check to correct variable name'
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
	@MetadataValues keyvalue_tbltype READONLY,
	@IdCategory smallint,
	@DossierType smallint,
	@Status smallint
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
			 inner join dbo.DossierRoles DR on Dossier.IdDossier = DR.IdDossier
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
			   AND (@EndDateFrom is null or Dossier.EndDate >= @EndDateFrom)
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
			   AND (@IdCategory is null or Dossier.IdCategory = @IdCategory)
			   AND (@DossierType is null or Dossier.DossierType = @DossierType)
			   AND (@Status is null or Dossier.Status = @Status)
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
PRINT 'Create [webapiprivate].[FascicleFolder_FX_FascicleFoldersWithSameName]'
GO

CREATE FUNCTION [webapiprivate].[FascicleFolder_FX_FascicleFoldersWithSameName](
	@ReferenceFascicleId uniqueidentifier,
	@DestinationFascicleId uniqueidentifier,
	@FascicleFolderLevel int
)
RETURNS TABLE
AS
RETURN
(
	SELECT [REF].[Name], [REF].[FascicleFolderLevel], [REF].[IdFascicleFolder] from [dbo].[FascicleFolders] REF 
	WHERE 
		REF.IdFascicle = @ReferenceFascicleId 
		AND (@FascicleFolderLevel IS NULL 
			OR @FascicleFolderLevel IS NOT NULL AND [REF].[FascicleFolderLevel] >= @FascicleFolderLevel)
		AND (exists (SELECT TOP 1 1 from [dbo].[FascicleFolders] DEST where [DEST].[IdFascicle] = @DestinationFascicleId and [REF].[Name] = [DEST].[Name] and [REF].[FascicleFolderLevel] = [DEST].[FascicleFolderLevel]))
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
PRINT 'Alter [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]'
GO

ALTER FUNCTION [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@Environment integer,
	@AnyEnvironment tinyint,
	@DocumentRequired tinyint,
    @ShowOnlyNoInstanceWorkflows tinyint,
	@ShowOnlyHasIsFascicleClosedRequired tinyint
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
			AND (
                ((@ShowOnlyHasIsFascicleClosedRequired IS NULL OR @ShowOnlyHasIsFascicleClosedRequired = 0) AND NOT EXISTS (SELECT 1 
                                                                                  FROM WorkflowEvaluationProperties WFP 
                                                                                  WHERE WFP.IdWorkflowRepository = WR.IdWorkflowRepository 
                                                                                        AND WFP.Name = '_dsw_v_Workflow_IsClosedFascicleRequired' AND WFP.ValueBoolean = 1)
                )  
             OR (@ShowOnlyHasIsFascicleClosedRequired = 1 AND EXISTS (SELECT 1
                                                   FROM WorkflowEvaluationProperties WFP 
                                                   WHERE WFP.IdWorkflowRepository = WR.IdWorkflowRepository 
                                                         AND WFP.Name = '_dsw_v_Workflow_IsClosedFascicleRequired' AND WFP.ValueBoolean = 1)
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

PRINT N'Alter function [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]' 
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
	where @IdTenantAOO IS NULL or (@IdTenantAOO  IS NOT NULL and du.IdTenantAOO = @IdTenantAOO)
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

PRINT N'Alter function [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]' 
GO

CREATE FUNCTION [webapiprivate].[DossierFolder_FX_GetParent] (
	@IdDossierFolder uniqueidentifier
)
RETURNS TABLE
AS 
	RETURN
	(
	WITH Ancestor AS 
	(
		SELECT DossierFolderNode.GetAncestor(1) AS Ancestor
		FROM DossierFolders 
		WHERE IdDossierFolder=@IdDossierFolder
	)
	SELECT   [DF].[IdDossierFolder]
	  		,[DF].[Name]
			,[DF].[Status]
            ,[DF].[JsonMetadata]
            ,[DF].[DossierFolderLevel]
            ,[DF].[DossierFolderPath]
		    ,[DF].[IdDossier] as Dossier_IdDossier
			,[DF].[IdFascicle] as Fascicle_IdFascicle
			,[DF].[IdCategory] as Category_IdCategory
			,[DFR].[IdRole] as Role_IdRole
	   FROM DossierFolders DF 
	   INNER JOIN Ancestor A ON DF.DossierFolderNode = A.Ancestor 
	   LEFT JOIN DossierFolderRoles DFR ON DFR.IdDossierFolder = DF.IdDossierFolder
	   WHERE DF.DossierFolderLevel > 1	 
	   GROUP BY  [DF].[IdDossierFolder]
		        ,[DF].[Name]
				,[DF].[Status]
				,[DF].[JsonMetadata]
				,[DF].[DossierFolderLevel]
				,[DF].[DossierFolderPath]
                ,[DF].[IdDossier]
				,[DF].[IdFascicle]
                ,[DF].[IdCategory]
                ,[DFR].[IdRole]
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