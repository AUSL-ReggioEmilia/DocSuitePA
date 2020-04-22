/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario MODIFICARE I VALORI 		*
*   <DOMINIO, nvarchar(256), MASTER_USL> --> Impostare il nome del dominio esattamente come compare nella DSW	*
*	<DEFAULT, nvarchar(256), MASTER_USL\biblosds>	--> Impostare il full name di default da imposatere nei campi NULL.					*
*****************************************************************************************************************************************/

--#############################################################################

-- @ColValue è il nome della colonna da aggiornare (RegistrationUser, LastChangedUser, SystemUser o Account)
-- @Dominio è la stringa di dominio (senza slash)
-- @Default è la stringa di default con cui sostituire gli eventuali valori NULL (nelle colonne RegistrationUser, SystemUser, Account)
--     NOTA: se il valore nella colonna LastChangedUser è NULL, rimane NULL

CREATE FUNCTION changeToFullUserName (@ColValue nvarchar(256), @Domain nvarchar(256), @DefaultUserName nvarchar(256))
RETURNS nvarchar(256)
AS
BEGIN
	RETURN CASE
		WHEN @ColValue IS NULL OR @ColValue = ''  THEN @DefaultUserName
		WHEN @ColValue LIKE '%\%'				  THEN @Domain+RIGHT(@ColValue, CHARINDEX('\', REVERSE(@ColValue)))
		ELSE @Domain+'\'+@ColValue
	END
END
GO
--#############################################################################
PRINT 'ALTER TABLE'

ALTER TABLE PECOC ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE PECOCLog ALTER COLUMN SystemUser NVARCHAR(256) NOT NULL
ALTER TABLE ResolutionContact ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE ResolutionDocumentSeriesItem ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE Resolution ALTER COLUMN PublishingUser NVARCHAR(256) NULL
ALTER TABLE Resolution ALTER COLUMN WarningUser NVARCHAR(256) NULL
ALTER TABLE Resolution ALTER COLUMN ProposeUser NVARCHAR(256) NULL
ALTER TABLE Resolution ALTER COLUMN WaitUser NVARCHAR(256) NULL
ALTER TABLE Resolution ALTER COLUMN ResponseUser NVARCHAR(256) NULL
ALTER TABLE Resolution ALTER COLUMN ProposerWarningUser NVARCHAR(256) NULL
ALTER TABLE Resolution ALTER COLUMN SupervisoryBoardWarningUser NVARCHAR(256) NULL
ALTER TABLE Resolution ALTER COLUMN ManagementWarningUser NVARCHAR(256) NULL
ALTER TABLE Resolution ALTER COLUMN LeaveUser NVARCHAR(256) NULL
ALTER TABLE ResolutionJournal ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
ALTER TABLE ResolutionJournalTemplate ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE ResolutionKinds ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
DROP INDEX [IX_ResolutionODG_DestinationUser_idDocFDQ] ON [dbo].[ResolutionODG]
ALTER TABLE ResolutionODG ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE ResolutionODG ALTER COLUMN CheckUser NVARCHAR(256) NULL
ALTER TABLE ResolutionODG ALTER COLUMN LastChangedUser NVARCHAR(256) NULL
ALTER TABLE ResolutionODG ALTER COLUMN PublicationUser NVARCHAR(256) NULL
ALTER TABLE ResolutionODG ALTER COLUMN LastPrintedUser NVARCHAR(256) NULL
ALTER TABLE ResolutionODG ALTER COLUMN LastFDQUser NVARCHAR(256) NULL
ALTER TABLE ResolutionLog ALTER COLUMN SystemUser NVARCHAR(256) NOT NULL
ALTER TABLE ResolutionODGDetails ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE ResolutionODGDetails ALTER COLUMN CheckUser NVARCHAR(256) NULL
ALTER TABLE ResolutionODGTaskDetails ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE ResolutionRole ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE ResolutionRole ALTER COLUMN LastChangedUser NVARCHAR(256) NULL
ALTER TABLE ResolutionWorkflow ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE ResolutionWorkflow ALTER COLUMN LastChangedUser NVARCHAR(256) NULL
ALTER TABLE WebPublication ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE WebPublication ALTER COLUMN LastChangedUser NVARCHAR(256) NULL

--#############################################################################
DECLARE @Dominio nvarchar(256)
DECLARE @Default nvarchar(256)
SET @Dominio = <dominio>
SET @Default = <nome utente di default>

PRINT 'PECOC'

UPDATE PECOC SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'PECOCLog'

UPDATE PECOCLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'Resolution'

UPDATE Resolution SET LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL), 
					  AdoptionUser = dbo.changeToFullUserName(AdoptionUser, @Dominio, NULL), 
					  PublishingUser = dbo.changeToFullUserName(PublishingUser, @Dominio, NULL),
					  EffectivenessUser = dbo.changeToFullUserName(EffectivenessUser, @Dominio, NULL),
					  ConfirmUser = dbo.changeToFullUserName(ConfirmUser, @Dominio, NULL),
					  WarningUser = dbo.changeToFullUserName(WarningUser, @Dominio, NULL),
					  ProposeUser = dbo.changeToFullUserName(ProposeUser, @Dominio, NULL),
					  WaitUser = dbo.changeToFullUserName(WaitUser, @Dominio, NULL),
					  ResponseUser = dbo.changeToFullUserName(ResponseUser, @Dominio, NULL),
					  ProposerWarningUser = dbo.changeToFullUserName(ProposerWarningUser, @Dominio, NULL),
					  SupervisoryBoardWarningUser = dbo.changeToFullUserName(SupervisoryBoardWarningUser, @Dominio, NULL),
					  ManagementWarningUser = dbo.changeToFullUserName(ManagementWarningUser, @Dominio, NULL),
					  LeaveUser = dbo.changeToFullUserName(LeaveUser, @Dominio, NULL)

	WHERE LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL) OR
		  AdoptionUser <> dbo.changeToFullUserName(AdoptionUser, @Dominio, NULL) OR
		  PublishingUser <> dbo.changeToFullUserName(PublishingUser, @Dominio, NULL) OR
		  EffectivenessUser <> dbo.changeToFullUserName(EffectivenessUser, @Dominio, NULL) OR
		  ConfirmUser <> dbo.changeToFullUserName(ConfirmUser, @Dominio, NULL) OR
		  WarningUser <> dbo.changeToFullUserName(WarningUser, @Dominio, NULL) OR
		  ProposeUser <> dbo.changeToFullUserName(ProposeUser, @Dominio, NULL) OR
		  WaitUser <> dbo.changeToFullUserName(WaitUser, @Dominio, NULL) OR
		  ResponseUser <> dbo.changeToFullUserName(ResponseUser, @Dominio, NULL) OR
		  ProposerWarningUser <> dbo.changeToFullUserName(ProposerWarningUser, @Dominio, NULL) OR
		  SupervisoryBoardWarningUser <> dbo.changeToFullUserName(SupervisoryBoardWarningUser, @Dominio, NULL) OR
		  ManagementWarningUser <> dbo.changeToFullUserName(ManagementWarningUser, @Dominio, NULL) OR
		  LeaveUser <> dbo.changeToFullUserName(LeaveUser, @Dominio, NULL);

--#############################################################################

PRINT 'ResolutionContact'

UPDATE ResolutionContact SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default)

--#############################################################################

PRINT 'ResolutionDocumentSeriesItem'

UPDATE ResolutionDocumentSeriesItem SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE ResolutionDocumentSeriesItem ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
--#############################################################################

PRINT 'ResolutionJournal'

UPDATE ResolutionJournal SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'ResolutionJournalTemplate'

UPDATE ResolutionJournalTemplate SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE ResolutionJournalTemplate ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
--#############################################################################

PRINT 'ResolutionKinds'

UPDATE ResolutionKinds SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE ResolutionKinds ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
--#############################################################################

PRINT 'ResolutionLog'

UPDATE ResolutionLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'ResolutionODG'

UPDATE ResolutionODG SET 
					RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL),					
					DestinationUser = dbo.changeToFullUserName(DestinationUser, @Dominio, NULL),
					PublicationUser = dbo.changeToFullUserName(PublicationUser, @Dominio, NULL),
					LastPrintedUser = dbo.changeToFullUserName(LastPrintedUser, @Dominio, NULL),
					LastFDQUser = dbo.changeToFullUserName(LastFDQUser, @Dominio, NULL),
					CheckUser = dbo.changeToFullUserName(CheckUser, @Dominio, NULL)

	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	   OR DestinationUser <> dbo.changeToFullUserName(DestinationUser, @Dominio, NULL)
	   OR PublicationUser <> dbo.changeToFullUserName(PublicationUser, @Dominio, NULL)
	   OR LastPrintedUser <> dbo.changeToFullUserName(LastPrintedUser, @Dominio, NULL)
	   OR LastFDQUser <> dbo.changeToFullUserName(LastFDQUser, @Dominio, NULL)
	   OR CheckUser <> dbo.changeToFullUserName(CheckUser, @Dominio, NULL)

ALTER TABLE ResolutionODG ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
CREATE NONCLUSTERED INDEX [IX_ResolutionODG_DestinationUser_idDocFDQ] ON [dbo].[ResolutionODG]
(
	[DestinationUser] ASC,
	[idDocFDQ] ASC
)
INCLUDE ([idODG],
	[idTipologia],
	[dataODG],
	[RegistrationUser],
	[RegistrationDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

--#############################################################################

PRINT 'ResolutionODGDetails'

UPDATE ResolutionODGDetails SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    CheckUser = dbo.changeToFullUserName(CheckUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR CheckUser <> dbo.changeToFullUserName(CheckUser, @Dominio, NULL)

ALTER TABLE ResolutionODGDetails ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
--#############################################################################

PRINT 'ResolutionODGTaskDetails'

UPDATE ResolutionODGTaskDetails SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE ResolutionODGTaskDetails ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL

--#############################################################################

PRINT 'ResolutionRole'

UPDATE ResolutionRole SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE ResolutionRole ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL

--#############################################################################

PRINT 'ResolutionWorkflow'

UPDATE ResolutionWorkflow SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE ResolutionWorkflow ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
--#############################################################################

PRINT 'WebPublication'

UPDATE WebPublication SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE WebPublication ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
--#############################################################################
