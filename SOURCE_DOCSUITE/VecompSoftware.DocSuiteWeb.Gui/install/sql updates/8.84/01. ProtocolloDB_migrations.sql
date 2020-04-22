/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<UTENTE_DEFAULT, varchar(256),>	--> Settare il nome dell'utente.																	*
*	<PARAMETRO_FASCICLE_CONTAINER_ENABLED, varchar(4), False> --> Impostare False se non presente, Impostare True se presente e settare la chiave successiva		*
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
PRINT 'Versionamento database alla 8.84'
GO

EXEC dbo.VersioningDatabase N'8.84',N'DSW Version','MigrationDate'
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
PRINT N'ALTER [dbo].[ProtocolUsers] ADD COLUMN [Note]';
GO

ALTER TABLE [dbo].[ProtocolUsers] ADD Note nvarchar(4000) null
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
PRINT N'Migrazione cancellazione IdContainer dalla tabella CategoryFascicleRights';
GO

DECLARE @idSpecialRole SMALLINT
SELECT @idSpecialRole = idRole FROM [dbo].[Role]
WHERE UniqueId='00000000-0000-0000-0000-000000000000'

IF( (CAST('<PARAMETRO_FASCICLE_CONTAINER_ENABLED, varchar(4), false>'  AS BIT) = CAST('True' AS BIT)))
BEGIN
	INSERT INTO [dbo].[CategoryFascicleRights] ([IdCategoryFascicleRight]
		  ,[IdCategoryFascicle]
		  ,[IdRole]
		  ,[IdContainer]
		  ,[RegistrationUser]
		  ,[RegistrationDate])
	SELECT NEWID(), T.IdCategoryFascicle, @idSpecialRole, NULL, '<UTENTE_DEFAULT, varchar(256),>', GETDATE()
	FROM
	(
		SELECT IdCategoryFascicle
		FROM [dbo].[CategoryFascicleRights] CFR
		WHERE IdContainer IS NOT NULL AND NOT EXISTS
		(
			SELECT TOP 1 1
			FROM [dbo].[CategoryFascicleRights] 
			WHERE IdCategoryFascicle = CFR.IdCategoryFascicle 
			AND IdRole IS NOT NULL
		)
		GROUP BY IdCategoryFascicle
	) AS T

	DELETE FROM [dbo].[CategoryFascicleRights] 
	WHERE IdContainer IS NOT NULL
END

INSERT INTO [dbo].[CategoryFascicleRights] ([IdCategoryFascicleRight]
      ,[IdCategoryFascicle]
      ,[IdRole]
      ,[IdContainer]
      ,[RegistrationUser]
      ,[RegistrationDate])
SELECT NEWID(), CF.IdCategoryFascicle, CFR.IdRole, NULL, '<UTENTE_DEFAULT, varchar(256),>', GETDATE()
FROM [dbo].[CategoryFascicles] CF
INNER JOIN [dbo].[CategoryFascicles] CF2 on CF2.IdCategory = CF.IdCategory AND CF2.FascicleType = 1
INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdCategoryFascicle = CF2.IdCategoryFascicle
WHERE CF.FascicleType=2
AND NOT EXISTS 
(
    SELECT TOP 1 1 
    FROM [dbo].[CategoryFascicleRights] 
    WHERE IdCategoryFascicle = CF.IdCategoryFascicle
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
PRINT N'Alter sql function [webapiprivate].[Category_FX_FindCategories]';
GO

ALTER FUNCTION [webapiprivate].[Category_FX_FindCategories]
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@Name nvarchar(256),
	@LoadRoot bit,
	@ParentId smallint,
	@ParentAllDescendants bit,
	@FullCode nvarchar(256),
	@FascicleFilterEnabled bit,
	@FascicleType smallint,	
	@HasFascicleInsertRights bit,
	@Manager nvarchar(256),
	@Secretary nvarchar(256),
	@Role smallint,
	@Container smallint
)
RETURNS TABLE 
AS
RETURN 
(
	WITH FoundCategories AS (
	SELECT DISTINCT C.idCategory 
	FROM [dbo].[Category] C 
		LEFT OUTER JOIN [CategoryFascicles] CF ON C.idCategory = CF.idCategory 
		LEFT OUTER JOIN [CategoryFascicleRights] CFR ON CFR.IdCategoryFascicle = CF.IdCategoryFascicle 
	WHERE
		IsActive = 1 
		AND StartDate <= GETDATE() 
		AND 
		(
			EndDate IS NULL 
			OR EndDate > GETDATE()
		)
		AND 
		(
			@LoadRoot = 1 
			AND C.idParent IS NULL
		)
		OR 
		(
			(
				@LoadRoot IS NULL 
				OR @LoadRoot = 0
			) 
			AND 
			(
				@ParentId IS NULL 
				OR 
				(
					(
						(
							(
								@ParentAllDescendants IS NULL 
								OR @ParentAllDescendants = 0
							) 
							AND C.idParent = @ParentId
						) 
						OR 
						(
							@ParentAllDescendants = 1 
							AND C.FullIncrementalPath LIKE CAST(@ParentId AS NVARCHAR) + '|%'
						)
					) 
				)
			)
			AND 
			(
				@FullCode IS NULL 
				OR C.FullCode = @FullCode
			)
			AND 
			(
				@Name IS NULL 
				OR C.Name LIKE '%' + @Name + '%'
			)
			AND 
			(
				(@FascicleFilterEnabled IS NULL OR @FascicleFilterEnabled = 0) 
				OR 
				(
					@FascicleFilterEnabled = 1 
					AND 
					(
						(
							@HasFascicleInsertRights IS NULL 
							OR @HasFascicleInsertRights = 0 
							OR @FascicleType NOT IN (1,2)
						) 
						OR 
						(
							@HasFascicleInsertRights = 1 
							AND @FascicleType IN (1,2) 
							AND 
							(
								EXISTS 
								(
									SELECT 1 
									FROM [dbo].[RoleUser] RU 
									WHERE
										RU.idRole = CFR.IdRole 
										AND RU.Account = @Domain + '\' + @UserName
										AND Type IN ('RP','SP')
								)
								OR EXISTS 
								(
									SELECT 1 
									FROM SecurityGroups SG 
										INNER JOIN RoleGroup RG ON RG.idGroup = SG.idGroup 
									WHERE
										RG.idRole = CFR.IdRole 
										AND SG.AllUsers = 1 
								)
							)
						)
					)
					AND 
					(
						@FascicleType IS NULL 
						OR 
						(
							CF.IdCategoryFascicle IS NOT NULL 
							AND CF.FascicleType = @FascicleType
						)
					)
					AND 
					(
						@Manager IS NULL 
						OR EXISTS 
						(
							SELECT 1 
							FROM [dbo].[RoleUser] RU 
							WHERE
								CFR.idRole = RU.idRole 
								AND RU.Account = @Manager 
								AND Type = 'RP' 
						)
					)
					AND 
					(
						@Secretary IS NULL 
						OR EXISTS 
						(
							SELECT 1 
							FROM [dbo].[RoleUser] RU 
							WHERE
								CFR.idRole = RU.idRole 
								AND RU.Account = @Secretary 
								AND Type = 'SP'
						)
					)
					AND 
					(
						@Role IS NULL 
						OR 
						(
							CFR.IdCategoryFascicleRight IS NOT NULL 
							AND CFR.IdRole = @Role
						)
					)
					AND 
					(
						@Container IS NULL 
						OR 
						(
							EXISTS 
							(
								SELECT 1 
								FROM CategoryFascicleRights CFR2 
								WHERE
									CFR2.IdCategoryFascicle = CF.IdCategoryFascicle 
									AND CFR2.IdContainer = @Container 
							)
							OR NOT EXISTS 
							(
								SELECT 1 
								FROM CategoryFascicleRights CFR2 
								WHERE
									CFR2.IdCategoryFascicle = CF.IdCategoryFascicle 
									AND CFR2.IdContainer IS NOT NULL 
							)
						)
					)
				)
			)
		)
	)

	SELECT
	[Category].[idCategory] AS IdCategory,
	[Category].[Name],
	[Category].[isActive],
	[Category].[Code],
	[Category].[FullIncrementalPath],
	[Category].[FullCode],
	[Category].[RegistrationDate],
	[Category].[RegistrationUser],
	[Category].[UniqueId],
	[Category].[StartDate],
	[Category].[EndDate],
	[CategoryParent].[idCategory] AS CategoryParent_IdCategory,
	(
		SELECT TOP 1 CAST(1 AS BIT) 
		FROM Category C_TMP 
		WHERE
			C_TMP.idParent = Category.idCategory 
	)
	AS HasChildren,
	(
		CASE			 
			WHEN @FascicleType IS NULL THEN CAST(0 AS BIT) 
			WHEN (@FascicleType = 1 OR @FascicleType = 2) THEN
			( 
				SELECT DISTINCT CAST(1 AS BIT) 
				FROM [dbo].[CategoryFascicles] CF 
					INNER JOIN [dbo].[CategoryFascicleRights] CFR ON CFR.IdCategoryFascicle = CF.IdCategoryFascicle 
				WHERE
					CF.idCategory = Category.idCategory 
					AND 
					(
						EXISTS 
						(
							SELECT 1 
							FROM [dbo].[RoleUser] RU 
							WHERE
								RU.idRole = CFR.IdRole 
								AND 
								(
									RU.Account = @Domain + '\' + @UserName
									AND Type IN ('RP','SP')
								)
						)
						OR EXISTS 
						(
							SELECT 1 
							FROM SecurityGroups SG 
								INNER JOIN RoleGroup RG ON RG.idGroup = SG.idGroup 
							WHERE
								RG.idRole = CFR.IdRole 
								AND SG.AllUsers = 1 
						)
					)
			)
            ELSE CAST(0 AS BIT) 
		END
	)
	AS HasFascicleDefinition 
FROM
	Category 
	INNER JOIN FoundCategories FC ON FC.idCategory = Category.idCategory 
	LEFT OUTER JOIN Category CategoryParent ON CategoryParent.idCategory = Category.idParent 
WHERE
	Category.isActive = 1
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