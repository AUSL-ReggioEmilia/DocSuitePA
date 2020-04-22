
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

PRINT 'Aggiunta colonna [Timestamp] nella tabella [Protocol]';
GO

ALTER TABLE [dbo].[Protocol] ADD [Timestamp] TIMESTAMP not null
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolContact]';
GO

ALTER TABLE [dbo].[ProtocolContact] ADD [UniqueId] [uniqueidentifier] null
GO

UPDATE [dbo].[ProtocolContact] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[ProtocolContact] ADD CONSTRAINT [DF_ProtocolContact_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

ALTER TABLE [dbo].[ProtocolContact] ALTER COLUMN [UniqueId] [uniqueidentifier] not null
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

PRINT 'Aggiunta colonne [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolContact]';
GO

ALTER TABLE [dbo].[ProtocolContact] ADD [UniqueIdProtocol] uniqueidentifier null
GO
ALTER TABLE [dbo].[ProtocolContact] ADD [RegistrationDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolContact] ADD [RegistrationUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[ProtocolContact] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolContact] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE PC SET PC.UniqueIdProtocol = P.UniqueId,
			  PC.RegistrationDate = P.RegistrationDate,
			  PC.RegistrationUser = P.RegistrationUser,
			  PC.LastChangedDate = P.LastChangedDate,
			  PC.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolContact] AS PC
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PC.[Year] and P.[Number] = PC.[Number]

ALTER TABLE [dbo].[ProtocolContact] ALTER COLUMN [UniqueIdProtocol] uniqueidentifier not null
GO
ALTER TABLE [dbo].[ProtocolContact] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null
GO
ALTER TABLE [dbo].[ProtocolContact] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Aggiunta colonna [TimeStamp] nella tabella [ProtocolContact]';
GO
ALTER TABLE [dbo].[ProtocolContact] ADD [Timestamp] TIMESTAMP not null
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

PRINT 'Creato indice univoco [IX_ProtocolContact_UniqueId] nella tabella [ProtocolContact]';
GO

CREATE UNIQUE INDEX [IX_ProtocolContact_UniqueId] ON [dbo].[ProtocolContact]([UniqueId] ASC);
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolRole]';
GO

ALTER TABLE [dbo].[ProtocolRole] ADD [UniqueId] [uniqueidentifier] null
GO

UPDATE [dbo].[ProtocolRole] SET [UniqueId] = NEWID()

GO

ALTER TABLE [dbo].[ProtocolRole] ADD CONSTRAINT [DF_ProtocolRole_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [UniqueId] [uniqueidentifier] not null
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

PRINT 'Aggiunta colonne [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolRole]';
GO

ALTER TABLE [dbo].[ProtocolRole] ADD [UniqueIdProtocol] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolRole] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolRole] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE PR SET PR.UniqueIdProtocol = P.UniqueId,
			  PR.RegistrationDate = P.RegistrationDate,
			  PR.RegistrationUser = P.RegistrationUser,
			  PR.LastChangedDate = P.LastChangedDate,
			  PR.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolRole] AS PR
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PR.[Year] and P.[Number] = PR.[Number]

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null
GO
ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null
GO
ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Aggiunta colonna [TimeStamp] nella tabella [ProtocolRole]';
GO

ALTER TABLE [dbo].[ProtocolRole] ADD [Timestamp] TIMESTAMP not null
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

PRINT 'Creato indice univoco [IX_ProtocolContact_UniqueId] nella tabella [ProtocolRole]';
GO

CREATE UNIQUE INDEX [IX_ProtocolRole_UniqueId] ON [dbo].[ProtocolRole]([UniqueId] ASC);
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
PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolParer]';
GO

ALTER TABLE [dbo].[ProtocolParer] ADD [UniqueId] [uniqueidentifier] null
GO

UPDATE [dbo].[ProtocolParer] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[ProtocolParer] ADD CONSTRAINT [DF_ProtocolParer_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

ALTER TABLE [dbo].[ProtocolParer] ALTER COLUMN [UniqueId] [uniqueidentifier] not null
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

PRINT 'Aggiunta colonne [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolParer]';
GO

ALTER TABLE [dbo].[ProtocolParer] ADD [UniqueIdProtocol] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolParer] ADD [RegistrationDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolParer] ADD [RegistrationUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[ProtocolParer] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolParer] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE PP SET PP.UniqueIdProtocol = P.UniqueId,
			  PP.RegistrationDate = P.RegistrationDate,
			  PP.RegistrationUser = P.RegistrationUser,
			  PP.LastChangedDate = P.LastChangedDate,
			  PP.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolParer] AS PP
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PP.[Year] and P.[Number] = PP.[Number]

ALTER TABLE [dbo].[ProtocolParer] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null
GO
ALTER TABLE [dbo].[ProtocolParer] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null
GO
ALTER TABLE [dbo].[ProtocolParer] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Aggiunta colonna [TimeStamp] nella tabella [ProtocolParer]';
GO

ALTER TABLE [dbo].[ProtocolParer] ADD [Timestamp] TIMESTAMP not null
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

PRINT 'Creato indice univoco [IX_ProtocolContact_UniqueId] nella tabella [ProtocolParer]';
GO

CREATE UNIQUE INDEX [IX_ProtocolParer_UniqueId] ON [dbo].[ProtocolParer]([UniqueId] ASC);
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
PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolContactManual]';
GO

ALTER TABLE [dbo].[ProtocolContactManual] ADD [UniqueId] [uniqueidentifier] null
GO

UPDATE [dbo].[ProtocolContactManual] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[ProtocolContactManual] ADD CONSTRAINT [DF_ProtocolContactManual_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

ALTER TABLE [dbo].[ProtocolContactManual] ALTER COLUMN [UniqueId] [uniqueidentifier] not null
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

PRINT 'Aggiunta colonne [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolContactManual]';
GO

ALTER TABLE [dbo].[ProtocolContactManual] ADD [UniqueIdProtocol] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolContactManual] ADD [RegistrationDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolContactManual] ADD [RegistrationUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[ProtocolContactManual] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolContactManual] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE PCM SET PCM.UniqueIdProtocol = P.UniqueId,
			   PCM.RegistrationDate = P.RegistrationDate,
			   PCM.RegistrationUser = P.RegistrationUser,
			   PCM.LastChangedDate = P.LastChangedDate,
			   PCM.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolContactManual] AS PCM
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PCM.[Year] and P.[Number] = PCM.[Number]

ALTER TABLE [dbo].[ProtocolContactManual] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null
GO
ALTER TABLE [dbo].[ProtocolContactManual] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null
GO
ALTER TABLE [dbo].[ProtocolContactManual] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Aggiunta colonna [TimeStamp] nella tabella [ProtocolContactManual]';
GO

ALTER TABLE [dbo].[ProtocolContactManual] ADD [Timestamp] TIMESTAMP not null
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

PRINT 'Creato indice univoco [IX_ProtocolContact_UniqueId] nella tabella [ProtocolContactManual]';
GO

CREATE UNIQUE INDEX [IX_ProtocolContactManual_UniqueId] ON [dbo].[ProtocolContactManual]([UniqueId] ASC);
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolLog]';
GO

ALTER TABLE [dbo].[ProtocolLog] ADD [UniqueId] [uniqueidentifier] null
GO

UPDATE [dbo].[ProtocolLog] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[ProtocolLog] ADD CONSTRAINT [DF_ProtocolLog_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

ALTER TABLE [dbo].[ProtocolLog] ALTER COLUMN [UniqueId] [uniqueidentifier] not null
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

PRINT 'Aggiunta colonna [UniqueIdProtocol] nella tabella [ProtocolLog]';
GO

ALTER TABLE [dbo].[ProtocolLog] ADD [UniqueIdProtocol] [uniqueidentifier] null
GO

UPDATE PL SET PL.[UniqueIdProtocol] = P.UniqueId
FROM [dbo].[ProtocolLog] AS PL
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PL.[Year] and P.[Number] = PL.[Number]

ALTER TABLE [dbo].[ProtocolLog] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null
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

PRINT 'Creato indice univoco [IX_ProtocolLog_UniqueId] nella tabella [ProtocolLog]';
GO

CREATE UNIQUE INDEX [IX_ProtocolLog_UniqueId] ON [dbo].[ProtocolLog]([UniqueId] ASC);
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolRoleUser]';
GO

ALTER TABLE [dbo].[ProtocolRoleUser] ADD [UniqueId] [uniqueidentifier] null
GO

UPDATE [dbo].[ProtocolRoleUser] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[ProtocolRoleUser] ADD CONSTRAINT [DF_ProtocolRoleUser_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

ALTER TABLE [dbo].[ProtocolRoleUser] ALTER COLUMN [UniqueId] [uniqueidentifier] not null
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

PRINT 'Aggiunta colonne [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolRoleUser]';
GO

ALTER TABLE [dbo].[ProtocolRoleUser] ADD [UniqueIdProtocol] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolRoleUser] ADD [RegistrationDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolRoleUser] ADD [RegistrationUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[ProtocolRoleUser] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolRoleUser] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE PRU SET PRU.UniqueIdProtocol = P.UniqueId,
			   PRU.RegistrationDate = P.RegistrationDate,
			   PRU.RegistrationUser = P.RegistrationUser,
			   PRU.LastChangedDate = P.LastChangedDate,
			   PRU.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolRoleUser] AS PRU
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PRU.[Year] and P.[Number] = PRU.[Number]

ALTER TABLE [dbo].[ProtocolRoleUser] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null
GO
ALTER TABLE [dbo].[ProtocolRoleUser] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null
GO
ALTER TABLE [dbo].[ProtocolRoleUser] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Aggiunta colonna [Timestamp] nella tabella [ProtocolRoleUser]';
GO

ALTER TABLE [dbo].[ProtocolRoleUser] ADD [Timestamp] TIMESTAMP not null
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

PRINT 'Creato indice univoco [IX_ProtocolRoleUser_UniqueId] nella tabella [ProtocolRoleUser]';
GO

CREATE UNIQUE INDEX [IX_ProtocolRoleUser_UniqueId] ON [dbo].[ProtocolRoleUser]([UniqueId] ASC);
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolMessage]';
GO

ALTER TABLE [dbo].[ProtocolMessage] ADD [UniqueId] [uniqueidentifier] null
GO

UPDATE [dbo].[ProtocolMessage] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[ProtocolMessage] ADD CONSTRAINT [DF_ProtocolMessage_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

ALTER TABLE [dbo].[ProtocolMessage] ALTER COLUMN [UniqueId] [uniqueidentifier] not null
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

PRINT 'Aggiunta colonne [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolMessage]';
GO

ALTER TABLE [dbo].[ProtocolMessage] ADD [UniqueIdProtocol] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolMessage] ADD [RegistrationDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolMessage] ADD [RegistrationUser] nvarchar(256) null
GO
ALTER TABLE [dbo].[ProtocolMessage] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolMessage] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE PM SET PM.UniqueIdProtocol = P.UniqueId,
			  PM.RegistrationDate = P.RegistrationDate,
			  PM.RegistrationUser = P.RegistrationUser,
			  PM.LastChangedDate = P.LastChangedDate,
			  PM.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolMessage] AS PM
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PM.[Year] and P.[Number] = PM.[Number]

ALTER TABLE [dbo].[ProtocolMessage] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null
GO
ALTER TABLE [dbo].[ProtocolMessage] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null
GO
ALTER TABLE [dbo].[ProtocolMessage] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Aggiunta colonna [Timestamp] nella tabella [ProtocolMessage]';
GO

ALTER TABLE [dbo].[ProtocolMessage] ADD [Timestamp] TIMESTAMP not null
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

PRINT 'Creato indice univoco [IX_ProtocolMessage_UniqueId] nella tabella [ProtocolMessage]';
GO

CREATE UNIQUE INDEX [IX_ProtocolMessage_UniqueId] ON [dbo].[ProtocolMessage]([UniqueId] ASC);
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

PRINT 'Aggiunta colonna [Timestamp] nella tabella [ProtocolHighlightUsers]';
GO

ALTER TABLE [dbo].[ProtocolHighlightUsers] ADD [Timestamp] TIMESTAMP not null
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolLinks]';
GO

ALTER TABLE [dbo].[ProtocolLinks] ADD [UniqueId] [uniqueidentifier] null
GO

UPDATE [dbo].[ProtocolLinks] SET [UniqueId] = NEWID()
GO

ALTER TABLE [dbo].[ProtocolLinks] ADD CONSTRAINT [DF_ProtocolLinks_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [UniqueId] [uniqueidentifier] not null
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

PRINT 'Aggiunta colonne [UniqueIdProtocolParent], [UniqueIdProtocolSon], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolLinks]';
GO

ALTER TABLE [dbo].[ProtocolLinks] ADD [UniqueIdProtocolParent] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolLinks] ADD [UniqueIdProtocolSon] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolLinks] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolLinks] ADD [LastChangedUser] nvarchar(256) null
GO

UPDATE PL SET PL.UniqueIdProtocolParent = P.UniqueId,
			  PL.RegistrationDate = P.RegistrationDate,
			  PL.RegistrationUser = P.RegistrationUser,
			  PL.LastChangedDate = P.LastChangedDate,
			  PL.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolLinks] AS PL
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PL.[Year] and P.[Number] = PL.[Number]

UPDATE PS SET PS.UniqueIdProtocolSon = P.UniqueId
FROM [dbo].[ProtocolLinks] AS PS
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PS.[YearSon] and P.[Number] = PS.[NumberSon]

ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [UniqueIdProtocolParent] [uniqueidentifier] not null
GO
ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [UniqueIdProtocolSon] [uniqueidentifier] not null
GO
ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null
GO
ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [RegistrationUser] nvarchar(256) not null
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

PRINT 'Aggiunta colonna [TimeStamp] nella tabella [ProtocolLinks]';
GO

ALTER TABLE [dbo].[ProtocolLinks] ADD [Timestamp] TIMESTAMP not null
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

PRINT 'Creato indice univoco [IX_ProtocolContact_UniqueId] nella tabella [ProtocolLinks]';
GO

CREATE UNIQUE INDEX [IX_ProtocolLinks_UniqueId] ON [dbo].[ProtocolLinks]([UniqueId] ASC);
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

PRINT 'Creazione FK a Protocol nella tabella [ProtocolContact]'
GO

ALTER TABLE [dbo].[ProtocolContact] WITH CHECK ADD CONSTRAINT [FK_ProtocolContact_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolContact] CHECK CONSTRAINT [FK_ProtocolContact_Protocol_UniqueId]
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

PRINT 'Creazione FK a Protocol nella tabella [ProtocolContactManual]'
GO

ALTER TABLE [dbo].[ProtocolContactManual] WITH CHECK ADD CONSTRAINT [FK_ProtocolContactManual_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolContactManual] CHECK CONSTRAINT [FK_ProtocolContactManual_Protocol_UniqueId]
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

PRINT 'Creazione FK a Protocol nella tabella [ProtocolRole]'
GO

ALTER TABLE [dbo].[ProtocolRole] WITH CHECK ADD CONSTRAINT [FK_ProtocolRole_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolRole] CHECK CONSTRAINT [FK_ProtocolRole_Protocol_UniqueId]
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

PRINT 'Creazione FK a Protocol nella tabella [ProtocolRoleUser]'
GO

ALTER TABLE [dbo].[ProtocolRoleUser] WITH CHECK ADD CONSTRAINT [FK_ProtocolRoleUser_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolRoleUser] CHECK CONSTRAINT [FK_ProtocolRoleUser_Protocol_UniqueId]
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

PRINT 'Creazione FK a Protocol nella tabella [ProtocolParer]'
GO

ALTER TABLE [dbo].[ProtocolParer] WITH CHECK ADD CONSTRAINT [FK_ProtocolParer_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolParer] CHECK CONSTRAINT [FK_ProtocolParer_Protocol_UniqueId]
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

PRINT 'Creazione FK a Protocol nella tabella [ProtocolLinks]'
GO

ALTER TABLE [dbo].[ProtocolLinks] WITH CHECK ADD CONSTRAINT [FK_ProtocolLinks_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocolParent]) 
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolLinks] CHECK CONSTRAINT [FK_ProtocolLinks_Protocol_UniqueId]
GO

ALTER TABLE [dbo].[ProtocolLinks] WITH CHECK ADD CONSTRAINT [FK_ProtocolLinks_Protocol_Son_UniqueId] FOREIGN KEY ([UniqueIdProtocolSon]) 
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolLinks] CHECK CONSTRAINT [FK_ProtocolLinks_Protocol_Son_UniqueId]
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

PRINT 'Creazione FK a Protocol nella tabella [ProtocolMessage]'
GO

ALTER TABLE [dbo].[ProtocolMessage] WITH CHECK ADD CONSTRAINT [FK_ProtocolMessage_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolMessage] CHECK CONSTRAINT [FK_ProtocolMessage_Protocol_UniqueId]
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

PRINT 'Creazione FK a Protocol nella tabella [ProtocolLog]'
GO

ALTER TABLE [dbo].[ProtocolLog] WITH CHECK ADD CONSTRAINT [FK_ProtocolLog_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolLog] CHECK CONSTRAINT [FK_ProtocolLog_Protocol_UniqueId]
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