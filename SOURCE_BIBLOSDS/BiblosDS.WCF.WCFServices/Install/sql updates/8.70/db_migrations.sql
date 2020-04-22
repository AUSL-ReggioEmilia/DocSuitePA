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
PRINT N'Modifica descrizione lotto nella colonna [Name] della tabella [dbo].[AwardBatch]';
GO

UPDATE [dbo].[AwardBatch] SET [Name] = REPLACE([Name], 'Lotto vers.', 'Pacchetto di versamento.')
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
PRINT N'Aggiunta della colonna [IdPDVDocument] alla tabella [dbo].[AwardBatch]';
GO

ALTER TABLE [dbo].[AwardBatch] ADD [IdPDVDocument] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[AwardBatch]  WITH CHECK ADD  CONSTRAINT [FK_AwardBatch_PDV_Document] FOREIGN KEY([IdPDVDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO

ALTER TABLE [dbo].[AwardBatch] CHECK CONSTRAINT [FK_AwardBatch_PDV_Document]
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
PRINT N'Aggiunta della colonna [IdRDVDocument] alla tabella [dbo].[AwardBatch]';
GO

ALTER TABLE [dbo].[AwardBatch] ADD [IdRDVDocument] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[AwardBatch]  WITH CHECK ADD  CONSTRAINT [FK_AwardBatch_RDV_Document] FOREIGN KEY([IdRDVDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO

ALTER TABLE [dbo].[AwardBatch] CHECK CONSTRAINT [FK_AwardBatch_RDV_Document]
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
PRINT N'CREATE SQL function [dbo].[Preservations_FX_SearchPreservations]';
GO

CREATE FUNCTION [dbo].[Preservations_FX_SearchPreservations]
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
		[Archive_ThumbnailEnabled], [Archive_PdfConversionEnabled], [Archive_FullSignEnabled], [Archive_VerifyPreservationDateEnabled], 
		[Archive_VerifyPreservationIncrementalEnabled], [Archive_TransitoEnabled], [Archive_FiscalDocumentType], 
		[Archive_ODBCConnection], 

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
			[A].[ThumbnailEnabled] AS [Archive_ThumbnailEnabled], [A].[PdfConversionEnabled] AS [Archive_PdfConversionEnabled], [A].[FullSignEnabled] AS [Archive_FullSignEnabled], [A].[VerifyPreservationDateEnabled] AS [Archive_VerifyPreservationDateEnabled], 
			[A].[VerifyPreservationIncrementalEnabled] AS [Archive_VerifyPreservationIncrementalEnabled], [A].[TransitoEnabled] AS [Archive_TransitoEnabled], [A].[FiscalDocumentType] AS [Archive_FiscalDocumentType], 
			[A].[ODBCConnection] AS [Archive_ODBCConnection], 

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
PRINT N'CREATE SQL function [dbo].[PreservationJournalings_FX_SearchAudits]';
GO

CREATE FUNCTION [dbo].[PreservationJournalings_FX_SearchAudits]
(	
	@IdArchive uniqueidentifier,
	@IdPreservation uniqueidentifier,
	@IdPreservationActivity uniqueidentifier,
	@StartDate datetime,
	@EndDate datetime,
	@Skip int,
	@Top int,
	@Sorting nvarchar(4)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT TOP(@Top) [IdPreservationJournaling], [IdPreservation], [IdPreservationJournalingActivity],
		[DateCreated], [DateActivity], [IdPreservationUser], [Notes],
		[DomainUser],

		[PreservationJournalingActivity_IdPreservationJournalingActivity], [PreservationJournalingActivity_KeyCode],
		[PreservationJournalingActivity_Description], [PreservationJournalingActivity_IsUserActivity],
		[PreservationJournalingActivity_IsUserDeleteEnable],

		[PreservationUser_IdPreservationUser], [PreservationUser_Name], [PreservationUser_Surname], 
		[PreservationUser_FiscalId], [PreservationUser_Address], [PreservationUser_Email], [PreservationUser_Enable], 
		[PreservationUser_DomainUser], [PreservationUser_DefaultUser], [PreservationUser_Username], 
		[PreservationUser_Password],

		[Preservation_IdPreservation], [Preservation_IdArchive], [Preservation_IdPreservationTaskGroup], 
		[Preservation_IdPreservationTask], [Preservation_Path], [Preservation_Label], [Preservation_PreservationDate], 
		[Preservation_StartDate], [Preservation_EndDate], [Preservation_CloseDate], [Preservation_IndexHash], 
		[Preservation_CloseContent], [Preservation_LastVerifiedDate], [Preservation_IdPreservationUser], 
		[Preservation_IdCompatibility], [Preservation_PathHash], [Preservation_IdArchiveBiblosStore], 
		[Preservation_IdDocumentIndex], [Preservation_IdDocumentClose], [Preservation_IdDocumentIndexXml], 
		[Preservation_IdDocumentIndedSigned], [Preservation_IdDocumentCloseSigned], [Preservation_PreservationSize], 
		[Preservation_LastSectionalValue], [Preservation_IdDocumentIndexXSLT], [Preservation_LockOnDocumentInsert]
	FROM (
		SELECT [PJ].[IdPreservationJournaling] AS [IdPreservationJournaling], [PJ].[IdPreservation] AS [IdPreservation], [PJ].[IdPreservationJournalingActivity] AS [IdPreservationJournalingActivity],
			[PJ].[DateCreated] AS [DateCreated], [PJ].[DateActivity] AS [DateActivity], [PJ].[IdPreservationUser] AS [IdPreservationUser], [PJ].[Notes] AS [Notes],
			[PJ].[DomainUser] AS [DomainUser],

			[PJA].[IdPreservationJournalingActivity] AS [PreservationJournalingActivity_IdPreservationJournalingActivity], [PJA].[KeyCode] AS [PreservationJournalingActivity_KeyCode],
			[PJA].[Description] AS [PreservationJournalingActivity_Description], [PJA].[IsUserActivity] AS [PreservationJournalingActivity_IsUserActivity],
			[PJA].[IsUserDeleteEnable] AS [PreservationJournalingActivity_IsUserDeleteEnable],

			[PU].[IdPreservationUser] AS [PreservationUser_IdPreservationUser], [PU].[Name] AS [PreservationUser_Name], [PU].[Surname] AS [PreservationUser_Surname], 
			[PU].[FiscalId] AS [PreservationUser_FiscalId], [PU].[Address] AS [PreservationUser_Address], [PU].[Email] AS [PreservationUser_Email], [PU].[Enable] AS [PreservationUser_Enable], 
			[PU].[DomainUser] AS [PreservationUser_DomainUser], [PU].[DefaultUser] AS [PreservationUser_DefaultUser], [PU].[Username] AS [PreservationUser_Username], 
			[PU].[Password] AS [PreservationUser_Password],

			[P].[IdPreservation] AS [Preservation_IdPreservation], [P].[IdArchive] AS [Preservation_IdArchive], [P].[IdPreservationTaskGroup] AS [Preservation_IdPreservationTaskGroup], 
			[P].[IdPreservationTask] AS [Preservation_IdPreservationTask], [P].[Path] AS [Preservation_Path], [P].[Label] AS [Preservation_Label], [P].[PreservationDate] AS [Preservation_PreservationDate], 
			[P].[StartDate] AS [Preservation_StartDate], [P].[EndDate] AS [Preservation_EndDate], [P].[CloseDate] AS [Preservation_CloseDate], [P].[IndexHash] AS [Preservation_IndexHash], 
			NULL AS [Preservation_CloseContent], [P].[LastVerifiedDate] AS [Preservation_LastVerifiedDate], [P].[IdPreservationUser] AS [Preservation_IdPreservationUser], 
			[P].[IdCompatibility] AS [Preservation_IdCompatibility], [P].[PathHash] AS [Preservation_PathHash], [P].[IdArchiveBiblosStore] AS [Preservation_IdArchiveBiblosStore], 
			[P].[IdDocumentIndex] AS [Preservation_IdDocumentIndex], [P].[IdDocumentClose] AS [Preservation_IdDocumentClose], [P].[IdDocumentIndexXml] AS [Preservation_IdDocumentIndexXml], 
			[P].[IdDocumentIndedSigned] AS [Preservation_IdDocumentIndedSigned], [P].[IdDocumentCloseSigned] AS [Preservation_IdDocumentCloseSigned], [P].[PreservationSize] AS [Preservation_PreservationSize], 
			[P].[LastSectionalValue] AS [Preservation_LastSectionalValue], [P].[IdDocumentIndexXSLT] AS [Preservation_IdDocumentIndexXSLT], [P].[LockOnDocumentInsert] AS [Preservation_LockOnDocumentInsert],
			ROW_NUMBER() OVER (ORDER BY [PJ].[IdPreservationJournaling]) AS ROW_NUM

		FROM [dbo].[PreservationJournaling] AS [PJ]
		INNER JOIN [dbo].[PreservationJournalingActivity] AS [PJA] ON [PJ].[IdPreservationJournalingActivity] = [PJA].[IdPreservationJournalingActivity]
		INNER JOIN [dbo].[PreservationUser] AS [PU] ON [PJ].[IdPreservationUser] = [PU].[IdPreservationUser]
		LEFT OUTER JOIN [dbo].[Preservation] AS [P] ON [P].[IdPreservation] = [PJ].[IdPreservation]
		WHERE ((@StartDate IS NOT NULL AND [PJ].[DateActivity] >= @StartDate) OR (@StartDate IS NULL))
		AND ((@EndDate IS NOT NULL AND [PJ].[DateActivity] <= @EndDate) OR (@EndDate IS NULL))
		AND ((@IdPreservationActivity IS NOT NULL AND [PJ].[IdPreservationJournalingActivity] = @IdPreservationActivity) OR (@IdPreservationActivity IS NULL))
		AND ((@IdArchive IS NOT NULL AND [P].[IdArchive] = @IdArchive) OR (@IdArchive IS NULL))
		AND ((@IdPreservation IS NOT NULL AND [PJ].[IdPreservation] = @IdPreservation) OR (@IdPreservation IS NULL))
	) tbl
	WHERE ROW_NUM > @Skip	
	ORDER BY 
		CASE WHEN @Sorting = 'ASC' THEN [tbl].[DateActivity] END ASC,
		CASE WHEN @Sorting = 'DESC' THEN [tbl].[DateActivity] END DESC
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
PRINT N'Aggiunta della colonna [IsRDVSigned] alla tabella [dbo].[AwardBatch]';
GO

ALTER TABLE [dbo].[AwardBatch] ADD [IsRDVSigned] bit NULL
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
PRINT N'Creazione utente di default BiblosDS nella tabella [ext].[Customer] e [ext].[CustomerLogin]';
GO

DECLARE @CustomerId VARCHAR(255) = NEWID();
INSERT INTO [ext].[Customer]
           ([IdCustomer]
           ,[Name]
           ,[DateCreated])
     VALUES
           (@CustomerId
           ,'BiblosDS'
           ,GETDATE())

INSERT INTO [ext].[CustomerLogin]
           ([IdCustomerLogin]
           ,[IdCustomer]
           ,[UserName]
           ,[Password]
           ,[DateCreated]
           ,[Roles])
     VALUES (NEWID()
			,@CustomerId
			,'BiblosDS'
			,'Ud7PaHX7QOLtz6i1udCpdw=='
			,GETDATE()
			,null)
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
PRINT N'Aggiunta indice [IX_Document_IdAwardBatch] alla tabella [dbo].[Document]';
GO

CREATE NONCLUSTERED INDEX [IX_Document_IdAwardBatch]
	ON [dbo].[Document] ([IdAwardBatch])
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