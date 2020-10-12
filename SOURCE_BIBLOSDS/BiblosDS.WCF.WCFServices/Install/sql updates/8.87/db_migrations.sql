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
PRINT N'Create CustomerCompany Table';
GO

CREATE TABLE [dbo].[CustomerCompany](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdCustomer] [varchar](255) NOT NULL,
	[IdCompany] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
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
PRINT N'Add UniqueComboCustomerCompany constraint';
GO

ALTER TABLE [dbo].[CustomerCompany]  WITH CHECK ADD FOREIGN KEY([IdCompany])
REFERENCES [dbo].[Company] ([IdCompany])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CustomerCompany]  WITH CHECK ADD FOREIGN KEY([IdCustomer])
REFERENCES [ext].[Customer] ([IdCustomer])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CustomerCompany]  
ADD CONSTRAINT UniqueComboCustomerCompany UNIQUE(IdCustomer, IdCompany)
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
PRINT N'Alter Customer table'
GO

ALTER TABLE ext.Customer 
ADD SignInfo NVARCHAR (MAX) NULL;

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
PRINT N'Alter PreservationJournalings_FX_SearchAudits'
GO

ALTER FUNCTION [dbo].[PreservationJournalings_FX_SearchAudits]
(	
	@IdArchive uniqueidentifier,
	@IdPreservation uniqueidentifier,
	@IdPreservationActivity uniqueidentifier,
	@IdCompany uniqueidentifier,
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
		INNER JOIN [dbo].[Archive] AS [A] on [P].IdArchive = [A].IdArchive
		INNER JOIN [dbo].[ArchiveCompany] AS [AC] on [A].IdArchive = [AC].IdArchive
		WHERE ((@StartDate IS NOT NULL AND [PJ].[DateActivity] >= @StartDate) OR (@StartDate IS NULL))
		AND ((@EndDate IS NOT NULL AND [PJ].[DateActivity] <= @EndDate) OR (@EndDate IS NULL))
		AND ((@IdPreservationActivity IS NOT NULL AND [PJ].[IdPreservationJournalingActivity] = @IdPreservationActivity) OR (@IdPreservationActivity IS NULL))
		AND ((@IdArchive IS NOT NULL AND [P].[IdArchive] = @IdArchive) OR (@IdArchive IS NULL))
		AND ((@IdCompany IS NOT NULL AND [AC].[IdCompany] = @IdCompany) OR (@IdCompany IS NULL))
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
PRINT N'Alter Company table'
GO

ALTER TABLE [dbo].[Company]
ADD SignInfo NVARCHAR (MAX) NULL;

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