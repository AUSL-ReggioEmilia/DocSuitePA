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

PRINT 'Creo la tabella MassimariScarto'
GO

CREATE TABLE [dbo].[MassimariScarto](
	[MassimarioScartoNode] [hierarchyid] NOT NULL,
	[IdMassimarioScarto] [uniqueidentifier] NOT NULL,
	[MassimarioScartoPath]  AS ([MassimarioScartoNode].[ToString]()) PERSISTED,
	[MassimarioScartoParentNode]  AS ([MassimarioScartoNode].[GetAncestor]((1))) PERSISTED,
	[MassimarioScartoLevel]  AS ([MassimarioScartoNode].[GetLevel]()) PERSISTED,
	[Name] [nvarchar](256) NOT NULL,
	[Code] [smallint] NULL,
	[Note] [nvarchar](1024) NULL,
	[ConservationPeriod] [smallint] NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,
	[Status] [smallint] NOT NULL,
	[FakeInsertId] [uniqueidentifier] NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[MassimarioScartoParentPath]  AS ([MassimarioScartoNode].[GetAncestor]((1)).ToString()) PERSISTED,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_MassimariScarto] PRIMARY KEY NONCLUSTERED 
(
	[MassimarioScartoNode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_MassimariScarto] UNIQUE NONCLUSTERED 
(
	[IdMassimarioScarto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_MassimariScarto_RegistationDate]
    ON [dbo].[MassimariScarto]([RegistrationDate] ASC);
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

PRINT 'Creo la function GetFullCodeFromMassimarioScarto'
GO

CREATE FUNCTION GetFullCodeFromMassimarioScarto 
(
	@massimarioId as uniqueidentifier
)
RETURNS nvarchar(256)
AS
BEGIN
	declare @fullCode nvarchar(256)

	SELECT @fullCode = CASE
		WHEN P.Code IS NULL THEN CAST(M.Code as nvarchar(256))
		ELSE CAST(P.Code as nvarchar(256)) + '|' + CAST(M.Code as nvarchar(256)) 
	   END
	FROM 
	[dbo].[MassimariScarto] M
	LEFT OUTER JOIN [dbo].[MassimariScarto] P on P.MassimarioScartoNode = M.MassimarioScartoParentNode
	WHERE M.IdMassimarioScarto = @massimarioId

	RETURN @fullCode
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

PRINT 'Aggiungo la colonna FullCode in MassimariScarto'
GO

ALTER TABLE [dbo].[MassimariScarto] ADD [FullCode] AS ([dbo].[GetFullCodeFromMassimarioScarto](IdMassimarioScarto))
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

PRINT 'Creo la tabella MassimariScartoSchema'
GO

CREATE TABLE [dbo].[MassimariScartoSchema](
	[IdMassimarioScartoSchema] [uniqueidentifier] NOT NULL,
	[IdMassimarioScarto] [uniqueidentifier] NOT NULL,
	[Version] [smallint] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_MassimariScartoSchema] PRIMARY KEY NONCLUSTERED 
(
	[IdMassimarioScartoSchema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_MassimariScartoSchema] UNIQUE NONCLUSTERED 
(
	[IdMassimarioScartoSchema] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_MassimariScartoSchema_RegistationDate]
    ON [dbo].[MassimariScartoSchema]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[MassimariScartoSchema]  WITH CHECK ADD  CONSTRAINT [FK_MassimariScartoSchema_MassimariScarto] FOREIGN KEY([IdMassimarioScarto])
REFERENCES [dbo].[MassimariScarto] ([IdMassimarioScarto])
GO

ALTER TABLE [dbo].[MassimariScartoSchema] CHECK CONSTRAINT [FK_MassimariScartoSchema_MassimariScarto]
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

PRINT 'Aggiungo la SP MassimarioScarto_Insert'
GO

CREATE PROCEDURE [dbo].[MassimarioScarto_Insert] 
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
BEGIN
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION

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
	COMMIT
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

PRINT 'Aggiungo la SP MassimarioScarto_Update'
GO

CREATE PROCEDURE [dbo].[MassimarioScarto_Update]
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
BEGIN
	DECLARE @version smallint
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION
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
	COMMIT
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

PRINT 'Aggiungo la Function MassimarioScarto_RootChildren'
GO

CREATE FUNCTION [dbo].[MassimarioScarto_RootChildren]
(	
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(	 
	SELECT distinct [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp]
	FROM [dbo].[MassimariScarto]
	WHERE [MassimarioScartoNode].GetAncestor(1) = (SELECT [MassimarioScartoNode]  
	FROM [dbo].[MassimariScarto]
	WHERE [IdMassimarioScarto] = (SELECT [IdMassimarioScarto] FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode] = hierarchyid::GetRoot()))
	AND ((@includeLogicalDelete = 0 AND ([EndDate] is null OR [EndDate] > GETDATE()) AND [Status] = 1) OR (@includeLogicalDelete = 1 AND [Status] in (0,1)))
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

PRINT 'Aggiungo la Function MassimarioScarto_GetMassimariFull'
GO

CREATE FUNCTION [dbo].[MassimarioScarto_GetMassimariFull]
(	
	@name nvarchar(256),
	@fullCode nvarchar(256) = NULL,
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT distinct M1.[MassimarioScartoNode],M1.[IdMassimarioScarto] as UniqueId,M1.[RegistrationDate],M1.[RegistrationUser],M1.[LastChangedDate],M1.[LastChangedUser],M1.[Status],
		M1.[Name],M1.[Code],M1.[FullCode],M1.[Note],M1.[ConservationPeriod],M1.[StartDate],M1.[EndDate],M1.[MassimarioScartoPath],M1.[MassimarioScartoLevel],M1.[MassimarioScartoParentPath],M1.[FakeInsertId],M1.[Timestamp]
    FROM (SELECT * FROM [dbo].[MassimariScarto]
			WHERE (@name IS NOT NULL AND @name != '' AND [Name] like '%'+@name+'%')
			OR (@fullCode IS NOT NULL AND [FullCode] = @fullCode)) M2,
    [dbo].[MassimariScarto] M1
    WHERE 
	[M2].[MassimarioScartoNode].IsDescendantOf(M1.MassimarioScartoNode) = 1
	AND M1.MassimarioScartoNode != hierarchyid::GetRoot()
	AND ((@includeLogicalDelete = 0 AND (M1.[EndDate] is null OR M1.[EndDate] > GETDATE()) AND M1.[Status] = 1) OR (@includeLogicalDelete = 1 AND M1.[Status] in (0,1)))
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

PRINT 'Aggiungo la Function MassimarioScarto_AllChildrenByParent'
GO

CREATE FUNCTION [dbo].[MassimarioScarto_AllChildrenByParent]
(	
	@parentId uniqueidentifier,
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(	 
	SELECT distinct [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp]
	FROM [dbo].[MassimariScarto]
	WHERE [MassimarioScartoNode].GetAncestor(1) = (SELECT [MassimarioScartoNode]  
	FROM [dbo].[MassimariScarto]
	WHERE [IdMassimarioScarto] = @parentId)
	AND ((@includeLogicalDelete = 0 AND ([EndDate] is null OR [EndDate] > GETDATE()) AND [Status] = 1) OR (@includeLogicalDelete = 1 AND [Status] in (0,1)))
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

PRINT 'Aggiungo la Function MassimarioScarto_GetMassimari'
GO

CREATE FUNCTION [dbo].[MassimarioScarto_GetMassimari]
(	
	@name nvarchar(256),
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT * FROM [dbo].[MassimarioScarto_GetMassimariFull] (@name, NULL, @includeLogicalDelete)
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

PRINT 'Inserisco il valore Root di default'
GO


DECLARE @IdMassimarioScarto uniqueidentifier
DECLARE @RegistrationDate datetimeoffset(7)
DECLARE @RegistrationUser nvarchar(256)
DECLARE @LastChangedDate datetimeoffset(7)
DECLARE @LastChangedUser nvarchar(256)
DECLARE @Status smallint
DECLARE @Name nvarchar(256)
DECLARE @Code smallint
DECLARE @Note nvarchar(1024)
DECLARE @ConservationPeriod smallint
DECLARE @StartDate datetimeoffset(7)
DECLARE @EndDate datetimeoffset(7)
DECLARE @FakeInsertId uniqueidentifier

SET @IdMassimarioScarto=NEWID()
SET @Status=1
SET @Name=N'Root'
SET @StartDate=GETDATE()
SET @RegistrationUser=N'anonymous_api'
SET @RegistrationDate=GETDATE()

EXECUTE [dbo].[MassimarioScarto_Insert] 
   @IdMassimarioScarto
  ,@RegistrationDate
  ,@RegistrationUser
  ,@LastChangedDate
  ,@LastChangedUser
  ,@Status
  ,@Name
  ,@Code
  ,@Note
  ,@ConservationPeriod
  ,@StartDate
  ,@EndDate
  ,@FakeInsertId
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
PRINT 'CREATE FUNCTION [dbo].[SplitString]'
GO

CREATE FUNCTION [dbo].[SplitString]
    (
        @List NVARCHAR(MAX),
        @Delim NVARCHAR(10)
    )
    RETURNS TABLE
    AS
        RETURN ( SELECT [Value] FROM 
          ( 
            SELECT 
              [Value] = LTRIM(RTRIM(SUBSTRING(@List, [Number],
              CHARINDEX(@Delim, @List + @Delim, [Number]) - [Number])))
            FROM (SELECT Number = ROW_NUMBER() OVER (ORDER BY name)
              FROM sys.all_objects) AS x
              WHERE Number <= LEN(@List)
              AND SUBSTRING(@Delim + @List, [Number], LEN(@Delim)) = @Delim
          ) AS y
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
PRINT 'Modifica Function [AvailableFasciclesFromProtocol]'
GO

ALTER FUNCTION [dbo].[AvailableFasciclesFromProtocol]
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
		LEFT JOIN Category C on C.IdCategory = P.IdCategoryAPI
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 1 AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = P.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 1 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 1 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FP.IdFascicleProtocol IS NULL
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
PRINT 'Modifica Function [AvailableFasciclesFromResolution]'
GO

ALTER FUNCTION [dbo].[AvailableFasciclesFromResolution]
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
		LEFT JOIN Category C on C.IdCategory = R.IdCategoryAPI
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 2 AND CF.FascicleType in (1, 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = R.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 1 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 1 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FR.IdFascicleResolution IS NULL
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