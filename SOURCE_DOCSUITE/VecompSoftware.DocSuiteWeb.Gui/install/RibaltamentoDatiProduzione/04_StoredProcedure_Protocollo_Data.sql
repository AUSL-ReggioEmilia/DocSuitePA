-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vecomp Software Srl
-- Create date: 2014-02-12
-- Description:	Ribaltamento dati da Produzione per Validazione - Protocollo - Data
-- =============================================
CREATE PROCEDURE GenerateValidationData_Data
	-- Dichiarazione variabili
	@StartDate Date = '20130101',
	@EndDate Date = '20131231',
	@DefaultDocumentName varchar(MAX) = 'Documento.pdf',
	@DefaultBiblosId varchar(MAX) = '100',
	@DefaultBiblosGuid uniqueidentifier = '00000000-0000-0000-0000-000000000000',
	@EnavContactNodeId varchar(MAX) = '250',
	
	-- Creazione funzioni di utilità
	-- Creazione funzione [Random]
	@vwRandom nvarchar(MAX) = 'CREATE VIEW dbo.vwRandom AS SELECT RAND() as RandomValue;',
	-- Creazione funzione [Scramble]
	@fnScramble nvarchar(MAX) = 'CREATE FUNCTION Character_Scramble(@OrigVal varchar(max))
		RETURNS varchar(max)
		WITH ENCRYPTION
		AS
		BEGIN
		-- Variables used
		DECLARE @NewVal varchar(max);
		DECLARE @OrigLen int;
		DECLARE @CurrLen int;
		DECLARE @LoopCt int;
		DECLARE @Rand int; 
		-- Set variable default values
		SET @NewVal = '''';
		SET @OrigLen = DATALENGTH(@OrigVal);
		SET @CurrLen = @OrigLen;
		SET @LoopCt = 1; 
		-- Loop through the characters passed
		WHILE @LoopCt <= @OrigLen
			BEGIN
				-- Current length of possible characters
				SET @CurrLen = DATALENGTH(@OrigVal);
				-- Random position of character to use
				SELECT
					@Rand = Convert(int,(((1) - @CurrLen) * RandomValue + @CurrLen))
				FROM
					dbo.vwRandom;
				-- Assembles the value to be returned
				SET @NewVal = @NewVal + SUBSTRING(@OrigVal,@Rand,1);
				-- Removes the character from available options
				SET @OrigVal = Replace(@OrigVal,SUBSTRING(@OrigVal,@Rand,1),'''');
				-- Advance the loop
				SET @LoopCt = @LoopCt + 1;
			END
			-- Returns new value
			Return LOWER(@NewVal);
		END',
	-- Creazione funzione [Mask]
	@fnCharacterMask nvarchar(MAX) = 'CREATE FUNCTION Character_Mask(@OrigVal varchar(max),@InPlain int,@MaskChar char(1))
		RETURNS varchar(max)
		WITH ENCRYPTION
		AS
		BEGIN
			-- Variables used
			DECLARE @PlainVal varchar(max);
			DECLARE @MaskVal varchar(max);
			DECLARE @MaskLen int;
			-- Captures the portion of @OrigVal that remains in plain text
			SET @PlainVal = RIGHT(@OrigVal,@InPlain);
			-- Defines the length of the repeating value for the mask
			SET @MaskLen = (DATALENGTH(@OrigVal) - @InPlain);
			-- Captures the mask value
			SET @MaskVal = REPLICATE(@MaskChar, @MaskLen);
			-- Returns the masked value
			Return @MaskVal + @PlainVal;
		END',
	-- Creazione funzione [NumericVariance]
	@fnNumericVariance nvarchar(MAX) = 'CREATE FUNCTION Numeric_Variance(@OrigVal float,@VarPercent numeric(5,2))
		RETURNS float
		WITH ENCRYPTION
		AS
		BEGIN
			-- Variable used
			DECLARE @Rand int;
			-- Random position of character to use
			SELECT
				@Rand = Convert(int,((((0-@VarPercent)+1) - @VarPercent) * RandomValue + @VarPercent))
			FROM
				dbo.vwRandom;
			RETURN @OrigVal + CONVERT(INT,((@OrigVal*@Rand)/100));
		END',
	-- Creazione funzione [RandomChar]
	@fnRandomChar nvarchar(MAX) = 'CREATE FUNCTION RandomChar(@OrigVal varchar(max))
		RETURNS varchar(max)
		WITH ENCRYPTION
		AS
		BEGIN
		-- Variables used
		DECLARE @NewVal varchar(max);
		DECLARE @OrigLen int;
		DECLARE @LoopCt int;
		DECLARE @Rand char; 
		-- Set variable default values
		SET @NewVal = '''';
		SET @OrigLen = DATALENGTH(@OrigVal);
		SET @LoopCt = 1; 
		-- Loop through the characters passed
		WHILE @LoopCt <= @OrigLen
			BEGIN
				-- Random character to use
				SET @Rand = SUBSTRING(@OrigVal,@LoopCt,1)
				IF @Rand NOT IN ('' '',''.'')
				BEGIN
					SELECT
						@Rand = CHAR(CAST( (122 - 65) * RandomValue + 65 AS INTEGER ))
					FROM
						dbo.vwRandom;
				END		
				-- Assembles the value to be returned
				SET @NewVal = @NewVal + @Rand;
				-- Advance the loop
				SET @LoopCt = @LoopCt + 1;
			END
			-- Returns new value
			Return @NewVal;
		END',
	-- Pulizia
	@dropFunctions nvarchar(MAX) =
	'-- Cancellazione funzione [Random]
		DROP VIEW dbo.vwRandom
		-- Cancellazione funzione [Scramble]
		DROP FUNCTION Character_Scramble
		-- Cancellazione funzione [Mask]
		DROP FUNCTION Character_Mask
		-- Cancellazione funzione [NumericVariance]
		DROP FUNCTION Numeric_Variance
		-- Cancellazione funzione [RandomChar]
		DROP FUNCTION RandomChar'
		
AS
BEGIN
	EXEC(@vwRandom)
	EXEC(@fnScramble)
	EXEC(@fnCharacterMask)
	EXEC(@fnNumericVariance)
	EXEC(@fnRandomChar)
	
	-- Cancellazione dati da Protocollo
	-- PRINT 'Cancellazione dati da Protocollo esterni al range ' + CONVERT(varchar, @StartDate, 103) + ' - ' + CONVERT(varchar, @EndDate, 103)

	-- PRINT 'Pulizia dei Log'
	Truncate Table [ProtocolLog]
	Truncate Table [TableLog]

	-- PRINT 'Cancellazione di AdvancedProtocol'
	DELETE Ap
	FROM [AdvancedProtocol] Ap
	LEFT JOIN [Protocol] P
		   ON Ap.Year = p.Year And Ap.Number = P.Number
	where p.RegistrationDate < @StartDate or p.RegistrationDate > @EndDate or p.Year is null

	-- PRINT 'Offuscamento dati sensibili AdvancedProtocol'
	-- PRINT '--> Subject'
	UPDATE [AdvancedProtocol] SET [Subject] = dbo.Character_Scramble([Subject]) FROM [AdvancedProtocol]
	-- PRINT '--> Note'
	UPDATE [AdvancedProtocol] SET [Note] = dbo.Character_Scramble([Note]) FROM [AdvancedProtocol]

	-- PRINT 'Cancellazione di ProtocolImport'
	DELETE PIm
	FROM [ProtocolImport] PIm
	LEFT JOIN [Protocol] P
		   ON PIm.Year = p.Year And PIm.Number = P.Number
	where p.RegistrationDate < @StartDate or p.RegistrationDate > @EndDate or p.Year is null

	-- PRINT 'Offuscamento dati sensibili ProtocolImport'
	-- PRINT '--> Note'
	UPDATE [ProtocolImport] SET [Note] = dbo.Character_Scramble([Note]) FROM [ProtocolImport]

	-- PRINT 'Cancellazione di ProtocolRole'
	DELETE Pr
	FROM [ProtocolRole] Pr
	LEFT JOIN [Protocol] P
		   ON Pr.Year = p.Year And Pr.Number = P.Number
	where p.RegistrationDate < @StartDate or p.RegistrationDate > @EndDate or p.Year is null

	-- PRINT 'Cancellazione di ProtocolLinks'
	DELETE Pl
	FROM [ProtocolLinks] Pl
	LEFT JOIN [Protocol] P
		   ON Pl.Year = p.Year And Pl.Number = P.Number
	where p.RegistrationDate < @StartDate or p.RegistrationDate > @EndDate or p.Year is null

	-- PRINT 'Cancellazione di ProtocolContact'
	DELETE Pc
	FROM [ProtocolContact] Pc
	LEFT JOIN [Protocol] P
		   ON Pc.Year = p.Year And Pc.Number = P.Number
	where p.RegistrationDate < @StartDate or p.RegistrationDate > @EndDate or p.Year is null

	-- PRINT 'Cancellazione di Contact'
	DELETE C
	FROM [Contact] C
	LEFT JOIN [ProtocolContact] Pc
		   ON C.Incremental = Pc.IDContact
	where Pc.Year is null

	-- PRINT 'Offuscamento dati sensibili Contact'
	-- PRINT '--> Description'
	UPDATE [Contact] SET [Description] = dbo.Character_Scramble([Description]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> FiscalCode'
	UPDATE [Contact] SET [FiscalCode]= dbo.Character_Scramble([FiscalCode]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> Address'
	UPDATE [Contact] SET [Address]= dbo.Character_Scramble([Address]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> CivicNumber'
	UPDATE [Contact] SET [CivicNumber]= dbo.Character_Scramble([CivicNumber]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> ZipCode'
	UPDATE [Contact] SET [ZipCode]= dbo.Character_Scramble([ZipCode]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> City'
	UPDATE [Contact] SET [City]= dbo.Character_Scramble([City]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> CityCode'
	UPDATE [Contact] SET [CityCode]= dbo.Character_Scramble([CityCode]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> TelephoneNumber'
	UPDATE [Contact] SET [TelephoneNumber]= dbo.Character_Scramble([TelephoneNumber]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> FaxNumber'
	UPDATE [Contact] SET [FaxNumber]= dbo.Character_Scramble([FaxNumber]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> EMailAddress'
	UPDATE [Contact] SET [EMailAddress]= dbo.Character_Scramble([EMailAddress]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'
	-- PRINT '--> CertifydMail'
	UPDATE [Contact] SET [CertifydMail]= dbo.Character_Scramble([CertifydMail]) FROM [Contact] WHERE FullIncrementalPath NOT LIKE @EnavContactNodeId + '%'

	-- PRINT 'Cancellazione di ProtocolContactManual'
	DELETE Pcm
	FROM [ProtocolContactManual] Pcm
	LEFT JOIN [Protocol] P
		   ON Pcm.Year = p.Year And Pcm.Number = P.Number
	where p.RegistrationDate < @StartDate or p.RegistrationDate > @EndDate or p.Year is null

	-- PRINT 'Offuscamento dati sensibili ProtocolContactManual'
	-- PRINT '--> Description'
	UPDATE [ProtocolContactManual] SET [Description] = dbo.Character_Scramble([Description]) FROM [ProtocolContactManual]
	-- PRINT '--> FiscalCode'
	UPDATE [ProtocolContactManual] SET [FiscalCode]= dbo.Character_Scramble([FiscalCode]) FROM [ProtocolContactManual]
	-- PRINT '--> Address'
	UPDATE [ProtocolContactManual] SET [Address]= dbo.Character_Scramble([Address]) FROM [ProtocolContactManual]
	-- PRINT '--> CivicNumber'
	UPDATE [ProtocolContactManual] SET [CivicNumber]= dbo.Character_Scramble([CivicNumber]) FROM [ProtocolContactManual]
	-- PRINT '--> ZipCode'
	UPDATE [ProtocolContactManual] SET [ZipCode]= dbo.Character_Scramble([ZipCode]) FROM [ProtocolContactManual]
	-- PRINT '--> City'
	UPDATE [ProtocolContactManual] SET [City]= dbo.Character_Scramble([City]) FROM [ProtocolContactManual]
	-- PRINT '--> CityCode'
	UPDATE [ProtocolContactManual] SET [CityCode]= dbo.Character_Scramble([CityCode]) FROM [ProtocolContactManual]
	-- PRINT '--> TelephoneNumber'
	UPDATE [ProtocolContactManual] SET [TelephoneNumber]= dbo.Character_Scramble([TelephoneNumber]) FROM [ProtocolContactManual]
	-- PRINT '--> FaxNumber'
	UPDATE [ProtocolContactManual] SET [FaxNumber]= dbo.Character_Scramble([FaxNumber]) FROM [ProtocolContactManual]
	-- PRINT '--> EMailAddress'
	UPDATE [ProtocolContactManual] SET [EMailAddress]= dbo.Character_Scramble([EMailAddress]) FROM [ProtocolContactManual]
	-- PRINT '--> CertifydMail'
	UPDATE [ProtocolContactManual] SET [CertifydMail]= dbo.Character_Scramble([CertifydMail]) FROM [ProtocolContactManual]

	-- PRINT 'Cancellazione effettiva dei Protocolli'
	delete from [Protocol]
	where RegistrationDate < @StartDate or RegistrationDate > @EndDate

	-- PRINT 'Offuscamento dati sensibili Protocollo'
	-- PRINT '--> Object'
	UPDATE [Protocol] SET [Object] = dbo.Character_Scramble([Object]) FROM [Protocol]
	-- PRINT '--> ObjectChangeReason'
	UPDATE [Protocol] SET [ObjectChangeReason] = dbo.Character_Scramble([ObjectChangeReason]) FROM [Protocol]
	-- PRINT '--> LastChangedReason'
	UPDATE [Protocol] SET [LastChangedReason] = dbo.Character_Scramble([LastChangedReason]) FROM [Protocol]

	-- PRINT 'Aggiornamento Documenti'
	-- PRINT '--> DocumentCode'
	UPDATE [Protocol] SET [DocumentCode] = @DefaultDocumentName
	-- PRINT '--> idDocument'
	UPDATE [Protocol] SET [idDocument] = @DefaultBiblosId
	-- PRINT '--> idAttachments'
	UPDATE [Protocol] SET [idAttachments] = @DefaultBiblosId
	-- PRINT '--> idAnnexed'
	UPDATE [Protocol] SET [idAnnexed] = @DefaultBiblosGuid

	-- PRINT 'Cancellazione Messaggi'
	truncate table [Message]
	truncate table [MessageAttachment]
	truncate table [MessageContact]
	truncate table [MessageContactEmail]
	truncate table [MessageEmail]
	truncate table [MessageLog]

	-- PRINT 'Cancellazione PECMailBoxLog'
	truncate table [PECMailBoxLog]

	-- PRINT 'Cancellazione PECMailLog'
	truncate table [PECMailLog]

	-- PRINT 'Cancellazione PECMailContent'
	DELETE Pmc
	FROM [PECMailContent] Pmc
	LEFT JOIN [PECMail] Pm
		   ON Pmc.IDPECMail = Pm.IDPECMail
	where Pm.RegistrationDate < @StartDate or Pm.RegistrationDate > @EndDate or Pm.IDPECMail is null

	-- PRINT 'Cancellazione PECMailReceipts'
	DELETE Pmr
	FROM [PECMailReceipt] Pmr
	LEFT JOIN [PECMail] Pm
		   ON Pmr.IDPECMail = Pm.IDPECMail
	where Pm.RegistrationDate < @StartDate or Pm.RegistrationDate > @EndDate or Pm.IDPECMail is null

	-- PRINT 'Offuscamento dati sensibili di PECMailReceipts'
	-- PRINT '--> Subject'
	UPDATE [PECMailReceipt] SET [Subject] = dbo.Character_Scramble([Subject]) FROM [PECMailReceipt]

	-- PRINT 'Cancellazione PECMailAttachments'
	DELETE Pma
	FROM [PECMailAttachment] Pma
	LEFT JOIN [PECMail] Pm
		   ON Pma.IDPECMail = Pm.IDPECMail
	where Pm.RegistrationDate < @StartDate or Pm.RegistrationDate > @EndDate or Pm.IDPECMail is null

	-- PRINT 'Cancellazione Stream'
	ALTER TABLE dbo.PECMailAttachment DROP COLUMN AttachmentStream
	ALTER TABLE dbo.PECMailAttachment SET (LOCK_ESCALATION = TABLE)

	ALTER TABLE dbo.PECMailAttachment ADD AttachmentStream text NULL
	ALTER TABLE dbo.PECMailAttachment SET (LOCK_ESCALATION = TABLE)

	-- PRINT 'Offuscamento PECMailAttachments'
	-- PRINT '--> DocumentName'
	UPDATE [PECMailAttachment] SET [AttachmentName] = dbo.Character_Scramble([AttachmentName]) FROM [PECMailAttachment]
	-- PRINT '--> IdDocument'
	UPDATE [PECMailAttachment] SET [IDDocument] = @DefaultBiblosGuid

	-- PRINT 'Cancellazione PECMail'
	delete from [PECMail]
	where RegistrationDate < @StartDate or RegistrationDate > @EndDate

	-- PRINT 'Offuscamento dati sensibili di PECMail'
	-- PRINT '--> MailSubject'
	UPDATE [PECMail] SET [MailSubject] = dbo.Character_Scramble([MailSubject]) FROM [PECMail]
	-- PRINT '--> MailSenders'
	UPDATE [PECMail] SET [MailSenders] = dbo.Character_Scramble([MailSenders]) FROM [PECMail]
	-- PRINT '--> MailRecipients'
	UPDATE [PECMail] SET [MailRecipients] = dbo.Character_Scramble([MailRecipients]) FROM [PECMail]
	-- PRINT '--> MailBody'
	UPDATE [PECMail] SET [MailBody] = dbo.Character_Scramble([MailBody]) FROM [PECMail]

	-- PRINT 'Aggiornamento documenti'
	UPDATE [PECMail] SET [IDMailContent] = @DefaultBiblosGuid
	UPDATE [PECMail] SET [IDAttachments] = @DefaultBiblosGuid
	UPDATE [PECMail] SET [IDPostacert] = @DefaultBiblosGuid
	UPDATE [PECMail] SET [IDDaticert] = @DefaultBiblosGuid
	UPDATE [PECMail] SET [IDSegnatura] = @DefaultBiblosGuid
	UPDATE [PECMail] SET [IDSmime] = @DefaultBiblosGuid
	UPDATE [PECMail] SET [IDEnvelope] = @DefaultBiblosGuid

	-- PRINT 'Offuscamento dati sensibili di PECMailBox'
	-- PRINT '--> Username'
	UPDATE [PECMailBox] SET [Username] = dbo.Character_Scramble([Username]) FROM [PECMailBox]
	-- PRINT '--> Password'
	UPDATE [PECMailBox] SET [Password] = dbo.Character_Scramble([Password]) FROM [PECMailBox]

	IF @@TRANCOUNT > 0 COMMIT TRANSACTION;
	-- PRINT 'Fine transazione *Protocollo*'

	EXEC(@dropFunctions)
END

