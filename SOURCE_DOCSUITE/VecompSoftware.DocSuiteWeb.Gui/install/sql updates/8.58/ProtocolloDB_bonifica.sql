
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

PRINT 'Aggiunta colonna [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [MessageContact]';
GO

ALTER TABLE [dbo].[MessageContact] ADD [RegistrationDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[MessageContact] ADD [RegistrationUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[MessageContact] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[MessageContact] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE MC SET MC.RegistrationDate = M.RegistrationDate,
			  MC.RegistrationUser = M.RegistrationUser,
			  MC.LastChangedDate = M.LastchangedDate,
			  MC.LastChangedUser = M.LastchangedUser
FROM [dbo].[MessageContact] AS MC
INNER JOIN [dbo].[Message] M on M.[IDMessage] = MC.[IDMessage]

ALTER TABLE [dbo].[MessageContact] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET not null
GO
ALTER TABLE [dbo].[MessageContact] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Creato indice univoco [IX_MessageContact_UniqueId] nella tabella [MessageContact]';
GO

CREATE UNIQUE INDEX [IX_MessageContact_UniqueId] ON [dbo].[MessageContact]([UniqueId] ASC);
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

PRINT 'Aggiunta colonna [TimeStamp] nella tabella [MessageContact]';
GO

ALTER TABLE [dbo].[MessageContact] ADD [Timestamp] TIMESTAMP not null
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
PRINT 'Aggiunta colonna [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [MessageEmail]';
GO

ALTER TABLE [dbo].[MessageEmail] ADD [RegistrationDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[MessageEmail] ADD [RegistrationUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[MessageEmail] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[MessageEmail] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE ME SET ME.RegistrationDate = M.RegistrationDate,
			  ME.RegistrationUser = M.RegistrationUser,
			  ME.LastChangedDate = M.LastchangedDate,
			  ME.LastChangedUser = M.LastchangedUser
FROM [dbo].[MessageEmail] AS ME
INNER JOIN [dbo].[Message] M on M.[IDMessage] = ME.[IDMessage]

ALTER TABLE [dbo].[MessageEmail] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET not null
GO
ALTER TABLE [dbo].[MessageEmail] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Creato indice univoco [IX_MessageEmail_UniqueId] nella tabella [MessageEmail]';
GO
CREATE UNIQUE INDEX [IX_MessageEmail_UniqueId] ON [dbo].[MessageEmail]([UniqueId] ASC);
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
PRINT 'Aggiunta colonna [TimeStamp] nella tabella [MessageEmail]';
GO

ALTER TABLE [dbo].[MessageEmail] ADD [Timestamp] TIMESTAMP not null
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
PRINT 'Aggiunta colonna [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [MessageContactEmail]';
GO

ALTER TABLE [dbo].[MessageContactEmail] ADD [RegistrationDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[MessageContactEmail] ADD [RegistrationUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[MessageContactEmail] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[MessageContactEmail] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE MCE SET MCE.RegistrationDate = MC.RegistrationDate,
			  MCE.RegistrationUser = MC.RegistrationUser,
			  MCE.LastChangedDate = MC.LastchangedDate,
			  MCE.LastChangedUser = MC.LastchangedUser
FROM [dbo].[MessageContactEmail] AS MCE
INNER JOIN [dbo].[MessageContact] MC ON MC.[IDMessageContact] = MCE.[IDMessageContact]


ALTER TABLE [dbo].[MessageContactEmail] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET not null
GO
ALTER TABLE [dbo].[MessageContactEmail] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Creato indice univoco [IX_MessageContactEmail_UniqueId] nella tabella [MessageContactEmail]';
GO
CREATE UNIQUE INDEX [IX_MessageContactEmail_UniqueId] ON [dbo].[MessageContactEmail]([UniqueId] ASC);
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
PRINT 'Aggiunta colonna [TimeStamp] nella tabella [MessageContactEmail]';
GO

ALTER TABLE [dbo].[MessageContactEmail] ADD [Timestamp] TIMESTAMP not null
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
--##############################################################################
PRINT 'Aggiunta colonna [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [MessageAttachment]';
GO

ALTER TABLE [dbo].[MessageAttachment] ADD [RegistrationDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[MessageAttachment] ADD [RegistrationUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[MessageAttachment] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[MessageAttachment] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE MA SET MA.RegistrationDate = M.RegistrationDate,
			  MA.RegistrationUser = M.RegistrationUser,
			  MA.LastChangedDate = M.LastChangedDate,
			  MA.LastChangedUser = M.LastChangedUser
FROM [dbo].[MessageAttachment] AS MA
INNER JOIN [dbo].[Message] M ON M.[IDMessage] = MA.[IDMessage]


ALTER TABLE [dbo].[MessageAttachment] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET(7) not null
GO
ALTER TABLE [dbo].[MessageAttachment] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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
PRINT 'Creato indice univoco [IX_MessageAttachment_UniqueId] nella tabella [MessageAttachment]';
GO
CREATE UNIQUE INDEX [IX_MessageAttachment_UniqueId] ON [dbo].[MessageAttachment]([UniqueId] ASC);
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
PRINT 'Aggiunta colonna [TimeStamp] nella tabella [MessageAttachment]';
GO

ALTER TABLE [dbo].[MessageAttachment] ADD [Timestamp] TIMESTAMP not null
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
--##############################################################################
PRINT 'Creato indice univoco [IX_MessageLog_UniqueId] nella tabella [MessageLog]';
GO
CREATE UNIQUE INDEX [IX_MessageLog_UniqueId] ON [dbo].[MessageLog]([UniqueId] ASC);
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
--##############################################################################
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