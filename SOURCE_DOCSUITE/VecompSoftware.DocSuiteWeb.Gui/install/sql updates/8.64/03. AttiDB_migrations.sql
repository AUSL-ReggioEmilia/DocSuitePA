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
PRINT 'Versionamento database alla 8.64'
GO

EXEC dbo.VersioningDatabase N'8.64'
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
PRINT 'Creazione indice univoco non clustered per la colonna [UniqueId] nella tabella [Resolution]'
GO
 
CREATE UNIQUE NONCLUSTERED INDEX [IX_Resolution_UniqueId] ON [dbo].[Resolution]
(
	[UniqueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT 'Creazione della tabella [ResolutionActivities]'
GO

CREATE TABLE[dbo].[ResolutionActivities]
(
	[IdResolutionActivity] [uniqueidentifier] NOT NULL,
	[IdResolution] [int] NOT NULL, 
	[Description] [nvarchar](256) NOT NULL, 
	[Status] [smallint] NOT NULL,
	[JsonDocuments] [nvarchar] (max) NULL,
	[ActivityDate] [datetimeoffset](7) NOT NULL,
	[ActivityType] [smallint] NOT NULL,
	[WorkflowType] [char](3) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
	[UniqueIdResolution] [uniqueidentifier] NOT NULL

 CONSTRAINT [PK_IdResolutionActivity] PRIMARY KEY NONCLUSTERED 
(
	[IdResolutionActivity] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO  

CREATE CLUSTERED INDEX [IX_ResolutionActivities_RegistrationDate]   
    ON [dbo].[ResolutionActivities] ([RegistrationDate] ASC);	
	GO  


ALTER TABLE [dbo].[ResolutionActivities]  WITH CHECK ADD  CONSTRAINT [FK_ResolutionActivities_Resolution] FOREIGN KEY([IdResolution])
REFERENCES [dbo].[Resolution] ([idResolution])
GO

ALTER TABLE [dbo].[ResolutionActivities] CHECK CONSTRAINT [FK_ResolutionActivities_Resolution]
GO

ALTER TABLE [dbo].[ResolutionActivities]  WITH CHECK ADD  CONSTRAINT [FK_ResolutionActivities_Resolution_UniqueId] FOREIGN KEY([UniqueIdResolution])
REFERENCES [dbo].[Resolution] ([UniqueId])
GO

ALTER TABLE [dbo].[ResolutionActivities] CHECK CONSTRAINT [FK_ResolutionActivities_Resolution_UniqueId]
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