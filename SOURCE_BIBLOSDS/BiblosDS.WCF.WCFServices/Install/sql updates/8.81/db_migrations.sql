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
PRINT N'Creazione tabella [PreservationDocuments]';
GO

CREATE TABLE [dbo].[PreservationDocuments](
	[IdPreservationDocument] [uniqueidentifier] NOT NULL,
	[IdPreservation] [uniqueidentifier] NOT NULL,
	[IdDocument] [uniqueidentifier] NOT NULL,
	[IdPreservationException] [uniqueidentifier] NULL,
	[Path] [nvarchar](260) NULL,
	[Hash] [nvarchar](256) NULL,
	[PreservationIndex] [bigint] NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_IdPreservationDocument] PRIMARY KEY NONCLUSTERED 
(
	[IdPreservationDocument] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PreservationDocuments]  WITH CHECK ADD  CONSTRAINT [FK_PreservationDocuments_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO

ALTER TABLE [dbo].[PreservationDocuments] CHECK CONSTRAINT [FK_PreservationDocuments_Document]
GO

ALTER TABLE [dbo].[PreservationDocuments]  WITH CHECK ADD  CONSTRAINT [FK_PreservationDocuments_Preservation] FOREIGN KEY([IdPreservation])
REFERENCES [dbo].[Preservation] ([IdPreservation])
GO

ALTER TABLE [dbo].[PreservationDocuments] CHECK CONSTRAINT [FK_PreservationDocuments_Preservation]
GO

ALTER TABLE [dbo].[PreservationDocuments]  WITH CHECK ADD  CONSTRAINT [FK_PreservationDocuments_PreservationException] FOREIGN KEY([IdPreservationException])
REFERENCES [dbo].[PreservationException] ([IdPreservationException])
GO

ALTER TABLE [dbo].[PreservationDocuments] CHECK CONSTRAINT [FK_PreservationDocuments_PreservationException]
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
PRINT N'Aggiunta clustered index [IX_PreservationDocuments_RegistrationDate] alla tabella [PreservationDocuments]';
GO

CREATE CLUSTERED INDEX [IX_PreservationDocuments_RegistrationDate] ON [dbo].[PreservationDocuments]
(
	[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT N'Aggiunta unique index [IX_PreservationDocuments_IdDocument] alla tabella [PreservationDocuments]';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PreservationDocuments_IdDocument] ON [dbo].[PreservationDocuments]
(
	[IdDocument] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT N'Create procedure [Preservation_SP_ResetPreservationTask]';
GO

CREATE PROCEDURE [dbo].[Preservation_SP_ResetPreservationTask]
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
				[ExecutedDate] = NULL 
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
				[ExecutedDate] = NULL 
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
PRINT N'Inserimento nuovo status MovedToPreservation in [DocumentStatus]';
GO

INSERT INTO [dbo].[DocumentStatus]
           ([IdDocumentStatus]
           ,[Description])
	VALUES
		(
			7,
			'Moved to preservation'
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