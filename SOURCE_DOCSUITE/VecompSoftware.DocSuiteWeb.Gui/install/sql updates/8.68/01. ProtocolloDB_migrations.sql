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
PRINT 'Versionamento database alla 8.68'
GO

EXEC dbo.VersioningDatabase N'8.68'
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

--#############################################################################
PRINT N'DROP ODG STRUCTURES';
GO

DROP VIEW [dbo].[ResolutionODGTasksEmpty]
GO
DROP VIEW [dbo].[ResolutionODGTaskDetailsEmpty]
GO
DROP VIEW [dbo].[ResolutionODGDetailsEmpty]
GO
DROP VIEW [dbo].[ResolutionODGEmpty]
GO

DROP SYNONYM [dbo].[ResolutionODGTasks]
GO
DROP SYNONYM [dbo].[ResolutionODGTaskDetails]
GO
DROP SYNONYM [dbo].[ResolutionODGDetails]
GO
DROP SYNONYM [dbo].[ResolutionODG]

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
PRINT N'DROP CONTRACT STRUCTURES';
GO

DROP TABLE [dbo].[Contract]
GO
DROP TABLE [dbo].[ContractTable]

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
PRINT N'Aggiunta della colonna Timestamp alla tabella Contact';
GO

ALTER TABLE [dbo].[Contact] ADD [Timestamp] TIMESTAMP NOT NULL
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
PRINT N'Aggiunta della colonna [Status] alla tabella [cqrs].[DocumentUnit]';
GO

ALTER TABLE [cqrs].[DocumentUnits] ADD [Status] smallint NULL
GO

UPDATE [cqrs].[DocumentUnits] SET [Status] = 1
GO

ALTER TABLE [cqrs].[DocumentUnits] ALTER COLUMN [Status] smallint NOT NULL
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
PRINT N'Aggiunta della colonna [UniqueIdDocumentSeriesItem] alla tabella [dbo].[DocumentSeriesItemLog]' 
GO

ALTER TABLE [dbo].[DocumentSeriesItemLog] ADD [UniqueIdDocumentSeriesItem] [uniqueidentifier] NULL
GO

UPDATE DSIL SET DSIL.[UniqueIdDocumentSeriesItem] = DSI.UniqueId
FROM [dbo].[DocumentSeriesItemLog] AS DSIL 
INNER JOIN [dbo].[DocumentSeriesItem] DSI ON DSI.Id = DSIL.IdDocumentSeriesItem
WHERE DSIL.[UniqueIdDocumentSeriesItem] IS NULL
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
PRINT N'Aggiunta delle colonna [UniqueId] alla tabella [dbo].[DocumentSeriesItemLog]';
GO

ALTER TABLE [dbo].[DocumentSeriesItemLog] ADD [UniqueId] uniqueidentifier NULL
GO

UPDATE [dbo].[DocumentSeriesItemLog] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[DocumentSeriesItemLog] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
GO

CREATE UNIQUE INDEX [IX_DocumentSeriesItemLog_UniqueId] ON [dbo].[DocumentSeriesItemLog]([UniqueId] ASC);
GO

ALTER TABLE [dbo].[DocumentSeriesItemLog] ADD CONSTRAINT [DF_DocumentSeriesItemLog_UniqueId] 
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
PRINT N'Aggiunta delle colonna [Timestamp] alla tabella [dbp].[DocumentSeriesItemLog]';
GO

ALTER TABLE [dbo].[DocumentSeriesItemLog] ADD [Timestamp] TIMESTAMP NOT NULL
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
PRINT 'Creazione tabella [DossierLinks]'
GO

CREATE TABLE [dbo].[DossierLinks](
	[IdDossierLink] [uniqueidentifier],
	[IdDossierParent] [uniqueidentifier] NOT NULL,
    [IdDossierSon] [uniqueidentifier] NOT NULL,
	[DossierLinkType] [smallint] NOT NULL,
	[RegistrationDate] [DateTimeOffset](7) DEFAULT GETUTCDATE() NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL
 CONSTRAINT [PK_DossierLinks] PRIMARY KEY NONCLUSTERED 
(
	[IdDossierLink] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DossierLinks] WITH CHECK ADD CONSTRAINT [FK_DossierLinks_Dossiers_Parent] FOREIGN KEY([IdDossierParent])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO
ALTER TABLE [dbo].[DossierLinks] CHECK CONSTRAINT [FK_DossierLinks_Dossiers_Parent]
GO

ALTER TABLE [dbo].[DossierLinks] WITH CHECK ADD CONSTRAINT [FK_DossierLinks_Dossiers_Son] FOREIGN KEY([IdDossierSon])
REFERENCES [dbo].[Dossiers] ([IdDossier])
GO
ALTER TABLE [dbo].[DossierLinks] CHECK CONSTRAINT [FK_DossierLinks_Dossiers_Son]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DossierLinks_IdDossierParent_IdDossierSon] 
	ON [dbo].[DossierLinks]([IdDossierParent] ASC, [IdDossierSon] ASC)
GO

CREATE CLUSTERED INDEX [IX_DossierLinks_RegistrationDate]
    ON [dbo].[DossierLinks]([RegistrationDate] ASC);
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
PRINT N'Creata tabella [uds].[UDSTypologies]';
GO

CREATE TABLE [uds].[UDSTypologies]
(
	[IdUDSTypology] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Status] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
CONSTRAINT [PK_UDSTypologies] PRIMARY KEY NONCLUSTERED
(
	[IdUDSTypology] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_UDSTypologies_RegistrationDate] 
	ON [uds].[UDSTypologies] ([RegistrationDate] ASC)
GO

CREATE UNIQUE INDEX [IX_UDSTypologies_Name]
	ON [uds].[UDSTypologies]([Name] ASC);
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
PRINT N'Creata tabella [uds].[UDSRepositoryTypologies]';
GO

CREATE TABLE [uds].[UDSRepositoryTypologies]
(
	[IdUDSRepositoryTypology] [uniqueidentifier] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdUDSTypology] [uniqueidentifier] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
CONSTRAINT [PK_UDSRepositoryTypologies] PRIMARY KEY NONCLUSTERED
(
	[IdUDSRepositoryTypology] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [uds].[UDSRepositoryTypologies] ADD DEFAULT (newsequentialid()) FOR [IdUDSRepositoryTypology]
GO

ALTER TABLE [uds].[UDSRepositoryTypologies] ADD  CONSTRAINT [DF_UDSRepositoryTypologies_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]
GO

CREATE CLUSTERED INDEX [IX_UDSRepositoryTypologies_RegistrationDate]
    ON [uds].[UDSRepositoryTypologies]([RegistrationDate] ASC);
GO

ALTER TABLE [uds].[UDSRepositoryTypologies] WITH CHECK ADD  CONSTRAINT [FK_UDSRepositoryTypologies_UDSTypologies] FOREIGN KEY([IdUDSTypology])
REFERENCES [uds].[UDSTypologies] ([IdUDSTypology])
GO

ALTER TABLE [uds].[UDSRepositoryTypologies] CHECK CONSTRAINT [FK_UDSRepositoryTypologies_UDSTypologies]
GO

ALTER TABLE [uds].[UDSRepositoryTypologies]  WITH CHECK ADD  CONSTRAINT [FK_UDSRepositoryTypologies_UDSRepositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [uds].[UDSRepositoryTypologies] CHECK CONSTRAINT [FK_UDSRepositoryTypologies_UDSRepositories]
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
PRINT N'Modificata la colonna [Account] alla tabella [dbo].[RoleUser]' 
GO

ALTER TABLE [dbo].[RoleUser] ALTER COLUMN [Account] nvarchar(256) NOT NULL
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
PRINT N'Creazione indice [IX_RoleUser_Type_Enabled]';
GO

CREATE NONCLUSTERED INDEX [IX_RoleUser_Type_Enabled]
ON [dbo].[RoleUser] ([Type],[Enabled])
INCLUDE ([idRole],[Account])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
   BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
   BEGIN
        INSERT INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
   END
GO

--#############################################################################
PRINT  N'Creazione indice [IX_RoleUser_idRole_Type_Enabled]';
GO

CREATE NONCLUSTERED INDEX [IX_RoleUser_idRole_Type_Enabled]
ON [dbo].[RoleUser] ([idRole],[Type],[Enabled])
INCLUDE ([Account])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
   BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
   BEGIN
        INSERT INTO #tmpErrors (Error) VALUES (1);
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