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

PRINT 'Creazione colonna UniqueId nullable nella tabella Message'
GO
-- 1 
ALTER TABLE [dbo].[Message] 
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
PRINT 'Aggiornamento della tabella Message'
GO
-- 2 
UPDATE [dbo].[Message]
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
ALTER TABLE [dbo].[Message]
    ADD CONSTRAINT [DF_Message_UniqueId] 
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
ALTER TABLE [dbo].[Message] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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




PRINT 'Creazione colonna UniqueId nullable nella tabella MessageAttachment'
GO
-- 1 
ALTER TABLE [dbo].[MessageAttachment] 
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
PRINT 'Aggiornamento della tabella MessageAttachment'
GO
-- 2 
UPDATE [dbo].[MessageAttachment]
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
ALTER TABLE [dbo].[MessageAttachment]
    ADD CONSTRAINT [DF_MessageAttachment_UniqueId] 
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
ALTER TABLE [dbo].[MessageAttachment] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella MessageContact'
GO
-- 1 
ALTER TABLE [dbo].[MessageContact] 
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
PRINT 'Aggiornamento della tabella MessageContact'
GO
-- 2 
UPDATE [dbo].[MessageContact]
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
ALTER TABLE [dbo].[MessageContact]
    ADD CONSTRAINT [DF_MessageContact_UniqueId] 
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
ALTER TABLE [dbo].[MessageContact] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella MessageContactEmail'
GO
-- 1 
ALTER TABLE [dbo].[MessageContactEmail] 
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
PRINT 'Aggiornamento della tabella MessageContactEmail'
GO
-- 2 
UPDATE [dbo].[MessageContactEmail]
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
ALTER TABLE [dbo].[MessageContactEmail]
    ADD CONSTRAINT [DF_MessageContactEmail_UniqueId] 
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
ALTER TABLE [dbo].[MessageContactEmail] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella MessageEmail'
GO
-- 1 
ALTER TABLE [dbo].[MessageEmail] 
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
PRINT 'Aggiornamento della tabella MessageEmail'
GO
-- 2 
UPDATE [dbo].[MessageEmail]
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
ALTER TABLE [dbo].[MessageEmail]
    ADD CONSTRAINT [DF_MessageEmail_UniqueId] 
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
ALTER TABLE [dbo].[MessageEmail] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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




PRINT 'Creazione colonna UniqueId nullable nella tabella MessageLog'
GO
-- 1 
ALTER TABLE [dbo].[MessageLog] 
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
PRINT 'Aggiornamento della tabella MessageLog'
GO
-- 2 
UPDATE [dbo].[MessageLog]
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
ALTER TABLE [dbo].[MessageLog]
    ADD CONSTRAINT [DF_MessageLog_UniqueId] 
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
ALTER TABLE [dbo].[MessageLog] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella Collaboration'
GO
-- 1 
ALTER TABLE [dbo].[Collaboration] 
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
PRINT 'Aggiornamento della tabella Collaboration'
GO
-- 2 
UPDATE [dbo].[Collaboration]
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
ALTER TABLE [dbo].[Collaboration]
    ADD CONSTRAINT [DF_Collaboration_UniqueId] 
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
ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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


PRINT 'Creazione colonna UniqueId nullable nella tabella CollaborationLog'
GO
-- 1 
ALTER TABLE [dbo].[CollaborationLog] 
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
PRINT 'Aggiornamento della tabella CollaborationLog'
GO
-- 2 
UPDATE [dbo].[CollaborationLog]
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
ALTER TABLE [dbo].[CollaborationLog]
    ADD CONSTRAINT [DF_CollaborationLog_UniqueId] 
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
ALTER TABLE [dbo].[CollaborationLog] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella Protocol'
GO
-- 1 
ALTER TABLE [dbo].[Protocol] 
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
PRINT 'Aggiornamento della tabella Protocol'
GO
-- 2 
UPDATE [dbo].[Protocol]
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
ALTER TABLE [dbo].[Protocol]
    ADD CONSTRAINT [DF_Protocol_UniqueId] 
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
ALTER TABLE [dbo].[Protocol] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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


PRINT 'Creazione colonna UniqueId nullable nella tabella Type'
GO
-- 1 
ALTER TABLE [dbo].[Type] 
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
PRINT 'Aggiornamento della tabella Type'
GO
-- 2 
UPDATE [dbo].[Type]
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
ALTER TABLE [dbo].[Type]
    ADD CONSTRAINT [DF_Type_UniqueId] 
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
ALTER TABLE [dbo].[Type] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT 'Creazione colonna UniqueId nullable nella tabella PECMail'
GO
-- 1 
ALTER TABLE [dbo].[PECMail] 
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
PRINT 'Aggiornamento della tabella PECMail'
GO
-- 2 
UPDATE [dbo].[PECMail]
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
ALTER TABLE [dbo].[PECMail]
    ADD CONSTRAINT [DF_PECMail_UniqueId] 
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
ALTER TABLE [dbo].[PECMail] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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



PRINT 'Creazione colonna UniqueId nullable nella tabella PECMailBox'
GO
-- 1 
ALTER TABLE [dbo].[PECMailBox] 
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
PRINT 'Aggiornamento della tabella PECMailBox'
GO
-- 2 
UPDATE [dbo].[PECMailBox]
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
ALTER TABLE [dbo].[PECMailBox]
    ADD CONSTRAINT [DF_PECMailBox_UniqueId] 
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
ALTER TABLE [dbo].[PECMailBox] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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