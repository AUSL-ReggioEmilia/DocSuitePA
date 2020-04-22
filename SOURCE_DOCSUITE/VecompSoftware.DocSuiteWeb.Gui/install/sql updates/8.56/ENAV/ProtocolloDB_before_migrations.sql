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
PRINT 'Versionamento database alla 8.56'
GO

EXEC dbo.VersioningDatabase N'8.56'
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
PRINT 'ALTER TABLE [dbo].[PECMailattachment] ALTER COLUMN [attachmentname] nvarchar(256)'
GO

ALTER TABLE [dbo].[PECMailattachment] ALTER COLUMN [attachmentname] nvarchar(256) NOT NULL
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
PRINT 'Aggiunta colonne IdUDSRepository e DocumentUnitType nella tabella PECMail'
GO

ALTER TABLE [dbo].[PECMail] ADD IdUDS [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[PECMail] ADD [IdUDSRepository] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[PECMail] ADD [DocumentUnitType] [int] NULL
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
PRINT 'Aggiungo la CONSTRAINT FK_PECMail_UDSRepositories'
GO

ALTER TABLE [dbo].[PECMail]  WITH CHECK ADD  CONSTRAINT [FK_PECMail_UDSRepositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [dbo].[PECMail] CHECK CONSTRAINT [FK_PECMail_UDSRepositories]
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
PRINT 'Aggiorno DocumentUnitType per le PEC già protocollate'
GO

UPDATE [dbo].[PECMail] SET [DocumentUnitType] = 1 WHERE [Year] is not null AND [Number] is not null
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
PRINT 'Aggiungo la colonna Alias per la tabella UDSRepositories'
GO

ALTER TABLE [uds].[UDSRepositories] ADD Alias [nvarchar](4) NULL
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
PRINT 'Aggiungo la CONSTRAINT FK_Protocol_UDSRepositories'
GO

ALTER TABLE [dbo].[Protocol]  WITH CHECK ADD  CONSTRAINT [FK_Protocol_UDSRepositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [dbo].[Protocol] CHECK CONSTRAINT [FK_Protocol_UDSRepositories]
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
PRINT 'Creazione tabella [ProtocolHighlightUsers]'

CREATE TABLE [dbo].[ProtocolHighlightUsers] (
	[IdProtocolHighlightUser] [uniqueidentifier] NOT NULL,
	[Year] [smallint] NOT NULL,
	[Number] [int] NOT NULL,
	[UniqueIdProtocol] [uniqueidentifier] NOT NULL,
	[Account] [nvarchar](256) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,	
 CONSTRAINT [PK_ProtocolHighlightUsers] PRIMARY KEY NONCLUSTERED 
(
	[IdProtocolHighlightUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_ProtocolHighlightUsers_RegistationDate]
    ON [dbo].[ProtocolHighlightUsers]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[ProtocolHighlightUsers]  WITH CHECK ADD  CONSTRAINT [FK_ProtocolHighlightUsers_Protocol] FOREIGN KEY([Year], [Number])
REFERENCES [dbo].[Protocol] ([Year], [Number])
GO
ALTER TABLE [dbo].[ProtocolHighlightUsers] CHECK CONSTRAINT [FK_ProtocolHighlightUsers_Protocol]
GO

ALTER TABLE [dbo].[ProtocolHighlightUsers]  WITH CHECK ADD  CONSTRAINT [FK_ProtocolHighlightUsers_Protocol_UniqueId] FOREIGN KEY([UniqueIdProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])
GO
ALTER TABLE [dbo].[ProtocolHighlightUsers] CHECK CONSTRAINT [FK_ProtocolHighlightUsers_Protocol_UniqueId]
GO

--#############################################################################

PRINT N'Creazione indice univoco [IX_ProtocolHighlightUsers_UniqueIdProtocol_Account]';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ProtocolHighlightUsers_UniqueIdProtocol_Account]
    ON [dbo].[ProtocolHighlightUsers]([UniqueIdProtocol] ASC, [Account] ASC);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
BEGIN
INSERT INTO #tmpErrors (Error) VALUES (1);
BEGIN TRANSACTION;
END
GO

--#############################################################################

PRINT 'Creazione colonna UniqueId nullable nella tabella DocumentSeriesItem'
GO
-- 1 
ALTER TABLE [dbo].[DocumentSeriesItem] 
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
PRINT 'Aggiornamento della tabella DocumentSeriesItem'
GO
-- 2 
UPDATE [dbo].[DocumentSeriesItem]
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
ALTER TABLE [dbo].[DocumentSeriesItem]
    ADD CONSTRAINT [DF_DocumentSeriesItem_UniqueId] 
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
ALTER TABLE [dbo].[DocumentSeriesItem] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT N'Aggiunto unique indice [IX_DocumentSeriesItem_UniqueId] in DocumentSeriesItem';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DocumentSeriesItem_UniqueId]
    ON [dbo].[DocumentSeriesItem]([UniqueId] ASC);
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

PRINT N'Aggiunto unique indice [IX_Role_UniqueId] in Role';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Role_UniqueId]
    ON [dbo].[Role]([UniqueId] ASC);
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

PRINT 'Creazione colonna UniqueId nullable nella tabella DocumentSeriesItemRole'
GO
-- 1 
ALTER TABLE [dbo].[DocumentSeriesItemRole] 
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

PRINT 'Aggiornamento della tabella DocumentSeriesItemRole'
GO
-- 2 
UPDATE [dbo].[DocumentSeriesItemRole]
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
ALTER TABLE [dbo].[DocumentSeriesItemRole]
    ADD CONSTRAINT [DF_DocumentSeriesItemRole_UniqueId] 
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
ALTER TABLE [dbo].[DocumentSeriesItemRole] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT N'Aggiunto unique indice [IX_DocumentSeriesItemRole_UniqueId] in DocumentSeriesItemRole';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DocumentSeriesItemRole_UniqueId]
    ON [dbo].[DocumentSeriesItemRole]([UniqueId] ASC);
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

PRINT 'Modifica tabella ProtocolRole colonna RegistrationDate nullable'
GO

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [RegistrationDate] DATETIMEOFFSET(7) NULL
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

PRINT N'Modificata [FK_Protocol_ProtocolJournalLog] in Protocol';
GO

ALTER TABLE [dbo].[Protocol] DROP CONSTRAINT [FK_Protocol_ProtocolJournalLog]
GO

ALTER TABLE [dbo].[Protocol]  WITH CHECK ADD  CONSTRAINT [FK_Protocol_ProtocolJournalLog] FOREIGN KEY([IdProtocolJournalLog])
REFERENCES [dbo].[ProtocolJournalLog] ([IdProtocolJournalLog]) on delete set null
GO

ALTER TABLE [dbo].[Protocol] CHECK CONSTRAINT [FK_Protocol_ProtocolJournalLog]
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

PRINT N'Modificata colonna [Conservation] in Container';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [Conservation] smallint null
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

PRINT N'Modificata colonna [isActive] in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [isActive] tinyint null
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

PRINT N'Modificata colonna [Collapsed] in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [Collapsed] tinyint null
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
PRINT N'ALTER TABLE [dbo].[FasciclePeriods] ADD [Timestamp] TIMESTAMP NOT NULL';
GO

ALTER TABLE [dbo].[FasciclePeriods] ADD [Timestamp] TIMESTAMP NOT NULL
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

UPDATE [uds].[UDSSchemaRepositories] SET [SchemaXML] = '
<xs:schema id="SchemaUnitaDocumentariaSpecifica"
    targetNamespace="http://tempuri.org/SchemaUnitaDocumentariaSpecifica.xsd"
    elementFormDefault="qualified"
    xmlns="http://tempuri.org/SchemaUnitaDocumentariaSpecifica.xsd"
    xmlns:mstns="http://tempuri.org/SchemaUnitaDocumentariaSpecifica.xsd"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
>

  <!-- Lista di documenti -->
  <xs:complexType name="Documents">
    <!--Tipo del documento:
        1) documento principale 
        2) documento allegato
        3) documento annesso
      -->
    <xs:all>
      <!--Almeno un documento deve essere inserito se viene creato un Documents-->
      <xs:element name="Document" type="Document" minOccurs="1" maxOccurs="1" ></xs:element>
      <xs:element name="DocumentAttachment" type="Document" minOccurs="0" maxOccurs="1"></xs:element>
      <xs:element name="DocumentAnnexed" type="Document" minOccurs="0" maxOccurs="1"></xs:element>
    </xs:all>
  </xs:complexType>

  <!-- Definizione del classificatore  -->
  <xs:complexType name="Category">
    <xs:attribute name="IdCategory" use="required"></xs:attribute>
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="DefaultEnabled" type ="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="true"></xs:attribute>
  </xs:complexType>

  <!-- Definizione del contenitore  -->
  <xs:complexType name="Container">
    <xs:attribute name="IdContainer" use="required"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="true"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di un documento -->
  <xs:complexType name="Document">
    <xs:sequence minOccurs="0">
      <xs:element name="Instances" type="DocumentInstance" minOccurs="1" maxOccurs="unbounded"></xs:element>
    </xs:sequence >
    <xs:attribute name="SectionLabel" type="xs:string" use="required"></xs:attribute>
    <xs:attribute name="Label" type="xs:string" use="required"></xs:attribute>
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="BiblosArchive" type="xs:string" use="required"></xs:attribute>
    <xs:attribute name="CreateBiblosArchive" type="xs:boolean" use="optional" default="false"></xs:attribute>
    <xs:attribute name="AllowMultiFile" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="UploadEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="ScannerEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="SignEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="SignRequired" type="xs:boolean" use="optional" default="false"></xs:attribute>
    <xs:attribute name="CopyProtocol" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="CopyResolution" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="CopySeries" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="Required" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="false"></xs:attribute>
  </xs:complexType>

  <!-- Definizione della serie documentale-->
  <xs:complexType name="DocumentInstance">
    <xs:attribute name="IdDocument"></xs:attribute>
    <xs:attribute name="DocumentContent" type="xs:string"></xs:attribute>
    <xs:attribute name="DocumentName" type="xs:string"></xs:attribute>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione delle serie documentali-->
  <xs:complexType name="DocumentSeries">
    <xs:sequence minOccurs="0">
      <xs:element name="Instances" type="DocumentSeriesInstance"></xs:element>
    </xs:sequence>
  </xs:complexType>

  <!-- Definizione della serie documentale-->
  <xs:complexType name="DocumentSeriesInstance">
    <xs:attribute name="IdDocumentSeries"></xs:attribute>
    <!--Descrivere il resto-->
  </xs:complexType>

  <xs:simpleType name="ContactType">
    <xs:restriction base="xs:string">
      <xs:enumeration value="Sender" />
      <xs:enumeration value="Recipient" />
    </xs:restriction>
  </xs:simpleType>

  <!-- Definizione dei contatti-->
  <xs:complexType name="Contacts">
    <xs:sequence minOccurs="0">
        <!-- Contatto -->
        <xs:element name="ContactInstances" type="ContactInstance" minOccurs="0" maxOccurs="unbounded"></xs:element>
        <!-- Contatto Manuale-->
        <xs:element name="ContactManualInstances" type="ContactManualInstance" minOccurs="0" maxOccurs="unbounded"></xs:element>
    </xs:sequence>
    <xs:attribute name="SectionLabel" type="xs:string" use="required"></xs:attribute>
    <xs:attribute name="ContactType" type="ContactType" use="optional"></xs:attribute>
    <xs:attribute name="ADEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="AddressBookEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="ADDistributionListEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="ManualEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="ExcelImportEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
    <xs:attribute name="AllowMultiContact" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="CustomAction" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="Required" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="true"></xs:attribute>
  </xs:complexType>

  <!-- Definizione dei contatti-->
  <xs:complexType name="ContactInstance">
    <xs:attribute name="IdContact"></xs:attribute>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei contatti manuali-->
  <xs:complexType name="ContactManualInstance">
    <xs:attribute name="ContactDescription"></xs:attribute>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei Authorizations-->
  <xs:complexType name="Authorizations">
    <xs:sequence minOccurs="0">
      <xs:element name="Instances" type="AuthorizationInstance" minOccurs="1" maxOccurs="unbounded"></xs:element>
    </xs:sequence>
    <xs:attribute name="SectionLabel" type="xs:string" use="required"></xs:attribute>
    <xs:attribute name="AllowMultiAuthorization" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="CustomAction" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="Required" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="true"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di un Authorization-->
  <xs:complexType name="AuthorizationInstance">
    <xs:attribute name="IdAuthorization"></xs:attribute>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei messages-->
  <xs:complexType name="Messages">
    <xs:sequence>
      <xs:element name="Instances" type="MessageInstance" minOccurs="1" maxOccurs="unbounded"></xs:element>
    </xs:sequence>
    <xs:attribute name="SectionLabel" type="xs:string" use="required"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di un message-->
  <xs:complexType name="MessageInstance">
    <xs:attribute name="IdMessage"></xs:attribute>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione delle PECMails-->
  <xs:complexType name="PECMails">
    <xs:sequence>
      <xs:element name="Instances" type="PECInstance" minOccurs="1" maxOccurs="unbounded"></xs:element>
    </xs:sequence>
    <xs:attribute name="SectionLabel" type="xs:string" use="required"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di una pec-->
  <xs:complexType name="PECInstance">
    <xs:attribute name="IdPECMail"></xs:attribute>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei Protocols-->
  <xs:complexType name="Protocols">
    <xs:sequence>
      <xs:element name="Instances" type="ProtocolInstance" minOccurs="1" maxOccurs="unbounded"></xs:element>
    </xs:sequence>
    <xs:attribute name="SectionLabel" type="xs:string" use="required"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di un Protocol-->
  <xs:complexType name="ProtocolInstance">
    <xs:attribute name="IdProtocol"></xs:attribute>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione dei Resolutions-->
  <xs:complexType name="Resolutions">
    <xs:sequence>
      <xs:element name="Instances" type="ResolutionInstance" minOccurs="1" maxOccurs="unbounded"></xs:element>
    </xs:sequence>
    <xs:attribute name="SectionLabel" type="xs:string" use="required"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di un Resolution-->
  <xs:complexType name="ResolutionInstance">
    <xs:attribute name="IdResolution"></xs:attribute>
    <!--Descrivere il resto-->
  </xs:complexType>

  <!-- Definizione di un campo testuale -->
  <xs:complexType name="TextField">
    <xs:attribute name="Label" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="ColumnName" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="Multiline" type="xs:boolean" use="optional" default="false"></xs:attribute>
    <xs:attribute name="DefaultValue" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="DefaultSearchValue" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="CustomAction" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="Value" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="Required" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="HiddenField" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="true"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di un campo data -->
  <xs:complexType name="DateField">
    <xs:attribute name="Label" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="ColumnName" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="DefaultValue" type="xs:dateTime" use="optional"  ></xs:attribute>
    <xs:attribute name="DefaultSearchValue" type="xs:string" use="optional"  ></xs:attribute>
    <xs:attribute name="CustomAction" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="Value" type="xs:dateTime" use="optional"  ></xs:attribute>
    <xs:attribute name="RestrictedYear" type="xs:boolean" use="optional" default="false"></xs:attribute>
    <xs:attribute name="Required" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="HiddenField" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="true"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di un campo booleano -->
  <xs:complexType name="BoolField">
    <xs:attribute name="Label" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="ColumnName" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="DefaultValue" type="xs:boolean" use="optional"></xs:attribute>
    <xs:attribute name="DefaultSearchValue" type="xs:boolean" use="optional"></xs:attribute>
    <xs:attribute name="CustomAction" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="Value" type="xs:boolean" use="optional"></xs:attribute>
    <xs:attribute name="Required" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="HiddenField" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="false"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di un campo numerico -->
  <xs:complexType name="NumberField">
    <xs:attribute name="Label" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="ColumnName" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="DefaultValue" type="xs:double" use="optional" ></xs:attribute>
    <xs:attribute name="DefaultSearchValue" type="xs:double" use="optional" ></xs:attribute>
    <xs:attribute name="CustomAction" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="Value" type="xs:double" use="optional" ></xs:attribute>
    <xs:attribute name="Required" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="HiddenField" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="true"></xs:attribute>
  </xs:complexType>

  <!-- Definizione di un campo data -->
  <xs:complexType name="EnumField">
    <xs:sequence>
      <xs:element name="Options" type="xs:string" minOccurs="1" maxOccurs="unbounded"></xs:element>
    </xs:sequence>
    <xs:attribute name="Label" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="ColumnName" type ="xs:string" use="required"></xs:attribute>
    <xs:attribute name="DefaultValue" type="xs:string" use="optional"  ></xs:attribute>
    <xs:attribute name="DefaultSearchValue" type="xs:string" use="optional"  ></xs:attribute>
    <xs:attribute name="CustomAction" type="xs:string" use="optional" ></xs:attribute>
    <xs:attribute name="Value" type="xs:string" use="optional"  ></xs:attribute>
    <xs:attribute name="Required" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="HiddenField" type ="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"></xs:attribute>
    <xs:attribute name="Searchable" type ="xs:boolean" use="optional" default="true"></xs:attribute>
  </xs:complexType>

  <!-- Definizione dei campi "BASE" di un singolo UDS -->
  <xs:complexType name="Section">
    <xs:choice minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Enum" type="EnumField" minOccurs="0"></xs:element>
      <xs:element name="Text" type="TextField" minOccurs="0"></xs:element>
      <xs:element name="Date" type="DateField" minOccurs="0"></xs:element>
      <xs:element name="Number" type="NumberField" minOccurs="0"></xs:element>
      <xs:element name="Bool" type="BoolField" minOccurs="0"></xs:element>
    </xs:choice>
    <xs:attribute name="Label" type="xs:string" use="required"></xs:attribute>
  </xs:complexType>

  <!-- Definizione dei campi "RELAZIONE" di un singolo UDS verso entità esterne -->
  <xs:complexType name="Relations">
    <xs:choice minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Contacts" type="Contacts" minOccurs="0" maxOccurs="unbounded"></xs:element>
      <xs:element name="Messages" type="Messages" minOccurs="0"></xs:element>
      <xs:element name="PECMails" type="PECMails" minOccurs="0"></xs:element>
      <xs:element name="Protocols" type="Protocols" minOccurs="0"></xs:element>
      <xs:element name="Resolutions" type="Resolutions" minOccurs="0"></xs:element>
    </xs:choice>
    <xs:attribute name="Label" type="xs:string" use="required"></xs:attribute>
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required"></xs:attribute>
  </xs:complexType>

  <xs:complexType name="Metadata">
    <xs:sequence minOccurs="0" maxOccurs="unbounded">
      <xs:element name="Sections" type="Section" minOccurs="0" maxOccurs="unbounded"></xs:element>
    </xs:sequence>
  </xs:complexType>

  <!-- In una unità documentaria sono necessari i seguenti campi:
       1) Title             -> unico per tutte le unità documentarie
       2) Subject           -> oggetto dell''unità documentaria
       3) Category          -> classificatore
       4) Authorizations    -> diritti sull''unità documentaria
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
  <xs:element name="UnitaDocumentariaSpecifica">
    <xs:complexType>
      <xs:sequence minOccurs="1" maxOccurs="1">
        <xs:element name="Title" type="xs:string" minOccurs="1" maxOccurs="1"></xs:element>
        <xs:element name="Alias" minOccurs="0" maxOccurs="1">
          <xs:simpleType>
            <xs:restriction base="xs:string">
              <xs:maxLength value="4" />
            </xs:restriction>
          </xs:simpleType>
        </xs:element>
        <xs:element name="Subject" type="xs:string" minOccurs="1" maxOccurs="1"></xs:element>
        <xs:element name="Category" type="Category" minOccurs="1" maxOccurs="1"></xs:element>
        <xs:element name="Container" type="Container" minOccurs="1" maxOccurs="1"></xs:element>
        <xs:element name="Documents" type="Documents" minOccurs="0" maxOccurs="1"></xs:element>
        <xs:element name="Authorizations" type="Authorizations" minOccurs="0" maxOccurs="1"></xs:element>
        <xs:element name="Relations" type="Relations" minOccurs="0" maxOccurs="1"></xs:element>
        <xs:element name="Metadata" type="Metadata" minOccurs="0" maxOccurs="1"></xs:element>
      </xs:sequence>
      <xs:attribute name="WorkflowEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
      <xs:attribute name="DocumentUnitSynchronizeEnabled" type="xs:boolean" use="optional" default="true"></xs:attribute>
      <xs:attribute name="ProtocolEnabled" type="xs:boolean" use="optional" default="false"></xs:attribute>
      <xs:attribute name="PecEnabled" type="xs:boolean" use="optional" default="false"></xs:attribute>
      <xs:attribute name="PecButtonEnabled" type="xs:boolean" use="optional" default="false"></xs:attribute>
      <xs:attribute name="MailButtonEnabled" type="xs:boolean" use="optional" default="false"></xs:attribute>
      <xs:attribute name="MailRoleButtonEnabled" type="xs:boolean" use="optional" default="false"></xs:attribute>
      <xs:attribute name="CancelMotivationRequired" type="xs:boolean" use="optional" default="true"></xs:attribute>
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

--#############################################################################

PRINT 'Creazione della tabella DocumentUnits'
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'cqrs')
BEGIN
	EXEC('CREATE SCHEMA cqrs');
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cqrs].[DocumentUnits](
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
	[IdContainer] [smallint] NOT NULL,
	[IdCategory] [smallint] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NULL,
	[IdFascicle] [uniqueidentifier] NULL,
	[EntityId] [int] NOT NULL,
	[Year] [smallint] NOT NULL,
	[Number] [int] NOT NULL,
	[Title] [nvarchar](256) NOT NULL,
	[Subject] [nvarchar](1000) NULL,
	[DocumentUnitName] [nvarchar](256) NOT NULL,
	[Environment] [int] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DocumentUnit] PRIMARY KEY NONCLUSTERED 
(
	[IdDocumentUnit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [cqrs].[DocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnits_Container] FOREIGN KEY([IdContainer])
REFERENCES [dbo].[Container] ([idContainer])
GO
ALTER TABLE [cqrs].[DocumentUnits] CHECK CONSTRAINT [FK_DocumentUnits_Container]
GO

ALTER TABLE [cqrs].[DocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnits_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([idCategory])
GO
ALTER TABLE [cqrs].[DocumentUnits] CHECK CONSTRAINT [FK_DocumentUnits_Category]
GO

ALTER TABLE [cqrs].[DocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnits_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
ALTER TABLE [cqrs].[DocumentUnits] CHECK CONSTRAINT [FK_DocumentUnits_Fascicles]
GO

ALTER TABLE [cqrs].[DocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnits_UDSRespositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO
ALTER TABLE [cqrs].[DocumentUnits] CHECK CONSTRAINT [FK_DocumentUnits_UDSRespositories]
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

PRINT N'Aggiunto clustered indice [IX_DocumentUnits_RegistationDate] in DocumentUnits';
GO

CREATE CLUSTERED INDEX [IX_DocumentUnits_RegistationDate]
	ON [cqrs].[DocumentUnits]([RegistrationDate] ASC);
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

PRINT 'Creazione della tabella DocumentUnitRoles'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cqrs].[DocumentUnitRoles](
	[IdDocumentUnitRole] [uniqueidentifier] NOT NULL,
	[UniqueIdRole] [uniqueidentifier] NOT NULL,
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
	[RoleLabel] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DocumentUnitRole] PRIMARY KEY NONCLUSTERED  
(
	[IdDocumentUnitRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [cqrs].[DocumentUnitRoles]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitRoles_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO
ALTER TABLE [cqrs].[DocumentUnitRoles] CHECK CONSTRAINT [FK_DocumentUnitRoles_DocumentUnits]
GO

ALTER TABLE [cqrs].[DocumentUnitRoles]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitRoles_Role] FOREIGN KEY([UniqueIdRole])
REFERENCES [dbo].[Role] ([UniqueId])
GO
ALTER TABLE [cqrs].[DocumentUnitRoles] CHECK CONSTRAINT [FK_DocumentUnitRoles_Role]
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

PRINT N'Aggiunto clustered indice [IX_DocumentUnitRoles_RegistationDate] in DocumentUnitRoles';
GO

CREATE CLUSTERED INDEX [IX_DocumentUnitRoles_RegistationDate]
	ON [cqrs].[DocumentUnitRoles]([RegistrationDate] ASC);
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

PRINT 'Creazione della tabella DocumentUnitChains'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cqrs].[DocumentUnitChains](
	[IdDocumentUnitChain] [uniqueidentifier] NOT NULL,
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
	[DocumentName] [nvarchar](256) NULL,
	[ArchiveName] [nvarchar](256) NOT NULL,
	[IdArchiveChain] [uniqueidentifier] NOT NULL,
	[ChainType] [smallint] NOT NULL,
	[DocumentLabel] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DocumentUnitChain] PRIMARY KEY NONCLUSTERED  
(
	[IdDocumentUnitChain] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [cqrs].[DocumentUnitChains]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitChains_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO
ALTER TABLE [cqrs].[DocumentUnitChains] CHECK CONSTRAINT [FK_DocumentUnitChains_DocumentUnits]
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

PRINT N'Aggiunto clustered indice [IX_DocumentUnitChains_RegistationDate] in DocumentUnitChains';
GO

CREATE CLUSTERED INDEX [IX_DocumentUnitChains_RegistationDate]
	ON [cqrs].[DocumentUnitChains]([RegistrationDate] ASC);
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

PRINT N'Modifico la Function FascicleDocumentUnits';
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[FascicleDocumentUnits]
(
	@FascicleId uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(    

	SELECT DU.[IdDocumentUnit] as UniqueId
	  ,DU.[IdFascicle]
	  ,DU.[EntityId]
	  ,DU.[Year]
	  ,CAST(DU.[Number] as varchar) as Number
	  ,DU.[Title]
	  ,DU.[Subject]
	  ,DU.[DocumentUnitName]
	  ,DU.[Environment]
	  ,DU.[RegistrationUser]
	  ,DU.[RegistrationDate]
	  ,COALESCE(FP.ReferenceType, FR.ReferenceType, FD.ReferenceType) as ReferenceType
	  ,CT.idCategory as Category_IdCategory
	  ,CT.Name as Category_Name
	  ,C.idContainer as Container_IdContainer
	  ,C.Name as Container_Name
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN dbo.FascicleProtocols FP ON FP.UniqueIdProtocol = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleResolutions FR ON FR.UniqueIdResolution = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleDocumentSeriesItems FD ON FD.IdDocumentSeriesItem = DU.EntityId
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE 
	(FP.UniqueIdProtocol IS NOT NULL AND FP.IdFascicle = @FascicleId)
	OR (FR.UniqueIdResolution IS NOT NULL AND FR.IdFascicle = @FascicleId)
	OR (FD.IdDocumentSeriesItem IS NOT NULL AND FD.IdFascicle = @FascicleId)

);
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

PRINT N'Modifico la Function AvailableFasciclesFromResolution';
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[AvailableFasciclesFromResolution]
(
	@UniqueIdResolution uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate						
	FROM Fascicles F 
		LEFT JOIN FascicleResolutions FR on F.IdFascicle = FR.IdFascicle AND FR.UniqueIdResolution = @UniqueIdResolution
		LEFT JOIN Resolution R on R.UniqueId = @UniqueIdResolution
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 2 AND CF.FascicleType = 1 
		WHERE F.IdCategory = R.IdCategoryAPI AND FR.IdFascicleResolution IS NULL
		AND F.EndDate IS NULL
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate
);
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

PRINT N'Modifico la Function AvailableFasciclesFromProtocol';
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[AvailableFasciclesFromProtocol]
(
	@UniqueIdProtocol uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate						
	FROM Fascicles F 
		LEFT JOIN FascicleProtocols FP on F.IdFascicle = FP.IdFascicle AND FP.UniqueIdProtocol = @UniqueIdProtocol
		LEFT JOIN Protocol P on P.UniqueId = @UniqueIdProtocol
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 1 AND CF.FascicleType = 1 
		WHERE F.IdCategory = P.IdCategoryAPI AND FP.IdFascicleProtocol IS NULL
		AND F.EndDate IS NULL
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate
);
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

PRINT N'ALTER FUNCTION [dbo].[FascicolableDocumentUnitsSecurityUser]';
GO

ALTER FUNCTION [dbo].[FascicolableDocumentUnitsSecurityUser]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset,
	@IncludeThreshold bit,
	@ThresholdFrom datetimeoffset
)
RETURNS TABLE
AS
RETURN
(
	WITH 
	
	MySecurityGroups AS (
		SELECT SG.IdGroup 
		FROM [dbo].[SecurityGroups] SG 
		INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		GROUP BY SG.IdGroup
	)

   
	SELECT DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title] AS [Number],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit] as [UniqueId],
		   CT.idCategory AS Category_IdCategory,
		   CT.Name AS Category_Name,
		   C.idContainer AS Container_IdContainer,
		   C.Name AS Container_Name
	FROM [cqrs].[DocumentUnits] DU
	INNER JOIN [dbo].[Category] CT on DU.idCategory = CT.idCategory
	INNER JOIN [dbo].[Container] C on DU.idContainer = C.idContainer
	INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
	LEFT OUTER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
	
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DU.IdDocumentUnit = DUR.IdDocumentUnit
	LEFT OUTER JOIN [dbo].[Role] RL on DUR.UniqueIdRole = RL.UniqueId
	LEFT OUTER JOIN [dbo].[RoleGroup] RG on RL.idRole = RG.idRole
	LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup

	WHERE ( (@IncludeThreshold = 0 AND DU.RegistrationDate BETWEEN @DateFrom AND @DateTo) OR
				( @IncludeThreshold = 1 AND ( DU.RegistrationDate BETWEEN @ThresholdFrom AND CAST(getdate()-60 AS DATE) OR 
											  DU.RegistrationDate BETWEEN @DateFrom AND @DateTo))
			   )
			   AND ( (C_MSG.IdGroup IS NOT NULL AND (CASE Environment 
													WHEN 1 THEN CG.Rights 
													WHEN 2 THEN CG.ResolutionRights 
													WHEN 3 THEN '0'
													WHEN 4 THEN CG.DocumentSeriesRights
													WHEN 5 THEN '0'
													ELSE CG.UDSRights
													END) like '1%') OR 
					 ( DUR.UniqueIdRole IS NULL OR 
					  (DUR.UniqueIdRole IS NOT NULL AND (CASE Environment 
													WHEN 1 THEN RG.ProtocolRights 
													WHEN 2 THEN RG.ResolutionRights 
													WHEN 3 THEN '0'
													WHEN 4 THEN RG.DocumentSeriesRights
													ELSE '0'
													END like '1%') AND MSG.IdGroup IS NOT NULL)
					 ) 
				   )
			   AND NOT (C_MSG.IdGroup IS NULL AND MSG.IdGroup IS NULL)
			   AND DU.[IdFascicle] IS NULL
			   AND DU.Environment in (1,2)
	GROUP BY DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit],
		   CT.idCategory,
		   CT.Name,
		   C.idContainer,
		   C.Name
);

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

PRINT N'ALTER FUNCTION [dbo].[FascicolableDocumentUnits]';
GO

ALTER FUNCTION [dbo].[FascicolableDocumentUnits]
(
    @DateFrom datetimeoffset,
    @DateTo datetimeoffset,
    @IncludeThreshold bit,
    @ThresholdFrom datetimeoffset,
    @RoleNames xml,
    @TenantId as uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
    WITH 
    MyNames AS ( 
	SELECT x.y.value('.','varchar(255)') AS GroupName
		FROM @RoleNames.nodes('/ArrayOfDomainGroupModel/DomainGroupModel/Name/text()') as x ( y )
		),
    MyRoles AS (
        SELECT R.UniqueId, Environment = 1
			FROM [Role] R
			INNER JOIN [RoleGroup] RG ON R.IdRole = RG.IdRole
			INNER JOIN MyNames MM on RG.GroupName = MM.GroupName
			WHERE RG.ProtocolRights like '1%' and R.IsActive = 1
			GROUP BY R.UniqueId
		UNION ALL 
		SELECT R.UniqueId, Environment = 2
				FROM [Role] R
				INNER JOIN [RoleGroup] RG ON R.IdRole = RG.IdRole
				INNER JOIN MyNames MM on RG.GroupName = MM.GroupName
				WHERE RG.ResolutionRights like '1%' and R.IsActive = 1
				GROUP BY R.UniqueId
		--UNION ALL 
		--SELECT R.UniqueId, Environment = 4
		--		FROM [Role] R
		--		INNER JOIN [RoleGroup] RG ON R.IdRole = RG.IdRole
		--		INNER JOIN  MyNames MM on RG.GroupName = MM.GroupName
		--		WHERE RG.DocumentSeriesRights like '1%' and R.IsActive = 1
		--		GROUP BY R.UniqueId
    )
    
	SELECT DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title] AS [Number],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit] as [UniqueId],
		   CT.idCategory AS Category_IdCategory,
		   CT.Name AS Category_Name,
		   C.idContainer AS Container_IdContainer,
		   C.Name AS Container_Name
	FROM [cqrs].[DocumentUnits] DU
	INNER JOIN [dbo].[Category] CT on DU.idCategory = CT.idCategory
	INNER JOIN [dbo].[Container] C on DU.idContainer = C.idContainer
	INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
	LEFT OUTER JOIN MyNames MM on CG.GroupName =  MM.GroupName

    LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DU.IdDocumentUnit = DUR.IdDocumentUnit
    LEFT OUTER JOIN MyRoles D_MSG on DUR.UniqueIdRole = D_MSG.UniqueId AND DU.Environment = D_MSG.Environment

	WHERE ( (@IncludeThreshold = 0 AND DU.RegistrationDate BETWEEN @DateFrom AND @DateTo) OR
                ( @IncludeThreshold = 1 AND ( DU.RegistrationDate BETWEEN @ThresholdFrom AND CAST(getdate()-60 AS DATE) OR 
                                              DU.RegistrationDate BETWEEN @DateFrom AND @DateTo))
               )
               AND ( (MM.GroupName IS NOT NULL AND (CASE DU.Environment 
													WHEN 1 THEN CG.Rights 
													WHEN 2 THEN CG.ResolutionRights 
													WHEN 3 THEN '0'
													WHEN 4 THEN CG.DocumentSeriesRights
													WHEN 5 THEN '0'
													ELSE CG.UDSRights
													END) like '1%' COLLATE DATABASE_DEFAULT) OR
                     (MM.GroupName IS NULL AND D_MSG.UniqueId IS NOT NULL) OR
                     (MM.GroupName IS NOT NULL AND D_MSG.UniqueId IS NOT NULL)
                   )
               AND NOT (D_MSG.UniqueId IS NULL AND MM.GroupName IS NULL)

               AND DU.[IdFascicle] IS NULL
			   AND DU.Environment in (1,2)
	GROUP BY DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit],
		   CT.idCategory,
		   CT.Name,
		   C.idContainer,
		   C.Name
);


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

--#############################################################################

PRINT 'CREATE FUNCTION [dbo].[AuthorizedDocuments]'
GO

CREATE FUNCTION [dbo].[AuthorizedDocuments] 
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset
)
RETURNS TABLE 
AS
RETURN 
(
WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
        GROUP BY SG.IdGroup
    )

	SELECT distinct DU.[IdDocumentUnit] as UniqueId
	  ,DU.[IdFascicle]
	  ,DU.[EntityId]
	  ,DU.[Year]
	  ,CAST(DU.[Number] as varchar) as Number
	  ,DU.[Title]
	  ,DU.[Subject]
	  ,DU.[DocumentUnitName]
	  ,DU.[Environment]
	  ,DU.[RegistrationUser]
	  ,DU.[RegistrationDate]
	  ,DU.[IdUDSRepository]
	  ,DU.idCategory as Category_IdCategory
	  ,CT.Name as Category_Name
	  ,DU.idContainer as Container_IdContainer
	  ,C.Name as Container_Name
	  ,(SELECT TOP 1 DocumentName 
	   FROM cqrs.DocumentUnitChains CUC 
	   WHERE CUC.IdDocumentUnit = DU.IdDocumentUnit AND CUC.DocumentName is not null AND CUC.DocumentName <> ''
	   ORDER BY CUC.RegistrationDate DESC) as DocumentUnitChain_DocumentName
	 FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
    LEFT OUTER JOIN [dbo].[Role] R on DUR.UniqueIdRole = R.UniqueId
    LEFT OUTER JOIN [dbo].[RoleGroup] RG on R.idRole = RG.idRole
    LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE DU.RegistrationDate BETWEEN @DateFrom AND @DateTo 
	AND ( (DUR.UniqueIdRole IS NOT NULL
				AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
				OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
				OR ((DU.Environment = 4 OR DU.Environment > 100) AND (RG.DocumentSeriesRights like '1%'))) 
				AND MSG.IdGroup IS NOT NULL)
				)
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

PRINT N'Creazione indice univoco [IX_DocumentUnits_DocumentUnitName_Year_Number]';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DocumentUnits_DocumentUnitName_Year_Number]
    ON [cqrs].[DocumentUnits]([DocumentUnitName] ASC, [Year] ASC, [Number] ASC);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
BEGIN
INSERT INTO #tmpErrors (Error) VALUES (1);
BEGIN TRANSACTION;
END
GO

--#############################################################################

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


--#############################################################################

PRINT N'Creazione function [AtVisionSignCollaborations]';
GO

CREATE FUNCTION [dbo].[AtVisionSignCollaborations](
	@UserName nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration			
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			right outer join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration				
		WHERE			
			Collaboration.IdStatus in ('IN', 'DL') and Collaboration.IdStatus is not null
			and Collaboration.RegistrationUser = @UserName
)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
BEGIN
INSERT INTO #tmpErrors (Error) VALUES (1);
BEGIN TRANSACTION;
END
GO
--#############################################################################

--#############################################################################

PRINT 'Creo la tabella MassimariScarto'
GO

CREATE TABLE [dbo].[MassimariScarto](
	[MassimarioScartoNode] [hierarchyid] NOT NULL,
	[IdMassimarioScarto] [uniqueidentifier] NOT NULL,
	[MassimarioScartoPath]  AS ([MassimarioScartoNode].[ToString]()) PERSISTED,
	[MassimarioScartoParentNode]  AS ([MassimarioScartoNode].[GetAncestor]((1))) PERSISTED,
	[MassimarioScartoLevel]  AS ([MassimarioScartoNode].[GetLevel]()) PERSISTED,
	[Name] [nvarchar](256) NOT NULL,
	[Code] [smallint] NULL,
	[Note] [nvarchar](1024) NULL,
	[ConservationPeriod] [smallint] NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,
	[Status] [smallint] NOT NULL,
	[FakeInsertId] [uniqueidentifier] NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[MassimarioScartoParentPath]  AS ([MassimarioScartoNode].[GetAncestor]((1)).ToString()) PERSISTED,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_MassimariScarto] PRIMARY KEY NONCLUSTERED 
(
	[MassimarioScartoNode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_MassimariScarto] UNIQUE NONCLUSTERED 
(
	[IdMassimarioScarto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_MassimariScarto_RegistationDate]
    ON [dbo].[MassimariScarto]([RegistrationDate] ASC);
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

PRINT 'Creo la function GetFullCodeFromMassimarioScarto'
GO

CREATE FUNCTION GetFullCodeFromMassimarioScarto 
(
	@massimarioId as uniqueidentifier
)
RETURNS nvarchar(256)
AS
BEGIN
	declare @fullCode nvarchar(256)

	SELECT @fullCode = CASE
		WHEN P.Code IS NULL THEN CAST(M.Code as nvarchar(256))
		ELSE CAST(P.Code as nvarchar(256)) + '|' + CAST(M.Code as nvarchar(256)) 
	   END
	FROM 
	[dbo].[MassimariScarto] M
	LEFT OUTER JOIN [dbo].[MassimariScarto] P on P.MassimarioScartoNode = M.MassimarioScartoParentNode
	WHERE M.IdMassimarioScarto = @massimarioId

	RETURN @fullCode
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

PRINT 'Aggiungo la colonna FullCode in MassimariScarto'
GO

ALTER TABLE [dbo].[MassimariScarto] ADD [FullCode] AS ([dbo].[GetFullCodeFromMassimarioScarto](IdMassimarioScarto))
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

PRINT 'Creo la tabella MassimariScartoSchema'
GO

CREATE TABLE [dbo].[MassimariScartoSchema](
	[IdMassimarioScartoSchema] [uniqueidentifier] NOT NULL,
	[IdMassimarioScarto] [uniqueidentifier] NOT NULL,
	[Version] [smallint] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_MassimariScartoSchema] PRIMARY KEY NONCLUSTERED 
(
	[IdMassimarioScartoSchema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_MassimariScartoSchema] UNIQUE NONCLUSTERED 
(
	[IdMassimarioScartoSchema] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_MassimariScartoSchema_RegistationDate]
    ON [dbo].[MassimariScartoSchema]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[MassimariScartoSchema]  WITH CHECK ADD  CONSTRAINT [FK_MassimariScartoSchema_MassimariScarto] FOREIGN KEY([IdMassimarioScarto])
REFERENCES [dbo].[MassimariScarto] ([IdMassimarioScarto])
GO

ALTER TABLE [dbo].[MassimariScartoSchema] CHECK CONSTRAINT [FK_MassimariScartoSchema_MassimariScarto]
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

PRINT 'Aggiungo la SP MassimarioScarto_Insert'
GO

CREATE PROCEDURE [dbo].[MassimarioScarto_Insert] 
	@IdMassimarioScarto uniqueidentifier, 
	@RegistrationDate datetimeoffset(7),
	@RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256),
	@Status smallint,
	@Name nvarchar(256), 
	@Code smallint, 
	@Note nvarchar(1024), 
	@ConservationPeriod smallint, 
	@StartDate datetimeoffset(7), 
	@EndDate datetimeoffset(7),
	@FakeInsertId uniqueidentifier
AS
BEGIN
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION

		-- Recupero il parent node
		IF @FakeInsertId IS NOT NULL
			BEGIN
				SELECT @parentNode = [MassimarioScartoNode] FROM [dbo].[MassimariScarto] WHERE [IdMassimarioScarto] = @FakeInsertId
			END
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode] = hierarchyid::GetRoot())
				BEGIN
					SET @parentNode = hierarchyid::GetRoot()
				END	
			END

		-- Recupero il max node in base al parent node
		SELECT @maxNode = MAX([MassimarioScartoNode]) FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode].GetAncestor(1) = @parentNode;

		IF @FakeInsertId IS NOT NULL			
			BEGIN
				SET @node = @parentNode.GetDescendant(@maxNode, NULL)
			END			
		ELSE
			BEGIN
				IF EXISTS(SELECT TOP 1 * FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode] = hierarchyid::GetRoot())
				BEGIN
					SET @node = hierarchyid::GetRoot().GetDescendant(@maxNode, NULL)
					PRINT @node.ToString()
				END	
				ELSE
				BEGIN
					SET @node = hierarchyid::GetRoot()
					PRINT @node.ToString()
				END
			END
	
		
		INSERT INTO [dbo].[MassimariScarto]([MassimarioScartoNode],[IdMassimarioScarto],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[Note],[ConservationPeriod],[StartDate],[EndDate]) 
		VALUES (@node, @IdMassimarioScarto, @RegistrationDate, @RegistrationUser, NULL, NULL, @Status, @Name, @Code, @Note, @ConservationPeriod, @StartDate, @EndDate)

		SELECT [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp] FROM [dbo].[MassimariScarto] WHERE [IdMassimarioScarto] = @IdMassimarioScarto
	COMMIT
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

PRINT 'Aggiungo la SP MassimarioScarto_Update'
GO

CREATE PROCEDURE [dbo].[MassimarioScarto_Update]
	@IdMassimarioScarto uniqueidentifier, 
	@RegistrationDate datetimeoffset(7),
	@RegistrationUser nvarchar(256),
	@LastChangedDate datetimeoffset(7),
	@LastChangedUser nvarchar(256),
	@Status smallint,
	@Name nvarchar(256), 
	@Code smallint, 
	@Note nvarchar(1024), 
	@ConservationPeriod smallint, 
	@StartDate datetimeoffset(7), 
	@EndDate datetimeoffset(7),
	@FakeInsertId uniqueidentifier,
	@Timestamp_Original timestamp
AS
BEGIN
	DECLARE @version smallint
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION
		-- Modifico i valori in MassimarioScarto
		UPDATE [dbo].[MassimariScarto] 
		SET [Name] = @Name, [Code] = @Code, [Note] = @Note, 
		[Status] = @Status, [ConservationPeriod] = @ConservationPeriod,
		[StartDate] = @StartDate, [EndDate] = @EndDate,
		[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser
		WHERE [IdMassimarioScarto] = @IdMassimarioScarto

		--Setto la EndDate verso i figli
		UPDATE [dbo].[MassimariScarto] SET [EndDate] = @EndDate
		WHERE [MassimarioScartoNode].GetAncestor(1) = (SELECT [MassimarioScartoNode]  
		FROM [dbo].[MassimariScarto]
		WHERE [IdMassimarioScarto] = @IdMassimarioScarto) AND ([EndDate] is null OR [EndDate] > @EndDate)
		
		SELECT [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp] FROM [dbo].[MassimariScarto] WHERE [IdMassimarioScarto] = @IdMassimarioScarto	
	COMMIT
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

PRINT 'Aggiungo la Function MassimarioScarto_RootChildren'
GO

CREATE FUNCTION [dbo].[MassimarioScarto_RootChildren]
(	
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(	 
	SELECT distinct [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp]
	FROM [dbo].[MassimariScarto]
	WHERE [MassimarioScartoNode].GetAncestor(1) = (SELECT [MassimarioScartoNode]  
	FROM [dbo].[MassimariScarto]
	WHERE [IdMassimarioScarto] = (SELECT [IdMassimarioScarto] FROM [dbo].[MassimariScarto] WHERE [MassimarioScartoNode] = hierarchyid::GetRoot()))
	AND ((@includeLogicalDelete = 0 AND ([EndDate] is null OR [EndDate] > GETDATE()) AND [Status] = 1) OR (@includeLogicalDelete = 1 AND [Status] in (0,1)))
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

PRINT 'Aggiungo la Function MassimarioScarto_GetMassimariFull'
GO

CREATE FUNCTION [dbo].[MassimarioScarto_GetMassimariFull]
(	
	@name nvarchar(256),
	@fullCode nvarchar(256) = NULL,
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT distinct M1.[MassimarioScartoNode],M1.[IdMassimarioScarto] as UniqueId,M1.[RegistrationDate],M1.[RegistrationUser],M1.[LastChangedDate],M1.[LastChangedUser],M1.[Status],
		M1.[Name],M1.[Code],M1.[FullCode],M1.[Note],M1.[ConservationPeriod],M1.[StartDate],M1.[EndDate],M1.[MassimarioScartoPath],M1.[MassimarioScartoLevel],M1.[MassimarioScartoParentPath],M1.[FakeInsertId],M1.[Timestamp]
    FROM (SELECT * FROM [dbo].[MassimariScarto]
			WHERE (@name IS NOT NULL AND @name != '' AND [Name] like '%'+@name+'%')
			OR (@fullCode IS NOT NULL AND [FullCode] = @fullCode)) M2,
    [dbo].[MassimariScarto] M1
    WHERE 
	[M2].[MassimarioScartoNode].IsDescendantOf(M1.MassimarioScartoNode) = 1
	AND M1.MassimarioScartoNode != hierarchyid::GetRoot()
	AND ((@includeLogicalDelete = 0 AND (M1.[EndDate] is null OR M1.[EndDate] > GETDATE()) AND M1.[Status] = 1) OR (@includeLogicalDelete = 1 AND M1.[Status] in (0,1)))
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

PRINT 'Aggiungo la Function MassimarioScarto_AllChildrenByParent'
GO

CREATE FUNCTION [dbo].[MassimarioScarto_AllChildrenByParent]
(	
	@parentId uniqueidentifier,
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(	 
	SELECT distinct [MassimarioScartoNode],[IdMassimarioScarto] as UniqueId,[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
		[Name],[Code],[FullCode],[Note],[ConservationPeriod],[StartDate],[EndDate],[MassimarioScartoPath],[MassimarioScartoLevel],[MassimarioScartoParentPath],[FakeInsertId],[Timestamp]
	FROM [dbo].[MassimariScarto]
	WHERE [MassimarioScartoNode].GetAncestor(1) = (SELECT [MassimarioScartoNode]  
	FROM [dbo].[MassimariScarto]
	WHERE [IdMassimarioScarto] = @parentId)
	AND ((@includeLogicalDelete = 0 AND ([EndDate] is null OR [EndDate] > GETDATE()) AND [Status] = 1) OR (@includeLogicalDelete = 1 AND [Status] in (0,1)))
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

PRINT 'Aggiungo la Function MassimarioScarto_GetMassimari'
GO

CREATE FUNCTION [dbo].[MassimarioScarto_GetMassimari]
(	
	@name nvarchar(256),
	@includeLogicalDelete bit
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT * FROM [dbo].[MassimarioScarto_GetMassimariFull] (@name, NULL, @includeLogicalDelete)
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

PRINT 'Inserisco il valore Root di default'
GO


DECLARE @IdMassimarioScarto uniqueidentifier
DECLARE @RegistrationDate datetimeoffset(7)
DECLARE @RegistrationUser nvarchar(256)
DECLARE @LastChangedDate datetimeoffset(7)
DECLARE @LastChangedUser nvarchar(256)
DECLARE @Status smallint
DECLARE @Name nvarchar(256)
DECLARE @Code smallint
DECLARE @Note nvarchar(1024)
DECLARE @ConservationPeriod smallint
DECLARE @StartDate datetimeoffset(7)
DECLARE @EndDate datetimeoffset(7)
DECLARE @FakeInsertId uniqueidentifier

SET @IdMassimarioScarto=NEWID()
SET @Status=1
SET @Name=N'Root'
SET @StartDate=GETDATE()
SET @RegistrationUser=N'anonymous_api'
SET @RegistrationDate=GETDATE()

EXECUTE [dbo].[MassimarioScarto_Insert] 
   @IdMassimarioScarto
  ,@RegistrationDate
  ,@RegistrationUser
  ,@LastChangedDate
  ,@LastChangedUser
  ,@Status
  ,@Name
  ,@Code
  ,@Note
  ,@ConservationPeriod
  ,@StartDate
  ,@EndDate
  ,@FakeInsertId
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
PRINT 'CREATE FUNCTION [dbo].[SplitString]'
GO

CREATE FUNCTION [dbo].[SplitString]
    (
        @List NVARCHAR(MAX),
        @Delim NVARCHAR(10)
    )
    RETURNS TABLE
    AS
        RETURN ( SELECT [Value] FROM 
          ( 
            SELECT 
              [Value] = LTRIM(RTRIM(SUBSTRING(@List, [Number],
              CHARINDEX(@Delim, @List + @Delim, [Number]) - [Number])))
            FROM (SELECT Number = ROW_NUMBER() OVER (ORDER BY name)
              FROM sys.all_objects) AS x
              WHERE Number <= LEN(@List)
              AND SUBSTRING(@Delim + @List, [Number], LEN(@Delim)) = @Delim
          ) AS y
        );
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
PRINT 'Modifica Function [AvailableFasciclesFromProtocol]'
GO

ALTER FUNCTION [dbo].[AvailableFasciclesFromProtocol]
(
	@UniqueIdProtocol uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate			
	FROM Fascicles F 
		LEFT JOIN FascicleProtocols FP on F.IdFascicle = FP.IdFascicle AND FP.UniqueIdProtocol = @UniqueIdProtocol
		LEFT JOIN Protocol P on P.UniqueId = @UniqueIdProtocol
		LEFT JOIN Category C on C.IdCategory = P.IdCategoryAPI
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 1 AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = P.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 1 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 1 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FP.IdFascicleProtocol IS NULL
		AND F.EndDate IS NULL
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate
);
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
PRINT 'Modifica Function [AvailableFasciclesFromResolution]'
GO

ALTER FUNCTION [dbo].[AvailableFasciclesFromResolution]
(
	@UniqueIdResolution uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate						
	FROM Fascicles F 
		LEFT JOIN FascicleResolutions FR on F.IdFascicle = FR.IdFascicle AND FR.UniqueIdResolution = @UniqueIdResolution
		LEFT JOIN Resolution R on R.UniqueId = @UniqueIdResolution
		LEFT JOIN Category C on C.IdCategory = R.IdCategoryAPI
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 2 AND CF.FascicleType in (1, 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = R.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 1 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 1 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FR.IdFascicleResolution IS NULL
		AND F.EndDate IS NULL
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate
);
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

--#############################################################################

PRINT 'Aggiungo la colonna IdMassimarioScarto nella tabella Category'
GO

ALTER TABLE [dbo].[Category] ADD [IdMassimarioScarto] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[Category]  WITH CHECK ADD  CONSTRAINT [FK_Category_MassimariScarto] FOREIGN KEY([IdMassimarioScarto])
REFERENCES [dbo].[MassimariScarto] ([IdMassimarioScarto])
GO

ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_MassimariScarto]
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

PRINT 'Creo la Tabella CategorySchemas'
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CategorySchemas](
	[idCategorySchema] [uniqueidentifier] NOT NULL,
	[Version] [smallint] NOT NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,
	[Note] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256)  NULL,
	[LastChangedDate] [datetimeoffset](7)  NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_CategorySchemas] PRIMARY KEY NONCLUSTERED 
(
	[idCategorySchema] ASC
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

PRINT 'Aggiunto clustered indice [IX_CategorySchema_RegistrationDate]';
GO

CREATE CLUSTERED INDEX [IX_CategorySchemas_RegistrationDate]
    ON [dbo].[CategorySchemas]([RegistrationDate] ASC);
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

PRINT 'Creo la colonna IdCategorySchema in Category'
GO

ALTER TABLE [dbo].[Category]
ADD [IdCategorySchema] uniqueidentifier NULL
GO

UPDATE [dbo].[Category]
SET [IdCategorySchema] = 'A4E50885-87EA-4DD2-988C-97F54EA5E645'
GO

ALTER TABLE [dbo].[Category]
ALTER COLUMN [IdCategorySchema] uniqueidentifier NOT NULL
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

PRINT 'Inserisco un record nella tabella CategorySchemas con Version=1'
GO

DECLARE @ProtocolRegistrationDate as datetimeoffset(7)

IF EXISTS(SELECT TOP (1) * FROM [dbo].[Protocol])
	SELECT TOP (1) @ProtocolRegistrationDate = [RegistrationDate]
			FROM [dbo].[Protocol]
			ORDER BY [RegistrationDate] ASC
ELSE
	SET @ProtocolRegistrationDate = GETDATE()

INSERT INTO [dbo].[CategorySchemas]
           ([IdCategorySchema]
           ,[Version]
           ,[StartDate]
           ,[Note]
           ,[RegistrationDate]
           ,[RegistrationUser])
     VALUES
           ('A4E50885-87EA-4DD2-988C-97F54EA5E645'
           ,1
           ,@ProtocolRegistrationDate
           ,'TITOLARIO DELL''ENTE'
           ,GETDATE()
           ,'system')
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

PRINT 'Creazione FK IdCategorySchema in Category'
GO

ALTER TABLE [dbo].[Category] WITH CHECK ADD CONSTRAINT [FK_Category_CategorySchemas] FOREIGN KEY([IdCategorySchema])
REFERENCES [dbo].[CategorySchemas] ([idCategorySchema])

ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_CategorySchemas]
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
PRINT 'Aggiungo la colonna [StartDate] nella tabella [Category]';
GO

ALTER TABLE [dbo].[Category] ADD [StartDate] datetimeoffset(7) NULL
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
PRINT 'Inserisco in StartDate la RegistrationDate del primo protocollo creato in DB';
GO

DECLARE @ProtocolRegistrationDate as datetimeoffset(7)

IF EXISTS(SELECT TOP (1) * FROM [dbo].[Protocol])
	SELECT TOP (1) @ProtocolRegistrationDate = [RegistrationDate]
			FROM [dbo].[Protocol]
			ORDER BY [RegistrationDate] ASC
ELSE
	SET @ProtocolRegistrationDate = GETDATE()

UPDATE [dbo].[Category] SET [StartDate] = @ProtocolRegistrationDate
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
PRINT 'Setto come NOT NULL la colonna StartDate della tabella Category';
GO

ALTER TABLE [dbo].[Category] ALTER COLUMN [StartDate] datetimeoffset(7) NOT NULL
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

PRINT 'Aggiungo la colonna [EndDate] nella tabella [Category]';
GO

ALTER TABLE [dbo].[Category] ADD [EndDate] datetimeoffset(7) NULL
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
PRINT 'Modificata colonna LogDescription in CollaborationLog';
GO

ALTER TABLE [dbo].[CollaborationLog] ALTER COLUMN [LogDescription] NVARCHAR(MAX) NULL
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
--#############################################################################
PRINT 'Creazione tabella WorkflowRoleMappings'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowRoleMappings](
	[IdWorkflowRoleMapping] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NOT NULL,
	[MappingTag] [nvarchar](100) NOT NULL,
	[AuthorizationType] [smallint] NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL	
 CONSTRAINT [PK_WorkflowRoleMappings] PRIMARY KEY NONCLUSTERED 
(
	[IdWorkflowRoleMapping] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkflowRoleMappings]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowRoleMappings_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([idRole])
GO
ALTER TABLE [dbo].[WorkflowRoleMappings] CHECK CONSTRAINT [FK_WorkflowRoleMappings_Role]
GO

ALTER TABLE [dbo].[WorkflowRoleMappings]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowRoleMappings_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
ALTER TABLE [dbo].[WorkflowRoleMappings] CHECK CONSTRAINT [FK_WorkflowRoleMappings_WorkflowRepositories]
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

PRINT 'Aggiunto clustered indice [IX_WorkflowRoleMappings_RegistrationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowRoleMappings_RegistrationDate]
    ON [dbo].[WorkflowRoleMappings]([RegistrationDate] ASC);
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
PRINT N'Aggiunto nonclustered indice [IX_WorkflowRoleMappings_IdWorkflowRepository_IdRole_MappingTag] in WorkflowRoleMapping';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_WorkflowRoleMappings_IdWorkflowRepository_IdRole_MappingTag] 
	ON [dbo].[WorkflowRoleMappings]([IdWorkflowrepository] ASC, [IdRole] ASC, [MappingTag] ASC)
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
PRINT 'Aggiungo la colonna [Subject] nella tabella [WorkflowActivities]';
GO

ALTER TABLE [dbo].[WorkflowActivities] ADD [Subject] nvarchar(500) NULL
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
PRINT N'Aggiungo la colonna IdWorkflowInstance';
GO

ALTER TABLE [dbo].[Collaboration] ADD [IdWorkflowInstance] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[Collaboration]  WITH CHECK ADD  CONSTRAINT [FK_Collaboration_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[Collaboration] CHECK CONSTRAINT [FK_Collaboration_WorkflowInstances]
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
PRINT N'Modifico la colonna IsActive della tabella ProtocolDraft';
GO

ALTER TABLE [dbo].[ProtocolDraft] ALTER COLUMN [IsActive] [bit] NOT NULL
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ProtocolDraft]';
GO

ALTER TABLE [dbo].[ProtocolDraft] ADD [UniqueId] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[ProtocolDraft] ADD CONSTRAINT [DF_ProtocolDraft_UniqueId] 
	DEFAULT NEWSEQUENTIALID() FOR [UniqueId];
GO

UPDATE [dbo].[ProtocolDraft] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL
GO

ALTER TABLE [dbo].[ProtocolDraft] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
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

PRINT 'Creato indice univoco [IX_ProtocolDraft_UniqueId] nella tabella [ProtocolDraft]';
GO

CREATE UNIQUE INDEX [IX_ProtocolDraft_UniqueId] ON [dbo].[ProtocolDraft]([UniqueId] ASC);
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

PRINT 'Aggiunta colonna [Timestamp] nella tabella [ProtocolDraft]';
GO

ALTER TABLE [dbo].[ProtocolDraft] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT 'Aggiunto clustered indice [IX_WorkflowRoleMappings_RegistrationDate]';
GO

ALTER TABLE [dbo].[WorkflowInstances] ADD [Json] [nvarchar](max) NULL;

GO

UPDATE [dbo].[WorkflowInstances] SET [Json] = (SELECT [Json] FROM [dbo].[WorkflowRepositories] WHERE [WorkflowInstances].[IdWorkflowRepository] = [WorkflowRepositories].[IdWorkflowRepository]);

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

PRINT 'Aggiunta colonna [UniqueIdDocumentSeriesItem] nella tabella [DocumentSeriesItemRole]';
GO

ALTER TABLE [DocumentSeriesItemRole] ADD [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NULL
GO

UPDATE DSIR SET DSIR.UniqueIdDocumentSeriesItem = DSI.UniqueId
FROM [dbo].[DocumentSeriesItemRole] AS DSIR
INNER JOIN [dbo].[DocumentSeriesItem] DSI ON DSI.Id = DSIR.IdDocumentSeriesItem
GO

ALTER TABLE [DocumentSeriesItemRole] ALTER COLUMN [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NOT NULL
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

PRINT 'Bonifica FOREIGN KEY [FK_FascicleDocumentSeriesItems_DocumentSeriesItem]';
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] DROP CONSTRAINT [FK_FascicleDocumentSeriesItems_DocumentSeriesItem]
GO

UPDATE [DocumentSeriesItem] SET [UniqueId] = newid()
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] ADD [UniqueIdDocumentSeriesItem] UNIQUEIDENTIFIER NOT NULL
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems]  WITH CHECK ADD  CONSTRAINT [FK_FascicleDocumentSeriesItems_DocumentSeriesItem] FOREIGN KEY([UniqueIdDocumentSeriesItem])
REFERENCES [dbo].[DocumentSeriesItem] ([UniqueId])
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] CHECK CONSTRAINT [FK_FascicleDocumentSeriesItems_DocumentSeriesItem]
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
PRINT N'Aggiunto unique index [IX_IdFascicle_IdDocumentSeriesItem_ReferenceType] in [FascicleDocumentSeriesItems]';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FascicleDocumentSeriesItems_IdFascicle_UniqueIdDocumentSeriesItem_ReferenceType] 
	ON [dbo].[FascicleDocumentSeriesItems]([IdFascicle] ASC, [UniqueIdDocumentSeriesItem] ASC, [ReferenceType] ASC)
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

PRINT N'Aggiunto unique index [IX_FascicleUDS_IdFascicle_IdUDS_ReferenceType] in [FascicleUDS]';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FascicleUDS_IdFascicle_IdUDS_ReferenceType] 
	ON [dbo].[FascicleUDS]([IdFascicle] ASC, [IdUDS] ASC, [ReferenceType] ASC)
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

PRINT N'Aggiunto constraint [CHK_UDSRepositories_Status_IdContainer] in [UDSRepositories]';
GO

ALTER TABLE [uds].[UDSRepositories] 
ADD CONSTRAINT [CHK_UDSRepositories_Status_IdContainer] CHECK (NOT(Status = 2 AND IdContainer IS NULL));
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
--#############################################################################

PRINT 'CREATE FUNCTION [dbo].[CategoryHierarcyCode]'
GO

CREATE FUNCTION [dbo].[CategoryHierarcyCode](@IdCategory AS SMALLINT)
    RETURNS NVARCHAR(1000) AS
BEGIN
	DECLARE @Names NVARCHAR(1000) 
	SELECT @Names = COALESCE(@Names + '.','') + CAST(Code AS NVARCHAR(5))
	FROM [dbo].[SplitString]((SELECT TOP 1 FullIncrementalPath FROM [dbo].[Category] WHERE IdCategory = @IdCategory),'|') INNER JOIN [dbo].[Category] ON [Value] = IdCategory 
	ORDER BY [FullCode]
    RETURN @Names
END;
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

PRINT 'CREATE FUNCTION [dbo].[CategoryHierarcyDescription]'
GO

CREATE FUNCTION [dbo].[CategoryHierarcyDescription](@IdCategory as smallint)
    RETURNS NVARCHAR(2000) AS
BEGIN
	DECLARE @Names NVARCHAR(2000) 
	SELECT @Names = COALESCE(@Names + ', ','') + CAST(Code AS NVARCHAR(5)) + '.' + CAST(Name AS NVARCHAR(256))
	FROM [dbo].[SplitString]((SELECT TOP 1 FullIncrementalPath FROM [dbo].[Category] WHERE IdCategory = @IdCategory),'|') INNER JOIN [dbo].[Category] on [Value] = IdCategory 
	ORDER BY [FullCode]

    RETURN @Names
END;
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

PRINT 'CREATE FUNCTION [dbo].[GetProtocolRoleModels]'
GO

CREATE FUNCTION [dbo].[GetProtocolRoleModels]
(
	@ProtocolYear smallint, @ProtocolNumber int
)
RETURNS TABLE 
AS
RETURN 
(
WITH tblChild AS
(
    SELECT R.UniqueId, R.IdRole, R.Name, R.idRoleFather, PR.DistributionType, PR.[Type], 1 as IsAuthorized
        FROM [dbo].[ProtocolRole] PR 
		INNER JOIN [dbo].[Role] R ON R.idRole = PR.idRole AND PR.Year = @ProtocolYear  and PR.Number = @ProtocolNumber
		GROUP BY R.UniqueId, R.IdRole, R.idRoleFather, R.Name, PR.DistributionType, PR.[Type]
    UNION ALL
    SELECT R.UniqueId, R.IdRole, R.Name, R.idRoleFather, NULL, NULL,  0 as IsAuthorized
		FROM [dbo].[Role] R 
		INNER JOIN tblChild ON R.IdRole = tblChild.idRoleFather
),
Results AS 
(SELECT UniqueId, IdRole, Name, (SELECT TOP 1 I.UniqueId FROM tblChild I WHERE I.idRole = tblChild.idRoleFather) AS UniqueIdFather, 
	DistributionType, [Type], IsAuthorized
    FROM tblChild)

SELECT UniqueId, IdRole, Name, UniqueIdFather, DistributionType, [Type], CAST(MAX(IsAuthorized) as bit) AS IsAuthorized
    FROM Results
	GROUP BY UniqueId, IdRole, Name, UniqueIdFather, DistributionType, [Type]
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

PRINT 'CREATE FUNCTION [dbo].[GetProtocolContactModels]'
GO


CREATE FUNCTION [dbo].[GetProtocolContactModels]
(
	@ProtocolYear smallint, @ProtocolNumber int
)
RETURNS TABLE 
AS
RETURN 
(
WITH tblChild AS
(
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, PC.ComunicationType, PC.[Type], 1 as IsSelected
        FROM [dbo].[ProtocolContact] PC 
		INNER JOIN [dbo].[Contact] C ON C.Incremental = PC.IDContact AND PC.Year = @ProtocolYear  and PC.Number = @ProtocolNumber
		GROUP BY C.UniqueId, C.Incremental, C.IncrementalFather, C.IdContactType, C.[Description], PC.ComunicationType, PC.[Type]
    UNION ALL
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, tblChild.ComunicationType, NULL, 0 as IsSelected
		FROM [dbo].[Contact] C 
		INNER JOIN tblChild ON C.Incremental = tblChild.IncrementalFather
),
Results As
(SELECT UniqueId, Incremental as IdContact, IdContactType AS ContactType, [Description], (SELECT TOP 1 I.UniqueId FROM tblChild I WHERE I.Incremental = tblChild.IncrementalFather) AS UniqueIdFather, 
	ComunicationType, [Type], IsSelected
    FROM tblChild
UNION ALL
SELECT UniqueId, NULL, 'M'+PCM.IdContactType,PCM.[Description],NULL,PCM.ComunicationType,PCM.[Type],CAST(1 as bit) AS IsSelected
	FROM ProtocolContactManual PCM
	WHERE PCM.[Year] = @ProtocolYear AND PCM.[Number] = @ProtocolNumber
)
SELECT UniqueId, IdContact, ContactType, [Description], UniqueIdFather, ComunicationType, [Type], CAST(MAX(IsSelected) as bit) AS IsSelected
FROM Results
GROUP BY UniqueId, IdContact, ContactType, [Description], UniqueIdFather, ComunicationType, [Type]
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

PRINT 'ALTER FUNCTION [dbo].[RegisteredCollaborations]'
GO

ALTER FUNCTION [dbo].[RegisteredCollaborations](
	@UserName nvarchar(255), 
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset)
	RETURNS TABLE
AS 
	RETURN
	(
		WITH CollaborationTableValue AS(
            SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
                     Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
                     Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
                     Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
                     Collaboration.LastChangedDate, Collaboration.idDocumentSeriesItem                                          
            FROM dbo.Collaboration Collaboration                                   
			left outer join dbo.CollaborationSigns s4_ on Collaboration.IdCollaboration = s4_.IdCollaboration and (s4_.SignUser = @UserName)
            left outer join dbo.CollaborationUsers u5_ on Collaboration.IdCollaboration = u5_.IdCollaboration and (u5_.Account = @UserName)
            left outer join dbo.RoleUser RU_C1 on Collaboration.RegistrationUser = RU_C1.Account AND RU_C1.Account = @UserName AND RU_C1.[Enabled] = 1   
            left outer join dbo.CollaborationUsers RU_C2 on Collaboration.IdCollaboration = RU_C2.IdCollaboration and RU_C2.DestinationType = 'S'
            left outer join dbo.RoleUser RU_C2_S on RU_C2.IdRole = RU_C2_S.IdRole and RU_C2_S.Account = @UserName AND RU_C2_S.[Enabled] = 1 
            left outer join dbo.CollaborationSigns RU_C3 on Collaboration.IdCollaboration = RU_C3.IdCollaboration
            left outer join dbo.RoleUser RU_C3_S on RU_C3.SignUser = RU_C3_S.Account and RU_C3_S.Account = @UserName AND RU_C3_S.[Enabled] = 1                   
            WHERE Collaboration.RegistrationDate between @DateFrom and @DateTo AND
            (1 = 0 or Collaboration.RegistrationUser = @UserName
                              or s4_.IdCollaborationSign is not null
                              or u5_.IdCollaborationUser is not null
                              or RU_C1.IdRole is not null
                              or RU_C2_S.Incremental is not null
                              or RU_C3_S.Incremental is not null)       
                              and Collaboration.IdStatus = 'PT' and Collaboration.DocumentType in ('P', 'D', 'A', 'O', 'S', 'U', 'W')
			GROUP BY Collaboration.IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
                     Collaboration.MemorandumDate, Collaboration.Object, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
                     Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
                     Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
                     Collaboration.LastChangedDate, Collaboration.idDocumentSeriesItem
			)

            SELECT Collaboration.*, C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
					C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
                    C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
                    r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
                    dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
			FROM CollaborationTableValue Collaboration
            inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
            inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
            inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
            left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
            left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
            left outer join dbo.Protocol p1_ on Collaboration.Year = p1_.Year and Collaboration.Number = p1_.Number
            WHERE (p1_.RegistrationDate between @DateFrom and @DateTo and p1_.idStatus = 0)
						or (r2_.ProposeDate between @DateFrom and @DateTo and r2_.idStatus = 0)
                        or (dsi3_.RegistrationDate between @DateFrom and @DateTo and dsi3_.Status in (1, 3))   
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

PRINT 'ALTER FUNCTION [dbo].[ToManageCollaborations]'
GO

ALTER FUNCTION [dbo].[ToManageCollaborations](
	@UserName nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
		WHERE  Collaboration.IdStatus = 'DP'
			   and (1 = (SELECT TOP (1) 1
						   FROM   dbo.CollaborationUsers C_CU
						   WHERE  C_CU.IdCollaboration = Collaboration.IdCollaboration
								  and C_CU.Account = @UserName
								  and C_CU.DestinationType = 'P')
					and 1 = 1
					 OR EXISTS (SELECT Account
								FROM   RoleUser
								WHERE  Type = 'S'
									   AND Enabled = 1
									   AND Account = @UserName
									   AND IdRole IN (SELECT IdRole
													  FROM   CollaborationUsers
													  WHERE  IdCollaboration = Collaboration.IdCollaboration
															 AND DestinationType = 'S'
															 and DestinationFirst = 1)
									   AND Account NOT IN (SELECT Account
														   FROM   CollaborationUsers
														   WHERE  IdCollaboration = Collaboration.IdCollaboration
																  AND DestinationType = 'P'
																  and DestinationFirst = 1)))
			   and Collaboration.DocumentType in ('P', 'D', 'A', 'O', 'S', 'U', 'W')
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

PRINT N'Creazione TYPE [string_list_tbltype]';
GO

CREATE TYPE string_list_tbltype AS TABLE (val NVARCHAR(256) NOT NULL)
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

PRINT N'Creazione function [CurrentActivityAllCollaborations]';
GO

CREATE FUNCTION [dbo].[CurrentActivityAllCollaborations](
	@UserName nvarchar(255),
	@Signers string_list_tbltype READONLY)
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			inner join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration
		WHERE ((CC_CS.SignUser = @UserName AND CC_CS.IsActive <> 1) AND Collaboration.IdStatus NOT IN ('PT', 'DP')) 
		      OR (Collaboration.IdStatus = 'IN' AND CC_CS.SignUser IN (SELECT val FROM @Signers))
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

PRINT N'Creazione function [CurrentActivityActiveCollaborations]';
GO

CREATE FUNCTION [dbo].[CurrentActivityActiveCollaborations](
	@UserName nvarchar(255),
	@Signers string_list_tbltype READONLY)
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			inner join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration
		WHERE ((CC_CS.SignUser = @UserName AND CC_CS.IsActive = 1) 
					AND Collaboration.IdStatus <> 'PT') 
		      OR (Collaboration.IdStatus = 'IN' AND CC_CS.SignUser IN (SELECT val FROM @Signers))
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

PRINT N'Creazione function [CurrentActivityPastCollaborations]';
GO

CREATE FUNCTION [dbo].[CurrentActivityPastCollaborations](
	@UserName nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
			inner join dbo.CollaborationSigns C_CSM on Collaboration.IdCollaboration = C_CSM.IdCollaboration AND C_CSM.IsActive = 1
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
		WHERE ((C_CS.SignUser = @UserName AND C_CS.IsActive <> 1 AND C_CS.Incremental < C_CSM.Incremental) 
					AND Collaboration.IdStatus <> 'PT')
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

PRINT N'Creazione function [ProtocolAdmissionCollaborations]';
GO

CREATE FUNCTION [dbo].[ProtocolAdmissionCollaborations](
	@UserName nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration			
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			inner join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration
		WHERE	(Collaboration.RegistrationUser = @UserName OR CC_CS.SignUser = @UserName) AND Collaboration.IdStatus = 'DP'
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

PRINT N'Creazione function [IsCheckedOutCollaborations]';
GO

CREATE FUNCTION [dbo].[IsCheckedOutCollaborations](
	@UserName nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration			
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
		WHERE	C_CV.CheckedOut = 1 AND C_CV.CheckOutUser = @UserName
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
PRINT N'Creazione function [DocumentUnit_HasViewableDocument]';
GO

CREATE FUNCTION [dbo].[DocumentUnit_HasViewableDocument] 
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@IdDocumentUnit uniqueidentifier
)
RETURNS BIT
AS
BEGIN
    declare @HasRight bit;

	WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
        GROUP BY SG.IdGroup
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
	WHERE DU.IdDocumentUnit = @IdDocumentUnit
	
	and ( exists ( select top 1 CG.idContainerGroup
				 from [dbo].[ContainerGroup] CG 
				 INNER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
				 where CG.IdContainer = DU.IdContainer AND 
				 C_MSG.IdGroup IS NOT NULL AND ((DU.Environment = 1 AND (CG.Rights like '__1%'))
					OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))))

	 OR exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
				where  RG.IdRoleGroup = DUR.UniqueIdRole AND
				MSG.IdGroup IS NOT NULL AND ((DU.Environment = 1 AND (RG.ProtocolRights like '__1%'))
				OR (DU.Environment = 2 AND (RG.ResolutionRights like '__1%'))
				OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '__1%')))
				 ))
    
	RETURN @HasRight;


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
PRINT N'Creazione function [PeriodicFasciclesFromDocumentUnit]';
GO

CREATE FUNCTION [dbo].[PeriodicFasciclesFromDocumentUnit]
(
	@UniqueIdUD uniqueidentifier,
	@Environment smallint
)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate			
	FROM Fascicles F 
		LEFT JOIN cqrs.DocumentUnits D on D.IdDocumentUnit = @UniqueIdUD
		LEFT JOIN Category C on C.IdCategory = D.IdCategory
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 1 AND CF.FascicleType in (2 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE ((F.IdCategory = D.IdCategory or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 1 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = @Environment and FascicleType = 2
			ORDER BY IdCategory DESC))))
			AND ((D.RegistrationDate BETWEEN F.StartDate AND F.EndDate) 
					OR (F.EndDate IS NULL AND D.RegistrationDate >= F.StartDate))
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate
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