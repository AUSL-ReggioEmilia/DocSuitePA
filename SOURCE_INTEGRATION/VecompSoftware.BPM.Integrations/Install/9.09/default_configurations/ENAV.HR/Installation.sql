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
PRINT N'Creazione tabella [SKYDOC_COMMANDS]'

CREATE TABLE [dbo].[SKYDOC_COMMANDS](
	[IdCommand] [uniqueidentifier] NOT NULL,
	[Discriminator] [smallint] NOT NULL,
	[Riferimento_dossier] [nvarchar](256) NULL,
	[Riferimento_fascicolo] [nvarchar](256) NULL,
	[Mapping_Tag_Esecutore] [nvarchar](256) NULL,
	[Mapping_Tag_Autorizzato] [nvarchar](256) NULL,
	[Classificazione] [nvarchar](20) NULL,
	[Contenitore] [nvarchar](256) NULL,
	[Oggetto] [nvarchar](256) NULL,
	[Tipologia] [smallint] NULL,
	[Contatto_01] [nvarchar](256) NULL,
	[Contatto_02] [nvarchar](256) NULL,
	[Contatto_03] [nvarchar](256) NULL,
	[Contatto_04] [nvarchar](256) NULL,
	[DataInserimento] [datetimeoffset](7) NOT NULL,
	[Tenant_ID] [uniqueidentifier] NOT NULL,
	[WF_SkyDocStarted] [datetimeoffset](7) NULL,
	[WF_SkyDocID] [uniqueidentifier] NULL,
	[WF_SkyDocStatus] [smallint] NULL,
 CONSTRAINT [PK_SKYDOC_COMMANDS] PRIMARY KEY NONCLUSTERED 
(
	[IdCommand] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [IX_SKYDOC_COMMANDS_DataInserimento] 
	ON [dbo].[SKYDOC_COMMANDS] ([DataInserimento] ASC)
GO

PRINT N'Creazione tabella [SKYDOC_DOCUMENTS]'

CREATE TABLE [dbo].[SKYDOC_DOCUMENTS](
	[IdDocument] [uniqueidentifier] NOT NULL,
	[IdCommand] [uniqueidentifier] NOT NULL,
	[Discriminator] [smallint] NOT NULL,
	[Nome_File] [nvarchar](256) NULL,
	[Content] [varbinary](max) NOT NULL,
	[DataInserimento] [datetimeoffset](7) NOT NULL,
	[WF_SkyDocStarted] [datetimeoffset](7) NULL,
	[WF_SkyDocID] [uniqueidentifier] NULL,
	[WF_SkyDocStatus] [smallint] NULL,
 CONSTRAINT [PK_SKYDOC_DOCUMENTS] PRIMARY KEY NONCLUSTERED 
(
	[IdDocument] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SKYDOC_DOCUMENTS]  WITH CHECK ADD  CONSTRAINT [FK_SKYDOC_DOCUMENTS_IdCommand] FOREIGN KEY([IdCommand])
REFERENCES [dbo].[SKYDOC_COMMANDS] ([IdCommand])
GO

ALTER TABLE [dbo].[SKYDOC_DOCUMENTS] CHECK CONSTRAINT [FK_SKYDOC_DOCUMENTS_IdCommand]
GO

CREATE CLUSTERED INDEX [IX_SKYDOC_DOCUMENTS_DataInserimento] 
	ON [dbo].[SKYDOC_DOCUMENTS] ([DataInserimento] ASC)
GO

PRINT N'Creazione tabella [SKYDOC_EVENTS]'

CREATE TABLE [dbo].[SKYDOC_EVENTS](
	[IdEvent] [uniqueidentifier] NOT NULL,
	[Discriminator] [smallint] NOT NULL,
	[IdCommand] [uniqueidentifier] NULL,
	[IdentificativoUnivoco] [uniqueidentifier] NOT NULL,
	[Anno] [int] NOT NULL,
	[Numero] [nvarchar](256) NOT NULL,
	[Classificazione] [nvarchar](2000) NULL,
	[Oggetto] [nvarchar](4000) NOT NULL,
	[Data] [datetimeoffset](7) NOT NULL,
	[WF_EHCStarted] [datetimeoffset](7) NULL,
	[WF_EHCStatus] [smallint] NULL,
 CONSTRAINT [PK_SKYDOC_EVENTS] PRIMARY KEY NONCLUSTERED 
(
	[IdEvent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SKYDOC_EVENTS]  WITH CHECK ADD  CONSTRAINT [FK_SKYDOC_EVENTS_IdCommand] FOREIGN KEY([IdCommand])
REFERENCES [dbo].[SKYDOC_COMMANDS] ([IdCommand])
GO

ALTER TABLE [dbo].[SKYDOC_EVENTS] CHECK CONSTRAINT [FK_SKYDOC_EVENTS_IdCommand]
GO

CREATE CLUSTERED INDEX [IX_SKYDOC_EVENTS_Data] 
	ON [dbo].[SKYDOC_EVENTS] ([Data] ASC)
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