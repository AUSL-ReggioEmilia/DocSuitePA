/***************************************************************************************************************************************************
*	Prima di lanciare lo script è necessario mettere un UTENTE DI DEFAULT per la colonna registrationUser della tabella Container (CTRL+SHIFT+M).  *
*	<UTENTE_DEFAULT, nvarchar(256),>	--> Settare il nome dell'utente.																		   *
****************************************************************************************************************************************************/
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
PRINT 'Versionamento database alla 8.67'
GO

EXEC dbo.VersioningDatabase N'8.67'
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
PRINT N'Aggiunta colonne per inserimento automatico gruppi in Container ';
GO

Alter TABLE dbo.Container ADD
     [AutomaticSecurityGroups] [tinyint] NULL,
	 [PrefixSecurityGroupName] [nvarchar](30) NULL,
	 [TenantId] [uniqueidentifier] NULL,
	 [ContainerType] [int] NULL,
	 [SecurityUserAccount] [NVARCHAR](256) NULL,
	 [SecurityUserDisplayName] [nvarchar](256) NULL
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
PRINT N'Modificata colonna [RegistrationUser] in [dbo].[Container]';
GO

ALTER TABLE [dbo].[Container]  ALTER COLUMN [RegistrationUser] NVARCHAR (256) NULL
GO

UPDATE [dbo].[Container] SET [RegistrationUser] = '<UTENTE_DEFAULT, nvarchar(256),>' WHERE RegistrationUser IS NULL
GO

ALTER TABLE [dbo].[Container]  ALTER COLUMN [RegistrationUser] NVARCHAR (256) NOT NULL
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