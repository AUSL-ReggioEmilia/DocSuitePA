/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<NOME_DB_PROTOCOLLO, varchar(256), Protocollo>	--> Settare il nome del DB.															*
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
PRINT 'Versionamento database alla 8.63'
GO

EXEC dbo.VersioningDatabase N'8.63'
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [UniqueId] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[ParameterEnv] ADD CONSTRAINT [DF_ParameterEnv_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
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
PRINT 'Aggiunta colonna [Timestamp] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [Timestamp] TIMESTAMP NOT NULL
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

ALTER TABLE [dbo].[ParameterEnv] ADD [RegistrationDate] [datetimeoffset](7) NULL;
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

ALTER TABLE [dbo].[ParameterEnv] ADD [RegistrationUser] [nvarchar](256) NULL;
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

PRINT N'Creazione della colonna [LastChangedDate] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [LastChangedDate] [datetimeoffset](7) NULL;
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

PRINT N'Creazione della colonna [LastChangedUser] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [LastChangedUser] [nvarchar](256) NULL;
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
PRINT N'Aggiunta della colonna [UniqueId] nella tabella [RoleUser]';
GO

ALTER TABLE [dbo].[RoleUser]
ADD [UniqueId] uniqueidentifier NULL;
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
PRINT N'Copia dei valori [UniqueId] nella tabella [RoleUser] dalla tabella [RoleUser] in protocollo';
GO

UPDATE [dbo].[RoleUser]
SET [dbo].[RoleUser].[UniqueId] = (
SELECT TOP 1 [UniqueId]
FROM <NOME_DB_PROTOCOLLO, varchar(256), Protocollo>.[dbo].[RoleUser] AS PROT
WHERE  [dbo].[RoleUser].[Incremental] = PROT.[Incremental]
and [dbo].[RoleUser].[IdRole] = PROT.[IdRole]
);
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
PRINT N'Drop index [XAKName] dalla tabella Container';
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Container]') AND name = N'XAKName')
DROP INDEX [XAKName] ON [dbo].[Container] WITH ( ONLINE = OFF )
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
PRINT N'Modificata colonna [Name] dalla tabella [dbo].[Container] ';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [Name] nvarchar(256) not null
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
PRINT N'Creazione NON CLUSTERED index [XAKName] dalla tabella Container';
GO

CREATE UNIQUE NONCLUSTERED INDEX [XAKName] ON [dbo].[Container]
(
	[idContainer] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
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
PRINT N'Creo nuovo indice [IX_Collaboration_IdParent_isActive_StartDate_EndDate] alla tabella [Category]';
GO

CREATE NONCLUSTERED INDEX [IX_Collaboration_IdParent_isActive_StartDate_EndDate] ON [dbo].[Category] ([idParent],[isActive],[StartDate],[EndDate])
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