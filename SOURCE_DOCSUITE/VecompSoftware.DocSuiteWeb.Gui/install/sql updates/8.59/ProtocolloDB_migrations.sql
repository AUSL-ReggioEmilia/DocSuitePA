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
PRINT 'Versionamento database alla 8.59'
GO

EXEC dbo.VersioningDatabase N'8.59'
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
PRINT 'Creata tabella [ContactLists]';
GO

CREATE TABLE [dbo].[ContactLists](
	[IdContactList] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[TimeStamp] [timestamp] NOT NULL
CONSTRAINT [PK_ContactLists] PRIMARY KEY NONCLUSTERED 
(
	[IdContactList] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

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

PRINT 'Aggiunto clustered indice [IX_ContactLists_RegistrationDate]';
GO

CREATE CLUSTERED INDEX [IX_ContactLists_RegistrationDate]
    ON [dbo].[ContactLists]([RegistrationDate] ASC);
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

PRINT 'Creata tabella [ContactContactLists]';
GO

CREATE TABLE [dbo].[ContactContactLists](
	[IdContactContactList] [uniqueidentifier] NOT NULL,
	[IdContact] [int] NOT NULL,
	[IdContactList] [uniqueidentifier] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_ContactContactLists] PRIMARY KEY NONCLUSTERED 
(
	[IdContactContactList] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ContactContactLists] ADD  CONSTRAINT [DF_ContactContactLists_IdContactContactList]  DEFAULT (newsequentialid()) FOR [IdContactContactList]
GO

ALTER TABLE [dbo].[ContactContactLists] ADD  CONSTRAINT [DF_ContactContactLists_RegistrationDate]  DEFAULT (getdate()) FOR [RegistrationDate]
GO

ALTER TABLE [dbo].[ContactContactLists]  WITH CHECK ADD  CONSTRAINT [FK_ContactContactLists_Contact] FOREIGN KEY([IdContact])
REFERENCES [dbo].[Contact] ([Incremental])
GO

ALTER TABLE [dbo].[ContactContactLists] CHECK CONSTRAINT [FK_ContactContactLists_Contact]
GO

ALTER TABLE [dbo].[ContactContactLists]  WITH CHECK ADD  CONSTRAINT [FK_ContactContactLists_ContactLists] FOREIGN KEY([IdContactList])
REFERENCES [dbo].[ContactLists] ([IdContactList])
GO

ALTER TABLE [dbo].[ContactContactLists] CHECK CONSTRAINT [FK_ContactContactLists_ContactLists]
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

PRINT 'Modifica LogType di ProtocolLog per i log di accettazione/rifiuto';
GO

UPDATE [ProtocolLog] SET [LogType] = 'AR' 
WHERE [LogDescription] like '%- Settore rifiutato%' or [LogDescription] like '%- Settore accettato%'
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