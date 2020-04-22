
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

PRINT 'Aggiunta colonna nullable [UniqueId] nella tabella [ProtocolContact]';
GO

ALTER TABLE [dbo].[ProtocolContact] ADD [UniqueId] [uniqueidentifier] null

GO

ALTER TABLE [dbo].[ProtocolContact] ADD CONSTRAINT [DF_ProtocolContact_UniqueId] 
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

PRINT 'Aggiunta colonne nullable [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolContact]';
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

PRINT 'Aggiunta colonna nullable [UniqueId] nella tabella [ProtocolRole]';
GO

ALTER TABLE [dbo].[ProtocolRole] ADD [UniqueId] [uniqueidentifier] null
GO

ALTER TABLE [dbo].[ProtocolRole] ADD CONSTRAINT [DF_ProtocolRole_UniqueId] 
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

PRINT 'Aggiunta colonne nullable [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolRole]';
GO

ALTER TABLE [dbo].[ProtocolRole] ADD [UniqueIdProtocol] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolRole] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolRole] ADD [LastChangedUser] nvarchar(256) null
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

PRINT 'Aggiunta colonna nullable [UniqueId] nella tabella [ProtocolParer]';
GO

ALTER TABLE [dbo].[ProtocolParer] ADD [UniqueId] [uniqueidentifier] null

GO

ALTER TABLE [dbo].[ProtocolParer] ADD CONSTRAINT [DF_ProtocolParer_UniqueId] 
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

PRINT 'Aggiunta colonne nullable [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolParer]';
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

PRINT 'Aggiunta colonna nullable [UniqueId] nella tabella [ProtocolContactManual]';
GO

ALTER TABLE [dbo].[ProtocolContactManual] ADD [UniqueId] [uniqueidentifier] null
GO

ALTER TABLE [dbo].[ProtocolContactManual] ADD CONSTRAINT [DF_ProtocolContactManual_UniqueId] 
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

PRINT 'Aggiunta colonne nullable [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolContactManual]';
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

PRINT 'Aggiunta colonna nullable [UniqueId] nella tabella [ProtocolLog]';
GO

ALTER TABLE [dbo].[ProtocolLog] ADD [UniqueId] [uniqueidentifier] null
GO

ALTER TABLE [dbo].[ProtocolLog] ADD CONSTRAINT [DF_ProtocolLog_UniqueId] 
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

PRINT 'Aggiunta colonna nullable [UniqueIdProtocol] nella tabella [ProtocolLog]';
GO

ALTER TABLE [dbo].[ProtocolLog] ADD [UniqueIdProtocol] [uniqueidentifier] null
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

PRINT 'Aggiunta colonna nullable [UniqueId] nella tabella [ProtocolRoleUser]';
GO

ALTER TABLE [dbo].[ProtocolRoleUser] ADD [UniqueId] [uniqueidentifier] null
GO

ALTER TABLE [dbo].[ProtocolRoleUser] ADD CONSTRAINT [DF_ProtocolRoleUser_UniqueId] 
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

PRINT 'Aggiunta colonne nullable [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolRoleUser]';
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

PRINT 'Aggiunta colonna nullable [UniqueId] nella tabella [ProtocolMessage]';
GO

ALTER TABLE [dbo].[ProtocolMessage] ADD [UniqueId] [uniqueidentifier] null
GO

ALTER TABLE [dbo].[ProtocolMessage] ADD CONSTRAINT [DF_ProtocolMessage_UniqueId] 
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

PRINT 'Aggiunta colonne nullable [UniqueIdProtocol], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolMessage]';
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

ALTER TABLE [dbo].[ProtocolLinks] ADD CONSTRAINT [DF_ProtocolLinks_UniqueId] 
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

PRINT 'Aggiunta colonne nullable [UniqueIdProtocolParent], [UniqueIdProtocolSon], [RegistrationDate], [RegistrationUser], [LastChangedDate], [LastChangedUser] nella tabella [ProtocolLinks]';
GO

ALTER TABLE [dbo].[ProtocolLinks] ADD [UniqueIdProtocolParent] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolLinks] ADD [UniqueIdProtocolSon] [uniqueidentifier] null
GO
ALTER TABLE [dbo].[ProtocolLinks] ADD [LastChangedDate] datetimeoffset(7) null
GO
ALTER TABLE [dbo].[ProtocolLinks] ADD [LastChangedUser] nvarchar(256) null
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