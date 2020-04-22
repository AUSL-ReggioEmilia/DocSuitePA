/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<UTENTE_DEFAULT, varchar(256),>	--> Settare il nome dell'utente.
*	<DELIBERE_DESCRIPTION, varchar(256), Delibera> --> Settare la descrizione per le Delibere impostate da cliente (Valore in TabMaster).					
*	<DETERMINE_DESCRIPTION, varchar(256), Determina> --> Settare la descrizione per le Determine impostate da cliente (Valore in TabMaster).	
*   <SERIES_DESCRIPTION, varchar(256), Serie Documentali> --> Settare la descrizione per le Serie Documentali impostate da cliente (Valore del parametro ProtocolEnv.DocumentSeriesName).
*****************************************************************************************************************************************/
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
PRINT 'Versionamento database alla 8.63'
GO

EXEC dbo.VersioningDatabase N'8.63'
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
PRINT N'Creazione della colonna [IsAbsent] della tabella [dbo].[CollaborationSigns]';
GO

ALTER TABLE [dbo].[CollaborationSigns] ADD [IsAbsent] [bit] NULL;
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [UniqueId] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[ParameterEnv] ADD CONSTRAINT [DF_ParameterEnv_UniqueId] 
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

PRINT 'Aggiunta colonna [Timestamp] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT N'Creazione della colonna [RegistrationDate] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [RegistrationDate] [datetimeoffset](7) NULL;
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

PRINT N'Creazione della colonna [RegistrationUser] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [RegistrationUser] [nvarchar](256) NULL;
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

PRINT N'Creazione della colonna [LastChangedDate] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [LastChangedDate] [datetimeoffset](7) NULL;
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

PRINT N'Creazione della colonna [LastChangedUser] nella tabella [ParameterEnv]';
GO

ALTER TABLE [dbo].[ParameterEnv] ADD [LastChangedUser] [nvarchar](256) NULL;
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
PRINT N'Crea tabella [TemplateCollaborations]';
GO

CREATE TABLE [dbo].[TemplateCollaborations](
	[IdTemplateCollaboration] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar] (256) NOT NULL,
	[Status] [smallint] NOT NULL,
	[DocumentType] [nvarchar] (256) NOT NULL,
	[IdPriority] [nvarchar] (256) NOT NULL,
	[Object] [nvarchar] (4000) NULL,
	[Note] [nvarchar](4000) NULL,
	[IsLocked] [bit] NULL,
	[WSManageable] [bit] NOT NULL,
	[WSDeletable] [bit] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_TemplateCollaboration] PRIMARY KEY NONCLUSTERED 
(
	[IdTemplateCollaboration] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
PRINT N'Aggiunta dell indice CLUSTERED alla colonna [RegistrationDate] della tabella [dbo].[TemplateCollaborations]';
GO

CREATE CLUSTERED INDEX [IX_TemplateCollaborations_RegistrationDate]   
    ON [dbo].[TemplateCollaborations] ([RegistrationDate] ASC);	
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
PRINT N'Creazione della tabella [TemplateDocumentRepositories]';
GO

CREATE TABLE [dbo].[TemplateDocumentRepositories](
	[IdTemplateDocumentRepository] [uniqueidentifier] NOT NULL,
	[IdStatus] [smallint] NULL,
	[IdLocation] [smallint] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[QualityTag] [nvarchar](1000) NOT NULL,
	[Version] [int] NOT NULL,
	[Object] [nvarchar](4000) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_TemplateDocumentRepositories] PRIMARY KEY NONCLUSTERED 
(
	[IdTemplateDocumentRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TemplateDocumentRepositories] ADD  CONSTRAINT [DF_TemplateDocumentRepositories_RegistrationDate]  DEFAULT (sysdatetimeoffset()) FOR [RegistrationDate]
GO

CREATE CLUSTERED INDEX [IX_TemplateDocumentRepositories_RegistrationDate]
    ON [dbo].[TemplateDocumentRepositories]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[TemplateDocumentRepositories]  WITH CHECK ADD  CONSTRAINT [FK_TemplateDocumentRepositories_Location] FOREIGN KEY([IdLocation])
REFERENCES [dbo].[Location] ([idLocation])
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
PRINT N'Creazione della tabella [TemplateCollaborationUsers]';
GO

CREATE TABLE [dbo].[TemplateCollaborationUsers](
	[IdTemplateCollaborationUser] [uniqueidentifier] NOT NULL,
	[IdTemplateCollaboration] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NULL,
	[Account] [nvarchar] (256) NULL,
	[Incremental] [smallint] NOT NULL,
	[UserType] [int] NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[IsValid] [bit] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
	 CONSTRAINT [PK_TemplateCollaborationUsers] PRIMARY KEY NONCLUSTERED 
(
	[IdTemplateCollaborationUser] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
PRINT N'Aggiunta dell indice CLUSTERED alla colonna [RegistrationDate] della tabella [dbo].[TemplateCollaborationUsers]';
GO

CREATE CLUSTERED INDEX [IX_TemplateCollaborationUsers_RegistrationDate]   
    ON [dbo].[TemplateCollaborationUsers] ([RegistrationDate] ASC);	
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
PRINT N'[FK_TemplateCollaborationUsers_TemplateCollaborations]';
GO

ALTER TABLE [dbo].[TemplateCollaborationUsers]  WITH CHECK ADD  CONSTRAINT [FK_TemplateCollaborationUsers_TemplateCollaborations] FOREIGN KEY([IdTemplateCollaboration])
REFERENCES [dbo].[TemplateCollaborations] ([IdTemplateCollaboration])
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
PRINT N'[FK_TemplateCollaborationUsers_Role]';
GO

ALTER TABLE [dbo].[TemplateCollaborationUsers]  WITH CHECK ADD  CONSTRAINT [FK_TemplateCollaborationUsers_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([idRole])
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
PRINT N'Creazione della tabella [TemplateCollaborationDocumentRepositories]';
GO

CREATE TABLE [dbo].[TemplateCollaborationDocumentRepositories](
       [IdTemplateCollaborationDocumentRepository] [uniqueidentifier] NOT NULL,
       [IdTemplateCollaboration] [uniqueidentifier] NOT NULL,
       [IdTemplateDocumentRepository] [uniqueidentifier] NOT NULL,
       [ChainType] [smallint] NULL,
       [RegistrationUser] [nvarchar](256) NOT NULL,
       [RegistrationDate] [datetimeoffset](7) NOT NULL,
       [LastChangedUser] [nvarchar](256) NULL,
       [LastChangedDate] [datetimeoffset](7) NULL,
       [Timestamp] [timestamp] NOT NULL,
       CONSTRAINT [PK_TemplateCollaborationDocumentRepositories] PRIMARY KEY NONCLUSTERED 
(
       [IdTemplateCollaborationDocumentRepository] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
PRINT N'Aggiunta dell indice CLUSTERED alla colonna [RegistrationDate] della tabella [dbo].[TemplateCollaborationRepositoryDocuments]';
GO

CREATE CLUSTERED INDEX [IX_TemplateCollaborationDocumentRepositories_RegistrationDate]   
    ON [dbo].[TemplateCollaborationDocumentRepositories] ([RegistrationDate] ASC);	
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
PRINT N'[FK_TemplateCollaborationDocumentRepositories_TemplateCollaborations]';
GO

ALTER TABLE [dbo].[TemplateCollaborationDocumentRepositories]  WITH CHECK ADD  CONSTRAINT [FK_TemplateCollaborationDocumentRepositories_TemplateCollaborations] FOREIGN KEY([IdTemplateCollaboration])
REFERENCES [dbo].[TemplateCollaborations] ([IdTemplateCollaboration])
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
PRINT N'[FK_TemplateCollaborationDocumentRepositories_TemplateDocumentRepositories]';
GO

ALTER TABLE [dbo].[TemplateCollaborationDocumentRepositories]  WITH CHECK ADD  CONSTRAINT [FK_TemplateCollaborationDocumentRepositories_TemplateDocumentRepositories] FOREIGN KEY([IdTemplateDocumentRepository])
REFERENCES [dbo].[TemplateDocumentRepositories] ([IdTemplateDocumentRepository])
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
PRINT N'Creazione della tabella [TemplateCollaborationRoles]';
GO

CREATE TABLE [dbo].[TemplateCollaborationRoles](
	[IdTemplateCollaborationRole] [uniqueidentifier] NOT NULL,
	[IdTemplateCollaboration] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	 CONSTRAINT [PK_TemplateCollaborationRoles] PRIMARY KEY NONCLUSTERED 
(
	[IdTemplateCollaborationRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TemplateCollaborationRoles] ADD CONSTRAINT [DF_TemplateCollaborationRoles_RegistrationDate] DEFAULT (getdate()) FOR [RegistrationDate]
GO

ALTER TABLE [dbo].[TemplateCollaborationRoles] ADD CONSTRAINT [DF_TemplateCollaborationRoles_IdTemplateCollaborationRole] DEFAULT (newsequentialid()) FOR [IdTemplateCollaborationRole]
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
PRINT N'Aggiunta dell indice CLUSTERED alla colonna [RegistrationDate] della tabella [dbo].[TemplateCollaborationRoles]';
GO

CREATE CLUSTERED INDEX [IX_TemplateCollaborationRoles_RegistrationDate]   
    ON [dbo].[TemplateCollaborationRoles] ([RegistrationDate] ASC);	
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
PRINT N'[FK_TemplateCollaborationRoles_TemplateCollaborations]';
GO

ALTER TABLE [dbo].[TemplateCollaborationRoles]  WITH CHECK ADD  CONSTRAINT [FK_TemplateCollaborationRoles_TemplateCollaborations] FOREIGN KEY([IdTemplateCollaboration])
REFERENCES [dbo].[TemplateCollaborations] ([IdTemplateCollaboration])
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
PRINT N'[FK_TemplateCollaborationRoles_Role]';
GO

ALTER TABLE [dbo].[TemplateCollaborationRoles]  WITH CHECK ADD  CONSTRAINT [FK_TemplateCollaborationRoles_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([IdRole])
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
PRINT N'Inserimento Template di default nella tabella [dbo].[TemplateDocumentRepositories]';
GO

--Protocollo
INSERT INTO [dbo].[TemplateCollaborations]([IdTemplateCollaboration], [Name], [Status], [DocumentType], [IdPriority], [Object], [Note], [IsLocked], [WSManageable], [WSDeletable], [RegistrationUser], [RegistrationDate], [LastChangedUser], [LastChangedDate]) VALUES (newid(), 'Protocollo', 1, 'P', 'N', null, null, 1, 0, 0, '<UTENTE_DEFAULT, varchar(256), ''>', GETUTCDATE(), null, null)
GO
--Uoia
INSERT INTO [dbo].[TemplateCollaborations]([IdTemplateCollaboration], [Name], [Status], [DocumentType], [IdPriority], [Object], [Note], [IsLocked], [WSManageable], [WSDeletable], [RegistrationUser], [RegistrationDate], [LastChangedUser], [LastChangedDate]) VALUES (newid(), 'Uoia', 1, 'U', 'N', null, null, 1, 0, 0, '<UTENTE_DEFAULT, varchar(256), ''>', GETUTCDATE(), null, null)
GO
--Delibere
INSERT INTO [dbo].[TemplateCollaborations]([IdTemplateCollaboration], [Name], [Status], [DocumentType], [IdPriority], [Object], [Note], [IsLocked], [WSManageable], [WSDeletable], [RegistrationUser], [RegistrationDate], [LastChangedUser], [LastChangedDate]) VALUES (newid(), '<DELIBERE_DESCRIPTION, varchar(256), Delibera>', 1, 'D', 'N', null, null, 1, 0, 0, '<UTENTE_DEFAULT, varchar(256), ''>', GETUTCDATE(), null, null)
GO
--Determine
INSERT INTO [dbo].[TemplateCollaborations]([IdTemplateCollaboration], [Name], [Status], [DocumentType], [IdPriority], [Object], [Note], [IsLocked], [WSManageable], [WSDeletable], [RegistrationUser], [RegistrationDate], [LastChangedUser], [LastChangedDate]) VALUES (newid(), '<DETERMINE_DESCRIPTION, varchar(256), Determina>', 1, 'A', 'N', null, null, 1, 0, 0, '<UTENTE_DEFAULT, varchar(256), ''>', GETUTCDATE(), null, null)
GO
--Serie Documentali
INSERT INTO [dbo].[TemplateCollaborations]([IdTemplateCollaboration], [Name], [Status], [DocumentType], [IdPriority], [Object], [Note], [IsLocked], [WSManageable], [WSDeletable], [RegistrationUser], [RegistrationDate], [LastChangedUser], [LastChangedDate]) VALUES (newid(), '<SERIES_DESCRIPTION, varchar(256), Serie Documentali>', 1, 'S', 'N', null, null, 1, 0, 0, '<UTENTE_DEFAULT, varchar(256), ''>', GETUTCDATE(), null, null)
GO
--UDS
INSERT INTO [dbo].[TemplateCollaborations]([IdTemplateCollaboration], [Name], [Status], [DocumentType], [IdPriority], [Object], [Note], [IsLocked], [WSManageable], [WSDeletable], [RegistrationUser], [RegistrationDate], [LastChangedUser], [LastChangedDate]) VALUES (newid(), 'Archivi', 1, 'UDS', 'N', null, null, 1, 0, 0, '<UTENTE_DEFAULT, varchar(256), ''>', GETUTCDATE(), null, null)
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
PRINT N'Aggiunta colonna TemplateName nella tabella [dbo].[Collaboration]';
GO

ALTER TABLE [dbo].[Collaboration] ADD [TemplateName] [nvarchar](256) NULL;
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
PRINT N'Aggiunta colonna IdUDS nella tabella [dbo].[Collaboration]';
GO

ALTER TABLE [dbo].[Collaboration] ADD [IdUDS] [uniqueidentifier] NULL;
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
PRINT N'Aggiunta colonna IdUDSRepository nella tabella [dbo].[Collaboration]';
GO

ALTER TABLE [dbo].[Collaboration] ADD [IdUDSRepository] [uniqueidentifier] NULL;
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
PRINT N'Drop index [IX_Collaboration_DocumentType_IdStatus_RegistrationUser]';
GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.Collaboration') AND NAME ='IX_Collaboration_DocumentType_IdStatus_RegistrationUser')
	DROP INDEX [IX_Collaboration_DocumentType_IdStatus_RegistrationUser] ON [dbo].[Collaboration]
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
PRINT N'Modifica della colonna DocumentType nella tabella [dbo].[Collaboration]';
GO

ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [DocumentType] [nvarchar](256) NULL;
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
PRINT N'Create index [IX_Collaboration_DocumentType_IdStatus_RegistrationUser]';
GO

CREATE NONCLUSTERED INDEX [IX_Collaboration_DocumentType_IdStatus_RegistrationUser] ON [dbo].[Collaboration]
(
	[DocumentType] ASC,
	[IdStatus] ASC,
	[RegistrationUser] ASC
)
INCLUDE ( 	[IdCollaboration]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
PRINT N'Modifica valori della colonna [TemplateName] nella tabella [dbo].[Collaboration]';
GO

UPDATE [C] SET [C].[TemplateName] = [TC].[Name]
FROM [dbo].[Collaboration] C
INNER JOIN [dbo].[TemplateCollaborations] TC ON [C].[DocumentType] = [TC].[DocumentType] AND [TC].[IsLocked] = 1
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
PRINT N'Aggiorno la colonna SchemaXML della tabella UDSSchemRepositories';
GO

UPDATE [uds].[UDSSchemaRepositories] SET [SchemaXML] = '<xs:schema id="SchemaUnitaDocumentariaSpecifica"
    targetNamespace="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd"
    elementFormDefault="qualified"
    xmlns="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd"
    xmlns:mstns="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
>
  <!-- Tipi base -->
  <xs:complexType name="BaseType">
    <xs:attribute name="Label" type ="xs:token" use="required"/>
    <xs:attribute name="ClientId" type ="xs:token" use="optional"  default="0" />
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"/>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="true"/>
  </xs:complexType>

  <xs:complexType name="DynamicBaseType">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:sequence>
          <xs:element name="Layout" type="LayoutPosition" minOccurs="0" maxOccurs="1"/>
          <xs:element name="JavaScript" type="JavaScript" minOccurs="0" maxOccurs="1"/>
        </xs:sequence>
        <xs:attribute name="Required" type ="xs:boolean" use="required"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  
  <xs:complexType name="FieldBaseType">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:attribute name="ColumnName" type ="xs:token" use="required"/>
        <xs:attribute name="HiddenField" type ="xs:boolean" use="required"/>
        <xs:attribute name="Format" type="xs:token" use="optional"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Lista di documenti -->
  <xs:complexType name="Documents">
    <!--Tipo del documento:
        1) documento principale 
        2) documento allegato
        3) documento annesso
      -->
    <xs:all>
      <!--Almeno un documento deve essere inserito se viene creato un Documents-->
      <xs:element name="Document" type="Document" minOccurs="1" maxOccurs="1" />
      <xs:element name="DocumentAttachment" type="Document" minOccurs="0" maxOccurs="1" />
      <xs:element name="DocumentAnnexed" type="Document" minOccurs="0" maxOccurs="1" />
    </xs:all>
  </xs:complexType>

  <!-- Definizione del classificatore  -->
  <xs:complexType name="Category">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:attribute name="IdCategory" use="required"/>
        <xs:attribute name="DefaultEnabled" type ="xs:boolean" use="optional" default="true"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Definizione del contenitore  -->
  <xs:complexType name="Container">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:attribute name="IdContainer" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Definizione di un documento -->
  <xs:complexType name="Document">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:sequence>
          <xs:element name="Instances" type="DocumentInstance" minOccurs="0" maxOccurs="unbounded"/>
        </xs:sequence >
        <xs:attribute name="BiblosArchive" type="xs:token" use="required"/>
        <xs:attribute name="CreateBiblosArchive" type="xs:boolean" use="optional" default="false"/>
        <xs:attribute name="AllowMultiFile" type="xs:boolean" use="required"/>
        <xs:attribute name="Deletable" type="xs:boolean" use="required"/>
        <xs:attribute name="UploadEnabled" type="xs:boolean" use="optional" default="true"/>
        <xs:attribute name="ScannerEnabled" type="xs:boolean" use="optional" default="true"/>
        <xs:attribute name="SignEnabled" type="xs:boolean" use="optional" default="true"/>
        <xs:attribute name="SignRequired" type="xs:boolean" use="optional" default="false"/>
        <xs:attribute name="CopyProtocol" type="xs:boolean" use="optional" default="false"/>
        <xs:attribute name="CopyResolution" type="xs:boolean" use="optional" default="false"/>
        <xs:attribute name="CopySeries" type="xs:boolean" use="optional" default="false"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Definizione della serie documentale-->
  <xs:complexType name="DocumentInstance">
    <xs:attribute name="IdDocument"/>
    <xs:attribute name="DocumentContent" type="xs:token"/>
    <xs:attribute name="DocumentName" type="xs:token"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione delle serie documentali-->
  <xs:complexType name="DocumentSeries">
    <xs:sequence minOccurs="0">
      <xs:element name="Instances" type="DocumentSeriesInstance"/>
    </xs:sequence>
  </xs:complexType>

  <!-- Definizione della serie documentale-->
  <xs:complexType name="DocumentSeriesInstance">
    <xs:attribute name="IdDocumentSeries"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei contatti-->

  <xs:simpleType name="ContactType">
    <xs:restriction base="xs:string">
      <xs:enumeration value="Sender" />
      <xs:enumeration value="Recipient" />
    </xs:restriction>
  </xs:simpleType>

  <xs:complexType name="Contacts">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:sequence>
          <!-- Contatto -->
          <xs:element name="ContactInstances" type="ContactInstance" minOccurs="0" maxOccurs="unbounded"/>
          <!-- Contatto Manuale-->
          <xs:element name="ContactManualInstances" type="ContactManualInstance" minOccurs="0" maxOccurs="unbounded"/>
        </xs:sequence >
        <xs:attribute name="ContactType" type="ContactType" use="optional"/>
        <xs:attribute name="ADEnabled" type="xs:boolean" use="optional" default="true"/>
        <xs:attribute name="AddressBookEnabled" type="xs:boolean" use="optional" default="true"/>
        <xs:attribute name="ADDistributionListEnabled" type="xs:boolean" use="optional" default="true"/>
        <xs:attribute name="ManualEnabled" type="xs:boolean" use="optional" default="true"/>
        <xs:attribute name="ExcelImportEnabled" type="xs:boolean" use="optional" default="true"/>
        <xs:attribute name="AllowMultiContact" type="xs:boolean" use="required"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Definizione dei contatti-->
  <xs:complexType name="ContactInstance">
    <xs:attribute name="IdContact"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei contatti manuali-->
  <xs:complexType name="ContactManualInstance">
    <xs:attribute name="ContactDescription"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei Authorizations-->
  <xs:complexType name="Authorizations">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:sequence>
          <xs:element name="Instances" type="AuthorizationInstance" minOccurs="0" maxOccurs="unbounded"/>
        </xs:sequence >
        <xs:attribute name="AuthorizationType" type="AuthorizationType" use="required"/>
        <xs:attribute name="AllowMultiAuthorization" type="xs:boolean" use="required"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  
  <!-- Definizione della tipologia di autorizzazione-->
  <xs:simpleType name="AuthorizationType">
    <xs:restriction base="xs:string">
      <xs:enumeration value="Authorized" />
      <xs:enumeration value="CarbonCopy" />
    </xs:restriction>
  </xs:simpleType>
  
  <!-- Definizione di un Authorization-->
  <xs:complexType name="AuthorizationInstance">
    <xs:attribute name="IdAuthorization"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei messages-->
  <xs:complexType name="Messages">
    <xs:sequence>
      <xs:element name="Instances" type="MessageInstance" minOccurs="1" maxOccurs="unbounded"/>
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required"/>
  </xs:complexType>

  <!-- Definizione di un message-->
  <xs:complexType name="MessageInstance">
    <xs:attribute name="IdMessage"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione delle PECMails-->
  <xs:complexType name="PECMails">
    <xs:sequence>
      <xs:element name="Instances" type="PECInstance" minOccurs="1" maxOccurs="unbounded"/>
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required"/>
  </xs:complexType>

  <!-- Definizione di una pec-->
  <xs:complexType name="PECInstance">
    <xs:attribute name="IdPECMail"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei Protocols-->
  <xs:complexType name="Protocols">
    <xs:sequence>
      <xs:element name="Instances" type="ProtocolInstance" minOccurs="1" maxOccurs="unbounded"/>
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required"/>
  </xs:complexType>

  <!-- Definizione di un Protocol-->
  <xs:complexType name="ProtocolInstance">
    <xs:attribute name="IdProtocol"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei Resolutions-->
  <xs:complexType name="Resolutions">
    <xs:sequence>
      <xs:element name="Instances" type="ResolutionInstance" minOccurs="1" maxOccurs="unbounded"/>
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required"/>
  </xs:complexType>

  <!-- Definizione di un Resolution-->
  <xs:complexType name="ResolutionInstance">
    <xs:attribute name="IdResolution"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione delle Collaborations-->
  <xs:complexType name="Collaborations">
    <xs:sequence>
      <xs:element name="Instances" type="CollaborationInstance" minOccurs="1" maxOccurs="unbounded"/>
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required"/>
  </xs:complexType>

  <!-- Definizione di una Collaboration-->
  <xs:complexType name="CollaborationInstance">
    <xs:attribute name="IdCollaboration"/>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione di un campo testuale -->
  <xs:complexType name="TextField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="Multiline" type="xs:boolean" use="optional" default="false"/>
        <xs:attribute name="DefaultValue" type="xs:string" use="optional"/>
        <xs:attribute name="DefaultSearchValue" type="xs:string" use="optional"/>
        <xs:attribute name="Value" type="xs:string" use="optional"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Definizione di un campo data -->
  <xs:complexType name="DateField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultValue" type="xs:dateTime" use="optional"/>
        <xs:attribute name="DefaultSearchValue" type="xs:dateTime" use="optional"/>
        <xs:attribute name="Value" type="xs:dateTime" use="optional"/>
        <xs:attribute name="RestrictedYear" type="xs:boolean" use="optional" default="false"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Definizione di un campo booleano -->
  <xs:complexType name="BoolField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultValue" type="xs:boolean" use="optional"/>
        <xs:attribute name="DefaultSearchValue" type="xs:boolean" use="optional"/>
        <xs:attribute name="Value" type="xs:boolean" use="optional"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Definizione di un campo numerico -->
  <xs:complexType name="NumberField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultValue" type="xs:double" use="optional"/>
        <xs:attribute name="DefaultSearchValue" type="xs:double" use="optional"/>
        <xs:attribute name="Value" type="xs:double" use="optional"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Definizione di un campo enum -->

  <xs:complexType name="OptionEnum">
    <xs:sequence>
      <xs:element name="Option" type="xs:token" minOccurs="1" maxOccurs="unbounded"/>
    </xs:sequence>
  </xs:complexType>

  <xs:complexType name="EnumField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:sequence>
          <xs:element name="Options" type="OptionEnum" minOccurs="1" maxOccurs="1">
            <!--<xs:unique name="Option_unique">
              <xs:selector xpath="mstns:Option"/>
              <xs:field xpath="."/>
            </xs:unique>-->
          </xs:element>
        </xs:sequence>
        <xs:attribute name="DefaultValue" type="xs:string" use="optional"/>
        <xs:attribute name="DefaultSearchValue" type="xs:string" use="optional"/>
        <xs:attribute name="Value" type="xs:string" use="optional"/>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <!-- Definizione dei campi "BASE" di un singolo UDS -->
  <xs:complexType name="Section">
    <xs:choice minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Enum" type="EnumField" minOccurs="0"/>
      <xs:element name="Text" type="TextField" minOccurs="0"/>
      <xs:element name="Date" type="DateField" minOccurs="0"/>
      <xs:element name="Number" type="NumberField" minOccurs="0"/>
      <xs:element name="Bool" type="BoolField" minOccurs="0"/>
    </xs:choice>
    <xs:attribute name="SectionLabel" type="xs:token" use="required"/>
    <xs:attribute name="SectionId" type="xs:token" use="required"/>
  </xs:complexType>

  <xs:complexType name="Metadata">
    <xs:sequence minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Sections" type="Section" minOccurs="1" maxOccurs="unbounded">
        <xs:unique name="Section_LabelUnique">
          <xs:selector xpath="./*"/>
          <xs:field xpath="@Label"/>
        </xs:unique>
        <!--<xs:unique name="Section_ClientUnique">
          <xs:selector xpath="./*"/>
          <xs:field xpath="@ClientId"/>
        </xs:unique>-->
      </xs:element>
    </xs:sequence>
  </xs:complexType>

  <!-- Definizione dei campi "RELAZIONE" di un singolo UDS verso entità esterne -->
  <xs:complexType name="Relations">
    <xs:choice minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Contacts" type="Contacts" minOccurs="0" />
      <xs:element name="Messages" type="Messages" minOccurs="0"/>
      <xs:element name="PECMails" type="PECMails" minOccurs="0"/>
      <xs:element name="Protocols" type="Protocols" minOccurs="0"/>
      <xs:element name="Resolutions" type="Resolutions" minOccurs="0"/>
      <xs:element name="Collaborations" type="Collaborations" minOccurs="0"/>
    </xs:choice>
    <xs:attribute name="Label" type="xs:string" use="required"/>
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"/>
  </xs:complexType>

  <!-- Definizione della sezione Javascript -->
  <xs:complexType name="JavaScriptSpecification">
    <xs:sequence>
      <xs:element name="FunctionName" type="xs:token" minOccurs="1" maxOccurs="1"/>
      <xs:element name="EventType" type="JSEventType" minOccurs="1" maxOccurs="1"/>
    </xs:sequence>
  </xs:complexType>

  <xs:complexType name="JavaScript">
    <xs:sequence>
      <xs:element name="Validation" type="JavaScriptSpecification" minOccurs="0" maxOccurs="1"/>
      <xs:element name="Action" type="JavaScriptSpecification" minOccurs="0" maxOccurs="1"/>
    </xs:sequence>
  </xs:complexType>

  <xs:simpleType name="JSEventType">
    <xs:restriction base="xs:string">
      <xs:enumeration value="onchange" />
      <xs:enumeration value="onclick" />
      <xs:enumeration value="onmouseover" />
      <xs:enumeration value="onmouseout" />
      <xs:enumeration value="onkeydown" />
      <xs:enumeration value="onkeypress" />
      <xs:enumeration value="onkeyup" />
      <xs:enumeration value="onload" />
      <xs:enumeration value="onselect" />
      <xs:enumeration value="onsubmit" />
    </xs:restriction>
  </xs:simpleType>

  <!-- Definizione della sezione Layout -->

  <xs:complexType name="LayoutPosition">
    <xs:sequence>
      <xs:element name="PanelId" type="xs:token" minOccurs="1" maxOccurs="1"/>
      <xs:element name="RowNumber" type="xs:unsignedInt" minOccurs="1" maxOccurs="1"/>
      <xs:element name="CSS" type="xs:token" minOccurs="0" maxOccurs="1"/>
    </xs:sequence>
  </xs:complexType>

  <xs:simpleType name="LayoutType">
    <xs:restriction base="xs:string">
      <xs:enumeration value="GridOneColumn" />
      <xs:enumeration value="GridTwoColumn" />
      <xs:enumeration value="GridThreeColumn" />
    </xs:restriction>
  </xs:simpleType>

  <!-- In una unità documentaria sono necessari i seguenti campi:
       1) Title             -> unico per tutte le unità documentarie
       2) Subject           -> oggetto unità documentaria
       3) Category          -> classificatore
       4) Authorizations    -> diritti unità documentaria
       5) Documents-> inserisco dei documenti (1...N) principali, (1...N) annessi, (1...N) allegati
       6) Metadata -> i metadata possono essere dei campi semplici o dei collegamenti 1:N con elementi esterni.
                      DocumentSeries
                      Message
                      PEC
                      Protocol
                      Resolution
                      Pratiche
                      Contatti
  -->

  <xs:simpleType name="AliasType">
    <xs:restriction base="xs:token">
      <xs:minLength value="2" />
      <xs:maxLength value="4" />
    </xs:restriction>
  </xs:simpleType>

  <xs:element name="UnitaDocumentariaSpecifica">
    <xs:complexType>
      <xs:sequence minOccurs="1" maxOccurs="1">
        <xs:element name="Title" type="xs:string" minOccurs="1" maxOccurs="1"/>
        <xs:element name="Alias" type="AliasType" minOccurs="1" maxOccurs="1"/>
        <xs:element name="Subject" type="xs:string" minOccurs="1" maxOccurs="1"/>
        <xs:element name="Category" type="Category" minOccurs="1" maxOccurs="1"/>
        <xs:element name="Container" type="Container" minOccurs="1" maxOccurs="1"/>
        <xs:element name="Documents" type="Documents" minOccurs="1" maxOccurs="1"/>
        <!--<xs:element name="Contacts" type="Contacts" minOccurs="0" maxOccurs="unbounded"/>-->
        <xs:element name="Authorizations" type="Authorizations" minOccurs="0" maxOccurs="1"/>
        <xs:element name="Relations" type="Relations" minOccurs="0" maxOccurs="1"/>
        <xs:element name="Metadata" type="Metadata" minOccurs="1" maxOccurs="1"/>
        <xs:element name="CustomJavaScript" type="xs:string" minOccurs="0" maxOccurs="1"/>
        <xs:element name="CustomCSS" type="xs:string" minOccurs="0" maxOccurs="1"/>
      </xs:sequence>
      <xs:attribute name="WorkflowEnabled" type="xs:boolean" use="optional" default="true"/>
      <xs:attribute name="DocumentUnitSynchronizeEnabled" type="xs:boolean" use="optional" default="true"/>
      <xs:attribute name="DocumentUnitSynchronizeTitle" type="xs:string" use="optional" default="{Year}/{Number:0000000}"/>
      <xs:attribute name="ProtocolEnabled" type="xs:boolean" use="optional" default="false"/>
      <xs:attribute name="PECEnabled" type="xs:boolean" use="optional" default="false"/>
      <xs:attribute name="PECButtonEnabled" type="xs:boolean" use="optional" default="false"/>
      <xs:attribute name="MailButtonEnabled" type="xs:boolean" use="optional" default="false"/>
      <xs:attribute name="MailRoleButtonEnabled" type="xs:boolean" use="optional" default="false"/>
      <xs:attribute name="CancelMotivationRequired" type="xs:boolean" use="optional" default="true"/>
      <xs:attribute name="IncrementalIdentityEnabled" type="xs:boolean" use="optional" default="true" />
      <xs:attribute name="Layout" type="LayoutType" use="optional" default="GridOneColumn"/>
    </xs:complexType>
  </xs:element>

</xs:schema>'
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
PRINT N'Aggiunta della colonna [UniqueId] alla tabella [RoleUser] e popolamento ';
GO

ALTER TABLE [dbo].[RoleUser]
ADD [UniqueId] uniqueidentifier NULL;
GO

UPDATE [dbo].[RoleUser]
SET [UniqueId] = newid()
WHERE [UniqueId] IS NULL
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
PRINT N'Creazione Indice NON CLUSTERED [IX_DocumentUnits_Year_Number_RegistrationDate_IdUDSRepository] su  [cqrs].[DocumentUnits]';
GO

CREATE NONCLUSTERED INDEX [IX_DocumentUnits_Year_Number_RegistrationDate_IdUDSRepository] ON [cqrs].[DocumentUnits]
(
       [Number] ASC,
       [RegistrationDate] ASC,
       [Year] ASC,
       [IdUDSRepository] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
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
PRINT N'Drop index [XAKName] dalla tabella Container';
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Container]') AND name = N'XAKName')
DROP INDEX [XAKName] ON [dbo].[Container] WITH ( ONLINE = OFF )
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
PRINT N'Modificata colonna [Name] dalla tabella [dbo].[Container] ';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [Name] nvarchar(256) not null
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
PRINT N'Creazione NON CLUSTERED index [XAKName] dalla tabella Container';
GO

CREATE UNIQUE NONCLUSTERED INDEX [XAKName] ON [dbo].[Container]
(
	[idContainer] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
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

PRINT N'Modificata colonna [Name] dalla tabella [dbo].[DocumentSeries] ';
GO

ALTER TABLE [dbo].[DocumentSeries] ALTER COLUMN [Name] nvarchar(256) not null
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
PRINT N'Creo nuovo indice [IX_Collaboration_IdParent_isActive_StartDate_EndDate] alla tabella [Category]';
GO

CREATE NONCLUSTERED INDEX [IX_Collaboration_IdParent_isActive_StartDate_EndDate] ON [dbo].[Category] ([idParent],[isActive],[StartDate],[EndDate])
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