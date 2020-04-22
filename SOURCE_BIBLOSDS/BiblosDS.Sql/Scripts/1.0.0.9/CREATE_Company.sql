CREATE TABLE [dbo].[Company]
(
      [IdCompany] [nvarchar](255) NOT NULL,
      [RagioneSociale] [varchar](500) NULL,
      [PartitaIVA] [varchar](50) NULL,
      [CF] [varchar](50) NULL,
      [Indirizzo] [varchar](500) NULL,
      [TemplateFileChiusura] [text] NULL,
      [TemplateFileIndice] [text] NULL,
      [TemplateFileXSLT] [text] NULL,
      [PostaCertificata] [varchar](500) NULL,
	CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
	(
				[IdCompany] ASC
	)
)
