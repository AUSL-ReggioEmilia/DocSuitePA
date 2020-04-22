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
PRINT 'Versionamento database alla 8.75'
GO

EXEC dbo.VersioningDatabase N'8.75'
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
PRINT N'Modificata colonna [IdFascicleFolder] nella tabella [dbo].[FascicleDocuments]'
GO

ALTER TABLE [dbo].[FascicleDocuments] ALTER COLUMN [IdFascicleFolder] uniqueidentifier NOT NULL
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
PRINT N'Modificata colonna [IdFascicleFolder] nella tabella [dbo].[FascicleDocumentSeriesItems]'
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] ALTER COLUMN [IdFascicleFolder] uniqueidentifier NOT NULL
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
PRINT N'Creato indice IX_FascicleDocumentSeriesItems_IdFascicle_UniqueIdDocumentSeriesItem_IdFascicleFolder in tabella [dbo].[FascicleDocumentSeriesItems]'
GO

CREATE UNIQUE INDEX IX_FascicleDocumentSeriesItems_IdFascicle_UniqueIdDocumentSeriesItem_IdFascicleFolder
ON [dbo].[FascicleDocumentSeriesItems] ([IdFascicle],[UniqueIdDocumentSeriesItem],[IdFascicleFolder])
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
PRINT N'Modificata colonna [IdFascicleFolder] nella tabella [dbo].[FascicleProtocols]'
GO

ALTER TABLE [dbo].[FascicleProtocols] ALTER COLUMN [IdFascicleFolder] uniqueidentifier NOT NULL
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
PRINT N'Creato indice IX_FascicleProtocols_IdFascicle_UniqueIdProtocol_IdFascicleFolder in tabella [dbo].[FascicleProtocols]'
GO

CREATE UNIQUE INDEX IX_FascicleProtocols_IdFascicle_UniqueIdProtocol_IdFascicleFolder
ON [dbo].[FascicleProtocols] ([IdFascicle],[UniqueIdProtocol],[IdFascicleFolder])
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
PRINT N'Modificata colonna [IdFascicleFolder] nella tabella [dbo].[FascicleResolutions]'
GO

ALTER TABLE [dbo].[FascicleResolutions] ALTER COLUMN [IdFascicleFolder] uniqueidentifier NOT NULL
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
PRINT N'Creato indice IX_FascicleResolutions_IdFascicle_UniqueIdResolution_IdFascicleFolder in tabella [dbo].[FascicleResolutions]'
GO

CREATE UNIQUE INDEX IX_FascicleResolutions_IdFascicle_UniqueIdResolution_IdFascicleFolder
ON [dbo].[FascicleResolutions] ([IdFascicle],[UniqueIdResolution],[IdFascicleFolder])
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
PRINT N'Modificata colonna [IdFascicleFolder] nella tabella [dbo].[FascicleUDS]'
GO

ALTER TABLE [dbo].[FascicleUDS] ALTER COLUMN [IdFascicleFolder] uniqueidentifier NOT NULL
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
PRINT N'Creato indice IX_FascicleUDS_IdFascicle_IdUDSRepository_IdUDS_IdFascicleFolder in tabella [dbo].[FascicleUDS]'
GO

CREATE UNIQUE INDEX IX_FascicleUDS_IdFascicle_IdUDSRepository_IdUDS_IdFascicleFolder
ON [dbo].[FascicleUDS] ([IdFascicle],[IdUDSRepository],[IdUDS],[IdFascicleFolder])
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
PRINT N'ALTER TABLE [dbo].[Role] DROP COLUMN [DocmLocation]'
GO
ALTER TABLE [dbo].[Role] DROP CONSTRAINT [FK_Role_DocmLocation_Location]
GO
ALTER TABLE [dbo].[Role] DROP COLUMN [DocmLocation] 
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
PRINT N'ALTER TABLE [dbo].[Role] DROP COLUMN [ReslLocation]'
GO
ALTER TABLE [dbo].[Role] DROP CONSTRAINT [FK_Role_ReslLocation_Location]
GO
ALTER TABLE [dbo].[Role] DROP COLUMN [ReslLocation] 
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
PRINT N'ALTER TABLE [dbo].[Role] DROP COLUMN [ProtLocation]'
GO
ALTER TABLE [dbo].[Role] DROP CONSTRAINT [FK_Role_ProtLocation_Location]
GO
ALTER TABLE [dbo].[Role] DROP COLUMN [ProtLocation] 
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
PRINT N'ALTER TABLE [dbo].[ProtocolLog] ALTER COLUMN [Hash]'
GO

ALTER TABLE [dbo].[ProtocolLog] ALTER COLUMN [Hash] [nchar](64) NOT NULL
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
PRINT N'ALTER TABLE [dbo].[DocumentSerisItemLog] ALTER COLUMN [Hash]'
GO


ALTER TABLE [dbo].[DocumentSeriesItemLog] ALTER COLUMN [Hash] [nchar](64) NOT NULL
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
PRINT N'ALTER TABLE [dbo].[TableLog] ALTER COLUMN [Hash]'
GO

ALTER TABLE [dbo].[TableLog] ALTER COLUMN [Hash] [nchar](64) NOT NULL
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
PRINT N'ALTER TABLE uds.UDSLogs ALTER COLUMN [Hash]'
GO


ALTER TABLE uds.UDSLogs ALTER COLUMN [Hash] [nchar](64) NOT NULL
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
PRINT N'ALTER TABLE [dbo].[PECMailLog] ALTER COLUMN [Hash]'
GO


ALTER TABLE [dbo].[PecMailLog] ALTER COLUMN [Hash] [nchar](64) NOT NULL
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
PRINT N'ALTER TABLE [dbo].[FascicleLogs] ALTER COLUMN [Hash]'
GO

ALTER TABLE [dbo].[FascicleLogs] ALTER COLUMN [Hash] [nchar](64) NOT NULL
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
PRINT N'ALTER TABLE [dbo].[DossierLogs] ALTER COLUMN [Hash]'
GO


ALTER TABLE [dbo].[DossierLogs] ALTER COLUMN [Hash] [nchar](64) NOT NULL
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

PRINT N'Bonifica tabella CategoryFascicles'
GO

UPDATE CategoryFascicles SET dswenvironment = 0 
WHERE idcategoryfascicle IN (
SELECT idcategoryfascicle
FROM categoryfascicles c
WHERE dswenvironment = (SELECT MIN(dswenvironment) 
FROM categoryfascicles f WHERE f.idcategory = c.idcategory AND Fascicletype IN (0,1))
AND Fascicletype IN (0,1) AND dswenvironment != 0
)
GO

DELETE FROM CategoryFascicles WHERE Fascicletype IN (0,1) AND dswenvironment != 0
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
PRINT N'Creazione della tabella [dbo].[DocumentSeriesConstraints]'
GO

CREATE TABLE [dbo].[DocumentSeriesConstraints](
	[IdDocumentSeriesConstraint] [uniqueidentifier] NOT NULL,
	[IdDocumentSeries] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
CONSTRAINT [PK_DocumentSeriesConstraints] PRIMARY KEY NONCLUSTERED  
(
	[IdDocumentSeriesConstraint] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[DocumentSeriesConstraints]  WITH CHECK ADD  CONSTRAINT [FK_DocumentSeriesConstraints_DocumentSeries] FOREIGN KEY([IdDocumentSeries])
REFERENCES [dbo].[DocumentSeries] ([Id])
GO

ALTER TABLE [dbo].[DocumentSeriesConstraints] CHECK CONSTRAINT [FK_DocumentSeriesConstraints_DocumentSeries]
GO

CREATE CLUSTERED INDEX [IX_DocumentSeriesConstraints_RegistrationDate]
    ON [dbo].[DocumentSeriesConstraints]([RegistrationDate] ASC);
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
PRINT N'ALTER TABLE [dbo].[DocumentSeriesItem] ADD COLUMN [HasMainDocument]'
GO

ALTER TABLE [dbo].[DocumentSeriesItem] ADD [HasMainDocument] [bit] NULL
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
PRINT N'Creazione della tabella [dbo].[TransparentAdministrationMonitorLogs]'
GO

CREATE TABLE [dbo].[TransparentAdministrationMonitorLogs](
	[IdTransparentAdministrationMonitorLog] [uniqueidentifier] NOT NULL,
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
	[DocumentUnitName] [nvarchar](256) NOT NULL,
	[Date] [datetimeoffset](7) NOT NULL,
	[Note] [nvarchar](MAX) NULL,
	[Rating] [nvarchar](20) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
CONSTRAINT [PK_TransparentAdministrationMonitorLogs] PRIMARY KEY NONCLUSTERED  
(
	[IdTransparentAdministrationMonitorLog] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[TransparentAdministrationMonitorLogs]  WITH CHECK ADD  CONSTRAINT [FK_TransparentAdministrationMonitorLogs_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO

ALTER TABLE [dbo].[TransparentAdministrationMonitorLogs] CHECK CONSTRAINT [FK_TransparentAdministrationMonitorLogs_DocumentUnits]
GO

CREATE CLUSTERED INDEX [IX_TransparentAdministrationMonitorLogs_RegistrationDate]
    ON [dbo].[TransparentAdministrationMonitorLogs]([RegistrationDate] ASC);
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
PRINT N'ALTER TABLE [dbo].[DocumentSeriesItem] ADD COLUMN [Constraint]'
GO

ALTER TABLE [dbo].[DocumentSeriesItem] ADD [ConstraintValue] [nvarchar](256) NULL
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
PRINT N'ALTER TABLE [dbo].[TransparentAdministrationMonitorLogs] ADD [IdRole] ';
GO

ALTER TABLE [dbo].[TransparentAdministrationMonitorLogs] ADD [IdRole] [smallint] NULL
GO

ALTER TABLE [dbo].[TransparentAdministrationMonitorLogs]  WITH CHECK ADD  CONSTRAINT [FK_TransparentAdministrationMonitorLogs_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([idRole])
GO

ALTER TABLE [dbo].[TransparentAdministrationMonitorLogs] CHECK CONSTRAINT [FK_TransparentAdministrationMonitorLogs_Role]
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