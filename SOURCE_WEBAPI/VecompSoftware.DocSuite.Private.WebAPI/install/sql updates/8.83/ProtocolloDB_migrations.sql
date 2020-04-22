/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<DBProtocollo, varcahr(50), DBProtocollo>  --> Settare il nome del DB di protocollo.				        						*
*	<DBPratiche, varcahr(50), DBPratiche>  --> Se esiste il DB di Pratiche settare il nome.					    					*
*	<DBAtti, varcahr(50), DBAtti>			   --> Se esiste il DB di Atti settare il nome.												*
*	<ESISTE_DB_ATTI, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva		*
*	<ESISTE_DB_PRATICHE, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
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
PRINT 'Versionamento database alla 8.83'
GO

EXEC dbo.VersioningDatabase N'8.83',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT N'Modifica SQL Function [dbo].[Fascicles_FX_HasViewableRight]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_HasViewableRight]
(
	@UserName NVARCHAR (256), 
	@Domain NVARCHAR (256), 
	@IdFascicle UNIQUEIDENTIFIER
)
RETURNS BIT
AS
BEGIN
    DECLARE @HasRight AS BIT;
    DECLARE @EmptyRights AS NVARCHAR (20);
    SET @EmptyRights = '00000000000000000000';
    WITH   MySecurityGroups
    AS     (SELECT IdGroup
            FROM   [dbo].[UserSecurityGroups](@UserName, @Domain)),
           MyResponsibleRoles
    AS     (SELECT idRole
            FROM   [dbo].[FascicleRoles]
            WHERE  IdFascicle = @IdFascicle
                   AND RoleAuthorizationType = 0 AND IsMaster = 1)
    SELECT @HasRight = CAST (COUNT(1) AS BIT)
    FROM   [dbo].[Fascicles] AS F
    WHERE  F.IdFascicle = @IdFascicle
           AND (EXISTS (SELECT 1
                        FROM   MySecurityGroups AS SG
                               INNER JOIN [dbo].[RoleGroup] AS RG ON SG.IdGroup = RG.IdGroup 
                                                  INNER JOIN [dbo].[FascicleRoles] AS FR
                               ON FR.IdFascicle = F.IdFascicle AND RG.IdRole = FR.IdRole
                        WHERE  ((F.FascicleType = 4 AND FR.RoleAuthorizationType = 0)
                                OR (F.FascicleType IN (1, 2) AND FR.RoleAuthorizationType = 1)
                                OR (F.FascicleType = 2 AND FR.RoleAuthorizationType = 0 AND IsMaster = 1) 
                                                      OR (F.FascicleType IN (1, 2) AND FR.RoleAuthorizationType = 0 AND IsMaster = 0)
                                                      OR (F.IdContainer is not null and F.FascicleType = 1  AND FR.RoleAuthorizationType = 0 AND IsMaster = 1))
                               AND ((RG.ProtocolRights <> @EmptyRights)
                                    OR (RG.ResolutionRights <> @EmptyRights)
                                    OR (RG.DocumentRights <> @EmptyRights)
                                    OR (RG.DocumentSeriesRights <> @EmptyRights)))
                OR EXISTS (SELECT 1
                           FROM   [dbo].[RoleUser] AS RU
                                  INNER JOIN
                                  [dbo].[CategoryFascicleRights] AS CFR
                                  ON CFR.IdRole = RU.idRole
                                  INNER JOIN
                                  [dbo].[CategoryFascicles] AS CF
                                  ON CFR.IdCategoryFascicle = CF.IdCategoryFascicle
                           WHERE  CF.IdCategory = F.IdCategory
                                  AND RU.Account = @Domain + '\' + @UserName
                                  AND F.FascicleType = 1
                                  AND RU.[Type] IN ('RP', 'SP')
                                  AND ((EXISTS (SELECT 1 FROM   MyResponsibleRoles) 
                                                        AND (RU.idRole IN (SELECT idRole FROM MyResponsibleRoles))) OR (NOT EXISTS (SELECT 1 FROM MyResponsibleRoles))))
                OR 
                                  (NOT exists ( SELECT 1 
                                                        FROM [dbo].[FascicleRoles] FR
                                                        WHERE FR.IdFascicle = F.IdFascicle AND FR.RoleAuthorizationType = 0)
                                               AND exists (
                                                            SELECT 1
                                                            FROM [dbo].[ContainerGroup] CG
                                                            where CG.idContainer = F.IdContainer
                                                            AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
                                                            AND CG.FascicleRights LIKE '__1%'
                                                         )
                                               )
                    );
    RETURN @HasRight;
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
                                  OR Fascicle.MetadataValues LIKE '%' + @MetadataValue + '%')
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
                OR Fascicle.MetadataValues LIKE '%' + @MetadataValue + '%')
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
PRINT N'Alter stored procedure [dbo].[SecurityUsers_Insert]';
GO

ALTER PROCEDURE [dbo].[SecurityUsers_Insert] 
       @Account nvarchar(256), 
       @Description nvarchar(256),
       @UserDomain nvarchar(256),
	   @RegistrationUser nvarchar(256),
       @RegistrationDate datetimeoffset(7),
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
	   @UniqueId uniqueidentifier,
	   @IdGroup int
AS 

DECLARE @LastUsedIdUser INT, @EntityId INT, @SecurityUserId uniqueidentifier

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION SecurityUsersInsert

SELECT top(1) @LastUsedIdUser = IdUser FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[SecurityUsers] ORDER BY idUser DESC
IF(@LastUsedIdUser is null)
BEGIN
	SET @LastUsedIdUser = 0
END

SET @EntityId = @LastUsedIdUser + 1
SET @SecurityUserId = newid()

INSERT INTO  <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[SecurityUsers] ([idUser], [Account], [Description], [idGroup], [RegistrationUser], [RegistrationDate], [UserDomain], [UniqueId])
       VALUES (@EntityId , @Account, @Description, @IdGroup, @RegistrationUser, @RegistrationDate, @UserDomain, @SecurityUserId)

IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN	 
	INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[SecurityUsers] ([idUser], [Account], [Description], [idGroup], [RegistrationUser], [RegistrationDate], [UserDomain], [UniqueId])
       VALUES (@EntityId , @Account, @Description, @IdGroup, @RegistrationUser, @RegistrationDate, @UserDomain, @SecurityUserId)
	END
	
IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 	
	BEGIN	 
	INSERT INTO  <DBPratiche, varchar(50), DBPratiche>.[dbo].[SecurityUsers] ([idUser], [Account], [Description], [idGroup], [RegistrationUser], [RegistrationDate], [UserDomain], [UniqueId])
       VALUES (@EntityId , @Account, @Description, @IdGroup, @RegistrationUser, @RegistrationDate, @UserDomain, @SecurityUserId)
	END

COMMIT TRANSACTION SecurityUsersInsert
END TRY

BEGIN CATCH 
     ROLLBACK TRANSACTION SecurityUsersInsert

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
PRINT N'Alter stored procedure [dbo].[SecurityGroups_Insert]';
GO

ALTER PROCEDURE [dbo].[SecurityGroups_Insert] 
       @GroupName nvarchar(256), 
       @FullIncrementalPath nvarchar(256),
       @LogDescription nvarchar(256),
	   @RegistrationUser nvarchar(256),
       @RegistrationDate datetimeoffset(7),
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
	   @UniqueId uniqueidentifier,
	   @TenantId uniqueidentifier,
	   @IdSecurityGroupTenant int,
	   @AllUsers bit,
	   @idGroupFather int
AS

DECLARE @LastUsedIdGroup INT, @EntityId INT, @SecurityGroupId uniqueidentifier

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION SecurityGroupsInsert

SELECT top(1) @LastUsedIdGroup = IdGroup FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[SecurityGroups] ORDER BY idGroup DESC
IF(@LastUsedIdGroup is null)
BEGIN
	SET @LastUsedIdGroup = 0
END

SET @EntityId = @LastUsedIdGroup + 1
SET @SecurityGroupId = newid()
SET @FullIncrementalPath = @EntityId

IF(@idGroupFather IS NOT NULL)
BEGIN
	SET @FullIncrementalPath = @idGroupFather + '|' + @EntityId
END

INSERT INTO  <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[securitygroups] ([idGroup], [GroupName], [FullIncrementalPath], [idGroupFather], [RegistrationUser], [RegistrationDate], [AllUsers], [UniqueId], [TenantId], [IdSecurityGroupTenant])
       VALUES (@EntityId , @GroupName, @FullIncrementalPath, @idGroupFather, @RegistrationUser, @RegistrationDate, @AllUsers, @SecurityGroupId, @TenantId, @EntityId)

IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN	 
	INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[securitygroups] ([idGroup], [GroupName], [FullIncrementalPath], [idGroupFather], [RegistrationUser], [RegistrationDate], [AllUsers], [UniqueId], [TenantId], [IdSecurityGroupTenant])
       VALUES (@EntityId , @GroupName, @FullIncrementalPath, @idGroupFather, @RegistrationUser, @RegistrationDate, @AllUsers, @SecurityGroupId, @TenantId, @EntityId)
	END
	
IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 	
	BEGIN	 
	INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[securitygroups] ([idGroup], [GroupName], [FullIncrementalPath], [idGroupFather], [RegistrationUser], [RegistrationDate], [AllUsers], [UniqueId], [TenantId], [IdSecurityGroupTenant])
       VALUES (@EntityId , @GroupName, @FullIncrementalPath, @idGroupFather, @RegistrationUser, @RegistrationDate, @AllUsers, @SecurityGroupId, @TenantId, @EntityId)
	END

COMMIT TRANSACTION SecurityGroupsInsert
RETURN @EntityId
END TRY

BEGIN CATCH 
     ROLLBACK TRANSACTION SecurityGroupsInsert

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
PRINT 'Alter stored procedure [dbo].[MassimarioScarto_Update]'
GO

ALTER PROCEDURE [dbo].[MassimarioScarto_Update]
	@IdMassimarioScarto uniqueidentifier, 
	@RegistrationDate datetimeoffset(7),
	@RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256),
	@Status smallint,
	@Name nvarchar(256), 
	@Code smallint, 
	@Note nvarchar(1024), 
	@ConservationPeriod smallint, 
	@StartDate datetimeoffset(7), 
	@EndDate datetimeoffset(7),
	@FakeInsertId uniqueidentifier,
	@Timestamp_Original timestamp
AS
	DECLARE @version smallint
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRY
	BEGIN TRANSACTION MassimarioScartoUpdate
		-- Modifico i valori in MassimarioScarto
		UPDATE [dbo].[MassimariScarto] 
		SET [Name] = @Name, [Code] = @Code, [Note] = @Note, 
		[Status] = @Status, [ConservationPeriod] = @ConservationPeriod,
		[StartDate] = @StartDate, [EndDate] = @EndDate,
		[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser
		WHERE [IdMassimarioScarto] = @IdMassimarioScarto

		--Setto la EndDate verso i figli
		UPDATE [dbo].[MassimariScarto] SET [EndDate] = @EndDate
		WHERE [MassimarioScartoNode].GetAncestor(1) = (SELECT [MassimarioScartoNode]  
		FROM [dbo].[MassimariScarto]
		WHERE [IdMassimarioScarto] = @IdMassimarioScarto) AND ([EndDate] is null OR [EndDate] > @EndDate)
		
		SELECT [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp] FROM [dbo].[MassimariScarto] WHERE [IdMassimarioScarto] = @IdMassimarioScarto	
	COMMIT TRANSACTION MassimarioScartoUpdate
	END TRY

	BEGIN CATCH 
		 ROLLBACK TRANSACTION MassimarioScartoUpdate

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
PRINT 'Alter stored procedure [dbo].[MassimarioScarto_Insert]'
GO

ALTER PROCEDURE [dbo].[MassimarioScarto_Insert] 
	@IdMassimarioScarto uniqueidentifier, 
	@RegistrationDate datetimeoffset(7),
	@RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256),
	@Status smallint,
	@Name nvarchar(256), 
	@Code smallint, 
	@Note nvarchar(1024), 
	@ConservationPeriod smallint, 
	@StartDate datetimeoffset(7), 
	@EndDate datetimeoffset(7),
	@FakeInsertId uniqueidentifier
AS
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRY
	BEGIN TRANSACTION MassimarioScartoInsert

		-- Recupero il parent node
		IF @FakeInsertId IS NOT NULL
			BEGIN
				SELECT @parentNode = [MassimarioScartoNode] FROM [dbo].[MassimariScarto] WHERE [IdMassimarioScarto] = @FakeInsertId
			END
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode] = hierarchyid::GetRoot())
				BEGIN
					SET @parentNode = hierarchyid::GetRoot()
				END	
			END

		-- Recupero il max node in base al parent node
		SELECT @maxNode = MAX([MassimarioScartoNode]) FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode].GetAncestor(1) = @parentNode;

		IF @FakeInsertId IS NOT NULL			
			BEGIN
				SET @node = @parentNode.GetDescendant(@maxNode, NULL)
			END			
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode] = hierarchyid::GetRoot())
				BEGIN
					SET @node = hierarchyid::GetRoot().GetDescendant(@maxNode, NULL)
					PRINT @node.ToString()
				END	
				ELSE
				BEGIN
					SET @node = hierarchyid::GetRoot()
					PRINT @node.ToString()
				END
			END
	
		
		INSERT INTO [dbo].[MassimariScarto]([MassimarioScartoNode],[IdMassimarioScarto],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[Note],[ConservationPeriod],[StartDate],[EndDate]) 
		VALUES (@node, @IdMassimarioScarto, @RegistrationDate, @RegistrationUser, NULL, NULL, @Status, @Name, @Code, @Note, @ConservationPeriod, @StartDate, @EndDate)

		SELECT [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp] FROM [dbo].[MassimariScarto] WHERE [IdMassimarioScarto] = @IdMassimarioScarto

	COMMIT TRANSACTION MassimarioScartoInsert
	END TRY

	BEGIN CATCH 
		 ROLLBACK TRANSACTION MassimarioScartoInsert

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
PRINT N'Alter stored procedure [dbo].[ContainerGroups_Insert]';
GO

ALTER PROCEDURE [dbo].[ContainerGroups_Insert]
	   @idContainer smallint,
	   @Rights char(20),
       @GroupName varchar(255), 
       @ResolutionRights char(20),
       @DocumentRights char(20),
	   @RegistrationUser nvarchar(256),
	   @RegistrationDate datetimeoffset(7),
       @DocumentSeriesRights char(20),
       @idGroup int,
       @DeskRights char(20),
       @UDSRights char(20),
	   @PrivacyLevel int,
	   @FascicleRights char(20)
AS 
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION ContainerGroupInsert

	DECLARE @idContainerGroup uniqueidentifier
	SET @idContainerGroup = newid()

	     INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel], [FascicleRights])
         VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel, @FascicleRights)

		 IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		 BEGIN 
		  INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel], [FascicleRights])
          Values(@idContainer, @Rights, @idGroup, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel, @FascicleRights)
		END
		IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
		BEGIN 
			INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel], [FascicleRights])
			VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel, @FascicleRights)
		END
	
      COMMIT TRANSACTION ContainerGroupInsert
END TRY
BEGIN CATCH 	
	ROLLBACK TRANSACTION ContainerGroupInsert

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
PRINT 'Alter stored procedure [dbo].[Container_Update] ';
GO

ALTER PROCEDURE [dbo].[Container_Update] 
	@idContainer smallint, 
	@Name nvarchar(1000),
	@Note varchar(2000), 
	@DocmLocation smallint,
	@ProtLocation smallint,
	@ReslLocation smallint,
	@isActive tinyint,
	@Massive tinyint,
	@Conservation smallint,
	@RegistrationDate datetimeoffset(7),
    @RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256), 
	@DocumentSeriesAnnexedLocation smallint,
	@DocumentSeriesLocation smallint,
	@DocumentSeriesUnpublishedAnnexedLocation smallint,
	@ProtocolRejection tinyint,
	@ActiveFrom datetime,
	@ActiveTo datetime,
	@idArchive int,
	@Privacy tinyint,
	@HeadingFrontalino nvarchar(511),
	@HeadingLetter nvarchar(256),
	@ProtAttachLocation smallint,
	@idProtocolType smallint,
	@DeskLocation smallint,
	@UDSLocation smallint,
	@UniqueId uniqueidentifier,
	@AutomaticSecurityGroups tinyint,
	@PrefixSecurityGroupName nvarchar(256),
	@TenantId uniqueidentifier,
	@ContainerType smallint,
	@SecurityUserAccount nvarchar(256),
	@SecurityUserDisplayName nvarchar(256),
	@ManageSecureDocument bit,
	@PrivacyLevel int,
	@PrivacyEnabled bit
AS 

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION ContainerUpdate		

	--Aggiornamento contenitore
	UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation, [ManageSecureDocument] = @ManageSecureDocument, [PrivacyLevel] = @PrivacyLevel, [PrivacyEnabled] = @PrivacyEnabled  Where [idContainer] = @idContainer

	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN

		
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation, [ManageSecureDocument] = @ManageSecureDocument, [PrivacyLevel] = @PrivacyLevel, [PrivacyEnabled] = @PrivacyEnabled  Where [idContainer] = @idContainer
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
	UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation, [ManageSecureDocument] = @ManageSecureDocument, [PrivacyLevel] = @PrivacyLevel, [PrivacyEnabled] = @PrivacyEnabled  Where [idContainer] = @idContainer
	END
	COMMIT TRANSACTION ContainerUpdate
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION ContainerUpdate

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
PRINT 'Alter stored procedure [dbo].[Container_Insert]';
GO

ALTER PROCEDURE [dbo].[Container_Insert] 
       @Name nvarchar(1000),
       @Note varchar(2000), 
       @DocmLocation smallint,
       @ProtLocation smallint,
       @ReslLocation smallint,
       @isActive tinyint,
       @Massive tinyint,
       @Conservation smallint,
       @RegistrationDate datetimeoffset(7),
       @RegistrationUser nvarchar(256),
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
       @DocumentSeriesAnnexedLocation smallint,
       @DocumentSeriesLocation smallint,
       @DocumentSeriesUnpublishedAnnexedLocation smallint,
       @ProtocolRejection tinyint,
       @ActiveFrom datetime,
       @ActiveTo datetime,
       @idArchive int,
       @Privacy tinyint,
       @HeadingFrontalino nvarchar(511),
       @HeadingLetter nvarchar(256),
       @ProtAttachLocation smallint,
       @idProtocolType smallint,
       @DeskLocation smallint,
       @UDSLocation smallint,
       @UniqueId uniqueidentifier,
       @AutomaticSecurityGroups tinyint,
       @PrefixSecurityGroupName nvarchar(256),
       @TenantId uniqueidentifier,
       @ContainerType smallint,
	   @SecurityUserAccount nvarchar(256),
	   @SecurityUserDisplayName nvarchar(256),
	   @ManageSecureDocument bit,
	   @PrivacyLevel int,
	   @PrivacyEnabled bit
AS 

       DECLARE @EntityShortId smallint, @LastUsedIdContainer smallint, @RightsFull nvarchar(20), @ResolutionRightsFull nvarchar(20), @DocumentRightsFull nvarchar(20), @DocumentSeriesRightsFull nvarchar(20), 
			   @DeskRightsFull nvarchar(20), @UDSRightsFull nvarchar(20),@RightsIns nvarchar(20), @ResolutionRightsIns nvarchar(20), @DocumentRightsIns nvarchar(20), @DocumentSeriesRightsIns nvarchar(20), 
			   @DeskRightsIns nvarchar(20), @UDSRightsIns nvarchar(20),@RightsVis nvarchar(20), @ResolutionRightsVis nvarchar(20), @DocumentRightsVis nvarchar(20), @DocumentSeriesRightsVis nvarchar(20), 
			   @DeskRightsVis nvarchar(20), @UDSRightsVis nvarchar(20), @SecurityUserName nvarchar(100), @SecurityUserDescription nvarchar(100), @SecurityUserDomain nvarchar(100), @SecurityGroupIdFull int, 
			   @SecurityGroupIdVis int, @SecurityGroupIdIns int, @SecurityGroupName nvarchar(256)


	SELECT  @LastUsedIdContainer = [LastUsedIdContainer] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter]

       --Ad oggi è implementata solo la gestione dei diritti degli archivi,
       --Per ottenere un corretto funzionamento con le altre UD occorre implementarne la gestione
       --diritti protocollo

       SET @RightsFull = 
             CASE 
                    WHEN @ContainerType = 1 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @RightsIns = 
             CASE 
                    WHEN @ContainerType = 1 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @RightsVis = 
             CASE 
                    WHEN @ContainerType = 1 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

             --diritti atti

       SET @ResolutionRightsFull = 
             CASE 
                    WHEN @ContainerType = 2 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @ResolutionRightsIns = 
             CASE 
                    WHEN @ContainerType = 2 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @ResolutionRightsVis = 
             CASE 
                    WHEN @ContainerType = 2 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

             --diritti pratiche

       SET @DocumentRightsFull = 
             CASE 
                    WHEN @ContainerType = 4 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentRightsIns = 
             CASE 
                    WHEN @ContainerType = 4 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentRightsVis = 
             CASE 
                    WHEN @ContainerType = 4 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END           

             --diritti serie documentali

       SET @DocumentSeriesRightsFull = 
             CASE 
                    WHEN @ContainerType = 8 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentSeriesRightsIns = 
             CASE 
                    WHEN @ContainerType = 8 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentSeriesRightsVis = 
             CASE 
                    WHEN @ContainerType = 8 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END    

             --diritti tavoli

       SET @DeskRightsFull = 
             CASE 
                    WHEN @ContainerType = 16 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DeskRightsIns = 
             CASE 
                    WHEN @ContainerType = 16 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DeskRightsVis = 
             CASE 
                    WHEN @ContainerType = 16 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END    

             -- diritti UDS
             
       SET @UDSRightsFull = 
             CASE 
                    WHEN @ContainerType = 32 THEN '11111111000000000000'
                    ELSE '00000000000000000000'
             END

       SET @UDSRightsIns = 
             CASE 
                    WHEN @ContainerType = 32 THEN '11111000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @UDSRightsVis = 
             CASE 
                    WHEN @ContainerType = 32 THEN '00110000000000000000'
                    ELSE '00000000000000000000'
             END    

	   IF(@SecurityUserAccount IS NOT NULL AND @SecurityUserAccount != '')
			BEGIN			
			SELECT TOP 1 @SecurityUserDomain = Value FROM dbo.SplitString(@SecurityUserAccount, '\')
			SELECT TOP 1 @SecurityUserName = Value FROM (SELECT Value, row_number() over(order by (select null)) as rownum FROM dbo.SplitString(@SecurityUserAccount, '\')) T where T.rownum > 1 AND T.rownum <= 2
			END

       SET @EntityShortId = @LastUsedIdContainer + 1

       SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
       BEGIN TRY
       BEGIN TRANSACTION ContainerInsert
       UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter] SET [LastUsedidContainer] = @EntityShortId

       --Inserimento contenitore
       INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
             [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
             [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
             [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId], [ManageSecureDocument], [PrivacyLevel], [PrivacyEnabled]) 
             
       VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation,   @RegistrationUser, @RegistrationDate, @DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
             @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId, @ManageSecureDocument, @PrivacyLevel, @PrivacyEnabled)

	   IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
       BEGIN
             update  <DBAtti, varchar(50), DBAtti>.[dbo].[Parameter] set [LastUsedidContainer] = @EntityShortId
             --Inserimento contenitore
             INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
                    [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
                    [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
                    [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId], [ManageSecureDocument], [PrivacyLevel], [PrivacyEnabled])  
             
             VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation,@RegistrationUser, @RegistrationDate, 
					@DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
                    @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId, @ManageSecureDocument, @PrivacyLevel, @PrivacyEnabled)

       END

	   IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
       BEGIN 
             update <DBPratiche, varchar(50), DBPratiche>.[dbo].[Parameter] set [LastUsedidContainer] = @EntityShortId

             --Inserimento contenitore
             INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
                    [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
                    [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
                    [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId], [ManageSecureDocument], [PrivacyLevel], [PrivacyEnabled]) 
             
             VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation, @RegistrationUser, @RegistrationDate, 
					@DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
                    @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId, @ManageSecureDocument, @PrivacyLevel, @PrivacyEnabled)

        END

	   IF(@AutomaticSecurityGroups = 1)
       BEGIN 
             --inserimento  gruppo con tutti i diritti
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_full'
             EXEC @SecurityGroupIdFull = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsFull, @SecurityGroupName, @ResolutionRightsFull, @DocumentRightsFull, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsFull, @SecurityGroupIdFull, @DeskRightsFull, @UDSRightsFull, @PrivacyLevel, '00000000000000000000'
             
			 --inserimento  gruppo con diritti di inserimento
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_ins'
             EXEC @SecurityGroupIdIns = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
			 EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsIns, @SecurityGroupName, @ResolutionRightsIns, @DocumentRightsIns, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsIns, @SecurityGroupIdIns, @DeskRightsIns, @UDSRightsIns, @PrivacyLevel, '00000000000000000000'
             
			 --inserimento gruppo con diritti di visualizzazione
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_vis'
             EXEC @SecurityGroupIdVis = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsVis, @SecurityGroupName, @ResolutionRightsVis, @DocumentRightsVis, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsVis, @SecurityGroupIdVis, @DeskRightsVis, @UDSRightsVis, @PrivacyLevel, '00000000000000000000'
       
			IF(@SecurityUserAccount IS NOT NULL AND @SecurityUserDomain IS NOT NULL AND @SecurityUserName IS NOT NULL)
			BEGIN			

			EXEC [dbo].[SecurityUsers_Insert] @SecurityUserName, @SecurityUserDisplayName, @SecurityUserDomain, @RegistrationUser, @RegistrationDate, null, null, null, @SecurityGroupIdFull 

			END
	   END


       COMMIT TRANSACTION ContainerInsert
       SELECT @EntityShortId as idContainer
       END TRY

       BEGIN CATCH 
          ROLLBACK TRANSACTION ContainerInsert

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
PRINT N'Alter stored procedure [dbo].[Container_Delete]';
GO

ALTER PROCEDURE [dbo].[Container_Delete] 
	@idContainer smallint, 
	@DocmLocation smallint,
	@ProtLocation smallint,
	@ReslLocation smallint,
	@idProtocolType smallint,
	@DeskLocation smallint,
	@DocumentSeriesAnnexedLocation smallint,
	@DocumentSeriesLocation smallint,
	@DocumentSeriesUnpublishedAnnexedLocation smallint,
	@ProtAttachLocation smallint,
	@UDSLocation smallint
AS 
	DECLARE @LastChangedDate datetimeoffset(7)

	SET @LastChangedDate = GETDATE()

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION ContainerDelete	

	--Cancellazione logica contenitore
		UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [idContainer] = @idContainer
	
	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Container]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [idContainer] = @idContainer
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [idContainer] = @idContainer
	END
	COMMIT TRANSACTION ContainerDelete
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION ContainerDelete

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
PRINT 'Alter stored procedure [dbo].[Category_Update]';
GO

ALTER PROCEDURE [dbo].[Category_Update] 
	@idCategory smallint, 
    @Name nvarchar(100),
	@idParent smallint, 
    @isActive tinyint,
    @Code smallint,
    @FullIncrementalPath nvarchar(256),
	@FullCode nvarchar(256), 
    @RegistrationUser nvarchar(256),
	@RegistrationDate datetimeoffset(7),
    @LastChangedUser nvarchar(256), 
	@LastChangedDate datetimeoffset(7),
    @UniqueId uniqueidentifier,
	@IdMassimarioScarto uniqueidentifier,
	@IdCategorySchema uniqueidentifier,
	@StartDate datetimeoffset(7),
	@EndDate datetimeoffset(7),
	@IdMetadataRepository uniqueidentifier
AS 

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE 	
	BEGIN TRY
	BEGIN TRANSACTION CategoryUpdate	

	--Aggiornamento classificatore in Protocollo
	UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category] SET [Name] = @Name, 
		[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
		[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
		[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
		[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository Where [idCategory] = @idCategory
    SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]

	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		--Aggiornamento classificatore in Atti
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Category] SET [Name] = @Name, 
			[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
			[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
			[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
			[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBAtti, varchar(50), DBAtti>.[dbo].[Category]
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		--Aggiornamento classificatore in Pratiche
		UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category] SET [Name] = @Name, 
			[idParent] = @idParent, [isActive] = @isActive, [Code] = @Code, [FullIncrementalPath] = @FullIncrementalPath, [FullCode] = @FullCode, 
			[RegistrationUser] = @RegistrationUser, [RegistrationDate] = @RegistrationDate, [LastChangedUser] = @LastChangedUser, [LastChangedDate] = @LastChangedDate, 
			[UniqueId] = @UniqueId,	[IdMassimarioScarto] = @IdMassimarioScarto, [IdCategorySchema] = @IdCategorySchema,
			[StartDate] = @StartDate, [EndDate] = @EndDate, [IdMetadataRepository] = @IdMetadataRepository Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]
	END
	COMMIT TRANSACTION CategoryUpdate
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION CategoryUpdate

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
PRINT 'Alter stored procedure [dbo].[Category_Insert]';
GO

ALTER PROCEDURE [dbo].[Category_Insert] 
	   @idCategory smallint,
       @Name nvarchar(100),
       @idParent smallint, 
       @isActive tinyint,
       @Code smallint, 
	   @FullIncrementalPath nvarchar(256),
	   @FullCode nvarchar(256), 
       @RegistrationUser nvarchar(256),
	   @RegistrationDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
	   @LastChangedDate datetimeoffset(7),
       @UniqueId uniqueidentifier,
	   @IdMassimarioScarto uniqueidentifier,
	   @IdCategorySchema uniqueidentifier,
	   @StartDate datetimeoffset(7),
	   @EndDate datetimeoffset(7),
	   @IdMetadataRepository uniqueidentifier
AS 

	DECLARE @EntityShortId smallint, @LastUsedIdCategory smallint, @ParentFullCode nvarchar(255)
	SELECT @LastUsedIdCategory = [LastUsedIdCategory] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter]
	SET @EntityShortId = @LastUsedIdCategory + 1
	SET @FullIncrementalPath = @EntityShortId
	SET @FullCode =  RIGHT('0000' + cast(@Code as nvarchar(4)), 4)

	IF(@idParent is not null)
		BEGIN
		SET @ParentFullCode = (SELECT TOP 1 FullCode FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category] where idCategory = @idParent)
		SET @FullIncrementalPath = (SELECT TOP 1 FullIncrementalPath FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category] where idCategory = @idParent) + '|' + cast(@EntityShortId as nvarchar(256))
		SET @FullCode = @ParentFullCode + '|' + @FullCode
		END
	

    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION CategoryInsert
	
	--Inserimento classificatore in db Protocollo
	UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
	INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
             [LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository]) 
             
    VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate, 
		   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository)
    
	SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]

	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
	    --Inserimento classificatore in db Atti
       	UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
		INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
				[LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository]) 
             
		VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate, 
			   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository)

		SELECT [FullSearchComputed] FROM <DBAtti, varchar(50), DBAtti>.[dbo].[Category]
    END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
    BEGIN 
        --Inserimento classificatore in db Pratiche
       	UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Parameter] SET [LastUsedIdCategory] = @EntityShortId
		INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]([idCategory], [Name], [idParent], [isActive], [Code], [FullIncrementalPath], [FullCode], [RegistrationUser], [RegistrationDate],
				[LastChangedUser], [LastChangedDate], [UniqueId], [IdMassimarioScarto], [IdCategorySchema], [StartDate], [EndDate], [IdMetadataRepository]) 
             
		VALUES(@EntityShortId, @Name, @idParent, @isActive, @Code, @FullIncrementalPath, @FullCode, @RegistrationUser, @RegistrationDate, @LastChangedUser, @LastChangedDate,  
			   @UniqueId, @IdMassimarioScarto, @IdCategorySchema, @StartDate, @EndDate, @IdMetadataRepository)

		SELECT [FullSearchComputed] FROM <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]
    END
 
	COMMIT TRANSACTION CategoryInsert
	SELECT @EntityShortId as idCategory
	END TRY

	BEGIN CATCH 	
		ROLLBACK TRANSACTION CategoryInsert

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
PRINT N'Alter stored procedure [dbo].[Category_Delete]';
GO

ALTER PROCEDURE [dbo].[Category_Delete] 
	@idCategory smallint, 
	@idParent smallint, 
	@IdMassimarioScarto uniqueidentifier,
	@IdCategorySchema uniqueidentifier,
	@IdMetadataRepository uniqueidentifier
AS 
	DECLARE @LastChangedDate datetimeoffset(7) = GETDATE()

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION CategoryDelete		

	--Cancellazione logica contenitore
		UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]  SET [LastChangedDate] = @LastChangedDate, [isActive] = 0 Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category]
	
	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Category]  SET [LastChangedDate] = @LastChangedDate, [isActive] = 0 Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBAtti, varchar(50), DBAtti>.[dbo].[Category]
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]  SET [LastChangedDate] = @LastChangedDate, [isActive] = 0 Where [idCategory] = @idCategory
		SELECT [FullSearchComputed] FROM <DBPratiche, varchar(50), DBPratiche>.[dbo].[Category]
	END
	COMMIT TRANSACTION CategoryDelete
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION CategoryDelete

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