/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<DBProtocollo, varcahr(50), DBProtocollo>  --> Settare il nome del DB di protocollo.				        						*
*	<DBPratiche, varcahr(50), DBPratiche>  --> Se esiste il DB di Pratiche settare il nome.					    					*
*	<DBAtti, varcahr(50), DBAtti>			   --> Se esiste il DB di Atti settare il nome.												*
*	<ESISTE_DB_ATTI, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva		*
*	<ESISTE_DB_PRATICHE, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
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
PRINT 'Versionamento database alla 8.79'
GO

EXEC dbo.VersioningDatabase N'8.79'
GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT 'GENERATE SPECIAL ROLE TO GENERIC EVALUATION FASCICLE RIGHT';
GO
DECLARE @LastUsedidRole smallint
DECLARE @IdSecurityGroup int 
DECLARE @GroupName nvarchar(256)
SELECT  @LastUsedidRole = [LastUsedidRole]+1 FROM [dbo].[Parameter]
SELECT  top 1 @IdSecurityGroup =idGroup,@GroupName=GroupName FROM [dbo].[SecurityGroups] where SecurityGroups.AllUsers = 1

INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Role]
           ([idRole]
           ,[IdRoleTenant]
           ,[UniqueId]
           ,[idRoleFather]
           ,[Name]
           ,[isActive]
           ,[isChanged]
           ,[TenantId]
           ,[FullIncrementalPath]
           ,[EMailAddress]
           ,[UriSharepoint]
           ,[ServiceCode]
           ,[Collapsed]
           ,[ActiveFrom]
           ,[ActiveTo]
           ,[RegistrationUser]
           ,[RegistrationDate])
     VALUES
           (@LastUsedidRole
           ,@LastUsedidRole
           ,'00000000-0000-0000-0000-000000000000'
           ,null
           ,'settore speciale per la fascicolazione'
           ,1
           ,0
           ,'00000000-0000-0000-0000-000000000000'
           ,@LastUsedidRole
           ,''
           ,''
           ,''
           ,0
           ,null
           ,null
           ,'SYSTEM'
           ,GETUTCDATE())
INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[RoleGroup]
           ([IdRoleGroup]
           ,[idRole]
           ,[idGroup]
           ,[GroupName]
           ,[ProtocolRights]
           ,[DocumentSeriesRights]
           ,[DocumentRights]
           ,[ResolutionRights]
           ,[RegistrationUser]
           ,[RegistrationDate])
     VALUES
           (NEWID()
           ,@LastUsedidRole
           ,@IdSecurityGroup
           ,@GroupName
           ,'11100000000000000000'
           ,'10000000000000000000'
           ,'11100000000000000000'
           ,'10000000000000000000'
           ,'SYSTEM'
           ,GETUTCDATE())
UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter] SET [LastUsedidRole] = @LastUsedidRole

IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
BEGIN 
	INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[Role]
				([idRole]
				,[IdRoleTenant]
				,[UniqueId]
				,[idRoleFather]
				,[Name]
				,[isActive]
				,[isChanged]
				,[TenantId]
				,[FullIncrementalPath]
				,[EMailAddress]
				,[UriSharepoint]
				,[ServiceCode]
				,[Collapsed]
				,[ActiveFrom]
				,[ActiveTo]
				,[RegistrationUser]
				,[RegistrationDate])
			VALUES
				(@LastUsedidRole
				,@LastUsedidRole
				,'00000000-0000-0000-0000-000000000000'
				,null
				,'settore speciale per la fascicolazione'
				,1
				,0
				,'00000000-0000-0000-0000-000000000000'
				,@LastUsedidRole
				,''
				,''
				,''
				,0
				,null
				,null
				,'SYSTEM'
				,GETUTCDATE())
	INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[RoleGroup]
				([IdRoleGroup]
				,[idRole]
				,[idGroup]
				,[GroupName]
				,[ProtocolRights]
				,[DocumentSeriesRights]
				,[DocumentRights]
				,[ResolutionRights]
				,[RegistrationUser]
				,[RegistrationDate])
			VALUES
				(NEWID()
				,@LastUsedidRole
				,@IdSecurityGroup
				,@GroupName
				,'11100000000000000000'
				,'10000000000000000000'
				,'11100000000000000000'
				,'10000000000000000000'
				,'SYSTEM'
				,GETUTCDATE())
	UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Parameter] SET [LastUsedidRole] = @LastUsedidRole
END

IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
BEGIN 
	INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[Role]
			   ([idRole]
			   ,[IdRoleTenant]
			   ,[UniqueId]
			   ,[idRoleFather]
			   ,[Name]
			   ,[isActive]
			   ,[isChanged]
			   ,[TenantId]
			   ,[FullIncrementalPath]
			   ,[EMailAddress]
			   ,[UriSharepoint]
			   ,[ServiceCode]
			   ,[Collapsed]
			   ,[ActiveFrom]
			   ,[ActiveTo]
			   ,[RegistrationUser]
			   ,[RegistrationDate])
		 VALUES
			   (@LastUsedidRole
			   ,@LastUsedidRole
			   ,'00000000-0000-0000-0000-000000000000'
			   ,null
			   ,'settore speciale per la fascicolazione'
			   ,1
			   ,0
			   ,'00000000-0000-0000-0000-000000000000'
			   ,@LastUsedidRole
			   ,''
			   ,''
			   ,''
			   ,0
			   ,null
			   ,null
			   ,'SYSTEM'
			   ,GETUTCDATE())
	INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[RoleGroup]
			   ([IdRoleGroup]
			   ,[idRole]
			   ,[idGroup]
			   ,[GroupName]
			   ,[ProtocolRights]
			   ,[DocumentSeriesRights]
			   ,[DocumentRights]
			   ,[ResolutionRights]
			   ,[RegistrationUser]
			   ,[RegistrationDate])
		 VALUES
			   (NEWID()
			   ,@LastUsedidRole
			   ,@IdSecurityGroup
			   ,@GroupName
			   ,'11100000000000000000'
			   ,'10000000000000000000'
			   ,'11100000000000000000'
			   ,'10000000000000000000'
			   ,'SYSTEM'
			   ,GETUTCDATE())
	UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Parameter] SET [LastUsedidRole] = @LastUsedidRole
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
PRINT N'ALTER TABLE [dbo].[Role] DROP COLUMN [DocumentSeriesEnabled]';
GO

ALTER TABLE [dbo].[Role] DROP COLUMN [DocumentSeriesEnabled]

GO
IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Creazione Tabella WorkflowEvaluationProperties'
GO
CREATE TABLE [dbo].[WorkflowEvaluationProperties] (
	[IdWorkflowEvaluationProperty] [uniqueidentifier] NOT NULL,
	[IdWorkflowRepository] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[WorkflowType] [smallint] NOT NULL,
	[PropertyType] [smallint] NOT NULL,​
	[ValueInt] [bigint] NULL,​
	[ValueDate] [datetime] NULL,​
	[ValueDouble] [float] NULL,​
	[ValueBoolean] [bit] NULL,​
	[ValueGuid] [uniqueidentifier] NULL,
	[ValueString] [nvarchar](max) NULL,​
	[RegistrationUser] [nvarchar](256) NOT NULL,​
	[RegistrationDate] [datetimeoffset](7) NOT NULL,​
	[LastChangedUser] [nvarchar](256) NULL,​
	[LastChangedDate] [datetimeoffset](7) NULL,​
	[Timestamp] [timestamp] NOT NULL​
CONSTRAINT [PK_WorkflowEvaluationProperties] PRIMARY KEY NONCLUSTERED
	(
	[IdWorkflowEvaluationProperty] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_WorkflowEvaluationProperties_RegistrationDate] ON [dbo].[WorkflowEvaluationProperties]
(
[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WorkflowEvaluationProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowEvaluationProperties_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
      
CREATE UNIQUE INDEX [IX_WorkflowEvaluationProperties_IdWorkflowRepository_Name_WorkflowType] 
	ON [dbo].[WorkflowEvaluationProperties] ([IdWorkflowRepository], [Name], [WorkflowType])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Aggiungere la colonna IdContainer nella tabella Fascicles'
GO
ALTER TABLE [dbo].[Fascicles]
ADD [IdContainer] [smallint] NULL
CONSTRAINT [FK_Fascicles_Containers] FOREIGN KEY([IdContainer]) REFERENCES [dbo].[Container] ([IdContainer])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Aggiungere la colonna FascicleRights nella tabella CategoryGroup'
GO
ALTER TABLE [dbo].[ContainerGroup]
ADD [FascicleRights] [nvarchar](20) NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Aggiungere la colonna FascicleRights nella tabella RoleGroup'
GO

ALTER TABLE [dbo].[RoleGroup]
ADD [FascicleRights] [nvarchar](20) NULL
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'Creazione Tabella CategoryFascicleRights'
GO

CREATE TABLE [dbo].[CategoryFascicleRights] (
	[IdCategoryFascicleRight] [uniqueidentifier] NOT NULL,​
	[IdCategoryFascicle] [uniqueidentifier] NOT NULL,
	[IdRole] [smallint] NULL,
	[IdContainer] [SMALLINT] NULL,​
	[RegistrationUser] [nvarchar](256) NOT NULL,​
	[RegistrationDate] [datetimeoffset](7) NOT NULL,​
	[LastChangedUser] [nvarchar](256) NULL,​
	[LastChangedDate] [datetimeoffset](7) NULL,​
	[Timestamp] [timestamp] NOT NULL
	CONSTRAINT [PK_CategoryFascicleRights] PRIMARY KEY NONCLUSTERED
	(
		[IdCategoryFascicleRight] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)ON [PRIMARY]
GO

ALTER TABLE [dbo].[CategoryFascicleRights]  WITH CHECK ADD  CONSTRAINT [FK_CategoryFascicleRights_CategoryFascicle] FOREIGN KEY([IdCategoryFascicle])
REFERENCES [dbo].[CategoryFascicles] ([IdCategoryFascicle])
GO

ALTER TABLE [dbo].[CategoryFascicleRights]  WITH CHECK ADD  CONSTRAINT [FK_CategoryFascicleRights_Role] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Role] ([IdRole])
GO

ALTER TABLE [dbo].[CategoryFascicleRights]  WITH CHECK ADD  CONSTRAINT [FK_CategoryFascicleRights_Container] FOREIGN KEY([IdContainer])
REFERENCES [dbo].[Container] ([IdContainer])
GO

CREATE CLUSTERED INDEX [IX_CategoryFascicleRights_RegistrationDate] ON [dbo].[CategoryFascicleRights]
(
[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_CategoryFascicleRights_IdRole_IdContainer_IdCategoryFascicle] ON [dbo].[CategoryFascicleRights]
(
	[IdRole] ASC,
	[IdContainer] ASC,
	[IdCategoryFascicle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CategoryFascicleRights] WITH CHECK ADD
CONSTRAINT [CHK_CategoryFascicleRights_IdRole_IdContainer] CHECK  ((NOT ([IdContainer] IS NULL AND [IdRole] IS NULL) AND NOT ([IdContainer] IS NOT NULL AND [IdRole] IS NOT NULL)))
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'UPDATE [uds].[UDSSchemaRepositories] SET [SchemaXML]';
GO

UPDATE [uds].[UDSSchemaRepositories] SET [SchemaXML] = '<xs:schema xmlns="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd" xmlns:mstns="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd" xmlns:xs="http://www.w3.org/2001/XMLSchema" id="SchemaUnitaDocumentariaSpecifica" targetNamespace="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd" elementFormDefault="qualified">
  <!-- Tipi base -->
  <xs:simpleType name="guid">
    <xs:annotation>
      <xs:documentation xml:lang="en">

        The representation of a GUID, generally the id of an element.

      </xs:documentation>
    </xs:annotation>
    <xs:restriction base="xs:token">
      <xs:pattern value="[\da-fA-F]{8}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{12}"/>
    </xs:restriction>
  </xs:simpleType>
  <xs:complexType name="BaseType">
    <xs:attribute name="Label" type="xs:token" use="required" />
    <xs:attribute name="ClientId" type="xs:token" use="optional" default="0" />
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required" />
    <xs:attribute name="Searchable" type="xs:boolean" use="required" />
    <xs:attribute name="ModifyEnabled" type="xs:boolean" use="required" />
  </xs:complexType>
  <xs:complexType name="DynamicBaseType">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:sequence>
          <xs:element name="Layout" type="LayoutPosition" minOccurs="0" maxOccurs="1" />
          <xs:element name="JavaScript" type="JavaScript" minOccurs="0" maxOccurs="1" />
          <xs:element name="CustomCSS" type="xs:string" minOccurs="0" maxOccurs="1" />
          <xs:element name="CSSClassName" type="xs:token" minOccurs="0" maxOccurs="1" />
        </xs:sequence>
        <xs:attribute name="Required" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name="FieldBaseType">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:attribute name="ColumnName" type="xs:token" use="required" />
        <xs:attribute name="HiddenField" type="xs:boolean" use="required" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
        <xs:attribute name="ResultPosition" type="xs:short" use="required" />
        <xs:attribute name="Format" type="xs:token" use="optional" />
        <xs:attribute name="ConservationPosition" type="xs:short" use="optional" />
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
      <xs:element name="DocumentDematerialisation" type="Document" minOccurs="0" maxOccurs="1" />
    </xs:all>
  </xs:complexType>
  <!-- Definizione del classificatore  -->
  <xs:complexType name="Category">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:attribute name="IdCategory" use="required" />
        <xs:attribute name="UniqueId" type="xs:token" />
        <xs:attribute name="DefaultEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione del contenitore  -->
  <xs:complexType name="Container">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:attribute name="IdContainer" use="required" />
        <xs:attribute name="UniqueId" type="xs:token" />
        <xs:attribute name="CreateContainer" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un documento -->
  <xs:complexType name="Document">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:sequence>
          <xs:element name="Instances" type="DocumentInstance" minOccurs="0" maxOccurs="unbounded" />
        </xs:sequence>
        <xs:attribute name="BiblosArchive" type="xs:token" use="required" />
        <xs:attribute name="CreateBiblosArchive" type="xs:boolean" use="required" />
        <xs:attribute name="AllowMultiFile" type="xs:boolean" use="required" />
        <xs:attribute name="Deletable" type="xs:boolean" use="required" />
        <xs:attribute name="UploadEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ScannerEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="DematerialisationEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="SignEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="SignRequired" type="xs:boolean" use="required" />
        <xs:attribute name="CopyProtocol" type="xs:boolean" use="required" />
        <xs:attribute name="CopyResolution" type="xs:boolean" use="required" />
        <xs:attribute name="CopySeries" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione della serie documentale-->
  <xs:complexType name="DocumentInstance">
    <xs:attribute name="IdDocument" />
    <xs:attribute name="DocumentContent" type="xs:token" />
    <xs:attribute name="DocumentName" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione delle serie documentali-->
  <xs:complexType name="DocumentSeries">
    <xs:sequence minOccurs="0">
      <xs:element name="Instances" type="DocumentSeriesInstance" />
    </xs:sequence>
  </xs:complexType>
  <!-- Definizione della serie documentale-->
  <xs:complexType name="DocumentSeriesInstance">
    <xs:attribute name="IdDocumentSeries" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei contatti-->
  <xs:simpleType name="ContactType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="None" />
      <xs:enumeration value="Sender" />
      <xs:enumeration value="Recipient" />
      <xs:enumeration value="AccountAuthorization" />
      <xs:enumeration value="RoleAuthorization" />
    </xs:restriction>
  </xs:simpleType>
  <xs:complexType name="Contacts">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:sequence>
          <!-- Contatto -->
          <xs:element name="ContactInstances" type="ContactInstance" minOccurs="0" maxOccurs="unbounded" />
          <!-- Contatto Manuale-->
          <xs:element name="ContactManualInstances" type="ContactManualInstance" minOccurs="0" maxOccurs="unbounded" />
        </xs:sequence>
        <xs:attribute name="ContactType" type="ContactType" use="optional" />
        <xs:attribute name="ADEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="AddressBookEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ADDistributionListEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ManualEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="ExcelImportEnabled" type="xs:boolean" use="required" />
        <xs:attribute name="AllowMultiContact" type="xs:boolean" use="required" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
        <xs:attribute name="ResultPosition" type="xs:short" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione dei contatti-->
  <xs:complexType name="ContactInstance">
    <xs:attribute name="IdContact" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei contatti manuali-->
  <xs:complexType name="ContactManualInstance">
    <xs:attribute name="ContactDescription" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei Authorizations-->
  <xs:complexType name="Authorizations">
    <xs:complexContent>
      <xs:extension base="DynamicBaseType">
        <xs:sequence>
          <xs:element name="Instances" type="AuthorizationInstance" minOccurs="0" maxOccurs="unbounded" />
        </xs:sequence>
        <xs:attribute name="AllowMultiAuthorization" type="xs:boolean" use="required" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
        <xs:attribute name="ResultPosition" type="xs:short" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione della tipologia di autorizzazione-->
  <xs:simpleType name="AuthorizationType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="Responsible" />
      <xs:enumeration value="Accounted" />
      <xs:enumeration value="Consulted" />
      <xs:enumeration value="Informed" />
    </xs:restriction>
  </xs:simpleType>
  <!-- Definizione della tipologia di autorizzazione-->
  <xs:simpleType name="AuthorizationInstanceType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="Role" />
      <xs:enumeration value="User" />
    </xs:restriction>
  </xs:simpleType>
  <!-- Definizione di un Authorization-->
  <xs:complexType name="AuthorizationInstance">
    <xs:attribute name="IdAuthorization" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <xs:attribute name="Username" type="xs:token" />
    <xs:attribute name="AuthorizationType" type="AuthorizationType" use="required" />
    <xs:attribute name="AuthorizationInstanceType" type="AuthorizationInstanceType" use="required" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei messages-->
  <xs:complexType name="Messages">
    <xs:sequence>
      <xs:element name="Instances" type="MessageInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di un message-->
  <xs:complexType name="MessageInstance">
    <xs:attribute name="IdMessage" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione delle PECMails-->
  <xs:complexType name="PECMails">
    <xs:sequence>
      <xs:element name="Instances" type="PECInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di una pec-->
  <xs:complexType name="PECInstance">
    <xs:attribute name="IdPECMail" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei Protocols-->
  <xs:complexType name="Protocols">
    <xs:sequence>
      <xs:element name="Instances" type="ProtocolInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di un Protocol-->
  <xs:complexType name="ProtocolInstance">
    <xs:attribute name="IdProtocol" type="xs:token" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione dei Resolutions-->
  <xs:complexType name="Resolutions">
    <xs:sequence>
      <xs:element name="Instances" type="ResolutionInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di un Resolution-->
  <xs:complexType name="ResolutionInstance">
    <xs:attribute name="IdResolution" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione delle Collaborations-->
  <xs:complexType name="Collaborations">
    <xs:sequence>
      <xs:element name="Instances" type="CollaborationInstance" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
    <xs:attribute name="Label" type="xs:token" use="required" />
  </xs:complexType>
  <!-- Definizione di una Collaboration-->
  <xs:complexType name="CollaborationInstance">
    <xs:attribute name="IdCollaboration" type="xs:int" use="required" />
    <xs:attribute name="UniqueId" type="xs:token" />
    <!--Descrivere il resto-->
  </xs:complexType>
  <!-- Definizione di un campo testuale -->
  <xs:complexType name="TextField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="Multiline" type="xs:boolean" use="required" />
        <xs:attribute name="IsSignature" type="xs:boolean" use="required" />
        <xs:attribute name="DefaultValue" type="xs:string" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:string" use="optional" />
        <xs:attribute name="Value" type="xs:string" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un campo data -->
  <xs:complexType name="DateField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultValue" type="xs:dateTime" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:dateTime" use="optional" />
        <xs:attribute name="Value" type="xs:dateTime" use="optional" />
        <xs:attribute name="RestrictedYear" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un campo booleano -->
  <xs:complexType name="BoolField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultValue" type="xs:boolean" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:boolean" use="optional" />
        <xs:attribute name="Value" type="xs:boolean" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un campo numerico -->
  <xs:complexType name="NumberField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultValue" type="xs:double" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:double" use="optional" />
        <xs:attribute name="Value" type="xs:double" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione di un campo enum -->
  <xs:complexType name="OptionEnum">
    <xs:sequence>
      <xs:element name="Option" type="xs:token" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
  </xs:complexType>
  <xs:complexType name="EnumField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:sequence>
          <xs:element name="Options" type="OptionEnum" minOccurs="1" maxOccurs="1">
            <!--<xs:unique name="Option_unique">

              <xs:selector xpath="mstns:Option" />

              <xs:field xpath="." />

            </xs:unique>-->
          </xs:element>
        </xs:sequence>
         <xs:attribute name="MultipleValues" type="xs:boolean" />
        <xs:attribute name="DefaultValue" type="xs:token" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:token" use="optional" />
        <xs:attribute name="Value" type="xs:token" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name="LookupField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:attribute name="DefaultSearchValue" type="xs:string" use="optional" />
        <xs:attribute name="Value" type="xs:string" use="optional" />
        <xs:attribute name="MultipleValues" type="xs:boolean" />
        <xs:attribute name="LookupArchiveName" type="xs:token" />
        <xs:attribute name="LookupArchiveColumnName" type="xs:token" />
        <xs:attribute name="LookupValue" type="xs:string" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name="StatusType">
    <xs:simpleContent>
      <xs:extension base="xs:token">
        <xs:attribute name="IconPath" type="xs:token" use="required" />
        <xs:attribute name="MappingTag" type="xs:token" use="optional" />
        <xs:attribute name="TagValue" type="xs:token" use="optional" />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>
  <xs:complexType name="StatusEnum">
    <xs:sequence>
      <xs:element name="State" type="StatusType" minOccurs="1" maxOccurs="unbounded" />
    </xs:sequence>
  </xs:complexType>
  <xs:complexType name="StatusField">
    <xs:complexContent>
      <xs:extension base="FieldBaseType">
        <xs:sequence>
          <xs:element name="Options" type="StatusEnum" minOccurs="1" maxOccurs="1" />
        </xs:sequence>
        <xs:attribute name="DefaultValue" type="xs:token" use="optional" />
        <xs:attribute name="DefaultSearchValue" type="xs:token" use="optional" />
        <xs:attribute name="Value" type="xs:token" use="optional" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- Definizione dei campi "BASE" di un singolo UDS -->
  <xs:complexType name="Section">
    <xs:choice minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Enum" type="EnumField" minOccurs="0" />
      <xs:element name="Text" type="TextField" minOccurs="0" />
      <xs:element name="Date" type="DateField" minOccurs="0" />
      <xs:element name="Number" type="NumberField" minOccurs="0" />
      <xs:element name="Bool" type="BoolField" minOccurs="0" />
      <xs:element name="Lookup" type="LookupField" minOccurs="0" />
      <xs:element name="Status" type="StatusField" minOccurs="0" />
    </xs:choice>
    <xs:attribute name="ColNumber" type="xs:unsignedInt" use="optional" />
    <xs:attribute name="RowNumber" type="xs:unsignedInt" use="optional" />
    <xs:attribute name="CSS" type="xs:token" use="optional" />
    <xs:attribute name="SectionLabel" type="xs:token" use="required" />
    <xs:attribute name="SectionId" type="xs:token" use="required" />
  </xs:complexType>
  <xs:complexType name="Metadata">
    <xs:sequence minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Sections" type="Section" minOccurs="1" maxOccurs="unbounded">
        <xs:unique name="Section_LabelUnique">
          <xs:selector xpath="./*" />
          <xs:field xpath="@Label" />
        </xs:unique>
        <xs:unique name="Section_ClientUnique">
          <xs:selector xpath="./*" />
          <xs:field xpath="@ClientId" />
        </xs:unique>
        <xs:unique name="Section_ColumnNameUnique">
          <xs:selector xpath="./*" />
          <xs:field xpath="@ColumnName" />
        </xs:unique>
      </xs:element>
    </xs:sequence>
  </xs:complexType>
  <!-- Definizione dei campi "RELAZIONE" di un singolo UDS verso entità esterne -->
  <xs:complexType name="Relations">
    <xs:choice minOccurs="1" maxOccurs="unbounded">
      <xs:element name="Contacts" type="Contacts" minOccurs="0" />
      <xs:element name="Messages" type="Messages" minOccurs="0" />
      <xs:element name="PECMails" type="PECMails" minOccurs="0" />
      <xs:element name="Protocols" type="Protocols" minOccurs="0" />
      <xs:element name="Resolutions" type="Resolutions" minOccurs="0" />
      <xs:element name="Collaborations" type="Collaborations" minOccurs="0" />
    </xs:choice>
    <xs:attribute name="Label" type="xs:token" use="required" />
    <xs:attribute name="ReadOnly" type="xs:boolean" use="required" />
  </xs:complexType>
  <!-- Definizione della sezione Javascript -->
  <xs:complexType name="JavaScriptSpecification">
    <xs:sequence>
      <xs:element name="FunctionName" type="xs:token" minOccurs="1" maxOccurs="1" />
      <xs:element name="EventType" type="JSEventType" minOccurs="1" maxOccurs="1" />
    </xs:sequence>
  </xs:complexType>
  <xs:complexType name="JavaScript">
    <xs:sequence>
      <xs:element name="Validation" type="JavaScriptSpecification" minOccurs="0" maxOccurs="1" />
      <xs:element name="Action" type="JavaScriptSpecification" minOccurs="0" maxOccurs="1" />
    </xs:sequence>
  </xs:complexType>
  <xs:simpleType name="JSEventType">
    <xs:restriction base="xs:token">
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
      <xs:element name="PanelId" type="xs:token" minOccurs="0" maxOccurs="1" />
      <xs:element name="ColNumber" type="xs:unsignedInt" minOccurs="1" maxOccurs="1" />
      <xs:element name="RowNumber" type="xs:unsignedInt" minOccurs="1" maxOccurs="1" />
      <xs:element name="CSS" type="xs:token" minOccurs="0" maxOccurs="1" />
    </xs:sequence>
  </xs:complexType>
  <xs:simpleType name="LayoutType">
    <xs:restriction base="xs:token">
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
  <xs:simpleType name="TitleType">
    <xs:restriction base="xs:token">
      <xs:minLength value="3" />
      <xs:maxLength value="55" />
    </xs:restriction>
  </xs:simpleType>
  <xs:simpleType name="AliasType">
    <xs:restriction base="xs:token">
      <xs:minLength value="2" />
      <xs:maxLength value="4" />
    </xs:restriction>
  </xs:simpleType>
  <xs:complexType name="SubjectType">
    <xs:complexContent>
      <xs:extension base="BaseType">
        <xs:attribute name="DefaultValue" type="xs:string" use="optional" />
        <xs:attribute name="Value" type="xs:string" use="optional" />
        <xs:attribute name="ResultVisibility" type="xs:boolean" use="required" />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:simpleType name="ProtocolDirectionType">
    <xs:restriction base="xs:token">
      <xs:enumeration value="None" />
      <xs:enumeration value="In" />
      <xs:enumeration value="Out" />
      <xs:enumeration value="InternalOffice" />
    </xs:restriction>
  </xs:simpleType>
  <xs:element name="UnitaDocumentariaSpecifica">
    <xs:complexType>
      <xs:sequence minOccurs="1" maxOccurs="1">
        <xs:element name="Title" type="TitleType" minOccurs="1" maxOccurs="1" nillable="false" />
        <xs:element name="Alias" type="AliasType" minOccurs="1" maxOccurs="1" nillable="false" />
        <xs:element name="Subject" type="SubjectType" minOccurs="1" maxOccurs="1" />
        <xs:element name="Category" type="Category" minOccurs="1" maxOccurs="1" />
        <xs:element name="Container" type="Container" minOccurs="1" maxOccurs="1" />
        <xs:element name="Documents" type="Documents" minOccurs="1" maxOccurs="1" nillable="false" />
        <xs:element name="Authorizations" type="Authorizations" minOccurs="0" maxOccurs="1" />
        <xs:element name="Contacts" type="Contacts" minOccurs="0" maxOccurs="unbounded" />
        <xs:element name="Messages" type="Messages" minOccurs="0" maxOccurs="1" />
        <xs:element name="PECMails" type="PECMails" minOccurs="0" maxOccurs="1" />
        <xs:element name="Protocols" type="Protocols" minOccurs="0" maxOccurs="1" />
        <xs:element name="Resolutions" type="Resolutions" minOccurs="0" maxOccurs="1" />
        <xs:element name="Collaborations" type="Collaborations" minOccurs="0" maxOccurs="1" />
        <xs:element name="Metadata" type="Metadata" minOccurs="1" maxOccurs="1" />
        <xs:element name="CustomJavaScript" type="xs:string" minOccurs="0" maxOccurs="1" />
        <xs:element name="CustomCSS" type="xs:string" minOccurs="0" maxOccurs="1" />
      </xs:sequence>
      <xs:attribute name="WorkflowEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="DocumentUnitSynchronizeEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="DocumentUnitSynchronizeTitle" type="xs:token" use="optional" default="{Year}/{Number:0000000}" />
      <xs:attribute name="ProtocolEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="PECEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="PECButtonEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="MailButtonEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="MailRoleButtonEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="CancelMotivationRequired" type="xs:boolean" use="required" />
      <xs:attribute name="IncrementalIdentityEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="SignatureMetadataEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="Layout" type="LayoutType" use="required" />
      <xs:attribute name="ProtocolDirection" type="ProtocolDirectionType" use="optional" />
      <xs:attribute name="ConservationEnabled" type="xs:boolean" use="required" />
      <xs:attribute name="HideRegistrationIdentifier" type="xs:boolean" use="required" />
      <xs:attribute name="UDSId" type="xs:token" use="optional" />
      <xs:attribute name="RequiredRevisionUDSRepository" type="xs:boolean" use="required" />
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
PRINT N'INSERT INTO [dbo].[CategoryFascicleRights]'
GO

INSERT INTO [dbo].[CategoryFascicleRights]
           ([IdCategoryFascicleRight]
           ,[IdCategoryFascicle]
           ,[IdRole]
		   ,[RegistrationDate]
		   ,[RegistrationUser])
SELECT newid(),
			CF.IdCategoryFascicle, 
			RG.idRole,
			GETUTCDATE(),
			'SYSTEM'
FROM CategoryFascicles CF 
INNER JOIN CategoryGroup CG on CF.IdCategory = CG.idCategory and CF.DSWEnvironment = 0
INNER JOIN RoleGroup RG on cg.idGroup = RG.idGroup
INNER JOIN [Role] R on RG.idRole = RG.idRole
WHERE R.isActive = 1
GROUP BY CF.IdCategoryFascicle, RG.idRole
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO

--#############################################################################
PRINT N'CREATE UNIQUE INDEX [IX_CategoryFascicles_IdCategory_FascicleType_DSWEnvironment] '
GO
CREATE UNIQUE INDEX [IX_CategoryFascicles_IdCategory_FascicleType_DSWEnvironment] 
	ON [dbo].[CategoryFascicles] ([IdCategory], [FascicleType], [DSWEnvironment])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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
PRINT N'Aggiungere la colonna Status nella tabella ProtocolRoleUser'
GO
ALTER TABLE [dbo].[ProtocolRoleUser]
ADD [Status] [smallint] NOT NULL DEFAULT(0)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
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

