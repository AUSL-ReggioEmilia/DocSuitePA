SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;
GO

--#############################################################################
PRINT N'Migrazione documenti conservati nella tabella [PreservationDocuments]';
GO

INSERT INTO [dbo].[PreservationDocuments]
           ([IdPreservationDocument]
           ,[IdPreservation]
           ,[IdDocument]
           ,[IdPreservationException]
           ,[PreservationIndex]
           ,[RegistrationUser]
           ,[RegistrationDate])
	SELECT
		NEWID(),
		[IdPreservation],
		[IdDocument],
		[IdPreservationException],
		[PreservationIndex],
		'BiblosDs',
		GETDATE()
	FROM [dbo].[Document]
	WHERE
		[IdPreservation] IS NOT NULL
		AND
		(
			[IsDetached] IS NULL
			OR [IsDetached] = 0
		)
		AND IsLatestVersion = 1
GO