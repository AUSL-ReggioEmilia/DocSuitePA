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
PRINT 'Versionamento database alla 8.72'
GO

EXEC dbo.VersioningDatabase N'8.72'
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
PRINT N'Creazione Tabella UDSLogs'

CREATE TABLE [uds].[UDSLogs](
	[IdUDSLog] [uniqueidentifier] NOT NULL,
	[IdUDS] [uniqueidentifier] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[Environment] [int] NOT NULL,
	[LogDate] [DateTimeOffset](7) NOT NULL,
	[SystemComputer] [nvarchar](256) NOT NULL,
	[SystemUser] [nvarchar](256) NOT NULL,
	[LogType] [smallint] NOT NULL,
	[LogDescription] [nvarchar](MAX) NOT NULL,
	[Severity] [smallint] NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
	CONSTRAINT [PK_UDSLogs] PRIMARY KEY NONCLUSTERED
	(
		[IdUDSLog] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [uds].[UDSLogs] WITH CHECK ADD CONSTRAINT [FK_UDSLogs_UDSRepositories] FOREIGN KEY ([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [uds].[UDSLogs] CHECK CONSTRAINT [FK_UDSLogs_UDSRepositories]
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
PRINT N'Creazione Tabella TemplateReports'

CREATE TABLE [dbo].[TemplateReports](
	[IdTemplateReport] [uniqueidentifier] NOT NULL,
	[IdArchiveChain] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Status] [smallint] NOT NULL,
	[Environment] [int] NOT NULL,
	[ReportBuilderJsonModel] [nvarchar](max) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_TemplateReport] PRIMARY KEY NONCLUSTERED 
(
	[IdTemplateReport] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_TemplateReports_RegistrationDate] 
	ON [dbo].[TemplateReports] ([RegistrationDate] ASC)
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
PRINT N'Creazione Tabella UDSAuthorizations'

CREATE TABLE [uds].[UDSAuthorizations](
	[IdUDSAuthorization] [uniqueidentifier] NOT NULL,
	[IdUDS] [uniqueidentifier] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[AuthorizationLabel] [nvarchar](256) NULL,
	[Environment] [int] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
	CONSTRAINT [PK_UDSAuthorization] PRIMARY KEY NONCLUSTERED
	(
	[IdUDSAuthorization] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_UDSAuthorizations_RegistrationDate] 
	ON [uds].[UDSAuthorizations] ([RegistrationDate] ASC)
GO

ALTER TABLE [uds].[UDSAuthorizations] WITH CHECK ADD CONSTRAINT [FK_UDSAuthorizations_UDSRepositories] FOREIGN KEY ([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [uds].[UDSAuthorizations] CHECK CONSTRAINT [FK_UDSAuthorizations_UDSRepositories]
GO	

ALTER TABLE [uds].[UDSAuthorizations] WITH CHECK ADD CONSTRAINT [FK_UDSAuthorizations_Role] FOREIGN KEY ([IdRole])
REFERENCES [dbo].[Role] ([IdRole])
GO

ALTER TABLE [uds].[UDSAuthorizations] CHECK CONSTRAINT [FK_UDSAuthorizations_Role] 
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
PRINT N'Aggiunta colonna Type in [cqrs].[DocumentUnitRoles]'

ALTER TABLE [cqrs].[DocumentUnitRoles] ADD [RoleAuthorizationType] smallint NULL
GO

UPDATE [cqrs].[DocumentUnitRoles] SET [RoleAuthorizationType] = 1 WHERE [RoleAuthorizationType] IS NULL
GO

ALTER TABLE [cqrs].[DocumentUnitRoles] ALTER COLUMN [RoleAuthorizationType] smallint NOT NULL
GO

ALTER TABLE [cqrs].[DocumentUnitRoles] ADD CONSTRAINT [DF_DocumentUnitRoles_RoleAuthorizationType] 
	DEFAULT 1 FOR [RoleAuthorizationType];
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