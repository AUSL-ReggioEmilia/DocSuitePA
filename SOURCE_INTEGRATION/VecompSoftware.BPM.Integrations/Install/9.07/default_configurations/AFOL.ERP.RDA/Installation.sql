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
PRINT N'Creazione tabella [PECSecurity].[PECAttachments]'

CREATE TABLE [dbo].[DOCSUITE_COMMANDS](
	[IdDocSuiteCommand] [uniqueidentifier] NOT NULL,
	[Discriminator] [smallint] NOT NULL,
	[Riferimento_RDA] [nvarchar](256) NULL,
	[Fornitore_Denominazione] [nvarchar](256) NOT NULL,
	[Fornitore_PIVACF] [nvarchar](256) NOT NULL,
	[Numero] [nvarchar](256) NOT NULL,
	[Data] [datetimeoffset](7) NOT NULL,
	[Centro_Costo] [nvarchar](256) NULL,
	[Tipologia] [nvarchar](256) NULL,
	[Area_Richiedente] [nvarchar](256) NULL,
	[CIG] [nvarchar](256) NULL,
	[DataInserimento] [datetimeoffset](7) NOT NULL,
	[Tenant_ID] [uniqueidentifier] NOT NULL,
	[WF_DocSuiteStarted] [datetimeoffset](7) NULL,
	[WF_DocSuiteID] [uniqueidentifier] NULL,
	[WF_DocSuiteStatus] [smallint] NULL,
	[Identificativo_UDS] [uniqueidentifier] NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DOCSUITE_COMMANDS] PRIMARY KEY NONCLUSTERED 
(
	[IdDocSuiteCommand] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [IX_DOCSUITE_COMMANDS_RegistrationDate] 
	ON [dbo].[DOCSUITE_COMMANDS] ([DataInserimento] ASC)
GO
CREATE TABLE [dbo].[DOCSUITE_DOCUMENTS](
	[IdDocSuiteDocument] [uniqueidentifier] NOT NULL,
	[IdDocSuiteCommand] [uniqueidentifier] NOT NULL,
	[Discriminator] [smallint] NOT NULL,
	[Nome_File] [nvarchar](256) NULL,
	[Content] [varbinary](max) NOT NULL,
	[DataInserimento] [datetimeoffset](7) NOT NULL,
	[WF_DocSuiteStarted] [datetimeoffset](7) NULL,
	[WF_DocSuiteID] [uniqueidentifier] NULL,
	[WF_DocSuiteStatus] [smallint] NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DOCSUITE_DOCUMENTS] PRIMARY KEY NONCLUSTERED 
(
	[IdDocSuiteDocument] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [IX_DOCSUITE_DOCUMENTS_RegistrationDate] 
	ON [dbo].[DOCSUITE_DOCUMENTS] ([DataInserimento] ASC)
GO
CREATE TABLE [dbo].[DOCSUITE_EVENTS](
	[IdDocSuiteEvent] [uniqueidentifier] NOT NULL,
	[Discriminator] [smallint] NOT NULL,
	[Anno] [int] NOT NULL,
	[Numero] [nvarchar](256) NOT NULL,
	[Classificazione] [nvarchar](2000) NOT NULL,
	[Oggetto] [nvarchar](4000) NOT NULL,
	[Data] [datetimeoffset](7) NOT NULL,
	[WF_DocSuiteStarted] [datetimeoffset](7) NULL,
	[WF_DocSuiteStatus] [smallint] NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DOCSUITE_EVENTS] PRIMARY KEY NONCLUSTERED 
(
	[IdDocSuiteEvent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DOCSUITE_DOCUMENTS]  WITH CHECK ADD  CONSTRAINT [FK_DOCSUITE_DOCUMENTS_IdDocSuiteCommand] FOREIGN KEY([IdDocSuiteCommand])
REFERENCES [dbo].[DOCSUITE_COMMANDS] ([IdDocSuiteCommand])
GO
ALTER TABLE [dbo].[DOCSUITE_DOCUMENTS] CHECK CONSTRAINT [FK_DOCSUITE_DOCUMENTS_IdDocSuiteCommand]
GO
CREATE CLUSTERED INDEX [IX_DOCSUITE_EVENTS_RegistrationDate] 
	ON [dbo].[DOCSUITE_EVENTS] ([Data] ASC)
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