/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<UTENTE_DEFAULT, varchar(256),>	--> Settare il nome dell'utente      															*
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

PRINT 'Aggiunta colonna [UniqueId] nella tabella [ParameterEnv]';
GO

UPDATE [dbo].[ParameterEnv] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL
GO

ALTER TABLE [dbo].[ParameterEnv] ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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

UPDATE [dbo].[ParameterEnv] SET [RegistrationDate] = GETUTCDATE()
GO

ALTER TABLE [dbo].[ParameterEnv] ALTER COLUMN [RegistrationDate] [datetimeoffset](7) NOT NULL;
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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

UPDATE [dbo].[ParameterEnv] SET [RegistrationUser] = '<UTENTE_DEFAULT, varchar(256),>'
GO

ALTER TABLE [dbo].[ParameterEnv] ALTER COLUMN [RegistrationUser] [nvarchar](256) NOT NULL;
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'La colonna [UniqueId] nella tabella [RoleUser] viene resa NOT NULL';
GO

ALTER TABLE [dbo].[RoleUser]
ALTER COLUMN [UniqueId] [uniqueidentifier] NOT NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Modifica valori della colonna TemplateName nella tabella [dbo].[Collaboration]';
GO

UPDATE [C] SET [C].[TemplateName] = [TC].[Name]
FROM [dbo].[Collaboration] C
INNER JOIN [dbo].[TemplateCollaborations] TC ON [C].[DocumentType] = [TC].[DocumentType] AND [TC].[IsLocked] = 1
WHERE [C].[TemplateName] IS NULL
GO

UPDATE [Collaboration] SET [TemplateName] = 'Workflow'  WHERE [DocumentType]='W'
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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

ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [TemplateName] [nvarchar](256) NOT NULL;
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'Aggiornamento dello Schema UDS';
GO
update uds.UDSSchemaRepositories set SchemaXML = '
<xs:schema id="SchemaUnitaDocumentariaSpecifica"
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
        <xs:attribute name="IsSignature" type="xs:boolean" use="optional" default="false"/>
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
      <xs:attribute name="SignatureMetadataEnabled" type="xs:boolean" use="optional" default="false" />
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