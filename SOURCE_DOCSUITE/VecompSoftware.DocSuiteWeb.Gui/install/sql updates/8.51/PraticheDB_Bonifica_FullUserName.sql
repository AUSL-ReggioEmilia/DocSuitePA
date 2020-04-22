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
ALTER TABLE DocumentTokenUser ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE DocumentTokenUser ALTER COLUMN LastChangedUser NVARCHAR(256) NULL


--#############################################################################
DECLARE @Dominio nvarchar(256)
DECLARE @Default nvarchar(256)
SET @Dominio = <dominio>
SET @Default = <nome utente di default>

PRINT 'Document'

UPDATE Document SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)


--#############################################################################

PRINT 'DocumentFolder'

UPDATE DocumentFolder SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				          LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'DocumentLog'

UPDATE DocumentLog SET SystemUser = dbo.changeToFullUserName(SystemUser, @Dominio, @Default)
	WHERE SystemUser <> dbo.changeToFullUserName(SystemUser, @Dominio, @Default) OR SystemUser IS NULL

--#############################################################################

PRINT 'DocumentObject'

UPDATE DocumentObject SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				          LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'DocumentToken'

UPDATE DocumentToken SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
					     LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'DocumentTokenUser'

UPDATE DocumentTokenUser SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
							 LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE DocumentTokenUser ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL

--#############################################################################

PRINT 'DocumentVersioning'

UPDATE DocumentVersioning SET CheckOutUser = dbo.changeToFullUserName(CheckOutUser, @Dominio, NULL),
							  CheckInUser = dbo.changeToFullUserName(CheckInUser, @Dominio, NULL)
	WHERE CheckOutUser <> dbo.changeToFullUserName(CheckOutUser, @Dominio, NULL) 
	   OR CheckInUser <> dbo.changeToFullUserName(CheckInUser, @Dominio, NULL)

--#############################################################################
