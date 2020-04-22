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
-- Description:	Ribaltamento dati da Produzione per Validazione - Protocollo - Start
-- =============================================
CREATE PROCEDURE GenerateValidationData_Start
	-- Dichiarazione variabili
	@DefaultLocationId varchar(MAX) = '5000',
	@DefaultLocationNameDocm varchar(MAX) = 'Pratiche DEMO',
	@DefaultLocationNameProt varchar(MAX) = 'Protocollo DEMO',
	@DefaultLocationDocumentServer varchar(MAX) = 'BIBLOSSERVER',
	@DefaultLocationDocmBiblosDSDB varchar(MAX) = 'BIBLOSARCHIVE',
	@DefaultLocationProtBiblosDSDB varchar(MAX) = 'BIBLOSARCHIVE',
	@DefaultLocationReslBiblosDSDB varchar(MAX) = 'BIBLOSARCHIVE',
	@ValidationDomainName varchar(MAX) = 'LAB***REMOVED***',
	@AdminUserAccount varchar(MAX) = 'gcolasanti.vecomp',
	@AdminUserDescription varchar(MAX) = 'Giordano Colasanti',
	@AdminGroupName varchar(MAX) = 'admin'
AS
BEGIN
	BEGIN TRANSACTION TransactionWithGos;
	-- PRINT 'Inizio transazione Protocollo - Parte Condivisa'

	-- Inserimento Location di Test
	-- PRINT 'Inserimento Location di Test'
	SET NOCOUNT ON;
	-- Update the row if it exists.    
	UPDATE [Location]
	SET idLocation = @DefaultLocationId
	WHERE idLocation = @DefaultLocationId
	-- Insert the row if the UPDATE statement failed.	
	IF (@@ROWCOUNT = 0 )
	BEGIN
		INSERT INTO [Location]([idLocation],[Name],[DocumentServer],[DocmBiblosDSDB],[ProtBiblosDSDB],[ReslBiblosDSDB],[DSNString])
		VALUES (@DefaultLocationId,@DefaultLocationNameProt,@DefaultLocationDocumentServer,@DefaultLocationDocmBiblosDSDB,@DefaultLocationProtBiblosDSDB,@DefaultLocationReslBiblosDSDB,'')
	END
	SET NOCOUNT OFF;
	                      
	-- Aggiornamento ParameterEnv
	-- PRINT 'Aggiornamento ParameterEnv'
	UPDATE [ParameterEnv] SET [KeyValue] = @ValidationDomainName where [KeyName] = 'Domain'
	UPDATE [ParameterEnv] SET [KeyValue] = [KeyValue] + ' Ambiente di validazione' where [KeyName] = 'CorporateName' and [KeyValue] not like '%Ambiente di validazione'

	-- Aggiornamento Role
	-- PRINT 'Aggiornamento Role'
	UPDATE [Role] SET [DocmLocation] = @DefaultLocationId where [DocmLocation] is not null
	UPDATE [Role] SET [ProtLocation] = @DefaultLocationId where [ProtLocation] is not null
	UPDATE [Role] SET [ReslLocation] = @DefaultLocationId where [ReslLocation] is not null

	-- Aggiornamento Container
	-- PRINT 'Aggiornamento Container'
	UPDATE [Container] SET [DocmLocation] = @DefaultLocationId where [DocmLocation] is not null
	UPDATE [Container] SET [ProtLocation] = @DefaultLocationId where [ProtLocation] is not null
	UPDATE [Container] SET [ReslLocation] = @DefaultLocationId where [ReslLocation] is not null

	-- Aggiornamento Location di Protocollo
	-- PRINT 'Aggiornamento Location di Protocollo'
	UPDATE [Protocol] SET [idLocation] = @DefaultLocationId

	-- PRINT 'Aggiornamento Location PEC'
	UPDATE [PECMail] SET [IDLocation] = @DefaultLocationId

	-- PRINT 'Aggiornamento Location PECMailBox'
	UPDATE [PECMailBox] SET [IDLocation] = @DefaultLocationId

	-- PRINT 'Pulizia Location'
	Delete from [Location] where idLocation <> @DefaultLocationId

	-- Aggiornamento Utenze
	-- PRINT 'Aggiornamento Utenze'
	Truncate Table [SecurityUsers]

	-- PRINT 'Creazione Gruppo Amministratori'
	INSERT INTO [SecurityGroups] ([idGroup],[GroupName],[FullIncrementalPath],[RegistrationUser],[idGroupFather],[LogDescription],[RegistrationDate],[LastChangedDate],[LastChangedUser])
	VALUES (1,'admin-docsuite',1,'Admin',NULL,NULL,CURRENT_TIMESTAMP,CURRENT_TIMESTAMP,'Admin')

	-- PRINT 'Creazione Utente Amministratore'
	INSERT INTO [SecurityUsers] ([idUser],[Account],[Description],[idGroup],[RegistrationDate],[LastChangedUser],[LastChangedDate],[RegistrationUser])
	VALUES (1,@AdminUserAccount,@AdminUserDescription,1,CURRENT_TIMESTAMP,'Admin',CURRENT_TIMESTAMP,'Admin')

	IF @@TRANCOUNT > 0 COMMIT TRANSACTION;
	-- PRINT 'Fine transazione'

END
GO
