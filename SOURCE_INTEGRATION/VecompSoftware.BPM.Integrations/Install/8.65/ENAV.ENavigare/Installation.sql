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
PRINT N'Creazione tabella [dbo].[SkyDOC_DocumentSeries]'

CREATE TABLE [dbo].[SkyDOC_DocumentSeries](
	[UniqueID] [uniqueidentifier] NOT NULL,
	[Oggetto] [nvarchar](max) NOT NULL,
	[Codice] [nvarchar](256) NOT NULL,
	[Abstract] [nvarchar](1000) NOT NULL,
	[DataValiditaScheda] [datetime2](7) NULL,
	[DataUltimoAggiornamento] [datetimeoffset](7) NULL,
	[DataPubblicazione] [datetime2](7) NULL,
	[DataRitiro] [datetime2](7) NULL,
	[ProceduraNomiFile] [nvarchar](max) NULL,
	[ProceduraPosizioni] [nvarchar](256) NULL,
	[LineeGuidaNomiFile] [nvarchar](max) NULL,
	[LineeGuidaPosizioni] [nvarchar](256) NULL,
	[ModulisticaNomiFile] [nvarchar](max) NULL,
	[ModulisticaPosizioni] [nvarchar](256) NULL,
	[Url] [nvarchar](256) NULL,
	[InEvidenza] [bit] NOT NULL,
CONSTRAINT [PK_SkyDOC_DocumentSeries] PRIMARY KEY CLUSTERED 
(
       [UniqueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SkyDOC_DocumentSeries] ADD  CONSTRAINT [DF_SkyDOC_DocumentSeries_InEvidenza]  DEFAULT ((0)) FOR [InEvidenza]
GO

CREATE VIEW [dbo].[V_SkyDOC_ActiveDocumentSeries]
AS
SELECT        TOP (1000) UniqueID, Oggetto, Codice, DataPubblicazione, CONVERT(datetime2, DataUltimoAggiornamento) AS DataUltimoAggiornamento, ProceduraPosizioni, InEvidenza, Url
FROM            dbo.SkyDOC_DocumentSeries
WHERE        (GETDATE() BETWEEN DataPubblicazione AND (CASE WHEN DataRitiro IS NULL THEN GETDATE() ELSE DataRitiro END))
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