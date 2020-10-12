/****************************************************************************************************************************************
* Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)  *
* <DBProtocollo, varchar(255), DBProtocollo>  --> Settare il nome del DB di protocollo.                  *
* <DBPratiche, varchar(255), DBPratiche>  --> Se esiste il DB di Pratiche settare il nome.              *
* <DBAtti, varchar(255), DBAtti>      --> Se esiste il DB di Atti settare il nome.            *
* <EXIST_DB_ATTI, varchar(255), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva  *
* <EXIST_DB_PRATICHE, varchar(255), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
* <EXIST_DB_PROTOCOLLO, varchar(255), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
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
PRINT 'Versionamento database alla 9.02'
GO

EXEC dbo.VersioningDatabase N'9.02',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT 'CREATE SQL Function [webapiprivate].[Category_FX_FindFascicolableCategory]'
GO

CREATE FUNCTION [webapiprivate].[Category_FX_FindFascicolableCategory]
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@CategoryId smallint
)
RETURNS TABLE 
AS
RETURN 
(
    WITH   CategoryPaths
	AS 
    (            
		SELECT [Value] AS idCategory 
        FROM [dbo].[SplitString]
        (
            (
                SELECT FullIncrementalPath 
                FROM Category 
                WHERE idCategory = @CategoryId
            ), '|'
        )
	),
    CategoryFascicleOccurrence
    AS 
    (
        SELECT TOP 1 idCategory
        FROM CategoryFascicles
        WHERE idCategory IN 
        (
            SELECT idCategory
            FROM CategoryPaths
        )
        AND FascicleType = 1
        ORDER BY idCategory desc
    )

	SELECT [Category].[idCategory] AS IdCategory,[Category].[Name],[Category].[isActive],[Category].[Code],[Category].[FullIncrementalPath],[Category].[FullCode],
	[Category].[RegistrationDate],[Category].[RegistrationUser],[Category].[UniqueId],[Category].[StartDate],[Category].[EndDate],
	[CategoryParent].[idCategory] AS CategoryParent_IdCategory,
	(SELECT TOP 1 CAST(1 AS bit) FROM Category C_TMP 
			WHERE C_TMP.idParent = Category.idCategory
	) As HasChildren, 
	(SELECT CAST(1 AS bit)) AS HasFascicleDefinition
	FROM Category
	LEFT OUTER JOIN Category CategoryParent ON CategoryParent.idCategory = Category.idParent    
	WHERE Category.idCategory = (SELECT idCategory FROM CategoryFascicleOccurrence)
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
PRINT N'Create Stored Procedure [dbo].[Location_Insert]';
GO

CREATE PROCEDURE [dbo].[Location_Insert] 
    @idLocation smallint,
    @UniqueId uniqueidentifier,
	@Name varchar(100),
	@DocumentServer varchar(255),
	@ProtBiblosDSDB varchar(100),
	@DocmBiblosDSDB varchar(100),
	@ReslBiblosDSDB varchar(100),
	@ConservationServer varchar(255),
	@ConsBiblosDSDB varchar(100)
	
AS
	DECLARE @isAttiEnable BIT
	DECLARE @isPraticheEnable BIT
	DECLARE @isProtocolloEnable BIT
	SET @isAttiEnable = CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT)
	SET @isPraticheEnable = CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT)
	SET @isProtocolloEnable = CAST('<EXIST_DB_PROTOCOLLO, varchar(255), True>' AS BIT)

	DECLARE @LastUsedIdLocation INT, @EntityId INT

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION LocationInsert

	SELECT top(1) @LastUsedIdLocation = idLocation FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Location] ORDER BY idLocation DESC
	IF(@LastUsedIdLocation is null)
	BEGIN
		SET @LastUsedIdLocation = 0
	END

	SET @EntityId = @LastUsedIdLocation + 1

	INSERT INTO  <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Location] 
				([idLocation], [UniqueId], [Name], [DocumentServer],[ProtBiblosDSDB], [DocmBiblosDSDB], [ReslBiblosDSDB], [ConservationServer], [ConsBiblosDSDB])
		 VALUES (@EntityId, @UniqueId, @Name, @DocumentServer,@ProtBiblosDSDB, @DocmBiblosDSDB, @ReslBiblosDSDB,@ConservationServer, @ConsBiblosDSDB )

	IF( (CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT) = CAST('True' AS BIT))) 
		BEGIN	 
		INSERT INTO <DBAtti, varchar(255), DBAtti>.[dbo].[Location] 
					([idLocation], [Name], [DocumentServer],[ProtBiblosDSDB], [DocmBiblosDSDB], [ReslBiblosDSDB], [ConservationServer], [ConsBiblosDSDB], [UniqueId])
			 VALUES (@EntityId, @Name, @DocumentServer, @ProtBiblosDSDB, @DocmBiblosDSDB, @ReslBiblosDSDB, @ConservationServer, @ConsBiblosDSDB, @UniqueId )
		END
	
	IF( (CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT) = CAST('True' AS BIT))) 		
		BEGIN	 
		INSERT INTO <DBPratiche, varchar(255), DBPratiche>.[dbo].[Location] 
					([idLocation], [Name], [DocumentServer], [ProtBiblosDSDB], [DocmBiblosDSDB], [ReslBiblosDSDB], [ConservationServer], [ConsBiblosDSDB], [UniqueId])
			 VALUES (@EntityId, @Name, @DocumentServer, @ProtBiblosDSDB, @DocmBiblosDSDB, @ReslBiblosDSDB, @ConservationServer, @ConsBiblosDSDB, @UniqueId )
		END

	COMMIT TRANSACTION LocationInsert													   
	END TRY

	BEGIN CATCH 
    	PRINT ERROR_MESSAGE() 
    	ROLLBACK TRANSACTION LocationInsert
	END CATCH
GO

--#############################################################################
PRINT N'Create Stored Procedure [dbo].[Location_Update]';
GO

CREATE PROCEDURE [dbo].[Location_Update] 
    @idLocation smallint,
    @UniqueId uniqueidentifier,
	@Name varchar(100),
	@DocumentServer varchar(255),
	@ProtBiblosDSDB varchar(100),
	@DocmBiblosDSDB varchar(100),
	@ReslBiblosDSDB varchar(100),
	@ConservationServer varchar(255),
	@ConsBiblosDSDB varchar(100)
	
AS
	DECLARE @isAttiEnable BIT
	DECLARE @isPraticheEnable BIT
	DECLARE @isProtocolloEnable BIT
	SET @isAttiEnable = CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT)
	SET @isPraticheEnable = CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT)
	SET @isProtocolloEnable = CAST('<EXIST_DB_PROTOCOLLO, varchar(255), True>' AS BIT)

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION LocationUpdate
	
	UPDATE  <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Location] SET
				[UniqueId]=@UniqueId, [Name]=@Name, [DocumentServer]=@DocumentServer,[ProtBiblosDSDB]=@ProtBiblosDSDB,
				[DocmBiblosDSDB]=@DocmBiblosDSDB, [ReslBiblosDSDB]=@ReslBiblosDSDB,[ConservationServer]=@ConservationServer,
				[ConsBiblosDSDB]=@ConsBiblosDSDB WHERE [idLocation]=@idLocation

	IF( (CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT) = CAST('True' AS BIT))) 
		BEGIN	 
		UPDATE  <DBAtti, varchar(255), DBAtti>.[dbo].[Location] SET
				[Name]=@Name, [DocumentServer]=@DocumentServer, [ProtBiblosDSDB]=@ProtBiblosDSDB,
				[DocmBiblosDSDB]=@DocmBiblosDSDB, [ReslBiblosDSDB]=@ReslBiblosDSDB, [ConservationServer]=@ConservationServer,
				[ConsBiblosDSDB]=@ConsBiblosDSDB, [UniqueId]=@UniqueId WHERE [idLocation]=@idLocation
		END
	
	IF( (CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT) = CAST('True' AS BIT))) 		
		BEGIN	 
		UPDATE  <DBPratiche, varchar(255), DBPratiche>.[dbo].[Location] SET
				[Name]=@Name, [DocumentServer]=@DocumentServer,[ProtBiblosDSDB]=@ProtBiblosDSDB,
				[DocmBiblosDSDB]=@DocmBiblosDSDB, [ReslBiblosDSDB]=@ReslBiblosDSDB, [ConservationServer]=@ConservationServer,
				[ConsBiblosDSDB]=@ConsBiblosDSDB, [UniqueId]=@UniqueId WHERE [idLocation]=@idLocation
		END

	COMMIT TRANSACTION LocationUpdate													   
	END TRY

	BEGIN CATCH 
    	PRINT ERROR_MESSAGE() 
    	ROLLBACK TRANSACTION LocationUpdate
	END CATCH
GO
--#############################################################################
PRINT N'Create Stored Procedure [dbo].[Location_Delete]';
GO

CREATE PROCEDURE [dbo].[Location_Delete] 
    @idLocation smallint,
    @UniqueId uniqueidentifier,
	@Name varchar(100),
	@DocumentServer varchar(255),
	@ProtBiblosDSDB varchar(100),
	@DocmBiblosDSDB varchar(100),
	@ReslBiblosDSDB varchar(100),
	@ConservationServer varchar(255),
	@ConsBiblosDSDB varchar(100)
	
AS
	DECLARE @isAttiEnable BIT
	DECLARE @isPraticheEnable BIT
	DECLARE @isProtocolloEnable BIT
	SET @isAttiEnable = CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT)
	SET @isPraticheEnable = CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT)
	SET @isProtocolloEnable = CAST('<EXIST_DB_PROTOCOLLO, varchar(255), True>' AS BIT)

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION LocationDelete
	
	IF EXISTS (SELECT TOP 1 [idLocation] FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Location] WHERE [idLocation] = @idLocation)
		 BEGIN 
              DELETE FROM <DBProtocollo, varchar(255), DBProtocollo>.[dbo].[Location] WHERE [idLocation] = @idLocation
	     END


	IF( (CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT) = CAST('True' AS BIT))) 
		IF EXISTS (SELECT TOP 1 [idLocation] FROM [Location] WHERE [idLocation] = @idLocation)
		 BEGIN 
              DELETE FROM <DBAtti, varchar(255), DBAtti>.[dbo].[Location] WHERE [idLocation] = @idLocation
	     END
	
	IF( (CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT) = CAST('True' AS BIT))) 	
		IF EXISTS (SELECT TOP 1 [idLocation] FROM [Location] WHERE [idLocation] = @idLocation)
		 BEGIN 
              DELETE FROM <DBPratiche, varchar(255), DBPratiche>.[dbo].[Location] WHERE [idLocation] = @idLocation
	     END	

	COMMIT TRANSACTION LocationDelete													   
	END TRY

	BEGIN CATCH 
    	PRINT ERROR_MESSAGE() 
    	ROLLBACK TRANSACTION LocationDelete
	END CATCH
GO


--#############################################################################
PRINT 'CREATE SQL Function [webapiprivate].[DocumentUnit_FX_DocumentUnitsByChain]'
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_DocumentUnitsByChain]
(
    @UserName NVARCHAR(256),
    @Domain NVARCHAR(256),
    @IdTenantAOO UNIQUEIDENTIFIER,
	@Chains string_list_tbltype READONLY    
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
    INNER JOIN [cqrs].[DocumentUnitChains] DUC on DUC.IdDocumentUnit = DU.IdDocumentUnit
	INNER JOIN [dbo].[TenantAOO] TAOO on TAOO.IdTenantAOO = DU.IdTenantAOO
	INNER JOIN [dbo].[Category] CT on DU.idCategory = CT.idCategory
	INNER JOIN [dbo].[Container] C on DU.idContainer = C.idContainer
	INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
	LEFT OUTER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
	
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DU.IdDocumentUnit = DUR.IdDocumentUnit
	LEFT OUTER JOIN [dbo].[Role] RL on DUR.UniqueIdRole = RL.UniqueId
	LEFT OUTER JOIN [dbo].[RoleGroup] RG on RL.idRole = RG.idRole
	LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup

	WHERE DU.IdTenantAOO = @IdTenantAOO 
    AND 
    ( 
        (
            C_MSG.IdGroup IS NOT NULL 
            AND 
            (
                CASE Environment 
				WHEN 1 THEN CG.Rights 
				WHEN 2 THEN CG.ResolutionRights 
				WHEN 3 THEN '0'
				WHEN 4 THEN CG.DocumentSeriesRights
				WHEN 5 THEN '0'
				ELSE CG.UDSRights
				END
            ) like '1%'
        ) 
        OR 
		( 
            DUR.UniqueIdRole IS NULL 
            OR 
            (
                DUR.UniqueIdRole IS NOT NULL 
                AND 
                (
                    CASE Environment 
                    WHEN 1 THEN RG.ProtocolRights 
                    WHEN 2 THEN RG.ResolutionRights 
                    WHEN 3 THEN '0'
                    WHEN 4 THEN RG.DocumentSeriesRights
                    ELSE '0'
                    END like '1%'
                ) 
                AND MSG.IdGroup IS NOT NULL
            )
        ) 
	)
	AND NOT (C_MSG.IdGroup IS NULL AND MSG.IdGroup IS NULL)
    AND DUC.IdArchiveChain IN 
    (
        SELECT val FROM @Chains
    )
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