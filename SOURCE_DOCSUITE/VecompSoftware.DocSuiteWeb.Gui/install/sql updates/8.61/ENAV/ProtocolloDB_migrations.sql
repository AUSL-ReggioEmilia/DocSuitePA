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

PRINT 'Versionamento database alla 8.61'
GO

EXEC dbo.VersioningDatabase N'8.61'
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
PRINT 'Drop and Create view [ResolutionContact]'
GO

DROP VIEW [dbo].[ResolutionContactEmpty]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ResolutionContactEmpty] AS SELECT TOP 0 
	CAST( NULL AS  [int] ) [idResolution],
	CAST( NULL AS  [int] ) [idContact],
	CAST( NULL AS  [char](1) ) [ComunicationType],
	CAST( NULL AS  [smallint] ) [Incremental],
	CAST( NULL AS  [varchar](20) ) [RegistrationUser],
	CAST( NULL AS  [datetime] ) [RegistrationDate],
	CAST( NULL AS  [uniqueidentifier]) [UniqueId],
	CAST( NULL AS  [timestamp]) [Timestamp],
	CAST( NULL AS  [varchar](30) ) [LastChangedUser],
	CAST( NULL AS  [datetimeoffset](7) ) [LastChangedDate],
	CAST( NULL AS  [uniqueidentifier] ) [UniqueIdResolution]
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

PRINT 'Creazione tabella [FascicleDocuments]'

CREATE TABLE [dbo].[FascicleDocuments] (
	[IdFascicleDocument] [uniqueidentifier] NOT NULL,
	[IdFascicle] [uniqueidentifier] NOT NULL,
	[IdArchiveChain] [uniqueidentifier] NOT NULL,
	[ChainType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,	
 CONSTRAINT [PK_FascicleDocuments] PRIMARY KEY NONCLUSTERED 
(
	[IdFascicleDocument] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_FascicleDocuments_RegistationDate]
    ON [dbo].[FascicleDocuments]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[FascicleDocuments]  WITH CHECK ADD  CONSTRAINT [FK_FascicleDocuments_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
ALTER TABLE [dbo].[FascicleDocuments] CHECK CONSTRAINT [FK_FascicleDocuments_Fascicles]
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

PRINT 'Creazione tabella [FascicleRoles]'

CREATE TABLE [dbo].[FascicleRoles] (
	[IdFascicleRole] [uniqueidentifier] NOT NULL,
	[IdFascicle] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[RoleAuthorizationType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,	
 CONSTRAINT [PK_FascicleRoles] PRIMARY KEY NONCLUSTERED 
(
	[IdFascicleRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_FascicleRoles_RegistationDate]
    ON [dbo].[FascicleRoles]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[FascicleRoles]  WITH CHECK ADD  CONSTRAINT [FK_FascicleRoles_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
ALTER TABLE [dbo].[FascicleRoles] CHECK CONSTRAINT [FK_FascicleRoles_Fascicles]
GO

ALTER TABLE [dbo].[FascicleRoles]  WITH CHECK ADD  CONSTRAINT [FK_FascicleRoles_Roles] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([IdRole])
GO
ALTER TABLE [dbo].[FascicleRoles] CHECK CONSTRAINT [FK_FascicleRoles_Roles]
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

PRINT 'Aggiungo la colonna [Timestamp] nella tabella [WorkflowRoles]';
GO

ALTER TABLE [dbo].[WorkflowRoles] ADD  [Timestamp] TIMESTAMP not null
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