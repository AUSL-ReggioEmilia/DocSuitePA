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
PRINT N'Update [Server] set ServerRole = Undefined';
GO

UPDATE [Server] SET [ServerRole] = 'Undefined'
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
PRINT 'Create unique index IX_ArchiveServerConfig_IdArchive into table [dbo].[ArchiveServerConfig]'
GO

CREATE UNIQUE INDEX [IX_ArchiveServerConfig_IdArchive] 
	ON [dbo].[ArchiveServerConfig] ([IdArchive])
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
PRINT N'Alter table [Server] add column [DocumentServiceUrl]';
GO

ALTER TABLE [Server] ADD [DocumentServiceUrl] NVARCHAR(500) NULL
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
PRINT N'Alter table [Server] add column [DocumentServiceBinding]';
GO

ALTER TABLE [Server] ADD [DocumentServiceBinding] NVARCHAR(256) NULL
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
PRINT N'Alter table [Server] add column [DocumentServiceBindingConfiguration]';
GO

ALTER TABLE [Server] ADD [DocumentServiceBindingConfiguration] NVARCHAR(256) NULL
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
PRINT N'Alter table [Server] add column [StorageServiceUrl]';
GO

ALTER TABLE [Server] ADD [StorageServiceUrl] NVARCHAR(500) NULL
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
PRINT N'Alter table [Server] add column [StorageServiceBinding]';
GO

ALTER TABLE [Server] ADD [StorageServiceBinding] NVARCHAR(256) NULL
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
PRINT N'Alter table [Server] add column [StorageServiceBindingConfiguration]';
GO

ALTER TABLE [Server] ADD [StorageServiceBindingConfiguration] NVARCHAR(256) NULL
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
PRINT N'Alter table Archive add column PreservationConfiguration'
GO

ALTER TABLE [dbo].[Archive] 
ADD PreservationConfiguration NVARCHAR (MAX) NULL;

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
PRINT N'Alter SQL function [Preservations_FX_SearchPreservations]'
GO

ALTER FUNCTION [dbo].[Preservations_FX_SearchPreservations]
(	
	@IncludeCloseContent bit,
	@IncludeReleatedEntities bit,
	@IdArchive uniqueidentifier,
	@Skip int,
	@Top int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT TOP(@Top) [IdPreservation], [IdArchive], [IdPreservationTaskGroup], [IdPreservationTask], 
		[Path], [Label], [PreservationDate], [StartDate], [EndDate], [CloseDate], 
		[IndexHash], [CloseContent], [LastVerifiedDate], [IdPreservationUser], [IdCompatibility], 
		[PathHash], [IdArchiveBiblosStore], [IdDocumentIndex], [IdDocumentClose], 
		[IdDocumentIndexXml], [IdDocumentIndedSigned], [IdDocumentCloseSigned], [PreservationSize], 
		[LastSectionalValue], [IdDocumentIndexXSLT], [LockOnDocumentInsert], 
		
		[Archive_IdArchive], [Archive_Name], [Archive_IsLegal], [Archive_MaxCache], [Archive_UpperCache], [Archive_LowerCache], 
		[Archive_LastIdBiblos], [Archive_AutoVersion], [Archive_AuthorizationAssembly], [Archive_AuthorizationClassName], 
		[Archive_EnableSecurity], [Archive_PathTransito], [Archive_PathCache], [Archive_PathPreservation], [Archive_LastAutoIncValue], 
		[Archive_ThumbnailEnabled], [Archive_PdfConversionEnabled], [Archive_FullSignEnabled],
		[Archive_TransitoEnabled], [Archive_FiscalDocumentType], 
		[Archive_ODBCConnection],[Archive_PreservationConfiguration], 

		[PreservationTaskGroup_IdPreservationTaskGroup], [PreservationTaskGroup_IdPreservationTaskGroupType], [PreservationTaskGroup_Name], [PreservationTaskGroup_IdPreservationUser], 
		[PreservationTaskGroup_IdPreservationSchedule], [PreservationTaskGroup_Expiry], [PreservationTaskGroup_EstimatedExpiry], [PreservationTaskGroup_Closed], [PreservationTaskGroup_IdCompatibility], 
		[PreservationTaskGroup_IdArchive], 

		[PreservationTask_IdPreservationTask], [PreservationTask_IdPreservationConsole], [PreservationTask_EstimatedDate], [PreservationTask_ExecutedDate], 
		[PreservationTask_StartDocumentDate], [PreservationTask_EndDocumentDate], [PreservationTask_IdPreservationTaskType], [PreservationTask_IdArchive], 
		[PreservationTask_IdPreservationTaskStatus], [PreservationTask_IdPreservationUser], [PreservationTask_IdPreservationTaskGroup], 
		[PreservationTask_Name], [PreservationTask_IdCorrelatedPreservationTask], [PreservationTask_Executed], [PreservationTask_Enabled], [PreservationTask_HasError], 
		[PreservationTask_ErrorMessages], [PreservationTask_ActivationPin], [PreservationTask_IdPreservation], [PreservationTask_LockDate], 

		[PreservationUser_IdPreservationUser], [PreservationUser_Name], [PreservationUser_Surname], [PreservationUser_FiscalId], [PreservationUser_Address], [PreservationUser_Email], 
		[PreservationUser_Enable], [PreservationUser_DomainUser], [PreservationUser_DefaultUser], [PreservationUser_Username], [PreservationUser_Password] 
	FROM (
		SELECT [P].[IdPreservation] AS [IdPreservation], [P].[IdArchive] AS [IdArchive], [P].[IdPreservationTaskGroup] AS [IdPreservationTaskGroup], [P].[IdPreservationTask] AS [IdPreservationTask], 
			[P].[Path] AS [Path], [P].[Label] AS [Label], [P].[PreservationDate] AS [PreservationDate], [P].[StartDate] AS [StartDate], [P].[EndDate] AS [EndDate], [P].[CloseDate] AS [CloseDate], 
			[P].[IndexHash] AS [IndexHash], CASE WHEN @includeCloseContent = 1 THEN [P].[CloseContent] ELSE NULL END AS [CloseContent], [P].[LastVerifiedDate] AS [LastVerifiedDate], [P].[IdPreservationUser] AS [IdPreservationUser], [P].[IdCompatibility] AS [IdCompatibility], 
			[P].[PathHash] AS [PathHash], [P].[IdArchiveBiblosStore] AS [IdArchiveBiblosStore], [P].[IdDocumentIndex] AS [IdDocumentIndex], [P].[IdDocumentClose] AS [IdDocumentClose], 
			[P].[IdDocumentIndexXml] AS [IdDocumentIndexXml], [P].[IdDocumentIndedSigned] AS [IdDocumentIndedSigned], [P].[IdDocumentCloseSigned] AS [IdDocumentCloseSigned], [P].[PreservationSize] AS [PreservationSize], 
			[P].[LastSectionalValue] AS [LastSectionalValue], [P].[IdDocumentIndexXSLT] AS [IdDocumentIndexXSLT], [P].[LockOnDocumentInsert] AS [LockOnDocumentInsert], 
		
			[A].[IdArchive] AS [Archive_IdArchive], [A].[Name] AS [Archive_Name], [A].[IsLegal] AS [Archive_IsLegal], [A].[MaxCache] AS [Archive_MaxCache], [A].[UpperCache] AS [Archive_UpperCache], [A].[LowerCache] AS [Archive_LowerCache], 
			[A].[LastIdBiblos] AS [Archive_LastIdBiblos], [A].[AutoVersion] AS [Archive_AutoVersion], [A].[AuthorizationAssembly] AS [Archive_AuthorizationAssembly], [A].[AuthorizationClassName] AS [Archive_AuthorizationClassName], 
			[A].[EnableSecurity] AS [Archive_EnableSecurity], [A].[PathTransito] AS [Archive_PathTransito], [A].[PathCache] AS [Archive_PathCache], [A].[PathPreservation] AS [Archive_PathPreservation], [A].[LastAutoIncValue] AS [Archive_LastAutoIncValue], 
			[A].[ThumbnailEnabled] AS [Archive_ThumbnailEnabled], [A].[PdfConversionEnabled] AS [Archive_PdfConversionEnabled], [A].[FullSignEnabled] AS [Archive_FullSignEnabled],
			[A].[TransitoEnabled] AS [Archive_TransitoEnabled], [A].[FiscalDocumentType] AS [Archive_FiscalDocumentType], 
			[A].[ODBCConnection] AS [Archive_ODBCConnection], [A].[PreservationConfiguration] AS [Archive_PreservationConfiguration],

			[PTG].[IdPreservationTaskGroup] AS [PreservationTaskGroup_IdPreservationTaskGroup], [PTG].[IdPreservationTaskGroupType] AS [PreservationTaskGroup_IdPreservationTaskGroupType], [PTG].[Name] AS [PreservationTaskGroup_Name], [PTG].[IdPreservationUser] AS [PreservationTaskGroup_IdPreservationUser], 
			[PTG].[IdPreservationSchedule] AS [PreservationTaskGroup_IdPreservationSchedule], [PTG].[Expiry] AS [PreservationTaskGroup_Expiry], [PTG].[EstimatedExpiry] AS [PreservationTaskGroup_EstimatedExpiry], [PTG].[Closed] AS [PreservationTaskGroup_Closed], [PTG].[IdCompatibility] AS [PreservationTaskGroup_IdCompatibility], 
			[PTG].[IdArchive] AS [PreservationTaskGroup_IdArchive], 

			[PT].[IdPreservationTask] AS [PreservationTask_IdPreservationTask], [PT].[IdPreservationConsole] AS [PreservationTask_IdPreservationConsole], [PT].[EstimatedDate] AS [PreservationTask_EstimatedDate], [PT].[ExecutedDate] AS [PreservationTask_ExecutedDate], 
			[PT].[StartDocumentDate] AS [PreservationTask_StartDocumentDate], [PT].[EndDocumentDate] AS [PreservationTask_EndDocumentDate], [PT].[IdPreservationTaskType] AS [PreservationTask_IdPreservationTaskType], [PT].[IdArchive] AS [PreservationTask_IdArchive], 
			[PT].[IdPreservationTaskStatus] AS [PreservationTask_IdPreservationTaskStatus], [PT].[IdPreservationUser] AS [PreservationTask_IdPreservationUser], [PT].[IdPreservationTaskGroup] AS [PreservationTask_IdPreservationTaskGroup], 
			[PT].[Name] AS [PreservationTask_Name], [PT].[IdCorrelatedPreservationTask] AS [PreservationTask_IdCorrelatedPreservationTask], [PT].[Executed] AS [PreservationTask_Executed], [PT].[Enabled] AS [PreservationTask_Enabled], [PT].[HasError] AS [PreservationTask_HasError], 
			[PT].[ErrorMessages] AS [PreservationTask_ErrorMessages], [PT].[ActivationPin] AS [PreservationTask_ActivationPin], [PT].[IdPreservation] AS [PreservationTask_IdPreservation], [PT].[LockDate] AS [PreservationTask_LockDate], 

			[PU].[IdPreservationUser] AS [PreservationUser_IdPreservationUser], [PU].[Name] AS [PreservationUser_Name], [PU].[Surname] AS [PreservationUser_Surname], [PU].[FiscalId] AS [PreservationUser_FiscalId], [PU].[Address] AS [PreservationUser_Address], [PU].[Email] AS [PreservationUser_Email], 
			[PU].[Enable] AS [PreservationUser_Enable], [PU].[DomainUser] AS [PreservationUser_DomainUser], [PU].[DefaultUser] AS [PreservationUser_DefaultUser], [PU].[Username] AS [PreservationUser_Username], [PU].[Password] AS [PreservationUser_Password], 
			ROW_NUMBER() OVER (ORDER BY [P].[IdPreservation]) AS ROW_NUM

		FROM [dbo].[Preservation] AS [P]
		LEFT OUTER JOIN [dbo].[Archive] AS [A] ON [P].[IdArchive] = CASE WHEN @IncludeReleatedEntities = 1 THEN [A].[IdArchive] ELSE NULL END
		LEFT OUTER JOIN [dbo].[PreservationTaskGroup] AS [PTG] ON [P].[IdPreservationTaskGroup] = CASE WHEN @IncludeReleatedEntities = 1 THEN [PTG].[IdPreservationTaskGroup] ELSE NULL END
		LEFT OUTER JOIN [dbo].[PreservationTask] AS [PT] ON [P].[IdPreservationTask] = CASE WHEN @IncludeReleatedEntities = 1 THEN [PT].[IdPreservationTask] ELSE NULL END
		LEFT OUTER JOIN [dbo].[PreservationUser] AS [PU] ON [P].[IdPreservationUser] = CASE WHEN @IncludeReleatedEntities = 1 THEN [PU].[IdPreservationUser] ELSE NULL END
		WHERE [P].[IdArchive] = @IdArchive AND ((@IncludeReleatedEntities = 1 AND A.IdArchive IS NOT NULL) OR (@IncludeReleatedEntities = 0))
	) tbl
	WHERE ROW_NUM > @Skip	
	ORDER BY [tbl].[StartDate] DESC
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
PRINT N'Insert status values in [dbo].[PreservationTaskStatus]'
GO

INSERT INTO [dbo].[PreservationTaskStatus] ([IdPreservationTaskStatus],[Status]) VALUES (NEWID(), 'Done')
GO

INSERT INTO [dbo].[PreservationTaskStatus] ([IdPreservationTaskStatus],[Status]) VALUES (NEWID(), 'Error')
GO

INSERT INTO [dbo].[PreservationTaskStatus] ([IdPreservationTaskStatus],[Status]) VALUES (NEWID(), 'NoDocuments')
GO

INSERT INTO [dbo].[PreservationTaskStatus] ([IdPreservationTaskStatus],[Status]) VALUES (NEWID(), 'ExistNoConservatedDocuments')
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
PRINT N'Insert type "Conservazione chiusura anno" in [dbo].[PreservationTaskType]'
GO

INSERT INTO [dbo].[PreservationTaskType]
(
	[IdPreservationTaskType],
	[Description],
	[Period],
	[KeyCode]
)
VALUES
(
	NEWID(),
	'Conservazione chiusura anno',
	0,
	6
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
PRINT N'Alter SQL function [Preservation_SP_ResetPreservationTask]'
GO

ALTER PROCEDURE [dbo].[Preservation_SP_ResetPreservationTask]
	@IdTask uniqueidentifier,
	@OnlyError bit,
	@ForceAutoInc bit
AS
BEGIN

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION ResetPreservationTask
	
	BEGIN TRY
		DECLARE @IdPreservation AS uniqueidentifier
		SELECT @IdPreservation = [IdPreservation]
		FROM [dbo].[PreservationTask]
		WHERE
			[IdPreservationTask] = @IdTask

		IF @IdPreservation IS NOT NULL
		BEGIN
			-- Update documents --
			UPDATE D
			SET 
				[D].[IsConservated] = 0
			FROM [dbo].[Document] D
			INNER JOIN [dbo].[PreservationDocuments] PD ON PD.IdDocument = D.IdDocument
			WHERE 
				[PD].[IdPreservation] = @IdPreservation

			If @ForceAutoInc = 1
			BEGIN
				UPDATE AV
				SET [AV].[ValueInt] = 0
				FROM [dbo].[AttributesValue] AV
				INNER JOIN [dbo].[Attributes] AT ON AT.IdAttribute = AV.IdAttribute
				INNER JOIN [dbo].[Document] D ON D.IdDocument = AV.IdDocument
				INNER JOIN [dbo].[PreservationDocuments] PD ON PD.IdDocument = D.IdDocument
				WHERE
					[AT].[IsAutoInc] = 1
					AND [PD].[IdPreservation] = @IdPreservation
			END

			-- Delete PreservationInStorageDevice --
			DELETE 
			FROM [dbo].[PreservationInStorageDevice] 
			WHERE 
				[IdPreservation] = @IdPreservation

			-- Delete PreservationJournaling --
			DELETE 
			FROM [dbo].[PreservationJournaling] 
			WHERE 
				[IdPreservation] = @IdPreservation

			DELETE
			FROM [dbo].[PreservationDocuments]
			WHERE
				[IdPreservation] = @IdPreservation
		END

		If @OnlyError = 1
			UPDATE [dbo].[PreservationTask] 
			SET 
				[IdPreservation] = NULL, 
				[ErrorMessages] = NULL, 
				[HasError] = 0, 
				[Executed] = 0, 
				[ExecutedDate] = NULL,
				[IdPreservationTaskStatus] = NULL
			WHERE 
				[IdPreservationTask] = @IdTask
				OR [IdCorrelatedPreservationTask] = @IdTask
				AND [HasError] = 1
		ELSE
			UPDATE [dbo].[PreservationTask] 
			SET 
				[IdPreservation] = NULL, 
				[ErrorMessages] = NULL, 
				[HasError] = 0, 
				[Executed] = 0, 
				[ExecutedDate] = NULL,
				[IdPreservationTaskStatus] = NULL
			WHERE 
				[IdPreservationTask] = @IdTask
				OR [IdCorrelatedPreservationTask] = @IdTask
	
		IF @IdPreservation IS NOT NULL
			DELETE 
			FROM [dbo].[Preservation] 
			WHERE 
				[IdPreservation] = @IdPreservation

	COMMIT TRANSACTION ResetPreservationTask
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION ResetPreservationTask

		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()

		SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		RAISERROR (@ErrorMessage, -- Message text.  
               @ErrorSeverity, -- Severity.  
               @ErrorState -- State.  
               ); 
	END CATCH
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