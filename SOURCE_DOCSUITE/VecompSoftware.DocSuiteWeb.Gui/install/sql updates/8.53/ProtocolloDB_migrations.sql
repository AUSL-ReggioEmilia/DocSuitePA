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
PRINT 'Versionamento database alla 8.53'
GO

EXEC dbo.VersioningDatabase N'8.53'
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
PRINT 'ALTER TABLE [dbo].[PECMailattachment] ALTER COLUMN [attachmentname] nvarchar(256)'
GO

ALTER TABLE [dbo].[PECMailattachment] ALTER COLUMN [attachmentname] nvarchar(256) NOT NULL
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
PRINT 'Aggiunta colonne IdUDSRepository e DocumentUnitType nella tabella PECMail'
GO

ALTER TABLE [dbo].[PECMail] ADD IdUDS [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[PECMail] ADD [IdUDSRepository] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[PECMail] ADD [DocumentUnitType] [int] NULL
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
PRINT 'Aggiungo la CONSTRAINT FK_PECMail_UDSRepositories'
GO

ALTER TABLE [dbo].[PECMail]  WITH CHECK ADD  CONSTRAINT [FK_PECMail_UDSRepositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [dbo].[PECMail] CHECK CONSTRAINT [FK_PECMail_UDSRepositories]
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
PRINT 'Aggiorno DocumentUnitType per le PEC già protocollate'
GO

UPDATE [dbo].[PECMail] SET [DocumentUnitType] = 1 WHERE [Year] is not null AND [Number] is not null
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
PRINT 'Aggiungo la colonna Alias per la tabella UDSRepositories'
GO

ALTER TABLE [uds].[UDSRepositories] ADD Alias [nvarchar](4) NULL
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
PRINT 'Aggiungo la CONSTRAINT FK_Protocol_UDSRepositories'
GO

ALTER TABLE [dbo].[Protocol]  WITH CHECK ADD  CONSTRAINT [FK_Protocol_UDSRepositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [dbo].[Protocol] CHECK CONSTRAINT [FK_Protocol_UDSRepositories]
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
PRINT 'Creazione tabella [ProtocolHighlightUsers]'

CREATE TABLE [dbo].[ProtocolHighlightUsers] (
	[IdProtocolHighlightUser] [uniqueidentifier] NOT NULL,
	[Year] [smallint] NOT NULL,
	[Number] [int] NOT NULL,
	[UniqueIdProtocol] [uniqueidentifier] NOT NULL,
	[Account] [nvarchar](256) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,	
 CONSTRAINT [PK_ProtocolHighlightUsers] PRIMARY KEY NONCLUSTERED 
(
	[IdProtocolHighlightUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_ProtocolHighlightUsers_RegistationDate]
    ON [dbo].[ProtocolHighlightUsers]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[ProtocolHighlightUsers]  WITH CHECK ADD  CONSTRAINT [FK_ProtocolHighlightUsers_Protocol] FOREIGN KEY([Year], [Number])
REFERENCES [dbo].[Protocol] ([Year], [Number])
GO
ALTER TABLE [dbo].[ProtocolHighlightUsers] CHECK CONSTRAINT [FK_ProtocolHighlightUsers_Protocol]
GO

ALTER TABLE [dbo].[ProtocolHighlightUsers]  WITH CHECK ADD  CONSTRAINT [FK_ProtocolHighlightUsers_Protocol_UniqueId] FOREIGN KEY([UniqueIdProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])
GO
ALTER TABLE [dbo].[ProtocolHighlightUsers] CHECK CONSTRAINT [FK_ProtocolHighlightUsers_Protocol_UniqueId]
GO

--#############################################################################

PRINT N'Creazione indice univoco [IX_ProtocolHighlightUsers_UniqueIdProtocol_Account]';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ProtocolHighlightUsers_UniqueIdProtocol_Account]
    ON [dbo].[ProtocolHighlightUsers]([UniqueIdProtocol] ASC, [Account] ASC);
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

PRINT 'Creazione colonna UniqueId nullable nella tabella DocumentSeriesItem'
GO
-- 1 
ALTER TABLE [dbo].[DocumentSeriesItem] 
ADD [UniqueId] [uniqueidentifier] NULL
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
PRINT 'Aggiornamento della tabella DocumentSeriesItem'
GO
-- 2 
UPDATE [dbo].[DocumentSeriesItem]
SET [UniqueId] = NEWID()
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
PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[DocumentSeriesItem]
    ADD CONSTRAINT [DF_DocumentSeriesItem_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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
PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[DocumentSeriesItem] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT N'Aggiunto unique indice [IX_DocumentSeriesItem_UniqueId] in DocumentSeriesItem';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DocumentSeriesItem_UniqueId]
    ON [dbo].[DocumentSeriesItem]([UniqueId] ASC);
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

PRINT N'Aggiunto unique indice [IX_Role_UniqueId] in Role';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Role_UniqueId]
    ON [dbo].[Role]([UniqueId] ASC);
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

PRINT 'Creazione colonna UniqueId nullable nella tabella DocumentSeriesItemRole'
GO
-- 1 
ALTER TABLE [dbo].[DocumentSeriesItemRole] 
ADD [UniqueId] [uniqueidentifier] NULL
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

PRINT 'Aggiornamento della tabella DocumentSeriesItemRole'
GO
-- 2 
UPDATE [dbo].[DocumentSeriesItemRole]
SET [UniqueId] = NEWID()
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

PRINT 'Creazione valore di default per UniqueId'
GO
-- 3
ALTER TABLE [dbo].[DocumentSeriesItemRole]
    ADD CONSTRAINT [DF_DocumentSeriesItemRole_UniqueId] 
	DEFAULT NEWSEQUENTIALID() 
	FOR [UniqueId];
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

PRINT 'Modifica UniqueId not nullable'
GO
-- 4
ALTER TABLE [dbo].[DocumentSeriesItemRole] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT N'Aggiunto unique indice [IX_DocumentSeriesItemRole_UniqueId] in DocumentSeriesItemRole';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DocumentSeriesItemRole_UniqueId]
    ON [dbo].[DocumentSeriesItemRole]([UniqueId] ASC);
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

PRINT 'Modifica tabella ProtocolRole colonna RegistrationDate nullable'
GO

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET(7) NULL
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

PRINT N'Modificata [FK_Protocol_ProtocolJournalLog] in Protocol';
GO

ALTER TABLE [dbo].[Protocol] DROP CONSTRAINT [FK_Protocol_ProtocolJournalLog]
GO

ALTER TABLE [dbo].[Protocol]  WITH CHECK ADD  CONSTRAINT [FK_Protocol_ProtocolJournalLog] FOREIGN KEY([IdProtocolJournalLog])
REFERENCES [dbo].[ProtocolJournalLog] ([IdProtocolJournalLog]) on delete set null
GO

ALTER TABLE [dbo].[Protocol] CHECK CONSTRAINT [FK_Protocol_ProtocolJournalLog]
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

PRINT N'Modificata colonna [Conservation] in Container';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [Conservation] smallint null
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

PRINT N'Modificata colonna [isActive] in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [isActive] tinyint null
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

PRINT N'Modificata colonna [Collapsed] in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [Collapsed] tinyint null
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
PRINT N'ALTER TABLE [dbo].[FasciclePeriods] ADD [Timestamp] TIMESTAMP NOT NULL';
GO

ALTER TABLE [dbo].[FasciclePeriods] ADD [Timestamp] TIMESTAMP NOT NULL
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