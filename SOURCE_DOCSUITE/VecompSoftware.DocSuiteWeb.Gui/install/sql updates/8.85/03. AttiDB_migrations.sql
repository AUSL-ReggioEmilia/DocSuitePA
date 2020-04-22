/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<DBProtocollo, varcahr(50), DBProtocollo>  --> Settare il nome del DB di protocollo.				        					
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
PRINT 'ALTER PROCEDURE [dbo].[VersioningDatabase]'
GO

ALTER PROCEDURE [dbo].[VersioningDatabase]
	@Version AS NCHAR(5),
	@AppName AS NCHAR(30),
	@MigrateLabel AS NCHAR(30)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @DateNow AS NCHAR(23)
	SET @DateNow = CONVERT(NCHAR(23), SYSDATETIME(), 126)
	IF EXISTS( select * from sys.extended_properties Where class_desc = 'DATABASE' And name = @AppName)
	BEGIN
		-- Aggiornamento property
		EXEC sys.sp_updateextendedproperty @name = @AppName, @value = @Version;
	END
	ELSE
	BEGIN 
		-- Aggiunta property
		EXEC sys.sp_addextendedproperty @name = @AppName, @value = @Version;		
	END

	IF EXISTS( select * from sys.extended_properties Where class_desc = 'DATABASE' And name = @MigrateLabel)
	BEGIN
		EXEC sys.sp_updateextendedproperty @name = @MigrateLabel, @value = @DateNow;
	END
	ELSE
	BEGIN 
		-- Aggiunta property
		EXEC sys.sp_addextendedproperty @name = @MigrateLabel, @value = @DateNow;
	END
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
PRINT 'Versionamento database alla 8.85'
GO

EXEC dbo.VersioningDatabase N'8.85',N'DSW Version','MigrationDate'
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
PRINT 'CONVERSION TABLE [ResolutionDocumentSeriesItem To Resolution] TO ABLE WEBAPI MODEL MAPPING COMPLIANCE'
GO

ALTER TABLE [dbo].[ResolutionDocumentSeriesItem] ADD [UniqueId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
GO

CREATE UNIQUE INDEX [UX_ResolutionDocumentSeriesItem_UniqueId] ON [dbo].[ResolutionDocumentSeriesItem]([UniqueId] ASC);
GO

ALTER TABLE [dbo].[ResolutionDocumentSeriesItem] ADD [Timestamp] TIMESTAMP not null
GO

ALTER TABLE [ResolutionDocumentSeriesItem] ADD [UniqueIdResolution] UNIQUEIDENTIFIER NULL
GO

UPDATE RSDI SET RSDI.UniqueIdResolution = R.UniqueId
FROM [dbo].[ResolutionDocumentSeriesItem] AS RSDI
INNER JOIN [dbo].[Resolution] R ON R.idResolution = RSDI.IdResolution
GO

ALTER TABLE [ResolutionDocumentSeriesItem] ALTER COLUMN [UniqueIdResolution] UNIQUEIDENTIFIER NOT NULL
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
PRINT 'CONVERSION TABLE [ResolutionDocumentSeriesItem To DocumentSeriesItem] TO ABLE WEBAPI MODEL MAPPING COMPLIANCE'
GO

ALTER TABLE [ResolutionDocumentSeriesItem] ADD [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NULL
GO

UPDATE RDSI SET RDSI.UniqueIdDocumentSeriesItem = I.UniqueId
FROM [dbo].[ResolutionDocumentSeriesItem] AS RDSI
INNER JOIN <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[DocumentSeriesItem] I ON I.Id = RDSI.IdDocumentSeriesItem
GO

ALTER TABLE [ResolutionDocumentSeriesItem] ALTER COLUMN [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NOT NULL
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
PRINT 'CONVERSION TABLE [ResolutionMessage] TO WEBAPI MODEL MAPPING'
GO

ALTER TABLE [dbo].[ResolutionMessage] ADD [UniqueId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
GO

CREATE UNIQUE INDEX [UX_ResolutionMessage_UniqueId] ON [dbo].[ResolutionMessage]([UniqueId] ASC);
GO

ALTER TABLE [dbo].[ResolutionMessage] ADD [RegistrationDate] DATETIMEOFFSET NOT NULL DEFAULT (sysdatetimeoffset())
GO

ALTER TABLE [dbo].[ResolutionMessage] ADD [Timestamp] TIMESTAMP NOT NULL 
GO

ALTER TABLE [ResolutionMessage] ADD [UniqueIdResolution] UNIQUEIDENTIFIER NULL
GO

UPDATE RM SET RM.UniqueIdResolution = R.UniqueId
FROM [dbo].[ResolutionMessage] AS RM
INNER JOIN [dbo].[Resolution] R ON R.idResolution = RM.IdResolution
GO

ALTER TABLE [ResolutionMessage] ALTER COLUMN [UniqueIdResolution] UNIQUEIDENTIFIER NOT NULL
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