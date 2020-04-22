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
PRINT 'Versionamento database alla 8.57'
GO

EXEC dbo.VersioningDatabase N'8.57'
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
PRINT 'Creazione tabella [ProtocolRoleRejected]'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[ProtocolRejectedRoles](
    [UniqueId] [uniqueidentifier] NOT NULL,
	[idRole] [smallint] NOT NULL,
	[UniqueIdProtocol] [uniqueidentifier] NOT NULL,
	[Year] [smallint] NOT NULL,
	[Number] [int] NOT NULL,
	[Rights] [nvarchar](256) NULL,
	[Note] [nvarchar](max) NULL,
	[DistributionType] [varchar](2) NULL,
	[Type] [varchar](2) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,	
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
	[Status] [smallint] NOT NULL    
CONSTRAINT [PK_ProtocolRejectedRoles] PRIMARY KEY NONCLUSTERED 
(
	[UniqueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ProtocolRejectedRoles] ADD  CONSTRAINT [DF_ProtocolRejectedRoles_UniqueId]  DEFAULT (newsequentialid()) FOR [UniqueId]
GO

ALTER TABLE [dbo].[ProtocolRejectedRoles] ADD CONSTRAINT [DF_ProtocolRejectedRoles_Status] DEFAULT 0 FOR [Status]
GO

ALTER TABLE [dbo].[ProtocolRejectedRoles]  WITH CHECK ADD  CONSTRAINT [FK_ProtocolRejectedRoles_Role] FOREIGN KEY([idRole])
REFERENCES [dbo].[Role] ([idRole])
GO

ALTER TABLE [dbo].[ProtocolRejectedRoles] CHECK CONSTRAINT [FK_ProtocolRejectedRoles_Role]
GO

ALTER TABLE [dbo].[ProtocolRejectedRoles]  WITH NOCHECK ADD  CONSTRAINT [FK_ProtocolRejectedRoles_Protocol] FOREIGN KEY([Year], [Number])
REFERENCES [dbo].[Protocol] ([Year], [Number])
GO

ALTER TABLE [dbo].[ProtocolRejectedRoles] CHECK CONSTRAINT [FK_ProtocolRejectedRoles_Protocol]
GO

ALTER TABLE [dbo].[ProtocolRejectedRoles]  WITH CHECK ADD  CONSTRAINT [FK_ProtocolRejectedRoles_Protocol_UniqueId] FOREIGN KEY([UniqueIdProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])
GO

ALTER TABLE [dbo].[ProtocolRejectedRoles] CHECK CONSTRAINT [FK_ProtocolRejectedRoles_Protocol_UniqueId]
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

PRINT 'ALTER TABLE PECMailAttachment ALTER COLUMN AttachmentName nvarchar(256) not null';
GO

ALTER TABLE PECMailAttachment ALTER COLUMN AttachmentName nvarchar(256) not null
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

PRINT 'Aggiunto clustered indice [IX_ProtocolRejectedRoles_RegistrationDate]';
GO

CREATE CLUSTERED INDEX [IX_ProtocolRejectedRoles_RegistrationDate]
    ON [dbo].[ProtocolRejectedRoles]([RegistrationDate] ASC);
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


PRINT 'Aggiunta colonna [Status] alla tabella [dbo].[ProtocolRole]';
GO

ALTER TABLE [dbo].[ProtocolRole] ADD [Status] SMALLINT NULL
GO

UPDATE [dbo].[ProtocolRole] SET [Status] = 0
GO

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [Status] SMALLINT NOT NULL
GO

ALTER TABLE [dbo].[ProtocolRole] ADD CONSTRAINT [DF_ProtocolRole_Status] DEFAULT 0 FOR [Status]
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