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
PRINT N'Creazione tabella [dbo].[FATTURE_ATTIVE]'

CREATE TABLE [dbo].[FATTURE_ATTIVE](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Cliente] [nvarchar](256) NULL,
	[Numero_Fattura] [nvarchar](256) NULL,
	[Data_Fattura] [datetime2](7) NULL,
	[Registro_IVA] [nvarchar](100) NULL,
	[Protocollo_IVA] [int] NULL,
	[Data_IVA] [datetime2](7) NULL,
	[Anno_IVA] [smallint] NULL,
	[FatturaXML] [nvarchar](max) NULL,
	[FatturaFilename] [nvarchar](250) NULL,
	[DataInserimento] [datetime2](7) NULL,
	[WF_DocSuiteStarted] [datetime2](7) NULL,
	[WF_DocSuiteID] [uniqueidentifier] NULL,
	[WF_DocSuiteStatus] [smallint] NULL,
	[Identificativo_SDI] [nvarchar](100) NULL,
	[Esito_SDI] [nvarchar](25) NULL,
	[DataEsito_SDI] [datetime2](7) NULL,
	[DescrizioneEsito_SDI] [nvarchar](max) NULL,
	[Anno_Protocollo] [smallint] NULL,
	[Numero_Protocollo] [nvarchar](50) NULL,
 CONSTRAINT [PK_FATTURE_ATTIVE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
GO
--#############################################################################
PRINT N'Creazione tabella [dbo].[FATTURE_PASSIVE]'

CREATE TABLE [dbo].[FATTURE_PASSIVE](
	[WF_DocSuiteID] [uniqueidentifier] NOT NULL,
	[Fornitore] [nvarchar](256) NULL,
	[PIVA] [nvarchar](16) NULL,
	[Numero_Fattura] [nvarchar](256) NULL,
	[Data_Fattura] [datetime2](7) NULL,
	[FatturaXML] [nvarchar](max) NULL,
	[FatturaFilename] [nvarchar](250) NULL,
	[Identificativo_SDI] [nvarchar](100) NULL,
	[Esito_SDI] [nvarchar](25) NULL,
	[DataEsito_SDI] [datetime2](7) NULL,
	[DescrizioneEsito_SDI] [nvarchar](max) NULL,
	[Anno_Protocollo] [smallint] NULL,
	[Numero_Protocollo] [nvarchar](50) NULL,
	[WF_DocSuiteProcessed] [datetime2](7) NULL,
	[WF_DocSuiteStatus] [smallint] NULL,
	[Registro_IVA] [nvarchar](100) NULL,
	[Protocollo_IVA] [int] NULL,
	[Data_IVA] [datetime2](7) NULL,
	[Anno_IVA] [smallint] NULL,
	[DataAggiornamento] [datetime2](7) NULL,
	[Autofattura] [varbinary](MAX) NULL,
	[AutofatturaFilename] [nvarchar](250) NULL,
 CONSTRAINT [PK_FATTURE_PASSIVE] PRIMARY KEY CLUSTERED 
(
	[WF_DocSuiteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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