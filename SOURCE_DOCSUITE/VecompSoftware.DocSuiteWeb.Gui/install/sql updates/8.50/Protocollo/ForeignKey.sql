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

PRINT 'Creazione ForeignKey nella tabella MessageAttachment'
GO

ALTER TABLE [dbo].[MessageAttachment] WITH CHECK
  ADD CONSTRAINT [FK_MessageAttachment_Message] 
	FOREIGN KEY ([IDMessage]) 
	REFERENCES [dbo].[Message] ([IDMessage]);
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

PRINT 'Creazione ForeignKey nella tabella MessageContact'
GO

ALTER TABLE [dbo].[MessageContact] WITH CHECK
  ADD CONSTRAINT [FK_MessageContact_Message] 
	FOREIGN KEY ([IDMessage]) 
	REFERENCES [dbo].[Message] ([IDMessage]);
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

PRINT 'Creazione ForeignKey nella tabella MessageContactEmail'
GO

ALTER TABLE [dbo].[MessageContactEmail] WITH CHECK
  ADD CONSTRAINT [FK_MessageContactEmail_MessageContact] 
	FOREIGN KEY ([IDMessageContact]) 
	REFERENCES [dbo].[MessageContact] ([IDMessageContact]);
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


PRINT 'Creazione ForeignKey nella tabella MessageEmail'
GO

ALTER TABLE [dbo].[MessageEmail] WITH CHECK
  ADD CONSTRAINT [FK_MessageEmail_Message] 
	FOREIGN KEY ([IDMessage]) 
	REFERENCES [dbo].[Message] ([IDMessage]);
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


PRINT 'Creazione ForeignKey nella tabella MessageLog'
GO

ALTER TABLE [dbo].[MessageLog] WITH CHECK
  ADD CONSTRAINT [FK_MessageLog_Message] 
	FOREIGN KEY ([IDMessage]) 
	REFERENCES [dbo].[Message] ([IDMessage]);
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

PRINT 'Conversione IdCollaboration nella tabella CollaborationLog'
GO

ALTER TABLE [dbo].[CollaborationLog] ALTER COLUMN [IdCollaboration] NUMERIC(10,0) null
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

PRINT 'Creazione ForeignKey nella tabella CollaborationLog'
GO

ALTER TABLE [dbo].[CollaborationLog] WITH CHECK
  ADD CONSTRAINT [FK_CollaborationLog_Collaboration] 
	FOREIGN KEY ([IdCollaboration]) 
	REFERENCES [dbo].[Collaboration] ([IdCollaboration]);
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

PRINT 'Creazione ForeignKey nella tabella Role'
GO

ALTER TABLE [dbo].[Role] WITH CHECK
  ADD CONSTRAINT [FK_Role_DocmLocation_Location] 
	FOREIGN KEY ([DocmLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione ForeignKey nella tabella Role'
GO

ALTER TABLE [dbo].[Role] WITH CHECK
  ADD CONSTRAINT [FK_Role_ProtLocation_Location] 
	FOREIGN KEY ([ProtLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione ForeignKey nella tabella Role'
GO

ALTER TABLE [dbo].[Role] WITH CHECK
  ADD CONSTRAINT [FK_Role_ReslLocation_Location] 
	FOREIGN KEY ([ReslLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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

PRINT 'Creazione ForeignKey nella tabella SecurityGroups'
GO

ALTER TABLE [dbo].[SecurityGroups] WITH CHECK
  ADD CONSTRAINT [FK_SecurityGroups_SecurityGroupsFather] 
	FOREIGN KEY ([idGroupFather]) 
	REFERENCES [dbo].[SecurityGroups] ([idGroup]);
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

PRINT 'Creazione ForeignKey nella tabella SecurityUsers'
GO

ALTER TABLE [dbo].[SecurityUsers] WITH CHECK
  ADD CONSTRAINT [FK_SecurityUsers_SecurityGroups] 
	FOREIGN KEY ([idGroup]) 
	REFERENCES [dbo].[SecurityGroups] ([idGroup]);
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


PRINT 'Creazione ForeignKey nella tabella CollaborationUsers'
GO

ALTER TABLE [dbo].[CollaborationUsers] WITH CHECK
  ADD CONSTRAINT [FK_CollaborationUsers_Role] 
	FOREIGN KEY ([idRole]) 
	REFERENCES [dbo].[Role] ([idRole]);
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

PRINT 'Creazione ForeignKey nella tabella Container'
GO

ALTER TABLE [dbo].[Container] WITH CHECK
  ADD CONSTRAINT [FK_Container_ProtLocation_Location] 
	FOREIGN KEY ([ProtLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione ForeignKey nella tabella Container'
GO

ALTER TABLE [dbo].[Container] WITH CHECK
  ADD CONSTRAINT [FK_Container_DocmLocation_Location] 
	FOREIGN KEY ([DocmLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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

PRINT 'Creazione ForeignKey nella tabella Container'
GO

ALTER TABLE [dbo].[Container] WITH CHECK
  ADD CONSTRAINT [FK_Container_ReslLocation_Location] 
	FOREIGN KEY ([ReslLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione PrimaryKey nella tabella ProtocolCheckLog'
GO

ALTER TABLE [dbo].[ProtocolCheckLog] WITH CHECK
  ADD CONSTRAINT [PK_ProtocolCheckLog] 
	PRIMARY KEY CLUSTERED 
	([IdProtocolCheckLog]);
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


PRINT 'Creazione PrimaryKey nella tabella ProtocolJournalLog'
GO

ALTER TABLE [dbo].[ProtocolJournalLog] WITH CHECK
  ADD CONSTRAINT [PK_ProtocolJournalLog] 
	PRIMARY KEY CLUSTERED 
	([IdProtocolJournalLog]);
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

PRINT 'Creazione ForeignKey nella tabella Protocol'
GO

ALTER TABLE [dbo].[Protocol] WITH CHECK
  ADD CONSTRAINT [FK_Protocol_ProtocolCheckLog] 
	FOREIGN KEY ([IdProtocolCheckLog]) 
	REFERENCES [dbo].[ProtocolCheckLog] ([IdProtocolCheckLog]);
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

PRINT 'Creazione ForeignKey nella tabella Protocol'
GO

ALTER TABLE [dbo].[Protocol] WITH CHECK
  ADD CONSTRAINT [FK_Protocol_ProtocolJournalLog] 
	FOREIGN KEY ([IdProtocolJournalLog]) 
	REFERENCES [dbo].[ProtocolJournalLog] ([IdProtocolJournalLog]);
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


PRINT 'Creazione ForeignKey nella tabella PECMail'
GO

ALTER TABLE [dbo].[PECMail] WITH CHECK
  ADD CONSTRAINT [FK_PECMail_Location] 
	FOREIGN KEY ([IDLocation]) 
	REFERENCES [dbo].[Location] ([idLocation]);
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

PRINT 'Creazione ForeignKey nella tabella PECMailBox'
GO

ALTER TABLE [dbo].[PECMailBox] WITH CHECK
  ADD CONSTRAINT [FK_PECMailBox_Location] 
	FOREIGN KEY ([idLocation]) 
	REFERENCES [dbo].[Location] ([idLocation]);
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

PRINT 'Creazione ForeignKey nella tabella OChartItem'
GO

ALTER TABLE [dbo].[OChartItem] WITH CHECK
  ADD CONSTRAINT [FK_OChartItem_OChartItemParent] 
	FOREIGN KEY ([IdParent]) 
	REFERENCES [dbo].[OChartItem] ([Id]);
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