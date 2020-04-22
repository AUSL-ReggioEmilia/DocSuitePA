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
PRINT 'Versionamento database alla 8.62'
GO

EXEC dbo.VersioningDatabase N'8.62'
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
PRINT N'Modificata la colonna[ZipCode] della tabella [dbo].[Contact] ';
GO

ALTER TABLE [dbo].[Contact] ALTER COLUMN [ZipCode] char(20) NULL
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
PRINT N'Creazione della tabella [dbo].[MetadataRepositories]';
GO

CREATE TABLE [dbo].[MetadataRepositories](

	[IdMetadataRepository] [UniqueIdentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Status] [smallint] NOT NULL,
	[Version] [int] NOT NULL, 
	[JsonMetadata] [nvarchar](4000) NOT NULL,
	[DateFrom] [datetimeoffset](7) NOT NULL,
	[DateTo] [datetimeoffset](7) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_MetadataRepositories] PRIMARY KEY NONCLUSTERED 
(
	[IdMetadataRepository] ASC
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
PRINT N'Aggiunta dell indice CLUSTERED alla colonna [RegistrationDate] della tabella [dbo].[MetadataRepositories]';
GO

CREATE CLUSTERED INDEX [IX_MetadataRepositories_RegistrationDate]   
    ON [dbo].[MetadataRepositories] ([RegistrationDate] ASC);	
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
PRINT N'Creazione della colonna [MetadataRepositories] della tabella [dbo].[Fascicles]';
GO

ALTER TABLE [dbo].[Fascicles] ADD [IdMetadataRepository] [UniqueIdentifier] NULL;
GO

ALTER TABLE [dbo].[Fascicles]  WITH CHECK ADD  CONSTRAINT [FK_Fascicles_MetadataRepositories] FOREIGN KEY([IdMetadataRepository])
REFERENCES [dbo].[MetadataRepositories] ([IdMetadataRepository])
GO

ALTER TABLE [dbo].[Fascicles] CHECK CONSTRAINT [FK_Fascicles_MetadataRepositories]
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