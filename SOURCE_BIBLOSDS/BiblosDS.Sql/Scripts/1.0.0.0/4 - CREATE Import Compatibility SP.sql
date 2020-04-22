-- =============================================
-- Script Template
-- =============================================
USE [BiblosDS2010]
GO

/****** Object:  StoredProcedure [dbo].[Migrate_AttributesFilename]    Script Date: 09/30/2011 18:46:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[Migrate_AttributesFilename]
	@idattribute as uniqueidentifier
as
update document
set name = attributesvalue.valuestring
from attributesvalue
where attributesvalue.iddocument=document.iddocument
and attributesvalue.idattribute = @idattribute

GO

/****** Object:  StoredProcedure [dbo].[Migrate_AttributesStringToValueDateTime]    Script Date: 09/30/2011 18:46:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[Migrate_AttributesStringToValueDateTime]
as

update dbo.AttributesValue
set ValueDateTime = CONVERT(datetime, ValueString, 103) 
where not ValueString is null
and ValueDateTime is null
and idattribute in
(
select idattribute from attributes
where AttributeType = 'System.DateTime'
)

GO

/****** Object:  StoredProcedure [dbo].[Migrate_AttributesStringToValueInt]    Script Date: 09/30/2011 18:46:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[Migrate_AttributesStringToValueInt]
as
update dbo.AttributesValue
set ValueInt = CONVERT(bigint, cast(ValueString as int)) 
where not ValueString is null
and ValueInt is null
and idattribute in
(
select idattribute from attributes
where AttributeType = 'System.Int64'
)
and ISNUMERIC(ValueString)= 1

GO



/****** Object:  StoredProcedure [Migrate_CreateStorage]    Script Date: 09/30/2011 18:44:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Migrate_CreateStorage]
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
			SELECT     NEWID(), @idStorageType, Drive.IdentificativoHW + '\'+@nomeArchivio, @nomeArchivio + '('+RTRIM(LTRIM(STR(Drive.IdSupporto)))+')' , 0, 1
			FROM         BiblosDs.dbo.AssegnazioneSlot INNER JOIN
								  BiblosDs.dbo.Archivio AS Archivio ON BiblosDs.dbo.AssegnazioneSlot.IdArchivio = Archivio.IdArchivio INNER JOIN
								  BiblosDs.dbo.Supporto ON BiblosDs.dbo.AssegnazioneSlot.IdSlot = BiblosDs.dbo.Supporto.IdSlot
										 AND BiblosDs.dbo.AssegnazioneSlot.IdDeposito = BiblosDs.dbo.Supporto.IdDeposito INNER JOIN
								  BiblosDs.dbo.Drive AS Drive ON BiblosDs.dbo.Supporto.IdSupporto = Drive.IdSupporto
			WHERE     (Archivio.NomeArchivio = @nomeArchivio)           
            
     select * from Storage where Name like @nomeArchivio+'%'         
 end
             
end

GO

/****** Object:  StoredProcedure [Migrate_FileSupporto_StorageArea]    Script Date: 09/30/2011 18:45:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Migrate_FileSupporto_StorageArea]
	@OldArchivio as varchar(255), 
	@IdSupporto as int, 
	@IdStorage as uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DECLARE @cmd AS NVARCHAR(1000)

SET @cmd ='INSERT INTO [BiblosDS2010].[dbo].[StorageArea]
           ([IdStorageArea]
           ,[IdStorage]
           ,[Path]
           ,[Name]
           ,[Priority] 
           ,[MaxSize] 
           ,[MaxFileNumber]                              
           ,[Enable])
select NEWID(), '''+cast(@IdStorage as varchar(255))+''',
       Right(''000000000000''+cast(FileSup.IdFile as varchar(255)), 8), cast(FileSup.IdFile as varchar(255)), FileSup.IdFile, 2048, 100000, 0 
FROM   ' + @OldArchivio + '.dbo.FileSup FileSup
where IdSupporto = '+ STR(@idSupporto) +'
and IdFile not in (select CAST(name as int) from [BiblosDS2010].[dbo].[StorageArea] where IdStorage = '''+cast(@IdStorage as varchar(255))+''')'

print @cmd
EXEC sp_executesql @cmd

select * from dbo.StorageArea
where IdStorage = @IdStorage
order by name

END

GO

/****** Object:  StoredProcedure [Migrate_OggettoSupporto_DocumentStorage]    Script Date: 09/30/2011 18:45:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Migrate_OggettoSupporto_DocumentStorage]
	@OldArchive as nvarchar(255),
	@IdArchive as uniqueidentifier,
	@IdSupporto as int, 
	@IdStorage as uniqueidentifier
AS
BEGIN
	DECLARE @SQLCMD as NVARCHAR(4000) 
	
	SET NOCOUNT ON;
	
	

SET @SQLCMD = 'INSERT INTO [BiblosDS2010].[dbo].[Document]
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
				SELECT  '+@OldArchive+'.dbo.Oggetto.IdOggetto,
				 newid(),
				 (select top 1 IdDocument from [BiblosDS2010].[dbo].Document 
					where IdBiblos = '+@OldArchive+'.dbo.Oggetto.IdOggettoPadre and IdArchive=''' + cast(@IdArchive as varchar(255)) + '''), 
				 (select top 1 IdStorageArea from [BiblosDS2010].[dbo].[StorageArea] 
					where Name = cast('+@OldArchive+'.dbo.FileSup.IdFile as varchar(255)) and IdStorage=''' + cast(@IdStorage as varchar(255)) + '''), 
				 ''' + cast(@IdStorage as varchar(255)) + ''', 
				 ''' + cast(@IdArchive as varchar(255)) + ''',
				 1,
				 null,
				 ''AUTOLOAD_CHILD'',
				 '+@OldArchive+'.dbo.Ubicazione.Lunghezza,
				 GETDATE(),
				 1,
				 1,
				 3,
				 0              
				   FROM   '+@OldArchive+'.dbo.FileSup INNER JOIN
						  '+@OldArchive+'.dbo.Ubicazione ON '+@OldArchive+'.dbo.FileSup.IdFile = '+@OldArchive+'.dbo.Ubicazione.IdFile right JOIN
						  '+@OldArchive+'.dbo.Oggetto ON '+@OldArchive+'.dbo.Ubicazione.IdOggetto = '+@OldArchive+'.dbo.Oggetto.IdOggetto  
						  where '+@OldArchive+'.dbo.Oggetto.IdOggettoPadre is not null and
						  '+@OldArchive+'.dbo.Oggetto.IdOggettoPadre <> 0
						  and '+@OldArchive+'.dbo.FileSup.IdSupporto = ' + STR(@IdSupporto) +'
						  and '+@OldArchive+'.dbo.Oggetto.IdOggetto in 
						  (
							select idoggetto from '+@OldArchive+'.dbo.Ubicazione
						  ) and '+@OldArchive+'.dbo.Oggetto.IdOggetto not in
						   (
							select IdBiblos from [BiblosDS2010].[dbo].[Document] where idarchive =  ''' + cast(@idArchive as varchar(255)) + '''
						   )'

print @SQLCMD						  
EXEC sp_executesql @SQLCMD

END

GO

/****** Object:  StoredProcedure [Migrate_ParentObjects_Documents]    Script Date: 09/30/2011 18:45:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Migrate_ParentObjects_Documents]
	@OldArchive as varchar(255),
	@NewArchive as varchar(255) 
AS
BEGIN
	DECLARE @SQLCMD as varchar(2000) 
	DECLARE @IdArchive as uniqueidentifier 
	
	SET NOCOUNT ON;
	
	SET @IdArchive = (SELECT IdArchive FROM [BiblosDS2010].dbo.Archive WHERE Name = @NewArchive) 
	IF @IdArchive IS NULL 
	BEGIN
		Print 'Archivio ' + @NewArchive + 'non trovato.' 
	END 
	ELSE 
	BEGIN 
		SET @SQLCMD = 'insert INTO [BiblosDS2010].[dbo].[Document]
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
           SELECT  '+@OldArchive+'.dbo.Oggetto.IdOggetto,
             newid(),
             NULL,
             NULL,  
             NULL,
             ''' + cast(@idArchive as varchar(255)) + ''',
             1,
             null,
             ''AUTOLOAD'',
             NULL,
             GETDATE(),
             1,
             1,
             3,
             0  
              FROM '+@OldArchive+'.dbo.Oggetto   
           WHERE '+@OldArchive+'.dbo.Oggetto.IdOggettoPadre is null or '+@OldArchive+'.dbo.Oggetto.IdOggettoPadre = 0
           and '+@OldArchive+'.dbo.Oggetto.IdOggetto not in
           (
			select IdBiblos from [BiblosDS2010].[dbo].[Document] where idarchive =  ''' + cast(@idArchive as varchar(255)) + '''
           )'
        --print @SQLCMD
        exec( @SQLCMD)
	END
END

GO


/****** Object:  StoredProcedure [Migrate_UpdateIdLastBiblos]    Script Date: 09/30/2011 18:46:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Migrate_UpdateIdLastBiblos]
	@newArchive varchar(255)
as 
declare @idArchive as uniqueidentifier 


if exists(select * from BiblosDS2010.dbo.Archive where Name = @newArchive)
begin	
	select @idArchive = IdArchive from BiblosDS2010.dbo.Archive where Name = @newArchive
	
	update Archive
	set LastIdBiblos = (Select MAX(IdBiblos) +1 from Document where IdArchive = @idArchive)
	where IdArchive = @idArchive
end
else
begin
	print 'Nessun archive presente con il nome selezionato.';
end

GO

/****** Object:  StoredProcedure [Migrate_ValoreAttributoOggetto_AttributeValue]    Script Date: 09/30/2011 18:46:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Migrate_ValoreAttributoOggetto_AttributeValue]
	@OldArchive as varchar(255),
	@NewArchive as varchar(255) 
as 
BEGIN
	DECLARE @IdArchive as uniqueidentifier 
	DECLARE @cmd AS NVARCHAR(4000)

	SET NOCOUNT ON;
	
	SET @IdArchive = (SELECT IdArchive FROM [BiblosDS2010].dbo.Archive WHERE Name = @NewArchive) 
	IF @IdArchive IS NULL 
	BEGIN
		Print 'Archivio ' + @NewArchive + ' non trovato.' 
	END 
	ELSE 
	BEGIN 
		SET @cmd = '
			INSERT INTO [BiblosDS2010].[dbo].[AttributesValue]
					   ([IdDocument]
					   ,[IdAttribute]          
					   ,[ValueString])
			SELECT 
			(select top 1 IdDocument from BiblosDS2010.dbo.Document where IdBiblos = '+ @OldArchive +'.[dbo].[ValoreAttributoOggetto].IdOggetto 
									   and IdArchive='''+ cast(@idArchive as varchar(255)) +'''),
			(select top 1 IdAttribute from BiblosDS2010.dbo.Attributes where IdArchive=''' + cast(@idArchive as varchar(255)) + '''
									   and Name='+ @OldArchive +'.[dbo].[ValoreAttributoOggetto].NomeAttributoOggetto collate Latin1_General_CI_AS  )             
				  ,'+ @OldArchive +'.[dbo].[ValoreAttributoOggetto].[Valore]
			FROM  '+ @OldArchive +'.[dbo].[ValoreAttributoOggetto]
			where IdOggetto in 
			(
				select IdOggetto from '++ @OldArchive +'.dbo.Ubicazione
			) and IdOggetto in
						   (
							select IdBiblos from [BiblosDS2010].[dbo].[Document] where idarchive =  ''' + cast(@idArchive as varchar(255)) + '''
						   )'

		EXEC sp_executesql @cmd
		print @cmd
	END
END

GO


/****** Object:  StoredProcedure [Migrate_ArchiveNoLegal]    Script Date: 10/17/2011 11:31:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [Migrate_ArchiveNoLegal] 
	@OldArchive as varchar(255),
	@NewArchive as varchar(255) 
AS
BEGIN
	DECLARE @SQLCMD as nvarchar(1000) 
	DECLARE @IdArchive as uniqueidentifier 

	SET NOCOUNT ON;

	SET @IdArchive = (SELECT IdArchive FROM [BiblosDS2010].dbo.Archive WHERE Name = @NewArchive) 
	IF @IdArchive IS NULL 
	BEGIN
		Print 'Archivio ' + @NewArchive + 'non trovato.' 
	END 
	ELSE 
	BEGIN 	
		-- Import degli attributi 
		SET @SQLCMD = N'INSERT INTO [BiblosDS2010].[dbo].Attributes  (
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
					''' + cast(@idArchive as varchar(255)) + ''' as IdArchive, 
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
			FROM  [' + @OldArchive +'].dbo.AttributoOggetto'
			
			print @SQLCMD
			EXEC sp_executesql @SQLCMD
	END
END

GO

create procedure MigrateArchiveLegal
	@DestinationDb as varchar(255),
	@SourceDb as varchar(255),
	@IdArchive as uniqueidentifier
as
DECLARE @Collation		AS VARCHAR(256)
Set @Collation		= 'Latin1_General_CI_AS'
DECLARE @Sql as nvarchar(1000) 
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
				+ 'SELECT NEWID() AS IdAttribute, attOgg.Nome as Name, ' + cast(@IdArchive as varchar(255))+ ', '
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
				+ 'WHERE NOT att.Name IS NULL'
		
		EXEC (@Sql)


		GO
		create procedure Migrate_StoragePathView
	@nomeArchivio as varchar(255)
as

SELECT     NEWID(), Drive.IdentificativoHW + '\'+@nomeArchivio, @nomeArchivio + '('+RTRIM(LTRIM(STR(Drive.IdSupporto)))+')' , 0, 1
			FROM         BiblosDs.dbo.AssegnazioneSlot INNER JOIN
								  BiblosDs.dbo.Archivio AS Archivio ON BiblosDs.dbo.AssegnazioneSlot.IdArchivio = Archivio.IdArchivio INNER JOIN
								  BiblosDs.dbo.Supporto ON BiblosDs.dbo.AssegnazioneSlot.IdSlot = BiblosDs.dbo.Supporto.IdSlot
										 AND BiblosDs.dbo.AssegnazioneSlot.IdDeposito = BiblosDs.dbo.Supporto.IdDeposito INNER JOIN
								  BiblosDs.dbo.Drive AS Drive ON BiblosDs.dbo.Supporto.IdSupporto = Drive.IdSupporto
			WHERE     (Archivio.NomeArchivio = @nomeArchivio) 
		
go

--set @nomeArchivio = 'BIBLATTI'
--set @MainPath = 'C:\Program Files\Vecomp Software\Biblos DS Server\Repository\Atti\'

create procedure Migrate_StorageWriteConfigurationCreate
	@MainPath as varchar(500),
	@nomeArchivio as varchar(255)
as


declare @IdStorage as uniqueidentifier
declare @IdArchive as uniqueidentifier
if (not exists(select * from Archive where Name = @nomeArchivio))
begin
		RAISERROR('Archivio non valido, definire un archivio presente nella tabella archive.', 18, 1)
		RETURN	
end
else
begin
set @IdStorage = NEWID()
select @IdArchive = IdArchive from Archive where Name = @nomeArchivio

select @IdStorage AS Storage, @IdArchive AS Archive
	
	BEGIN TRANSACTION TR_DataTransfer
	print 'Inizio creazione storage'
	
INSERT INTO [BiblosDS2010].[dbo].[Storage]
           ([IdStorage]
           ,[IdStorageType]
           ,[MainPath]
           ,[Name]
           ,[StorageRuleAssembly]
           ,[StorageRuleClassName]
           ,[Priority]
           ,[EnableFulText]
           ,[AuthenticationKey]
           ,[AuthenticationPassword]
           ,[IsVisible])
     VALUES
           (@IdStorage
           ,(select top 1 IdStorageType from StorageType where StorageClassName='BDSComStorage')
           ,@MainPath
           ,@nomeArchivio
           ,null
           ,null
           ,0
           ,0
           ,null
           ,null
           ,1)
           
           IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
           print 'Inizio creazione [StorageArea]'
           
           INSERT INTO [BiblosDS2010].[dbo].[StorageArea]
           ([IdStorageArea]
           ,[IdStorage]
           ,[Path]
           ,[Name]
           ,[Priority]
           ,[IdStorageStatus]
           ,[MaxSize]
           ,[CurrentSize]
           ,[MaxFileNumber]
           ,[CurrentFileNumber]
           ,[Enable])
     VALUES
           (NEWID()
           ,@IdStorage
           ,'Supporto'
           ,'Supporto_'+ @nomeArchivio
           ,0
           ,null
           ,104857600
           ,0
           ,2000
           ,0
           ,1)
           
           IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
           print 'Inizio creazione [ArchiveStorage]'
           
           INSERT INTO [BiblosDS2010].[dbo].[ArchiveStorage]
           ([IdArchive]
           ,[IdStorage]
           ,[Active])
     VALUES
           (@IdArchive
           ,@IdStorage
           ,1)
           
           IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		COMMIT TRANSACTION TR_DataTransfer 
end
GO

create procedure Migrate_ArchiveDirect
	
as
INSERT INTO [BiblosDS2010].[dbo].[Archive]
           ([IdArchive]
           ,[Name]
           ,[IsLegal]
           ,[MaxCache]
           ,[UpperCache]
           ,[LowerCache]
           ,[LastIdBiblos]
           ,[AutoVersion]
           ,[AuthorizationAssembly]
           ,[AuthorizationClassName]
           ,[EnableSecurity]
           ,[PathTransito]
           ,[PathCache]
           ,[PathPreservation]
           ,[LastAutoIncValue]
           ,[ThumbnailEnabled]
           ,[PdfConversionEnabled]
           ,[FullSignEnabled]
           ,[VerifyPreservationDateEnabled]
           ,[VerifyPreservationIncrementalEnabled]
           ,[TransitoEnabled]
           ,[FiscalDocumentType])
select newid(), NomeArchivio, 0, 1048576000, 52428800, 52428800, 0, 0, null, null, 0, 
'D:\BiblosDS2010\Transito\'+ NomeArchivio, null, null, 0, 0, 0, 1, null, null, 1, null  from biblosds.dbo.archivio
GO