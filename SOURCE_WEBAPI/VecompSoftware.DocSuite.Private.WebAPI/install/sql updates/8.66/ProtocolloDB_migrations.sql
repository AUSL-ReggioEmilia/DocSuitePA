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
PRINT N'Modifica SQL Function [Dossiers_FX_GetDossierContacts]' 
GO

ALTER FUNCTION [webapiprivate].[Dossiers_FX_GetDossierContacts]
(
	@IdDossier uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(

WITH tblChild AS
(
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, 1 as IsSelected
        FROM [dbo].[DossierContacts] DC 
		INNER JOIN [dbo].[Contact] C ON C.Incremental = DC.IdContact AND DC.IdDossier = @IdDossier
		GROUP BY C.UniqueId, C.Incremental, C.IncrementalFather, C.IdContactType, C.[Description]
),
Results As
(SELECT UniqueId, Incremental as IdContact, IdContactType AS ContactType, [Description], 
		(SELECT TOP 1 I.UniqueId FROM tblChild I WHERE I.Incremental = tblChild.IncrementalFather) AS UniqueIdFather, IsSelected
	FROM tblChild
)
SELECT UniqueId, IdContact, ContactType, [Description], UniqueIdFather, CAST(MAX(IsSelected) as bit) AS IsSelected
FROM Results
GROUP BY UniqueId, IdContact, ContactType, [Description], UniqueIdFather
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
PRINT N'Modifica SQL Function [Dossiers_FX_AuthorizedDossiers]' 
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
	@StartDateFrom datetimeoffset,
	@StartDateTo datetimeoffset,
	@EndDateFrom datetimeoffset,
	@EndDateTo datetimeoffset
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
		MyDossiers AS (
			select * from
			(select Dossier.IdDossier, row_number() over(order by Dossier.Year, Dossier.Number) as rownum 
			 FROM dbo.Dossiers Dossier
			 inner join dbo.Container C on Dossier.IdContainer = C.idContainer
			 inner join dbo.DossierRoles DR on Dossier.IdDossier = DR.IdDossier and DR.RoleAuthorizationType = 0
			 inner join dbo.Role R on DR.IdRole = R.idRole
			 WHERE  (
			   exists (select top 1 CG.IdContainerGroup
						from dbo.ContainerGroup CG
						INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
						where CG.IdContainer = Dossier.IdContainer and C_MSG.IdGroup is not null
							  and CG.DocumentRights like '___1%')
			   or exists (select top 1 RG.idRole
						from dbo.RoleGroup RG
						INNER JOIN Role R on RG.idRole = R.idRole
						INNER JOIN MySecurityGroups MSG on RG.idGroup = MSG.idGroup
						where R.idRole = DR.IdRole and MSG.IdGroup is not null
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
where exists (select * from MyDossiers where D.IdDossier = MyDossiers.IdDossier)
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

PRINT N'Creata stored procedure [DossierFolder_Insert]';
GO

CREATE PROCEDURE [dbo].[DossierFolder_Insert] 
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
BEGIN
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION

		-- Recupero il parent node
		IF @ParentInsertId IS NOT NULL
			BEGIN
				SELECT @parentNode = [DossierFolderNode] FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @ParentInsertId
			END
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[DossierFolders]  
				           WHERE [DossierFolderNode] = hierarchyid::GetRoot())
				BEGIN
					SET @parentNode = hierarchyid::GetRoot()
				END	
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[DossierFolders]  
				           WHERE [DossierFolderNode] = (select MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() AND @IdDossier is not null and [IdDossier] = @IdDossier))
				BEGIN
					SET @parentNode = (select MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() AND [IdDossier] = @IdDossier)
					PRINT @parentNode.ToString()
				END	
			END

		-- Recupero il max node in base al parent node
		SELECT @maxNode = MAX([DossierFolderNode]) FROM [dbo].[DossierFolders] WHERE [DossierFolderNode].GetAncestor(1) = @parentNode;

		IF @ParentInsertId IS NOT NULL			
			BEGIN
				SET @node = @parentNode.GetDescendant(@maxNode, NULL)
			END			
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[DossierFolders] WHERE [DossierFolderNode] = hierarchyid::GetRoot() 
							OR [DossierFolderNode] = (select MAX([DossierFolderNode]) from [dbo].[DossierFolders] where [DossierFolderNode].GetAncestor(1) = hierarchyid::GetRoot() and @IdDossier is not null and [IdDossier] = @IdDossier))
				BEGIN
					SET @node = @parentNode.GetDescendant(@maxNode, NULL)
					PRINT @node.ToString()
				END	
				ELSE
				BEGIN
					SET @node = hierarchyid::GetRoot()
					PRINT @node.ToString()
				END
			END
	
		
		INSERT INTO [dbo].[DossierFolders]([DossierFolderNode],[IdDossierFolder],[IdDossier],[IdFascicle],[IdCategory],[Name],[Status],[JsonMetadata],
		[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
		VALUES (@node, @IdDossierFolder, @IdDossier, @IdFascicle, @IdCategory, @Name, @Status, @JsonMetadata, @RegistrationDate, @RegistrationUser, NULL, NULL)

		SELECT [DossierFolderNode],[IdDossierFolder] AS UniqueId,[IdDossier],[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[JsonMetadata],[DossierFolderPath],[DossierFolderLevel],[DossierFolderParentNode],[ParentInsertId],[Timestamp] 
		FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder
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

PRINT N'Inserimento nodo root della DossierFolder';
GO

DECLARE @IdDossierFolder uniqueidentifier
DECLARE @IdDossier uniqueidentifier
DECLARE	@IdFascicle uniqueidentifier
DECLARE	@IdCategory smallint
DECLARE	@Name nvarchar(256)
DECLARE @Status smallint
DECLARE	@JsonMetadata nvarchar(max)
DECLARE @RegistrationDate datetimeoffset(7)
DECLARE @RegistrationUser nvarchar(256)
DECLARE @LastChangedDate datetimeoffset(7)
DECLARE @LastChangedUser nvarchar(256)
DECLARE @ParentInsertId uniqueidentifier

SET @IdDossierFolder=NEWID()
SET @Status=1
SET @Name='DSW ROOT'
SET @RegistrationUser=N'anonymous_api'
SET @RegistrationDate=GETDATE()

EXECUTE [dbo].[DossierFolder_Insert] 
   @IdDossierFolder
  ,@IdDossier
  ,@IdFascicle
  ,@IdCategory
  ,@Name
  ,@Status
  ,@JsonMetadata
  ,@RegistrationDate
  ,@RegistrationUser
  ,@LastChangedDate
  ,@LastChangedUser
  ,@ParentInsertId
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
PRINT N'Creata funzione [webapiprivate].[DossierFolder_FX_RootChildren]' 
GO

CREATE FUNCTION [webapiprivate].[DossierFolder_FX_RootChildren] (
	@IdDossier uniqueidentifier,
	@Status smallint
	)
	RETURNS TABLE
AS 
	RETURN
	(	  

	  WITH RootNode as (
			SELECT DossierFolderNode 
			FROM DossierFolders
			WHERE DossierFolderNode.GetAncestor(1) = (SELECT TOP 1 DossierFolderNode 
													  FROM DossierFolders 
												      WHERE DossierFolderNode = hierarchyid::GetRoot())
				  AND IdDossier = @IdDossier
	  )

	  SELECT [DF].[IdDossierFolder]
	  		,[DF].[Name]
			,[DF].[Status]
		    ,[DF].[IdDossier] as Dossier_IdDossier
			,[DF].[IdFascicle] as Fascicle_IdFascicle
			,[DF].[IdCategory] as Category_IdCategory
			,[DFR].[IdRole] as Role_IdRole
	   FROM DossierFolders DF 
	   LEFT JOIN DossierFolderRoles DFR ON DFR.IdDossierFolder = DF.IdDossierFolder
	   WHERE DF.DossierFolderNode.GetAncestor(1) = (SELECT DossierFolderNode FROM RootNode)
		     AND (@Status IS NULL OR @Status = 0 OR (DF.Status & @Status <> 0))
		GROUP BY [DF].[IdDossierFolder]
				,[DF].[Name]
				,[DF].[Status]
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

PRINT N'Creata funzione [webapiprivate].[DossierFolder_FX_AllChildrenByParent]' 
GO

CREATE FUNCTION [webapiprivate].[DossierFolder_FX_AllChildrenByParent] (
	@IdDossierFolder uniqueidentifier,
	@Status smallint
	)
	RETURNS TABLE
AS 
	RETURN
	(	  

	  SELECT [DF].[IdDossierFolder]
	  		,[DF].[Name]
			,[DF].[Status]
		    ,[DF].[IdDossier] as Dossier_IdDossier
			,[DF].[IdFascicle] as Fascicle_IdFascicle
			,[DF].[IdCategory] as Category_IdCategory
			,[DFR].[IdRole] as Role_IdRole
	   FROM DossierFolders DF 
	   LEFT JOIN DossierFolderRoles DFR ON DFR.IdDossierFolder = DF.IdDossierFolder
	   WHERE DossierFolderNode.GetAncestor(1) = (SELECT TOP 1 DossierFolderNode 
												 FROM DossierFolders 
												 WHERE IdDossierFolder = @IdDossierFolder)
			 AND (@Status IS NULL OR @Status = 0 OR (DF.Status & @Status <> 0))
		GROUP BY [DF].[IdDossierFolder]
		        ,[DF].[Name]
				,[DF].[Status]
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

PRINT N'Creata stored procedure [DossierFolder_Update]' 
GO

CREATE PROCEDURE [dbo].[DossierFolder_Update] 
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
	   @ParentInsertId uniqueidentifier,
	   @Timestamp_Original timestamp
AS
BEGIN
       SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
       BEGIN TRANSACTION
                
              UPDATE [dbo].[DossierFolders] SET [IdFascicle] = @IdFascicle, [IdCategory] = @IdCategory, [Name] = @Name, [Status] = @Status, 
												[JsonMetadata] = @JsonMetadata, [LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser
			  WHERE [IdDossierFolder] = @IdDossierFolder

              SELECT [DossierFolderNode],[IdDossierFolder] as UniqueId,[IdDossier],[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
              [Name],[JsonMetadata],[DossierFolderPath],[DossierFolderLevel],[DossierFolderParentNode],[ParentInsertId],[Timestamp] 
			  FROM [dbo].[DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder
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

PRINT N'Creata stored procedure [DossierFolder_Delete]' 
GO

CREATE  PROCEDURE [dbo].[DossierFolder_Delete] 
       @IdDossierFolder uniqueidentifier, 
       @IdDossier uniqueidentifier,
       @IdFascicle uniqueidentifier, 
       @IdCategory smallint,
	   @Timestamp_Original timestamp
AS
BEGIN
       SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
       BEGIN TRANSACTION
         IF EXISTS (SELECT TOP 1 [IdDossierFolder] FROM [DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder)
		 BEGIN 
              DELETE [dbo].[DossierFolders] WHERE [IdDossierFolder] = @IdDossierFolder
	     END 
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

PRINT N'Creata funzione [DossierFolder_FX_CountChildren]' 
GO

CREATE FUNCTION [webapiprivate].[DossierFolder_FX_CountChildren] (
	@IdDossierFolder uniqueidentifier
	)
RETURNS INT
AS
	BEGIN
	DECLARE @CountChildren INT;
		SELECT @CountChildren = COUNT(DF.IdDossierFolder)
		FROM DossierFolders DF 
		WHERE DF.DossierFolderNode.GetAncestor(1) = (SELECT TOP 1 DossierFolderNode
	    											 FROM DossierFolders 
												     WHERE IdDossierFolder = @IdDossierFolder)
		RETURN @CountChildren
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
PRINT N'Creazione SQL Function [DossierFolder_FX_NameAlreadyExists]' 
GO

CREATE FUNCTION [webapiprivate].[DossierFolder_FX_NameAlreadyExists] 
(
    @FolderName nvarchar(256), 
	@IdParent uniqueidentifier,
	@IdDossier uniqueidentifier
)
RETURNS BIT
AS
BEGIN
    declare @HasAlreadyExistingName bit;
	
	SELECT  @HasAlreadyExistingName = CAST(COUNT(1) AS BIT)
	FROM DossierFolders 
	WHERE DossierFolderNode.GetAncestor(1) = (SELECT TOP(1) DossierFoldernode 
	                                          FROM DossierFolders 
											  WHERE IdDossierFolder = @IdParent)
	      AND Name= @FolderName
		  AND IdDossier = @IdDossier
	RETURN @HasAlreadyExistingName;
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
PRINT N'Creazione SQL Function [DossierFolder_FX_FascicleAlreadyExists]' 
GO

CREATE FUNCTION [webapiprivate].[DossierFolder_FX_FascicleAlreadyExists] 
(
    @IdFascicle uniqueidentifier, 
	@IdParent uniqueidentifier,
	@IdDossier uniqueidentifier
)
RETURNS BIT
AS
BEGIN
    declare @HasTheSameFascicle bit;
	
	SELECT  @HasTheSameFascicle = CAST(COUNT(1) AS BIT)
	FROM DossierFolders 
	WHERE DossierFolderNode.GetAncestor(1) = (SELECT TOP(1) DossierFoldernode 
	                                          FROM DossierFolders 
											  WHERE IdDossierFolder = @IdParent)
	      AND IdFascicle= @IdFascicle
		  AND IdDossier = @IdDossier
	RETURN @HasTheSameFascicle;
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

PRINT N'Creata funzione [DossierFolder_FX_HasRootNode]' 
GO

CREATE FUNCTION [webapiprivate].[Dossiers_FX_HasRootNode](
	@IdDossier uniqueidentifier
)
RETURNS BIT
AS
BEGIN
	DECLARE @HasRootNode BIT;
	SELECT @HasRootNode = CAST(COUNT(1) AS BIT) FROM DossierFolders 
	WHERE IdDossier = @IdDossier
	AND DossierFolderNode.IsDescendantOf((SELECT TOP 1 DossierFolderNode FROM [dbo].[DossierFolders] WHERE [DossierFolderNode] = hierarchyid::GetRoot())) = 1
	RETURN @HasRootNode
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