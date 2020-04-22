/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<UTENTE_DEFAULT, varchar(256),>	--> Settare il nome dell'utente.															*
*****************************************************************************************************************************************/
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ParameterEnv]';
GO

UPDATE [dbo].[ParameterEnv] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL
GO

ALTER TABLE [dbo].[ParameterEnv] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT N'Creazione della colonna [RegistrationDate] nella tabella [ParameterEnv]';
GO

UPDATE [dbo].[ParameterEnv] SET [RegistrationDate] = GETUTCDATE()
GO

ALTER TABLE [dbo].[ParameterEnv] ALTER COLUMN [RegistrationDate] [datetimeoffset](7) NOT NULL;
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

PRINT N'Creazione della colonna [RegistrationUser] nella tabella [ParameterEnv]';
GO

UPDATE [dbo].[ParameterEnv] SET [RegistrationUser] = '<UTENTE_DEFAULT, varchar(256),>'
GO

ALTER TABLE [dbo].[ParameterEnv] ALTER COLUMN [RegistrationUser] [nvarchar](256) NOT NULL;
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
PRINT N'La colonna [UniqueId] nella tabella [RoleUser] viene resa NOT NULL';
GO

ALTER TABLE [dbo].[RoleUser]
ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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