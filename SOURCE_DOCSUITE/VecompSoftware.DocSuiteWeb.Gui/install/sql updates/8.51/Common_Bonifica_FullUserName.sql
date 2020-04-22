/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario MODIFICARE I VALORI 		*
*   <DOMINIO, nvarchar(256), MASTER_USL> --> Impostare il nome del dominio esattamente come compare nella DSW	*
*	<DEFAULT, nvarchar(256), MASTER_USL\biblosds>	--> Impostare il full name di default da imposatere nei campi NULL.					*
*****************************************************************************************************************************************/
PRINT 'ALTER TABLE'
ALTER TABLE ContactTitle ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE ContactTitle ALTER COLUMN LastChangedUser NVARCHAR(256) NULL
ALTER TABLE RoleUser ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE RoleUser ALTER COLUMN LastChangedUser NVARCHAR(256) NULL
ALTER TABLE SecurityGroups ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE SecurityGroups ALTER COLUMN LastChangedUser NVARCHAR(256) NULL
ALTER TABLE SecurityUsers ALTER COLUMN RegistrationUser NVARCHAR(256) NULL
ALTER TABLE SecurityUsers ALTER COLUMN LastChangedUser NVARCHAR(256) NULL

--#############################################################################
DECLARE @Dominio nvarchar(256)
DECLARE @Default nvarchar(256)
SET @Dominio = <dominio>
SET @Default = <nome utente di default>

PRINT 'TableDocType'

UPDATE TableDocType SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				        LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE TableDocType ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL

--#############################################################################

PRINT 'Contact'

UPDATE Contact SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				   LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'ContactTitle'

UPDATE ContactTitle SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				        LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE ContactTitle ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL

--#############################################################################

PRINT 'Category'

UPDATE Category SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'CategoryGroup'

UPDATE CategoryGroup SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				         LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'Container'

UPDATE Container SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				     LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'ContainerExtension'

UPDATE ContainerExtension SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				              LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'ContainerGroup'

UPDATE ContainerGroup SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				          LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'ContainerProperties'

UPDATE ContainerProperties SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				               LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'Role'

UPDATE Role SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'RoleGroup'

UPDATE RoleGroup SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				     LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

--#############################################################################

PRINT 'RoleUser'

UPDATE RoleUser SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
				    LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE RoleUser ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL

--#############################################################################

PRINT 'SecurityGroups'

UPDATE SecurityGroups SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
						  LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE SecurityGroups ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL

--#############################################################################

PRINT 'SecurityUsers'

UPDATE SecurityUsers SET RegistrationUser = dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default),
						 LastChangedUser = dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)
	WHERE RegistrationUser <> dbo.changeToFullUserName(RegistrationUser, @Dominio, @Default) OR RegistrationUser IS NULL
	   OR LastChangedUser <> dbo.changeToFullUserName(LastChangedUser, @Dominio, NULL)

ALTER TABLE SecurityUsers ALTER COLUMN RegistrationUser NVARCHAR(256) NOT NULL

--#############################################################################