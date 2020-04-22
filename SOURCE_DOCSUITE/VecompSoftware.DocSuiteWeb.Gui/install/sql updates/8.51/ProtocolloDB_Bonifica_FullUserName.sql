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
DROP INDEX [XAKProtocolLog] ON [dbo].[ProtocolLog]

ALTER TABLE ProtocolLog ALTER COLUMN SystemUser NVARCHAR(256) NOT NULL
ALTER TABLE [Message] ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL 
ALTER TABLE [Message] ALTER COLUMN LastChangedUser NVARCHAR(256) NULL 
ALTER TABLE MessageContactEmail ALTER COLUMN [User] NVARCHAR(256) NOT NULL 
ALTER TABLE MessageLog ALTER COLUMN SystemUser NVARCHAR(256) NOT NULL 
ALTER TABLE PecMail ALTER COLUMN RegistrationUser NVARCHAR(256) NULL 
ALTER TABLE PecMail ALTER COLUMN LastChangedUser NVARCHAR(256) NULL 
ALTER TABLE PECMailLog ALTER COLUMN SystemUser NVARCHAR(256) NOT NULL 
ALTER TABLE PECMailBoxLog ALTER COLUMN SystemUser NVARCHAR(256) NOT NULL 
ALTER TABLE DocumentSeries ALTER COLUMN RegistrationUser NVARCHAR(256) NULL 
ALTER TABLE DocumentSeries ALTER COLUMN LastChangedUser NVARCHAR(256) NULL 
ALTER TABLE DocumentSeriesItem ALTER COLUMN RegistrationUser NVARCHAR(256) NULL 
ALTER TABLE DocumentSeriesItem ALTER COLUMN LastChangedUser NVARCHAR(256) NULL 
ALTER TABLE DocumentSeriesItemLog ALTER COLUMN SystemUser NVARCHAR(256) NOT NULL 
ALTER TABLE CollaborationVersioning ALTER COLUMN CheckOutUser NVARCHAR(256) NULL 
ALTER TABLE OChart ALTER COLUMN RegistrationUser NVARCHAR(256) NULL 
ALTER TABLE OChart ALTER COLUMN LastChangedUser NVARCHAR(256) NULL 
ALTER TABLE OChartItem ALTER COLUMN RegistrationUser NVARCHAR(256) NULL 
ALTER TABLE OChartItem ALTER COLUMN LastChangedUser NVARCHAR(256) NULL 
ALTER TABLE UserError ALTER COLUMN SystemUser NVARCHAR(256) NOT NULL 
ALTER TABLE TenderLot ALTER COLUMN RegistrationUser NVARCHAR(256) NULL 
ALTER TABLE TenderLot ALTER COLUMN LastChangedUser NVARCHAR(256) NULL 
ALTER TABLE DeskLogs ALTER COLUMN SystemUser NVARCHAR(256) NOT NULL 

--#############################################################################
DECLARE @Dominio nvarchar(256)
DECLARE @Default nvarchar(256)
SET @Dominio = <dominio>
SET @Default = <nome utente di default>

PRINT 'Protocol'

UPDATE Protocol SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)


--#############################################################################

PRINT 'ProtocolContactIssue'

UPDATE ProtocolContactIssue SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL

--#############################################################################

PRINT 'ProtocolDraft'

UPDATE ProtocolDraft SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				         LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'ProtocolJournalLog'

UPDATE ProtocolJournalLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'ProtocolLinks'

UPDATE ProtocolLinks SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL

--#############################################################################

PRINT 'ProtocolLog'

UPDATE ProtocolLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

CREATE NONCLUSTERED INDEX [XAKProtocolLog] ON [dbo].[ProtocolLog]
(
	[Year] ASC,
	[Number] ASC,
	[LogType] ASC,
	[SystemUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

--#############################################################################

PRINT 'ProtocolRole'

UPDATE ProtocolRole SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL

--#############################################################################

PRINT 'ProtocolRoleUser'

UPDATE ProtocolRoleUser SET Account = dbo.changeToFullUserName(Account, @Dominio, @Default)
	WHERE Account <> dbo.changeToFullUserName(Account, @Dominio, @Default) OR Account IS NULL

--#############################################################################

PRINT 'ProtocolTransfert'

UPDATE ProtocolTransfert SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				             LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'Message'

UPDATE Message SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				   LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'MessageContactEmail'

UPDATE MessageContactEmail SET [User] = dbo.changeToFullUserName([User], @Dominio, @Default)
	WHERE [User] <> dbo.changeToFullUserName([User], @Dominio, @Default) OR [User] IS NULL

--#############################################################################

PRINT 'MessageLog'

UPDATE MessageLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'PecMail'

UPDATE PecMail SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				   LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL),
				   Handler = dbo.changeToFullUserName(Handler, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	   OR Handler <> dbo.changeToFullUserName(Handler, @Dominio, NULL)

ALTER TABLE PecMail ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL 

--#############################################################################

PRINT 'PECMailLog'

UPDATE PECMailLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'PECMailBoxLog'

UPDATE PECMailBoxLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'DocumentSeries'

UPDATE DocumentSeries SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				   LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE DocumentSeries ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
 
--#############################################################################

PRINT 'DocumentSeriesItem'

UPDATE DocumentSeriesItem SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				   LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE DocumentSeries ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
 
--#############################################################################

PRINT 'DocumentSeriesItemLinks'

UPDATE DocumentSeriesItemLinks SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				   LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'DocumentSeriesItemLog'

UPDATE DocumentSeriesItemLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'Collaboration'

UPDATE Collaboration SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
						 LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL),
						 PublicationUser = dbo.changeToFullUserName(PublicationUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	   OR PublicationUser <> dbo.changeToFullUserName(PublicationUser, @Dominio, NULL) 

--#############################################################################

PRINT 'CollaborationLog'

UPDATE CollaborationLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'CollaborationSigns'

UPDATE CollaborationSigns SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
							  LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL),
						      SignUser = dbo.changeToFullUserName(SignUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	   OR SignUser <> dbo.changeToFullUserName(SignUser, @Dominio, NULL)

--#############################################################################

PRINT 'CollaborationUsers'

UPDATE CollaborationUsers SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
							  LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'CollaborationVersioning'

UPDATE CollaborationVersioning SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
						      CheckOutUser = dbo.changeToFullUserName(CheckOutUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR CheckOutUser <> dbo.changeToFullUserName(CheckOutUser, @Dominio, NULL)

--#############################################################################

PRINT 'OChart'

UPDATE OChart SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				  LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE OChart ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
 
--#############################################################################

PRINT 'OChartItem'

UPDATE OChartItem SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				      LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE OChartItem ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL
 
--#############################################################################

PRINT 'OChartItemWorkflows'

UPDATE OChartItemWorkflows SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
							   LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'UserError'

UPDATE UserError SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'TenderLot'

UPDATE TenderLot SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				     LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE TenderLot ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL 

--#############################################################################

PRINT 'Desks'

UPDATE Desks SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				 LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'DeskLogs'

UPDATE DeskLogs SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'DeskDocumentVersions'

UPDATE DeskDocumentVersions SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
							    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'DeskDocuments'

UPDATE DeskDocuments SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
					     LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################