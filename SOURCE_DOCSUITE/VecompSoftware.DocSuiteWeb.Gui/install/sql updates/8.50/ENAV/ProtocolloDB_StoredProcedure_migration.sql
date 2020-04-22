SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;
GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
--#############################################################################
PRINT 'Versionamento database alla 8.50'
GO

EXEC dbo.VersioningDatabase N'8.50'
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT 'Copia dei contatti nei database di atti e pratiche...'
GO

-- =============================================
-- Author:		ATosato
-- Create date: 2015-11-30
-- Description:	Copia il contatto nei database di Pratiche e Atti
-- EXEC dbo.CopyContact 48080

-- NB: Lasciare il nome del DB vuoto in caso di assenza
-- =============================================
CREATE PROCEDURE CopyContact @idContact int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @isAttiEnable BIT
	DECLARE @isPraticheEnable BIT
	SET @isAttiEnable = CAST('<EXIST_DB_ATTI, varchar(255), False>' AS BIT)
	SET @isPraticheEnable = CAST('<EXIST_DB_PRATICHE, varchar(255), False>' AS BIT)

    IF @idContact IS NOT NULL
	BEGIN
		IF @isAttiEnable = CAST('True' AS BIT)
		BEGIN
			INSERT INTO <DB_ATTI, varchar(255), Nome db Atti o lasciare vuoto>.dbo.Contact(Incremental, IdContactType, IncrementalFather, Description, BirthDate, Code, SearchCode, FiscalCode, IdPlaceName, Address, CivicNumber, ZipCode, City, CityCode, TelephoneNumber, FaxNumber, EMailAddress,
					CertifydMail, Note, idRole, isActive, isLocked, isNotExpandable, FullIncrementalPath, RegistrationUser, RegistrationDate, LastChangedUser, LastChangedDate, IdTitle, IdRoleRootContact, ActiveFrom, ActiveTo, isChanged, UniqueId)
			SELECT Incremental, IdContactType, IncrementalFather, Description, BirthDate, Code, SearchCode, FiscalCode, IdPlaceName, Address, CivicNumber, ZipCode, City, CityCode, TelephoneNumber, FaxNumber, EMailAddress,
					CertifydMail, Note, idRole, isActive, isLocked, isNotExpandable, FullIncrementalPath, RegistrationUser, RegistrationDate, LastChangedUser, LastChangedDate, IdTitle, IdRoleRootContact, ActiveFrom, ActiveTo, isChanged, UniqueId
			FROM dbo.Contact 
			WHERE incremental = @idContact
		END

		IF @isPraticheEnable = CAST('True' AS BIT)
		BEGIN
			INSERT INTO <DB_PRATICHE, varchar(255), Nome db Pratiche o lasciare vuoto>.dbo.Contact(Incremental, IdContactType, IncrementalFather, Description, BirthDate, Code, SearchCode, FiscalCode, IdPlaceName, Address, CivicNumber, ZipCode, City, CityCode, TelephoneNumber, FaxNumber, EMailAddress,
					CertifydMail, Note, idRole, isActive, isLocked, isNotExpandable, FullIncrementalPath, RegistrationUser, RegistrationDate, LastChangedUser, LastChangedDate, IdTitle, IdRoleRootContact, ActiveFrom, ActiveTo, isChanged, UniqueId)
			SELECT Incremental, IdContactType, IncrementalFather, Description, BirthDate, Code, SearchCode, FiscalCode, IdPlaceName, Address, CivicNumber, ZipCode, City, CityCode, TelephoneNumber, FaxNumber, EMailAddress,
					CertifydMail, Note, idRole, isActive, isLocked, isNotExpandable, FullIncrementalPath, RegistrationUser, RegistrationDate, LastChangedUser, LastChangedDate, IdTitle, IdRoleRootContact, ActiveFrom, ActiveTo, isChanged, UniqueId
			FROM dbo.Contact 
			WHERE incremental = @idContact
		END		
	END	
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
	PRINT N'The transacted portion of the database update succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT N'The transacted portion of the database update FAILED.'
GO
DROP TABLE #tmpErrors
GO