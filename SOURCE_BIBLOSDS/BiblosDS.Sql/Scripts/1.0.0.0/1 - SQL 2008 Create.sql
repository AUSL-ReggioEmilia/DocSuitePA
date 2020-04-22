-- =============================================
-- Script Template
-- =============================================
USE [BiblosDS2010]
GO
/****** Object:  Schema [admin]    Script Date: 10/17/2011 10:49:05 ******/
CREATE SCHEMA [admin] AUTHORIZATION [dbo]
GO
/****** Object:  Table [dbo].[StorageType]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageType](
	[Type] [varchar](20) NULL,
	[IdStorageType] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[StorageAssembly] [varchar](255) NOT NULL,
	[StorageClassName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_StorageType] PRIMARY KEY CLUSTERED 
(
	[IdStorageType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StorageStatus]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageStatus](
	[Status] [varchar](255) NULL,
	[IdStorageStatus] [smallint] NOT NULL,
 CONSTRAINT [PK_StorageStatus] PRIMARY KEY CLUSTERED 
(
	[IdStorageStatus] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[Compatibility_CreateAttributes]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[Compatibility_CreateAttributes] 
	@IsLegal bit, 
	@OldBiblosDSArchive varchar(255), 
	@NewArchiveGuid varchar(255) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION

    -- dato che il nome della db è parametrico 
	-- si deve comporre il comando e quindi eseguirlo

	DECLARE @cmd AS NVARCHAR(4000)

	-- Import degli attributi 
if @IsLegal = 1
begin
SET @cmd = N'INSERT INTO Attributes 
		SELECT  newid() AS IdAttribute, 
				Nome as Name, 
				''' + @NewArchiveGuid + ''' as IdArchive, 
				Obbligatorio as IsRequired, 
				PosizioneInChiaveUnivoca as KeyOrder, 
			    CASE (Disabilitato) 
					WHEN 1 THEN 0 
					ELSE CASE (Obbligatorio) 
							WHEN 0 THEN 1 
							ELSE
								CASE (Modificabile) 
									WHEN 0 THEN 2 
									ELSE 3
								END
						 END
				END as IdMode, 
				DataPrincipale as IsMainDate, 
                ProgressivoIntero as IsEnumerator, 
                NULL as IsAutoInc, 
                Null as IsUnique,
				TipoCampo as AttributeType,
                PosizioneInFileChiusura as ConservationPosition, 
				PorzioneInChiaveUnivoca as KeyFilter, 
				FormatoInChiaveUnivoca as KeyFormat, 
				Validazione as Validation, 
				Formato as Format,  
				Null as IsChainAttribute 
		FROM  [' + @OldBiblosDSArchive +'].dbo.AttributoOggetto'
		
		EXEC sp_executesql @cmd

		SET @cmd = N'INSERT INTO ParameterConservazione SELECT 
				''' + @NewArchiveGuid + ''' as IdArchive, 
				Nome as Label, 
				Valore as Value 
		FROM  [' + @OldBiblosDSArchive+ '].dbo.Parametro'

		EXEC sp_executesql @cmd
		
		SET @cmd = N'INSERT INTO RoleConservazione SELECT 
				newid() as IdRoleC,
				Nome as Name, 
                Attivo as Enable, 
				RiceveChiusureTask as AlertEnable 
		FROM [' + @OldBiblosDSArchive + '].dbo.Ruolo' 

		EXEC sp_executesql @cmd

		SET @cmd = N'INSERT INTO UserConservazione SELECT 
						newid() as IdUserC, 
						Nome as Name,
						Cognome as Surname, 
						CodiceFiscale as FiscalId, 
						Residenza as Address, 
						eMail as Email,
						Attivo as Enable,
						DomainUser 
				FROM ['+ @OldBiblosDSArchive +'].dbo.Soggetto'

		EXEC sp_executesql @cmd

end
else
begin
	SET @cmd = N'INSERT INTO Attributes  (
			[IdAttribute]
           ,[Name]
           ,[IdArchive]
           ,[IsRequired]
           ,[KeyOrder]
           ,[IdMode]
           ,[IsMainDate]
           ,[IsEnumerator]
           ,[IsAutoInc]
           ,[IsUnique]
           ,[AttributeType]
           ,[ConservationPosition]                      
           ,[KeyFilter]
           ,[KeyFormat]
           ,[Validation]
           ,[Format]
           ,[IsChainAttribute]
           )
		SELECT  newid() AS IdAttribute, 
				Nome as Name, 
				''' + @NewArchiveGuid + ''' as IdArchive, 
				0 as IsRequired, 
				Null as KeyOrder, 
			    0 as IdMode, 
				Null as IsMainDate, 
                Null as IsEnumerator, 
                Null as IsAutoInc, 
                Null as IsUnique,
				''System.String'' as AttributeType,
                Null as ConservationPosition, 
				Null as KeyFilter, 
				Null as KeyFormat, 
				Null as Validation, 
				Null as Format,
				Null as IsChainAttribute   
		FROM  [' + @OldBiblosDSArchive +'].dbo.AttributoOggetto
		where Nome not in (Select cast(Name as varchar(255)) collate Latin1_General_CI_AS from Attributes where IdArchive = '''+@NewArchiveGuid+''' and AttributoOggetto.Nome = cast(Attributes.Name as varchar(255)) collate Latin1_General_CI_AS )'
		EXEC sp_executesql @cmd
end





--SET @cmd = N'INSERT INTO RoleUserConservazione SELECT 
--				' + @NewArchiveGuid + ' as IdArchive, 
--				(SELECT IdUserC FROM UserConservazione UC WHERE UC.CodiceFiscale = (SELECT SI.CodiceFiscale FROM [' + @OldBiblosDSArchive + '].dbo.Soggetto SI WHERE SI.IdSoggetto = RS.IdSoggetto)) AS IdUserC, 
--				(SELECT IdRoleC FROM RoleConservazione RC WHERE RC.Name = (SELECT RI.Nome FROM [' + @OldBiblosDSArchive + '].dbo.Ruolo RI WHERE RI.Nome = RS.Nome)) AS IdRoleC
--		FROM [' + @OldBiblosDSArchive + '].dbo.RuoloSoggetto RS'
--
--EXEC sp_executesql @cmd

	COMMIT				

END
GO
/****** Object:  Table [dbo].[CertificateStore]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CertificateStore](
	[IdCertificate] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Name] [varchar](255) NULL,
	[IsDefault] [smallint] NOT NULL,
 CONSTRAINT [PK_CertificateStore] PRIMARY KEY CLUSTERED 
(
	[IdCertificate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_PreservationAlertRole]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_PreservationAlertRole](
	[IdPreservationAlertRole] [uniqueidentifier] NOT NULL,
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AlertRole] PRIMARY KEY CLUSTERED 
(
	[IdPreservationAlertRole] ASC,
	[IdPreservationRole] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AttributesMode]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttributesMode](
	[IdMode] [int] NOT NULL,
	[Description] [varchar](255) NULL,
 CONSTRAINT [PK_AttributesStatus] PRIMARY KEY CLUSTERED 
(
	[IdMode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttributesGroup]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttributesGroup](
	[IdAttributeGroup] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IdAttributeGroupType] [int] NOT NULL,
	[Description] [varchar](250) NULL,
	[IsVisible] [smallint] NULL,
 CONSTRAINT [PK_AttributeGroup_1] PRIMARY KEY CLUSTERED 
(
	[IdAttributeGroup] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Archive]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Archive](
	[IdArchive] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[IsLegal] [smallint] NOT NULL,
	[MaxCache] [bigint] NULL,
	[UpperCache] [bigint] NULL,
	[LowerCache] [bigint] NULL,
	[LastIdBiblos] [int] NOT NULL,
	[AutoVersion] [smallint] NULL,
	[AuthorizationAssembly] [varchar](255) NULL,
	[AuthorizationClassName] [varchar](50) NULL,
	[EnableSecurity] [smallint] NULL,
	[PathTransito] [text] NULL,
	[PathCache] [text] NULL,
	[PathPreservation] [text] NULL,
	[LastAutoIncValue] [bigint] NULL,
	[ThumbnailEnabled] [bit] NULL,
	[PdfConversionEnabled] [bit] NULL,
	[FullSignEnabled] [bit] NULL,
	[VerifyPreservationDateEnabled] [bit] NULL,
	[VerifyPreservationIncrementalEnabled] [bit] NULL,
	[TransitoEnabled] [bit] NULL,
 CONSTRAINT [PK_Archive] PRIMARY KEY CLUSTERED 
(
	[IdArchive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentStatus]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentStatus](
	[IdDocumentStatus] [smallint] NOT NULL,
	[Description] [varchar](255) NULL,
 CONSTRAINT [PK_DocumentStatus] PRIMARY KEY CLUSTERED 
(
	[IdDocumentStatus] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentNodeType]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentNodeType](
	[Id] [smallint] NOT NULL,
	[Description] [varchar](255) NULL,
 CONSTRAINT [PK_DocumentNodeType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentCache]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentCache](
	[IdDocument] [uniqueidentifier] NOT NULL,
	[ServerName] [varchar](50) NOT NULL,
	[FileName] [varchar](250) NOT NULL,
	[Signature] [varchar](250) NULL,
	[DateCreated] [datetime] NULL,
	[Size] [float] NULL,
 CONSTRAINT [PK_DocumentCache] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC,
	[ServerName] ASC,
	[FileName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[DataBaseVersion]    Script Date: 10/17/2011 10:48:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[DataBaseVersion]
AS
SELECT     '1.100' AS LocalPath
GO
/****** Object:  Table [dbo].[Component]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Component](
	[IdComponent] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
	[Server] [varchar](50) NULL,
	[InstallPath] [varchar](250) NULL,
	[Enable] [smallint] NOT NULL,
 CONSTRAINT [PK_Component] PRIMARY KEY CLUSTERED 
(
	[IdComponent] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OperationType]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OperationType](
	[Type] [varchar](255) NULL,
	[IdOperationType] [smallint] NOT NULL,
 CONSTRAINT [PK_OperationType_1] PRIMARY KEY CLUSTERED 
(
	[IdOperationType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Log]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Log](
	[IdEntry] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdOperationType] [smallint] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[TimeStamp] [datetime] NULL,
	[IdStorage] [uniqueidentifier] NOT NULL,
	[IdCorrelation] [uniqueidentifier] NULL,
	[Message] [text] NULL,
	[Server] [varchar](255) NOT NULL,
	[Client] [varchar](255) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[IdEntry] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[KeyValue]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[KeyValue](
	[KeyValue] [varchar](255) NULL,
	[IdDocument] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_KeyValue] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Journal]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Journal](
	[IdEntry] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdOperationType] [smallint] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[UserAgent] [varchar](255) NULL,
	[TimeStamp] [datetime] NULL,
	[IdStorage] [uniqueidentifier] NOT NULL,
	[IdCorrelation] [uniqueidentifier] NULL,
	[Message] [text] NULL,
 CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED 
(
	[IdEntry] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[InizializeMigrateArchiveDSConfig]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InizializeMigrateArchiveDSConfig] 
	@IsLegal bit, 
	@OldBiblosDSArchive varchar(255), 
	@NewArchiveGuid varchar(255) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION

    -- dato che il nome della db è parametrico 
	-- si deve comporre il comando e quindi eseguirlo

	DECLARE @cmd AS NVARCHAR(1000)

	-- Import degli attributi 
if @IsLegal = 1
begin
SET @cmd = N'INSERT INTO Attributes 
		SELECT  newid() AS IdAttribute, 
				Nome as Name, 
				''' + @NewArchiveGuid + ''' as IdArchive, 
				Obbligatorio as IsRequired, 
				PosizioneInChiaveUnivoca as KeyOrder, 
			    CASE (Disabilitato) 
					WHEN 1 THEN 0 
					ELSE CASE (Obbligatorio) 
							WHEN 0 THEN 1 
							ELSE
								CASE (Modificabile) 
									WHEN 0 THEN 2 
									ELSE 3
								END
						 END
				END as IdMode, 
				DataPrincipale as IsMainDate, 
                ProgressivoIntero as IsEnumerator, 
                NULL as IsAutoInc, 
                Null as IsUnique,
				TipoCampo as AttributeType,
                PosizioneInFileChiusura as ConservationPosition, 
				PorzioneInChiaveUnivoca as KeyFilter, 
				FormatoInChiaveUnivoca as KeyFormat, 
				Validazione as Validation, 
				Formato as Format,  
				Null as IsChainAttribute 
		FROM  [' + @OldBiblosDSArchive +'].dbo.AttributoOggetto'
		
		EXEC sp_executesql @cmd

		SET @cmd = N'INSERT INTO ParameterConservazione SELECT 
				''' + @NewArchiveGuid + ''' as IdArchive, 
				Nome as Label, 
				Valore as Value 
		FROM  [' + @OldBiblosDSArchive+ '].dbo.Parametro'

		EXEC sp_executesql @cmd
		
		SET @cmd = N'INSERT INTO RoleConservazione SELECT 
				newid() as IdRoleC,
				Nome as Name, 
                Attivo as Enable, 
				RiceveChiusureTask as AlertEnable 
		FROM [' + @OldBiblosDSArchive + '].dbo.Ruolo' 

		EXEC sp_executesql @cmd

		SET @cmd = N'INSERT INTO UserConservazione SELECT 
						newid() as IdUserC, 
						Nome as Name,
						Cognome as Surname, 
						CodiceFiscale as FiscalId, 
						Residenza as Address, 
						eMail as Email,
						Attivo as Enable,
						DomainUser 
				FROM ['+ @OldBiblosDSArchive +'].dbo.Soggetto'

		EXEC sp_executesql @cmd

end
else
begin
	SET @cmd = N'INSERT INTO Attributes 
		SELECT  newid() AS IdAttribute, 
				Nome as Name, 
				''' + @NewArchiveGuid + ''' as IdArchive, 
				0 as IsRequired, 
				Null as KeyOrder, 
			    0 as IdMode, 
				Null as IsMainDate, 
                Null as IsEnumerator, 
                Null as IsAutoInc, 
                Null as IsUnique,
				''System.String'' as AttributeType,
                Null as ConservationPosition, 
				Null as KeyFilter, 
				Null as KeyFormat, 
				Null as Validation, 
				Null as Format,
				Null as IsChainAttribute   
		FROM  [' + @OldBiblosDSArchive +'].dbo.AttributoOggetto'
		
		EXEC sp_executesql @cmd
end





--SET @cmd = N'INSERT INTO RoleUserConservazione SELECT 
--				' + @NewArchiveGuid + ' as IdArchive, 
--				(SELECT IdUserC FROM UserConservazione UC WHERE UC.CodiceFiscale = (SELECT SI.CodiceFiscale FROM [' + @OldBiblosDSArchive + '].dbo.Soggetto SI WHERE SI.IdSoggetto = RS.IdSoggetto)) AS IdUserC, 
--				(SELECT IdRoleC FROM RoleConservazione RC WHERE RC.Name = (SELECT RI.Nome FROM [' + @OldBiblosDSArchive + '].dbo.Ruolo RI WHERE RI.Nome = RS.Nome)) AS IdRoleC
--		FROM [' + @OldBiblosDSArchive + '].dbo.RuoloSoggetto RS'
--
--EXEC sp_executesql @cmd

	COMMIT				

END
GO
/****** Object:  StoredProcedure [admin].[InizializeMigrateArchiveDSConfig]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [admin].[InizializeMigrateArchiveDSConfig] 
	@IsLegal bit, 
	@OldBiblosDSArchive varchar(255), 
	@NewArchiveGuid varchar(255) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION

    -- dato che il nome della db è parametrico 
	-- si deve comporre il comando e quindi eseguirlo

	DECLARE @cmd AS NVARCHAR(4000)

	-- Import degli attributi 
if @IsLegal = 1
begin
SET @cmd = N'INSERT INTO Attributes 
		SELECT  newid() AS IdAttribute, 
				Nome as Name, 
				''' + @NewArchiveGuid + ''' as IdArchive, 
				Obbligatorio as IsRequired, 
				PosizioneInChiaveUnivoca as KeyOrder, 
			    CASE (Disabilitato) 
					WHEN 1 THEN 0 
					ELSE CASE (Obbligatorio) 
							WHEN 0 THEN 1 
							ELSE
								CASE (Modificabile) 
									WHEN 0 THEN 2 
									ELSE 3
								END
						 END
				END as IdMode, 
				DataPrincipale as IsMainDate, 
                ProgressivoIntero as IsEnumerator, 
                NULL as IsAutoInc, 
                Null as IsUnique,
				TipoCampo as AttributeType,
                PosizioneInFileChiusura as ConservationPosition, 
				PorzioneInChiaveUnivoca as KeyFilter, 
				FormatoInChiaveUnivoca as KeyFormat, 
				Validazione as Validation, 
				Formato as Format,  
				Null as IsChainAttribute 
		FROM  [' + @OldBiblosDSArchive +'].dbo.AttributoOggetto'
		
		EXEC sp_executesql @cmd

		SET @cmd = N'INSERT INTO ParameterConservazione SELECT 
				''' + @NewArchiveGuid + ''' as IdArchive, 
				Nome as Label, 
				Valore as Value 
		FROM  [' + @OldBiblosDSArchive+ '].dbo.Parametro'

		EXEC sp_executesql @cmd
		
		SET @cmd = N'INSERT INTO RoleConservazione SELECT 
				newid() as IdRoleC,
				Nome as Name, 
                Attivo as Enable, 
				RiceveChiusureTask as AlertEnable 
		FROM [' + @OldBiblosDSArchive + '].dbo.Ruolo' 

		EXEC sp_executesql @cmd

		SET @cmd = N'INSERT INTO UserConservazione SELECT 
						newid() as IdUserC, 
						Nome as Name,
						Cognome as Surname, 
						CodiceFiscale as FiscalId, 
						Residenza as Address, 
						eMail as Email,
						Attivo as Enable,
						DomainUser 
				FROM ['+ @OldBiblosDSArchive +'].dbo.Soggetto'

		EXEC sp_executesql @cmd

end
else
begin
	SET @cmd = N'INSERT INTO Attributes  (
			[IdAttribute]
           ,[Name]
           ,[IdArchive]
           ,[IsRequired]
           ,[KeyOrder]
           ,[IdMode]
           ,[IsMainDate]
           ,[IsEnumerator]
           ,[IsAutoInc]
           ,[IsUnique]
           ,[AttributeType]
           ,[ConservationPosition]                      
           ,[KeyFilter]
           ,[KeyFormat]
           ,[Validation]
           ,[Format]
           ,[IsChainAttribute]
           )
		SELECT  newid() AS IdAttribute, 
				Nome as Name, 
				''' + @NewArchiveGuid + ''' as IdArchive, 
				0 as IsRequired, 
				Null as KeyOrder, 
			    0 as IdMode, 
				Null as IsMainDate, 
                Null as IsEnumerator, 
                Null as IsAutoInc, 
                Null as IsUnique,
				''System.String'' as AttributeType,
                Null as ConservationPosition, 
				Null as KeyFilter, 
				Null as KeyFormat, 
				Null as Validation, 
				Null as Format,
				Null as IsChainAttribute   
		FROM  [' + @OldBiblosDSArchive +'].dbo.AttributoOggetto'
		
		EXEC sp_executesql @cmd
end





--SET @cmd = N'INSERT INTO RoleUserConservazione SELECT 
--				' + @NewArchiveGuid + ' as IdArchive, 
--				(SELECT IdUserC FROM UserConservazione UC WHERE UC.CodiceFiscale = (SELECT SI.CodiceFiscale FROM [' + @OldBiblosDSArchive + '].dbo.Soggetto SI WHERE SI.IdSoggetto = RS.IdSoggetto)) AS IdUserC, 
--				(SELECT IdRoleC FROM RoleConservazione RC WHERE RC.Name = (SELECT RI.Nome FROM [' + @OldBiblosDSArchive + '].dbo.Ruolo RI WHERE RI.Nome = RS.Nome)) AS IdRoleC
--		FROM [' + @OldBiblosDSArchive + '].dbo.RuoloSoggetto RS'
--
--EXEC sp_executesql @cmd

	COMMIT				

END
GO
/****** Object:  Table [dbo].[PreservationTaskStatus]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTaskStatus](
	[IdPreservationTaskStatus] [uniqueidentifier] NOT NULL,
	[Status] [varchar](50) NOT NULL,
 CONSTRAINT [PK_PreservationTaskStatus] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskStatus] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationStorageDeviceStatus]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationStorageDeviceStatus](
	[IdPreservationStorageDeviceStatus] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[KeyCode] [varchar](100) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_PreservationStorageDeviceStatus] PRIMARY KEY CLUSTERED 
(
	[IdPreservationStorageDeviceStatus] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RuleOperator]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RuleOperator](
	[IdRuleOperator] [int] NOT NULL,
	[Descrizione] [varchar](50) NULL,
 CONSTRAINT [PK_RuleOperator] PRIMARY KEY CLUSTERED 
(
	[IdRuleOperator] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationTaskGroupType]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTaskGroupType](
	[IdPreservationTaskGroupType] [uniqueidentifier] NOT NULL,
	[Description] [varchar](255) NOT NULL,
 CONSTRAINT [PK_PreservationTaskGroupType_1] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskGroupType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationUser]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationUser](
	[IdPreservationUser] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Surname] [varchar](255) NOT NULL,
	[FiscalId] [char](16) NOT NULL,
	[Address] [varchar](255) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[Enable] [bit] NOT NULL,
	[DomainUser] [varchar](255) NOT NULL,
 CONSTRAINT [PK_PreservationUser] PRIMARY KEY CLUSTERED 
(
	[IdPreservationUser] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationHolidays]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationHolidays](
	[IdPreservationHolidays] [uniqueidentifier] NOT NULL,
	[HolidayDate] [smalldatetime] NOT NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_PreservationHolidays] PRIMARY KEY CLUSTERED 
(
	[IdPreservationHolidays] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PreservationHolidays] UNIQUE NONCLUSTERED 
(
	[HolidayDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationExceptionType]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationExceptionType](
	[IdPreservationExceptionType] [uniqueidentifier] NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[IsFail] [bit] NOT NULL,
 CONSTRAINT [PK_ExceptionType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationExceptionType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PermissionMode]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PermissionMode](
	[IdMode] [smallint] NOT NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_PermissionMode] PRIMARY KEY CLUSTERED 
(
	[IdMode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PermissionAllowed]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PermissionAllowed](
	[IdPermission] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NULL,
	[IdMode] [smallint] NULL,
 CONSTRAINT [PK_PermissionAllowed] PRIMARY KEY CLUSTERED 
(
	[IdPermission] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationJournalingActivity]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationJournalingActivity](
	[IdPreservationJournalingActivity] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[KeyCode] [varchar](50) NOT NULL,
	[Description] [varchar](250) NOT NULL,
	[IsUserActivity] [bit] NULL,
	[IsUserDeleteEnable] [bit] NULL,
 CONSTRAINT [PK_PreservationJournalingActivity] PRIMARY KEY CLUSTERED 
(
	[IdPreservationJournalingActivity] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationSchedule]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationSchedule](
	[IdPreservationSchedule] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Period] [varchar](255) NULL,
	[ValidWeekDays] [varchar](255) NULL,
	[FrequencyType] [smallint] NOT NULL,
	[Active] [tinyint] NOT NULL,
 CONSTRAINT [PK_PreservationSchedule_1] PRIMARY KEY CLUSTERED 
(
	[IdPreservationSchedule] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'es. 5|15|30' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PreservationSchedule', @level2type=N'COLUMN',@level2name=N'Period'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'es. 1|2|3|4|5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PreservationSchedule', @level2type=N'COLUMN',@level2name=N'ValidWeekDays'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0 = Cadenzato, 1 = Giornaliero, 2 = Settimanale, 3 = Mensile' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PreservationSchedule', @level2type=N'COLUMN',@level2name=N'FrequencyType'
GO
/****** Object:  Table [dbo].[PreservationRole]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationRole](
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Enable] [bit] NOT NULL,
	[AlertEnable] [bit] NOT NULL,
	[KeyCode] [smallint] NOT NULL,
 CONSTRAINT [PK_RoleConservazione] PRIMARY KEY CLUSTERED 
(
	[IdPreservationRole] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationPeriod]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationPeriod](
	[IdPreservationPeriod] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Periods] [varchar](255) NOT NULL,
	[DayofWeek] [varchar](255) NOT NULL,
	[Enable] [smallint] NOT NULL,
 CONSTRAINT [PK_PreservationPeriod] PRIMARY KEY CLUSTERED 
(
	[IdPreservationPeriod] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationParameters]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationParameters](
	[IdArchive] [uniqueidentifier] NOT NULL,
	[Label] [varchar](50) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_PreservationParameters] PRIMARY KEY CLUSTERED 
(
	[IdArchive] ASC,
	[Label] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationException]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationException](
	[IdPreservationException] [uniqueidentifier] NOT NULL,
	[IdPreservationExceptionType] [uniqueidentifier] NULL,
	[IdPreservationExceptionCorrelated] [uniqueidentifier] NULL,
	[KeyName] [varchar](50) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[IsBlocked] [bit] NULL,
	[IdCompatibility] [int] NULL,
 CONSTRAINT [PK_PreservationException] PRIMARY KEY CLUSTERED 
(
	[IdPreservationException] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationCompany]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationCompany](
	[IdArchive] [uniqueidentifier] NOT NULL,
	[RagioneSociale] [varchar](255) NOT NULL,
	[Indirizzo] [varchar](255) NOT NULL,
	[PIVA] [varchar](50) NOT NULL,
	[NomeEsteso] [varchar](255) NOT NULL,
	[TemplateFileChiusura] [varchar](255) NOT NULL,
	[TipoArchivio] [varchar](255) NOT NULL,
	[CF] [varchar](255) NULL,
	[PostaCertificata] [varchar](255) NULL,
 CONSTRAINT [PK_ParameterConservazione] PRIMARY KEY CLUSTERED 
(
	[IdArchive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationAlertType]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationAlertType](
	[IdPreservationAlertType] [uniqueidentifier] NOT NULL,
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
	[IdPreservationConsole] [int] NULL,
	[AlertText] [varchar](255) NOT NULL,
	[Offset] [smallint] NOT NULL,
 CONSTRAINT [PK_AlertType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationAlertType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationTaskType]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTaskType](
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[Period] [smallint] NOT NULL,
	[IdPreservationPeriod] [uniqueidentifier] NULL,
 CONSTRAINT [PK_PreservationTaskType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationTaskGroup]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTaskGroup](
	[IdPreservationTaskGroup] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskGroupType] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[IdPreservationUser] [uniqueidentifier] NOT NULL,
	[IdPreservationSchedule] [uniqueidentifier] NOT NULL,
	[Expiry] [datetime] NOT NULL,
	[EstimatedExpiry] [datetime] NULL,
	[Closed] [datetime] NULL,
 CONSTRAINT [PK_PreservationTaskGroup] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskGroup] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationUserRole]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationUserRole](
	[IdPreservationUserRole] [uniqueidentifier] NOT NULL,
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
	[IdPreservationUser] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NULL,
 CONSTRAINT [PK_PreservationUserRole] PRIMARY KEY CLUSTERED 
(
	[IdPreservationUserRole] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreservationStorageDevice]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationStorageDevice](
	[IdPreservationStorageDevice] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdPreservationStorageDeviceOriginal] [uniqueidentifier] NULL,
	[IdPreservationStorageDeviceStatus] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[Label] [varchar](255) NOT NULL,
	[Location] [varchar](255) NULL,
	[DateStorageDevice] [datetime] NULL,
	[DateCreated] [datetime] NULL,
	[DomainUser] [varchar](255) NOT NULL,
	[LastVerifyDate] [datetime] NULL,
 CONSTRAINT [PK_PreservationStorageDevice] PRIMARY KEY CLUSTERED 
(
	[IdPreservationStorageDevice] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Storage]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Storage](
	[IdStorage] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdStorageType] [uniqueidentifier] NOT NULL,
	[MainPath] [varchar](255) NULL,
	[Name] [varchar](255) NULL,
	[StorageRuleAssembly] [varchar](255) NULL,
	[StorageRuleClassName] [varchar](255) NULL,
	[Priority] [int] NULL,
	[EnableFulText] [smallint] NOT NULL,
	[AuthenticationKey] [varchar](250) NULL,
	[AuthenticationPassword] [varchar](250) NULL,
	[IsVisible] [smallint] NULL,
 CONSTRAINT [PK_Storage] PRIMARY KEY CLUSTERED 
(
	[IdStorage] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [admin].[InizializeBiblosDS]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [admin].[InizializeBiblosDS]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert StorageTypes 
	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
		VALUES ('SqlServer', newid(), 'BiblosDS.Library.Storage.SQL', 'SQL2008Storage')

	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
		VALUES ('SharePoint', newid(), 'BiblosDS.Library.Storage.SharePoint', 'SharePointStorage')

	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
		VALUES ('Legacy	BiblosDS', newid(), 'BiblosDS.Library.Storage.FileSystem', 'FileSystem')

	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
		VALUES ('Window Azure', newid(), 'BiblosDS.Library.Storage.Azure', 'AzureStorage')

	INSERT INTO TaskType (IdTaskType, Description, Period)
		VALUES (newid(), 'Conservazione Ciclo Attivo', 15)

	INSERT INTO TaskType (IdTaskType, Description, Period)
		VALUES (newid(), 'Conservazione Ciclo Passivo', 30)
	
	INSERT INTO TaskType (IdTaskType, Description, Period)
		VALUES (newid(), 'Verifica Certificati e Marche', 180)
	
	INSERT INTO TaskType (IdTaskType, Description, Period)
		VALUES (newid(), 'Verifica Supporti Legali', 30)

	INSERT INTO AttributesMode (IdMode, Description) 
		VALUES (0, 'ReadOnly after insert') 

	INSERT INTO AttributesMode (IdMode, Description) 
		VALUES (1, 'Modify if Empty') 

	INSERT INTO AttributesMode (IdMode, Description) 
		VALUES (2, 'Modify if not Archived')
		
	INSERT INTO AttributesMode (IdMode, Description) 
		VALUES (2, 'Modify Always')

	INSERT INTO ExceptionTypeConservazione (IdExceptionConservazioneType, IsFail, Description) 
		 VALUES ( 0 , 0,	'Nessun Errore') 

	INSERT INTO ExceptionTypeConservazione (IdExceptionConservazioneType, IsFail, Description) 
		 VALUES ( 1, 0,	'Manca Valore Chiave Univoca')
	
	INSERT INTO ExceptionTypeConservazione (IdExceptionConservazioneType, IsFail, Description) 
		 VALUES ( 2, 0,	'Validazione Fallita')

	INSERT INTO ExceptionTypeConservazione (IdExceptionConservazioneType, IsFail, Description) 
		 VALUES ( 3, 0,	'Chiave Univoca Duplicata')

	INSERT INTO ExceptionTypeConservazione (IdExceptionConservazioneType, IsFail, Description) 
		 VALUES ( 4, 0,	'Numerazione Progressiva Errata')

	INSERT INTO ExceptionTypeConservazione (IdExceptionConservazioneType, IsFail, Description) 
		 VALUES ( 5, 1,	'Manca Valore Campo Obbligatorio')

	DECLARE @guid15g as uniqueidentifier
	SET @guid15g = newid() 

	INSERT INTO PeriodConservazione (IdPeriodC, Name, Periods, DayOfWeek, Enable) 
		 VALUES (newid(), 'Decade', '10|20|31', '1|2|3|4|5', 1) 

	INSERT INTO PeriodConservazione (IdPeriodC, Name, Periods, DayOfWeek, Enable) 
		 VALUES (@guid15g, 'Quindicinale', '15|31', '1|2|3|4|5', 1)

	INSERT INTO PeriodConservazione (IdPeriodC, Name, Periods, DayOfWeek, Enable) 
		 VALUES (newid(), 'Mensile', '31', '1|2|3|4|5', 1)

	INSERT INTO PeriodConservazione (IdPeriodC, Name, Periods, DayOfWeek, Enable) 
		 VALUES (newid(), 'Semestrale', '0630|1231', '1|2|3|4|5', 1)

	INSERT INTO PeriodConservazione (IdPeriodC, Name, Periods, DayOfWeek, Enable) 
		 VALUES (newid(), 'Annuale', '1231', '1|2|3|4|5', 1)

	INSERT INTO TaskType (IdTaskType, Description, IdPeriodC, Period) 
		 VALUES (newid(), 'Conservazione Sostitutiva', @guid15g, 0) 

	INSERT INTO AlertType (IdAlertType, AlertText, Offset) 
		 VALUES (newid(), 'Conservazione da eseguire', 5)

	INSERT INTO AlertType (IdAlertType, AlertText, Offset) 
		 VALUES (newid(), 'Verifica conservazioni da eseguire', 20)

	INSERT INTO AlertType (IdAlertType, AlertText, Offset) 
		 VALUES (newid(), 'Controllo validità certificati informatici da eseguire', 20)
		 
		 
  insert into dbo.DocumentStatus(IdDocumentStatus, Description) values(1, 'Undefined')
  insert into dbo.DocumentStatus(IdDocumentStatus, Description) values(2, 'In Transito')
  insert into dbo.DocumentStatus(IdDocumentStatus, Description) values(3, 'In Storage')
  insert into dbo.DocumentStatus(IdDocumentStatus, Description) values(4, 'In Cache')
  
  
  
  insert into dbo.PermissionMode(IdMode, [Description]) values(-1, 'Full contrl')
  insert into dbo.PermissionMode(IdMode, [Description]) values(1, 'Read')
  insert into dbo.PermissionMode(IdMode, [Description]) values(2, 'Write')
  insert into dbo.PermissionMode(IdMode, [Description]) values(3, 'Modify')
  
  insert CertificateStore( IdCertificate, CertificateName, IsDefault) values(NEWID(), 'BiblosDS', 1)
  
  
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(0 ,'Is Equal')
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(1 ,'Is Greather Than')
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(2 ,'Is Greather Or Equal Than')
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(3 ,'Is Less Than')
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(4 ,'Is Less Or Equal Than')   	
 	
END
GO
/****** Object:  StoredProcedure [dbo].[ImportFromPreservation]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[ImportFromPreservation] AS
DECLARE 
	@DestinationDb	AS VARCHAR(100) = 'BiblosDs2010',
	@SourceDb		AS VARCHAR(100) = 'BIBLFTAT',
	@Collation		AS VARCHAR(256) = 'Latin1_General_CI_AS',
	@IdArchive		AS VARCHAR(38),
	@Sql			AS NVARCHAR(MAX),
	@Simulation		AS BIT = 1
BEGIN

	IF ISNULL(@DestinationDb, '') = '' OR ISNULL(@SourceDb, '') = ''
	BEGIN
		RAISERROR('Specificare un database d''origine ed uno di destinazione.', 18, 1)
		RETURN
	END
	
	BEGIN TRANSACTION TR_Cleanup
		
		BEGIN TRY
		
			PRINT 'Pulizia pre-importazione.'
			--TODO: Implementare!
			
		END TRY
		BEGIN CATCH
			
			--Un po' di messaggi.
			PRINT 'ERRORE: ' + ERROR_MESSAGE()
			PRINT 'Procedura interrotta prematuramente.'
			--Rollback della transazione (è inutile importare i dati per metà!).
			ROLLBACK TRANSACTION TR_Cleanup
			--Interrompe l'esecuzione della procedura e torna al chiamante.
			RETURN
			
		END CATCH
		/***/
		IF @Simulation = 1 
			ROLLBACK TRANSACTION TR_Cleanup
		ELSE
			COMMIT TRANSACTION TR_Cleanup

	BEGIN TRANSACTION TR_DataTransfer
	
		BEGIN TRY
			PRINT 'Inizio procedura d''importazione dati.'
			/*********************************************/
			
			PRINT 'Creazione ARCHIVIO con il seguente nome: ' + @SourceDb
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[Archive] ('
						+ 'IdArchive,'
						+ 'Name,'
						+ 'IsLegal,'
						+ 'MaxCache,'
						+ 'UpperCache,'
						+ 'LowerCache,'
						+ 'LastIdBiblos,'
						+ 'EnableSecurity,'
						+ 'PathPreservation'
						+ ') '
						+ 'SELECT NEWID(), ''' + @SourceDb + ''', 1, 104857600, 83886080, 52428800, 0, 0, ''C:\Lavori\Docs\BiblosDS\WorkingDir'''
						+ 'WHERE NOT EXISTS (SELECT IdArchive FROM [' + @DestinationDb + '].[dbo].[Archive] WHERE RTRIM(LTRIM(Name)) = RTRIM(LTRIM(''' + @SourceDb + ''')))'
			
			EXEC (@Sql)
			
			DECLARE @TmpArchive TABLE (IdArchive VARCHAR(36) NOT NULL)
			
			INSERT INTO @TmpArchive
			SELECT TOP 1 CAST(IdArchive AS VARCHAR(40))
			FROM BiblosDs2010.dbo.Archive 
			WHERE Name = @SourceDb
			
			DECLARE Cur 
			CURSOR 
				LOCAL 
				READ_ONLY
				FAST_FORWARD 
			FOR
				SELECT TOP 1 IdArchive FROM @TmpArchive
				
			OPEN Cur
			
			FETCH Cur INTO @IdArchive
			
			CLOSE Cur
			
			DEALLOCATE Cur
			
			DELETE @TmpArchive
			
			SET @IdArchive = '''' + @IdArchive + ''''			
			
			PRINT 'Importazione ATTRIBUTI (Archivio)'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[Attributes]('
						+ 'IdAttribute,'
						+ 'Name,'
						+ 'IdArchive,'
						+ 'IsRequired,'
						+ 'KeyOrder,'
						+ 'IdMode,'
						+ 'IsMainDate,'
						+ 'IsAutoInc,'
						+ 'AttributeType,'
						+ 'ConservationPosition,'
						+ 'KeyFormat,'
						+ 'Validation,'
						+ 'Format'
						+ ') '
					+ 'SELECT NEWID() AS IdAttribute, attOgg.Nome as Name, ' + @IdArchive + ', '
						+ 'CASE ISNULL(attOgg.Obbligatorio, 0) '
						+ '	WHEN 0 THEN 0 '
						+ '	ELSE 1 '
						+ '	END AS IsRequired, '
						+ 'attOgg.PosizioneInChiaveUnivoca AS KeyOrder, '
						+ '0 AS IdMode, '
						+ 'CASE ISNULL(attOgg.DataPrincipale, 0) '
								+ 'WHEN 0 THEN 0 '
								+ 'ELSE 1 '
								+ 'END AS IsMainDate, '
						+ 'CASE attOgg.ProgressivoIntero '
							+ 'WHEN 0 THEN 0 '
							+ 'ELSE 1 '
							+ 'END AS IsAutoInc, '
						+ 'CASE LTRIM(RTRIM(UPPER(attOgg.TipoCampo))) '
							+ 'WHEN ''DATETIME'' THEN ''System.DateTime'' '
							+ 'WHEN ''INT'' THEN ''System.Int64'' '
							+ 'WHEN ''BIGINT'' THEN ''System.Int64'' '
							+ 'WHEN ''SMALLINT'' THEN ''System.Int64'' '
							+ 'WHEN ''TINYINT'' THEN ''System.Int64'' '
							+ 'ELSE ''System.String'' '
							+ 'END AS AttributeType, '
						+ 'attOgg.PosizioneInFileChiusura AS ConservationPosition, '
						+ 'ISNULL(attOgg.PorzioneInChiaveUnivoca, attOgg.FormatoInChiaveUnivoca) AS KeyFormat, '
						+ 'attOgg.Validazione AS Validation, '
						+ 'attOgg.Formato AS Format '
					+ 'FROM [' + @SourceDb + '].[dbo].[AttributoOggetto] attOgg '
					+ 'LEFT OUTER JOIN [' + @DestinationDb + '].[dbo].[Attributes] att ON RTRIM(LTRIM(att.Name)) COLLATE ' + @Collation + ' = RTRIM(LTRIM(attOgg.Nome)) '
					+ 'WHERE att.Name IS NULL'
			
			EXEC (@Sql)
			
			PRINT 'Importazione PARAMETRI'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationParameters]('
						+ 'IdArchive,'
						+ 'Label,'
						+ 'Value'
						+ ') '
						+ 'SELECT ' + @IdArchive + ', Nome, Valore '
						+ 'FROM [' + @SourceDb +'].[dbo].[Parametro] '
						+ 'WHERE Nome NOT IN (SELECT Label FROM [' + @DestinationDb + '].[dbo].[PreservationParameters])'
			EXEC (@Sql)
			
			PRINT 'Importazione RUOLI'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationRole]('
						+ 'IdPreservationRole,'
						+ 'Name,'
						+ 'Enable,'
						+ 'AlertEnable'
						+ ') '
						+ 'SELECT NEWID(), Nome, Attivo, RiceveChiusureTask '
						+ 'FROM [' + @SourceDb + '].[dbo].[Ruolo]'
						+ 'WHERE Nome NOT IN (SELECT Name FROM [' + @DestinationDb + '].[dbo].[PreservationRole])'
			
			EXEC (@Sql)
			
			PRINT 'Importazione SOGGETTI'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationUser]('
						+ 'IdPreservationUser,'
						+ 'Name,'
						+ 'Surname,'
						+ 'FiscalId,'
						+ 'Address,'
						+ 'Email,'
						+ 'Enable,'
						+ 'DomainUser'
						+ ') '
						+ 'SELECT NEWID(), Nome, Cognome, CodiceFiscale, Residenza, eMail, Attivo, DomainUser '
						+ 'FROM [' + @SourceDb + '].[dbo].[Soggetto] '
						+ 'WHERE RTRIM(LTRIM(ISNULL(Nome, ''X''))) + RTRIM(LTRIM(ISNULL(Cognome,''X''))) + RTRIM(LTRIM(ISNULL(CodiceFiscale, ''X''))) NOT IN ('
							+ 'SELECT RTRIM(LTRIM(ISNULL(Name, ''X''))) + RTRIM(LTRIM(ISNULL(Surname, ''X''))) + RTRIM(LTRIM(ISNULL(FiscalId, ''X''))) FROM [' + @DestinationDb + '].[dbo].[PreservationUser]'
						+ ')'
			EXEC (@Sql)
			
			PRINT 'Importazione RUOLI SOGGETTI'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationUserRole]('
						+ 'IdPreservationUserRole,'
						+ 'IdPreservationRole,'
						+ 'IdPreservationUser,'
						+ 'IdArchive'
						+ ') '
						+ 'SELECT NEWID() AS IdPreservationUserRole, rol.IdPreservationRole, usr.IdPreservationUser, ' + @IdArchive + ' '
						+ 'FROM [' + @SourceDb + '].[dbo].[Ruolo_Soggetto] rs '
						+ '	INNER JOIN [' + @SourceDb + '].[dbo].[Soggetto] sogg ON rs.IdSoggetto = sogg.IdSoggetto '
						+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationUser] usr ON RTRIM(LTRIM(ISNULL(sogg.Nome, ''X''))) + RTRIM(LTRIM(ISNULL(sogg.Cognome, ''X''))) + RTRIM(LTRIM(ISNULL(sogg.CodiceFiscale, ''X''))) = RTRIM(LTRIM(ISNULL(usr.Name, ''X''))) + RTRIM(LTRIM(ISNULL(usr.Surname, ''X''))) + RTRIM(LTRIM(ISNULL(usr.FiscalId, ''X''))) '
						+ '	INNER JOIN [' + @SourceDb + '].[dbo].[Ruolo] ruo ON rs.IdRuolo = ruo.IdRuolo '
						+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationRole] rol ON RTRIM(LTRIM(ruo.Nome)) = RTRIM(LTRIM(rol.Name))'
						+ ' WHERE CAST(rol.IdPreservationRole AS VARCHAR(40)) + CAST(usr.IdPreservationUser AS VARCHAR(40)) NOT IN (SELECT CAST(IdPreservationRole AS VARCHAR(40)) + CAST(IdPreservationUser AS VARCHAR(40)) FROM [' + @DestinationDb + '].[dbo].[PreservationUserRole])'
			
			EXEC (@Sql)
			
			PRINT 'Importazione TIPO TASK'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskType]('
						+ 'IdPreservationTaskType,'
						+ 'Description,'
						+ 'Period'
						+ ')'
						+ ' SELECT NEWID(), Descrizione, 0'
						+ ' FROM [' + @SourceDb + '].[dbo].[TipoTask]'
						+ ' WHERE RTRIM(LTRIM(Descrizione)) NOT IN (SELECT RTRIM(LTRIM([Description])) FROM [' + @DestinationDb + '].[dbo].[PreservationTaskType])'
			EXEC (@Sql)
			
			PRINT 'Importazione NOMI TASK (TaskStatus)'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskStatus]('
						+ 'IdPreservationTaskStatus,'
						+ 'Status'
						+ ') '
						+ 'SELECT NEWID() AS IdTaskStatus, tsk.Nome AS Status '
						+ 'FROM [' + @SourceDb + '].[dbo].[Task] tsk '
						+ 'WHERE tsk.Nome NOT IN (SELECT Status FROM [' + @DestinationDb + '].[dbo].[PreservationTaskStatus])'
			
			EXEC (@Sql)
			
			PRINT 'Importazione RUOLI TASK'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskRole] ('
						+ 'IdPreservationTaskType,'
						+ 'IdPreservationRole,'
						+ 'CreationDate'
						+ ') '
						+ 'SELECT IdPreservationTaskType, IdPreservationRole, GETDATE() '
						+ 'FROM [' + @SourceDb + '].[dbo].[TipoTask] tipo '
							+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Ruolo] ruolo ON  ruolo.IdRuolo = tipo.IdRuoloEsecutore '
							+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskType] pTT ON RTRIM(LTRIM(pTT.Description)) = RTRIM(LTRIM(tipo.Descrizione)) '
							+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationRole] rol ON RTRIM(LTRIM(rol.Name)) = RTRIM(LTRIM(ruolo.Nome))'
			
			EXEC(@Sql)
			
			PRINT 'Importazione SCADENZIARIO'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationSchedule]('
						+ 'IdPreservationSchedule,'
						+ 'Name,'
						+ 'Period,'
						+ 'ValidWeekDays,'
						+ 'FrequencyType,'
						+ 'Active'
						+ ') '
						+ 'SELECT NEWID(), Nome, Periodicita, REPLACE(REPLACE(GiorniSettimanaValidi, Char(13), ''''), Char(10), ''''), TipologiaFrequenza, Attivo '
						+ 'FROM [' + @SourceDb + '].[dbo].[Scadenziario] '
						+ 'WHERE RTRIM(LTRIM(ISNULL(Nome, ''X''))) + RTRIM(LTRIM(ISNULL(REPLACE(REPLACE(GiorniSettimanaValidi, Char(13), ''''), Char(10), ''''), ''X''))) + CAST(ISNULL(TipologiaFrequenza, -1) AS VARCHAR(10)) NOT IN ('
							+ 'SELECT RTRIM(LTRIM(ISNULL(Name, ''X''))) + RTRIM(LTRIM(ISNULL(REPLACE(REPLACE(ValidWeekDays, Char(13), ''''), Char(10), ''''), ''X''))) + CAST(ISNULL(FrequencyType, -1) AS VARCHAR(10)) '
							+ 'FROM [' + @DestinationDb + '].[dbo].[PreservationSchedule]'
						+ ')'
			EXEC (@Sql)
			
			PRINT 'Importazione SCADENZIARIO - TIPO TASK'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationSchedule_TaskType]('
						+ 'IdPreservationSchedule,'
						+ 'IdPreservationTaskType'
						+ ') '
						+ 'SELECT IdPreservationSchedule, IdPreservationTaskType '
						+ 'FROM [' + @SourceDb + '].[dbo].[Scadenziario_TipoTask] scadTT '
							+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Scadenziario] scad ON scadTT.IdScadenziario = scad.IdScadenziario '
								+ ' INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationSchedule] sched ON RTRIM(LTRIM(ISNULL(scad.Nome, ''X''))) + RTRIM(LTRIM(ISNULL(scad.Periodicita, ''X''))) = RTRIM(LTRIM(ISNULL(sched.Name, ''X''))) + RTRIM(LTRIM(ISNULL(sched.Period, ''X''))) '
							+ 'INNER JOIN [' + @SourceDb + '].[dbo].[TipoTask] tt ON scadTT.IdTipoTask = tt.IdTipoTask '
								+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskType] ptt ON tt.Descrizione = ptt.Description '
						+ 'WHERE CAST(IdPreservationSchedule AS VARCHAR(40)) + CAST(IdPreservationTaskType AS VARCHAR(40)) NOT IN (SELECT CAST(IdPreservationSchedule AS VARCHAR(40)) + CAST(IdPreservationTaskType AS VARCHAR(40)) FROM [' + @DestinationDb + '].[dbo].[PreservationSchedule_TaskType])'
			
			EXEC (@Sql)
			
			PRINT 'Importazione FESTIVITA'''
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationHolidays]('
						+ 'IdPreservationHolidays,'
						+ 'HolidayDate,'
						+ 'Description'
						+ ') '
						+ 'SELECT NEWID() AS IdPreservationHolidays, DataFestivita, Descrizione '
						+ 'FROM [' + @SourceDb + '].[dbo].[Festivita] '
						+ 'WHERE Descrizione NOT IN (SELECT Description FROM [' + @DestinationDb + '].[dbo].[PreservationHolidays])'
			
			EXEC (@Sql)
			
			PRINT 'Importazione TIPO GRUPPO TASK'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskGroupType]('
						+ 'IdPreservationTaskGroupType,'
						+ 'Description'
						+ ') '
						+ 'SELECT NEWID(), Descrizione '
						+ 'FROM [' + @SourceDb + '].[dbo].[TipoGruppoTask] '
						+ 'WHERE Descrizione NOT IN (SELECT Description FROM [' + @DestinationDb + '].[dbo].[PreservationTaskGroupType])'
			EXEC (@Sql)
			
			PRINT 'Importazione GRUPPO TASK'
			/*
				*************** N.B. ***************
				Possono esistere più gruppi task avente il medesimo nome, il medesimo soggetto incaricato e lo stesso identico scadenziario.
				Attenzione ai doppi!
			*/
			SET @Sql = 'IF EXISTS(SELECT Nome, IdSoggettoResponsabile, IdScadenziario FROM [' + @SourceDb + '].[dbo].[GruppoTask] GROUP BY Nome, IdSoggettoResponsabile, IdScadenziario HAVING COUNT(*) > 1) '
						+ 'BEGIN '
							+ 'RAISERROR (''Esistono dei GRUPPI TASK aventi lo stesso nome, lo stesso soggetto incaricato e lo stesso scadenziario.'', 18, 1) '
							+ 'RETURN '
						+ 'END'
			EXEC (@Sql)
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskGroup]('
						+ 'IdPreservationTaskGroup,'
						+ 'IdPreservationTaskGroupType,'
						+ 'Name,'
						+ 'IdPreservationUser,'
						+ 'IdPreservationSchedule,'
						+ 'Expiry,'
						+ 'EstimatedExpiry,'
						+ 'Closed'
						+ ') '
						+ 'SELECT NEWID(), ptgt.IdPreservationTaskGroupType, gt.Nome AS Name, usr.IdPreservationUser, '
							+ 'sched.IdPreservationSchedule, gt.Scadenza AS Expiry, gt.ScadenzaTeorica AS EstimatedExpiry, gt.Chiuso AS Closed '
						+ 'FROM [' + @SourceDb + '].[dbo].[GruppoTask] gt '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[TipoGruppoTask] tgt ON gt.IdTipoGruppoTask = tgt.IdTipoGruppoTask '
						+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskGroupType] ptgt ON ptgt.Description = tgt.Descrizione '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Soggetto] sogg ON gt.IdSoggettoResponsabile =  sogg.IdSoggetto '
						+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationUser] usr ON RTRIM(LTRIM(ISNULL(sogg.Nome, ''X''))) + RTRIM(LTRIM(ISNULL(sogg.Cognome,''X''))) + RTRIM(LTRIM(ISNULL(sogg.CodiceFiscale, ''X''))) = RTRIM(LTRIM(ISNULL(usr.Name, ''X''))) + RTRIM(LTRIM(ISNULL(usr.Surname,''X''))) + RTRIM(LTRIM(ISNULL(usr.FiscalId, ''X''))) '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Scadenziario] scad ON gt.IdScadenziario = scad.IdScadenziario '
						+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationSchedule] sched ON sched.Name = scad.Nome '
						+ 'WHERE NOT EXISTS(SELECT IdPreservationTaskGroupType, Name, IdPreservationUser, IdPreservationSchedule '
											+ 'FROM [' + @DestinationDb + '].[dbo].[PreservationTaskGroup] '
											+ 'WHERE IdPreservationTaskGroupType = ptgt.IdPreservationTaskGroupType '
												+ 'AND Name = gt.Nome '
												+ 'AND IdPreservationUser = usr.IdPreservationUser '
												+ 'AND IdPreservationSchedule = sched.IdPreservationSchedule'
												+ ')'
						
			EXEC (@Sql)
			
			PRINT 'Importazione TASK'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTask]('
						+ 'IdPreservationTask,'
						+ 'IdPreservationConsole,'
						+ 'EstimatedDate,'
						+ 'ExecutedDate,'
						+ 'IdPreservationTaskType,'
						+ 'IdArchive,'
						+ 'IdPreservationTaskStatus,'
						+ 'IdPreservationUser,'
						+ 'IdPreservationTaskGroup'
						+ ')'
						+ 'SELECT NEWID() AS IdPreservationTask, tsk.IdTask AS IdPreservationConsole, tsk.DataScadenza AS EstimatedDate, tsk.DataCompletamento AS ExecutedDate, pTT.IdPreservationTaskType, ' + @IdArchive + ', tskStatus.IdPreservationTaskStatus, usr.IdPreservationUser, pGrp.IdPreservationTaskGroup '
						+ 'FROM [' + @SourceDb + '].[dbo].[Task] tsk '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[GruppoTask] grp ON tsk.IdGruppoTask = grp.IdGruppoTask '
						+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskGroup] pGrp ON RTRIM(LTRIM(UPPER(pGrp.Name))) = RTRIM(LTRIM(UPPER(grp.Nome))) '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Soggetto] sogg ON tsk.IdSoggettoEsecutore = sogg.IdSoggetto '
						+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationUser] usr ON UPPER(RTRIM(LTRIM(sogg.Nome)) + RTRIM(LTRIM(sogg.Cognome)) + RTRIM(LTRIM(sogg.CodiceFiscale))) = UPPER(RTRIM(LTRIM(usr.Name)) + RTRIM(LTRIM(usr.Surname)) + RTRIM(LTRIM(usr.FiscalId))) '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[TipoTask] tt ON tsk.IdTipoTask = tt.IdTipoTask '
						+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskType] pTT ON UPPER(RTRIM(LTRIM(tt.Descrizione))) = UPPER(RTRIM(LTRIM(pTT.Description))) '
						+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskStatus] tskStatus ON UPPER(RTRIM(LTRIM(tsk.Nome))) = UPPER(RTRIM(LTRIM(tskStatus.Status))) '
						+ 'WHERE NOT EXISTS(SELECT IdTask, EstimatedDate, IdPreservationTaskType, IdArchive, IdPreservationTaskStatus, IdPreservationTaskGroup FROM [' + @DestinationDb + '].[dbo].[PreservationTask] '
											+ 'WHERE IdTask = tsk.IdTask '
												+ 'AND EstimatedDate = tsk.DataScadenza '
												+ 'AND IdPreservationTaskType = pTT.IdPreservationTaskType '
												+ 'AND IdArchive = ' + @IdArchive + ' '
												+ 'AND IdPreservationTaskStatus = tskStatus.IdPreservationTaskStatus '
												+ 'AND IdPreservationTaskGroup = pGrp.IdPreservationTaskGroup)'
			
			EXEC (@Sql)
			
			PRINT 'Importazione CONSERVAZIONE SOSTITUTIVA'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[Preservation]('
						+ 'IdPreservation,'
						+ 'IdArchive,'
						+ 'IdPreservationTaskGroup,'
						+ 'IdPreservationTask,'
						+ 'Path,'
						+ 'Label,'
						+ 'PreservationDate,'
						+ 'StartDate,'
						+ 'EndDate,'
						+ 'CloseDate,'
						+ 'IndexHash,'
						+ 'CloseContent,'
						+ 'LastVerifiedDate,'
						+ 'IdPreservationUser'
						+ ') '
						+ 'SELECT NEWID() AS IdPreservation, ' + @IdArchive + ', pTskGrp.IdPreservationTaskGroup, pTsk.IdPreservationTask,'
							+ 'cons.Collocazione AS Path, Cons.NomeSupporto AS Label,cons.DataConservazione AS PreservationDate,'
							+ 'cons.DataInizio AS StartDate, cons.DataFine AS EndDate, cons.DataChiusura AS CloseDate,'
							+ 'cons.FileIndiceHashHSA1 AS IndexHash, cons.FileChiusura AS CloseContent, cons.DataFirma AS LastVerifiedDate,'
							+ '(SELECT TOP 1 IdPreservationUser FROM [' + @DestinationDb + '].[dbo].[PreservationUser]) '
						+ 'FROM [' + @SourceDb + '].[dbo].[ConservazioneSostitutiva] cons '
							+ 'INNER JOIN [' + @SourceDb + '].[dbo].[GruppoTask] tskGrp ON tskGrp.IdGruppoTask = cons.IdGruppoTask '
								+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskGroup] pTskGrp ON RTRIM(LTRIM(pTskGrp.Name)) = RTRIM(LTRIM(tskGrp.Nome)) '
							+ 'LEFT OUTER JOIN [' + @SourceDb + '].[dbo].[Task] tsk ON tsk.IdTask = cons.IdTaskConservazione '
								+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTask] pTsk ON pTsk.IdPreservationConsole = tsk.IdTask '
						+ 'WHERE NOT EXISTS (SELECT IdArchive, IdPreservationTaskGroup, IdPreservationTask, IdPreservationUser FROM [' + @DestinationDb + '].[dbo].[Preservation] '
											+ 'WHERE IdArchive = ' + @IdArchive + ' '
												+ 'AND IdPreservationTaskGroup = pTskGrp.IdPreservationTaskGroup '
												+ 'AND IdPreservationTask = pTsk.IdPreservationTask '
												+ 'AND IdPreservationUser = (SELECT TOP 1 IdPreservationUser FROM [' + @DestinationDb + '].[dbo].[PreservationUser]))'
			
			EXEC (@Sql)
			
			PRINT 'Importazione TIPOLOGIA AVVISI'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationAlertType]('
						+ 'IdPreservationAlertType,'
						+ 'IdPreservationRole,'
						+ 'IdPreservationConsole,'
						+ 'AlertText,'
						+ 'Offset'
						+ ') '
						+ 'SELECT NEWID(), rol.IdPreservationRole, tipo.IdTipoAvviso, tipo.TestoAvviso, tipo.Offset '
						+ 'FROM [' + @SourceDb + '].[dbo].[TipoAvviso] tipo '
						+	'INNER JOIN [' + @SourceDb + '].[dbo].[Ruolo] ruo ON tipo.IdRuoloCC = ruo.IdRuolo '
						+		'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationRole] rol ON UPPER(RTRIM(LTRIM(rol.Name))) = UPPER(RTRIM(LTRIM(ruo.Nome))) '
						+ 'WHERE NOT EXISTS (SELECT * FROM [' + @DestinationDb + '].[dbo].[PreservationAlertType] '
											+ 'WHERE IdPreservationRole = rol.IdPreservationRole '
												+ 'AND AlertText = tipo.TestoAvviso)'
			
			EXEC (@Sql)
			
			PRINT 'Importazione TIPOLOGIA AVVISI - TASK'
			
			SET @Sql =	'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationAlertTask]('
						+ 'IdPreservationAlertType,'
						+ 'IdPreservationTaskType'
						+ ') '
						+ 'SELECT alr.IdPreservationAlertType, pTT.IdPreservationTaskType '
						+ 'FROM [' + @SourceDb + '].[dbo].[TipoAvviso] tipo '
						+ '	INNER JOIN [' + @SourceDb + '].[dbo].[Ruolo] ruo ON tipo.IdRuoloCC = ruo.IdRuolo '
						+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationRole] rol ON UPPER(RTRIM(LTRIM(rol.Name))) = UPPER(RTRIM(LTRIM(ruo.Nome))) '
						+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationAlertType] alr ON alr.IdPreservationRole = rol.IdPreservationRole '
						+ '			AND alr.AlertText = tipo.TestoAvviso AND alr.Offset = tipo.Offset '
						+ '	INNER JOIN [' + @SourceDb + '].[dbo].[TipoTask] tt ON tipo.IdTipoTask = tt.IdTipoTask '
						+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskType] pTT ON pTT.Description = tt.Descrizione '
						+ 'WHERE pTT.IdPreservationTaskType NOT IN(SELECT IdPreservationTaskType FROM [' + @DestinationDb + '].[dbo].[PreservationAlertTask])'
			
			EXEC (@Sql)
			
			PRINT 'Importazione AVVISI'
			
			SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationAlert]('
						+ 'IdPreservationAlert,'
						+ 'IdPreservationAlertType,'
						+ 'IdPreservationTask,'
						+ 'MadeDate,'
						+ 'AlertDate,'
						+ 'ForwardFrequency'
						+ ')'
			+ 'SELECT NEWID() AS IdPreservationAlert, alr.IdPreservationAlertType, pTsk.IdPreservationTask, avv.Effettuato AS MadeDate,'
				+ 'avv.DataAvviso AS AlertDate, avv.FrequenzaDiReinvio AS ForwardFrequency '
			+ 'FROM [' + @SourceDb + '].[dbo].[Avviso] avv '
			+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationAlertType] alr ON alr.IdPreservationConsole = avv.IdTipoAvviso '
			+ '	INNER JOIN [' + @SourceDb + '].[dbo].[Task] tsk ON tsk.IdTask = avv.IdTask '
			+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTask] pTsk ON pTsk.IdPreservationConsole = tsk.IdTask '
			+ 'WHERE NOT EXISTS(SELECT IdPreservationAlert, IdPreservationAlertType, IdPreservationTask FROM [' + @DestinationDb + '].[dbo].[PreservationAlert] '
								+ 'WHERE IdPreservationAlertType = alr.IdPreservationAlertType '
									+ 'AND IdPreservationTask = pTsk.IdPreservationTask)'
			
			EXEC (@Sql)
			
		END TRY
		BEGIN CATCH
			--Un po' di messaggi.
			PRINT 'ERRORE: ' + ERROR_MESSAGE()
			PRINT 'Procedura interrotta prematuramente.'
			--Rollback della transazione (è inutile importare i dati per metà!).
			ROLLBACK TRANSACTION TR_DataTransfer
			--Interrompe l'esecuzione della procedura e torna al chiamante.
			RETURN;
		
		END CATCH
	
	PRINT 'La procedura e'' stata completata con successo.'
	
	IF @Simulation = 1
		ROLLBACK TRANSACTION TR_DataTransfer
	ELSE
		COMMIT TRANSACTION TR_DataTransfer 

END
GO
/****** Object:  StoredProcedure [dbo].[Compatibility_MergeAttributoOggetto]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[Compatibility_MergeAttributoOggetto]
	@nomeArchivio varchar(255)
as 
/****** Script for SelectTopNRows command from SSMS  ******/

declare @idArchive as uniqueidentifier 
select @idArchive = IdArchive from BiblosDS2010.dbo.Archive where Name = @nomeArchivio

DECLARE @cmd AS NVARCHAR(4000)

SET @cmd = '
INSERT INTO [BiblosDS2010].[dbo].[AttributesValue]
           ([IdDocument]
           ,[IdAttribute]          
           ,[ValueString])
SELECT 
(select IdDocument from BiblosDS2010.dbo.Document where IdBiblos = IdOggetto and IdArchive='''+cast(@idArchive as varchar(255))+'''),
(select top 1 IdAttribute from BiblosDS2010.dbo.Attributes where IdArchive='''+cast(@idArchive as varchar(255))+''' and Name=cast(NomeAttributoOggetto as varchar(255)) collate Latin1_General_CI_AS  )				
      ,[Valore]
FROM  '+@nomeArchivio+'.[dbo].[ValoreAttributoOggetto]'

  print @cmd
		EXEC sp_executesql @cmd
GO
/****** Object:  Table [dbo].[_PreservationObjectType]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_PreservationObjectType](
	[IdPreservationObjectType] [dbo].[_Id_] NOT NULL,
	[Description] [dbo].[_Stringa_] NULL,
 CONSTRAINT [PK_PreservationObjectType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationObjectType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Attributes]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Attributes](
	[IdAttribute] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IsRequired] [smallint] NOT NULL,
	[KeyOrder] [smallint] NULL,
	[IdMode] [int] NOT NULL,
	[IsMainDate] [smallint] NULL,
	[IsEnumerator] [smallint] NULL,
	[IsAutoInc] [smallint] NULL,
	[IsUnique] [smallint] NULL,
	[AttributeType] [varchar](255) NOT NULL,
	[ConservationPosition] [smallint] NULL,
	[DefaultValue] [varchar](255) NULL,
	[MaxLenght] [int] NULL,
	[KeyFilter] [varchar](255) NULL,
	[KeyFormat] [varchar](255) NULL,
	[Validation] [varchar](255) NULL,
	[Format] [varchar](255) NULL,
	[IsChainAttribute] [smallint] NULL,
	[IdAttributeGroup] [uniqueidentifier] NULL,
	[IsVisible] [smallint] NULL,
 CONSTRAINT [PK_Attributes_1] PRIMARY KEY CLUSTERED 
(
	[IdAttribute] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ArchiveStorage]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArchiveStorage](
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IdStorage] [uniqueidentifier] NOT NULL,
	[Active] [smallint] NOT NULL,
 CONSTRAINT [PK_ArchiveStorage] PRIMARY KEY CLUSTERED 
(
	[IdArchive] ASC,
	[IdStorage] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[Compatibility_CreateStorage]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[Compatibility_CreateStorage]
	@nomeArchivio varchar(255)
as                                     
declare @idStorage as uniqueidentifier  
declare @idStorageType as uniqueidentifier
declare @idSupporto as int                   
select @idStorageType = IdStorageType from BiblosDS2010.dbo.StorageType where StorageClassName = 'BDSComStorage'
set @idStorage = NEWID()
DECLARE @cmd AS NVARCHAR(1000)
--verifica dell'esistenza di uno storage configurato, se non esiste lo crea
if exists(select * from BiblosDS2010.dbo.Storage where Name = @nomeArchivio)
begin
	select @idStorage = IdStorage from BiblosDS2010.dbo.Storage where Name = @nomeArchivio
end
else
begin
if not exists ( SELECT   *
     FROM         BiblosDs.dbo.Drive Drive INNER JOIN
             BiblosDs.dbo.Archivio Archivio ON Drive.IdSupporto = Archivio.IdSupportoInScrittura
             where NomeArchivio = @nomeArchivio)
 begin
	print 'Impossibile configurare lo storagee. Nessun DRIVE definito in biblos'
	return 0
 end
 else
 begin
	insert into Storage
			([IdStorage]
           ,[IdStorageType]
           ,[MainPath]
           ,[Name]  
           ,[EnableFulText]        
           ,[IsVisible])                
     SELECT     @idStorage, @idStorageType,Drive.IdentificativoHW+'\'+Archivio.NomeArchivio, Archivio.NomeArchivio, 0, 1
     FROM         BiblosDs.dbo.Drive Drive INNER JOIN
             BiblosDs.dbo.Archivio Archivio ON Drive.IdSupporto = Archivio.IdSupportoInScrittura
             where NomeArchivio = @nomeArchivio
 end
             
end

SELECT @idSupporto = Drive.IdSupporto
FROM BiblosDs.dbo.Drive Drive INNER JOIN
BiblosDs.dbo.Archivio Archivio ON Drive.IdSupporto = Archivio.IdSupportoInScrittura
where NomeArchivio = @nomeArchivio
print @idSupporto

SET @cmd ='INSERT INTO [BiblosDS2010].[dbo].[StorageArea]
           ([IdStorageArea]
           ,[IdStorage]
           ,[Path]
           ,[Name]
           ,[Priority] 
           ,[MaxSize] 
           ,[MaxFileNumber]                              
           ,[Enable])
select NEWID(), '''+cast(@idStorage as varchar(255))+''', Right(''000000000000''+cast(IdFile as varchar(255)), 8), cast(IdFile as varchar(255)), IdFile, 10000, 10000, 0 
FROM   '+@nomeArchivio+'.dbo.FileSup FileSup
where IdSupporto = '+cast(@idSupporto as varchar(255))+'
and IdFile not in (select CAST(name as int) from [BiblosDS2010].[dbo].[StorageArea] where IdStorage = '''+cast(@idStorage as varchar(255))+''')'

print @cmd
EXEC sp_executesql @cmd
GO
/****** Object:  Table [dbo].[StorageArea]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageArea](
	[IdStorageArea] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdStorage] [uniqueidentifier] NOT NULL,
	[Path] [varchar](255) NULL,
	[Name] [varchar](255) NOT NULL,
	[Priority] [int] NULL,
	[IdStorageStatus] [smallint] NULL,
	[MaxSize] [bigint] NOT NULL,
	[CurrentSize] [bigint] NULL,
	[MaxFileNumber] [bigint] NOT NULL,
	[CurrentFileNumber] [bigint] NULL,
	[Enable] [smallint] NOT NULL,
 CONSTRAINT [PK_StorageArea] PRIMARY KEY CLUSTERED 
(
	[IdStorageArea] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationTaskRole]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationTaskRole](
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NULL,
 CONSTRAINT [PK_TaskRole] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskType] ASC,
	[IdPreservationRole] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreservationSchedule_TaskType]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationSchedule_TaskType](
	[IdPreservationSchedule] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[Offset] [smallint] NULL,
 CONSTRAINT [PK_PreservationSchedule_TaskType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationSchedule] ASC,
	[IdPreservationTaskType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreservationTask]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTask](
	[IdPreservationTask] [uniqueidentifier] NOT NULL,
	[IdPreservationConsole] [int] NULL,
	[EstimatedDate] [datetime] NOT NULL,
	[ExecutedDate] [datetime] NULL,
	[StartDocumentDate] [datetime] NULL,
	[EndDocumentDate] [datetime] NULL,
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskStatus] [uniqueidentifier] NULL,
	[IdPreservationUser] [uniqueidentifier] NULL,
	[IdPreservationTaskGroup] [uniqueidentifier] NOT NULL,
	[Name] [varchar](250) NULL,
 CONSTRAINT [PK_TaskConservazione] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTask] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationAlertTask]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationAlertTask](
	[IdPreservationAlertType] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_AlertTask] PRIMARY KEY CLUSTERED 
(
	[IdPreservationAlertType] ASC,
	[IdPreservationTaskType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[Compatibility_MergeOggetto]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[Compatibility_MergeOggetto]
	@nomeArchivio varchar(255)
as 
declare @idStorage as uniqueidentifier 
declare @idArchive as uniqueidentifier 
--verifica dell'esistenza di uno storage configurato, se non esiste lo crea
if exists(select * from BiblosDS2010.dbo.Storage where Name = @nomeArchivio)
begin
	select @idStorage = IdStorage from BiblosDS2010.dbo.Storage where Name = @nomeArchivio
	select @idArchive = IdArchive from BiblosDS2010.dbo.Archive where Name = @nomeArchivio
	DECLARE @cmd AS NVARCHAR(4000)
	print 'Init'
	if @idArchive is null
	begin
		print 'Configurare un archivio per continuare'
	end
	  
                      
     
	
	SET @cmd = 'INSERT INTO [BiblosDS2010].[dbo].[Document]
           ([IdBiblos]
           ,[IdDocument]
           ,[IdParentBiblos]
           ,[IdStorageArea]
           ,[IdStorage]
           ,[IdArchive]
           ,[IsVisible]           
           ,[DateExpire]          
           ,[Name]
           ,[Size]          
           ,[DateMain]
           ,ChainOrder
           ,[Version]
           ,IdDocumentStatus
           ,IsCheckOut
           )  
           SELECT  Oggetto.IdOggetto,
				newid(),
			  (select IdParentBiblos from [BiblosDS2010].[dbo].Document where IdBiblos = Oggetto.IdOggettoPadre and IdArchive='''+cast(@idArchive as varchar(255))+'''),
             (select IdStorageArea from [BiblosDS2010].[dbo].[StorageArea] where Name = cast(FileSup.IdFile as varchar(255)) and IdStorage='''+cast(@idStorage as varchar(255))+'''),  
             '''+cast(@idStorage as varchar(255))+''', 
             '''+cast(@idArchive as varchar(255))+''',
             1,
             null,--Oggetto.DataScadenza
             ''AUTOLOAD'',
             Ubicazione.Lunghezza,
             GETDATE(),
             1,
             1,
             3,
             0              
		   FROM         '+@nomeArchivio+'.dbo.FileSup INNER JOIN
                      '+@nomeArchivio+'.dbo.Ubicazione ON FileSup.IdFile = Ubicazione.IdFile right JOIN
                      '+@nomeArchivio+'.dbo.Oggetto ON Ubicazione.IdOggetto = Oggetto.IdOggetto  
                      where IdOggettoPadre is null or IdOggettoPadre = 0 '      
                      
                      
        print @cmd
		EXEC sp_executesql @cmd

        SET @cmd = ' INSERT INTO [BiblosDS2010].[dbo].[Document]
           ([IdBiblos]
           ,[IdDocument]
           ,[IdParentBiblos]
           ,[IdStorageArea]
           ,[IdStorage]
           ,[IdArchive]
           ,[IsVisible]           
           ,[DateExpire]          
           ,[Name]
           ,[Size]          
           ,[DateMain]
            ,ChainOrder
           ,[Version]
           ,IdDocumentStatus
           ,IsCheckOut
           )  
            SELECT  Oggetto.IdOggetto,
				newid(),
			  (select top 1 IdDocument from [BiblosDS2010].[dbo].Document where IdBiblos = Oggetto.IdOggettoPadre and IdArchive='''+cast(@idArchive as varchar(255))+'''), 
             (select top 1 IdStorageArea from [BiblosDS2010].[dbo].[StorageArea] where Name = cast(FileSup.IdFile as varchar(255)) and IdStorage='''+cast(@idStorage as varchar(255))+'''), 
             '''+cast(@idStorage as varchar(255))+''', 
             '''+cast(@idArchive as varchar(255))+''',
             1,
             null,--Oggetto.DataScadenza
             ''AUTOLOAD_CHILD'',
             Ubicazione.Lunghezza,
             GETDATE(),
              1,
             1,
            3,
             0              
		   FROM         '+@nomeArchivio+'.dbo.FileSup INNER JOIN
                      '+@nomeArchivio+'.dbo.Ubicazione ON FileSup.IdFile = Ubicazione.IdFile right JOIN
                      '+@nomeArchivio+'.dbo.Oggetto ON Ubicazione.IdOggetto = Oggetto.IdOggetto  
                      where not IdOggettoPadre is null and IdOggettoPadre <> 0  '  
                      
                               
                      
           print @cmd
		EXEC sp_executesql @cmd              	
	
end
else
begin
print 'Storage non configurato...'
end
GO
/****** Object:  Table [dbo].[StorageRule]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageRule](
	[IdStorage] [uniqueidentifier] NOT NULL,
	[IdAttribute] [uniqueidentifier] NOT NULL,
	[RuleOrder] [smallint] NOT NULL,
	[RuleFormat] [varchar](255) NULL,
	[RuleFilter] [varchar](255) NOT NULL,
	[IdRuleOperator] [int] NULL,
 CONSTRAINT [PK_StorageRule] PRIMARY KEY CLUSTERED 
(
	[IdStorage] ASC,
	[IdAttribute] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StorageAreaRule]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageAreaRule](
	[IdAttribute] [uniqueidentifier] NOT NULL,
	[RuleOrder] [smallint] NOT NULL,
	[RuleFormat] [varchar](255) NULL,
	[IdRuleOperator] [int] NULL,
	[RuleFilter] [varchar](255) NULL,
	[IdStorageArea] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_StorageAreaRule_1] PRIMARY KEY CLUSTERED 
(
	[IdAttribute] ASC,
	[IdStorageArea] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationAlert]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationAlert](
	[IdPreservationAlert] [uniqueidentifier] NOT NULL,
	[IdPreservationAlertType] [uniqueidentifier] NOT NULL,
	[IdPreservationTask] [uniqueidentifier] NOT NULL,
	[MadeDate] [datetime] NULL,
	[AlertDate] [datetime] NULL,
	[ForwardFrequency] [tinyint] NULL,
 CONSTRAINT [PK_PreservationAlert] PRIMARY KEY CLUSTERED 
(
	[IdPreservationAlert] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Data in cui è stato effettuata l''operazione' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PreservationAlert', @level2type=N'COLUMN',@level2name=N'MadeDate'
GO
/****** Object:  Table [dbo].[Preservation]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Preservation](
	[IdPreservation] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskGroup] [uniqueidentifier] NOT NULL,
	[IdPreservationTask] [uniqueidentifier] NULL,
	[Path] [varchar](255) NULL,
	[Label] [varchar](255) NULL,
	[PreservationDate] [datetime] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CloseDate] [datetime] NULL,
	[IndexHash] [varchar](255) NULL,
	[CloseContent] [image] NULL,
	[LastVerifiedDate] [datetime] NULL,
	[IdPreservationUser] [uniqueidentifier] NULL,
	[IdCompatibility] [int] NULL,
 CONSTRAINT [PK_Conservazione_1] PRIMARY KEY CLUSTERED 
(
	[IdPreservation] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Document]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Document](
	[IdBiblos] [int] NOT NULL,
	[IdDocument] [uniqueidentifier] NOT NULL,
	[IdParentBiblos] [uniqueidentifier] NULL,
	[IdStorageArea] [uniqueidentifier] NULL,
	[IdStorage] [uniqueidentifier] NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[ChainOrder] [int] NOT NULL,
	[StorageVersion] [decimal](18, 6) NULL,
	[Version] [decimal](18, 6) NOT NULL,
	[IdParentVersion] [uniqueidentifier] NULL,
	[IdDocumentLink] [uniqueidentifier] NULL,
	[IdCertificate] [uniqueidentifier] NULL,
	[SignHeader] [varchar](255) NULL,
	[FullSign] [varchar](255) NULL,
	[DocumentHash] [varchar](255) NULL,
	[IsLinked] [smallint] NULL,
	[IsVisible] [smallint] NOT NULL,
	[IsConservated] [smallint] NULL,
	[DateExpire] [datetime] NULL,
	[DateCreated] [datetime] NULL,
	[Name] [varchar](255) NULL,
	[Size] [bigint] NULL,
	[IdNodeType] [smallint] NULL,
	[IsConfirmed] [smallint] NULL,
	[IdDocumentStatus] [smallint] NOT NULL,
	[IsCheckOut] [smallint] NOT NULL,
	[DateMain] [datetime] NULL,
	[IdPreservation] [uniqueidentifier] NULL,
	[IsDetached] [bit] NULL,
	[IdUserCheckOut] [varchar](250) NULL,
	[PrimaryKeyValue] [varchar](250) NULL,
	[PrimaryKeyValueIndex]  AS (case when [PrimaryKeyValue] IS NULL OR [PrimaryKeyValue]='' then CONVERT([nvarchar](250),[IdDocument],(0)) else [PrimaryKeyValue] end),
	[IdPreservationException] [uniqueidentifier] NULL,
	[PreservationIndex] [bigint] NULL,
	[IdThumbnail] [varchar](250) NULL,
	[IdPdf] [varchar](250) NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationJournaling]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationJournaling](
	[IdPreservationJournaling] [uniqueidentifier] NOT NULL,
	[IdPreservation] [uniqueidentifier] NULL,
	[IdPreservationJournalingActivity] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NULL,
	[DateActivity] [datetime] NULL,
	[IdPreservationUser] [uniqueidentifier] NOT NULL,
	[Notes] [text] NULL,
	[DomainUser] [varchar](250) NOT NULL,
 CONSTRAINT [PK_PreservationJournaling] PRIMARY KEY CLUSTERED 
(
	[IdPreservationJournaling] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationInStorageDevice]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationInStorageDevice](
	[IdPreservation] [uniqueidentifier] NOT NULL,
	[IdPreservationStorageDevice] [uniqueidentifier] NOT NULL,
	[Path] [varchar](250) NULL,
 CONSTRAINT [PK_PreservationInStorageDevice] PRIMARY KEY CLUSTERED 
(
	[IdPreservation] ASC,
	[IdPreservationStorageDevice] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Transito]    Script Date: 10/17/2011 10:48:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transito](
	[LocalPath] [varchar](255) NULL,
	[IdDocument] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Status] [smallint] NULL,
	[Retry] [int] NOT NULL,
	[DateRetry] [datetime] NULL,
	[DateCreated] [datetime] NULL,
 CONSTRAINT [PK_Transito] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Permission](
	[PermissionName] [varchar](255) NOT NULL,
	[IdDocument] [uniqueidentifier] NOT NULL,
	[IdMode] [smallint] NOT NULL,
	[IsGroup] [smallint] NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[PermissionName] ASC,
	[IdDocument] ASC,
	[IdMode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExceptionConservazione]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExceptionConservazione](
	[IdExceptionType] [uniqueidentifier] NOT NULL,
	[IdConservazione] [uniqueidentifier] NOT NULL,
	[IdDocument] [uniqueidentifier] NOT NULL,
	[DateException] [datetime] NULL,
 CONSTRAINT [PK_ExceptionConservazione_1] PRIMARY KEY CLUSTERED 
(
	[IdExceptionType] ASC,
	[IdConservazione] ASC,
	[IdDocument] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[Compatibility_UpdateIdLastBiblos]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[Compatibility_UpdateIdLastBiblos]
	@nomeArchivio varchar(255)
as 
declare @idArchive as uniqueidentifier 
--verifica dell'esistenza di uno storage configurato, se non esiste lo crea
if exists(select * from BiblosDS2010.dbo.Archive where Name = @nomeArchivio)
begin	
	select @idArchive = IdArchive from BiblosDS2010.dbo.Archive where Name = @nomeArchivio
	
	update Archive
	set LastIdBiblos = (Select MAX(IdBiblos) +1 from Document where IdArchive = @idArchive)
	where IdArchive = @idArchive
end
else
begin
	print 'Nessun archive presente con il nome selezionato.';
end
GO
/****** Object:  Table [dbo].[AttributesValue]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttributesValue](
	[IdDocument] [uniqueidentifier] NOT NULL,
	[IdAttribute] [uniqueidentifier] NOT NULL,
	[ValueInt] [bigint] NULL,
	[ValueFloat] [float] NULL,
	[ValueDateTime] [datetime] NULL,
	[ValueString] [varchar](8000) NULL,
 CONSTRAINT [PK_AttributesValue_1] PRIMARY KEY NONCLUSTERED 
(
	[IdDocument] ASC,
	[IdAttribute] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Cache]    Script Date: 10/17/2011 10:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Cache](
	[LocalPath] [varchar](255) NULL,
	[Score] [int] NULL,
	[IdDocument] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
 CONSTRAINT [PK_Cache] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[BiblosDS_DeleteAllData]    Script Date: 10/17/2011 10:49:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[BiblosDS_DeleteAllData]
	@name as varchar(255)
as

declare @idArchive as uniqueidentifier

select @idArchive = IdArchive from Archive where Name = @name
print @idArchive
begin tran

delete from DocumentCache
where IdDocument in (
	Select IdDocument from Document
	where IdArchive = @IdArchive
)
IF @@ERROR != 0
BEGIN
    ROLLBACK TRANSACTION
	RETURN
END


delete from Transito
where IdDocument in (
	Select IdDocument from Document
	where IdArchive = @IdArchive
)
IF @@ERROR != 0
BEGIN
    ROLLBACK TRANSACTION
	RETURN
END

delete from AttributesValue where IdDocument in(
	Select IdDocument from Document
	where IdArchive = @IdArchive
)
IF @@ERROR != 0
BEGIN
    ROLLBACK TRANSACTION
	RETURN
END

delete from Document where IdArchive = @idArchive and not IdParentBiblos is null
IF @@ERROR != 0
BEGIN
    ROLLBACK TRANSACTION
	RETURN
END

delete from Document where IdArchive = @idArchive and IdParentBiblos is null
IF @@ERROR != 0
BEGIN
    ROLLBACK TRANSACTION
	RETURN
END

/*
delete from Attributes where IdArchive = @idArchive
IF @@ERROR != 0
BEGIN
    ROLLBACK TRANSACTION
	RETURN
END

delete from Archive where IdArchive = @idArchive
IF @@ERROR != 0
BEGIN
    ROLLBACK TRANSACTION
	RETURN
END

declare @idStorage as uniqueidentifier

select @idStorage = IdStorage from Storage where Name = @name

print @idStorage
delete from StorageArea where IdStorage = @IdStorage
IF @@ERROR != 0
BEGIN
    ROLLBACK TRANSACTION
	RETURN
END

delete from Storage where IdStorage = @idStorage
IF @@ERROR != 0
BEGIN
    ROLLBACK TRANSACTION
	RETURN
END
*/

commit
GO
/****** Object:  Default [DF_PreservationAlertTask_Enabled]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[PreservationAlertTask] ADD  CONSTRAINT [DF_PreservationAlertTask_Enabled]  DEFAULT ((1)) FOR [Enabled]
GO
/****** Object:  Default [DF_PreservationJournalingActivity_IdPreservationJournalingActivity]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationJournalingActivity] ADD  CONSTRAINT [DF_PreservationJournalingActivity_IdPreservationJournalingActivity]  DEFAULT (newid()) FOR [IdPreservationJournalingActivity]
GO
/****** Object:  Default [DF_PreservationSchedule_FrequencyType]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationSchedule] ADD  CONSTRAINT [DF_PreservationSchedule_FrequencyType]  DEFAULT ((1)) FOR [FrequencyType]
GO
/****** Object:  Default [DF_PreservationSchedule_Active]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationSchedule] ADD  CONSTRAINT [DF_PreservationSchedule_Active]  DEFAULT ((1)) FOR [Active]
GO
/****** Object:  Default [DF_PreservationStorageDevice_IdPreservationStorageDevice]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationStorageDevice] ADD  CONSTRAINT [DF_PreservationStorageDevice_IdPreservationStorageDevice]  DEFAULT (newid()) FOR [IdPreservationStorageDevice]
GO
/****** Object:  Default [DF_PreservationStorageDeviceStatus_IdPreservationStorageDeviceStatus]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationStorageDeviceStatus] ADD  CONSTRAINT [DF_PreservationStorageDeviceStatus_IdPreservationStorageDeviceStatus]  DEFAULT (newid()) FOR [IdPreservationStorageDeviceStatus]
GO
/****** Object:  Default [DF_Transito_DateCreated]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[Transito] ADD  CONSTRAINT [DF_Transito_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  ForeignKey [FK_ArchiveStorage_Archive]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[ArchiveStorage]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStorage_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[ArchiveStorage] CHECK CONSTRAINT [FK_ArchiveStorage_Archive]
GO
/****** Object:  ForeignKey [FK_ArchiveStorage_Storage]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[ArchiveStorage]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStorage_Storage] FOREIGN KEY([IdStorage])
REFERENCES [dbo].[Storage] ([IdStorage])
GO
ALTER TABLE [dbo].[ArchiveStorage] CHECK CONSTRAINT [FK_ArchiveStorage_Storage]
GO
/****** Object:  ForeignKey [FK_Attributes_Archive]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Attributes]  WITH CHECK ADD  CONSTRAINT [FK_Attributes_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[Attributes] CHECK CONSTRAINT [FK_Attributes_Archive]
GO
/****** Object:  ForeignKey [FK_Attributes_AttributeGroup]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Attributes]  WITH CHECK ADD  CONSTRAINT [FK_Attributes_AttributeGroup] FOREIGN KEY([IdAttributeGroup])
REFERENCES [dbo].[AttributesGroup] ([IdAttributeGroup])
GO
ALTER TABLE [dbo].[Attributes] CHECK CONSTRAINT [FK_Attributes_AttributeGroup]
GO
/****** Object:  ForeignKey [FK_Attributes_AttributesMode]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Attributes]  WITH CHECK ADD  CONSTRAINT [FK_Attributes_AttributesMode] FOREIGN KEY([IdMode])
REFERENCES [dbo].[AttributesMode] ([IdMode])
GO
ALTER TABLE [dbo].[Attributes] CHECK CONSTRAINT [FK_Attributes_AttributesMode]
GO
/****** Object:  ForeignKey [FK_AttributesValue_Attributes]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[AttributesValue]  WITH CHECK ADD  CONSTRAINT [FK_AttributesValue_Attributes] FOREIGN KEY([IdAttribute])
REFERENCES [dbo].[Attributes] ([IdAttribute])
GO
ALTER TABLE [dbo].[AttributesValue] CHECK CONSTRAINT [FK_AttributesValue_Attributes]
GO
/****** Object:  ForeignKey [FK_AttributesValue_Document]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[AttributesValue]  WITH CHECK ADD  CONSTRAINT [FK_AttributesValue_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[AttributesValue] CHECK CONSTRAINT [FK_AttributesValue_Document]
GO
/****** Object:  ForeignKey [FK_Cache_Document]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Cache]  WITH CHECK ADD  CONSTRAINT [FK_Cache_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Cache] CHECK CONSTRAINT [FK_Cache_Document]
GO
/****** Object:  ForeignKey [FK_Document_Archive]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Archive]
GO
/****** Object:  ForeignKey [FK_Document_CertificateStore]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_CertificateStore] FOREIGN KEY([IdCertificate])
REFERENCES [dbo].[CertificateStore] ([IdCertificate])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_CertificateStore]
GO
/****** Object:  ForeignKey [FK_Document_Document]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Document] FOREIGN KEY([IdParentBiblos])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Document]
GO
/****** Object:  ForeignKey [FK_Document_Document1]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Document1] FOREIGN KEY([IdDocumentLink])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Document1]
GO
/****** Object:  ForeignKey [FK_Document_DocumentNodeType]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_DocumentNodeType] FOREIGN KEY([IdNodeType])
REFERENCES [dbo].[DocumentNodeType] ([Id])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_DocumentNodeType]
GO
/****** Object:  ForeignKey [FK_Document_DocumentParentVersion]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_DocumentParentVersion] FOREIGN KEY([IdParentVersion])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_DocumentParentVersion]
GO
/****** Object:  ForeignKey [FK_Document_DocumentStatus]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_DocumentStatus] FOREIGN KEY([IdDocumentStatus])
REFERENCES [dbo].[DocumentStatus] ([IdDocumentStatus])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_DocumentStatus]
GO
/****** Object:  ForeignKey [FK_Document_Preservation]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Preservation] FOREIGN KEY([IdPreservation])
REFERENCES [dbo].[Preservation] ([IdPreservation])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Preservation]
GO
/****** Object:  ForeignKey [FK_Document_PreservationException]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_PreservationException] FOREIGN KEY([IdPreservationException])
REFERENCES [dbo].[PreservationException] ([IdPreservationException])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_PreservationException]
GO
/****** Object:  ForeignKey [FK_Document_Storage]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Storage] FOREIGN KEY([IdStorage])
REFERENCES [dbo].[Storage] ([IdStorage])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Storage]
GO
/****** Object:  ForeignKey [FK_Document_StorageArea]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_StorageArea] FOREIGN KEY([IdStorageArea])
REFERENCES [dbo].[StorageArea] ([IdStorageArea])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_StorageArea]
GO
/****** Object:  ForeignKey [FK_ExceptionConservazione_Conservazione]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[ExceptionConservazione]  WITH CHECK ADD  CONSTRAINT [FK_ExceptionConservazione_Conservazione] FOREIGN KEY([IdConservazione])
REFERENCES [dbo].[Preservation] ([IdPreservation])
GO
ALTER TABLE [dbo].[ExceptionConservazione] CHECK CONSTRAINT [FK_ExceptionConservazione_Conservazione]
GO
/****** Object:  ForeignKey [FK_ExceptionConservazione_Document]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[ExceptionConservazione]  WITH CHECK ADD  CONSTRAINT [FK_ExceptionConservazione_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[ExceptionConservazione] CHECK CONSTRAINT [FK_ExceptionConservazione_Document]
GO
/****** Object:  ForeignKey [FK_ExceptionConservazione_ExceptionType]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[ExceptionConservazione]  WITH CHECK ADD  CONSTRAINT [FK_ExceptionConservazione_ExceptionType] FOREIGN KEY([IdExceptionType])
REFERENCES [dbo].[PreservationExceptionType] ([IdPreservationExceptionType])
GO
ALTER TABLE [dbo].[ExceptionConservazione] CHECK CONSTRAINT [FK_ExceptionConservazione_ExceptionType]
GO
/****** Object:  ForeignKey [FK_Permission_Document]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_Permission_Document]
GO
/****** Object:  ForeignKey [FK_Permission_PermissionMode]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_PermissionMode] FOREIGN KEY([IdMode])
REFERENCES [dbo].[PermissionMode] ([IdMode])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_Permission_PermissionMode]
GO
/****** Object:  ForeignKey [FK_Conservazione_Archive]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Preservation]  WITH CHECK ADD  CONSTRAINT [FK_Conservazione_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[Preservation] CHECK CONSTRAINT [FK_Conservazione_Archive]
GO
/****** Object:  ForeignKey [FK_Preservation_PreservationTask]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Preservation]  WITH CHECK ADD  CONSTRAINT [FK_Preservation_PreservationTask] FOREIGN KEY([IdPreservationTask])
REFERENCES [dbo].[PreservationTask] ([IdPreservationTask])
GO
ALTER TABLE [dbo].[Preservation] CHECK CONSTRAINT [FK_Preservation_PreservationTask]
GO
/****** Object:  ForeignKey [FK_Preservation_PreservationTaskGroup]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Preservation]  WITH CHECK ADD  CONSTRAINT [FK_Preservation_PreservationTaskGroup] FOREIGN KEY([IdPreservationTaskGroup])
REFERENCES [dbo].[PreservationTaskGroup] ([IdPreservationTaskGroup])
GO
ALTER TABLE [dbo].[Preservation] CHECK CONSTRAINT [FK_Preservation_PreservationTaskGroup]
GO
/****** Object:  ForeignKey [FK_Preservation_PreservationUser]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[Preservation]  WITH CHECK ADD  CONSTRAINT [FK_Preservation_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[Preservation] CHECK CONSTRAINT [FK_Preservation_PreservationUser]
GO
/****** Object:  ForeignKey [FK_PreservationAlert_PreservationAlertType]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[PreservationAlert]  WITH CHECK ADD  CONSTRAINT [FK_PreservationAlert_PreservationAlertType] FOREIGN KEY([IdPreservationAlertType])
REFERENCES [dbo].[PreservationAlertType] ([IdPreservationAlertType])
GO
ALTER TABLE [dbo].[PreservationAlert] CHECK CONSTRAINT [FK_PreservationAlert_PreservationAlertType]
GO
/****** Object:  ForeignKey [FK_PreservationAlert_PreservationTask]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[PreservationAlert]  WITH CHECK ADD  CONSTRAINT [FK_PreservationAlert_PreservationTask] FOREIGN KEY([IdPreservationTask])
REFERENCES [dbo].[PreservationTask] ([IdPreservationTask])
GO
ALTER TABLE [dbo].[PreservationAlert] CHECK CONSTRAINT [FK_PreservationAlert_PreservationTask]
GO
/****** Object:  ForeignKey [FK_AlertTask_AlertType]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[PreservationAlertTask]  WITH CHECK ADD  CONSTRAINT [FK_AlertTask_AlertType] FOREIGN KEY([IdPreservationAlertType])
REFERENCES [dbo].[PreservationAlertType] ([IdPreservationAlertType])
GO
ALTER TABLE [dbo].[PreservationAlertTask] CHECK CONSTRAINT [FK_AlertTask_AlertType]
GO
/****** Object:  ForeignKey [FK_PreservationAlertTask_PreservationTaskType]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[PreservationAlertTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationAlertTask_PreservationTaskType] FOREIGN KEY([IdPreservationTaskType])
REFERENCES [dbo].[PreservationTaskType] ([IdPreservationTaskType])
GO
ALTER TABLE [dbo].[PreservationAlertTask] CHECK CONSTRAINT [FK_PreservationAlertTask_PreservationTaskType]
GO
/****** Object:  ForeignKey [FK_PreservationAlertType_PreservationRole]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[PreservationAlertType]  WITH CHECK ADD  CONSTRAINT [FK_PreservationAlertType_PreservationRole] FOREIGN KEY([IdPreservationRole])
REFERENCES [dbo].[PreservationRole] ([IdPreservationRole])
GO
ALTER TABLE [dbo].[PreservationAlertType] CHECK CONSTRAINT [FK_PreservationAlertType_PreservationRole]
GO
/****** Object:  ForeignKey [FK_AziendaConservazione_Archive]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[PreservationCompany]  WITH CHECK ADD  CONSTRAINT [FK_AziendaConservazione_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationCompany] CHECK CONSTRAINT [FK_AziendaConservazione_Archive]
GO
/****** Object:  ForeignKey [FK_PreservationException_PreservationExceptionType]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[PreservationException]  WITH CHECK ADD  CONSTRAINT [FK_PreservationException_PreservationExceptionType] FOREIGN KEY([IdPreservationExceptionType])
REFERENCES [dbo].[PreservationExceptionType] ([IdPreservationExceptionType])
GO
ALTER TABLE [dbo].[PreservationException] CHECK CONSTRAINT [FK_PreservationException_PreservationExceptionType]
GO
/****** Object:  ForeignKey [FK_PreservationInStorageDevice_Preservation]    Script Date: 10/17/2011 10:48:46 ******/
ALTER TABLE [dbo].[PreservationInStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationInStorageDevice_Preservation] FOREIGN KEY([IdPreservation])
REFERENCES [dbo].[Preservation] ([IdPreservation])
GO
ALTER TABLE [dbo].[PreservationInStorageDevice] CHECK CONSTRAINT [FK_PreservationInStorageDevice_Preservation]
GO
/****** Object:  ForeignKey [FK_PreservationInStorageDevice_PreservationStorageDevice]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationInStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationInStorageDevice_PreservationStorageDevice] FOREIGN KEY([IdPreservationStorageDevice])
REFERENCES [dbo].[PreservationStorageDevice] ([IdPreservationStorageDevice])
GO
ALTER TABLE [dbo].[PreservationInStorageDevice] CHECK CONSTRAINT [FK_PreservationInStorageDevice_PreservationStorageDevice]
GO
/****** Object:  ForeignKey [FK_PreservationJournaling_Preservation]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationJournaling]  WITH CHECK ADD  CONSTRAINT [FK_PreservationJournaling_Preservation] FOREIGN KEY([IdPreservation])
REFERENCES [dbo].[Preservation] ([IdPreservation])
GO
ALTER TABLE [dbo].[PreservationJournaling] CHECK CONSTRAINT [FK_PreservationJournaling_Preservation]
GO
/****** Object:  ForeignKey [FK_PreservationJournaling_PreservationJournalingActivity]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationJournaling]  WITH CHECK ADD  CONSTRAINT [FK_PreservationJournaling_PreservationJournalingActivity] FOREIGN KEY([IdPreservationJournalingActivity])
REFERENCES [dbo].[PreservationJournalingActivity] ([IdPreservationJournalingActivity])
GO
ALTER TABLE [dbo].[PreservationJournaling] CHECK CONSTRAINT [FK_PreservationJournaling_PreservationJournalingActivity]
GO
/****** Object:  ForeignKey [FK_PreservationJournaling_PreservationUser]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationJournaling]  WITH CHECK ADD  CONSTRAINT [FK_PreservationJournaling_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[PreservationJournaling] CHECK CONSTRAINT [FK_PreservationJournaling_PreservationUser]
GO
/****** Object:  ForeignKey [FK_ParameterConservazione_Archive]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationParameters]  WITH CHECK ADD  CONSTRAINT [FK_ParameterConservazione_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationParameters] CHECK CONSTRAINT [FK_ParameterConservazione_Archive]
GO
/****** Object:  ForeignKey [FK_PreservationSchedule_TaskType_Schedule]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationSchedule_TaskType]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationSchedule_TaskType_Schedule] FOREIGN KEY([IdPreservationSchedule])
REFERENCES [dbo].[PreservationSchedule] ([IdPreservationSchedule])
GO
ALTER TABLE [dbo].[PreservationSchedule_TaskType] CHECK CONSTRAINT [FK_PreservationSchedule_TaskType_Schedule]
GO
/****** Object:  ForeignKey [FK_PreservationSchedule_TaskType_TaskType]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationSchedule_TaskType]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationSchedule_TaskType_TaskType] FOREIGN KEY([IdPreservationTaskType])
REFERENCES [dbo].[PreservationTaskType] ([IdPreservationTaskType])
GO
ALTER TABLE [dbo].[PreservationSchedule_TaskType] CHECK CONSTRAINT [FK_PreservationSchedule_TaskType_TaskType]
GO
/****** Object:  ForeignKey [FK_PreservationStorageDevice_Archive]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationStorageDevice_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationStorageDevice] CHECK CONSTRAINT [FK_PreservationStorageDevice_Archive]
GO
/****** Object:  ForeignKey [FK_PreservationStorageDevice_PreservationStorageDevice]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationStorageDevice_PreservationStorageDevice] FOREIGN KEY([IdPreservationStorageDeviceOriginal])
REFERENCES [dbo].[PreservationStorageDevice] ([IdPreservationStorageDevice])
GO
ALTER TABLE [dbo].[PreservationStorageDevice] CHECK CONSTRAINT [FK_PreservationStorageDevice_PreservationStorageDevice]
GO
/****** Object:  ForeignKey [FK_PreservationStorageDevice_PreservationStorageDeviceStatus]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationStorageDevice_PreservationStorageDeviceStatus] FOREIGN KEY([IdPreservationStorageDeviceStatus])
REFERENCES [dbo].[PreservationStorageDeviceStatus] ([IdPreservationStorageDeviceStatus])
GO
ALTER TABLE [dbo].[PreservationStorageDevice] CHECK CONSTRAINT [FK_PreservationStorageDevice_PreservationStorageDeviceStatus]
GO
/****** Object:  ForeignKey [FK_PreservationTask_Archive]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_Archive]
GO
/****** Object:  ForeignKey [FK_PreservationTask_PreservationTaskGroup]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_PreservationTaskGroup] FOREIGN KEY([IdPreservationTaskGroup])
REFERENCES [dbo].[PreservationTaskGroup] ([IdPreservationTaskGroup])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_PreservationTaskGroup]
GO
/****** Object:  ForeignKey [FK_PreservationTask_PreservationTaskStatus]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_PreservationTaskStatus] FOREIGN KEY([IdPreservationTaskStatus])
REFERENCES [dbo].[PreservationTaskStatus] ([IdPreservationTaskStatus])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_PreservationTaskStatus]
GO
/****** Object:  ForeignKey [FK_PreservationTask_PreservationTaskType]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_PreservationTaskType] FOREIGN KEY([IdPreservationTaskType])
REFERENCES [dbo].[PreservationTaskType] ([IdPreservationTaskType])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_PreservationTaskType]
GO
/****** Object:  ForeignKey [FK_PreservationTask_PreservationUser]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_PreservationUser]
GO
/****** Object:  ForeignKey [FK_PreservationTaskGroup_PreservationSchedule]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTaskGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationTaskGroup_PreservationSchedule] FOREIGN KEY([IdPreservationSchedule])
REFERENCES [dbo].[PreservationSchedule] ([IdPreservationSchedule])
GO
ALTER TABLE [dbo].[PreservationTaskGroup] CHECK CONSTRAINT [FK_PreservationTaskGroup_PreservationSchedule]
GO
/****** Object:  ForeignKey [FK_PreservationTaskGroup_PreservationTaskGroupType]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTaskGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationTaskGroup_PreservationTaskGroupType] FOREIGN KEY([IdPreservationTaskGroupType])
REFERENCES [dbo].[PreservationTaskGroupType] ([IdPreservationTaskGroupType])
GO
ALTER TABLE [dbo].[PreservationTaskGroup] CHECK CONSTRAINT [FK_PreservationTaskGroup_PreservationTaskGroupType]
GO
/****** Object:  ForeignKey [FK_PreservationTaskGroup_PreservationUser]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTaskGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationTaskGroup_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[PreservationTaskGroup] CHECK CONSTRAINT [FK_PreservationTaskGroup_PreservationUser]
GO
/****** Object:  ForeignKey [FK_PreservationTaskRole_PreservationRole]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTaskRole]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTaskRole_PreservationRole] FOREIGN KEY([IdPreservationRole])
REFERENCES [dbo].[PreservationRole] ([IdPreservationRole])
GO
ALTER TABLE [dbo].[PreservationTaskRole] CHECK CONSTRAINT [FK_PreservationTaskRole_PreservationRole]
GO
/****** Object:  ForeignKey [FK_PreservationTaskRole_PreservationTaskType]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTaskRole]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTaskRole_PreservationTaskType] FOREIGN KEY([IdPreservationTaskType])
REFERENCES [dbo].[PreservationTaskType] ([IdPreservationTaskType])
GO
ALTER TABLE [dbo].[PreservationTaskRole] CHECK CONSTRAINT [FK_PreservationTaskRole_PreservationTaskType]
GO
/****** Object:  ForeignKey [FK_PreservationTaskType_PreservationPeriod]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationTaskType]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTaskType_PreservationPeriod] FOREIGN KEY([IdPreservationPeriod])
REFERENCES [dbo].[PreservationPeriod] ([IdPreservationPeriod])
GO
ALTER TABLE [dbo].[PreservationTaskType] CHECK CONSTRAINT [FK_PreservationTaskType_PreservationPeriod]
GO
/****** Object:  ForeignKey [FK_PreservationUserRole_PreservationRole]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationUserRole]  WITH CHECK ADD  CONSTRAINT [FK_PreservationUserRole_PreservationRole] FOREIGN KEY([IdPreservationRole])
REFERENCES [dbo].[PreservationRole] ([IdPreservationRole])
GO
ALTER TABLE [dbo].[PreservationUserRole] CHECK CONSTRAINT [FK_PreservationUserRole_PreservationRole]
GO
/****** Object:  ForeignKey [FK_PreservationUserRole_PreservationUser]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationUserRole]  WITH CHECK ADD  CONSTRAINT [FK_PreservationUserRole_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[PreservationUserRole] CHECK CONSTRAINT [FK_PreservationUserRole_PreservationUser]
GO
/****** Object:  ForeignKey [FK_RoleUserConservazione_Archive]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[PreservationUserRole]  WITH CHECK ADD  CONSTRAINT [FK_RoleUserConservazione_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationUserRole] CHECK CONSTRAINT [FK_RoleUserConservazione_Archive]
GO
/****** Object:  ForeignKey [FK_Storage_StorageType]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[Storage]  WITH CHECK ADD  CONSTRAINT [FK_Storage_StorageType] FOREIGN KEY([IdStorageType])
REFERENCES [dbo].[StorageType] ([IdStorageType])
GO
ALTER TABLE [dbo].[Storage] CHECK CONSTRAINT [FK_Storage_StorageType]
GO
/****** Object:  ForeignKey [FK_StorageArea_Storage]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[StorageArea]  WITH CHECK ADD  CONSTRAINT [FK_StorageArea_Storage] FOREIGN KEY([IdStorage])
REFERENCES [dbo].[Storage] ([IdStorage])
GO
ALTER TABLE [dbo].[StorageArea] CHECK CONSTRAINT [FK_StorageArea_Storage]
GO
/****** Object:  ForeignKey [FK_StorageArea_StorageStatus]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[StorageArea]  WITH CHECK ADD  CONSTRAINT [FK_StorageArea_StorageStatus] FOREIGN KEY([IdStorageStatus])
REFERENCES [dbo].[StorageStatus] ([IdStorageStatus])
GO
ALTER TABLE [dbo].[StorageArea] CHECK CONSTRAINT [FK_StorageArea_StorageStatus]
GO
/****** Object:  ForeignKey [FK_StorageAreaRule_Attributes]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[StorageAreaRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageAreaRule_Attributes] FOREIGN KEY([IdAttribute])
REFERENCES [dbo].[Attributes] ([IdAttribute])
GO
ALTER TABLE [dbo].[StorageAreaRule] CHECK CONSTRAINT [FK_StorageAreaRule_Attributes]
GO
/****** Object:  ForeignKey [FK_StorageAreaRule_RuleOperator]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[StorageAreaRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageAreaRule_RuleOperator] FOREIGN KEY([IdRuleOperator])
REFERENCES [dbo].[RuleOperator] ([IdRuleOperator])
GO
ALTER TABLE [dbo].[StorageAreaRule] CHECK CONSTRAINT [FK_StorageAreaRule_RuleOperator]
GO
/****** Object:  ForeignKey [FK_StorageAreaRule_StorageArea]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[StorageAreaRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageAreaRule_StorageArea] FOREIGN KEY([IdStorageArea])
REFERENCES [dbo].[StorageArea] ([IdStorageArea])
GO
ALTER TABLE [dbo].[StorageAreaRule] CHECK CONSTRAINT [FK_StorageAreaRule_StorageArea]
GO
/****** Object:  ForeignKey [FK_StorageRule_Attributes]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[StorageRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageRule_Attributes] FOREIGN KEY([IdAttribute])
REFERENCES [dbo].[Attributes] ([IdAttribute])
GO
ALTER TABLE [dbo].[StorageRule] CHECK CONSTRAINT [FK_StorageRule_Attributes]
GO
/****** Object:  ForeignKey [FK_StorageRule_RuleOperator]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[StorageRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageRule_RuleOperator] FOREIGN KEY([IdRuleOperator])
REFERENCES [dbo].[RuleOperator] ([IdRuleOperator])
GO
ALTER TABLE [dbo].[StorageRule] CHECK CONSTRAINT [FK_StorageRule_RuleOperator]
GO
/****** Object:  ForeignKey [FK_StorageRule_Storage]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[StorageRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageRule_Storage] FOREIGN KEY([IdStorage])
REFERENCES [dbo].[Storage] ([IdStorage])
GO
ALTER TABLE [dbo].[StorageRule] CHECK CONSTRAINT [FK_StorageRule_Storage]
GO
/****** Object:  ForeignKey [FK_Transito_Document]    Script Date: 10/17/2011 10:48:47 ******/
ALTER TABLE [dbo].[Transito]  WITH CHECK ADD  CONSTRAINT [FK_Transito_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Transito] CHECK CONSTRAINT [FK_Transito_Document]
GO
