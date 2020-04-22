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
PRINT 'Versionamento database alla 8.50'
GO

EXEC dbo.VersioningDatabase N'8.50'
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
PRINT 'Creazione delle tabelle di Workflow...'
GO

/****** Object:  Table [dbo].[WorkflowActivities] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowActivities](
	[IdWorkflowActivity] [uniqueidentifier] NOT NULL,
	[IdWorkflowInstance] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NULL,
	[ActivityType] [smallint] NOT NULL,
	[Status] [smallint] NOT NULL,
	[DueDate] [datetimeoffset](7) NULL,
	[Duration] [tinyint] NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_WorkflowActivities] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowActivity] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkflowActivityLogs] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkflowActivityLogs](
	[IdWorkflowActivityLog] [uniqueidentifier] NOT NULL,
	[IdWorkflowActivity] [uniqueidentifier] NOT NULL,
	[LogDate] [datetimeoffset](7) NOT NULL,
	[SystemComputer] [nvarchar](256) NOT NULL,
	[SystemUser] [nvarchar](256) NOT NULL,
	[LogType] [smallint] NOT NULL,
	[LogDescription] [varchar](2000) NOT NULL,
	[Severity] [smallint] NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_WorkflowActivityLogs] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowActivityLog] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkflowInstances] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowInstances](
	[IdWorkflowInstance] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[InstanceId] [uniqueidentifier] NULL,
	[Status] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_WorkflowInstance] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowInstance] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkflowProperties] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowProperties](
	[IdWorkflowProperty] [uniqueidentifier] NOT NULL,
	[IdWorkflowActivity] [uniqueidentifier] NULL,
	[IdWorkflowInstance] [uniqueidentifier] NULL,
	[Name] [nvarchar](256) NOT NULL,
	[WorkflowType] [smallint] NOT NULL,
	[PropertyType] [smallint] NOT NULL,
	[ValueInt] [bigint] NULL,
	[ValueDate] [datetime] NULL,
	[ValueDouble] [float] NULL,
	[ValueBoolean] [bit] NULL,
	[ValueGuid] [uniqueidentifier] NULL,
	[ValueString] [nvarchar](max) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_WorkflowProperties] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowProperty] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkflowRepositories] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowRepositories](
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Version] [nvarchar](5) NOT NULL,
	[ActiveFrom] [datetimeoffset](7) NOT NULL,
	[ActiveTo] [datetimeoffset](7) NULL,
	[Xaml] [xml] NOT NULL,
	[Json] [nvarchar](max) NOT NULL,
	[Status] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL
 CONSTRAINT [PK_WorkflowRepositories] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_WorkflowRepositories_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[WorkflowActivityLogs] ADD  CONSTRAINT [DF_WorkflowActivityLogs_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]
GO

ALTER TABLE [dbo].[WorkflowActivities]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActivities_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[WorkflowActivities] CHECK CONSTRAINT [FK_WorkflowActivities_WorkflowInstances]
GO

ALTER TABLE [dbo].[WorkflowActivityLogs]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActivityLogs_WorkflowActivities] FOREIGN KEY([IdWorkflowActivity])
REFERENCES [dbo].[WorkflowActivities] ([IdWorkflowActivity])
GO

ALTER TABLE [dbo].[WorkflowActivityLogs] CHECK CONSTRAINT [FK_WorkflowActivityLogs_WorkflowActivities]
GO

ALTER TABLE [dbo].[WorkflowInstances]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowInstances_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO

ALTER TABLE [dbo].[WorkflowInstances] CHECK CONSTRAINT [FK_WorkflowInstances_WorkflowRepositories]
GO

ALTER TABLE [dbo].[WorkflowProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowProperties_WorkflowActivities] FOREIGN KEY([IdWorkflowActivity])
REFERENCES [dbo].[WorkflowActivities] ([IdWorkflowActivity])
GO

ALTER TABLE [dbo].[WorkflowProperties] CHECK CONSTRAINT [FK_WorkflowProperties_WorkflowActivities]
GO

ALTER TABLE [dbo].[WorkflowProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowProperties_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[WorkflowProperties] CHECK CONSTRAINT [FK_WorkflowProperties_WorkflowInstances]
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
PRINT 'Creazione delle tabelle di UDS...'
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'uds')
BEGIN
	EXEC('CREATE SCHEMA uds');
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [uds].[UDSSchemaRepositories](
	[IdUDSSchemaRepository] [uniqueidentifier] NOT NULL,
	[SchemaXML] [xml] NULL,
	[Version] [smallint] NOT NULL,
	[ActiveDate] [datetimeoffset](7) NOT NULL,
	[ExpiredDate] [datetimeoffset](7) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[Timestamp] [timestamp] NULL,
 CONSTRAINT [PK_UDSSchemaRepositories] PRIMARY KEY CLUSTERED 
(
	[IdUDSSchemaRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [uds].[UDSRepositories](
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdUDSSchemaRepository] [uniqueidentifier] NOT NULL,
	[IdContainer] [smallint] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[SequenceCurrentYear] [smallint] NOT NULL,
	[SequenceCurrentNumber] [int] NOT NULL,
	[ModuleXML] [xml] NOT NULL,
	[Version] [smallint] NOT NULL,
	[ActiveDate] [datetimeoffset](7) NOT NULL,
	[Status] [smallint] NOT NULL,
	[ExpiredDate] [datetimeoffset](7) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[Timestamp] [timestamp] NULL,
 CONSTRAINT [PK_UDSRepositories] PRIMARY KEY CLUSTERED 
(
	[IdUDSRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [uds].[UDSRepositories]  WITH CHECK ADD  CONSTRAINT [FK_UDSRepositories_UDSSchemaRepositories] FOREIGN KEY([IdUDSSchemaRepository])
REFERENCES [uds].[UDSSchemaRepositories] ([IdUDSSchemaRepository])
GO

ALTER TABLE [uds].[UDSRepositories] CHECK CONSTRAINT [FK_UDSRepositories_UDSSchemaRepositories]
GO

ALTER TABLE [uds].[UDSRepositories]  WITH CHECK ADD  CONSTRAINT [FK_UDSRepositories_Container] FOREIGN KEY([IdContainer])
REFERENCES [dbo].[Container] ([idContainer])
GO

ALTER TABLE [uds].[UDSRepositories] CHECK CONSTRAINT [FK_UDSRepositories_Container]
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
PRINT 'Inserimento parametro TenantId...'
GO

IF NOT EXISTS(SELECT * FROM ParameterEnv WHERE KeyName = 'TenantId')

begin
    INSERT INTO ParameterEnv(KeyName, KeyValue) VALUES ('TenantId', NEWID())
end

ELSE
BEGIN
 PRINT 'Colonna TenantId gia modificata nella tabella ParameterEnv'
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
PRINT 'Creazione colonne UDSLocation e UDSRights...'
GO

ALTER TABLE dbo.Container ADD UDSLocation [smallint] NULL
GO

ALTER TABLE dbo.ContainerGroup ADD UDSRights [char](10) NULL
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
PRINT 'Creazione colonne IdUDS e IdUDSRepository in Protocol...'
GO

ALTER TABLE dbo.Protocol ADD IdUDS [uniqueidentifier] NULL
GO

ALTER TABLE dbo.Protocol ADD IdUDSRepository [uniqueidentifier] NULL
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
PRINT 'Creazione tabella OChartItemWorkflows...'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OChartItemWorkflows](
	[IdOChartItemWorkflow] [uniqueidentifier] NOT NULL,
	[IdOChartItem] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
 CONSTRAINT [PK_OChartItemWorkflows] PRIMARY KEY CLUSTERED 
(
	[IdOChartItemWorkflow] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[OChartItemWorkflows]  WITH CHECK ADD  CONSTRAINT [FK_OChartItemWorkflows_OChartItem] FOREIGN KEY([IdOChartItem])
REFERENCES [dbo].[OChartItem] ([Id])
GO
ALTER TABLE [dbo].[OChartItemWorkflows] CHECK CONSTRAINT [FK_OChartItemWorkflows_OChartItem]
GO
ALTER TABLE [dbo].[OChartItemWorkflows]  WITH CHECK ADD  CONSTRAINT [FK_OChartItemWorkflows_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
ALTER TABLE [dbo].[OChartItemWorkflows] CHECK CONSTRAINT [FK_OChartItemWorkflows_WorkflowRepositories]
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
PRINT 'Creazione tabella WorkflowAuthorizations...'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowAuthorizations](
	[IdWorkflowAuthorization] [uniqueidentifier] NOT NULL,
	[IdWorkflowActivity] [uniqueidentifier] NOT NULL,
	[Account] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
 CONSTRAINT [PK_WorkflowAuthorizations] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowAuthorization] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkflowAuthorizations]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowAuthorizations_WorkflowActivities] FOREIGN KEY([IdWorkflowActivity])
REFERENCES [dbo].[WorkflowActivities] ([IdWorkflowActivity])
GO
ALTER TABLE [dbo].[WorkflowAuthorizations] CHECK CONSTRAINT [FK_WorkflowAuthorizations_WorkflowActivities]
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
PRINT 'Creazione tabella WorkflowRoles...'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowRoles](
	[IdWorkflowRole] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
 CONSTRAINT [PK_WorkflowRoles] PRIMARY KEY CLUSTERED 
(
	[IdWorkflowRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkflowRoles]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowRoles_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([idRole])
GO
ALTER TABLE [dbo].[WorkflowRoles] CHECK CONSTRAINT [FK_WorkflowRoles_Role]
GO
ALTER TABLE [dbo].[WorkflowRoles]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowRoles_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
ALTER TABLE [dbo].[WorkflowRoles] CHECK CONSTRAINT [FK_WorkflowRoles_WorkflowRepositories]
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
PRINT 'Creazione colonne IdUDSRepository e IdOChartItemWorkflow in RoleUser...'
GO

ALTER TABLE dbo.RoleUser ADD IdUDSRepository [uniqueidentifier] NULL
GO

ALTER TABLE dbo.RoleUser ADD IdOChartItemWorkflow [uniqueidentifier] NULL
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