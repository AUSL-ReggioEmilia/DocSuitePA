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
PRINT 'Versionamento database alla 8.86'
GO

EXEC dbo.VersioningDatabase N'8.86',N'PrivateWebAPI Version','PrivateWebAPI MigrationDate'
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
PRINT N'ALTER PROCEDURE [dbo].[FascicleFolder_Insert]';
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
	
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid, @allNode hierarchyid, @rootNode hierarchyid, @childNode hierarchyid,@insertiNode hierarchyid,@fascicleNode hierarchyid,@level smallint,@idSubFolder uniqueidentifier

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

		IF @ParentInsertId IS NOT NULL			
			BEGIN
			    SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @parentNode;
				SET @node = @parentNode.GetDescendant(@childNode, NULL)
				
				
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
				[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@node, @IdFascicleFolder, @IdFascicle,@IdCategory, @Name, @Status, @Typology, @RegistrationDate, @RegistrationUser, NULL, NULL)
				
			END	
		ELSE
			BEGIN
			    SELECT @maxNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @parentNode;
				SET @allNode = @parentNode.GetDescendant(@maxNode, NULL)
				PRINT @allNode.ToString()
				
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
													[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@allNode, @IdFascicle, @IdFascicle, NULL, @Name, @Status, 0, @RegistrationDate, @RegistrationUser, NULL, NULL)
				SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @allNode;
				
				SET @fascicleNode = @allNode.GetDescendant(@childNode, NULL)
				SET @idSubFolder = NEWID()
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
													[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@fascicleNode, @idSubFolder, @IdFascicle, @IdCategory, 'Fascicolo', @Status, 1, @RegistrationDate, @RegistrationUser, NULL, NULL)
				
				--EXEC [dbo].[FascicleSubFolder_Insert] @IdFascicle, @fascicleNode, @IdCategory, @idSubFolder, @RegistrationUser 
			END
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
PRINT N'ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_CanBeFascicolable]';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_CanBeFascicolable](
	@FascicleIdCategory smallint,
	@FascicleEnvironment int,
	@FascicleType smallint,
	@IdDocumentUnit uniqueidentifier,
	@Environment int,
	@CategoryId smallint
)
RETURNS BIT
AS
BEGIN
	DECLARE @CanBeFascicolable BIT
	DECLARE @List NVARCHAR(MAX)
	SELECT @List = FullIncrementalPath FROM Category WHERE  IdCategory = @CategoryId;
	
	WITH   
	SplitedValues AS
		(SELECT [Number], [Value] = LTRIM(RTRIM(SUBSTRING(@List, [Number], CHARINDEX('|', @List + '|', [Number]) - [Number])))
		 FROM (SELECT Number = ROW_NUMBER() OVER (ORDER BY name)
			   FROM sys.all_objects) AS x
			   WHERE Number <= LEN(@List) AND SUBSTRING('|' + @List, [Number], LEN('|')) = '|'
		 ),
	 AllCategories AS
		 (SELECT C.Value AS IdCategory, C.Number, CF.DSWEnvironment, CF.FascicleType
		  FROM SplitedValues C
		  INNER JOIN CategoryFascicles AS CF ON C.Value = CF.IdCategory
		 ),
	 Categories AS
		 (SELECT A.IdCategory, A.Number, A.DSWEnvironment, A.FascicleType
		  FROM AllCategories A
		  WHERE A.Number >= (SELECT MAX(I.Number) FROM AllCategories I WHERE I.FascicleType = 1)
		 )

	SELECT @CanBeFascicolable = cast(count(1) as bit)
	FROM Categories C
	WHERE NOT EXISTS (SELECT 1 FROM FascicleDocumentUnits WHERE IdDocumentUnit = @IdDocumentUnit) AND @FascicleIdCategory IN (SELECT IdCategory FROM Categories) AND 
		  (
			   (@FascicleType = 1 AND C.FascicleType in (0,1) AND NOT EXISTS ( SELECT 1 FROM Categories CI WHERE CI.DSWEnvironment = @Environment AND CI.IdCategory = @CategoryId AND CI.FascicleType = 2)) 
			OR (@FascicleType = 2 AND C.FascicleType = 2 AND C.DSWEnvironment = @Environment AND @Environment = @FascicleEnvironment AND C.IdCategory = @CategoryId)
		  )

	RETURN @CanBeFascicolable
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
PRINT N'ALTER FUNCTION [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]';
GO

ALTER FUNCTION [webapiprivate].[WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@Environment integer,
	@AnyEnvironment tinyint,
	@DocumentRequired tinyint,
    @ShowOnlyNoInstanceWorkflows tinyint
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