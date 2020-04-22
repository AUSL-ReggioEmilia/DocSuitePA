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
-- Description:	Ribaltamento dati da Produzione per Validazione - Pratiche - Data
-- =============================================
CREATE PROCEDURE GenerateValidationData_Data
	-- Dichiarazione variabili
	@StartDate Date = '20130101',
	@EndDate Date = '20131231',
	@DefaultDocumentName varchar(MAX) = 'Documento.pdf',
	@DefaultBiblosId varchar(MAX) = '100',
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
	
	BEGIN TRANSACTION TransactionWithGos;
	-- PRINT 'Inizio transazione *Pratiche*'
	
	-- Cancellazione dati da Pratiche
	-- PRINT 'Cancellazione dati da Pratiche esterni al range ' + CONVERT(varchar, @StartDate, 103) + ' - ' + CONVERT(varchar, @EndDate, 103)

	-- PRINT 'Pulizia dei Log'
	Truncate Table [DocumentLog]

	-- PRINT 'Cancellazione dei contatti'
	DELETE Dc
	FROM [DocumentContact] Dc
	LEFT JOIN [Document] D
		   ON Dc.Year = D.Year And Dc.Number = D.Number
	where D.RegistrationDate <= @StartDate or D.RegistrationDate >= @EndDate or D.Year is null

	-- PRINT 'Cancellazione di Contact'
	DELETE C
	FROM [Contact] C
	LEFT JOIN [DocumentContact] Dc
		   ON C.Incremental = Dc.IDContact
	where Dc.Year is null

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

	-- PRINT 'Cancellazione dei folder'
	DELETE Df
	FROM [DocumentFolder] Df
	LEFT JOIN [Document] D
		   ON Df.Year = D.Year And Df.Number = D.Number
	where D.RegistrationDate <= @StartDate or D.RegistrationDate >= @EndDate or D.Year is null

	-- PRINT 'Offuscamento dati sensibili folder'
	-- PRINT '--> FolderName'
	UPDATE [DocumentFolder] SET [FolderName] = dbo.Character_Scramble([FolderName]) FROM [DocumentFolder]
	-- PRINT '--> Description'
	UPDATE [DocumentFolder] SET [Description] = dbo.Character_Scramble([Description]) FROM [DocumentFolder]

	-- PRINT 'Cancellazione dei documenti'
	DELETE Do
	FROM [DocumentObject] Do
	LEFT JOIN [Document] D
		   ON Do.Year = D.Year And Do.Number = D.Number
	where D.RegistrationDate <= @StartDate or D.RegistrationDate >= @EndDate or D.Year is null

	-- PRINT 'Offuscamento dati sensibili dei documenti'
	-- PRINT '--> Object'
	UPDATE [DocumentObject] SET [Object] = dbo.Character_Scramble([Object]) FROM [DocumentObject]
	-- PRINT '--> Reason'
	UPDATE [DocumentObject] SET [Reason] = dbo.Character_Scramble([Reason]) FROM [DocumentObject]
	-- PRINT '--> Note'
	UPDATE [DocumentObject] SET [Note] = dbo.Character_Scramble([Note]) FROM [DocumentObject]

	-- PRINT 'Impostazione documento di Default [' + @DefaultDocumentName + '/idBiblos ' + @DefaultBiblosId + ']'
	UPDATE [DocumentObject] SET [Description] = @DefaultDocumentName, [IdBiblos] = @DefaultBiblosId

	-- PRINT 'Cancellazione dei documenti versioning'
	DELETE Dv
	FROM [DocumentVersioning] Dv
	LEFT JOIN [Document] D
		   ON Dv.Year = D.Year And Dv.Number = D.Number
	where D.RegistrationDate <= @StartDate or D.RegistrationDate >= @EndDate or D.Year is null

	-- PRINT 'Cancellazione dei DocumentToken'
	DELETE Dtk
	FROM [DocumentToken] Dtk
	LEFT JOIN [Document] D
		   ON Dtk.Year = D.Year And Dtk.Number = D.Number
	where D.RegistrationDate <= @StartDate or D.RegistrationDate >= @EndDate or D.Year is null

	-- PRINT 'Offuscamento dei DocumentToken'
	-- PRINT '--> Object'
	UPDATE [DocumentToken] SET [Object] = dbo.Character_Scramble([Object]) FROM [DocumentToken]
	-- PRINT '--> Reason'
	UPDATE [DocumentToken] SET [Reason] = dbo.Character_Scramble([Reason]) FROM [DocumentToken]
	-- PRINT '--> Note'
	UPDATE [DocumentToken] SET [Note] = dbo.Character_Scramble([Note]) FROM [DocumentToken]
	-- PRINT '--> ReasonResponse'
	UPDATE [DocumentToken] SET [ReasonResponse] = dbo.Character_Scramble([ReasonResponse]) FROM [DocumentToken]

	-- PRINT 'Cancellazione dei DocumentTokenUser'
	DELETE Dtku
	FROM [DocumentTokenUser] Dtku
	LEFT JOIN [Document] D
		   ON Dtku.Year = D.Year And Dtku.Number = D.Number
	where D.RegistrationDate <= @StartDate or D.RegistrationDate >= @EndDate or D.Year is null

	-- PRINT 'Cancellazione effettiva delle Pratiche'
	delete from [Document]
	where RegistrationDate <= @StartDate or RegistrationDate >= @EndDate

	-- PRINT 'Offuscamento dati sensibili Pratiche'
	-- PRINT '--> Name'
	UPDATE [Document] SET [Name] = dbo.Character_Scramble([Name]) FROM [Document]
	-- PRINT '--> Object'
	UPDATE [Document] SET [Object] = dbo.Character_Scramble([Object]) FROM [Document]
	-- PRINT '--> Manager'
	UPDATE [Document] SET [Manager] = dbo.Character_Scramble([Manager]) FROM [Document]
	-- PRINT '--> Note'
	UPDATE [Document] SET [Note] = dbo.Character_Scramble([Note]) FROM [Document]

	EXEC(@dropFunctions)

	IF @@TRANCOUNT > 0 COMMIT TRANSACTION;
	-- PRINT 'Fine transazione'

END

