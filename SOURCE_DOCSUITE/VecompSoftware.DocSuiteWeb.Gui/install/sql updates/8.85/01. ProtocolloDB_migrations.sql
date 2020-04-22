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
PRINT 'CONVERSION TABLE [DocumentSeriesItemLinks To DocumentSeriesItem] TO ABLE WEBAPI MODEL MAPPING COMPLIANCE'
GO

ALTER TABLE [dbo].[DocumentSeriesItemLinks] ADD [UniqueId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
GO

CREATE UNIQUE INDEX [UX_DocumentSeriesItemLinks_UniqueId] ON [dbo].[DocumentSeriesItemLinks]([UniqueId] ASC)
GO

ALTER TABLE [dbo].[DocumentSeriesItemLinks] ADD [Timestamp] TIMESTAMP NOT NULL
GO

ALTER TABLE [DocumentSeriesItemLinks] ADD [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NULL
GO

UPDATE DSIL SET DSIL.UniqueIdDocumentSeriesItem = DSI.UniqueId
FROM [dbo].[DocumentSeriesItemLinks] AS DSIL
INNER JOIN [dbo].[DocumentSeriesItem] DSI ON DSI.Id = DSIL.IdDocumentSeriesItem
GO

ALTER TABLE [DocumentSeriesItemLinks] ALTER COLUMN [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NOT NULL
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
PRINT 'CONVERSION TABLE [DocumentSeriesItemLinks To Resolution] TO ABLE WEBAPI MODEL MAPPING COMPLIANCE'
GO

ALTER TABLE [DocumentSeriesItemLinks] ADD [UniqueIdResolution] UNIQUEIDENTIFIER NULL
GO

UPDATE DSIL SET DSIL.UniqueIdResolution = R.UniqueId
FROM [dbo].[DocumentSeriesItemLinks] AS DSIL
INNER JOIN [dbo].[Resolution] R ON R.idResolution = DSIL.idResolution
GO

ALTER TABLE [DocumentSeriesItemLinks] ALTER COLUMN [UniqueIdResolution] UNIQUEIDENTIFIER NOT NULL
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
PRINT 'CONVERSION TABLE [DocumentSeriesItemMessage] TO WEBAPI MODEL MAPPING'
GO

ALTER TABLE [dbo].[DocumentSeriesItemMessage] ADD [UniqueId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
GO

CREATE UNIQUE INDEX [UX_DocumentSeriesItemMessage_UniqueId] ON [dbo].[DocumentSeriesItemMessage]([UniqueId] ASC);
GO

ALTER TABLE [dbo].[DocumentSeriesItemMessage] ADD [RegistrationDate] DATETIMEOFFSET NOT NULL DEFAULT (sysdatetimeoffset())
GO

ALTER TABLE [dbo].[DocumentSeriesItemMessage] ADD [Timestamp] TIMESTAMP NOT NULL 
GO

ALTER TABLE [DocumentSeriesItemMessage] ADD [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NULL
GO

UPDATE DSIM SET DSIM.UniqueIdDocumentSeriesItem = DSI.UniqueId
FROM [dbo].[DocumentSeriesItemMessage] AS DSIM
INNER JOIN [dbo].[DocumentSeriesItem] DSI ON DSI.Id = DSIM.IdDocumentSeriesItem
GO

ALTER TABLE [DocumentSeriesItemMessage] ALTER COLUMN [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NOT NULL
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
PRINT 'ALTER TABLE [dbo].[WorkflowActivities] ADD [ActivityAction]'
GO

ALTER TABLE [dbo].[WorkflowActivities] ADD [ActivityAction] smallint NOT NULL DEFAULT (-1)
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
PRINT 'ALTER TABLE [dbo].[WorkflowActivities] ADD [ActivityArea]'
GO

ALTER TABLE [dbo].[WorkflowActivities] ADD [ActivityArea] smallint NOT NULL DEFAULT (-1)
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

PRINT N'CREATE TABLE [cqrs].[DocumentUnitFascicleCategories]';
GO
CREATE TABLE [cqrs].[DocumentUnitFascicleCategories] (
	[IdDocumentUnitFascicleCategory] [uniqueidentifier] NOT NULL,
    [IdDocumentUnit] [uniqueidentifier] NOT NULL,
    [IdCategory] [smallint] NOT NULL,
    [IdFascicle] [uniqueidentifier] NULL,
    [RegistrationUser] [nvarchar](256) NOT NULL,
    [RegistrationDate] [datetimeoffset](7) NOT NULL,
    [LastChangedDate] [datetimeoffset](7) NULL,
    [LastChangedUser] [nvarchar](256) NULL,
    [Timestamp] [timestamp] NOT NULL
	
	CONSTRAINT [PK_DocumentUnitFascicleCategories] PRIMARY KEY NONCLUSTERED
	(
		[IdDocumentUnitFascicleCategory] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_DocumentUnitFascicleCategories_RegistrationDate] 
	ON [cqrs].[DocumentUnitFascicleCategories] ([RegistrationDate] ASC)
GO

CREATE UNIQUE INDEX [UX_IdDocumentUnit_IdCategory_IdFascicle] ON [cqrs].[DocumentUnitFascicleCategories]
(
	[IdDocumentUnit] ASC,
	[IdCategory] ASC,
	[IdFascicle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [cqrs].[DocumentUnitFascicleCategories]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitFascicleCategories_DocumentUnit] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO

ALTER TABLE [cqrs].[DocumentUnitFascicleCategories]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitFascicleCategories_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([IdCategory])
GO

ALTER TABLE [cqrs].[DocumentUnitFascicleCategories]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitFascicleCategories_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
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

PRINT N'CREATE TABLE [cqrs].[DocumentUnitFascicleHistoricizedCategories]';
GO
CREATE TABLE [cqrs].[DocumentUnitFascicleHistoricizedCategories] (
	[IdDocumentUnitFascicleHistoricizedCategory] [uniqueidentifier] NOT NULL,
    [IdDocumentUnit] [uniqueidentifier] NOT NULL,
    [IdCategory] [smallint] NOT NULL,
    [UnfascicolatedDate] [datetimeoffset](7) NOT NULL,
    [ReferencedFascicle] [nvarchar](512) NOT NULL,
    [RegistrationUser] [nvarchar](256) NOT NULL,
    [RegistrationDate] [datetimeoffset](7) NOT NULL,
    [LastChangedDate] [datetimeoffset](7) NULL,
    [LastChangedUser] [nvarchar](256) NULL,
    [Timestamp] [timestamp] NOT NULL
	
	CONSTRAINT [PK_DocumentUnitFascicleHistoricizedCategories] PRIMARY KEY NONCLUSTERED
	(
		[IdDocumentUnitFascicleHistoricizedCategory] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_DocumentUnitFascicleHistoricizedCategories_RegistrationDate] 
	ON [cqrs].[DocumentUnitFascicleHistoricizedCategories] ([RegistrationDate] ASC)
GO

CREATE UNIQUE INDEX [UX_IdDocumentUnit_IdCategory] ON [cqrs].[DocumentUnitFascicleHistoricizedCategories]
(
	[IdDocumentUnit] ASC,
	[IdCategory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [cqrs].[DocumentUnitFascicleHistoricizedCategories]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitFascicleHistoricizedCategories_DocumentUnit] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO

ALTER TABLE [cqrs].[DocumentUnitFascicleHistoricizedCategories]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitFascicleHistoricizedCategories_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([IdCategory])
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