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

PRINT 'Versionamento database alla 8.54'
GO

EXEC dbo.VersioningDatabase N'8.54'
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

PRINT 'Eliminazione colonna IdProtocolCheckLog'
GO

ALTER TABLE [dbo].[Protocol] DROP CONSTRAINT [FK_Protocol_ProtocolCheckLog]
GO

ALTER TABLE [dbo].[Protocol] DROP COLUMN [IdProtocolCheckLog]
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

PRINT 'Eliminazione tabella ProtocolCheckLog'
GO

DROP TABLE [dbo].[ProtocolCheckLog]
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

PRINT 'Cancella la colonna [DestinationUser] nella tabella [CollaborationUsers]';
GO
ALTER TABLE [dbo].[CollaborationUsers] DROP COLUMN [DestinationUser]
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

PRINT 'Cancella la colonna [AttachmentStream] nella tabella [PECMailAttachment]';
GO
ALTER TABLE [dbo].[PECMailAttachment] DROP COLUMN [AttachmentStream] 
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

PRINT 'Cancello le colonne [ProtocolNumberOut] nella tabella [PECMail]';
GO

ALTER TABLE [dbo].[PECMail] DROP COLUMN [ProtocolNumberOut] 
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

PRINT 'Cancello la colonna [ProtocolNumberIn] nella tabella [PECMail]';
GO

ALTER TABLE [dbo].[PECMail] DROP COLUMN [ProtocolNumberIn] 
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

PRINT 'Cancello la colonna [ProtocolYearOut] nella tabella [PECMail]';
GO

ALTER TABLE [dbo].[PECMail] DROP COLUMN [ProtocolYearOut] 
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

PRINT 'Cancello la colonna [ProtocolYearIn] nella tabella [PECMail]';
GO

ALTER TABLE [dbo].[PECMail] DROP COLUMN [ProtocolYearIn] 
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

PRINT 'Cancella la colonna [Incremental] nella tabella [ProtocolContact]';
GO
ALTER TABLE [dbo].[ProtocolContact] DROP COLUMN [Incremental] 
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

PRINT 'Cancella la colonna [IdUser] nella tabella [DeskRoleUsers]';
GO
ALTER TABLE [dbo].[DeskRoleUsers] DROP CONSTRAINT [FK_DesksRolesUsers_SecurityUsers]
GO
ALTER TABLE [dbo].[DeskRoleUsers] DROP CONSTRAINT [CK_DeskRoleUsers]
GO
ALTER TABLE [dbo].[DeskRoleUsers] DROP COLUMN [IdUser]
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

PRINT 'Aggiungo la colonna [Language] nella tabella [Contact]';
GO

ALTER TABLE [dbo].[Contact] ADD [Language] int null
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

PRINT 'Aggiungo la colonna [Nationality] nella tabella [Contact]';
GO

ALTER TABLE [dbo].[Contact] ADD [Nationality] nvarchar(256) null
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

PRINT 'Aggiungo la colonna [Language] nella tabella [ProtocolContactManual]';
GO

ALTER TABLE [dbo].[ProtocolContactManual] ADD [Language] int null
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

PRINT 'Aggiungo la colonna [Nationality] nella tabella [ProtocolContactManual]';
GO

ALTER TABLE [dbo].[ProtocolContactManual] ADD [Nationality] nvarchar(256) null
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

PRINT 'Aggiungo la colonna [Language] nella tabella [ProtocolContactManual]';
GO

ALTER TABLE [dbo].[TemplateProtocolContactManual] ADD [Language] int null
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

PRINT 'Aggiungo la colonna [Nationality] nella tabella [ProtocolContactManual]';
GO

ALTER TABLE [dbo].[TemplateProtocolContactManual] ADD [Nationality] nvarchar(256) null
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

PRINT 'Modificata colonna RegistrationUser in Category';
GO

ALTER TABLE [dbo].[Category] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Category';
GO

ALTER TABLE [dbo].[Category] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in CategoryGroup';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in CategoryGroup';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in Contact';
GO

ALTER TABLE [dbo].[Contact] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Contact';
GO

ALTER TABLE [dbo].[Contact] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in Container';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Container';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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


PRINT 'Modificata colonna RegistrationUser in ContainerGroup';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in ContainerGroup';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in RoleGroup';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in RoleGroup';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in DeskDocuments';
GO

ALTER TABLE [dbo].[DeskDocuments] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in DeskDocuments';
GO

ALTER TABLE [dbo].[DeskDocuments] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in DeskDocumentVersions';
GO

ALTER TABLE [dbo].[DeskDocumentVersions] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in DeskDocumentVersions';
GO

ALTER TABLE [dbo].[DeskDocumentVersions] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in DeskStoryBoards';
GO

ALTER TABLE [dbo].[DeskStoryBoards] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in DeskStoryBoards';
GO

ALTER TABLE [dbo].[DeskStoryBoards] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in DocumentSeriesItem';
GO

ALTER TABLE [dbo].[DocumentSeriesItem] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in DocumentSeriesItem';
GO

ALTER TABLE [dbo].[DocumentSeriesItem] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in DocumentSeriesItemLinks';
GO

ALTER TABLE [dbo].[DocumentSeriesItemLinks] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in DocumentSeriesItemLinks';
GO

ALTER TABLE [dbo].[DocumentSeriesItemLinks] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in PECMailBoxUsers';
GO

ALTER TABLE [dbo].[PECMailBoxUsers] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in PECMailBoxUsers';
GO

ALTER TABLE [dbo].[PECMailBoxUsers] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna Note in ProtocolRole'
GO

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [Note] [nvarchar](max) NULL
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

PRINT 'Modificata function [GetDiarioUnificatoDetails] '
GO

ALTER FUNCTION [dbo].[GetDiarioUnificatoDetails](@IdTipologia INT, @DataDal DATETIME,  @DataAl DATETIME, @User NVARCHAR(256), @Riferimento1 AS INT = NULL, @Riferimento2 AS INT = NULL,  @Riferimento3 AS UNIQUEIDENTIFIER = NULL)
RETURNS TABLE 
AS
RETURN 
(
	SELECT O.IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber,
	L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[CollaborationLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Collaborazione')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Collaborazione') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.LogDate BETWEEN @DataDal AND @DataAl	
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IdCollaboration = ISNULL(@Riferimento1, O.IdCollaboration) )
		
	UNION 

	SELECT CAST(NULL AS INT) AS IdCollaboration, O.IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber,
	L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[DocumentSeriesItemLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Serie Documentali/Archivi')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Serie Documentali/Archivi') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.LogDate BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IdDocumentSeriesItem = ISNULL(@Riferimento1, O.IdDocumentSeriesItem) )
		
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, O.IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber,
	L.IdLogKind as [Type], LogDate, CAST(O.[Type] AS VARCHAR(256)) AS LogType, O.[Description] AS LogDescription, O.SystemUser AS [User], O.Severity,
	CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[MessageLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Posta Elettronica')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Posta Elettronica') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IDMessage = ISNULL(@Riferimento1, O.IDMessage) )

	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, O.IDMail AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber,
	L.IdLogKind as [Type], O.[Date] AS LogDate, O.[Type] AS LogType, O.[Description] As LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[LogKind] L
	INNER JOIN [dbo].[PECMailLog] O ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'PEC')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'PEC') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[Date] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IDMail = ISNULL(@Riferimento1, O.IDMail) )
	
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, [Year] AS ProtYear, Number AS ProtNumber,
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)) COLLATE SQL_Latin1_General_CP1_CI_AS, O.[LogDescription] COLLATE SQL_Latin1_General_CP1_CI_AS, O.SystemUser COLLATE SQL_Latin1_General_CP1_CI_AS AS [User], O.Severity,
	CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[ProtocolLog] O 
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo')
	WHERE (@IdTipologia IS NULL OR @IdTipologia = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo'))
	AND L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo')
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND ( O.[Year] = ISNULL(@Riferimento1, O.[Year]) AND O.[Number] = ISNULL(@Riferimento2, O.[Number]))
 
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, O.IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber,
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[ResolutionLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Atti')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Atti') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.IdResolution = ISNULL(@Riferimento1, O.IdResolution) )

	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	[Year] AS DocmYear, Number AS DocmNumber, CAST(NULL AS smallint) AS ProtYear, CAST(NULL AS int) AS ProtNumber,
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[DocumentLog] O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Pratiche')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Pratiche') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND ( O.[Year] = ISNULL(@Riferimento1, O.[Year]) AND O.[Number] = ISNULL(@Riferimento2, O.[Number]) )

	UNION

	SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, ProtYear, ProtNumber,
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.[User] AS [User], O.Severity, UDSId, IdUDSRepository
	FROM (
		SELECT O.IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
		CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber,
		L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
		FROM [dbo].[CollaborationLog] O
		INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma')
		WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma') = @IdTipologia)
		AND O.SystemUser = @User
		AND O.LogDate BETWEEN @DataDal AND @DataAl	
		AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
		AND ( O.IdCollaboration = ISNULL(@Riferimento1, O.IdCollaboration) )
	)O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma') = @IdTipologia)
	AND O.[User] = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)	
)
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

PRINT 'Modificata function [GetDiarioUnificatoDetailsFiltered] '
GO

ALTER FUNCTION [dbo].[GetDiarioUnificatoDetailsFiltered](@IdTipologia INT, @DataDal DATETIME,  @DataAl DATETIME, @User NVARCHAR(256), @Riferimento1 AS INT = NULL, @Riferimento2 AS INT = NULL, @Subject NVARCHAR(MAX), @Riferimento3 AS UNIQUEIDENTIFIER = NULL)
RETURNS TABLE 
AS
RETURN 
(
	SELECT O.IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber, 
	L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[CollaborationLog] O
	INNER JOIN [dbo].[Collaboration] E ON E.IdCollaboration = O.IdCollaboration 
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Collaborazione')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Collaborazione') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.LogDate BETWEEN @DataDal AND @DataAl	
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[Object] LIKE '%'+@Subject+'%'
	AND ( O.IdCollaboration = ISNULL(@Riferimento1, O.IdCollaboration) )
		
	UNION 

	SELECT CAST(NULL AS INT) AS IdCollaboration, O.IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber, 
	L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[DocumentSeriesItemLog] O
	INNER JOIN [dbo].[DocumentSeriesItem] E ON E.Id = O.IdDocumentSeriesItem
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Serie Documentali/Archivi')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Serie Documentali/Archivi') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.LogDate BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[Subject] LIKE '%'+@Subject+'%'
	AND ( O.IdDocumentSeriesItem = ISNULL(@Riferimento1, O.IdDocumentSeriesItem) )
		
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, O.IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber, 
	L.IdLogKind as [Type], LogDate, CAST(O.[Type] AS VARCHAR(256)) AS LogType, O.[Description] AS LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[MessageLog] O
	INNER JOIN [dbo].[Message] E ON E.IDMessage = O.IDMessage 
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Posta Elettronica')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Posta Elettronica') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND (SELECT TOP 1 M.Subject FROM [dbo].[MessageEmail] M WHERE M.IDMessage = O.IDMessage) LIKE '%'+@Subject+'%'
	AND ( O.IDMessage = ISNULL(@Riferimento1, O.IDMessage) )

	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, O.IDMail AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber, 
	L.IdLogKind as [Type], O.[Date] AS LogDate, O.[Type] AS LogType, O.[Description] As LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[LogKind] L
	INNER JOIN [dbo].[PECMailLog] O ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'PEC')
	INNER JOIN [dbo].[PECMail] E ON E.IDPECMail = O.IDMail
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'PEC') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[Date] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[MailSubject] LIKE '%'+@Subject+'%'
	AND ( O.IDMail = ISNULL(@Riferimento1, O.IDMail) )
	
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, O.[Year] AS ProtYear, O.Number AS ProtNumber,
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)) COLLATE SQL_Latin1_General_CP1_CI_AS, O.[LogDescription] COLLATE SQL_Latin1_General_CP1_CI_AS, O.SystemUser COLLATE SQL_Latin1_General_CP1_CI_AS AS [User], O.Severity,
	CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL aS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[ProtocolLog] O 
	INNER JOIN [dbo].[Protocol] E ON E.[Year] = O.[Year] AND E.[Number] = O.[Number]
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo')
	WHERE (@IdTipologia IS NULL OR @IdTipologia = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo'))
	AND L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Protocollo')
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND E.[Object] LIKE '%'+@Subject+'%'
	AND ( O.[Year] = ISNULL(@Riferimento1, O.[Year]) AND O.[Number] = ISNULL(@Riferimento2, O.[Number]))
 
	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, O.IdResolution, 
	CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber, 
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL AS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[ResolutionLog] O
	INNER JOIN [dbo].[Resolution] E ON E.idResolution = O.idResolution 
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Atti')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Atti') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[Object] LIKE '%'+@Subject+'%'
	AND ( O.IdResolution = ISNULL(@Riferimento1, O.IdResolution) )

	UNION

	SELECT CAST(NULL AS INT) AS IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) AS IDMessage, CAST(NULL AS INT) AS IDPECMail, CAST(NULL AS INT) IdResolution, 
	O.[Year] AS DocmYear, O.Number AS DocmNumber, CAST(NULL AS smallint) AS ProtYear, CAST(NULL AS int) AS ProtNumber, 
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL aS UNIQUEIDENTIFIER) IdUDSRepository
	FROM [dbo].[DocumentLog] O
	INNER JOIN [dbo].[Document] E ON E.[Year] = O.[Year] AND E.[Number] = O.[Number]
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Pratiche')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Pratiche') = @IdTipologia)
	AND O.SystemUser = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
	AND E.[Object] LIKE '%'+@Subject+'%'
	AND ( O.[Year] = ISNULL(@Riferimento1, O.[Year]) AND O.[Number] = ISNULL(@Riferimento2, O.[Number]) )

	UNION

	SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, ProtYear, ProtNumber, 
	L.IdLogKind as [Type], O.[LogDate], CAST(O.[LogType] AS VARCHAR(256)), O.[LogDescription], O.[User] AS [User], O.Severity, UDSId, IdUDSRepository
	FROM (
		SELECT O.IdCollaboration, CAST(NULL AS INT) IdDocumentSeriesItem, CAST(NULL AS INT) IDMessage, CAST(NULL AS INT) IDPECMail, CAST(NULL AS INT) IdResolution, 
		CAST(NULL AS smallint) AS DocmYear, CAST(NULL AS int) AS DocmNumber, CAST(NULL AS smallint) AS ProtYear,CAST(NULL AS int) AS ProtNumber, 
		L.IdLogKind as [Type], LogDate, LogType, LogDescription, O.SystemUser AS [User], O.Severity, CAST(NULL AS UNIQUEIDENTIFIER) UDSId, CAST(NULL aS UNIQUEIDENTIFIER) IdUDSRepository
		FROM [dbo].[CollaborationLog] O
		INNER JOIN [dbo].[Collaboration] E ON E.IdCollaboration = O.IdCollaboration 
		INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma')
		WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma') = @IdTipologia)
		AND O.SystemUser = @User
		AND O.LogDate BETWEEN @DataDal AND @DataAl	
		AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)
		AND E.[Object] LIKE '%'+@Subject+'%'
		AND ( O.IdCollaboration = ISNULL(@Riferimento1, O.IdCollaboration) )
	)O
	INNER JOIN [dbo].[LogKind] L ON L.IdLogKind = (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma')
	WHERE (@IdTipologia IS NULL OR (SELECT TOP 1 IdLogKind FROM [LogKind] WHERE [Description] = 'Firma') = @IdTipologia)
	AND O.[User] = @User
	AND O.[LogDate] BETWEEN @DataDal AND @DataAl
	AND L.IdLogKind = ISNULL(@IdTipologia, L.IdLogKind)	
)
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

PRINT 'Modificata store procedure [GetDiarioUnificatoDettaglio] '
GO

ALTER PROCEDURE [dbo].[GetDiarioUnificatoDettaglio](@IdTipologia INT, @DataDal DATETIME,  @DataAl DATETIME, @User NVARCHAR(256), @Riferimento1 AS INT = NULL, @Riferimento2 AS INT = NULL, @Riferimento3 AS UNIQUEIDENTIFIER = NULL)
AS
BEGIN
	SET NOCOUNT ON;

	-- Dettaglio
	SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear,DocmNumber,ProtYear,ProtNumber,
	[Type],LogDate,LogType,LogDescription,[User],Severity, UDSId, IdUDSRepository, ROW_NUMBER() OVER(ORDER BY [LogDate] DESC) AS [Id]
	FROM dbo.GetDiarioUnificatoDetails(@IdTipologia,@DataDal,@DataAl,@User, @Riferimento1, @Riferimento2, @Riferimento3)
END

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

PRINT 'Modificata store procedure [GetDiarioUnificatoTestata] '
GO

ALTER PROCEDURE [dbo].[GetDiarioUnificatoTestata](@IdTipologia INT = NULL, @DataDal DATETIME,  @DataAl DATETIME, @User NVARCHAR(256), @Subject NVARCHAR(MAX) = NULL)
AS
BEGIN
	SET NOCOUNT ON;
	IF @Subject IS NULL 
		BEGIN 
			-- Testate
			SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber,ProtYear, ProtNumber,
			[Type],LogDate,LogType,LogDescription,[User],Severity, UDSId, IdUDSRepository, ROW_NUMBER() OVER(ORDER BY [LogDate] DESC, [Type]) AS [Id]
			FROM 
			(
				SELECT *, ROW_NUMBER() OVER(PARTITION BY [Type], IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, ProtYear, ProtNumber, UDSId, IdUDSRepository ORDER BY LogDate DESC ) AS [Ranking]
				FROM dbo.GetDiarioUnificatoDetails(@IdTipologia,@DataDal,@DataAl,@User, NULL, NULL, NULL)
			)A	
			WHERE  A.Ranking = 1
		END
	ELSE
		BEGIN
			-- Testate
		SELECT IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber,ProtYear, ProtNumber,
		[Type],LogDate,LogType,LogDescription,[User],Severity, UDSId, IdUDSRepository, ROW_NUMBER() OVER(ORDER BY [LogDate] DESC, [Type]) AS [Id]
		FROM 
		(
			SELECT *, ROW_NUMBER() OVER(PARTITION BY [Type], IdCollaboration, IdDocumentSeriesItem, IDMessage, IDPECMail, IdResolution, DocmYear, DocmNumber, ProtYear, ProtNumber, UDSId, IdUDSRepository ORDER BY LogDate DESC ) AS [Ranking]
			FROM dbo.GetDiarioUnificatoDetailsFiltered(@IdTipologia,@DataDal,@DataAl,@User, NULL, NULL, @Subject, null)
		)A
		WHERE A.Ranking = 1

		END
END
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

PRINT N'Migration column [RegistrationUser], [RegistrationDate] in [dbo].[FascicleProtocols]';
GO

UPDATE [dbo].[FascicleProtocols] SET RegistrationUser = F.RegistrationUser
FROM [dbo]. [Fascicles] F
INNER JOIN [dbo].[FascicleProtocols] FP ON FP.IdFascicle = F.IdFascicle
WHERE FP.RegistrationUser IS NULL
GO

UPDATE [dbo].[FascicleProtocols] SET RegistrationDate = F.RegistrationDate
FROM [dbo]. [Fascicles] F
INNER JOIN [dbo].[FascicleProtocols] FP ON FP.IdFascicle = F.IdFascicle
WHERE FP.RegistrationDate IS NULL
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