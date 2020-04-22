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